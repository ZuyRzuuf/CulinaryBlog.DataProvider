using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using RecipesDataProvider.Domain.Dto;
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

        await sut.Invoking(r => r
            .GetRecipes()).Should().ThrowAsync<UnknownDatabaseException>();
    }

    [Fact]
    public async Task CreateRecipe_ReturnsRecipe_WhenRecipeIsCreated()
    {
        const int numberRecipesInDatabase = RecipesDataCollection.ItemsNumber;

        var recipeDto = new CreateRecipeDto
        {
            Title = "CreateRecipe_ReturnsRecipe_WhenRecipeIsCreated"
        };
        var sut = _fixture.Sut;
        var createdRecipe = await sut.CreateRecipe(recipeDto);
        var databaseContent = await sut.GetRecipes();

        using (new AssertionScope())
        {
            createdRecipe.Should()
                .NotBeNull()
                .And
                .BeOfType<Recipe>();
            createdRecipe.Title.Should().Be(recipeDto.Title);
            databaseContent.Should()
                .HaveCount(numberRecipesInDatabase + 1)
                .And
                .ContainItemsAssignableTo<Recipe>()
                .And
                .ContainEquivalentOf(createdRecipe);
        }
    }

    [Fact]
    public async Task CreateRecipe_ThrowsRecipeHasToBeUniqueException_WhenRecipeTitleExists()
    {
        var sut = _fixture.Sut;
        var existingDatabaseContent = await sut.GetRecipes();
        var existingRecipe = existingDatabaseContent.FirstOrDefault();
        var recipeDto = new CreateRecipeDto
        {
            Title = existingRecipe!.Title
        };
        
        await sut.Invoking(r => r
            .CreateRecipe(recipeDto)).Should().ThrowAsync<RecipeHasToBeUniqueException>();
    }
}