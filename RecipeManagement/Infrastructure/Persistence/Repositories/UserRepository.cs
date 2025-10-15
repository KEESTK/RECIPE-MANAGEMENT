using Microsoft.EntityFrameworkCore;
using RecipeManagement.Application.Interfaces;
using RecipeManagement.Domain.Entities;

namespace RecipeManagement.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RecipeDbContext _context;

    public UserRepository(RecipeDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .Include(u => u.Recipes)
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.Recipes)
            .Include(u => u.Favorites)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
