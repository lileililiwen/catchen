using Catchen.Affiliates.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Affiliates;

public static class AffiliatesModuleExtensions
{
    public static IServiceCollection AddAffiliatesModule(this IServiceCollection services)
    {
        services.AddScoped<IAffiliateLinkService, AffiliateLinkService>();
        services.AddScoped<ICommissionImportService, CommissionImportService>();
        return services;
    }
}
