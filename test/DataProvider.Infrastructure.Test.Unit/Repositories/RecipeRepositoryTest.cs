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
using Xunit;

namespace DataProvider.Infrastructure.Test.Unit.Repositories;

public class RecipeRepositoryTest
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly IList<Recipe> _recipeInMemoryDatabase;

    public RecipeRepositoryTest()
    {
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _recipeInMemoryDatabase = new List<Recipe>
        {
            new() { Id = Guid.Parse("ab24fde6-495b-45b6-be3c-1343939b646a"), Title = "Recipe 1" },
            new() { Id = Guid.Parse("fe0efe1e-eab7-4ca4-a059-e51de04b0eed"), Title = "Recipe 2" },
            new() { Id = Guid.Parse("a4f5ceb4-3d74-444f-a05f-57e8cfd42061"), Title = "Recipe 3" },
        };
    }

    [Fact]
    public void GetRecipes_Returns_ListOfRecipes()
    {
        _recipeRepositoryMock.Setup(r => r.GetRecipes())
            .Returns(() => Task.FromResult(_recipeInMemoryDatabase));

        var actual = _recipeRepositoryMock.Object.GetRecipes();

        using (new AssertionScope())
        {
            actual.Result.Should().BeOfType<List<Recipe>>();
            actual.Result.Count.Should().Be(_recipeInMemoryDatabase.Count);
        }
    }

    [Fact]
    public async Task GetRecipesByTitle_ReturnsTwoRecipes_WhenPartOfTitleMatch()
    {
        const string recipesToFind = "recipe to get";
        
        _recipeInMemoryDatabase.Add(new Recipe() { Title = "First recipe to get", Id = Guid.NewGuid() });
        _recipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });
        
        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipesToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _recipeInMemoryDatabase as List<Recipe>;
                
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
        
        _recipeInMemoryDatabase.Add(new Recipe() { Title = recipesToFind, Id = Guid.NewGuid() });
        _recipeInMemoryDatabase.Add(new Recipe() { Title = "Second recipe to get", Id = Guid.NewGuid() });
        
        _recipeRepositoryMock.Setup(r => r.GetRecipesByTitle(recipesToFind))
            .Returns((string partialTitle) =>
            {
                var recipeInMemoryDatabase = _recipeInMemoryDatabase as List<Recipe>;
                
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
                var recipeInMemoryDatabase = _recipeInMemoryDatabase as List<Recipe>;
                
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
    public async Task GetRecipesById_ReturnsRecipe_WhenIdExists()
    {
        var recipeToGet = _recipeInMemoryDatabase.First();
        _recipeRepositoryMock.Setup(r => r.GetRecipeById(recipeToGet.Id))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });
                

        var actual = await _recipeRepositoryMock.Object.GetRecipeById(recipeToGet.Id);

        using (new AssertionScope())
        {
            actual.Should().BeOfType<Recipe>();
            actual.Title.Should().Be(recipeToGet.Title);
            actual.Id.Should().Be(recipeToGet.Id);
        }
    }

    [Fact]
    public async Task GetRecipesById_ThrowsRecipeDoesNotExistException_WhenIdDoesNotExists()
    {
        var recipeToGet = Guid.NewGuid();
        
        _recipeRepositoryMock.Setup(r => r.GetRecipeById(recipeToGet))
            .Returns((Guid id) =>
            {
                var recipe = _recipeInMemoryDatabase
                    .SingleOrDefault(r => r.Id == id);

                return Task.FromResult(recipe)!;
            });

        var actual = await _recipeRepositoryMock.Object.GetRecipeById(recipeToGet);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task CreateRecipe_ReturnsRecipe_WhenRecipeIsCreated()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "Newly created Recipe"
        };
        var id = Guid.NewGuid();
        var numberRecipesInDatabase = _recipeInMemoryDatabase.Count;
        
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

        var createdRecipe = await _recipeRepositoryMock.Object.CreateRecipe(recipeDto);
        
        using (new AssertionScope())
        {
            createdRecipe.Should()
                .NotBeNull()
                .And
                .BeOfType<Recipe>();
            createdRecipe.Id.Should().Be(id);
            createdRecipe.Title.Should().Be(recipeDto.Title);
            _recipeInMemoryDatabase.Should()
                .HaveCount(numberRecipesInDatabase + 1)
                .And
                .Contain(createdRecipe);
        }
    }

    [Fact]
    public async Task UpdateRecipe_Returns1_WhenRecipeIsUpdated()
    {
        var recipeToUpdate = _recipeInMemoryDatabase.First();
        var recipeDto = new UpdateRecipeDto
        {
            Id = recipeToUpdate.Id,
            Title = "Updated title"
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

        var result = await _recipeRepositoryMock.Object.UpdateRecipe(recipeDto);

        result.Should().Be(0);
    }

    [Fact]
    public async Task DeleteRecipe_Returns1_WhenRecipeIsDeleted()
    {
        var recipeToDelete = _recipeInMemoryDatabase.First();
        var id = recipeToDelete.Id;

        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(id))
            .Returns((Guid recipeId) =>
            {
                var recipes = _recipeInMemoryDatabase
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
                var recipes = _recipeInMemoryDatabase
                    .Where(r => r.Id == recipeId).ToList();

                var test = recipes.RemoveAll(r => r.Id == id);
    
                return Task.FromResult(test == 1 ? 1 : 0);
            });
    
        var result = await _recipeRepositoryMock.Object.DeleteRecipe(id);
    
        result.Should().Be(0);
    }
}