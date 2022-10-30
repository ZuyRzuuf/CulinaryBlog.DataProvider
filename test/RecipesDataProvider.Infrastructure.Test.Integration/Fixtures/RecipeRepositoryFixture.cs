using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Infrastructure.Repositories;
using RecipesDataProvider.Infrastructure.Test.Integration.Database.TestData;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;

public class RecipeRepositoryFixture : DatabaseSetupFixture, IDisposable
{
    public RecipeRepository Sut { get; }

    public readonly List<Recipe> RecipesCollection; 

    public RecipeRepositoryFixture() 
    {
        Sut = new RecipeRepository(MysqlTestContext);
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