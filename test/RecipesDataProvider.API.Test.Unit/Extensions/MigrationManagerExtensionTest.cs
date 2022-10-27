using FluentAssertions.Execution;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using RecipesDataProvider.API.Extensions;
using RecipesDataProvider.Infrastructure.Interfaces;
using Xunit;

namespace RecipesDataProvider.API.Test.Unit.Extensions;

public class MigrationManagerExtensionTest
{
    private readonly Mock<IMigrationRunner> _migrationRunnerMock;
    private readonly Mock<IRecipesDatabase> _recipeDatabaseMock;
    private readonly Mock<IHost> _hostMock;

    public MigrationManagerExtensionTest()
    {
        _migrationRunnerMock = new Mock<IMigrationRunner>();
        _recipeDatabaseMock = new Mock<IRecipesDatabase>();
        _hostMock = new Mock<IHost>();

        _hostMock
            .Setup(h => h.Services)
            .Returns(() => {
                return new ServiceCollection()
                    .AddSingleton(_recipeDatabaseMock.Object)
                    .AddScoped(_ => _migrationRunnerMock.Object)
                    .BuildServiceProvider();
            });

    }
    
    [Fact]
    public void MigrateDatabase_RunsMethodsCreateWithDefaultDatabaseNameAndListMigrationsAndMigrateUpOnce()
    {
        _hostMock.Object.MigrateDatabase();
        
        using var assertionScope = new AssertionScope();
            _recipeDatabaseMock.Verify(s => s.Create("culinary_blog"), Times.Once);
            _migrationRunnerMock.Verify(s => s.ListMigrations(), Times.Once);
            _migrationRunnerMock.Verify(s => s.MigrateUp(), Times.Once);
            _migrationRunnerMock.Verify(s => s.MigrateDown(123), Times.Never);
    }
    
    [Fact]
    public void MigrateDatabase_RunsMethodsCreateWithCustomDatabaseNameAndListMigrationsAndMigrateUpOnce()
    {
        using var scope = _hostMock.Object.Services.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<IRecipesDatabase>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        databaseService.Create("customDbName");
        migrationService.ListMigrations();
        migrationService.MigrateUp();

        using var assertionScope = new AssertionScope();
            _recipeDatabaseMock.Verify(s => s.Create("customDbName"), Times.Once);
            _migrationRunnerMock.Verify(s => s.ListMigrations(), Times.Once);
            _migrationRunnerMock.Verify(s => s.MigrateUp(), Times.Once);
            _migrationRunnerMock.Verify(s => s.MigrateDown(123), Times.Never);
    }
}