using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using DataProvider.Infrastructure.Database;
using DataProvider.Infrastructure.Exceptions;
using Xunit;

namespace DataProvider.Infrastructure.Test.Integration.Database;

public class MysqlContextTest
{
    private MysqlContext _mysqlContext = default!;
    private readonly IConfiguration _configuration;

    public MysqlContextTest()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(@"appsettings.test.json", false, false)
            .AddEnvironmentVariables()
            .Build();
    }

    [Fact]
    public void MysqlContext_ReturnsMySqlConnectionObject_WhenConnectionStringIsCorrect()
    {
        _mysqlContext = new MysqlContext(_configuration);
        
        var response = _mysqlContext.CreateConnection();
        
        response.Should().BeOfType<MySqlConnection>();
    }

    [Fact]
    public void MysqlContext_DoesNotThrowException_WhenConnectionStringIsCorrect()
    {
        const string connectionStringStub =
            "server=localhost;port=3306;userid=admin;password=admin;database=test_db";
        var appSettingsStub = new Dictionary<string, string> {
            {"ConnectionStrings:schema", connectionStringStub}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(appSettingsStub)
            .Build();
        
        Action action = () => new MysqlContext(configuration);

        action.Should().NotThrow();
    }

    [Fact]
    public void MysqlContext_ThrowsArgumentNullException_WhenConnectionStringIsNotFound()
    {
        const string connectionStringStub =
            "server=localhost;port=3306;userid=admin;password=admin;database=test_db";
        
        var appSettingsStub = new Dictionary<string, string> {
            {"ConnectionStrings:wrong", connectionStringStub}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(appSettingsStub)
            .Build();
        
        Action action = () => new MysqlContext(configuration);
        
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MysqlContext_ThrowsBadConnectionStringException_WhenConnectionStringIsIncorrect()
    {
        const string badConnectionStringStub =
            "server=localhost;userid=admin;password=admin;database=test_db";
        
        var appSettingsStub = new Dictionary<string, string> {
            {"ConnectionStrings:schema", badConnectionStringStub}
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(appSettingsStub)
            .Build();
    
        Action action = () => new MysqlContext(configuration);
        
        action.Should().Throw<BadConnectionStringException>();
    }

    [Fact]
    public void MysqlContext_ThrowsBadConnectionStringException_WhenConnectionStringIsIncorrect_2222222()
    {
        var exceptionMessage = "Custom message";

        Action action = () => throw new BadConnectionStringException(exceptionMessage);
        
        action.Should().Throw<BadConnectionStringException>().WithMessage(exceptionMessage);
    }
}