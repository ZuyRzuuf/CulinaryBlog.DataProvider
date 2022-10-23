using System.IO;
using System.Threading.Tasks;
using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RecipesDataProvider.Infrastructure.Database;
using RecipesDataProvider.API.Migrations;
using RecipesDataProvider.Infrastructure.Test.Integration.Database.TestData;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;

public class DatabaseSetupFixture : IAsyncLifetime
{
    private readonly string _connectionStringToDatabase;
    private readonly string _connectionStringToSchema;
    private readonly IConfiguration _configuration;
    private readonly MysqlContext _mysqlContext;

    private const string DatabaseName = "culinary_blog_test";
    
    public string ConnectionStringToDatabase => _connectionStringToDatabase;
    public string ConnectionStringToSchema => _connectionStringToSchema;
    protected MysqlContext MysqlTestContext => _mysqlContext;

    public DatabaseSetupFixture()
    {
        _configuration = ConfigurationProvider();
        _mysqlContext = new MysqlContext(_configuration);
        _connectionStringToDatabase = _configuration.GetConnectionString("database");
        _connectionStringToSchema = _configuration.GetConnectionString("schema");
        ServicesProvider();
    }

    public async Task InitializeAsync()
    {
        CreateTestDatabase();
        RunMigrations();
        await AddRecipesData();
    }

    public async Task DisposeAsync()
    {
        await using var connection = new MySqlConnection(_connectionStringToSchema);

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

    private static IConfiguration ConfigurationProvider()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@"appsettings.test.json", false, false)
            .AddEnvironmentVariables()
            .Build();
    }
    
    private ServiceProvider ServicesProvider()
    {
        return new ServiceCollection()
            .AddScoped<IConfiguration>(_ => _configuration)
            .AddSingleton<MysqlContext>(sp => 
                new MysqlContext(
                    sp.GetRequiredService<IConfiguration>(), 
                    "database"))
            .AddSingleton<RecipesDatabase>()
            // based on Fluent Migrator docs
            // https://fluentmigrator.github.io/articles/guides/upgrades/guide-2.0-to-3.0.html?tabs=di
            .AddLogging(lb => lb
                .AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(mrb => mrb
                .AddMySql5()
                .WithGlobalConnectionString(ConnectionStringToSchema)
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            .BuildServiceProvider();
    }

    private void CreateTestDatabase()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped<IConfiguration>(_ => _configuration)
            .AddSingleton<MysqlContext>(sp => 
                new MysqlContext(
                    sp.GetRequiredService<IConfiguration>(), 
                    "database"))
            .AddSingleton<RecipesDatabase>()
            .BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<RecipesDatabase>();

        databaseService.Create(DatabaseName);
    }

    private void RunMigrations()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(lb => lb
                .AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(
                builder => builder
                    .AddMySql5()
                    .WithGlobalConnectionString(ConnectionStringToSchema)
                    .ScanIn(typeof(Program).Assembly).For.Migrations())
            .BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        
        migrationService.ListMigrations();
        migrationService.MigrateUp();
    }

    private async Task AddRecipesData()
    {
        const string query = "INSERT INTO recipe (uuid, title) VALUES (@Uuid, @Title)";
        var recipes = RecipesDataCollection.Recipes;

        await using var connection = new MySqlConnection(_connectionStringToSchema);
        await connection.ExecuteAsync(query, recipes);
    }
}