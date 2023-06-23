using Dapper;
using DataProvider.Infrastructure.Database;
using DataProvider.Infrastructure.Interfaces;

namespace DataProvider.API.Migrations;

public class RecipesDatabase : IRecipesDatabase
{
    private readonly MysqlContext _mysqlContext;

    public RecipesDatabase(MysqlContext mysqlContext)
    {
        _mysqlContext = mysqlContext;
    }

    public void Create(string dbName)
    {
        const string query = "SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME = @name";
        var parameters = new DynamicParameters();
        
        parameters.Add("name", dbName);

        using var connection = _mysqlContext.CreateConnection();
        var records = connection.Query(query, parameters);
        if (!records.Any())
            connection.Execute($"CREATE DATABASE {dbName}");
    }
}