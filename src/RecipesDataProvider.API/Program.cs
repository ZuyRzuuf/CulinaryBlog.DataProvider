using System.Reflection;
using Dapper;
using FluentMigrator.Runner;
using RecipesDataProvider.API.Extensions;
using RecipesDataProvider.API.Migrations;
using RecipesDataProvider.Infrastructure;
using RecipesDataProvider.Infrastructure.Database;
using RecipesDataProvider.Infrastructure.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<MysqlContext>();
builder.Services.AddSingleton<RecipesDatabase>();
builder.Services.AddInfrastructureServices();
SqlMapper.AddTypeHandler(new MySqlGuidTypeHandler());
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