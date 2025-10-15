using Microsoft.EntityFrameworkCore;
using RecipeManagement.Application.Interfaces;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly RecipeDbContext _context;

    public FavoriteRepository(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<Favorite?> GetAsync(Guid userId, Guid recipeId)
    {
        return await _context.Favorites
            .Include(f => f.User)
            .Include(f => f.Recipe)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);
    }

    public async Task<IEnumerable<Recipe>> GetFavoritesByUserAsync(Guid userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .Include(f => f.Recipe)
                .ThenInclude(r => r.Author)
            .Include(f => f.Recipe)
                .ThenInclude(r => r.Steps)
            .Include(f => f.Recipe)
                .ThenInclude(r => r.RecipeIngredients)
                    .ThenInclude(ri => ri.Ingredient)
            .Include(f => f.Recipe)
                .ThenInclude(r => r.RecipeCategories)
                    .ThenInclude(rc => rc.Category)
            .Select(f => f.Recipe)
            .ToListAsync();
    }

    public async Task AddAsync(Favorite favorite)
    {
        await _context.Favorites.AddAsync(favorite);
    }

    public async Task DeleteAsync(Guid userId, Guid recipeId)
    {
        var favorite = await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.RecipeId == recipeId);

        if (favorite is not null)
            _context.Favorites.Remove(favorite);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
