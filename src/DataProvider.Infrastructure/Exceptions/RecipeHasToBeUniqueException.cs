namespace DataProvider.Infrastructure.Exceptions;

[Serializable]
public class RecipeHasToBeUniqueException : Exception
{
    public RecipeHasToBeUniqueException()
    {
    }

    public RecipeHasToBeUniqueException(string message = "Recipe has to be unique")
        : base(message)
    {
    }

    public RecipeHasToBeUniqueException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}