namespace Catchen.Data;

/// <summary>Database providers supported by the Catchen backend.</summary>
public enum DatabaseProvider
{
    /// <summary>PostgreSQL — the production provider (offshore managed instances).</summary>
    PostgreSql = 0,

    /// <summary>SQLite — local development and design-time tooling only.</summary>
    Sqlite = 1,
}
