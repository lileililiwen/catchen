using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Editorial;

/// <summary>
/// Composition seam for the Identity module (accounts, roles, region policy,
/// agreement evidence). The API composition root calls
/// <see cref="AddEditorialModule"/>; module services are registered here as the
/// capabilities land (launch-overseas-recipe-membership tasks 1.3+).
/// </summary>
public static class EditorialModuleExtensions
{
    public static IServiceCollection AddEditorialModule(this IServiceCollection services)
    {
        return services;
    }
}
