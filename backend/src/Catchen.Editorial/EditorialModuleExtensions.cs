using Catchen.Editorial.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Editorial;

public static class EditorialModuleExtensions
{
    public static IServiceCollection AddEditorialModule(this IServiceCollection services)
    {
        services.AddSingleton<IRecipeValidator, RecipeValidator>();
        services.AddScoped<IEditorialWorkflowService, EditorialWorkflowService>();
        return services;
    }
}
