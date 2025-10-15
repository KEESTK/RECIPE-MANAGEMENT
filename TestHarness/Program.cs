using Microsoft.EntityFrameworkCore;
using RecipeManagement.Infrastructure.Persistence;
using RecipeManagement.Infrastructure.Persistence.Repositories;
using RecipeManagement.Domain.Services;
using RecipeManagement.Application.Interfaces;
using RecipeManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestHarnessApp
{
    class Program
    {
        static async Task Main()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var ex = (Exception)args.ExceptionObject;
                Console.WriteLine($"\n💥 Global handler caught unhandled exception: {ex.GetType().Name} - {ex.Message}");
            };

            var results = new List<TestResult>();

            var options = new DbContextOptionsBuilder<RecipeDbContext>()
                .UseSqlite("Data Source=recipes_test.db")
                .Options;

            using var db = new RecipeDbContext(options);
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            IUserRepository userRepo = new UserRepository(db);
            IRecipeRepository recipeRepo = new RecipeRepository(db);
            IFavoriteRepository favoriteRepo = new FavoriteRepository(db);

            var userService = new UserService(userRepo);
            var recipeService = new RecipeService(recipeRepo, userRepo);
            var favoriteService = new FavoriteService(userRepo, recipeRepo, favoriteRepo);

            Console.WriteLine("=== RecipeManagement TestHarness (enhanced report mode) ===");

            try
            {
                // -------- Seed data ------------------------------------------------------
                await userService.RegisterUserAsync("demoUser", "hashed_password", "demo@example.com");
                var user = await userRepo.GetByUsernameAsync("demoUser");

                await userService.RegisterUserAsync("otherUser", "hashed_password2", "other@example.com");
                var otherUser = await userRepo.GetByUsernameAsync("otherUser");

                var noodles = new Ingredient("Noodles");
                var dinner = new Category("Dinner");
                db.Ingredients.Add(noodles);
                db.Categories.Add(dinner);
                await db.SaveChangesAsync();

                var recipe = new Recipe("Pasta", user!.Id, "Simple pasta dish");
                recipe.AddStep("Boil water", 1);
                recipe.AddIngredient(noodles.Id, "200g");
                recipe.AddCategory(dinner.Id);

                await recipeService.AddRecipeAsync(recipe);
                await recipeRepo.SaveChangesAsync();
                Console.WriteLine($"Seeded recipe '{recipe.Name}' ({recipe.Id}) by {user.Username}.");

                // -------- Tests ----------------------------------------------------------
                results.Add(await ExpectException<InvalidOperationException>("Register duplicate username should fail",
                    () => userService.RegisterUserAsync("demoUser", "irrelevant", "dup@example.com")));

                results.Add(await ExpectException<InvalidOperationException>("Recipe must have at least 1 step",
                    async () =>
                    {
                        var r = new Recipe("NoStep", user.Id, "No steps here");
                        r.AddIngredient(noodles.Id, "50g");
                        r.AddCategory(dinner.Id);
                        await recipeService.AddRecipeAsync(r);
                    }));

                results.Add(await ExpectException<InvalidOperationException>("Recipe must have at least 1 ingredient",
                    async () =>
                    {
                        var r = new Recipe("NoIngredient", user.Id, "No ingredient here");
                        r.AddStep("Do something", 1);
                        r.AddCategory(dinner.Id);
                        await recipeService.AddRecipeAsync(r);
                    }));

                results.Add(await ExpectException<InvalidOperationException>("Recipe must have at least 1 category",
                    async () =>
                    {
                        var r = new Recipe("NoCategory", user.Id, "No category here");
                        r.AddStep("Do something", 1);
                        r.AddIngredient(noodles.Id, "10g");
                        await recipeService.AddRecipeAsync(r);
                    }));

                results.Add(await ExpectException<InvalidOperationException>("Duplicate recipe name fails (global unique)",
                    async () =>
                    {
                        var dup = new Recipe("Pasta", user.Id, "Duplicate name");
                        dup.AddStep("Boil water", 1);
                        dup.AddIngredient(noodles.Id, "100g");
                        dup.AddCategory(dinner.Id);
                        await recipeService.AddRecipeAsync(dup);
                    }));

                results.Add(await ExpectException<DbUpdateException>("Duplicate ingredient name fails (DB unique index)",
                    async () =>
                    {
                        using var ctx = new RecipeDbContext(
                            new DbContextOptionsBuilder<RecipeDbContext>().UseSqlite("Data Source=recipes_test.db").Options);
                        ctx.Ingredients.Add(new Ingredient("Noodles"));
                        await ctx.SaveChangesAsync();
                    }));

                results.Add(await ExpectException<DbUpdateException>("Duplicate category name fails (DB unique index)",
                    async () =>
                    {
                        using var ctx = new RecipeDbContext(
                            new DbContextOptionsBuilder<RecipeDbContext>().UseSqlite("Data Source=recipes_test.db").Options);
                        ctx.Categories.Add(new Category("Dinner"));
                        await ctx.SaveChangesAsync();
                    }));

                results.Add(await ExpectException<DbUpdateException>("Adding non-existent IngredientId should fail (FK)",
                    async () =>
                    {
                        var r = new Recipe("BadFK", user.Id, "Invalid FK test");
                        r.AddStep("Anything", 1);
                        r.AddCategory(dinner.Id);
                        r.AddIngredient(Guid.NewGuid(), "1 unit");
                        await recipeService.AddRecipeAsync(r);
                    }));

                results.Add(await ExpectException<DbUpdateException>("Duplicate step order within a recipe fails (unique index)",
                    async () =>
                    {
                        var r = new Recipe("DupOrder", user.Id, "Step order test");
                        r.AddStep("First", 1);
                        r.AddStep("Second but same order", 1);
                        r.AddIngredient(noodles.Id, "1 unit");
                        r.AddCategory(dinner.Id);
                        db.Recipes.Add(r);
                        await db.SaveChangesAsync();
                    }));

                results.Add(await ExpectException<InvalidOperationException>("User cannot favorite their own recipe",
                    () => favoriteService.AddFavoriteAsync(user.Id, recipe.Id)));

                // 11) FAVORITES and advanced recipe tests
                results.Add(await ExpectSuccess("Create other's recipe",
                    async () =>
                    {
                        // Re-attach global entities to ensure EF tracking consistency
                        db.Attach(noodles);
                        db.Attach(dinner);

                        var trackedUser = await db.Users.FirstAsync(u => u.Username == "otherUser");

                        var othersRecipe = new Recipe("Salad", trackedUser.Id, "Light & fresh");
                        othersRecipe.AddStep("Chop veggies", 1);
                        othersRecipe.AddIngredient(noodles.Id, "pinch");
                        othersRecipe.AddCategory(dinner.Id);

                        await recipeService.AddRecipeAsync(othersRecipe);
                        await recipeRepo.SaveChangesAsync();

                        Console.WriteLine($"Created other's recipe '{othersRecipe.Name}' ({othersRecipe.Id}) by {trackedUser.Username}.");
                    }));

                // 12) FAVORITES: a user can favorite someone else’s recipe
                results.Add(await ExpectSuccess("Favorite someone else's recipe (first time)",
                    () => favoriteService.AddFavoriteAsync(user.Id, db.Recipes.First(r => r.Name == "Salad").Id)));

                results.Add(await ExpectSuccess("Favorite is idempotent (second time no-op)",
                    () => favoriteService.AddFavoriteAsync(user.Id, db.Recipes.First(r => r.Name == "Salad").Id)));

                results.Add(await ExpectSuccess("Verify favorites list contains one entry",
                    async () =>
                    {
                        var favs = await favoriteRepo.GetFavoritesByUserAsync(user.Id);
                        var salad = await db.Recipes.FirstAsync(r => r.Name == "Salad");
                        if (!(favs.Any(r => r.Id == salad.Id) && favs.Count() == 1))
                            throw new Exception("Favorites list incorrect");
                    }));

                results.Add(await ExpectSuccess("Query by Ingredient returns expected recipe",
                    async () =>
                    {
                        var byIng = await recipeRepo.GetByIngredientAsync(noodles.Id);
                        if (!byIng.Any(r => r.Name == "Pasta") && !byIng.Any(r => r.Name == "Salad"))
                            throw new Exception("Query by Ingredient missing expected recipe(s)");
                    }));

                results.Add(await ExpectSuccess("Query by Category returns expected recipe",
                    async () =>
                    {
                        var byCat = await recipeRepo.GetByCategoryAsync(dinner.Id);
                        if (!byCat.Any(r => r.Name == "Pasta") && !byCat.Any(r => r.Name == "Salad"))
                            throw new Exception("Query by Category missing expected recipe(s)");
                    }));

                results.Add(await ExpectSuccess("Cascade delete should remove related steps & joins",
                    async () =>
                    {
                        var pasta = await db.Recipes.FirstAsync(r => r.Name == "Pasta");
                        await recipeRepo.DeleteAsync(pasta.Id);
                        await recipeRepo.SaveChangesAsync();

                        var stepsRemain = await db.Steps.AnyAsync(s => s.RecipeId == pasta.Id);
                        var linksIngRemain = await db.RecipeIngredients.AnyAsync(ri => ri.RecipeId == pasta.Id);
                        var linksCatRemain = await db.RecipeCategories.AnyAsync(rc => rc.RecipeId == pasta.Id);
                        var favsRemain = await db.Favorites.AnyAsync(f => f.RecipeId == pasta.Id);

                        if (stepsRemain || linksIngRemain || linksCatRemain || favsRemain)
                            throw new Exception("Cascade delete did not clear related rows");
                    }));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n🚨 Unhandled exception caught at top level: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
            }
            finally
            {
                // ---------- Summary ------------------------------------------------------
                int passed = results.Count(r => r.Status == TestStatus.Passed);
                int failed = results.Count(r => r.Status == TestStatus.Failed);
                int skipped = results.Count(r => r.Status == TestStatus.Skipped);
                int total = results.Count;

                Console.WriteLine("\n=== SUMMARY ===");
                foreach (var r in results)
                {
                    var symbol = r.Status switch
                    {
                        TestStatus.Passed => "✅",
                        TestStatus.Failed => "❌",
                        TestStatus.Skipped => "⚪",
                        _ => "-"
                    };
                    Console.WriteLine($"{symbol} {r.Name}" +
                        (r.Status == TestStatus.Failed ? $" → {r.Message}" : ""));
                }

                Console.WriteLine($"\nTotal: {total} tests executed.");
                Console.WriteLine($"✅ Passed: {passed}");
                Console.WriteLine($"❌ Failed: {failed}");
                Console.WriteLine($"⚪ Skipped: {skipped}");
                Console.WriteLine("=== TestHarness finished ===");
            }
        }

        // ---- Helper types and methods ----
        enum TestStatus { Passed, Failed, Skipped }
        record TestResult(string Name, TestStatus Status, string Message = "");

        static async Task<TestResult> ExpectSuccess(string name, Func<Task> action)
        {
            try
            {
                await action();
                Console.WriteLine($"✅ {name}");
                return new TestResult(name, TestStatus.Passed);
            }
            catch (Exception ex)
            {
                var msg = $"{ex.GetType().Name}: {ex.Message}";
                Console.WriteLine($"❌ {name} — unexpected: {msg}");
                if (ex.InnerException != null)
                    Console.WriteLine($"   Inner: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}");
                return new TestResult(name, TestStatus.Failed, msg);
            }
        }

        static async Task<TestResult> ExpectException<T>(string name, Func<Task> action) where T : Exception
        {
            try
            {
                await action();
                var msg = $"Expected {typeof(T).Name} but no exception thrown";
                Console.WriteLine($"❌ {name} — {msg}");
                return new TestResult(name, TestStatus.Failed, msg);
            }
            catch (Exception ex)
            {
                bool match = ex is T || ex.InnerException is T;
                if (match)
                {
                    Console.WriteLine($"✅ {name}");
                    return new TestResult(name, TestStatus.Passed);
                }
                else
                {
                    var msg = $"Expected {typeof(T).Name}, got {ex.GetType().Name}/{ex.InnerException?.GetType().Name}";
                    Console.WriteLine($"❌ {name} — {msg}");
                    return new TestResult(name, TestStatus.Failed, msg);
                }
            }
        }
    }
}
