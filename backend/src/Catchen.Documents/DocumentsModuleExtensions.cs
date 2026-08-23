using Catchen.Documents.Services;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Catchen.Documents;

public static class DocumentsModuleExtensions
{
    public static IServiceCollection AddDocumentsModule(this IServiceCollection services)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        services.AddScoped<IRecipeDocumentService, RecipeDocumentService>();
        return services;
    }
}
