using RecipeManagement.Domain.Entities;
using RecipeManagement.Application.Interfaces;

namespace RecipeManagement.Domain.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Registers a new user with unique username.
    /// </summary>
    public async Task RegisterUserAsync(string username, string passwordHash, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        // Rule: Username must be unique
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser is not null)
            throw new InvalidOperationException($"The username '{username}' is already taken.");

        var user = new User(username, passwordHash, email);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves a user by their ID.
    /// </summary>
    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        return await _userRepository.GetByUsernameAsync(username);
    }

    /// <summary>
    /// Updates a user's email address.
    /// </summary>
    public async Task UpdateEmailAsync(Guid userId, string newEmail)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        user.ChangeEmail(newEmail);
        await _userRepository.SaveChangesAsync();
    }
}
