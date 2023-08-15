using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Queries;

namespace DataProvider.Domain.Interfaces;

public interface IRecipeRepository
{
    public Task<IList<Recipe>> GetRecipes();
    public Task<IList<Recipe>> GetRecipesByTitle(string partialTitle);
    public Task<IList<Recipe>> GetBasicRecipesByTitle(string partialTitle);
    public Task<RecipeQuery?> GetRecipeWithIngredientsById(Guid id);
    public Task<Recipe> GetRecipeById(Guid id);
    public Task<Recipe> CreateRecipe(CreateRecipeDto createRecipeDto);
    public Task<int> UpdateRecipe(UpdateRecipeDto updateRecipeDto);
    public Task<int> DeleteRecipe(Guid id);
}