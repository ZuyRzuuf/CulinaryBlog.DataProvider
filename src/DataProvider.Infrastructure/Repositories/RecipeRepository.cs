using System.Data;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using DataProvider.Infrastructure.Database;
using DataProvider.Infrastructure.Exceptions;

namespace DataProvider.Infrastructure.Repositories;

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

            return recipes.ToList();
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.InnerException, "Unknown database");
            throw new UnknownDatabaseException(e.Message, e.InnerException);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem with database connection occurs");
            throw new DatabaseConnectionProblemException(e.Message, e.InnerException);
        }
    }

    public async Task<Recipe> GetRecipeByUuid(Guid uuid)
    {
        try
        {
            const string query = "SELECT * FROM recipe WHERE uuid = @Uuid";

            var parameters = new DynamicParameters();
            
            parameters.Add("Uuid", uuid, DbType.Guid);

            using var connection = _mysqlContext.CreateConnection();

            var result = await connection.QueryAsync<Recipe>(query, parameters);
            var recipe = result.SingleOrDefault();

            if (recipe == null)
                throw new RecipeDoesNotExistException();

            return recipe;
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@Uuid}' doesn't exist", uuid);
            throw;
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.InnerException, "Unknown database");
            throw new UnknownDatabaseException(e.Message, e.InnerException);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem with database connection occurs");
            throw new DatabaseConnectionProblemException(e.Message, e.InnerException);
        }
    }

    public async Task<IList<Recipe>> GetRecipesByTitle(string partialTitle)
    {
        try
        {
            const string query = "SELECT * FROM recipe WHERE title LIKE @PartialTitle";

            var parameters = new DynamicParameters();
            
            parameters.Add("PartialTitle", "%" + partialTitle + "%", DbType.String);

            using var connection = _mysqlContext.CreateConnection();

            var result = await connection.QueryAsync<Recipe>(query, parameters);

            return result.ToList();
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.InnerException, "Unknown database");
            throw new UnknownDatabaseException(e.Message, e.InnerException);
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
        catch (MySqlException e)
        {
            if (e.Message.Contains("Duplicate entry"))
            {
                _logger.LogError(e, "Recipe name has to be unique. Recipe {@Title} exists", createRecipeDto.Title);
                throw new RecipeHasToBeUniqueException(e.Message, e.InnerException);
            }
            
            _logger.LogError(e.InnerException, "Unknown database");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem with database connection occurs");
            throw new DatabaseConnectionProblemException(e.Message, e.InnerException);
        }
    }

    public async Task<int> UpdateRecipe(UpdateRecipeDto updateRecipeDto)
    {
        try
        {
            const string query = "UPDATE recipe SET title = @Title WHERE uuid = @Uuid";

            var uuid = updateRecipeDto.Uuid;
            var title = updateRecipeDto.Title;
            var parameters = new DynamicParameters();
        
            parameters.Add("Uuid", uuid);
            parameters.Add("Title", title);

            using var connection = _mysqlContext.CreateConnection();
        
            var result = await connection.ExecuteAsync(query, parameters);

            if (result != 0) return result;
            
            throw new RecipeDoesNotExistException();
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@Recipe}' doesn't exist", updateRecipeDto.Title);
            throw;
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.InnerException, "Unknown database");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem with database connection occurs");
            throw new DatabaseConnectionProblemException(e.Message, e.InnerException);
        }
    }

    public async Task<int> DeleteRecipe(Guid uuid)
    {
        try
        {
            const string query = "DELETE FROM recipe WHERE uuid = @Uuid";

            var parameters = new DynamicParameters();
            parameters.Add("Uuid", uuid, DbType.Guid);

            using var connection = _mysqlContext.CreateConnection();

            var result = await connection.ExecuteAsync(query, parameters);

            if (result == 1) return result;
            
            throw new RecipeDoesNotExistException();
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@Uuid}' doesn't exist", uuid);
            throw;
        }
        catch (MySqlException e)
        {
            _logger.LogError(e.InnerException, "Unknown database");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Problem with database connection occurs");
            throw new DatabaseConnectionProblemException(e.Message, e.InnerException);
        }
    }
}