namespace Catchen.Commerce.Models;

/// <summary>
/// Signed provider webhook inbox (spec: signed webhook inboxes provide
/// idempotency and replay protection). Events are recorded raw before any
/// processing decision; EventId is the provider's unique event identifier.
/// </summary>
public sealed class WebhookEvent
{
    public Guid Id { get; set; }

    /// <summary>Provider event id — unique key for exactly-once processing.</summary>
    public required string Provider { get; set; }

    public required string EventId { get; set; }

    /// <summary>Provider event type, e.g. "checkout.session.completed".</summary>
    public required string EventType { get; set; }

    public required string PayloadJson { get; set; }

    public DateTimeOffset ReceivedAtUtc { get; set; }

    /// <summary>True once the event has been applied to the ledger.</summary>
    public bool Processed { get; set; }

    /// <summary>Duplicate delivery, forged signature, or unknown type.</summary>
    public string? SkipReason { get; set; }
}
