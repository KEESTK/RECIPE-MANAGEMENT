namespace RecipeManagement.Domain.Entities;

public class Recipe
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; }
    public string? Description { get; private set; }

    public Guid AuthorId { get; private set; }
    public User Author { get; private set; } = null!;

    public List<Step> Steps { get; private set; } = new();
    public List<RecipeIngredient> RecipeIngredients { get; private set; } = new();
    public List<RecipeCategory> RecipeCategories { get; private set; } = new();

    // EF Core requirement
    private Recipe() { }

    public Recipe(string name, Guid authorId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Recipe name cannot be empty.", nameof(name));

        Name = name;
        AuthorId = authorId;
        Description = description;
    }

    // --- Domain Behavior Methods ---

    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    public void AddStep(string description, int order)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Step description cannot be empty.", nameof(description));

        Steps.Add(new Step(Id, order, description));
    }

    public void AddIngredient(Guid ingredientId, string? amount = null)
    {
        if (RecipeIngredients.Any(ri => ri.IngredientId == ingredientId))
            return;

        RecipeIngredients.Add(new RecipeIngredient(Id, ingredientId, amount));
    }

    public void AddCategory(Guid categoryId)
    {
        if (RecipeCategories.Any(rc => rc.CategoryId == categoryId))
            return;

        RecipeCategories.Add(new RecipeCategory(Id, categoryId));
    }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Name)
               && Steps.Count >= 1
               && RecipeIngredients.Count >= 1
               && RecipeCategories.Count >= 1;
    }
}
