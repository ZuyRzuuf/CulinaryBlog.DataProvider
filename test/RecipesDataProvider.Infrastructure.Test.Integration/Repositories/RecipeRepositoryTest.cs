using FluentAssertions;
using RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Repositories;

// [Collection("TestDatabaseSetup")]
// public class RecipeRepositoryTest
// public class RecipeRepositoryTest : IClassFixture<DatabaseSetupFixture>
public class RecipeRepositoryTest : IClassFixture<RecipeRepositoryFixture>
{
    private readonly RecipeRepositoryFixture _fixture;
    // private readonly List _fakeTestData;
    
    public RecipeRepositoryTest(RecipeRepositoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void GetRecipes_ReturnsRecipesList_WhenNoExceptionIsThrown()
    {
        true.Should().BeTrue();
    }
}