using RecipeManagement.Domain.Entities;
using RecipeManagement.Application.Interfaces;

namespace RecipeManagement.Domain.Services;

public class RecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IUserRepository _userRepository;

    public RecipeService(IRecipeRepository recipeRepository, IUserRepository userRepository)
    {
        _recipeRepository = recipeRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Adds a new recipe after validating all business rules.
    /// </summary>
    public async Task AddRecipeAsync(Recipe recipe)
    {
        // Rule: Only registered users can manage recipes
        var author = await _userRepository.GetByIdAsync(recipe.AuthorId);
        if (author is null)
            throw new InvalidOperationException("Only registered users can create recipes.");

        // Rule: Recipe name must be globally unique
        var existing = await _recipeRepository.GetByNameAsync(recipe.Name);
        if (existing is not null)
            throw new InvalidOperationException($"A recipe with the name '{recipe.Name}' already exists.");

        // Rule: Must have at least one step, ingredient, and category
        if (!recipe.IsValid())
            throw new InvalidOperationException("Recipe must have at least one step, ingredient, and category.");

        await _recipeRepository.AddAsync(recipe);
        await _recipeRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a recipe by ID if it exists.
    /// </summary>
    public async Task DeleteRecipeAsync(Guid recipeId)
    {
        var recipe = await _recipeRepository.GetByIdAsync(recipeId);
        if (recipe is null)
            throw new KeyNotFoundException("Recipe not found.");

        await _recipeRepository.DeleteAsync(recipeId);
        await _recipeRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all recipes of a given user.
    /// </summary>
    public async Task<IEnumerable<Recipe>> GetRecipesByUserAsync(Guid userId)
    {
        return await _recipeRepository.GetByAuthorIdAsync(userId);
    }

    /// <summary>
    /// Retrieves all recipes that contain a specific ingredient.
    /// </summary>
    public async Task<IEnumerable<Recipe>> GetRecipesByIngredientAsync(Guid ingredientId)
    {
        return await _recipeRepository.GetByIngredientAsync(ingredientId);
    }

    /// <summary>
    /// Retrieves all recipes within a specific category.
    /// </summary>
    public async Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(Guid categoryId)
    {
        return await _recipeRepository.GetByCategoryAsync(categoryId);
    }
}
