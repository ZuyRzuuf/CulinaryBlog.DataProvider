using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RecipesDataProvider.API.Controllers;
using RecipesDataProvider.Domain.Dto;
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

    [Fact]
    public async Task CreateRecipe_Returns_CreatedAtActionResult()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "Newly created Recipe"
        };
        var uuid = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.CreateRecipe(recipeDto))
            .Returns((RecipeDto dto) =>
            {
                var recipe = new Recipe
                {
                    Uuid = uuid,
                    Title = dto.Title
                };
                
                _recipeInMemoryDatabase.Add(recipe);

                return Task.FromResult(recipe);
            });

        var response = await _recipeController.CreateRecipe(recipeDto);

        using (new AssertionScope())
        {
            response.Should().BeOfType<CreatedAtActionResult>();
            response.Should().BeAssignableTo<CreatedAtActionResult>();
        }
    }

    [Fact]
    public async Task CreateRecipe_ReturnsCreatedRecipe_WhenRecipeIsAddedToDatabase()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "Newly created Recipe"
        };
        var uuid = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.CreateRecipe(recipeDto))
            .Returns((RecipeDto dto) =>
            {
                var recipe = new Recipe
                {
                    Uuid = uuid,
                    Title = dto.Title
                };
                
                _recipeInMemoryDatabase.Add(recipe);

                return Task.FromResult(recipe);
            });

        var response = await _recipeController.CreateRecipe(recipeDto);
        var result = Assert.IsType<CreatedAtActionResult>(response);
        var recipe = (Recipe) result.Value!;

        using (new AssertionScope())
        {
            result.Value.Should().BeOfType<Recipe>();
            recipe.Uuid.Should().Be(uuid);
            recipe.Title.Should().Be(recipeDto.Title);
        }
    }

    [Fact]
    public async Task UpdateRecipe_ReturnsStatusCode204_WhenRecipeIsUpdated()
    {
        var recipeToUpdate = _recipeInMemoryDatabase.First();
        var recipeDto = new UpdateRecipeDto
        {
            Uuid = recipeToUpdate.Uuid,
            Title = "Updated Recipe Title"
        };
        
        _recipeRepositoryMock.Setup(r => r.UpdateRecipe(recipeDto))
            .Returns((UpdateRecipeDto dto) =>
            {
                var enumerable = _recipeInMemoryDatabase
                    .Where(r => r.Uuid == dto.Uuid)
                    .Select(r => 
                    { 
                        r.Title = dto.Title; 
                        return r;
                    });

                var test = _recipeInMemoryDatabase
                    .Any(r => r.Uuid == dto.Uuid);

                return Task.FromResult(test ? 1 : 0);
            });

        var response = await _recipeController.UpdateRecipe(recipeDto);
        var data = (NoContentResult)response;

        using (new AssertionScope())
        {
            response.Should().BeOfType<NoContentResult>();
            response.Should().BeAssignableTo<NoContentResult>();
            data.StatusCode.Should().Be(204);
        }
    }

    [Fact]
    public async Task UpdateRecipe_ReturnsStatusCode404_WhenRecipeDoesNotExist()
    {
        var recipeDto = new UpdateRecipeDto
        {
            Uuid = Guid.NewGuid(),
            Title = "Non existing Recipe"
        };
        
        _recipeRepositoryMock.Setup(r => r.UpdateRecipe(recipeDto))
            .Returns((UpdateRecipeDto dto) =>
            {
                var enumerable = _recipeInMemoryDatabase
                    .Where(r => r.Uuid == dto.Uuid)
                    .Select(r => 
                    { 
                        r.Title = dto.Title; 
                        return r;
                    });

                var test = _recipeInMemoryDatabase
                    .Any(r => r.Uuid == dto.Uuid);

                return Task.FromResult(test ? 1 : 0);
            });
    
        var response = await _recipeController.UpdateRecipe(recipeDto);
        var data = (ObjectResult)response;

        using (new AssertionScope())
        {
            response.Should().BeOfType<ObjectResult>();
            response.Should().BeAssignableTo<ObjectResult>();
            data.StatusCode.Should().Be(404);
        }
    }
    
    [Fact]
    public async Task DeleteRecipe_ReturnsStatusCode204_WhenRecipeIsDeleted()
    {
        var recipeToDelete = _recipeInMemoryDatabase.First();
        
        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(recipeToDelete.Uuid))
            .Returns((Guid recipeUuid) =>
            {
                var recipes = _recipeInMemoryDatabase
                    .Where(r => r.Uuid == recipeUuid)
                    .ToList();
    
                var test = recipes.RemoveAll(r => r.Uuid == recipeUuid);
    
                return Task.FromResult(test == 1 ? 1 : 0);
            });
    
        var response = await _recipeController.DeleteRecipe(recipeToDelete.Uuid);
        var data = (NoContentResult)response;
    
        using (new AssertionScope())
        {
            response.Should().BeOfType<NoContentResult>();
            response.Should().BeAssignableTo<NoContentResult>();
            data.StatusCode.Should().Be(204);
        }
    }
    
    [Fact]
    public async Task DeleteRecipe_ReturnsStatusCode404_WhenRecipeDoesNotExist()
    {
        var recipeToDelete = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(recipeToDelete))
            .Returns((Guid recipeUuid) =>
            {
                var recipes = _recipeInMemoryDatabase
                    .Where(r => r.Uuid == recipeUuid)
                    .ToList();
    
                var test = recipes
                    .RemoveAll(r => r.Uuid == recipeUuid);
    
                return Task.FromResult(test == 1 ? 1 : 0);
            });
    
        var response = await _recipeController.DeleteRecipe(recipeToDelete);
        var data = (ObjectResult)response;
    
        using (new AssertionScope())
        {
            response.Should().BeOfType<ObjectResult>();
            response.Should().BeAssignableTo<ObjectResult>();
            data.StatusCode.Should().Be(404);
        }
    }
}