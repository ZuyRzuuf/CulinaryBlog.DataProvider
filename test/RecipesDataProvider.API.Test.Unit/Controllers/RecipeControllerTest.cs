using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RecipesDataProvider.API.Controllers;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Domain.Interfaces;
using Xunit;

namespace RecipesDataProvider.API.Test.Unit.Controllers;

public class RecipeControllerTest
{
    private readonly RecipeController _recipeController;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly IList<Recipe> _recipeInMemoryDatabase;
    
    public RecipeControllerTest()
    {
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        var loggerMock = new Mock<ILogger<RecipeController>>();
        _recipeController = new RecipeController(_recipeRepositoryMock.Object, loggerMock.Object);
        _recipeInMemoryDatabase = new List<Recipe>
        {
            new() {Uuid = Guid.Parse("ab24fde6-495b-45b6-be3c-1343939b646a"), Title = "Recipe 1"},
            new() {Uuid = Guid.Parse("fe0efe1e-eab7-4ca4-a059-e51de04b0eed"), Title = "Recipe 2"},
            new() {Uuid = Guid.Parse("a4f5ceb4-3d74-444f-a05f-57e8cfd42061"), Title = "Recipe 3"}
        };
    }
    
    [Fact]
    public async Task GetRecipes_Returns_OkObjectResult()
    {
        var response = await _recipeController.GetRecipes();

        using (new AssertionScope())
        {
            response.Should().BeOfType<OkObjectResult>();
            response.Should().BeAssignableTo<OkObjectResult>();
        }
    }

    [Fact]
    public async Task GetRecipes_Returns_ListOfRecipes()
    {
        _recipeRepositoryMock.Setup(r => r.GetRecipes())
            .ReturnsAsync(_recipeInMemoryDatabase);
        
        var response = await _recipeController.GetRecipes();
        var result = Assert.IsType<OkObjectResult>(response);
        var recipes = (List<Recipe>) result.Value!;
        var recipesCount = recipes.Count;

        using (new AssertionScope())
        {
            result.Value.Should().BeOfType<List<Recipe>>();
            recipesCount.Should().Be(3);
        }
    }
}