using Dapper;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Domain.Interfaces;
using RecipesDataProvider.Infrastructure.Database;

namespace RecipesDataProvider.Infrastructure.Repositories;

public class RecipeRepository : IRecipeRepository
{
    private readonly MysqlContext _mysqlContext;

    public RecipeRepository(MysqlContext mysqlContext)
    {
        _mysqlContext = mysqlContext;
    }

    public async Task<IList<Recipe>> GetRecipes()
    {
        const string query = "SELECT * FROM recipe";

        using var connection = _mysqlContext.CreateConnection();
        var recipes = await connection.QueryAsync<Recipe>(query);

        return recipes.ToList();
    }
}