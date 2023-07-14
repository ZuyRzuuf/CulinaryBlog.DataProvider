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

    public async Task<Recipe> GetRecipeById(Guid id)
    {
        try
        {
            const string query = "SELECT * FROM recipe WHERE id = @Id";

            var parameters = new DynamicParameters();
            
            parameters.Add("Id", id, DbType.Guid);

            using var connection = _mysqlContext.CreateConnection();

            var result = await connection.QueryAsync<Recipe>(query, parameters);
            var recipe = result.SingleOrDefault();

            if (recipe == null)
                throw new RecipeDoesNotExistException();

            return recipe;
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@Id}' doesn't exist", id);
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
            const string query = "INSERT INTO recipe (id, title) VALUES (@Id, @Title)";
        
            var id = Guid.NewGuid();
            var title = createRecipeDto.Title;
            var parameters = new DynamicParameters();
        
            parameters.Add("Id", id);
            parameters.Add("Title", title);

            using var connection = _mysqlContext.CreateConnection();
        
            await connection.ExecuteAsync(query, parameters);

            return new Recipe
            {
                Id = id,
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
            const string query = "UPDATE recipe SET title = @Title WHERE id = @Id";

            var id = updateRecipeDto.Id;
            var title = updateRecipeDto.Title;
            var parameters = new DynamicParameters();
        
            parameters.Add("Id", id);
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

    public async Task<int> DeleteRecipe(Guid id)
    {
        try
        {
            const string query = "DELETE FROM recipe WHERE id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Guid);

            using var connection = _mysqlContext.CreateConnection();

            var result = await connection.ExecuteAsync(query, parameters);

            if (result == 1) return result;
            
            throw new RecipeDoesNotExistException();
        }
        catch (RecipeDoesNotExistException e)
        {
            _logger.LogError(e.InnerException, "Recipe '{@Id}' doesn't exist", id);
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