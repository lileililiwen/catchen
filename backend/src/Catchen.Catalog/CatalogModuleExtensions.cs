using Catchen.Catalog.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Catalog;

public static class CatalogModuleExtensions
{
    public static IServiceCollection AddCatalogModule(this IServiceCollection services)
    {
        services.AddSingleton<IEntitlementProvider, NoEntitlementProvider>();
        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IFavoritesService, FavoritesService>();
        services.AddScoped<ICommentsService, CommentsService>();
        return services;
    }
}
