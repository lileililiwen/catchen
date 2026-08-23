using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Catchen.Documents.Services;

/// <summary>
/// QuestPDF composers (MIT). Fonts: the default Liberation/DejaVu set shipped
/// with QuestPDF covers Latin text; no embedded CJK font is required for the
/// English-only Phase 1 documents.
/// </summary>
public static class RecipePdfComposer
{
    public static byte[] Compose(
        string title, string culturalContext,
        List<JsonElement> ingredients, List<JsonElement> instructions)
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(36);
                page.Header().Text(title).FontSize(20).Bold();
                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text("Ingredients").FontSize(14).Bold();
                    foreach (var ingredient in ingredients)
                    {
                        var name = GetString(ingredient, "name");
                        var quantity = string.Empty;
                        if (ingredient.TryGetProperty("quantity", out var q))
                        {
                            var valueText = q.TryGetProperty("value", out var v)
                                ? v.GetRawText()
                                : string.Empty;
                            var unitText = q.TryGetProperty("unit", out var u)
                                ? u.GetString() ?? string.Empty
                                : string.Empty;
                            quantity = $"{valueText} {unitText}".Trim();
                        }
                        var substitution = ingredient.TryGetProperty("substitution", out var s)
                            ? $" — substitute: {GetString(s, "item")}"
                            : string.Empty;
                        column.Item().Text($"{name}: {quantity}{substitution}");
                    }

                    column.Item().PaddingTop(8).Text("Instructions").FontSize(14).Bold();
                    for (var i = 0; i < instructions.Count; i++)
                    {
                        column.Item().Text($"{i + 1}. {GetString(instructions[i], null)}");
                    }

                    column.Item().PaddingTop(8).Text("Cultural context").FontSize(14).Bold();
                    column.Item().Text(culturalContext);
                });
            });
        }).GeneratePdf();
    }

    private static string GetString(JsonElement element, string? property)
    {
        if (property is null)
        {
            return element.ValueKind == JsonValueKind.String ? element.GetString() ?? string.Empty : element.GetRawText();
        }

        return element.TryGetProperty(property, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString() ?? string.Empty
            : string.Empty;
    }
}

public static class ShoppingListPdfComposer
{
    public static byte[] Compose(List<string> recipeTitles, List<(string Name, decimal Value, string? Unit)> rows)
    {
        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Margin(36);
                page.Header().Text("Shopping list").FontSize(20).Bold();
                page.Content().Column(column =>
                {
                    column.Spacing(6);
                    column.Item().Text($"For: {string.Join(", ", recipeTitles)}");
                    foreach (var row in rows)
                    {
                        column.Item().Text(
                            $"{row.Name}: {row.Value:0.##} {row.Unit}".TrimEnd());
                    }
                });
            });
        }).GeneratePdf();
    }
}
