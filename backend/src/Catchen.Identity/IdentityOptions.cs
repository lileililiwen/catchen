namespace Catchen.Identity;

/// <summary>Configuration for identity, JWT issuance and region policy.</summary>
public sealed class IdentityOptions
{
    public const string SectionName = "Identity";

    /// <summary>
    /// JWT signing secret. MUST be at least 32 characters; deployments fail
    /// fast at startup when it is missing or too short.
    /// </summary>
    public string JwtSecret { get; set; } = string.Empty;

    public string JwtIssuer { get; set; } = "catchen";

    public string JwtAudience { get; set; } = "catchen-clients";

    public int JwtLifetimeMinutes { get; set; } = 120;

    /// <summary>Agreement version a registrant must accept (e.g. "2026-08-consumer").</summary>
    public string RequiredAgreementVersion { get; set; } = "2026-08-consumer";

    /// <summary>
    /// Telephone country codes that can never register or authenticate,
    /// matched as E.164 prefixes. Default: Mainland China (+86).
    /// NOTE: no array initializer — ConfigurationBinder appends to
    /// pre-populated collections; defaults are applied in PostConfigure.
    /// </summary>
    public string[] BlockedPhoneCountryCodes { get; set; } = [];

    /// <summary>ISO-3166 alpha-2 declared countries refused at registration.</summary>
    public string[] BlockedDeclaredCountries { get; set; } = [];

    /// <summary>Audit events older than this many days are purged by the retention worker.</summary>
    public int AuditRetentionDays { get; set; } = 365;
}

/// <summary>Configuration for payment and campaign channel policies (task 1.4).</summary>
public sealed class ChannelPolicyOptions
{
    public const string SectionName = "ChannelPolicy";

    /// <summary>Payment methods clients may be offered. Domestic rails are absent by design.</summary>
    public string[] AllowedPaymentMethods { get; set; } = [];

    /// <summary>Campaign channels that can never be approved — domestic promotion surfaces.</summary>
    public string[] ProhibitedPromotionChannels { get; set; } = [];

    /// <summary>Distribution channels that can never be approved — domestic app distribution.</summary>
    public string[] ProhibitedDistributionChannels { get; set; } = [];
}

public static class PolicyDefaults
{
    public static readonly string[] AllowedPaymentMethods = ["stripe", "paypal", "apple_pay", "google_pay"];
    public static readonly string[] ProhibitedPromotionChannels = ["xiaohongshu", "douyin", "wechat_domestic_groups"];
    public static readonly string[] ProhibitedDistributionChannels = ["domestic_apk_stores"];
    public static readonly string[] BlockedPhoneCountryCodes = ["+86"];
    public static readonly string[] BlockedDeclaredCountries = ["CN"];
}
