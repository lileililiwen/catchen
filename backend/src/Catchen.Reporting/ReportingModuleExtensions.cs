using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Reporting;

/// <summary>
/// Composition seam for the Identity module (accounts, roles, region policy,
/// agreement evidence). The API composition root calls
/// <see cref="AddReportingModule"/>; module services are registered here as the
/// capabilities land (launch-overseas-recipe-membership tasks 1.3+).
/// </summary>
public static class ReportingModuleExtensions
{
    public static IServiceCollection AddReportingModule(this IServiceCollection services)
    {
        return services;
    }
}
