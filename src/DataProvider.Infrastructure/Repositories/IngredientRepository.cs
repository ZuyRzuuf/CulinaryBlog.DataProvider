using Dapper;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using DataProvider.Infrastructure.Database;
using DataProvider.Infrastructure.Exceptions;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace DataProvider.Infrastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly MysqlContext _mysqlContext;
    private readonly ILogger<IngredientRepository> _logger;
    
    public IngredientRepository(MysqlContext mysqlContext, ILogger<IngredientRepository> logger)
    {
        _mysqlContext = mysqlContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IList<Ingredient>> GetRecipeIngredientsByRecipeIdAsync(Guid recipeId)
    {
        try
        {
            const string query = "SELECT * FROM recipe_ingredient WHERE recipe_id = @recipeId";

            var parameters = new DynamicParameters();

            parameters.Add("@recipeId", recipeId);

            using var connection = _mysqlContext.CreateConnection();

            var result = await connection.QueryAsync<Ingredient>(query, parameters);
            var ingredients = result.ToList();

            if (ingredients.Any()) return ingredients;
            
            _logger.LogError("No ingredients found for recipe id {RecipeId}", recipeId);
            throw new NoIngredientsFoundException($"No ingredients found for recipe id {recipeId}");
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.InnerException, "Unknown database");
            throw new UnknownDatabaseException(e.Message, e.InnerException);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while getting recipe ingredients by recipe id");
            throw;
        }
    }
}