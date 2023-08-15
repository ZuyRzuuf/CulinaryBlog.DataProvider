namespace DataProvider.Infrastructure.Exceptions;

[Serializable]
public class InconsistentDatabaseOperationException : Exception
{
    public InconsistentDatabaseOperationException()
    {
    }

    public InconsistentDatabaseOperationException(string message = "Operation on database failed")
        : base(message)
    {
    }

    public InconsistentDatabaseOperationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}