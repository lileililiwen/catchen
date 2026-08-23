namespace Catchen.Commerce.Models;

/// <summary>
/// Durable access grant. Membership rows carry a PeriodEndUtc that verified
/// renewal events extend exactly once; single-recipe rows are open-ended and
/// follow refund/dispute policy (RevokedAtUtc set on confirmed refunds).
/// </summary>
public sealed class Entitlement
{
    public Guid Id { get; set; }

    public required Guid UserId { get; set; }

    public required ProductKind Kind { get; set; }

    /// <summary>For SingleRecipe entitlements: the logical recipe purchased.</summary>
    public Guid? RecipeId { get; set; }

    /// <summary>Order whose verified payment created this entitlement.</summary>
    public required Guid OrderId { get; set; }

    /// <summary>Null for open-ended single-recipe access.</summary>
    public DateTimeOffset? PeriodEndUtc { get; set; }

    public DateTimeOffset? RevokedAtUtc { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}
