using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using RecipesDataProvider.API.Controllers;
using RecipesDataProvider.API.Test.Integration.Database.TestData;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Infrastructure.Repositories;
using Serilog;
using Serilog.Extensions.Logging;

namespace RecipesDataProvider.API.Test.Integration.Fixtures;

public class RecipeControllerFixture : DatabaseSetupFixture, IDisposable
{
    public RecipeRepository RecipeRepository { get; }
    public RecipeRepository RecipeRepositoryThrowingException { get; }                                                                                                                                                                          
    public ILogger<RecipeController> Logger { get; }
    public List<Recipe> RecipesCollection { get; }
    
    public RecipeControllerFixture() 
    {
        RecipesCollection = RecipesDataCollection.Recipes;
        RecipeRepositoryThrowingException = new RecipeRepository(MysqlTestContextWithoutSchema);
        RecipeRepository = new RecipeRepository(MysqlTestContext);
        
        var serilogLogger = new LoggerConfiguration()
            .CreateBootstrapLogger();

        Logger = new SerilogLoggerFactory(serilogLogger)
            .CreateLogger<RecipeController>();
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
        const string query = "INSERT INTO recipe (uuid, title) VALUES (@Uuid, @Title)";

        await using var connection = new MySqlConnection(ConnectionStringToSchema);
        await connection.ExecuteAsync(query, RecipesCollection);
    }
}