using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RecipesDataProvider.Infrastructure.Helpers;
using RecipesDataProvider.Infrastructure.Interfaces;

namespace RecipesDataProvider.Infrastructure.Database;

public class MysqlContext : IDbContext
{
    private readonly string? _connectionString;

    public MysqlContext(IConfiguration configuration, string connectionStringName = "schema")
    {
        _connectionString = configuration.GetConnectionString(connectionStringName);
            
        if (!ConnectionStringValidator.Validate(_connectionString))
        {
            throw new BadConnectionStringException();
        }
    }

    public IDbConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}