using System.IO;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RecipesDataProvider.API.Migrations;
using RecipesDataProvider.Infrastructure.Database;
using Xunit;

namespace RecipesDataProvider.API.Test.Integration.Fixtures;

public class DatabaseSetupFixture : IAsyncLifetime
{
    private readonly string _connectionStringToDatabase;
    private readonly string _connectionStringToSchema;
    private readonly IConfiguration _configuration;
    private readonly MysqlContext _mysqlContext;

    private const string DatabaseName = "culinary_blog_test";
    
    public string ConnectionStringToDatabase => _connectionStringToDatabase;
    public string ConnectionStringToSchema => _connectionStringToSchema;
    protected MysqlContext MysqlTestContext { get; }
    public MysqlContext MysqlTestContextWithoutSchema { get; }

    public DatabaseSetupFixture()
    {
        _configuration = ConfigurationProvider();
        MysqlTestContext = new MysqlContext(_configuration);
        MysqlTestContextWithoutSchema = new MysqlContext(_configuration, "database");
        _connectionStringToDatabase = _configuration.GetConnectionString("database");
        _connectionStringToSchema = _configuration.GetConnectionString("schema");
    }

    public virtual async Task InitializeAsync()
    {
        CreateTestDatabase();
        RunMigrations();

        await Task.CompletedTask;
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
    
    private void CreateTestDatabase()
    {
        var serviceProvider = new ServiceCollection()
            .AddScoped(_ => _configuration)
            .AddSingleton(sp => 
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
}