namespace RecipeManagement.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Username { get; private set; }
    public string? Email { get; private set; }
    public string PasswordHash { get; private set; }

    // Navigation properties
    public List<Recipe> Recipes { get; private set; } = new();
    public List<Favorite> Favorites { get; private set; } = new();

    // EF Core requires a parameterless constructor
    private User() { }

    public User(string username, string passwordHash, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty.", nameof(passwordHash));

        Username = username;
        PasswordHash = passwordHash;
        Email = email;
    }

    // Domain methods
    public void ChangeEmail(string newEmail)
    {
        Email = newEmail;
    }

    public void AddFavorite(Favorite favorite)
    {
        if (!Favorites.Any(f => f.RecipeId == favorite.RecipeId))
            Favorites.Add(favorite);
    }

    public void RemoveFavorite(Guid recipeId)
    {
        Favorites.RemoveAll(f => f.RecipeId == recipeId);
    }
}
