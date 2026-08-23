using System.Text.Json;
using Catchen.Catalog.Models;
using Catchen.Commerce.Services;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Documents.Services;

public sealed record RecipePdfResult(byte[] PdfBytes, int Version);

/// <summary>
/// Versioned recipe PDFs and aggregated shopping-list PDFs (task 3.3).
/// Entitlements are enforced server-side BEFORE any byte is rendered:
/// premium recipes require an active membership or a purchase of that
/// recipe; shopping lists aggregate only recipes the user may read.
/// </summary>
public interface IRecipeDocumentService
{
    Task<RecipePdfResult?> RenderRecipePdfAsync(
        Guid userId, Guid recipeId, CancellationToken cancellationToken = default);

    /// <summary>Shopping list across the given recipes (entitlement-filtered).</summary>
    Task<RecipePdfResult?> RenderShoppingListAsync(
        Guid userId, IReadOnlyList<Guid> recipeIds, CancellationToken cancellationToken = default);
}

public sealed class RecipeDocumentService(DbContext db, IEntitlementLedger ledger) : IRecipeDocumentService
{
    public async Task<RecipePdfResult?> RenderRecipePdfAsync(
        Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        var recipe = await LatestLiveAsync(recipeId, cancellationToken);
        if (recipe is null)
        {
            return null;
        }

        if (!recipe.IsFree
            && !await ledger.HasFullAccessAsync(userId, recipe.RecipeId, cancellationToken))
        {
            return null; // locked: no bytes leave the server
        }

        var content = Parse(recipe.ContentJson);
        var pdf = RecipePdfComposer.Compose(
            recipe.Title,
            GetString(content, "culturalContext"),
            GetArray(content, "ingredients"),
            GetArray(content, "instructions"));
        return new RecipePdfResult(pdf, recipe.Version);
    }

    public async Task<RecipePdfResult?> RenderShoppingListAsync(
        Guid userId, IReadOnlyList<Guid> recipeIds, CancellationToken cancellationToken = default)
    {
        var totals = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
        var units = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var titles = new List<string>();

        foreach (var recipeId in recipeIds.Distinct())
        {
            var recipe = await LatestLiveAsync(recipeId, cancellationToken);
            if (recipe is null)
            {
                continue;
            }

            if (!recipe.IsFree
                && !await ledger.HasFullAccessAsync(userId, recipe.RecipeId, cancellationToken))
            {
                continue; // silently skip locked recipes from the aggregate
            }

            titles.Add(recipe.Title);
            var content = Parse(recipe.ContentJson);
            if (content is null || !content.TryGetValue("ingredients", out var ingredientsEl)
                || ingredientsEl.ValueKind != JsonValueKind.Array)
            {
                continue;
            }

            foreach (var ingredient in ingredientsEl.EnumerateArray())
            {
                var name = ingredient.TryGetProperty("name", out var nameEl)
                    ? nameEl.GetString() ?? "unknown"
                    : "unknown";
                decimal value = 0;
                var unit = string.Empty;
                if (ingredient.TryGetProperty("quantity", out var quantityEl)
                    && quantityEl.ValueKind == JsonValueKind.Object)
                {
                    if (quantityEl.TryGetProperty("value", out var valueEl)
                        && valueEl.TryGetDecimal(out var parsed))
                    {
                        value = parsed;
                    }

                    unit = quantityEl.TryGetProperty("unit", out var unitEl)
                        ? unitEl.GetString() ?? string.Empty
                        : string.Empty;
                }

                totals[name] = totals.GetValueOrDefault(name) + value;
                units[name] = unit;
            }
        }

        if (titles.Count == 0)
        {
            return null;
        }

        var rows = totals
            .Select(kv => (Name: kv.Key, Value: kv.Value, Unit: units.GetValueOrDefault(kv.Key)))
            .OrderBy(row => row.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var pdf = ShoppingListPdfComposer.Compose(titles, rows);
        return new RecipePdfResult(pdf, 1);
    }

    private async Task<PublishedRecipe?> LatestLiveAsync(Guid recipeId, CancellationToken ct)
    {
        return await db.Set<PublishedRecipe>()
            .AsNoTracking()
            .Where(r => r.RecipeId == recipeId && r.IsLive)
            .OrderByDescending(r => r.Version)
            .FirstOrDefaultAsync(ct);
    }

    private static Dictionary<string, JsonElement>? Parse(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string GetString(Dictionary<string, JsonElement>? content, string key)
    {
        return content is not null
        && content.TryGetValue(key, out var element)
        && element.ValueKind == JsonValueKind.String
            ? element.GetString() ?? string.Empty
            : string.Empty;
    }

    private static List<JsonElement> GetArray(Dictionary<string, JsonElement>? content, string key)
    {
        return content is not null
        && content.TryGetValue(key, out var element)
        && element.ValueKind == JsonValueKind.Array
            ? element.EnumerateArray().ToList()
            : [];
    }
}
