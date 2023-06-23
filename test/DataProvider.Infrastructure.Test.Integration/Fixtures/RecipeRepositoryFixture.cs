using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using DataProvider.Domain.Entities;
using DataProvider.Infrastructure.Repositories;
using DataProvider.Infrastructure.Test.Integration.Database.TestData;
using Serilog;
using Serilog.Extensions.Logging;

namespace DataProvider.Infrastructure.Test.Integration.Fixtures;

public class RecipeRepositoryFixture : DatabaseSetupFixture, IDisposable
{
    public RecipeRepository Sut { get; }
    public ILogger<RecipeRepository> Logger { get; }
    public readonly List<Recipe> RecipesCollection; 

    public RecipeRepositoryFixture() 
    {
        var serilogLogger = new LoggerConfiguration()
            .CreateBootstrapLogger();

        Logger = new SerilogLoggerFactory(serilogLogger)
            .CreateLogger<RecipeRepository>();

        Sut = new RecipeRepository(MysqlTestContext, Logger);
        RecipesCollection = RecipesDataCollection.Recipes;
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