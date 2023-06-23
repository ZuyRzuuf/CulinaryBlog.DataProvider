namespace DataProvider.Infrastructure.Exceptions;

[Serializable]
public class BadConnectionStringException : Exception
{
    public BadConnectionStringException()
    {
    }

    public BadConnectionStringException(string message)
        : base(message)
    {
    }

    public BadConnectionStringException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}