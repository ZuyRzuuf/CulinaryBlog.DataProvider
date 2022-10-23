using System;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;

public class RecipeRepositoryFixture : DatabaseSetupFixture, IDisposable
{
    public RecipeRepositoryFixture() 
    {
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
    }
}