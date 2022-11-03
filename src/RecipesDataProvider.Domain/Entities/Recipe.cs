namespace RecipesDataProvider.Domain.Entities;

public class Recipe
{
    public Guid Uuid { get; set; }
    public string? Title { get; set; } = null!;
}