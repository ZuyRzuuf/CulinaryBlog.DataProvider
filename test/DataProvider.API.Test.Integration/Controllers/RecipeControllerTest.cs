using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using DataProvider.API.Controllers;
using DataProvider.API.Test.Integration.Fixtures;
using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using Xunit;

namespace DataProvider.API.Test.Integration.Controllers;

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
    public async Task GetRecipesByTitle_ReturnsOkObjectResult_WhenNoExceptionIsThrown()
    {
        const string titleToFind = "Two Recipes to Find";
        var recipesToFind = new List<CreateRecipeDto>
        {
            new() { Title = $"{titleToFind} {Guid.NewGuid()}" },
            new() { Title = $"{titleToFind} {Guid.NewGuid()}" }
        };
        
        await _fixture.RecipeRepository.CreateRecipe(recipesToFind[0]);
        await _fixture.RecipeRepository.CreateRecipe(recipesToFind[1]);

        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.GetRecipesByTitle(titleToFind);
        var recipes = (OkObjectResult)result;

        recipes.Should()
            .BeOfType<OkObjectResult>()
            .And
            .NotBeNull();
    }

    [Fact]
    public async Task GetRecipesByTitle_ReturnsStatusCode200_WhenNoExceptionIsThrown()
    {
        const string titleToFind = "Two Recipes to Find";
        var recipesToFind = new List<CreateRecipeDto>
        {
            new() { Title = $"{titleToFind} {Guid.NewGuid()}" },
            new() { Title = $"{titleToFind} {Guid.NewGuid()}" }
        };
        
        await _fixture.RecipeRepository.CreateRecipe(recipesToFind[0]);
        await _fixture.RecipeRepository.CreateRecipe(recipesToFind[1]);

        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.GetRecipesByTitle(titleToFind);
        var recipes = (OkObjectResult)result;

        recipes.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetRecipesByTitle_ReturnsTypeRecipeList_WhenNoExceptionIsThrown()
    {
        var titleToFind = $"{Guid.NewGuid()} Recipes to Find";
        var recipesToFind = new List<CreateRecipeDto>
        {
            new() { Title = $"{titleToFind} {Guid.NewGuid()}" },
            new() { Title = $"{titleToFind} {Guid.NewGuid()}" }
        };
        
        await _fixture.RecipeRepository.CreateRecipe(recipesToFind[0]);
        await _fixture.RecipeRepository.CreateRecipe(recipesToFind[1]);

        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.GetRecipesByTitle(titleToFind);
        var recipes = (OkObjectResult)result;

        recipes.Value.Should()
            .BeOfType<List<Recipe>>()
            .And
            .BeEquivalentTo(recipesToFind);
    }

    [Fact]
    public async Task GetRecipesByTitle_ReturnsStatusCode500ButNotThrowsException_WhenServerThrowsException()
    {
        var recipeController = new RecipeController(_fixture.RecipeRepositoryThrowingException, _fixture.RecipeControllerLogger);
        var result = await recipeController.GetRecipesByTitle("Some title");
        var data = (ObjectResult)result;

        Action act = () => data.StatusCode.Should().Be(500);
        act.Should().NotThrow();
        result.Should().BeOfType<ObjectResult>();
    }

    [Fact]
    public async Task GetRecipeWithIngredientsById_ReturnsRecipe_WhenIdExists()
    {
        var recipesList = await _fixture.RecipeRepository.GetRecipes();
        var recipeToGet = recipesList.First();
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
        var result = await recipeController.GetRecipeWithIngredientsById(recipeToGet.Id);
        var objectResult = result as ObjectResult;
        
        if (objectResult?.StatusCode == 200)
        {
            var data = (OkObjectResult)result;

            result.Should()
                .BeOfType<OkObjectResult>()
                .And
                .BeAssignableTo<OkObjectResult>();
            data.StatusCode.Should().Be(200);
            data.Value.Should().BeEquivalentTo(recipeToGet);
        }
        else
        {
            Assert.Fail($"Expected 200 OK but got {objectResult?.StatusCode} instead.");
        }
    }
    
    [Fact]
    public async Task GetRecipeBeId_ReturnsStatusCode404_WhenRecipeDoesNotExists()
    {
        var recipeController = new RecipeController(_fixture.RecipeRepository, _fixture.RecipeControllerLogger);
    
        var result = await recipeController.GetRecipeWithIngredientsById(Guid.NewGuid());
        var data = (ObjectResult)result;
        
        result.Should()
            .BeOfType<ObjectResult>()
            .And
            .BeAssignableTo<ObjectResult>();
        data.StatusCode.Should().Be(404);
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
            .ContainKeys("Id", "Title")
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
            Id = recipeToUpdate.Id,
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
            Id = Guid.NewGuid(),
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
        var result = await recipeController.DeleteRecipe(recipeToDelete.Id);
        
        if(result is NoContentResult noContentResult)
        {
            noContentResult.Should()
                .BeOfType<NoContentResult>()
                .And
                .BeAssignableTo<NoContentResult>();
            noContentResult.StatusCode.Should().Be(204);
        }
        else
        {
            Assert.Fail($"Expected 204 No Content but got {result.GetType().Name} instead.");
        }
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