using System.Text;
using System.Text.Json;
using Catchen.Commerce.Models;
using Catchen.Identity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catchen.Commerce.Services;

public sealed record WebhookResult(bool Accepted, string? SkipReason)
{
    public bool Forged => SkipReason is "invalid_signature" or "replay_outside_tolerance";
}

/// <summary>
/// Signed webhook inbox (task 3.1): raw events are stored first (unique on
/// provider+eventId for idempotency), then applied to the ledger exactly
/// once. Forged or replayed callbacks make NO entitlement change and are
/// recorded as security events.
/// </summary>
public interface IWebhookInboxService
{
    Task<WebhookResult> IngestAsync(
        string provider, string body, string signatureHeader,
        CancellationToken cancellationToken = default);
}

public sealed class WebhookInboxService(
    DbContext db,
    IWebhookSignatureVerifier signatureVerifier,
    IEntitlementLedger ledger,
    IAuditWriter audit,
    TimeProvider clock,
    ILogger<WebhookInboxService> logger) : IWebhookInboxService
{
    private const string ProviderName = "stripe";

    public async Task<WebhookResult> IngestAsync(
        string provider, string body, string signatureHeader,
        CancellationToken cancellationToken = default)
    {
        var secret = WebhookSecrets.Current;
        if (!signatureVerifier.Verify(secret, body, signatureHeader, clock.GetUtcNow()))
        {
            await audit.WriteAsync("commerce", "webhook.forged", null,
                "WebhookEvent", Digest(body),
                new { provider }, cancellationToken);
            return new WebhookResult(false, "invalid_signature");
        }

        JsonDocument document;
        try
        {
            document = JsonDocument.Parse(body);
        }
        catch (JsonException)
        {
            return new WebhookResult(false, "unparseable_payload");
        }

        var eventId = document.RootElement.TryGetProperty("id", out var idEl)
            ? idEl.GetString()
            : null;
        var eventType = document.RootElement.TryGetProperty("type", out var typeEl)
            ? typeEl.GetString()
            : null;

        if (string.IsNullOrWhiteSpace(eventId) || string.IsNullOrWhiteSpace(eventType))
        {
            return new WebhookResult(false, "missing_event_fields");
        }

        // Idempotency: unique (provider, eventId). A duplicate delivery is
        // recorded as skipped and never re-applied.
        var duplicate = await db.Set<WebhookEvent>().AnyAsync(
            w => w.Provider == ProviderName && w.EventId == eventId, cancellationToken);
        if (duplicate)
        {
            logger.LogDuplicateEvent(eventId);
            return new WebhookResult(false, "duplicate_event");
        }

        var webhook = new WebhookEvent
        {
            Id = Guid.NewGuid(),
            Provider = ProviderName,
            EventId = eventId,
            EventType = eventType,
            PayloadJson = body,
            ReceivedAtUtc = clock.GetUtcNow(),
        };
        db.Set<WebhookEvent>().Add(webhook);
        await db.SaveChangesAsync(cancellationToken);

        var applied = await ledger.ApplyEventAsync(eventType, document.RootElement, cancellationToken);
        if (applied)
        {
            webhook.Processed = true;
            await db.SaveChangesAsync(cancellationToken);
            return new WebhookResult(true, null);
        }

        webhook.SkipReason = "unknown_event_type";
        await db.SaveChangesAsync(cancellationToken);
        return new WebhookResult(false, "unknown_event_type");
    }

    private static string Digest(string value)
    {
        return Convert.ToHexString(
            System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(value)))
            .ToLowerInvariant();
    }
}

internal static partial class WebhookLogging
{
    [LoggerMessage(Level = LogLevel.Information,
        Message = "Duplicate provider event {EventId} ignored (idempotent replay protection)")]
    public static partial void LogDuplicateEvent(this ILogger logger, string eventId);
}
