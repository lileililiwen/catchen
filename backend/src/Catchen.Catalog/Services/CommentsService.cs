using Catchen.Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Catalog.Services;

public sealed record CommentResult(Guid? CommentId, string? Violation)
{
    public bool Succeeded => CommentId is not null;

    public static CommentResult Ok(Guid id)
    {
        return new(id, null);
    }

    public static CommentResult Failed(string violation)
    {
        return new(null, violation);
    }
}

/// <summary>
/// Consumer comments under moderation. Blocked users cannot submit; hidden
/// comments stay in the database for audit but never appear publicly.
/// </summary>
public interface ICommentsService
{
    Task<CommentResult> AddAsync(Guid userId, Guid recipeId, string text, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RecipeComment>> ListVisibleAsync(Guid recipeId, CancellationToken cancellationToken = default);

    /// <summary>Moderator action: hide a comment (retained for audit).</summary>
    Task<bool> HideAsync(Guid commentId, string reasonCode, CancellationToken cancellationToken = default);

    /// <summary>Moderator action: block a repeat offender from new comments.</summary>
    Task<bool> BlockUserAsync(Guid userId, string reasonCode, CancellationToken cancellationToken = default);

    Task<bool> IsUserBlockedAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>Moderation workload counters (task 4.2).</summary>
    Task<(int Visible, int Hidden)> CountsAsync(CancellationToken cancellationToken = default);
}

public sealed class CommentsService(DbContext db, TimeProvider clock) : ICommentsService
{
    public async Task<CommentResult> AddAsync(Guid userId, Guid recipeId, string text, CancellationToken cancellationToken = default)
    {
        if (await IsUserBlockedAsync(userId, cancellationToken))
        {
            return CommentResult.Failed("user_blocked");
        }

        var liveRecipeExists = await db.Set<PublishedRecipe>()
            .AnyAsync(r => r.RecipeId == recipeId && r.IsLive, cancellationToken);
        if (!liveRecipeExists)
        {
            return CommentResult.Failed("recipe_not_found");
        }

        var trimmed = text?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length > 2000)
        {
            return CommentResult.Failed("text_invalid");
        }

        var comment = new RecipeComment
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            UserId = userId,
            Text = trimmed,
            Status = CommentStatus.Visible,
            CreatedAtUtc = clock.GetUtcNow(),
        };

        db.Set<RecipeComment>().Add(comment);
        await db.SaveChangesAsync(cancellationToken);
        return CommentResult.Ok(comment.Id);
    }

    public async Task<IReadOnlyList<RecipeComment>> ListVisibleAsync(Guid recipeId, CancellationToken cancellationToken = default)
    {
        var rows = await db.Set<RecipeComment>()
            .AsNoTracking()
            .Where(c => c.RecipeId == recipeId && c.Status == CommentStatus.Visible)
            .ToListAsync(cancellationToken);

        // In-memory ordering for provider portability (see CatalogService).
        return rows.OrderByDescending(c => c.CreatedAtUtc).ToList();
    }

    public async Task<bool> HideAsync(Guid commentId, string reasonCode, CancellationToken cancellationToken = default)
    {
        var updated = await db.Set<RecipeComment>()
            .Where(c => c.Id == commentId && c.Status == CommentStatus.Visible)
            .ExecuteUpdateAsync(
                set => set
                    .SetProperty(c => c.Status, CommentStatus.Hidden)
                    .SetProperty(c => c.ModerationReason, reasonCode),
                cancellationToken);
        return updated > 0;
    }

    public async Task<bool> BlockUserAsync(Guid userId, string reasonCode, CancellationToken cancellationToken = default)
    {
        // Moderation decision recorded on the user's visible comments: they are
        // hidden and stamped with the block reason.
        await db.Set<RecipeComment>()
            .Where(c => c.UserId == userId && c.Status == CommentStatus.Visible)
            .ExecuteUpdateAsync(
                set => set
                    .SetProperty(c => c.Status, CommentStatus.Hidden)
                    .SetProperty(c => c.ModerationReason, reasonCode),
                cancellationToken);

        var marker = await db.Set<RecipeComment>()
            .AnyAsync(c => c.UserId == userId && c.ModerationReason == "user_blocked", cancellationToken);
        if (marker)
        {
            return true;
        }

        db.Set<RecipeComment>().Add(new RecipeComment
        {
            Id = Guid.NewGuid(),
            RecipeId = Guid.Empty,
            UserId = userId,
            Text = "[moderation:block]",
            Status = CommentStatus.Hidden,
            ModerationReason = "user_blocked",
            CreatedAtUtc = clock.GetUtcNow(),
        });
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> IsUserBlockedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await db.Set<RecipeComment>().AnyAsync(
            c => c.UserId == userId && c.ModerationReason == "user_blocked", cancellationToken);
    }

    public async Task<(int Visible, int Hidden)> CountsAsync(CancellationToken cancellationToken = default)
    {
        var rows = await db.Set<RecipeComment>().AsNoTracking().ToListAsync(cancellationToken);
        return (
            rows.Count(c => c.Status == CommentStatus.Visible),
            rows.Count(c => c.Status == CommentStatus.Hidden));
    }
}
