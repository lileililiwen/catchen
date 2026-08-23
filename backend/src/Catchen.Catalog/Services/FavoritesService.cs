using Catchen.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Catalog.Services;

/// <summary>Favorites persist across devices for the authenticated user.</summary>
public interface IFavoritesService
{
    Task<bool> AddAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CatalogSummary>> ListMineAsync(Guid userId, CancellationToken cancellationToken = default);
}

public sealed class FavoritesService(DbContext db, TimeProvider clock) : IFavoritesService
{
    public async Task<bool> AddAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        var liveRecipeExists = await db.Set<PublishedRecipe>()
            .AnyAsync(r => r.RecipeId == recipeId && r.IsLive, cancellationToken);
        if (!liveRecipeExists)
        {
            return false;
        }

        var exists = await db.Set<RecipeFavorite>().AnyAsync(
            f => f.UserId == userId && f.RecipeId == recipeId, cancellationToken);
        if (exists)
        {
            return true;
        }

        db.Set<RecipeFavorite>().Add(new RecipeFavorite
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RecipeId = recipeId,
            CreatedAtUtc = clock.GetUtcNow(),
        });
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> RemoveAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        var deleted = await db.Set<RecipeFavorite>()
            .Where(f => f.UserId == userId && f.RecipeId == recipeId)
            .ExecuteDeleteAsync(cancellationToken);
        return deleted > 0;
    }

    public async Task<IReadOnlyList<CatalogSummary>> ListMineAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var rows = await db.Set<RecipeFavorite>()
            .AsNoTracking()
            .Where(f => f.UserId == userId)
            .Join(db.Set<PublishedRecipe>().AsNoTracking().Where(r => r.IsLive),
                f => f.RecipeId,
                r => r.RecipeId,
                (f, r) => r)
            .ToListAsync(cancellationToken);

        // In-memory ordering for provider portability (see CatalogService).
        return rows
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
    }
}
