using System.Data;

namespace RecipesDataProvider.Infrastructure.Interfaces;

public interface IDbContext
{
    public IDbConnection CreateConnection();
}