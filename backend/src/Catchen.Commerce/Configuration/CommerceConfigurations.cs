using Catchen.Commerce.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catchen.Commerce.Configuration;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Currency).HasMaxLength(3).IsRequired();
        builder.Property(o => o.Provider).HasMaxLength(16).IsRequired();
        builder.Property(o => o.ProviderReference).HasMaxLength(256);
        builder.HasIndex(o => new { o.Provider, o.ProviderReference }).IsUnique()
            .HasFilter("ProviderReference IS NOT NULL");
        builder.HasIndex(o => new { o.UserId, o.Status });
    }
}

public sealed class EntitlementConfiguration : IEntityTypeConfiguration<Entitlement>
{
    public void Configure(EntityTypeBuilder<Entitlement> builder)
    {
        builder.HasKey(e => e.Id);
        // Exactly-once grant per order.
        builder.HasIndex(e => e.OrderId).IsUnique();
        builder.HasIndex(e => new { e.UserId, e.Kind, e.RecipeId });
    }
}

public sealed class WebhookEventConfiguration : IEntityTypeConfiguration<WebhookEvent>
{
    public void Configure(EntityTypeBuilder<WebhookEvent> builder)
    {
        builder.ToTable("WebhookEvents");
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Provider).HasMaxLength(16).IsRequired();
        builder.Property(w => w.EventId).HasMaxLength(256).IsRequired();
        builder.Property(w => w.EventType).HasMaxLength(128).IsRequired();
        // Idempotency: a provider event id is stored at most once.
        builder.HasIndex(w => new { w.Provider, w.EventId }).IsUnique();
    }
}
