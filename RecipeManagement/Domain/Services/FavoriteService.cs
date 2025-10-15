using RecipeManagement.Domain.Entities;
using RecipeManagement.Application.Interfaces;

namespace RecipeManagement.Domain.Services;

public class FavoriteService
{
    private readonly IUserRepository _userRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly IFavoriteRepository _favoriteRepository;

    public FavoriteService(
        IUserRepository userRepository,
        IRecipeRepository recipeRepository,
        IFavoriteRepository favoriteRepository)
    {
        _userRepository = userRepository;
        _recipeRepository = recipeRepository;
        _favoriteRepository = favoriteRepository;
    }

    /// <summary>
    /// Marks a recipe as a user's favorite (if not already favorited).
    /// </summary>
    public async Task AddFavoriteAsync(Guid userId, Guid recipeId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User must be registered to add favorites.");

        var recipe = await _recipeRepository.GetByIdAsync(recipeId)
            ?? throw new InvalidOperationException("Recipe not found.");

        // Rule: A user cannot favorite their own recipe
        if (recipe.AuthorId == user.Id)
            throw new InvalidOperationException("A user cannot favorite their own recipe.");

        var existingFavorite = await _favoriteRepository.GetAsync(userId, recipeId);
        if (existingFavorite is not null)
            return; // Already favorited, no action needed

        var favorite = new Favorite(userId, recipeId);
        await _favoriteRepository.AddAsync(favorite);
        await _favoriteRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a recipe from a user's favorites.
    /// </summary>
    public async Task RemoveFavoriteAsync(Guid userId, Guid recipeId)
    {
        var favorite = await _favoriteRepository.GetAsync(userId, recipeId);
        if (favorite is null)
            return;

        await _favoriteRepository.DeleteAsync(userId, recipeId);
        await _favoriteRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Returns all favorite recipes of a user.
    /// </summary>
    public async Task<IEnumerable<Recipe>> GetFavoritesByUserAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User must be registered.");

        return await _favoriteRepository.GetFavoritesByUserAsync(userId);
    }
}
