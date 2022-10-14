using Dapper;
using RecipesDataProvider.Infrastructure.Database;

namespace RecipesDataProvider.Migrations;

public class Database
{
    private readonly MysqlContext _mysqlContext;

    public Database(MysqlContext mysqlContext)
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