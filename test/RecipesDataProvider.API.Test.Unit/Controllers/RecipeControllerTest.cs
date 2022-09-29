using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using RecipesDataProvider.Controllers;
using RecipesDataProvider.Domain.Entities;
using Xunit;

namespace RecipesDataProvider.API.Test.Unit.Controllers;

public class RecipeControllerTest
{
    private readonly RecipeController _recipeController;

    public RecipeControllerTest()
    {
        _recipeController = new RecipeController();
    }
    
    [Fact]
    public async Task GetRecipes_ReturnsStatusCodeOk()
    {
        var response = await _recipeController.GetRecipes();

        using (new AssertionScope())
        {
            response.Should().BeOfType<OkObjectResult>();
            response.Should().BeAssignableTo<OkObjectResult>();
        }
    }

    [Fact]
    public async Task GetRecipes_ReturnsListOfRecipes()
    {
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