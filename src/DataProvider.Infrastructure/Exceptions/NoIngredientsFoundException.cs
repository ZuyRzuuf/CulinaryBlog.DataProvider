namespace DataProvider.Infrastructure.Exceptions;

[Serializable]
public class NoIngredientsFoundException : Exception
{
    public NoIngredientsFoundException()
    {
    }

    public NoIngredientsFoundException(string message)
        : base(message)
    {
    }

    public NoIngredientsFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}