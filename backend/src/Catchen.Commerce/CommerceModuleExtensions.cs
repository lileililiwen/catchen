using Catchen.Catalog.Services;
using Catchen.Commerce.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Commerce;

public static class CommerceModuleExtensions
{
    public static IServiceCollection AddCommerceModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        var secret = configuration["Commerce:WebhookSecret"];
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
        {
            throw new InvalidOperationException(
                "Commerce:WebhookSecret is required and must be at least 32 characters. "
                + "Set Commerce__WebhookSecret (appsettings or environment).");
        }

        WebhookSecrets.Current = secret;

        services.AddSingleton<IWebhookSignatureVerifier, StripeSchemeSignatureVerifier>();
        services.AddScoped<IEntitlementLedger, EntitlementLedger>();
        services.AddScoped<IWebhookInboxService, WebhookInboxService>();
        services.AddScoped<ICheckoutService, CheckoutService>();

        // Commerce fulfills the catalog's entitlement seam (replaces the
        // pre-commerce NoEntitlementProvider registration).
        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IEntitlementProvider));
        if (descriptor is not null)
        {
            services.Remove(descriptor);
        }
        services.AddScoped<IEntitlementProvider>(
            sp => new CommerceEntitlementProvider(sp.GetRequiredService<IEntitlementLedger>()));

        return services;
    }
}

/// <summary>Adapts the ledger to the catalog's entitlement seam.</summary>
public sealed class CommerceEntitlementProvider(IEntitlementLedger ledger) : IEntitlementProvider
{
    public Task<bool> HasFullAccessAsync(Guid userId, Guid recipeId, CancellationToken cancellationToken = default)
    {
        return ledger.HasFullAccessAsync(userId, recipeId, cancellationToken);
    }
}
