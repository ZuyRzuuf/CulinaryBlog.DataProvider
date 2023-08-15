namespace DataProvider.Domain.Queries;

public record IngredientQuery
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Quantity { get; init; }
    public string QuantityType { get; init; } = string.Empty;
}