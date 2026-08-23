using System.Text.Json;

namespace Catchen.Editorial.Models;

public enum DraftStatus
{
    /// <summary>Being written; not yet submitted.</summary>
    Draft = 0,

    /// <summary>Submitted for review; validation evidence attached.</summary>
    Submitted = 1,

    /// <summary>Changes requested by reviewer; back to author.</summary>
    ChangesRequested = 2,

    /// <summary>Approved and published as an immutable catalog version.</summary>
    Published = 3,

    /// <summary>Was live, now withdrawn from the catalog.</summary>
    Unpublished = 4,
}

/// <summary>A recipe draft moving through the editorial workflow.</summary>
public sealed class RecipeDraft
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Cuisine { get; set; }

    public required string Difficulty { get; set; }

    public bool IsFree { get; set; }

    public required string ContentJson { get; set; }

    public required string ProvenanceJson { get; set; }

    public DraftStatus Status { get; set; }

    public required Guid AuthorUserId { get; set; }

    public Guid? ReviewerUserId { get; set; }

    /// <summary>Secondary usability review (substitutions/quantities/kitchenware) recorded before publication.</summary>
    public DateTimeOffset? SecondaryReviewAtUtc { get; set; }

    public Guid? SecondaryReviewerUserId { get; set; }

    /// <summary>Deterministic validation report from the last submit.</summary>
    public string? ValidationReportJson { get; set; }

    public Guid? PublishedRecipeId { get; set; }

    public int PublishedVersion { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }
}

public static class RecipeProvenanceJson
{
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

    public static string Serialize(ProvenanceEvidence provenance)
    {
        return JsonSerializer.Serialize(provenance, _options);
    }

    public static ProvenanceEvidence? TryParse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<ProvenanceEvidence>(json, _options);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}

public static class RecipeContentJson
{
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);

    public static string Serialize(RecipeContent content)
    {
        return JsonSerializer.Serialize(content, _options);
    }

    public static RecipeContent? TryParse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<RecipeContent>(json, _options);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
