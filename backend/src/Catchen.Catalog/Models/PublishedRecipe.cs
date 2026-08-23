namespace Catchen.Catalog.Models;

/// <summary>The five Phase-1 cuisine categories consumers browse.</summary>
public enum CuisineCategory
{
    Sichuan = 0,
    Cantonese = 1,
    FlourBased = 2,
    Vegetarian = 3,
    QuickHomeStyle = 4,
}

public enum RecipeDifficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
}

/// <summary>
/// An immutable published recipe version. Rows are never edited — a new
/// publication appends a row with an incremented Version for the same
/// logical RecipeId; unpublishing flips IsLive on the latest row.
/// </summary>
public sealed class PublishedRecipe
{
    public Guid Id { get; set; }

    /// <summary>Logical recipe identifier shared across versions.</summary>
    public Guid RecipeId { get; set; }

    public int Version { get; set; }

    public required string Title { get; set; }

    public CuisineCategory Cuisine { get; set; }

    public RecipeDifficulty Difficulty { get; set; }

    /// <summary>Public preview text (always visible, even for locked recipes).</summary>
    public required string PreviewText { get; set; }

    /// <summary>
    /// Validated structured content (ingredients with exact quantities and
    /// substitutions, instructions, equipment, cultural context). Full content
    /// is only served to entitled consumers.
    /// </summary>
    public required string ContentJson { get; set; }

    /// <summary>Comma-separated ingredient names for filter/search.</summary>
    public required string IngredientIndex { get; set; }

    /// <summary>Free recipes need no membership or purchase.</summary>
    public bool IsFree { get; set; }

    public bool IsLive { get; set; }

    public required Guid AuthorUserId { get; set; }

    public required Guid ReviewerUserId { get; set; }

    public DateTimeOffset PublishedAtUtc { get; set; }
}
