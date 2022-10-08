using RecipesDataProvider.Domain.Entities;

namespace RecipesDataProvider.Domain.Interfaces;

public interface IRecipeRepository
{
    public Task<IList<Recipe>> GetRecipes();
}