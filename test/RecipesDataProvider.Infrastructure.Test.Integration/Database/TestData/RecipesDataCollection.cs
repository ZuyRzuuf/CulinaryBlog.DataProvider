using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using RecipesDataProvider.Domain.Entities;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Database.TestData;

public record RecipesDataCollection
{
    public static List<Recipe> Recipes => new Faker<Recipe>()
            .RuleFor(r => r.Uuid, f => Guid.NewGuid())
            .RuleFor(r => r.Title, f => f.Lorem.Sentence(2))
            .GenerateLazy(10)
            .ToList();
}