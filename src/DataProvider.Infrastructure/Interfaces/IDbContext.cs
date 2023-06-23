using System.Data;

namespace DataProvider.Infrastructure.Interfaces;

public interface IDbContext
{
    public IDbConnection CreateConnection();
}