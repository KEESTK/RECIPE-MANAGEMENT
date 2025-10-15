namespace RecipeManagement.Domain.Entities;

public class Step
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid RecipeId { get; private set; }
    public Recipe Recipe { get; private set; } = null!;

    public int Order { get; private set; }
    public string Description { get; private set; }

    // EF Core requirement
    private Step() { }

    public Step(Guid recipeId, int order, string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Step description cannot be empty.", nameof(description));

        if (order <= 0)
            throw new ArgumentException("Step order must be greater than zero.", nameof(order));

        RecipeId = recipeId;
        Order = order;
        Description = description;
    }

    // --- Domain Behavior Methods ---

    public void UpdateDescription(string newDescription)
    {
        if (string.IsNullOrWhiteSpace(newDescription))
            throw new ArgumentException("Step description cannot be empty.", nameof(newDescription));

        Description = newDescription;
    }

    public void ChangeOrder(int newOrder)
    {
        if (newOrder <= 0)
            throw new ArgumentException("Step order must be greater than zero.", nameof(newOrder));

        Order = newOrder;
    }
}
