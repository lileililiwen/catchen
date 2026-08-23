using Microsoft.Extensions.Configuration;

namespace Catchen.Data;

/// <summary>
/// Resolves the active database provider from configuration
/// (<c>Database:Provider</c>, env-overridable via
/// <c>Database__Provider=postgres|sqlite</c>). PostgreSQL is the default:
/// production runs on offshore managed instances; SQLite exists for local
/// development and design-time tooling.
/// </summary>
public sealed class DatabaseProviderSelector
{
    public const string ProviderSection = "Database:Provider";
    public const string DefaultConnectionStringName = "Default";

    private readonly IConfiguration _configuration;

    public DatabaseProviderSelector(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public DatabaseProvider Select()
    {
        var raw = _configuration[ProviderSection];

        if (string.IsNullOrWhiteSpace(raw))
        {
            return DatabaseProvider.PostgreSql;
        }

        return raw.Trim().ToLowerInvariant() switch
        {
            "postgres" or "postgresql" or "npgsql" => DatabaseProvider.PostgreSql,
            "sqlite" => DatabaseProvider.Sqlite,
            _ => throw new InvalidOperationException(
                $"Unsupported {ProviderSection} value '{raw}'. Supported providers: postgres, sqlite."),
        };
    }

    /// <summary>
    /// Returns the configured connection string, failing with an actionable
    /// message when it is missing — a misconfigured deployment must not start.
    /// </summary>
    public string RequireConnectionString()
    {
        var connectionString = _configuration.GetConnectionString(DefaultConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string '{DefaultConnectionStringName}' is required "
                + "(set ConnectionStrings__Default).");
        }

        return connectionString;
    }
}
