using System.IO;
using System.Reflection;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataProvider.Infrastructure.Test.Integration;

public class DataProviderApplication : WebApplicationFactory<API.Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration(configuration =>
        {
            configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", false, false)
                .Build();
        });
        
        builder.ConfigureServices((context, service) =>
        {
            service.AddLogging(l => FluentMigratorLoggingExtensions.AddFluentMigratorConsole((ILoggingBuilder)l))
                .AddFluentMigratorCore()
                .ConfigureRunner(mrb => mrb.AddMySql5()
                    .WithGlobalConnectionString(context.Configuration.GetConnectionString("schema"))
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());
        });

        return builder.Build();
    }
}