namespace RecipesDataProvider.Infrastructure.Exceptions;

[Serializable]
public class DatabaseConnectionProblemException : Exception
{
    public DatabaseConnectionProblemException()
    {
    }

    public DatabaseConnectionProblemException(string message = "Cannot connect to database")
        : base(message)
    {
    }

    public DatabaseConnectionProblemException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}