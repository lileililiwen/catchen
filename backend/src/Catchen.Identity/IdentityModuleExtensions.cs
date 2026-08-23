using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Identity;

/// <summary>
/// Composition seam for the Identity module (accounts, roles, region policy,
/// agreement evidence). The API composition root calls
/// <see cref="AddIdentityModule"/>; module services are registered here as the
/// capabilities land (launch-overseas-recipe-membership tasks 1.3+).
/// </summary>
public static class IdentityModuleExtensions
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        return services;
    }
}
