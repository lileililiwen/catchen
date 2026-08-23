using Catchen.Catalog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catchen.Catalog.Configuration;

public sealed class PublishedRecipeConfiguration : IEntityTypeConfiguration<PublishedRecipe>
{
    public void Configure(EntityTypeBuilder<PublishedRecipe> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Title).HasMaxLength(200).IsRequired();
        builder.Property(r => r.PreviewText).HasMaxLength(1000).IsRequired();
        builder.Property(r => r.ContentJson).IsRequired();
        builder.Property(r => r.IngredientIndex).HasMaxLength(2000).IsRequired();
        builder.HasIndex(r => new { r.RecipeId, r.Version }).IsUnique();
        builder.HasIndex(r => new { r.IsLive, r.Cuisine, r.Difficulty });
    }
}

public sealed class RecipeFavoriteConfiguration : IEntityTypeConfiguration<RecipeFavorite>
{
    public void Configure(EntityTypeBuilder<RecipeFavorite> builder)
    {
        builder.HasKey(f => f.Id);
        builder.HasIndex(f => new { f.UserId, f.RecipeId }).IsUnique();
    }
}

public sealed class RecipeCommentConfiguration : IEntityTypeConfiguration<RecipeComment>
{
    public void Configure(EntityTypeBuilder<RecipeComment> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Text).HasMaxLength(2000).IsRequired();
        builder.Property(c => c.ModerationReason).HasMaxLength(128);
        builder.HasIndex(c => new { c.RecipeId, c.Status });
        builder.HasIndex(c => c.UserId);
    }
}
