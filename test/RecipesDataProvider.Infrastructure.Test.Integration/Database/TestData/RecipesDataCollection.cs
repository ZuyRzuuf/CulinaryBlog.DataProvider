using System;
using System.Collections.Generic;
using RecipesDataProvider.Domain.Entities;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Database.TestData;

public record RecipesDataCollection
{
    public static List<Recipe> Recipes => new()
    {
        new Recipe() { Uuid = Guid.NewGuid(), Title = "Recipe 01" },
        new Recipe() { Uuid = Guid.NewGuid(), Title = "Recipe 02" },
        new Recipe() { Uuid = Guid.NewGuid(), Title = "Recipe 03" },
    };
}