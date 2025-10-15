using Microsoft.EntityFrameworkCore;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence;

public class RecipeDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Step> Steps => Set<Step>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();
    public DbSet<RecipeCategory> RecipeCategories => Set<RecipeCategory>();

    public RecipeDbContext(DbContextOptions<RecipeDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Automatically apply all IEntityTypeConfiguration<T> classes
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RecipeDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
