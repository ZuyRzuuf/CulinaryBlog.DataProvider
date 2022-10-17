using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;

public class DatabaseSetupFixture : IAsyncLifetime
{
    private const string InitScriptPath = "database_create.sql";
    private const string DatabaseName = "culinary_blog_test";
    
    private readonly string _directory;
    private readonly string _connectionString;

    public string ConnectionString => _connectionString;

    public DatabaseSetupFixture()
    {
        _directory = Directory.GetCurrentDirectory();
        _connectionString = $"server=localhost;port=8084;userid=test;password=test;database={DatabaseName};";
        // _connectionString = $"server=localhost;port=8083;userid=root;password=root;database=culinary_blog;";
    }

    public async Task InitializeAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);

        await connection.OpenAsync();

        try
        {
            var script = new MySqlScript(connection)
            {
                Query = await File.ReadAllTextAsync($"{_directory}/{InitScriptPath}")
            };
            
            script.Error += async (_, args) =>
            {
                // try to drop database on error
                await DisposeAsync();
                throw args.Exception;
            };

            await script.ExecuteAsync();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await using var connection = new MySqlConnection(_connectionString);

        await connection.OpenAsync();
        
        try
        {
            await using MySqlCommand command = connection.CreateCommand();
            command.CommandText = $"drop database if exists {DatabaseName}";
            await command.ExecuteNonQueryAsync();
        }
        finally
        {
            await connection.CloseAsync();
        }
    }
}