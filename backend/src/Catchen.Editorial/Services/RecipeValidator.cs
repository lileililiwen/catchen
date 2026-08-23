using Catchen.Editorial.Models;

namespace Catchen.Editorial.Services;

public sealed record ValidationReport(bool IsValid, IReadOnlyList<string> Violations)
{
    public static ValidationReport Ok()
    {
        return new(true, Array.Empty<string>());
    }
}

/// <summary>
/// Deterministic content validation (task 2.2). Same input always yields the
/// same report: structure completeness, exact convertible quantities with a
/// whitelisted unit set, vague-measure blacklisting, substitution and
/// cultural-context requirements, and provenance attestations.
/// </summary>
public interface IRecipeValidator
{
    ValidationReport Validate(RecipeContent? content, ProvenanceEvidence? provenance);
}

public sealed partial class RecipeValidator : IRecipeValidator
{
    private static readonly HashSet<string> _allowedUnits = new(StringComparer.Ordinal)
    {
        "g", "kg", "ml", "l", "tsp", "tbsp", "cup", "oz", "lb", "fl oz",
    };

    private static readonly string[] _vaguePhrases =
    [
        "a little", "a dash", "a pinch", "to taste", "as needed",
        "appropriate amount", "some", "moderate amount",
        "适量", "少许", "按需",
    ];

    private static readonly string[] _validCuisines =
    [
        "sichuan", "cantonese", "flour_based", "vegetarian", "quick_home_style",
    ];

    private static readonly string[] _validDifficulties = ["easy", "medium", "hard"];

    public ValidationReport Validate(RecipeContent? content, ProvenanceEvidence? provenance)
    {
        var violations = new List<string>();

        if (content is null)
        {
            return new ValidationReport(false, ["content_unparseable"]);
        }

        if (string.IsNullOrWhiteSpace(content.Title))
        {
            violations.Add("title_missing");
        }

        if (string.IsNullOrWhiteSpace(content.PreviewText))
        {
            violations.Add("preview_missing");
        }

        var cuisine = content.Cuisine?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(cuisine) || !_validCuisines.Contains(cuisine))
        {
            violations.Add($"cuisine_invalid:{content.Cuisine}");
        }

        var difficulty = content.Difficulty?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(difficulty) || !_validDifficulties.Contains(difficulty))
        {
            violations.Add($"difficulty_invalid:{content.Difficulty}");
        }

        if (content.Ingredients is not { Count: > 0 })
        {
            violations.Add("ingredients_missing");
        }
        else
        {
            for (var i = 0; i < content.Ingredients.Count; i++)
            {
                var ingredient = content.Ingredients[i];
                var label = $"ingredient[{i}]";

                if (string.IsNullOrWhiteSpace(ingredient.Name))
                {
                    violations.Add($"{label}:name_missing");
                    continue;
                }

                var quantity = ingredient.Quantity;
                if (quantity is null)
                {
                    violations.Add($"{label}:quantity_missing");
                }
                else
                {
                    if (quantity.Value <= 0)
                    {
                        violations.Add($"{label}:quantity_not_positive");
                    }

                    if (quantity.Unit is null
                        || !_allowedUnits.Contains(quantity.Unit.Trim().ToLowerInvariant()))
                    {
                        violations.Add($"{label}:unit_not_convertible:{quantity.Unit}");
                    }
                }

                // The ingredient NAME itself must not carry a vague measure.
                foreach (var violation in VagueViolations(ingredient.Name, $"{label}:name"))
                {
                    violations.Add(violation);
                }

                if (ingredient.Substitution is { } substitution
                    && string.IsNullOrWhiteSpace(substitution.Item))
                {
                    violations.Add($"{label}:substitution_item_missing");
                }
            }
        }

        if (content.Instructions is not { Count: > 0 })
        {
            violations.Add("instructions_missing");
        }
        else
        {
            violations.AddRange(content.Instructions.SelectMany(
                (instruction, i) => VagueViolations(instruction, $"instruction[{i}]")));
        }

        if (content.Equipment is not { Count: > 0 })
        {
            violations.Add("equipment_missing");
        }

        if (string.IsNullOrWhiteSpace(content.CulturalContext))
        {
            violations.Add("cultural_context_missing");
        }

        if (provenance is null)
        {
            violations.Add("provenance_missing");
        }
        else
        {
            if (!provenance.OriginalTextAttested)
            {
                violations.Add("provenance_original_text_not_attested");
            }

            if (!provenance.OriginalPhotographyAttested)
            {
                violations.Add("provenance_original_photography_not_attested");
            }

            if (string.IsNullOrWhiteSpace(provenance.SourceNote))
            {
                violations.Add("provenance_source_note_missing");
            }
        }

        return violations.Count == 0
            ? ValidationReport.Ok()
            : new ValidationReport(false, violations);
    }

    private static IEnumerable<string> VagueViolations(string text, string label)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        foreach (var phrase in _vaguePhrases.Where(
            phrase => text.Contains(phrase, StringComparison.OrdinalIgnoreCase)))
        {
            yield return $"{label}:vague_quantity:\"{phrase}\"";
        }
    }
}
