using Catchen.Commerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Commerce.Services;

public sealed record CheckoutResult(
    Guid? OrderId,
    string? CheckoutUrl,
    string? Violation)
{
    public bool Succeeded => OrderId is not null;

    public static CheckoutResult Ok(Guid orderId, string checkoutUrl)
    {
        return new(orderId, checkoutUrl, null);
    }

    public static CheckoutResult Rejected(string violation)
    {
        return new(null, null, violation);
    }
}

/// <summary>
/// Provider-neutral checkout (task 3.1/3.2): creates a Pending order and a
/// provider checkout session. Entitlements change ONLY through verified
/// webhook events — never synchronously at checkout time.
/// </summary>
public interface ICheckoutService
{
    Task<CheckoutResult> StartMembershipCheckoutAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<CheckoutResult> StartRecipePurchaseAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default);

    /// <summary>Reconciled order history for the user (task 3.4).</summary>
    Task<IReadOnlyList<Order>> MyOrdersAsync(Guid userId, CancellationToken cancellationToken = default);
}

/// <summary>Static catalog of Phase-1 products (single currency, offshore).</summary>
public static class CommerceCatalog
{
    public const string Currency = "USD";
    public const long MembershipPriceMinorUnits = 499; // $4.99 / month
    public const long SingleRecipePriceMinorUnits = 199; // $1.99 each
    public const string Provider = "stripe";
}

public sealed class CheckoutService(DbContext db, TimeProvider clock) : ICheckoutService
{
    public async Task<CheckoutResult> StartMembershipCheckoutAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        return await CreateOrderAsync(userId, ProductKind.MonthlyMembership, null, cancellationToken);
    }

    public async Task<CheckoutResult> StartRecipePurchaseAsync(
        Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        // Only live published recipes can be purchased.
        var recipeLive = await db.Set<Catalog.Models.PublishedRecipe>().AnyAsync(
            r => r.RecipeId == recipeId && r.IsLive && !r.IsFree, cancellationToken);
        if (!recipeLive)
        {
            return CheckoutResult.Rejected("recipe_not_purchasable");
        }

        return await CreateOrderAsync(userId, ProductKind.SingleRecipe, recipeId, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> MyOrdersAsync(
        Guid userId, CancellationToken cancellationToken = default)
    {
        var list = await db.Set<Order>()
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return list;
    }

    private async Task<CheckoutResult> CreateOrderAsync(
        Guid userId, ProductKind kind, Guid? recipeId, CancellationToken ct)
    {
        var amount = kind == ProductKind.MonthlyMembership
            ? CommerceCatalog.MembershipPriceMinorUnits
            : CommerceCatalog.SingleRecipePriceMinorUnits;

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductKind = kind,
            RecipeId = recipeId,
            AmountMinorUnits = amount,
            Currency = CommerceCatalog.Currency,
            Status = OrderStatus.Pending,
            Provider = CommerceCatalog.Provider,
            // Provider-neutral session reference; the real gateway adapter
            // replaces this synthetic value when provider keys are configured.
            ProviderReference = $"cs_test_{orderNonce()}",
            CreatedAtUtc = clock.GetUtcNow(),
        };

        db.Set<Order>().Add(order);
        await db.SaveChangesAsync(ct);

        return new CheckoutResult(order.Id, $"/checkout/{order.ProviderReference}", null);
    }

    private static string orderNonce()
    {
        return Guid.NewGuid().ToString("N")[..24];
    }
}
