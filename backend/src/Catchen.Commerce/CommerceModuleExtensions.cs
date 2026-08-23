using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Commerce;

/// <summary>
/// Composition seam for the Identity module (accounts, roles, region policy,
/// agreement evidence). The API composition root calls
/// <see cref="AddCommerceModule"/>; module services are registered here as the
/// capabilities land (launch-overseas-recipe-membership tasks 1.3+).
/// </summary>
public static class CommerceModuleExtensions
{
    public static IServiceCollection AddCommerceModule(this IServiceCollection services)
    {
        return services;
    }
}
