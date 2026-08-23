using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catchen.Data;

public static class DataExtensions
{
    /// <summary>
    /// Registers <see cref="AppDbContext"/> against the configured provider
    /// (PostgreSQL by default; SQLite for local development). Retry-on-failure
    /// is enabled for PostgreSQL to tolerate transient connection drops.
    /// </summary>
    public static IServiceCollection AddCatchenData(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var selector = new DatabaseProviderSelector(configuration);
        var provider = selector.Select();
        var connectionString = selector.RequireConnectionString();

        services.AddSingleton(selector);

        services.AddDbContext<AppDbContext>(options =>
        {
            switch (provider)
            {
                case DatabaseProvider.PostgreSql:
                    options.UseNpgsql(connectionString, npgsql => npgsql.EnableRetryOnFailure());
                    break;
                case DatabaseProvider.Sqlite:
                    options.UseSqlite(connectionString);
                    break;
                default:
                    throw new InvalidOperationException($"Unhandled provider '{provider}'.");
            }
        });

        // Module services inject the BASE DbContext (Agents.md §2); forward to
        // the scoped AppDbContext so both resolve to the same instance.
        services.AddScoped<DbContext>(sp => sp.GetRequiredService<AppDbContext>());

        return services;
    }
}
