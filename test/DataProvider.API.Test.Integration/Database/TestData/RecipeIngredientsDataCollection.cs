using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using DataProvider.Domain.Entities;

namespace DataProvider.API.Test.Integration.Database.TestData;

public class RecipeIngredientsDataCollection
{
    private readonly Guid _recipeId;
    private readonly Random _rand = new();
    private readonly int _itemsNumber;

    public RecipeIngredientsDataCollection(Guid recipeId)
    {
        _recipeId = recipeId;
        _itemsNumber = _rand.Next(2, 5);
    }

    public List<Ingredient> Ingredients
    {
        get
        {
            return new Faker<Ingredient>()
                .RuleFor(i => i.Id, f => Guid.NewGuid())
                .RuleFor(i => i.Name, f => f.Lorem.Sentence(2))
                .RuleFor(i => i.RecipeId, f => _recipeId)
                .GenerateLazy(_itemsNumber)
                .ToList();
        }
    }
}
