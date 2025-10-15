namespace RecipeManagement.Domain.Entities;

public class RecipeIngredient
{
    public Guid RecipeId { get; private set; }
    public Recipe Recipe { get; private set; } = null!;

    public Guid IngredientId { get; private set; }
    public Ingredient Ingredient { get; private set; } = null!;

    public string? Amount { get; private set; }

    // EF Core requirement
    private RecipeIngredient() { }

    public RecipeIngredient(Guid recipeId, Guid ingredientId, string? amount = null)
    {
        if (recipeId == Guid.Empty)
            throw new ArgumentException("RecipeId cannot be empty.", nameof(recipeId));

        if (ingredientId == Guid.Empty)
            throw new ArgumentException("IngredientId cannot be empty.", nameof(ingredientId));

        RecipeId = recipeId;
        IngredientId = ingredientId;
        Amount = amount;
    }

    // --- Domain Behavior Methods ---

    public void UpdateAmount(string? newAmount)
    {
        Amount = newAmount;
    }
}
