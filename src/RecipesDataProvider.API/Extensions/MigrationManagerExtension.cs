using FluentMigrator.Runner;
using RecipesDataProvider.Migrations;

namespace RecipesDataProvider.Extensions;

public static class MigrationManagerExtension
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<Database>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        databaseService.Create("culinary_blog");
            
        migrationService.ListMigrations();
        migrationService.MigrateUp();

        return host;
    }
}