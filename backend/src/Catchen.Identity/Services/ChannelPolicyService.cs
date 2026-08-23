using Catchen.Identity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Catchen.Identity.Services;

public sealed record ChannelApprovalResult(Guid? ApprovalId, string? Violation)
{
    public bool Succeeded => ApprovalId is not null;

    public static ChannelApprovalResult Ok(Guid id)
    {
        return new(id, null);
    }

    public static ChannelApprovalResult Rejected(string violation)
    {
        return new(null, violation);
    }
}

/// <summary>
/// Payment and campaign channel policy (task 1.4). Domestic payment rails are
/// never offered; domestic promotion/distribution channels can never be
/// recorded as approved — attempts are rejected AND audited.
/// </summary>
public interface IChannelPolicyService
{
    IReadOnlyList<string> AllowedPaymentMethods();

    Task<ChannelApprovalResult> ApproveChannelAsync(
        string channel, string kind, Guid approverUserId, string approverRole,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ApprovedChannel>> ListApprovedAsync(string? kind = null, CancellationToken cancellationToken = default);
}

public sealed class ChannelPolicyService(
    DbContext db,
    IOptions<ChannelPolicyOptions> options,
    IAuditWriter audit,
    TimeProvider clock) : IChannelPolicyService
{
    private const string PromotionKind = "promotion";
    private const string DistributionKind = "distribution";

    public IReadOnlyList<string> AllowedPaymentMethods()
    {
        return options.Value.AllowedPaymentMethods;
    }

    public async Task<ChannelApprovalResult> ApproveChannelAsync(
        string channel, string kind, Guid approverUserId, string approverRole,
        CancellationToken cancellationToken = default)
    {
        var slug = channel?.Trim().ToLowerInvariant() ?? string.Empty;
        var normalizedKind = kind?.Trim().ToLowerInvariant() ?? string.Empty;

        if (approverRole != AppUserRoles.Administrator)
        {
            return ChannelApprovalResult.Rejected("forbidden_role");
        }

        if (normalizedKind is not (PromotionKind or DistributionKind))
        {
            return ChannelApprovalResult.Rejected("kind_invalid");
        }

        var prohibited = normalizedKind == PromotionKind
            ? options.Value.ProhibitedPromotionChannels
            : options.Value.ProhibitedDistributionChannels;

        if (prohibited.Contains(slug, StringComparer.Ordinal))
        {
            await audit.WriteAsync(
                "channels", "approval.policy_violation", approverUserId,
                "ApprovedChannel", slug,
                new { kind = normalizedKind },
                cancellationToken);

            return ChannelApprovalResult.Rejected("channel_prohibited");
        }

        var exists = await db.Set<ApprovedChannel>().AnyAsync(
            c => c.Channel == slug && c.Kind == normalizedKind, cancellationToken);
        if (exists)
        {
            return ChannelApprovalResult.Rejected("already_approved");
        }

        var approval = new ApprovedChannel
        {
            Id = Guid.NewGuid(),
            Channel = slug,
            Kind = normalizedKind,
            ApprovedByUserId = approverUserId,
            ApprovedAtUtc = clock.GetUtcNow(),
        };

        db.Set<ApprovedChannel>().Add(approval);
        await db.SaveChangesAsync(cancellationToken);

        await audit.WriteAsync(
            "channels", "approval.granted", approverUserId,
            "ApprovedChannel", approval.Id.ToString(),
            new { channel = slug, kind = normalizedKind },
            cancellationToken);

        return ChannelApprovalResult.Ok(approval.Id);
    }

    public async Task<IReadOnlyList<ApprovedChannel>> ListApprovedAsync(string? kind = null, CancellationToken cancellationToken = default)
    {
        var normalizedKindFilter = kind?.Trim().ToLowerInvariant();
        var query = db.Set<ApprovedChannel>().AsNoTracking().OrderBy(c => c.Channel);
        var list = string.IsNullOrWhiteSpace(kind)
            ? await query.ToListAsync(cancellationToken)
            : await query.Where(c => c.Kind == normalizedKindFilter).ToListAsync(cancellationToken);
        return list;
    }
}
