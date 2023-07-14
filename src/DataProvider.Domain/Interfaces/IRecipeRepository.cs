using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;

namespace DataProvider.Domain.Interfaces;

public interface IRecipeRepository
{
    public Task<IList<Recipe>> GetRecipes();
    public Task<IList<Recipe>> GetRecipesByTitle(string partialTitle);
    public Task<Recipe> GetRecipeById(Guid id);
    public Task<Recipe> CreateRecipe(CreateRecipeDto createRecipeDto);
    public Task<int> UpdateRecipe(UpdateRecipeDto updateRecipeDto);
    public Task<int> DeleteRecipe(Guid id);
}