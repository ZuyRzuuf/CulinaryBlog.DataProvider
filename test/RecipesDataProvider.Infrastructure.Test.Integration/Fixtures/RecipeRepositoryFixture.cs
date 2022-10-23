using System;
using RecipesDataProvider.Infrastructure.Repositories;

namespace RecipesDataProvider.Infrastructure.Test.Integration.Fixtures;

public class RecipeRepositoryFixture : DatabaseSetupFixture, IDisposable
{
    public RecipeRepository Sut { get; }

    public RecipeRepositoryFixture() 
    {
        Sut = new RecipeRepository(MysqlTestContext);
    }
    
    /// <inheritdoc />
    public void Dispose()
    {
    }
}