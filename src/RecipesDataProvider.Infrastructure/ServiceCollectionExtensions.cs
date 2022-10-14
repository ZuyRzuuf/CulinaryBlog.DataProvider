using Microsoft.Extensions.DependencyInjection;
using RecipesDataProvider.Domain.Interfaces;
using RecipesDataProvider.Infrastructure.Database;
using RecipesDataProvider.Infrastructure.Repositories;

namespace RecipesDataProvider.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<MysqlContext>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();

        return services;
    }
}