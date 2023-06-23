using FluentMigrator.Runner;
using DataProvider.Infrastructure.Interfaces;

namespace DataProvider.API.Extensions;

public static class MigrationManagerExtension
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<IRecipesDatabase>();
        var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

        databaseService.Create("culinary_blog");
            
        migrationService.ListMigrations();
        migrationService.MigrateUp();

        return host;
    }
}