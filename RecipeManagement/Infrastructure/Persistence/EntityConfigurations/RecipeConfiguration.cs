using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.EntityConfigurations;

public class RecipeConfiguration : IEntityTypeConfiguration<Recipe>
{
    public void Configure(EntityTypeBuilder<Recipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasIndex(r => r.Name).IsUnique();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Description)
            .HasMaxLength(1000);

        // --- Relationships ---

        // One Recipe → One Author (User)
        builder
            .HasOne(r => r.Author)
            .WithMany(u => u.Recipes)
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // One Recipe → Many Steps
        builder
            .HasMany(r => r.Steps)
            .WithOne(s => s.Recipe)
            .HasForeignKey(s => s.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // One Recipe → Many RecipeIngredients (M:N link to Ingredient)
        builder
            .HasMany(r => r.RecipeIngredients)
            .WithOne(ri => ri.Recipe)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // One Recipe → Many RecipeCategories (M:N link to Category)
        builder
            .HasMany(r => r.RecipeCategories)
            .WithOne(rc => rc.Recipe)
            .HasForeignKey(rc => rc.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // One Recipe → Many Favorites (M:N link to User)
        builder
            .HasMany<Favorite>()
            .WithOne(f => f.Recipe)
            .HasForeignKey(f => f.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Recipes");
    }
}
