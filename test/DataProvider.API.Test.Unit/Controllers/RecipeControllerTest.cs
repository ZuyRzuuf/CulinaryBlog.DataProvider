using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using DataProvider.API.Controllers;
using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using Xunit;

namespace DataProvider.API.Test.Unit.Controllers;

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
            new() {Id = Guid.Parse("ab24fde6-495b-45b6-be3c-1343939b646a"), Title = "Recipe 1"},
            new() {Id = Guid.Parse("fe0efe1e-eab7-4ca4-a059-e51de04b0eed"), Title = "Recipe 2"},
            new() {Id = Guid.Parse("a4f5ceb4-3d74-444f-a05f-57e8cfd42061"), Title = "Recipe 3"}
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
    public async Task GetRecipeById_Returns_OkObjectResult()
    {
        var recipeToGet = _recipeInMemoryDatabase.First();

        _recipeRepositoryMock.Setup(r => r.GetRecipeById(recipeToGet.Id))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });

        var response = await _recipeController.GetRecipeById(recipeToGet.Id);
    
        using (new AssertionScope())
        {
            response.Should().BeOfType<OkObjectResult>();
            response.Should().BeAssignableTo<OkObjectResult>();
        }
    }
    
    [Fact]
    public async Task GetRecipeById_ReturnsRecipe_WhenIdExists()
    {
        var recipeToGet = _recipeInMemoryDatabase.First();

        _recipeRepositoryMock.Setup(r => r.GetRecipeById(recipeToGet.Id))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
        
        var response = await _recipeController.GetRecipeById(recipeToGet.Id);
        var result = Assert.IsType<OkObjectResult>(response);
        var recipe = (Recipe)result.Value!;

        recipe.Should().BeEquivalentTo(recipeToGet);
    }
    
    [Fact]
    public async Task GetRecipeById_ReturnsStatusCode404_WhenIdDoesNotExist()
    {
        _recipeRepositoryMock.Setup(r => r.GetRecipeById(Guid.NewGuid()))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
        
        var response = await _recipeController.GetRecipeById(Guid.NewGuid());
        var result = (ObjectResult)response;

        result.StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task GetRecipesByTitle_ReturnsOkObjectResult_WhenTitleIsMatched()
    {
        const string recipesToFind = "recipe to get";
        
        _recipeInMemoryDatabase.Add(new Recipe() { Title = "First recipe to get", Id = Guid.NewGuid() });
        _recipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });

        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipesToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _recipeInMemoryDatabase as List<Recipe>;
                
                var recipe = recipeInMemoryDatabase!
                    .Where(r => r.Title!.Contains(partialTitle))
                    .Select(r =>
                    {
                        r.Title!.Contains(partialTitle);

                        return r;
                    })
                    .ToList();
    
                return Task.FromResult((IList<Recipe>)recipe);
            });
    
        var response = await _recipeController.GetRecipesByTitle(recipesToFind);
    
        using (new AssertionScope())
        {
            response.Should().BeOfType<OkObjectResult>();
            response.Should().BeAssignableTo<OkObjectResult>();
        }
    }
    
    [Fact]
    public async Task GetRecipesByTitle_ReturnsRecipe_WhenTitleIsMatched()
    {
        const string recipeTitleToFind = "First recipe to get";

        var recipeToFind = new Recipe() { Title = "First recipe to get", Id = Guid.NewGuid() };
        
        _recipeInMemoryDatabase.Add(recipeToFind);
        _recipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });

        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipeTitleToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _recipeInMemoryDatabase as List<Recipe>;
                
                var recipe = recipeInMemoryDatabase!
                    .Where(r => r.Title!.Contains(partialTitle))
                    .Select(r =>
                    {
                        r.Title!.Contains(partialTitle);

                        return r;
                    })
                    .ToList();
    
                return Task.FromResult((IList<Recipe>)recipe);
            });
        
        var response = await _recipeController.GetRecipesByTitle(recipeTitleToFind);
        var result = Assert.IsType<OkObjectResult>(response);
        var recipe = (List<Recipe>)result.Value!;

        recipe.Should().HaveCount(1);
        recipe[0].Should().BeEquivalentTo(recipeToFind);
    }
    
    [Fact]
    public async Task GetRecipeByTitle_ReturnsStatusCode200AndEmptyList_WhenTitleNotMatched()
    {
        const string recipeTitleToFind = "First recipe to get";

        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipeTitleToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _recipeInMemoryDatabase as List<Recipe>;
                
                var recipe = recipeInMemoryDatabase!
                    .Where(r => r.Title!.Contains(partialTitle))
                    .Select(r =>
                    {
                        r.Title!.Contains(partialTitle);

                        return r;
                    })
                    .ToList();
    
                return Task.FromResult((IList<Recipe>)recipe);
            });
        
        var response = await _recipeController.GetRecipesByTitle(recipeTitleToFind);
        var result = Assert.IsType<OkObjectResult>(response);

        result.Value.Should().BeOfType<List<Recipe>>();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CreateRecipe_Returns_CreatedAtActionResult()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "Newly created Recipe"
        };
        var id = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.CreateRecipe(recipeDto))
            .Returns((RecipeDto dto) =>
            {
                var recipe = new Recipe
                {
                    Id = id,
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
        var id = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.CreateRecipe(recipeDto))
            .Returns((RecipeDto dto) =>
            {
                var recipe = new Recipe
                {
                    Id = id,
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
            recipe.Id.Should().Be(id);
            recipe.Title.Should().Be(recipeDto.Title);
        }
    }

    [Fact]
    public async Task UpdateRecipe_ReturnsStatusCode204_WhenRecipeIsUpdated()
    {
        var recipeToUpdate = _recipeInMemoryDatabase.First();
        var recipeDto = new UpdateRecipeDto
        {
            Id = recipeToUpdate.Id,
            Title = "Updated Recipe Title"
        };
        
        _recipeRepositoryMock.Setup(r => r.UpdateRecipe(recipeDto))
            .Returns((UpdateRecipeDto dto) =>
            {
                var enumerable = _recipeInMemoryDatabase
                    .Where(r => r.Id == dto.Id)
                    .Select(r => 
                    { 
                        r.Title = dto.Title; 
                        return r;
                    });

                var test = _recipeInMemoryDatabase
                    .Any(r => r.Id == dto.Id);

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
            Id = Guid.NewGuid(),
            Title = "Non existing Recipe"
        };
        
        _recipeRepositoryMock.Setup(r => r.UpdateRecipe(recipeDto))
            .Returns((UpdateRecipeDto dto) =>
            {
                var enumerable = _recipeInMemoryDatabase
                    .Where(r => r.Id == dto.Id)
                    .Select(r => 
                    { 
                        r.Title = dto.Title; 
                        return r;
                    });

                var test = _recipeInMemoryDatabase
                    .Any(r => r.Id == dto.Id);

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
        
        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(recipeToDelete.Id))
            .Returns((Guid recipeId) =>
            {
                var recipes = _recipeInMemoryDatabase
                    .Where(r => r.Id == recipeId)
                    .ToList();
    
                var test = recipes.RemoveAll(r => r.Id == recipeId);
    
                return Task.FromResult(test == 1 ? 1 : 0);
            });
    
        var response = await _recipeController.DeleteRecipe(recipeToDelete.Id);
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
            .Returns((Guid recipeId) =>
            {
                var recipes = _recipeInMemoryDatabase
                    .Where(r => r.Id == recipeId)
                    .ToList();
    
                var test = recipes
                    .RemoveAll(r => r.Id == recipeId);
    
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