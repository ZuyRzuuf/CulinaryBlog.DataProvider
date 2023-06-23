namespace DataProvider.Infrastructure.Exceptions;

[Serializable]
public class UnknownDatabaseException : Exception
{
    public UnknownDatabaseException()
    {
    }

    public UnknownDatabaseException(string message = "No database selected or unknown database")
        : base(message)
    {
    }

    public UnknownDatabaseException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}