using FluentAssertions;
using FluentAssertions.Execution;
using RecipesDataProvider.Infrastructure.Helpers;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Unit.Helpers;

public class ConnectionStringValidatorTest
{
    [Fact]
    public void Validate_ReturnsTrue_WhenConnectionStringMatchPattern()
    {
        var validConnectionStrings = new[]
        {
            "server=localhost;port=3306;userid=test;password=test;database=test_db",
            "server=127.0.0.7;port=3306;userid=test;password=test;database=test_db",
            "server=localhost/db;port=3306;userid=test;password=test;database=test_db",
            "server=127.0.0.7/db;port=3306;userid=test;password=test;database=test_db",
            "server=localhost;port=3306;userid=test01;password=z^u*Kr8Nd$;database=test-db",
        };

        using (new AssertionScope())
        {
            foreach (var validConnectionString in validConnectionStrings)
            {
                ConnectionStringValidator.Validate(validConnectionString).Should().BeTrue();
            }
        }
    }

    [Fact]
    public void Validate_ReturnsFalse_WhenConnectionStringNotMatchPattern()
    {
        const string validConnectionString = "server=localhost;userid=test;password=test;database=test_db";

        ConnectionStringValidator.Validate(validConnectionString).Should().BeFalse();
    }
}