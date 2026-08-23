using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Documents;

/// <summary>
/// Composition seam for the Identity module (accounts, roles, region policy,
/// agreement evidence). The API composition root calls
/// <see cref="AddDocumentsModule"/>; module services are registered here as the
/// capabilities land (launch-overseas-recipe-membership tasks 1.3+).
/// </summary>
public static class DocumentsModuleExtensions
{
    public static IServiceCollection AddDocumentsModule(this IServiceCollection services)
    {
        return services;
    }
}
