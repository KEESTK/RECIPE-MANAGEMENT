using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.EntityConfigurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        // One Category → Many RecipeCategories (M:N link to Recipe)
        builder
            .HasMany(c => c.RecipeCategories)
            .WithOne(rc => rc.Category)
            .HasForeignKey(rc => rc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.ToTable("Categories");
    }
}
