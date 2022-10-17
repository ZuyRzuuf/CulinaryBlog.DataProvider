using FluentAssertions;
using RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Repositories;

public class RecipeRepositoryTest : IClassFixture<DatabaseSetupFixture>
{
    private readonly DatabaseSetupFixture _fixture;
    
    public RecipeRepositoryTest(DatabaseSetupFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test1()
    {
        true.Should().BeTrue();
    }
}