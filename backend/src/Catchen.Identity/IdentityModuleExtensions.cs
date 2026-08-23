using Catchen.Identity.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Identity;

public static class IdentityModuleExtensions
{
    public static IServiceCollection AddIdentityModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        var identityOptions = configuration.GetSection(IdentityOptions.SectionName);
        var jwtSecret = identityOptions["JwtSecret"];

        // Fail fast: a missing or short secret must never reach production.
        if (string.IsNullOrWhiteSpace(jwtSecret) || jwtSecret.Length < 32)
        {
            throw new InvalidOperationException(
                "Identity:JwtSecret is required and must be at least 32 characters. "
                + "Set Identity__JwtSecret (appsettings or environment).");
        }

        services.AddOptions<IdentityOptions>()
            .Bind(identityOptions)
            .PostConfigure(o =>
            {
                // ConfigurationBinder appends to pre-populated collections, so
                // defaults are applied AFTER binding when config omitted them.
                o.BlockedPhoneCountryCodes =
                    o.BlockedPhoneCountryCodes.Length == 0
                        ? PolicyDefaults.BlockedPhoneCountryCodes
                        : o.BlockedPhoneCountryCodes;
                o.BlockedDeclaredCountries =
                    o.BlockedDeclaredCountries.Length == 0
                        ? PolicyDefaults.BlockedDeclaredCountries
                        : o.BlockedDeclaredCountries;
            })
            .Validate(o => !string.IsNullOrWhiteSpace(o.JwtSecret) && o.JwtSecret.Length >= 32,
                "JwtSecret must be at least 32 characters")
            .Validate(o => o.JwtLifetimeMinutes > 0, "JwtLifetimeMinutes must be positive")
            .ValidateOnStart();

        services.AddOptions<ChannelPolicyOptions>()
            .Bind(configuration.GetSection(ChannelPolicyOptions.SectionName))
            .PostConfigure(o =>
            {
                o.AllowedPaymentMethods =
                    o.AllowedPaymentMethods.Length == 0
                        ? PolicyDefaults.AllowedPaymentMethods
                        : o.AllowedPaymentMethods;
                o.ProhibitedPromotionChannels =
                    o.ProhibitedPromotionChannels.Length == 0
                        ? PolicyDefaults.ProhibitedPromotionChannels
                        : o.ProhibitedPromotionChannels;
                o.ProhibitedDistributionChannels =
                    o.ProhibitedDistributionChannels.Length == 0
                        ? PolicyDefaults.ProhibitedDistributionChannels
                        : o.ProhibitedDistributionChannels;
            })
            .ValidateOnStart();

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPasswordHasher, Pbdkf2PasswordHasher>();
        services.AddSingleton<IPhoneNumberPolicy, PhoneNumberPolicy>();
        services.AddSingleton<IRegionPolicyService, RegionPolicyService>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuditWriter, AuditWriter>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IChannelPolicyService, ChannelPolicyService>();
        services.AddHostedService<AuditRetentionWorker>();

        return services;
    }
}
