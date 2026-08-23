namespace Catchen.Catalog.Services;

/// <summary>
/// Entitlement seam consumed by the catalog detail endpoint. Phase 1 commerce
/// (memberships, one-time purchases) registers its real implementation later;
/// until then nobody holds a paid entitlement.
/// </summary>
public interface IEntitlementProvider
{
    /// <summary>True when the user may read full premium content of the recipe.</summary>
    Task<bool> HasFullAccessAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default);
}

/// <summary>Default pre-commerce implementation: no paid entitlements exist.</summary>
public sealed class NoEntitlementProvider : IEntitlementProvider
{
    public Task<bool> HasFullAccessAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }
}
