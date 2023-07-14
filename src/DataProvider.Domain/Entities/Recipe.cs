namespace DataProvider.Domain.Entities;

public class Recipe
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = null!;
}