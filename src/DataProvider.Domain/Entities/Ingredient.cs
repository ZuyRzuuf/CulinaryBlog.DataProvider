namespace DataProvider.Domain.Entities;

public record Ingredient
{
    public Guid Id { get; init; }
    public Guid RecipeId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Quantity { get; init; }
    public string QuantityType { get; init; } = string.Empty;
}