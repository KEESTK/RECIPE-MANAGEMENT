using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.EntityConfigurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(i => i.Id);

        builder.HasIndex(i => i.Name).IsUnique();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        // One Ingredient → Many RecipeIngredients (M:N link to Recipe)
        builder
            .HasMany(i => i.RecipeIngredients)
            .WithOne(ri => ri.Ingredient)
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Ingredients");
    }
}
