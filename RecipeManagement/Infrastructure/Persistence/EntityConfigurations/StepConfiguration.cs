using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.EntityConfigurations;

public class StepConfiguration : IEntityTypeConfiguration<Step>
{
    public void Configure(EntityTypeBuilder<Step> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Order)
            .IsRequired();

        builder.Property(s => s.Description)
            .IsRequired()
            .HasMaxLength(1000);

        // One Recipe → Many Steps
        builder
            .HasOne(s => s.Recipe)
            .WithMany(r => r.Steps)
            .HasForeignKey(s => s.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional: enforce order uniqueness within a recipe
        builder
            .HasIndex(s => new { s.RecipeId, s.Order })
            .IsUnique();

        builder.ToTable("Steps");
    }
}
