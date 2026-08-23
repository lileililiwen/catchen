namespace Catchen.Commerce.Models;

public enum ProductKind
{
    MonthlyMembership = 0,

    /// <summary>One-time purchase of a single recipe PDF.</summary>
    SingleRecipe = 1,
}

public enum OrderStatus
{
    Pending = 0,

    /// <summary>Verified provider event confirmed payment.</summary>
    Paid = 1,

    Refunded = 2,

    Disputed = 3,

    Cancelled = 4,
}

/// <summary>
/// Provider-neutral order. Money is stored as minor units (cents) plus an
/// explicit ISO-4217 currency — never floating point.
/// </summary>
public sealed class Order
{
    public Guid Id { get; set; }

    public required Guid UserId { get; set; }

    public required ProductKind ProductKind { get; set; }

    /// <summary>For SingleRecipe orders: the logical recipe purchased.</summary>
    public Guid? RecipeId { get; set; }

    public required long AmountMinorUnits { get; set; }

    /// <summary>ISO-4217 alpha-3, e.g. "USD".</summary>
    public required string Currency { get; set; }

    public OrderStatus Status { get; set; }

    /// <summary>"stripe" or "paypal" — the configured offshore provider.</summary>
    public required string Provider { get; set; }

    /// <summary>Provider's checkout-session / order reference.</summary>
    public string? ProviderReference { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset? PaidAtUtc { get; set; }
}
