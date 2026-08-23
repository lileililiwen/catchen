using System.Text.Json.Serialization;

namespace Catchen.Editorial.Models;

/// <summary>
/// Structured, validated recipe content. Quantities are exact and
/// convertible; vague measures never reach a published recipe.
/// </summary>
public sealed class RecipeContent
{
    public required string Title { get; set; }

    /// <summary>One of: sichuan, cantonese, flour_based, vegetarian, quick_home_style.</summary>
    public required string Cuisine { get; set; }

    /// <summary>One of: easy, medium, hard.</summary>
    public required string Difficulty { get; set; }

    /// <summary>Public preview shown to consumers without entitlement.</summary>
    public required string PreviewText { get; set; }

    public required List<IngredientLine> Ingredients { get; set; } = [];

    /// <summary>Ordered cooking steps in English.</summary>
    public required List<string> Instructions { get; set; } = [];

    /// <summary>Western kitchenware required (no domestic-only tools assumed).</summary>
    public required List<string> Equipment { get; set; } = [];

    /// <summary>Dish origin or holiday context.</summary>
    public required string CulturalContext { get; set; }

    [JsonIgnore]
    public IEnumerable<string> IngredientNames =>
        Ingredients.Select(i => i.Name);
}

public sealed class IngredientLine
{
    public required string Name { get; set; }

    public required Quantity Quantity { get; set; }

    /// <summary>Western-supermarket substitute for an Asian-grocery item.</summary>
    public Substitution? Substitution { get; set; }
}

public sealed class Quantity
{
    public required decimal Value { get; set; }

    /// <summary>One of the whitelisted convertible units.</summary>
    public required string Unit { get; set; }
}

public sealed class Substitution
{
    public required string Item { get; set; }

    /// <summary>True when the ORIGINAL ingredient is Asian-grocery-only.</summary>
    public bool AsianGroceryOnly { get; set; }

    public string? Note { get; set; }
}

/// <summary>
/// Copyright provenance: originality attestations retained as evidence.
/// Approval is impossible without both attestations.
/// </summary>
public sealed class ProvenanceEvidence
{
    public bool OriginalTextAttested { get; set; }

    public bool OriginalPhotographyAttested { get; set; }

    /// <summary>Where the recipe comes from (author's own development, etc.).</summary>
    public required string SourceNote { get; set; }
}
