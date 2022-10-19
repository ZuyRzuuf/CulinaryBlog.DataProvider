using System.IO;
using System.Threading.Tasks;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using RecipesDataProvider.Infrastructure.Database;
using RecipesDataProvider.API.Migrations;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;

public class DatabaseSetupFixture : IAsyncLifetime
{
    private readonly string _connectionString;
    private readonly ServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly MysqlContext _mysqlContext;

    private const string DatabaseName = "culinary_blog_test";
    
    public string ConnectionString => _connectionString;
    public MysqlContext MysqlTestContext => _mysqlContext;

    public DatabaseSetupFixture()
    {
        _configuration = ConfigurationProvider();
        _mysqlContext = new MysqlContext(_configuration);
        _connectionString = $"{_configuration.GetConnectionString("local")};database={DatabaseName}";
        _serviceProvider = ServicesProvider();
    }

    public async Task InitializeAsync()
    {
        CreateTestDatabase();
        RunMigrations();
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
            .AddSingleton<MysqlContext>()
            .AddSingleton<RecipesDatabase>()
            // based on Fluent Migrator docs
            // https://fluentmigrator.github.io/articles/guides/upgrades/guide-2.0-to-3.0.html?tabs=di
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(c => c.AddMySql5()
                .WithGlobalConnectionString(ConnectionString)
                .ScanIn(typeof(Program).Assembly).For.Migrations())
            .BuildServiceProvider();
    }

    private void CreateTestDatabase()
    {
        using var scope = _serviceProvider.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<RecipesDatabase>();

        databaseService.Create("culinary_blog_test");
    }

    private void RunMigrations()
    {
        using var scope = _serviceProvider.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        
        migrationService.ListMigrations();
        migrationService.MigrateUp();
    }
}