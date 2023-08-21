using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataProvider.Domain.Entities;
using DataProvider.Domain.Interfaces;
using DataProvider.Infrastructure.Exceptions;
using FluentAssertions;
using MySql.Data.MySqlClient;

namespace DataProvider.Infrastructure.Test.Unit.Repositories;

public class IngredientRepositoryTests
{
    private readonly Mock<IIngredientRepository> _ingredientRepositoryMock = new();
    private readonly IList<Ingredient> _ingredientsInMemoryDatabase = new List<Ingredient>
    {
        new Ingredient
        {
            Id = Guid.Parse("d0b9c9a0-0b1a-4e1a-8f1a-0b1a0b1a0b1a"),
            Name = "Ingredient 1",
            Description = "Description for Ingredient 1",
            Quantity = 1,
            QuantityType = "Unit 1",
            RecipeId = Guid.Parse("d0b9c9a0-0b1a-4e1a-8f1a-0b1a0b1a0b1a")
        },
        new Ingredient
        {
            Id = Guid.Parse("d0b9c9a0-0b1a-4e1a-8f1a-0b1a0b1a0b1b"),
            Name = "Ingredient 2",
            Description = "Description for Ingredient 2",
            Quantity = 2,
            QuantityType = "Unit 2",
            RecipeId = Guid.Parse("d0b9c9a0-0b1a-4e1a-8f1a-0b1a0b1a0b1b")
        }
    };

    [Fact]
    public async Task GetRecipeIngredientsByRecipeIdAsync_WhenRecipeIdExists_ShouldReturnIngredients()
    {
        // Arrange
        var recipeId = Guid.Parse("d0b9c9a0-0b1a-4e1a-8f1a-0b1a0b1a0b1a");
        var expectedIngredients = _ingredientsInMemoryDatabase
            .Where(x => x.RecipeId == recipeId).ToList();
        
        _ingredientRepositoryMock
            .Setup(x => x.GetRecipeIngredientsByRecipeIdAsync(recipeId))
            .ReturnsAsync(expectedIngredients);
        
        // Act
        var actualIngredients = await _ingredientRepositoryMock.Object.GetRecipeIngredientsByRecipeIdAsync(recipeId);
        
        // Assert
        actualIngredients.Should().BeEquivalentTo(expectedIngredients);
    }
    
    [Fact]
    public async Task GetRecipeIngredientsByRecipeIdAsync_WhenNoIngredientsExist_ShouldThrowException()
    {
        // Arrange
        _ingredientRepositoryMock
            .Setup(x => x.GetRecipeIngredientsByRecipeIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new NoIngredientsFoundException());
        
        // Act
        var act = async () => 
            await _ingredientRepositoryMock.Object.GetRecipeIngredientsByRecipeIdAsync(It.IsAny<Guid>());
    
        // Assert
        await act.Should().ThrowAsync<NoIngredientsFoundException>();
    }
    
    [Fact]
    public async Task GetRecipeIngredientsByRecipeIdAsync_WhenMysqlExceptionOccurs_ShouldThrowException()
    {
        // Arrange
        _ingredientRepositoryMock
            .Setup(x => x.GetRecipeIngredientsByRecipeIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new UnknownDatabaseException());
        
        // Act
        var act = async () => 
            await _ingredientRepositoryMock.Object.GetRecipeIngredientsByRecipeIdAsync(It.IsAny<Guid>());
    
        // Assert
        await act.Should().ThrowAsync<UnknownDatabaseException>();
    }
}