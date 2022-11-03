using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using RecipesDataProvider.API.Controllers;
using RecipesDataProvider.API.Test.Integration.Fixtures;
using RecipesDataProvider.Domain.Entities;
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
}