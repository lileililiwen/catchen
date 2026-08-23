namespace Catchen.Identity.Models;

/// <summary>
/// Append-only compliance trail (agreement evidence, region-policy denials,
/// channel approvals). Privacy-minimized: payloads carry reason codes and
/// digests, never raw personal data or secrets.
/// </summary>
public sealed class AuditEvent
{
    public long Id { get; set; }

    public DateTimeOffset OccurredAtUtc { get; set; }

    /// <summary>Coarse area, e.g. "identity", "region-policy", "channels".</summary>
    public required string Category { get; set; }

    /// <summary>Machine-readable action, e.g. "registration.rejected".</summary>
    public required string Action { get; set; }

    public Guid? ActorUserId { get; set; }

    /// <summary>Entity kind the event is about, e.g. "AppUser".</summary>
    public required string SubjectType { get; set; }

    /// <summary>Identifier of the subject (may be a digest for anonymous subjects).</summary>
    public required string SubjectId { get; set; }

    /// <summary>JSON payload with reason codes and non-sensitive context.</summary>
    public required string PayloadJson { get; set; }
}
