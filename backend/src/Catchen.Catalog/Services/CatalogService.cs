using Catchen.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Catalog.Services;

public sealed record CatalogSummary(
    Guid RecipeId,
    int Version,
    string Title,
    string Cuisine,
    string Difficulty,
    string PreviewText,
    bool IsFree);

public sealed record CatalogDetail(
    CatalogSummary Summary,
    string? ContentJson,
    IReadOnlyList<string> PurchaseOptions);

public sealed record CatalogQuery(
    CuisineCategory? Category,
    RecipeDifficulty? Difficulty,
    string? Ingredient,
    string? SearchText);

/// <summary>
/// Public catalog browsing over live published recipes. Search and filters
/// are AND-combined (spec: only recipes matching ALL selected criteria).
/// </summary>
public interface ICatalogService
{
    Task<IReadOnlyList<CatalogSummary>> BrowseAsync(CatalogQuery query, CancellationToken cancellationToken = default);

    Task<CatalogDetail?> GetDetailAsync(Guid recipeId, Guid? userId, CancellationToken cancellationToken = default);
}

public sealed class CatalogService(DbContext db, IEntitlementProvider entitlements) : ICatalogService
{
    public async Task<IReadOnlyList<CatalogSummary>> BrowseAsync(CatalogQuery query, CancellationToken cancellationToken = default)
    {
        var rows = db.Set<PublishedRecipe>().AsNoTracking().Where(r => r.IsLive);

        if (query.Category is CuisineCategory category)
        {
            rows = rows.Where(r => r.Cuisine == category);
        }

        if (query.Difficulty is RecipeDifficulty difficulty)
        {
            rows = rows.Where(r => r.Difficulty == difficulty);
        }

        if (!string.IsNullOrWhiteSpace(query.Ingredient))
        {
            var ingredient = query.Ingredient.Trim().ToLowerInvariant();
            rows = rows.Where(r => r.IngredientIndex.Contains(ingredient));
        }

        if (!string.IsNullOrWhiteSpace(query.SearchText))
        {
            // ToLower() translates to SQL lower() on every supported provider
            // (unlike ToLowerInvariant); CA1304/CA1311/CA1862 are suppressed
            // because the StringComparison overloads do not translate to SQL.
#pragma warning disable CA1304, CA1311, CA1862
            var text = query.SearchText.Trim().ToLowerInvariant();
            rows = rows.Where(r => r.Title.ToLower().Contains(text)
                || r.PreviewText.ToLower().Contains(text));
#pragma warning restore CA1304, CA1311, CA1862
        }

        var materialized = await rows.ToListAsync(cancellationToken);

        // In-memory ordering: SQLite cannot sort DateTimeOffset and Phase 1
        // result sets are small; filters above remain server-evaluated.
        var list = materialized
            .OrderByDescending(r => r.PublishedAtUtc)
            .Select(r => new CatalogSummary(
                r.RecipeId,
                r.Version,
                r.Title,
                r.Cuisine.ToString(),
                r.Difficulty.ToString(),
                r.PreviewText,
                r.IsFree))
            .ToList();

        return list;
    }

    public async Task<CatalogDetail?> GetDetailAsync(Guid recipeId, Guid? userId, CancellationToken cancellationToken = default)
    {
        var recipe = await db.Set<PublishedRecipe>()
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeId && r.IsLive)
            .OrderByDescending(r => r.Version)
            .FirstOrDefaultAsync(cancellationToken);

        if (recipe is null)
        {
            return null;
        }

        var summary = new CatalogSummary(
            recipe.RecipeId,
            recipe.Version,
            recipe.Title,
            recipe.Cuisine.ToString(),
            recipe.Difficulty.ToString(),
            recipe.PreviewText,
            recipe.IsFree);

        if (recipe.IsFree)
        {
            return new CatalogDetail(summary, recipe.ContentJson, []);
        }

        if (userId is Guid authenticatedUserId
            && await entitlements.HasFullAccessAsync(authenticatedUserId, recipe.RecipeId, cancellationToken))
        {
            return new CatalogDetail(summary, recipe.ContentJson, []);
        }

        // Locked premium recipe: preview plus purchase options only — never
        // the protected instructions or assets.
        return new CatalogDetail(summary, null, ["membership", "single_recipe_purchase"]);
    }
}
