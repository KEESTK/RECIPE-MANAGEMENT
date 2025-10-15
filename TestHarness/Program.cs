using Microsoft.EntityFrameworkCore;
using RecipeManagement.Infrastructure.Persistence;
using RecipeManagement.Infrastructure.Persistence.Repositories;
using RecipeManagement.Domain.Services;
using RecipeManagement.Application.Interfaces;
using RecipeManagement.Domain.Entities;

// Configure EF Core (SQLite in local file)
var options = new DbContextOptionsBuilder<RecipeDbContext>()
    .UseSqlite("Data Source=recipes_test.db")
    .Options;

// Initialize DbContext
using var dbContext = new RecipeDbContext(options);

// Initialize repositories
IUserRepository userRepo = new UserRepository(dbContext);
IRecipeRepository recipeRepo = new RecipeRepository(dbContext);
IFavoriteRepository favoriteRepo = new FavoriteRepository(dbContext);

// Initialize services
var userService = new UserService(userRepo);
var recipeService = new RecipeService(recipeRepo, userRepo);
var favoriteService = new FavoriteService(userRepo, recipeRepo, favoriteRepo);

// Create database (if not exists)
await dbContext.Database.EnsureCreatedAsync();

Console.WriteLine("=== RecipeManagement TestHarness ===");

// Add a new user
await userService.RegisterUserAsync("demoUser", "hashed_password", "demo@example.com");
var user = await userRepo.GetByUsernameAsync("demoUser");

if (user is not null)
{
    Console.WriteLine($"User created: {user.Username} ({user.Id})");

    // ✅ Create some global entities (Ingredient & Category) first
    var noodles = new Ingredient("Noodles");
    var dinnerCategory = new Category("Dinner");

    dbContext.Ingredients.Add(noodles);
    dbContext.Categories.Add(dinnerCategory);
    await dbContext.SaveChangesAsync();

    Console.WriteLine($"Ingredient created: {noodles.Name} ({noodles.Id})");
    Console.WriteLine($"Category created: {dinnerCategory.Name} ({dinnerCategory.Id})");

    // ✅ Now create recipe that references those IDs
    var recipe = new Recipe("Pasta", user.Id, "Simple pasta dish");

    recipe.AddStep("Boil water", 1);
    recipe.AddIngredient(noodles.Id, "200g");
    recipe.AddCategory(dinnerCategory.Id);

    await recipeService.AddRecipeAsync(recipe);
    await recipeRepo.SaveChangesAsync();

    Console.WriteLine("Recipe created successfully!");
}
else
{
    Console.WriteLine("User not found — something went wrong.");
}
