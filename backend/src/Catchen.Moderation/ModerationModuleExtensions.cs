using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Moderation;

/// <summary>
/// Composition seam for the Identity module (accounts, roles, region policy,
/// agreement evidence). The API composition root calls
/// <see cref="AddModerationModule"/>; module services are registered here as the
/// capabilities land (launch-overseas-recipe-membership tasks 1.3+).
/// </summary>
public static class ModerationModuleExtensions
{
    public static IServiceCollection AddModerationModule(this IServiceCollection services)
    {
        return services;
    }
}
