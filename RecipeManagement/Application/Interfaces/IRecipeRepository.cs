using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Application.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id);
    Task<Recipe?> GetByNameAsync(string name);
    Task<IEnumerable<Recipe>> GetByAuthorIdAsync(Guid authorId);
    Task<IEnumerable<Recipe>> GetByIngredientAsync(Guid ingredientId);
    Task<IEnumerable<Recipe>> GetByCategoryAsync(Guid categoryId);
    Task AddAsync(Recipe recipe);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}
