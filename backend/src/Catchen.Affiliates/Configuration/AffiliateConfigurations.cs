using Catchen.Affiliates.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catchen.Affiliates.Configuration;

public sealed class AffiliateMerchantConfiguration : IEntityTypeConfiguration<AffiliateMerchant>
{
    public void Configure(EntityTypeBuilder<AffiliateMerchant> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Slug).HasMaxLength(64).IsRequired();
        builder.HasIndex(m => m.Slug).IsUnique();
        builder.Property(m => m.DisplayName).HasMaxLength(128).IsRequired();
        builder.Property(m => m.BaseUrl).HasMaxLength(512).IsRequired();
        builder.Property(m => m.AttributionTag).HasMaxLength(64).IsRequired();
    }
}

public sealed class AffiliateClickConfiguration : IEntityTypeConfiguration<AffiliateClick>
{
    public void Configure(EntityTypeBuilder<AffiliateClick> builder)
    {
        builder.ToTable("AffiliateClicks");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.MerchantSlug).HasMaxLength(64).IsRequired();
        builder.Property(c => c.CampaignId).HasMaxLength(64);
        builder.Property(c => c.VisitorPseudonym).HasMaxLength(64).IsRequired();
        builder.HasIndex(c => new { c.MerchantSlug, c.ClickedAtUtc });
    }
}

public sealed class CommissionStatementRowConfiguration : IEntityTypeConfiguration<CommissionStatementRow>
{
    public void Configure(EntityTypeBuilder<CommissionStatementRow> builder)
    {
        builder.ToTable("CommissionStatementRows");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Provider).HasMaxLength(16).IsRequired();
        builder.Property(r => r.ExternalRowId).HasMaxLength(128).IsRequired();
        builder.Property(r => r.MerchantSlug).HasMaxLength(64).IsRequired();
        builder.Property(r => r.Currency).HasMaxLength(3).IsRequired();
        builder.HasIndex(r => new { r.Provider, r.ExternalRowId }).IsUnique();
    }
}
