using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using DataProvider.API.Controllers;
using DataProvider.API.Test.Integration.Database.TestData;
using DataProvider.Domain.Entities;
using DataProvider.Infrastructure.Repositories;
using Serilog;
using Serilog.Extensions.Logging;

namespace DataProvider.API.Test.Integration.Fixtures;

public class RecipeControllerFixture : DatabaseSetupFixture, IDisposable
{
    public RecipeRepository RecipeRepository { get; }
    public RecipeRepository RecipeRepositoryThrowingException { get; }                                                                                                                                                                          
    public ILogger<RecipeController> RecipeControllerLogger { get; }
    public ILogger<RecipeRepository> RecipeRepositoryLogger { get; }
    public List<Recipe> RecipesCollection { get; }
    
    public RecipeControllerFixture() 
    {
        var serilogLogger = new LoggerConfiguration()
            .CreateBootstrapLogger();

        RecipeControllerLogger = new SerilogLoggerFactory(serilogLogger)
            .CreateLogger<RecipeController>();
        RecipeRepositoryLogger = new SerilogLoggerFactory(serilogLogger)
            .CreateLogger<RecipeRepository>();

        RecipesCollection = RecipesDataCollection.Recipes;
        RecipeRepositoryThrowingException = new RecipeRepository(MysqlTestContextWithoutSchema, RecipeRepositoryLogger);
        RecipeRepository = new RecipeRepository(MysqlTestContext, RecipeRepositoryLogger);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        await AddRecipesData();
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
    }

    private async Task AddRecipesData()
    {
        const string recipeQuery = "INSERT INTO recipe (id, title) VALUES (@Id, @Title)";
        const string ingredientQuery = """
           INSERT INTO recipe_ingredient
               (id, recipe_id, name, description, quantity, quantity_type)
           VALUES (@Id, @RecipeId, @Name, @Description, @Quantity, @QuantityType)
       """;

        var recipeIngredients = new List<Ingredient>();

        foreach (var recipe in RecipesCollection)
        {
            recipeIngredients.AddRange(new RecipeIngredientsDataCollection(recipe.Id).Ingredients);
        }

        await using var connection = new MySqlConnection(ConnectionStringToSchema);
        await connection.ExecuteAsync(recipeQuery, RecipesCollection);
        await connection.ExecuteAsync(ingredientQuery, recipeIngredients);
    }
}