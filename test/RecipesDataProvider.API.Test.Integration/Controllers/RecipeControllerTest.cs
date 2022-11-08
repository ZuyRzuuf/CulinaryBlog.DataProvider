using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RecipesDataProvider.API.Controllers;
using RecipesDataProvider.API.Test.Integration.Fixtures;
using RecipesDataProvider.Domain.Dto;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Infrastructure.Exceptions;
using Xunit;

namespace RecipesDataProvider.API.Test.Integration.Controllers;

public class RecipeControllerTest : IClassFixture<RecipeControllerFixture>
{
    private readonly RecipeControllerFixture _fixture;
    
    public RecipeControllerTest(RecipeControllerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetRecipes_ReturnsStatusCode200_WhenServerRespondedWithOkObjectResult()
    {
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.GetRecipes();
        var data = (OkObjectResult)result;
        var statusCode = data.StatusCode;

        Action act = () => statusCode.Should().Be(200);
        act.Should().NotThrow();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetRecipes_ReturnsListOfRecipes_WhenServerRespondedWithOkObjectResult()
    {
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = (OkObjectResult)await recipeController.GetRecipes();

        result.Value.Should().BeOfType<List<Recipe>>();
    }

    [Fact]
    public async Task GetRecipes_ReturnsStatusCode500ButNotThrowsException_WhenServerThrowsException()
    {
        var recipeController = new RecipeController(_fixture.RecipeRepositoryThrowingException, _fixture.RecipeControllerLogger);
        var result = await recipeController.GetRecipes();
        var data = (ObjectResult)result;

        Action act = () => data.StatusCode.Should().Be(500);
        act.Should().NotThrow();
        result.Should().BeOfType<ObjectResult>();
    }

    [Fact]
    public async Task CreateRecipe_ReturnsStatusCode201_WhenServerRespondedWithCreatedAtActionResult()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "CreateRecipe_ReturnsStatusCode201_WhenServerRespondedWithCreatedAtActionResult"
        };
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.CreateRecipe(recipeDto);
        var data = (CreatedAtActionResult)result;
        var statusCode = data.StatusCode;

        Action act = () => statusCode.Should().Be(201);
        act.Should().NotThrow();
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task CreateRecipe_ReturnsCreatedRecipe_WhenServerRespondedWithCreatedAtActionResult()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "CreateRecipe_ReturnsCreatedRecipe_WhenServerRespondedWithCreatedAtActionResult"
        };
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = (CreatedAtActionResult)await recipeController.CreateRecipe(recipeDto);
        var createdRecipe = (Recipe)result.Value!;

        result.Value.Should().BeOfType<Recipe>();
        result.RouteValues.Should()
            .ContainKeys("Uuid", "Title")
            .And
            .Contain("Title", recipeDto.Title);
        createdRecipe.Title.Should().Be(recipeDto.Title);
    }

    [Fact]
    public async Task UpdateRecipe_ReturnsStatusCode204_WhenRecipeIsUpdated()
    {
        var recipesList = await _fixture.RecipeRepository.GetRecipes();
        var recipeToUpdate = recipesList.First();
        var recipeDto = new UpdateRecipeDto
        {
            Uuid = recipeToUpdate.Uuid,
            Title = new Bogus.DataSets.Lorem().Sentences(2)
        };
        
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.UpdateRecipe(recipeDto);
        var data = (NoContentResult)result;

        result.Should()
            .BeOfType<NoContentResult>()
            .And
            .BeAssignableTo<NoContentResult>();
        data.StatusCode.Should().Be(204);
    }
    
    [Fact]
    public async Task UpdateRecipe_ReturnsStatusCode404_WhenRecipeDoesNotExists()
    {
        var recipeDto = new UpdateRecipeDto
        {
            Uuid = Guid.NewGuid(),
            Title = new Bogus.DataSets.Lorem().Sentences(2)
        };
        
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);

        var result = await recipeController.UpdateRecipe(recipeDto);
        var data = (ObjectResult)result;
        
        result.Should()
            .BeOfType<ObjectResult>()
            .And
            .BeAssignableTo<ObjectResult>();
        data.StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task DeleteRecipe_ReturnsStatusCode204_WhenRecipeIsDeleted()
    {
        var recipesList = await _fixture.RecipeRepository.GetRecipes();
        var recipeToDelete = recipesList.First();
        
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.DeleteRecipe(recipeToDelete.Uuid);
        var data = (NoContentResult)result;
    
        result.Should()
            .BeOfType<NoContentResult>()
            .And
            .BeAssignableTo<NoContentResult>();
        data.StatusCode.Should().Be(204);
    }
    
    [Fact]
    public async Task DeleteRecipe_ReturnsStatusCode404_WhenRecipeDoesNotExists()
    {
        var recipeToDelete = Guid.NewGuid();
        
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
    
        var result = await recipeController.DeleteRecipe(recipeToDelete);
        var data = (ObjectResult)result;
        
        result.Should()
            .BeOfType<ObjectResult>()
            .And
            .BeAssignableTo<ObjectResult>();
        data.StatusCode.Should().Be(404);
    }
}