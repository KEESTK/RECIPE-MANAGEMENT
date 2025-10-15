using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Application.Interfaces;

public interface IFavoriteRepository
{
    Task<Favorite?> GetAsync(Guid userId, Guid recipeId);
    Task<IEnumerable<Recipe>> GetFavoritesByUserAsync(Guid userId);
    Task AddAsync(Favorite favorite);
    Task DeleteAsync(Guid userId, Guid recipeId);
    Task SaveChangesAsync();
}
