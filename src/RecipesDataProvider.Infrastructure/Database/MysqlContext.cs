using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using RecipesDataProvider.Infrastructure.Helpers;
using RecipesDataProvider.Infrastructure.Interfaces;

namespace RecipesDataProvider.Infrastructure.Database;

public class MysqlContext : IDbContext
{
    private readonly string? _connectionString;

    public MysqlContext(IConfiguration configuration)
    {
        // TODO: throw an error is _connectionString is null, empty or is incorrect
        // server=localhost;port=8083;userid=root;password=root;database=culinary_blog
        // _connectionString = configuration.GetConnectionString("local");
        
        try
        {
            _connectionString = configuration.GetConnectionString("local");
            
            if (!ConnectionStringValidator.Validate(_connectionString))
            {
                throw new BadConnectionStringException();
            }
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public IDbConnection CreateConnection()
        => new MySqlConnection(_connectionString);
}