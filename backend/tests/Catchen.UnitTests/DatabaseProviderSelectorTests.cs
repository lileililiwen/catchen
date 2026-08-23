using Catchen.Data;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Catchen.UnitTests;

public sealed class DatabaseProviderSelectorTests
{
    private static DatabaseProviderSelector Selector(Dictionary<string, string?> settings)
    {
        return new(new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build());
    }

    [Fact]
    public void Defaults_to_PostgreSql_when_provider_is_absent()
    {
        Assert.Equal(DatabaseProvider.PostgreSql, Selector(new Dictionary<string, string?>()).Select());
    }

    [Theory]
    [InlineData("postgres", DatabaseProvider.PostgreSql)]
    [InlineData("postgresql", DatabaseProvider.PostgreSql)]
    [InlineData("npgsql", DatabaseProvider.PostgreSql)]
    [InlineData("SQLite", DatabaseProvider.Sqlite)]
    public void Maps_known_provider_aliases_case_insensitively(
        string raw, DatabaseProvider expected)
    {
        var selector = Selector(new Dictionary<string, string?>
        {
            ["Database:Provider"] = raw,
        });

        Assert.Equal(expected, selector.Select());
    }

    [Fact]
    public void Rejects_unknown_providers_with_an_actionable_error()
    {
        var selector = Selector(new Dictionary<string, string?>
        {
            ["Database:Provider"] = "oracle",
        });

        var exception = Assert.Throws<InvalidOperationException>(() => selector.Select());
        Assert.Contains("oracle", exception.Message);
    }

    [Fact]
    public void Requires_a_connection_string()
    {
        var selector = Selector(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Default"] = "",
        });

        Assert.Throws<InvalidOperationException>(() => selector.RequireConnectionString());
    }
}
