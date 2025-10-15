namespace RecipeManagement.Domain.Entities;

public class Favorite
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid RecipeId { get; private set; }
    public Recipe Recipe { get; private set; } = null!;

    // Optional: Track when a recipe was favorited
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    // EF Core requirement
    private Favorite() { }

    public Favorite(Guid userId, Guid recipeId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));

        if (recipeId == Guid.Empty)
            throw new ArgumentException("RecipeId cannot be empty.", nameof(recipeId));

        UserId = userId;
        RecipeId = recipeId;
    }
}
