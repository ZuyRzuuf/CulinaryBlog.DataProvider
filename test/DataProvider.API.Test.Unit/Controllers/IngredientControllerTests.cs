using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataProvider.API.Controllers;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DataProvider.API.Test.Unit.Controllers;

public class IngredientControllerTests
{
    private readonly IngredientController _ingredientController;
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock;
    private readonly IList<Ingredient> _ingredientsInMemoryDatabase;

    public IngredientControllerTests()
    {
        _ingredientRepositoryMock = new Mock<IIngredientRepository>();
        Mock<ILogger<IngredientController>> loggerMock = new();
        _ingredientController = new IngredientController(_ingredientRepositoryMock.Object, loggerMock.Object);
        _ingredientsInMemoryDatabase = new List<Ingredient>
        {
            new Ingredient
            {
                Id = Guid.Parse("b4b6a2cf-eb8e-4fb4-9ae6-87a17e3ec57a"),
                RecipeId = Guid.Parse("8b046154-8fb5-41c7-85cb-3c997b78c05a"),
                Name = "Ingredient 1",
                Description = "Description 1",
                Quantity = 4,
                QuantityType = "szt."
            },
            new Ingredient
            {
                Id = Guid.Parse("1b7bf29c-7907-41ce-9dd5-ba0a50d65539"),
                RecipeId = Guid.Parse("8b046154-8fb5-41c7-85cb-3c997b78c05a"),
                Name = "Ingredient 2",
                Description = "Description 2",
                Quantity = 200,
                QuantityType = "g"
            },
            new Ingredient
            {
                Id = Guid.Parse("5adfe252-a9ef-49de-9d8d-880f74e332b9"),
                RecipeId = Guid.Parse("565eb2b1-cd74-4796-8636-c479dff9e29f"),
                Name = "Ingredient 3",
                Description = "Description 3",
                Quantity = 1,
                QuantityType = "szt."
            }
        };
    }
    
    [Fact]
    public async Task GetRecipeIngredientsByRecipeId_WhenRecipeIdExists_ReturnsOkObjectResultWithIngredients()
    {
        // Arrange
        var recipeId = Guid.Parse("8b046154-8fb5-41c7-85cb-3c997b78c05a");
        _ingredientRepositoryMock.Setup(x => x.GetRecipeIngredientsByRecipeIdAsync(recipeId))
            .ReturnsAsync(_ingredientsInMemoryDatabase.Where(x => x.RecipeId == recipeId).ToList());
        
        // Act
        var result = await _ingredientController.GetRecipeIngredientsByRecipeId(recipeId);
        var okObjectResult = result as OkObjectResult;
        var ingredients = okObjectResult?.Value as List<Ingredient>;
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();
        okObjectResult?.Value.Should().BeOfType<List<Ingredient>>();
        ingredients?.Count.Should().Be(2);
        ingredients?[0].Name.Should().Be("Ingredient 1");
        ingredients?[1].Name.Should().Be("Ingredient 2");
    }
    
    [Fact]
    public async Task GetRecipeIngredientsByRecipeId_WhenRecipeIdDoesNotExist_ReturnsOkObjectResultWithEmptyList()
    {
        // Arrange
        var recipeId = Guid.Parse("8b046154-8fb5-41c7-85cb-3c997b78c05a");
        _ingredientRepositoryMock.Setup(x => x.GetRecipeIngredientsByRecipeIdAsync(recipeId))
            .ReturnsAsync(new List<Ingredient>());
        
        // Act
        var result = await _ingredientController.GetRecipeIngredientsByRecipeId(recipeId);
        var okObjectResult = result as OkObjectResult;
        var ingredients = okObjectResult?.Value as List<Ingredient>;
        
        // Assert
        result.Should().BeOfType<OkObjectResult>();
        okObjectResult?.Value.Should().BeOfType<List<Ingredient>>();
        ingredients?.Count.Should().Be(0);
    }
}