using RecipesDataProvider.Domain.Dto;
using RecipesDataProvider.Domain.Entities;

namespace RecipesDataProvider.Domain.Interfaces;

public interface IRecipeRepository
{
    public Task<IList<Recipe>> GetRecipes();
    public Task<Recipe> GetRecipeByUuid(Guid uuid);
    public Task<Recipe> CreateRecipe(CreateRecipeDto createRecipeDto);
    public Task<int> UpdateRecipe(UpdateRecipeDto updateRecipeDto);
    public Task<int> DeleteRecipe(Guid uuid);
}