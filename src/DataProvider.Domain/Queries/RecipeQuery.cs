using DataProvider.Domain.Entities;

namespace DataProvider.Domain.Queries;

public record RecipeQuery : Recipe
{
    public IEnumerable<IngredientQuery> Ingredients { get; set; } = new List<IngredientQuery>();
}