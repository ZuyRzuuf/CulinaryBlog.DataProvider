using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using DataProvider.Domain.Entities;

namespace DataProvider.API.Test.Integration.Database.TestData;

public record RecipesDataCollection
{
    private const int ItemsNumber = 10;

    public static List<Recipe> Recipes => new Faker<Recipe>()
            .RuleFor(r => r.Id, f => Guid.NewGuid())
            .RuleFor(r => r.Title, f => f.Lorem.Sentence(2))
            .GenerateLazy(ItemsNumber)
            .ToList();
}