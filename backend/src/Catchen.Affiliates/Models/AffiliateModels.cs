namespace Catchen.Affiliates.Models;

/// <summary>
/// An allowlisted overseas merchant reachable through disclosed affiliate
/// links. Domestic merchants can never be registered (guard in service).
/// </summary>
public sealed class AffiliateMerchant
{
    public Guid Id { get; set; }

    /// <summary>URL-safe slug used in /go/{slug} redirects.</summary>
    public required string Slug { get; set; }

    public required string DisplayName { get; set; }

    /// <summary>HTTPS base URL clicks are redirected to.</summary>
    public required string BaseUrl { get; set; }

    /// <summary>Disclosure tag appended to outbound URLs (e.g. "catchen-21").</summary>
    public required string AttributionTag { get; set; }

    public DateTimeOffset RegisteredAtUtc { get; set; }
}

/// <summary>
/// Privacy-minimized click attribution: no IP addresses, no user agents —
/// only the merchant, campaign and a salted daily pseudonym. A click is
/// NEVER treated as evidence of a sale.
/// </summary>
public sealed class AffiliateClick
{
    public long Id { get; set; }

    public required string MerchantSlug { get; set; }

    /// <summary>Optional campaign identifier supplied by the link.</summary>
    public string? CampaignId { get; set; }

    /// <summary>Salted daily pseudonym (hash of userId|utcDate|salt) or anonymous marker.</summary>
    public required string VisitorPseudonym { get; set; }

    public DateTimeOffset ClickedAtUtc { get; set; }
}

public enum CommissionRowStatus
{
    Accepted = 0,
    Duplicate = 1,
    Rejected = 2,
}

/// <summary>One imported row from a provider commission statement.</summary>
public sealed class CommissionStatementRow
{
    public Guid Id { get; set; }

    public required string Provider { get; set; }

    /// <summary>Provider's unique row identifier for deduplication.</summary>
    public required string ExternalRowId { get; set; }

    public required string MerchantSlug { get; set; }

    public required long AmountMinorUnits { get; set; }

    public required string Currency { get; set; }

    public CommissionRowStatus Status { get; set; }

    public string? RejectReason { get; set; }

    public DateTimeOffset ImportedAtUtc { get; set; }
}
