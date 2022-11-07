using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Components.Web;
using Moq;
using RecipesDataProvider.Domain.Dto;
using RecipesDataProvider.Domain.Entities;
using RecipesDataProvider.Domain.Interfaces;
using Xunit;

namespace RecipesDataProvider.Infrastructure.Test.Unit.Repositories;

public class RecipeRepositoryTest
{
    private readonly Mock<IRecipeRepository> _recipeRepositoryMock;
    private readonly IList<Recipe> _recipeInMemoryDatabase;

    public RecipeRepositoryTest()
    {
        _recipeRepositoryMock = new Mock<IRecipeRepository>();
        _recipeInMemoryDatabase = new List<Recipe>
        {
            new() { Uuid = Guid.Parse("ab24fde6-495b-45b6-be3c-1343939b646a"), Title = "Recipe 1" },
            new() { Uuid = Guid.Parse("fe0efe1e-eab7-4ca4-a059-e51de04b0eed"), Title = "Recipe 2" },
            new() { Uuid = Guid.Parse("a4f5ceb4-3d74-444f-a05f-57e8cfd42061"), Title = "Recipe 3" },
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
    public async Task CreateRecipe_ReturnsRecipe_WhenRecipeIsCreated()
    {
        var recipeDto = new CreateRecipeDto
        {
            Title = "Newly created Recipe"
        };
        var uuid = Guid.NewGuid();
        var numberRecipesInDatabase = _recipeInMemoryDatabase.Count;
        
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

        var createdRecipe = await _recipeRepositoryMock.Object.CreateRecipe(recipeDto);
        
        using (new AssertionScope())
        {
            createdRecipe.Should()
                .NotBeNull()
                .And
                .BeOfType<Recipe>();
            createdRecipe.Uuid.Should().Be(uuid);
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
            Uuid = recipeToUpdate.Uuid,
            Title = "Updated title"
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

        var result = await _recipeRepositoryMock.Object.UpdateRecipe(recipeDto);

        result.Should().Be(1);
    }

    [Fact]
    public async Task UpdateRecipe_Returns0_WhenRecipeDoesNotExist()
    {
        var recipeDto = new UpdateRecipeDto
        {
            Uuid = Guid.NewGuid(),
            Title = "Non existing recipe"
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

        var result = await _recipeRepositoryMock.Object.UpdateRecipe(recipeDto);

        result.Should().Be(0);
    }

    [Fact]
    public async Task DeleteRecipe_Returns1_WhenRecipeIsDeleted()
    {
        var recipeToDelete = _recipeInMemoryDatabase.First();
        var uuid = recipeToDelete.Uuid;

        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(uuid))
            .Returns((Guid recipeUuid) =>
            {
                var recipes = _recipeInMemoryDatabase
                    .Where(r => r.Uuid == recipeUuid).ToList();
                
                var test = recipes.RemoveAll(r => r.Uuid == recipeUuid);

                return Task.FromResult(test == 1 ? 1 : 0);
            });

        var result = await _recipeRepositoryMock.Object.DeleteRecipe(uuid);

        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteRecipe_Returns0_WhenRecipeDoesNotExist()
    {
        var uuid = Guid.NewGuid();
    
        _recipeRepositoryMock.Setup(r => r.DeleteRecipe(uuid))
            .Returns((Guid recipeUuid) =>
            {
                var recipes = _recipeInMemoryDatabase
                    .Where(r => r.Uuid == recipeUuid).ToList();

                var test = recipes.RemoveAll(r => r.Uuid == uuid);
    
                return Task.FromResult(test == 1 ? 1 : 0);
            });
    
        var result = await _recipeRepositoryMock.Object.DeleteRecipe(uuid);
    
        result.Should().Be(0);
    }
}