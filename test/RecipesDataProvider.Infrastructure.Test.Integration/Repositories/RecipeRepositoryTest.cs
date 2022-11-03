using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Infrastructure.Exceptions;
using RecipesDataProvider.Infrastructure.Repositories;
using RecipesDataProvider.Infrastructure.Test.Integration.Database.TestData;
using RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Repositories;

public class RecipeRepositoryTest : IClassFixture<RecipeRepositoryFixture>
{
    private readonly RecipeRepositoryFixture _fixture;
    
    public RecipeRepositoryTest(RecipeRepositoryFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetRecipes_ReturnsRecipesList_WhenNoExceptionIsThrown()
    {
        var sut = _fixture.Sut;
        const int expectedItemsNumber = RecipesDataCollection.ItemsNumber;

        var expected = _fixture.RecipesCollection;
        var result = await sut.GetRecipes();

        result.Should()
            .NotBeEmpty()
            .And
            .BeOfType<List<Recipe>>()
            .And
            .HaveCount(expectedItemsNumber)
            .And
            .OnlyHaveUniqueItems()
            .And
            .NotContainNulls(r => r.Title)
            .And
            .BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetRecipes_ThrowsDatabaseConnectionProblemException_WhenDatabaseReturnsException()
    {
        var sut = new RecipeRepository(_fixture.MysqlTestContextWithoutSchema, _fixture.Logger);

        await sut.Invoking(r => r.GetRecipes()).Should().ThrowAsync<DatabaseConnectionProblemException>();
    }
}