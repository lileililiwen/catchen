using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Catchen.Data;

/// <summary>
/// Design-time factory for <c>dotnet ef migrations add</c>. Migrations are
/// provider-agnostic; they are generated against SQLite here and applied by
/// whichever provider the runtime selects (PostgreSQL in production).
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite("Data Source=catchen-designmode.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
