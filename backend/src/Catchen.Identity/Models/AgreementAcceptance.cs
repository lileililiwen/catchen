namespace Catchen.Identity.Models;

/// <summary>
/// Evidence that a person accepted a specific agreement version at a specific
/// time. Privacy-minimized: raw IP addresses are stored only as salted-free
/// SHA-256 digests, user agents are truncated.
/// </summary>
public sealed class AgreementAcceptance
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    /// <summary>Version of the agreement text accepted (e.g. "2026-08-consumer").</summary>
    public required string AgreementVersion { get; set; }

    public DateTimeOffset AcceptedAtUtc { get; set; }

    /// <summary>SHA-256 hex digest of the client IP at acceptance time.</summary>
    public required string ClientIpHash { get; set; }

    /// <summary>Truncated (≤128 chars) User-Agent header at acceptance time.</summary>
    public required string ClientUserAgent { get; set; }
}
