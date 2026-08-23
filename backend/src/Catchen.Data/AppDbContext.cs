using Catchen.Affiliates;
using Catchen.Catalog;
using Catchen.Commerce;
using Catchen.Documents;
using Catchen.Editorial;
using Catchen.Identity;
using Catchen.Moderation;
using Catchen.Reporting;
using Microsoft.EntityFrameworkCore;

namespace Catchen.Data;

/// <summary>
/// Central application DbContext. Module entities are configured by
/// <c>IEntityTypeConfiguration&lt;T&gt;</c> implementations discovered in each
/// module assembly — adding a module's configuration requires exactly one
/// <see cref="ModelBuilder.ApplyConfigurationsFromAssembly"/> line here.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityModuleExtensions).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogModuleExtensions).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EditorialModuleExtensions).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CommerceModuleExtensions).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DocumentsModuleExtensions).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AffiliatesModuleExtensions).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ModerationModuleExtensions).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportingModuleExtensions).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
