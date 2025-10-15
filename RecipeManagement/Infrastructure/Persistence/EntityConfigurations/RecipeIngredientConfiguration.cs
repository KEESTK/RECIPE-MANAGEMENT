using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.EntityConfigurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        // Composite primary key
        builder.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        // Relationships
        builder
            .HasOne(ri => ri.Recipe)
            .WithMany(r => r.RecipeIngredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(ri => ri.Ingredient)
            .WithMany(i => i.RecipeIngredients)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(ri => ri.Amount)
            .HasMaxLength(100);

        builder.ToTable("RecipeIngredients");
    }
}
