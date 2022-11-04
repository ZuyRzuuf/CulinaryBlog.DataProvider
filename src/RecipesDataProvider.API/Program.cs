using System.Reflection;
using FluentMigrator.Runner;
using RecipesDataProvider.API.Extensions;
using RecipesDataProvider.API.Migrations;
using RecipesDataProvider.Infrastructure;
using RecipesDataProvider.Infrastructure.Database;
using RecipesDataProvider.Infrastructure.Interfaces;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .CreateBootstrapLogger();

builder.Host
    .UseSerilog((
        (hostBuilderContext, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(hostBuilderContext.Configuration)));

// Add services to the container.
builder.Services.AddSingleton<MysqlContext>();
builder.Services.AddSingleton<IRecipesDatabase, RecipesDatabase>();
builder.Services.AddInfrastructureServices();

// register Fluent Migrator
builder.Services.AddLogging(l => l.AddFluentMigratorConsole())
    .AddFluentMigratorCore()
    .ConfigureRunner(c => c.AddMySql5()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("schema"))
        .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// start Fluent Migrator migrations
app.MigrateDatabase();

app.Run();

// used for integration test configuration
// based on https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
namespace RecipesDataProvider.API
{
    public partial class Program { }
}