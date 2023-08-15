using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using DataProvider.Domain.Dto;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using DataProvider.Domain.Queries;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace DataProvider.Infrastructure.Test.Unit.Repositories;

public class RecipeRepositoryTest
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly IList<Recipe> _basicRecipeInMemoryDatabase;
    private readonly IList<RecipeQuery> _recipeInMemoryDatabase;

    public RecipeRepositoryTest()
    {
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _basicRecipeInMemoryDatabase = new List<Recipe>
        {
            new() { Id = Guid.Parse("ab24fde6-495b-45b6-be3c-1343939b646a"), Title = "Recipe 1" },
            new() { Id = Guid.Parse("fe0efe1e-eab7-4ca4-a059-e51de04b0eed"), Title = "Recipe 2" },
            new() { Id = Guid.Parse("a4f5ceb4-3d74-444f-a05f-57e8cfd42061"), Title = "Recipe 3" },
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
    public void GetRecipes_Returns_ListOfRecipes()
    {
        _recipeRepositoryMock.Setup(r => r.GetRecipes())
            .Returns(() => Task.FromResult(_basicRecipeInMemoryDatabase));

        var actual = _recipeRepositoryMock.Object.GetRecipes();

        using (new AssertionScope())
        {
            actual.Result.Should().BeOfType<List<Recipe>>();
            actual.Result.Count.Should().Be(_basicRecipeInMemoryDatabase.Count);
        }
    }

    [Fact]
    public async Task GetRecipesByTitle_ReturnsTwoRecipes_WhenPartOfTitleMatch()
    {
        const string recipesToFind = "recipe to get";
        
        _basicRecipeInMemoryDatabase.Add(new Recipe() { Title = "First recipe to get", Id = Guid.NewGuid() });
        _basicRecipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });
        
        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipesToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _basicRecipeInMemoryDatabase as List<Recipe>;
                
                var recipes = recipeInMemoryDatabase!
                    .Where(r => r.Title!.Contains(partialTitle))
                    .Select(r =>
                    {
                        r.Title!.Contains(partialTitle);

                        return r;
                    }).ToList();
                
                var result = Task.FromResult((IList<Recipe>)recipes);

                return result;
            });

        var actual = await _recipeRepositoryMock.Object.GetRecipesByTitle(recipesToFind);

        actual.Should()
            .BeOfType<List<Recipe>>()
            .And
            .NotBeNull()
            .And
            .HaveCount(2);
    }

    [Fact]
    public async Task GetRecipesByTitle_ReturnsOneRecipe_WhenWholeTitleMatch()
    {
        const string recipesToFind = "First recipe to get";
        
        _basicRecipeInMemoryDatabase.Add(new Recipe() { Title = recipesToFind, Id = Guid.NewGuid() });
        _basicRecipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });
        
        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipesToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _basicRecipeInMemoryDatabase as List<Recipe>;
                
                var recipes = recipeInMemoryDatabase!
                    .Where(r => r.Title!.Contains(partialTitle))
                    .Select(r =>
                    {
                        r.Title!.Contains(partialTitle);

                        return r;
                    }).ToList();
                
                var result = Task.FromResult((IList<Recipe>)recipes);

                return result;
            });

        var actual = await _recipeRepositoryMock.Object.GetRecipesByTitle(recipesToFind);

        actual.Should()
            .BeOfType<List<Recipe>>()
            .And
            .NotBeNull()
            .And
            .HaveCount(1);
    }

    [Fact]
    public async Task GetRecipesByTitle_ReturnsEmptyList_WhenNoTitleMatch()
    {
        const string recipesToFind = "Non existing recipe";
        
        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipesToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _basicRecipeInMemoryDatabase as List<Recipe>;
                
                var recipes = recipeInMemoryDatabase!
                    .Where(r => r.Title!.Contains(partialTitle))
                    .Select(r =>
                    {
                        r.Title!.Contains(partialTitle);

                        return r;
                    }).ToList();
                
                var result = Task.FromResult((IList<Recipe>)recipes);

                return result;
            });

        var actual = await _recipeRepositoryMock.Object.GetRecipesByTitle(recipesToFind);

        actual.Should()
            .BeOfType<List<Recipe>>()
            .And
            .BeEmpty()
            .And
            .HaveCount(0);
    }

    [Fact]
    public async Task GetBasicRecipesById_ReturnsRecipe_WhenIdExists()
    {
        var recipeToGet = _basicRecipeInMemoryDatabase.First();
        _recipeRepositoryMock.Setup(r => r.GetBasicRecipeById(recipeToGet.Id))
            .Returns((Guid id) =>
            {
                var recipe = _basicRecipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
                

        var actual = await _recipeRepositoryMock.Object.GetBasicRecipeById(recipeToGet.Id);

        using (new AssertionScope())
        {
            actual.Should().BeOfType<Recipe>();
            actual.Title.Should().Be(recipeToGet.Title);
            actual.Id.Should().Be(recipeToGet.Id);
        }
    }

    [Fact]
    public async Task GetBasicRecipesById_ThrowsRecipeDoesNotExistException_WhenIdDoesNotExists()
    {
        var recipeToGet = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.GetBasicRecipeById(recipeToGet))
            .Returns((Guid id) =>
            {
                var recipe = _basicRecipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });

        var actual = await _recipeRepositoryMock.Object.GetBasicRecipeById(recipeToGet);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task GetRecipesById_ReturnsRecipe_WhenIdExists()
    {
        var recipeToGet = _recipeInMemoryDatabase.First();
        
        _recipeRepositoryMock.Setup(r => r.GetRecipeWithIngredientsById(recipeToGet.Id))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
    
        var response = await _recipeRepositoryMock.Object.GetRecipeWithIngredientsById(recipeToGet.Id);

        using (new AssertionScope())
        {
            response.Should().BeOfType<RecipeQuery>();
            response.Should().BeEquivalentTo(recipeToGet);
        }
    }
    
    [Fact]
    public async Task GetRecipesById_ThrowsRecipeDoesNotExistException_WhenIdDoesNotExists()
    {
        var recipeToGet = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.GetRecipeWithIngredientsById(recipeToGet))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
    
        var response = await _recipeRepositoryMock.Object.GetRecipeWithIngredientsById(recipeToGet);

        response.Should().BeNull();
    }

    [Fact]
    public async Task CreateRecipe_ReturnsRecipe_WhenRecipeIsCreated()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "Newly created Recipe"
        };
        var id = Guid.NewGuid();
        var numberRecipesInDatabase = _basicRecipeInMemoryDatabase.Count;
        
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

        var createdRecipe = await _recipeRepositoryMock.Object.CreateRecipe(recipeDto);
        
        using (new AssertionScope())
        {
            createdRecipe.Should()
                .NotBeNull()
                .And
                .BeOfType<Recipe>();
            createdRecipe.Id.Should().Be(id);
            createdRecipe.Title.Should().Be(recipeDto.Title);
            _basicRecipeInMemoryDatabase.Should()
                .HaveCount(numberRecipesInDatabase + 1)
                .And
                .Contain(createdRecipe);
        }
    }

    [Fact]
    public async Task UpdateRecipe_Returns1_WhenRecipeIsUpdated()
    {
        var recipeToUpdate = _basicRecipeInMemoryDatabase.First();
        var recipeDto = new UpdateRecipeDto
        {
            Id = recipeToUpdate.Id,
            Title = "Updated title"
        };

        _recipeRepositoryMock.Setup(r => r.UpdateRecipe(recipeDto))
            .Returns((UpdateRecipeDto dto) =>
            {
                var enumerable = _basicRecipeInMemoryDatabase
                    .Where(r => r.Id == dto.Id)
                    .Select(r => 
                    { 
                        r.Title = dto.Title; 
                        return r;
                    });

                var test = _basicRecipeInMemoryDatabase
                    .Any(r => r.Id == dto.Id);

                return Task.FromResult(test ? 1 : 0);
            });

        var result = await _recipeRepositoryMock.Object.UpdateRecipe(recipeDto);

        result.Should().Be(1);
    }

    [Fact]
    public async Task UpdateRecipe_Returns0_WhenRecipeDoesNotExist()
    {
        var recipeDto = new UpdateRecipeDto
        {
            Id = Guid.NewGuid(),
            Title = "Non existing recipe"
        };

        _recipeRepositoryMock.Setup(r => r.UpdateRecipe(recipeDto))
            .Returns((UpdateRecipeDto dto) =>
            {
                var enumerable = _basicRecipeInMemoryDatabase
                    .Where(r => r.Id == dto.Id)
                    .Select(r => 
                    { 
                        r.Title = dto.Title; 
                        return r;
                    });

                var test = _basicRecipeInMemoryDatabase
                    .Any(r => r.Id == dto.Id);

                return Task.FromResult(test ? 1 : 0);
            });

        var result = await _recipeRepositoryMock.Object.UpdateRecipe(recipeDto);

        result.Should().Be(0);
    }

    [Fact]
    public async Task DeleteRecipe_Returns1_WhenRecipeIsDeleted()
    {
        var recipeToDelete = _basicRecipeInMemoryDatabase.First();
        var id = recipeToDelete.Id;

        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(id))
            .Returns((Guid recipeId) =>
            {
                var recipes = _basicRecipeInMemoryDatabase
                    .Where(r => r.Id == recipeId).ToList();
                
                var test = recipes.RemoveAll(r => r.Id == recipeId);

                return Task.FromResult(test == 1 ? 1 : 0);
            });

        var result = await _recipeRepositoryMock.Object.DeleteRecipe(id);

        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteRecipe_Returns0_WhenRecipeDoesNotExist()
    {
        var id = Guid.NewGuid();
    
        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(id))
            .Returns((Guid recipeId) =>
            {
                var recipes = _basicRecipeInMemoryDatabase
                    .Where(r => r.Id == recipeId).ToList();

                var test = recipes.RemoveAll(r => r.Id == id);
    
                return Task.FromResult(test == 1 ? 1 : 0);
            });
    
        var result = await _recipeRepositoryMock.Object.DeleteRecipe(id);
    
        result.Should().Be(0);
    }
}