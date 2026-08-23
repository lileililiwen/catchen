namespace Catchen.Identity.Models;

/// <summary>
/// An overseas campaign channel approved by an administrator for promotion or
/// distribution. Domestic channels (Xiaohongshu, Douyin, domestic WeChat
/// groups) can never be recorded here — the service rejects them outright.
/// </summary>
public sealed class ApprovedChannel
{
    public Guid Id { get; set; }

    /// <summary>Canonical channel slug, e.g. "google_ads", "instagram".</summary>
    public required string Channel { get; set; }

    /// <summary>"promotion" or "distribution".</summary>
    public required string Kind { get; set; }

    public required Guid ApprovedByUserId { get; set; }

    public DateTimeOffset ApprovedAtUtc { get; set; }
}
