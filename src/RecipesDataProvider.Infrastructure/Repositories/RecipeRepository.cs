using Dapper;
using Microsoft.Extensions.Logging;
using RecipesDataProvider.Domain.Dto;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Domain.Interfaces;
using RecipesDataProvider.Infrastructure.Database;
using RecipesDataProvider.Infrastructure.Exceptions;

namespace RecipesDataProvider.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly MysqlContext _mysqlContext;
    private readonly ILogger<RecipeRepository> _logger;

    public RecipeRepository(MysqlContext mysqlContext, ILogger<RecipeRepository> logger)
    {
        _mysqlContext = mysqlContext;
        _logger = logger;
    }

    public async Task<IList<Recipe>> GetRecipes()
    {
        try
        {
            const string query = "SELECT * FROM recipe";

            using var connection = _mysqlContext.CreateConnection();
            var recipes = await connection.QueryAsync<Recipe>(query);

            var temporaryRecipesList = recipes.ToList();
            return temporaryRecipesList;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem with database connection occurs");
            throw new DatabaseConnectionProblemException(e.Message, e.InnerException);
        }
    }

    public async Task<Recipe> CreateRecipe(CreateRecipeDto createRecipeDto)
    {
        try
        {
            const string query = "INSERT INTO recipe (uuid, title) VALUES (@Uuid, @Title)";
        
            var uuid = Guid.NewGuid();
            var title = createRecipeDto.Title;
            var parameters = new DynamicParameters();
        
            parameters.Add("Uuid", uuid);
            parameters.Add("Title", title);

            using var connection = _mysqlContext.CreateConnection();
        
            await connection.ExecuteAsync(query, parameters);

            return new Recipe
            {
                Uuid = uuid,
                Title = title
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem with database connection occurs");
            throw new DatabaseConnectionProblemException(e.Message, e.InnerException);
        }
    }
}