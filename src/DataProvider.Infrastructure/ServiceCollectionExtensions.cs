using Microsoft.Extensions.DependencyInjection;
using DataProvider.Domain.Interfaces;
using DataProvider.Infrastructure.Database;
using DataProvider.Infrastructure.Repositories;

namespace DataProvider.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<MysqlContext>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();

        return services;
    }
}