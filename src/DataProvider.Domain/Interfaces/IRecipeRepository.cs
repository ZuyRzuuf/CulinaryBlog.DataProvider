using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Queries;

namespace DataProvider.Domain.Interfaces;

public interface IRecipeRepository
{
    /// <summary>
    /// Get all recipes from database
    /// </summary>
    /// <returns>Recipes list</returns>
    public Task<IList<Recipe>> GetRecipes();
    /// <summary>
    /// Get all recipes from database by partial title
    /// </summary>
    /// <param name="partialTitle"></param>
    /// <returns>Recipes list</returns>
    public Task<IList<Recipe>> GetRecipesByTitle(string partialTitle);
    public Task<IList<Recipe>> GetBasicRecipesByTitle(string partialTitle);
    /// <summary>
    /// Get recipe with ingredients by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Recipe with ingredients</returns>
    public Task<RecipeQuery?> GetRecipeWithIngredientsById(Guid id);
    /// <summary>
    /// Get recipe by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Get recipe</returns>
    public Task<Recipe> GetRecipeById(Guid id);
    /// <summary>
    /// Create recipe 
    /// </summary>
    /// <param name="createRecipeDto"></param>
    /// <returns>Created recipe</returns>
    public Task<Recipe> CreateRecipe(CreateRecipeDto createRecipeDto);
    /// <summary>
    /// Update recipe
    /// </summary>
    /// <param name="updateRecipeDto"></param>
    /// <returns>int</returns>
    public Task<int> UpdateRecipe(UpdateRecipeDto updateRecipeDto);
    /// <summary>
    /// Delete recipe
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task<int> DeleteRecipe(Guid id);
}