using Catchen.Editorial.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catchen.Editorial.Configuration;

public sealed class RecipeDraftConfiguration : IEntityTypeConfiguration<RecipeDraft>
{
    public void Configure(EntityTypeBuilder<RecipeDraft> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Title).HasMaxLength(200).IsRequired();
        builder.Property(d => d.Cuisine).HasMaxLength(32).IsRequired();
        builder.Property(d => d.Difficulty).HasMaxLength(16).IsRequired();
        builder.Property(d => d.ContentJson).IsRequired();
        builder.Property(d => d.ProvenanceJson).IsRequired();
        builder.HasIndex(d => d.Status);
    }
}
