using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Affiliates;

/// <summary>
/// Composition seam for the Identity module (accounts, roles, region policy,
/// agreement evidence). The API composition root calls
/// <see cref="AddAffiliatesModule"/>; module services are registered here as the
/// capabilities land (launch-overseas-recipe-membership tasks 1.3+).
/// </summary>
public static class AffiliatesModuleExtensions
{
    public static IServiceCollection AddAffiliatesModule(this IServiceCollection services)
    {
        return services;
    }
}
