using System.Security.Cryptography;
using System.Text;
using Catchen.Affiliates.Models;
using Catchen.Identity.Models;
using Catchen.Identity.Services;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Affiliates.Services;

public sealed record RedirectResult(string? Destination, string? Violation)
{
    public bool Allowed => Destination is not null;
}

/// <summary>
/// Affiliate link attribution (task 4.1): clicks route ONLY to allowlisted
/// overseas merchants with a clear attribution tag; non-allowlisted
/// destinations are blocked before any redirect or click record.
/// </summary>
public interface IAffiliateLinkService
{
    Task<RedirectResult> ResolveRedirectAsync(
        string slug, string? campaignId, Guid? userId, CancellationToken cancellationToken = default);

    Task<RedirectResult> RegisterMerchantAsync(
        string slug, string displayName, string baseUrl, string attributionTag,
        Guid actorUserId, string actorRole, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AffiliateMerchant>> ListMerchantsAsync(CancellationToken cancellationToken = default);
}

public sealed class AffiliateLinkService(DbContext db, IAuditWriter audit, TimeProvider clock)
    : IAffiliateLinkService
{
    // Domestic surfaces can never be registered as affiliate destinations.
    private static readonly string[] _prohibitedSlugFragments =
    [
        "xiaohongshu", "douyin", "taobao", "jd.com", "pinduoduo", "wechat",
    ];

    public async Task<RedirectResult> ResolveRedirectAsync(
        string slug, string? campaignId, Guid? userId, CancellationToken cancellationToken = default)
    {
        var normalized = slug?.Trim().ToLowerInvariant() ?? string.Empty;
        var merchant = await db.Set<AffiliateMerchant>().AsNoTracking()
            .SingleOrDefaultAsync(m => m.Slug == normalized, cancellationToken);

        if (merchant is null)
        {
            return new RedirectResult(null, "merchant_not_allowlisted");
        }

        db.Set<AffiliateClick>().Add(new AffiliateClick
        {
            MerchantSlug = merchant.Slug,
            CampaignId = string.IsNullOrWhiteSpace(campaignId) ? null : campaignId.Trim(),
            VisitorPseudonym = Pseudonym(userId, clock.GetUtcNow()),
            ClickedAtUtc = clock.GetUtcNow(),
        });
        await db.SaveChangesAsync(cancellationToken);

        var separator = merchant.BaseUrl.Contains('?') ? '&' : '?';
        var destination = $"{merchant.BaseUrl}{separator}tag={merchant.AttributionTag}"
            + (string.IsNullOrWhiteSpace(campaignId) ? string.Empty : $"&cid={Uri.EscapeDataString(campaignId)}");

        return new RedirectResult(destination, null);
    }

    public async Task<RedirectResult> RegisterMerchantAsync(
        string slug, string displayName, string baseUrl, string attributionTag,
        Guid actorUserId, string actorRole, CancellationToken cancellationToken = default)
    {
        if (actorRole != AppUserRoles.Administrator)
        {
            return new RedirectResult(null, "forbidden_role");
        }

        var normalized = slug?.Trim().ToLowerInvariant() ?? string.Empty;
        if (normalized.Length is < 2 or > 64 || !Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri)
            || uri.Scheme != Uri.UriSchemeHttps)
        {
            return new RedirectResult(null, "merchant_invalid");
        }

        var prohibited = _prohibitedSlugFragments.FirstOrDefault(
            fragment => normalized.Contains(fragment, StringComparison.Ordinal));
        if (prohibited is not null)
        {
            await audit.WriteAsync("affiliates", "merchant.policy_violation", actorUserId,
                "AffiliateMerchant", normalized, new { }, cancellationToken);
            return new RedirectResult(null, "merchant_prohibited");
        }

        var exists = await db.Set<AffiliateMerchant>().AnyAsync(m => m.Slug == normalized, cancellationToken);
        if (exists)
        {
            return new RedirectResult(null, "already_registered");
        }

        db.Set<AffiliateMerchant>().Add(new AffiliateMerchant
        {
            Id = Guid.NewGuid(),
            Slug = normalized,
            DisplayName = displayName.Trim(),
            BaseUrl = baseUrl.Trim(),
            AttributionTag = attributionTag.Trim(),
            RegisteredAtUtc = clock.GetUtcNow(),
        });
        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync("affiliates", "merchant.registered", actorUserId,
            "AffiliateMerchant", normalized, new { baseUrl = uri.ToString() }, cancellationToken);

        return new RedirectResult($"/go/{normalized}", null);
    }

    public async Task<IReadOnlyList<AffiliateMerchant>> ListMerchantsAsync(
        CancellationToken cancellationToken = default)
    {
        return await db.Set<AffiliateMerchant>().AsNoTracking()
            .OrderBy(m => m.Slug)
            .ToListAsync(cancellationToken);
    }

    /// <summary>Salted daily pseudonym: stable per user per UTC day, never reversible.</summary>
    private static string Pseudonym(Guid? userId, DateTimeOffset day)
    {
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
            $"{userId?.ToString() ?? "anonymous"}|{day:yyyy-MM-dd}|catchen-click"))).ToLowerInvariant();
    }
}
