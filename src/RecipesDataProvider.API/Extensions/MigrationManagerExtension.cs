using FluentMigrator.Runner;
using RecipesDataProvider.API.Migrations;

namespace RecipesDataProvider.API.Extensions;

public static class MigrationManagerExtension
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<RecipesDatabase>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        databaseService.Create("culinary_blog");
            
        migrationService.ListMigrations();
        migrationService.MigrateUp();

        return host;
    }
}