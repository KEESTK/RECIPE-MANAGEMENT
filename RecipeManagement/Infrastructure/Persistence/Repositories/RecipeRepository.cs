using Microsoft.EntityFrameworkCore;
using RecipeManagement.Application.Interfaces;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly RecipeDbContext _context;

    public RecipeRepository(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<Recipe?> GetByIdAsync(Guid id)
    {
        return await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Recipe?> GetByNameAsync(string name)
    {
        return await _context.Recipes
            .Include(r => r.Author)
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .FirstOrDefaultAsync(r => r.Name == name);
    }

    public async Task<IEnumerable<Recipe>> GetByAuthorIdAsync(Guid authorId)
    {
        return await _context.Recipes
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .Where(r => r.AuthorId == authorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByIngredientAsync(Guid ingredientId)
    {
        return await _context.Recipes
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .Where(r => r.RecipeIngredients.Any(ri => ri.IngredientId == ingredientId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Recipe>> GetByCategoryAsync(Guid categoryId)
    {
        return await _context.Recipes
            .Include(r => r.Steps)
            .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeCategories)
                .ThenInclude(rc => rc.Category)
            .Where(r => r.RecipeCategories.Any(rc => rc.CategoryId == categoryId))
            .ToListAsync();
    }

    public async Task AddAsync(Recipe recipe)
    {
        await _context.Recipes.AddAsync(recipe);
    }

    public async Task DeleteAsync(Guid id)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe is not null)
            _context.Recipes.Remove(recipe);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
