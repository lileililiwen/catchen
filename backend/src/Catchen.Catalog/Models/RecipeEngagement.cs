namespace Catchen.Catalog.Models;

/// <summary>A consumer's favorite marker for a logical recipe.</summary>
public sealed class RecipeFavorite
{
    public Guid Id { get; set; }

    public required Guid UserId { get; set; }

    public required Guid RecipeId { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}

public enum CommentStatus
{
    /// <summary>Publicly visible.</summary>
    Visible = 0,

    /// <summary>Hidden by a moderator — retained for audit, not public.</summary>
    Hidden = 1,
}

/// <summary>A moderated consumer comment on a recipe.</summary>
public sealed class RecipeComment
{
    public Guid Id { get; set; }

    public required Guid RecipeId { get; set; }

    public required Guid UserId { get; set; }

    public required string Text { get; set; }

    public CommentStatus Status { get; set; }

    /// <summary>Reason code recorded when a moderator hides the comment.</summary>
    public string? ModerationReason { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }
}
