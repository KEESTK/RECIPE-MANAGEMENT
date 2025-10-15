using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.EntityConfigurations;

public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        // Composite primary key
        builder.HasKey(f => new { f.UserId, f.RecipeId });

        // Relationships
        builder
            .HasOne(f => f.User)
            .WithMany(u => u.Favorites)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(f => f.Recipe)
            .WithMany()
            .HasForeignKey(f => f.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.ToTable("Favorites");
    }
}
