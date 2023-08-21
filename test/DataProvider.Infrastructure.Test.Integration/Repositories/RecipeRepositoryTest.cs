using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using MySql.Data.MySqlClient;
using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Queries;
using DataProvider.Infrastructure.Exceptions;
using DataProvider.Infrastructure.Repositories;
using DataProvider.Infrastructure.Test.Integration.Fixtures;
using Xunit;

namespace DataProvider.Infrastructure.Test.Integration.Repositories;

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

        var result = await sut.GetRecipes();

        result.Should()
            .NotBeEmpty()
            .And
            .BeOfType<List<Recipe>>()
            .And
            .OnlyHaveUniqueItems()
            .And
            .NotContainNulls(r => r.Title);
    }

    [Fact]
    public async Task GetRecipes_ThrowsDatabaseConnectionProblemException_WhenDatabaseReturnsException()
    {
        var sut = new RecipeRepository(_fixture.MysqlTestContextWithoutSchema, _fixture.Logger);

        await sut.Invoking(r => r
            .GetRecipes()).Should().ThrowAsync<UnknownDatabaseException>();
    }
    
    [Fact]
    public async Task GetRecipesByTitle_ReturnsRecipesListWithTwoElements_WhenNoExceptionIsThrown()
    {
        const string recipesToFind = "Two Recipes to Get";
        var sut = _fixture.Sut;
        var recipesToCreate = new List<CreateRecipeDto>
        {
            new() { Title = $"{recipesToFind} {Guid.NewGuid().ToString()}" },
            new() { Title = $"{recipesToFind} {Guid.NewGuid().ToString()}" }
        };

        await sut.CreateRecipe(recipesToCreate[0]);
        await sut.CreateRecipe(recipesToCreate[1]);

        var result = await sut.GetRecipesByTitle(recipesToFind);

        result.Should()
            .NotBeEmpty()
            .And
            .BeOfType<List<Recipe>>()
            .And
            .OnlyHaveUniqueItems()
            .And
            .NotContainNulls(r => r.Title)
            .And
            .HaveCount(2);
    }
    
    [Fact]
    public async Task GetRecipesByTitle_ReturnsRecipesListWithOneElement_WhenNoExceptionIsThrown()
    {
        var sut = _fixture.Sut;
        var recipesToCreate = new List<CreateRecipeDto>
        {
            new() { Title = $"First Recipe to Get {Guid.NewGuid().ToString()}" },
            new() { Title = $"Second Recipe to Get {Guid.NewGuid().ToString()}" }
        };

        await sut.CreateRecipe(recipesToCreate[0]);
        await sut.CreateRecipe(recipesToCreate[1]);

        var result = await sut.GetRecipesByTitle("Second Recipe to Get");

        result.Should()
            .NotBeEmpty()
            .And
            .BeOfType<List<Recipe>>()
            .And
            .OnlyHaveUniqueItems()
            .And
            .NotContainNulls(r => r.Title)
            .And
            .HaveCount(1);
    }
    
    [Fact]
    public async Task GetRecipesByTitle_ReturnsRecipesListWithoutElements_WhenNoExceptionIsThrown()
    {
        var sut = _fixture.Sut;
        var result = await sut.GetRecipesByTitle("Non existing recipe");

        result.Should()
            .BeEmpty()
            .And
            .BeOfType<List<Recipe>>()
            .And
            .HaveCount(0);
    }
    
    [Fact]
    public async Task GetRecipesByTitle_ThrowsDatabaseConnectionProblemException_WhenDatabaseReturnsException()
    {
        var sut = new RecipeRepository(_fixture.MysqlTestContextWithoutSchema, _fixture.Logger);
    
        await sut.Invoking(r => r
            .GetRecipesByTitle("Some recipe")).Should().ThrowAsync<UnknownDatabaseException>();
    }
    
    [Fact]
    public async Task GetRecipeById_ReturnsRecipe_WhenRecipeExists()
    {
        var sut = _fixture.Sut;
        var recipes = await sut.GetRecipes();
        var recipeToGet = recipes.First();

        var result = await sut.GetRecipeById(recipeToGet.Id);
    
        result.Should()
            .NotBeNull()
            .And
            .BeOfType<Recipe>()
            .And
            .BeEquivalentTo(recipeToGet)
            ;
    }
    
    [Fact]
    public async Task GetRecipeById_ThrowsRecipeDoesNotExistException_WhenRecipeDoesNotExist()
    {
        var sut = _fixture.Sut;

        await sut.Invoking(r => r
            .GetRecipeById(Guid.NewGuid())).Should().ThrowAsync<RecipeDoesNotExistException>();
    }
    
    [Fact]
    public async Task GetRecipeById_ThrowsDatabaseConnectionProblemException_WhenDatabaseReturnsException()
    {
        var sut = new RecipeRepository(_fixture.MysqlTestContextWithoutSchema, _fixture.Logger);
    
        await sut.Invoking(r => r
            .GetRecipeById(Guid.NewGuid())).Should().ThrowAsync<UnknownDatabaseException>();
    }

    [Fact]
    public async Task GetRecipeWithIngredientsById_ReturnsRecipe_WhenRecipeExists()
    {
        var sut = _fixture.Sut;
        var recipes = await sut.GetRecipes();
        var recipeToGet = recipes.First();
    
        var result = await sut.GetRecipeWithIngredientsById(recipeToGet.Id);
    
        result.Should()
            .NotBeNull()
            .And
            .BeOfType<RecipeQuery>()
            .And
            .BeEquivalentTo(recipeToGet)
            ;
    }
    
    [Fact]
    public async Task GetRecipeWithIngredientsById_ThrowsRecipeDoesNotExistException_WhenRecipeDoesNotExist()
    {
        var sut = _fixture.Sut;
    
        await sut.Invoking(r => r
            .GetRecipeWithIngredientsById(Guid.NewGuid())).Should().ThrowAsync<RecipeDoesNotExistException>();
    }
    
    [Fact]
    public async Task GetRecipeWithIngredientsById_ThrowsDatabaseConnectionProblemException_WhenDatabaseReturnsException()
    {
        var sut = new RecipeRepository(_fixture.MysqlTestContextWithoutSchema, _fixture.Logger);
    
        await sut.Invoking(r => r
            .GetRecipeWithIngredientsById(Guid.NewGuid())).Should().ThrowAsync<UnknownDatabaseException>();
    }

    [Fact]
    public async Task CreateRecipe_ReturnsRecipe_WhenRecipeIsCreated()
    {
        var sut = _fixture.Sut;
        var recipes = await sut.GetRecipes();
        var numberRecipesInDatabase = recipes.Count;

        var recipeDto = new CreateRecipeDto
        {
            Title = new Bogus.DataSets.Lorem().Sentence(2)
        };
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

    [Fact]
    public async Task UpdateRecipe_Returns1_WhenRecipeIsUpdated()
    {
        var sut = _fixture.Sut;
        var baseRecipesList = await sut.GetRecipes();
        var recipeToUpdate = baseRecipesList.First();
        var recipeDto = new UpdateRecipeDto
        {
            Id = recipeToUpdate.Id,
            Title = new Bogus.DataSets.Lorem().Sentence(2)
        };

        var response = await sut.UpdateRecipe(recipeDto);

        response.Should().Be(1);
    }

    [Fact]
    public async Task UpdateRecipe_DatabaseContainsUpdatedRecipe_WhenRecipeIsUpdated()
    {
        var sut = _fixture.Sut;
        var baseRecipesList = await sut.GetRecipes();
        var recipeToUpdate = baseRecipesList.First();
        var recipeDto = new UpdateRecipeDto
        {
            Id = recipeToUpdate.Id,
            Title = new Bogus.DataSets.Lorem().Sentence(2)
        };

        await sut.UpdateRecipe(recipeDto);
        var updatedRecipesList = await sut.GetRecipes();

        updatedRecipesList.Should()
            .HaveCount(baseRecipesList.Count)
            .And
            .NotContain(recipeToUpdate)
            .And
            .ContainEquivalentOf(recipeDto);
    }
    
    [Fact]
    public async Task UpdateRecipe_ThrowsMySqlException_WhenDatabaseReturnsException()
    {
        var sut = new RecipeRepository(_fixture.MysqlTestContextWithoutSchema, _fixture.Logger);
        var recipeDto = new UpdateRecipeDto
        {
            Id = Guid.NewGuid(),
            Title = new Bogus.DataSets.Lorem().Sentence(2)
        };

        await sut.Invoking(r => r
            .UpdateRecipe(recipeDto)).Should().ThrowAsync<MySqlException>();
    }
    
    [Fact]
    public async Task UpdateRecipe_ThrowsRecipeDoesNotExistException_WhenRecipeToUpdateDoesNotExist()
    {
        var sut = _fixture.Sut;
        var recipeDto = new UpdateRecipeDto
        {
            Id = Guid.NewGuid(),
            Title = new Bogus.DataSets.Lorem().Sentence(2)
        };

        await sut.Invoking(r => r
            .UpdateRecipe(recipeDto)).Should().ThrowAsync<RecipeDoesNotExistException>();
    }
    
    [Fact]
    public async Task DeleteRecipe_Returns1_WhenRecipeIsDeleted()
    {
        var sut = _fixture.Sut;
        var baseRecipesList = await sut.GetRecipes();
        var recipeToDelete = baseRecipesList.First();
    
        var response = await sut.DeleteRecipe(recipeToDelete.Id);
    
        response.Should().BeGreaterThan(1);
    }
    
    [Fact]
    public async Task DeleteRecipe_DatabaseDoesNotContainDeletedRecipe_WhenRecipeIsDeleted()
    {
        var sut = _fixture.Sut;
        var baseRecipesList = await sut.GetRecipes();
        var recipeToDelete = baseRecipesList.First();
    
        await sut.DeleteRecipe(recipeToDelete.Id);
        var deletedRecipesList = await sut.GetRecipes();

        deletedRecipesList.Should()
            .HaveCount(baseRecipesList.Count - 1)
            .And
            .NotContain(recipeToDelete)
            .And
            .NotBeEquivalentTo(baseRecipesList);
        deletedRecipesList.Select(r => r.Id).Should().NotEqual(baseRecipesList.Select(r => r.Id));
    }
    
    [Fact]
    public async Task DeleteRecipe_ThrowsMySqlException_WhenDatabaseReturnsException()
    {
        var sut = new RecipeRepository(_fixture.MysqlTestContextWithoutSchema, _fixture.Logger);
        var id = Guid.NewGuid();
    
        await sut.Invoking(r => r
            .DeleteRecipe(id)).Should().ThrowAsync<MySqlException>();
    }
    
    [Fact]
    public async Task DeleteRecipe_ThrowsRecipeDoesNotExistException_WhenRecipeToDeleteDoesNotExist()
    {
        var sut = _fixture.Sut;
        var id = Guid.NewGuid();
    
        await sut.Invoking(r => r
            .DeleteRecipe(id)).Should().ThrowAsync<RecipeDoesNotExistException>();
    }
}