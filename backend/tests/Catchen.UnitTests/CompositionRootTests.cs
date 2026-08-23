using System.Net.Http.Json;
using Catchen.Data;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Catchen.UnitTests;

/// <summary>
/// Boots the real composition root (Program + every AddXxxModule +
/// AddCatchenData) against a SQLite test database and verifies /healthz.
/// </summary>
public sealed class CompositionRootTests
{
    [Fact]
    public async Task Healthz_reports_ok_and_active_provider()
    {
        await using var factory = new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("Database:Provider", "sqlite");
                builder.UseSetting("ConnectionStrings:Default", "Data Source=catchen-tests.db");
            });

        var client = factory.CreateClient();

        var response = await client.GetAsync("/healthz");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<HealthPayload>();
        Assert.Equal("ok", payload?.Status);
        Assert.Equal("Sqlite", payload?.Database);
    }

    private sealed record HealthPayload(string Status, string Database);
}

public sealed class AppDbContextModelTests
{
    [Fact]
    public void Model_builds_with_module_configurations_applied()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        using var dbContext = new AppDbContext(options);

        // Accessing Model forces model creation, which runs OnModelCreating's
        // ApplyConfigurationsFromAssembly scan over every module assembly.
        Assert.NotNull(dbContext.Model);
    }
}

public sealed class DesignTimeDbContextFactoryTests
{
    [Fact]
    public void Creates_a_context_configured_for_sqlite()
    {
        var context = new DesignTimeDbContextFactory().CreateDbContext([]);

        Assert.True(context.Database.IsSqlite());
    }
}
