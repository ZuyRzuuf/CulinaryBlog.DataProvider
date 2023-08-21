using DataProvider.Domain.Entities;

namespace DataProvider.Domain.Interfaces;

public interface IIngredientRepository
{
    /// <summary>
    /// Get all recipe ingredients from database by recipe id
    /// </summary>
    /// <param name="recipeId"></param>
    /// <returns>Recipe ingredients list</returns>
    public Task<IList<Ingredient>> GetRecipeIngredientsByRecipeIdAsync(Guid recipeId);
}