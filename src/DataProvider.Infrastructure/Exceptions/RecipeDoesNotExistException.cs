namespace DataProvider.Infrastructure.Exceptions;

[Serializable]
public class RecipeDoesNotExistException : Exception
{
    public RecipeDoesNotExistException()
    {
    }

    public RecipeDoesNotExistException(string message)
        : base(message)
    {
    }

    public RecipeDoesNotExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}