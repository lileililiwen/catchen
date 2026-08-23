using System.Text.Json;
using Catchen.Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Commerce.Services;

/// <summary>
/// Static holder for the webhook signing secret, set at composition-root
/// time from configuration. Kept out of DI-scoped services so the verifier
/// and inbox share one source of truth without leaking it into logs.
/// </summary>
public static class WebhookSecrets
{
    public static string Current { get; set; } = string.Empty;
}

/// <summary>
/// Applies verified provider events to the provider-neutral ledger:
/// grants/extends entitlements exactly once and transitions order states.
/// </summary>
public interface IEntitlementLedger
{
    /// <summary>Returns true when the event type was recognized and applied.</summary>
    Task<bool> ApplyEventAsync(string eventType, JsonElement payload, CancellationToken cancellationToken = default);

    Task<bool> HasFullAccessAsync(Guid userId, Guid? recipeId, CancellationToken cancellationToken = default);
}

public sealed class EntitlementLedger(DbContext db, TimeProvider clock) : IEntitlementLedger
{
    // Provider event types this deployment understands (Stripe-shaped).
    private const string CheckoutCompleted = "checkout.session.completed";
    private const string ChargeRefunded = "charge.refunded";
    private const string ChargeDisputed = "charge.dispute.created";

    public async Task<bool> ApplyEventAsync(string eventType, JsonElement payload, CancellationToken cancellationToken = default)
    {
        switch (eventType)
        {
            case CheckoutCompleted:
                await ApplyCheckoutCompletedAsync(payload, cancellationToken);
                return true;
            case ChargeRefunded:
                await ApplyRefundAsync(payload, cancellationToken);
                return true;
            case ChargeDisputed:
                await MarkDisputedAsync(payload, cancellationToken);
                return true;
            default:
                return false;
        }
    }

    public async Task<bool> HasFullAccessAsync(Guid userId, Guid? recipeId, CancellationToken cancellationToken = default)
    {
        var now = clock.GetUtcNow();

        // DateTimeOffset comparisons are evaluated in memory: SQLite cannot
        // translate them, and Phase 1 row counts are small.
        var memberships = await db.Set<Entitlement>()
            .AsNoTracking()
            .Where(e => e.UserId == userId
                && e.Kind == ProductKind.MonthlyMembership
                && e.RevokedAtUtc == null)
            .ToListAsync(cancellationToken);
        if (memberships.Any(e => e.PeriodEndUtc is { } end && end > now))
        {
            return true;
        }

        if (recipeId is Guid recipe)
        {
            return await db.Set<Entitlement>().AnyAsync(
                e => e.UserId == userId
                    && e.Kind == ProductKind.SingleRecipe
                    && e.RecipeId == recipe
                    && e.RevokedAtUtc == null,
                cancellationToken);
        }

        return false;
    }

    private async Task ApplyCheckoutCompletedAsync(JsonElement payload, CancellationToken ct)
    {
        var session = payload.GetProperty("data").GetProperty("object");
        var providerReference = session.TryGetProperty("id", out var idEl) ? idEl.GetString() : null;
        if (string.IsNullOrWhiteSpace(providerReference))
        {
            return;
        }

        var order = await db.Set<Order>().SingleOrDefaultAsync(
            o => o.ProviderReference == providerReference, ct);
        if (order is null || order.Status != OrderStatus.Pending)
        {
            return; // unknown reference or already transitioned — idempotent no-op
        }

        order.Status = OrderStatus.Paid;
        order.PaidAtUtc = clock.GetUtcNow();

        // Exactly-once grant: unique index on Entitlement.OrderId guards even
        // against concurrent replays.
        var alreadyGranted = await db.Set<Entitlement>().AnyAsync(e => e.OrderId == order.Id, ct);
        if (!alreadyGranted)
        {
            db.Set<Entitlement>().Add(new Entitlement
            {
                Id = Guid.NewGuid(),
                UserId = order.UserId,
                Kind = order.ProductKind,
                RecipeId = order.RecipeId,
                OrderId = order.Id,
                PeriodEndUtc = order.ProductKind == ProductKind.MonthlyMembership
                    ? clock.GetUtcNow().AddDays(30)
                    : null,
                CreatedAtUtc = clock.GetUtcNow(),
            });
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task ApplyRefundAsync(JsonElement payload, CancellationToken ct)
    {
        var charge = payload.GetProperty("data").GetProperty("object");
        var providerReference = charge.TryGetProperty("payment_intent", out var intentEl)
            ? intentEl.GetString()
            : null;
        if (string.IsNullOrWhiteSpace(providerReference))
        {
            return;
        }

        var order = await db.Set<Order>().SingleOrDefaultAsync(
            o => o.ProviderReference == providerReference, ct);
        if (order is null || order.Status is not (OrderStatus.Paid or OrderStatus.Disputed))
        {
            return;
        }

        order.Status = OrderStatus.Refunded;

        // Refund policy: revoke the entitlement the refunded order granted.
        var entitlements = await db.Set<Entitlement>()
            .Where(e => e.OrderId == order.Id && e.RevokedAtUtc == null)
            .ToListAsync(ct);
        foreach (var entitlement in entitlements)
        {
            entitlement.RevokedAtUtc = clock.GetUtcNow();
        }

        await db.SaveChangesAsync(ct);
    }

    private async Task MarkDisputedAsync(JsonElement payload, CancellationToken ct)
    {
        var dispute = payload.GetProperty("data").GetProperty("object");
        var providerReference = dispute.TryGetProperty("payment_intent", out var intentEl)
            ? intentEl.GetString()
            : null;
        if (string.IsNullOrWhiteSpace(providerReference))
        {
            return;
        }

        var order = await db.Set<Order>().SingleOrDefaultAsync(
            o => o.ProviderReference == providerReference, ct);
        if (order is null || order.Status == OrderStatus.Disputed)
        {
            return;
        }

        order.Status = OrderStatus.Disputed;
        await db.SaveChangesAsync(ct);
    }
}
