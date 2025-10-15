namespace RecipeManagement.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; }

    public List<RecipeCategory> RecipeCategories { get; private set; } = new();

    // EF Core requirement
    private Category() { }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        Name = name;
    }

    // --- Domain Behavior Methods ---

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Category name cannot be empty.", nameof(newName));

        Name = newName;
    }
}
