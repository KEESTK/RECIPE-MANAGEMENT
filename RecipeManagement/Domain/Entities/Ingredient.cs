namespace RecipeManagement.Domain.Entities;

public class Ingredient
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; }

    public List<RecipeIngredient> RecipeIngredients { get; private set; } = new();

    // EF Core requirement
    private Ingredient() { }

    public Ingredient(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ingredient name cannot be empty.", nameof(name));

        Name = name;
    }

    // --- Domain Behavior Methods ---

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Ingredient name cannot be empty.", nameof(newName));

        Name = newName;
    }
}
