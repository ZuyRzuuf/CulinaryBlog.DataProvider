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
using DataProvider.Domain.Queries;
using Xunit;

namespace DataProvider.API.Test.Unit.Controllers;

public class RecipeControllerTests
{
    private readonly RecipeController _recipeController;
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly IList<Recipe> _basicRecipeInMemoryDatabase;
    private readonly IList<RecipeQuery> _recipeInMemoryDatabase;
    
    public RecipeControllerTests()
    {
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        var loggerMock = new Mock<ILogger<RecipeController>>();
        _recipeController = new RecipeController(_recipeRepositoryMock.Object, loggerMock.Object);
        _basicRecipeInMemoryDatabase = new List<Recipe>
        {
            new()
            {
                Id = Guid.Parse("ab24fde6-495b-45b6-be3c-1343939b646a"), 
                Title = "Recipe 1"
            },
            new()
            {
                Id = Guid.Parse("fe0efe1e-eab7-4ca4-a059-e51de04b0eed"), 
                Title = "Recipe 2"
            },
            new()
            {
                Id = Guid.Parse("a4f5ceb4-3d74-444f-a05f-57e8cfd42061"), 
                Title = "Recipe 3"
            }
        };
        _recipeInMemoryDatabase = new List<RecipeQuery>
        {
            new()
            {
                Id = Guid.Parse("ab24fde6-495b-45b6-be3c-1343939b646a"), 
                Title = "Recipe 1",
                Ingredients = new List<IngredientQuery>
                {
                    new() {Name = "Ingredient 1", Description = "Description 1", Quantity = 10, QuantityType = "g"},
                    new() {Name = "Ingredient 2", Description = "Description 2", Quantity = 5, QuantityType = "szt"},
                    new() {Name = "Ingredient 3", Description = "Description 3", Quantity = 1, QuantityType = "l"}
                },
            },
            new()
            {
                Id = Guid.Parse("fe0efe1e-eab7-4ca4-a059-e51de04b0eed"), 
                Title = "Recipe 2",
                Ingredients = new List<IngredientQuery>
                {
                    new() {Name = "Ingredient 4", Description = "Description 4", Quantity = 10, QuantityType = "g"},
                    new() {Name = "Ingredient 5", Description = "Description 5", Quantity = 5, QuantityType = "szt"},
                    new() {Name = "Ingredient 6", Description = "Description 6", Quantity = 1, QuantityType = "l"}
                },
            },
            new()
            {
                Id = Guid.Parse("a4f5ceb4-3d74-444f-a05f-57e8cfd42061"), 
                Title = "Recipe 3",
                Ingredients = new List<IngredientQuery>
                {
                    new() {Name = "Ingredient 7", Description = "Description 7", Quantity = 10, QuantityType = "g"},
                    new() {Name = "Ingredient 8", Description = "Description 8", Quantity = 5, QuantityType = "szt"},
                    new() {Name = "Ingredient 9", Description = "Description 9", Quantity = 1, QuantityType = "l"}
                },
            }
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
            .ReturnsAsync(_basicRecipeInMemoryDatabase);
        
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
    public async Task GetRecipeWithIngredientsById_Returns_OkObjectResult()
    {
        var recipeToGet = _recipeInMemoryDatabase.First();

        _recipeRepositoryMock.Setup(r => r.GetRecipeWithIngredientsById(recipeToGet.Id))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });

        var response = await _recipeController.GetRecipeWithIngredientsById(recipeToGet.Id);
    
        using (new AssertionScope())
        {
            response.Should().BeOfType<OkObjectResult>();
            response.Should().BeAssignableTo<OkObjectResult>();
        }
    }
    
    [Fact]
    public async Task GetRecipeWithIngredientsById_ReturnsRecipe_WhenIdExists()
    {
        var recipeToGet = _recipeInMemoryDatabase.First();

        _recipeRepositoryMock.Setup(r => r.GetRecipeWithIngredientsById(recipeToGet.Id))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
        
        var response = await _recipeController.GetRecipeWithIngredientsById(recipeToGet.Id);
        var result = Assert.IsType<OkObjectResult>(response);
        var recipe = (RecipeQuery)result.Value!;

        recipe.Should().BeEquivalentTo(recipeToGet);
    }
    
    [Fact]
    public async Task GetRecipeWithIngredientsById_ReturnsStatusCode404_WhenIdDoesNotExist()
    {
        _recipeRepositoryMock.Setup(r => r.GetRecipeWithIngredientsById(Guid.NewGuid()))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
        
        var response = await _recipeController.GetRecipeWithIngredientsById(Guid.NewGuid());
        var result = (ObjectResult)response;

        result.StatusCode.Should().Be(404);
    }
    
    [Fact]
    public async Task GetRecipesByTitle_ReturnsOkObjectResult_WhenTitleIsMatched()
    {
        const string recipesToFind = "recipe to get";
        
        _basicRecipeInMemoryDatabase.Add(new Recipe() { Title = "First recipe to get", Id = Guid.NewGuid() });
        _basicRecipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });

        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipesToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _basicRecipeInMemoryDatabase as List<Recipe>;
                
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
        
        _basicRecipeInMemoryDatabase.Add(recipeToFind);
        _basicRecipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });

        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipeTitleToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _basicRecipeInMemoryDatabase as List<Recipe>;
                
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
                var recipeInMemoryDatabase = _basicRecipeInMemoryDatabase as List<Recipe>;
                
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
                
                _basicRecipeInMemoryDatabase.Add(recipe);

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
                
                _basicRecipeInMemoryDatabase.Add(recipe);

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