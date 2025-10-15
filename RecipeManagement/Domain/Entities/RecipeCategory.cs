namespace RecipeManagement.Domain.Entities;

public class RecipeCategory
{
    public Guid RecipeId { get; private set; }
    public Recipe Recipe { get; private set; } = null!;

    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    // EF Core requirement
    private RecipeCategory() { }

    public RecipeCategory(Guid recipeId, Guid categoryId)
    {
        if (recipeId == Guid.Empty)
            throw new ArgumentException("RecipeId cannot be empty.", nameof(recipeId));

        if (categoryId == Guid.Empty)
            throw new ArgumentException("CategoryId cannot be empty.", nameof(categoryId));

        RecipeId = recipeId;
        CategoryId = categoryId;
    }
}
