using System;
using FluentAssertions;
using FluentAssertions.Execution;
using DataProvider.Infrastructure.Exceptions;
using Xunit;

namespace DataProvider.Infrastructure.Test.Unit.Exceptions;

public class DatabaseConnectionProblemExceptionTests
{
    [Fact]
    public void DatabaseConnectionProblemExceptionTest_ThrowsExceptionWithoutAnyMessage()
    {
        Action action = () => throw new DatabaseConnectionProblemException();
        
        action.Should().Throw<DatabaseConnectionProblemException>();
    }

    [Fact]
    public void DatabaseConnectionProblemExceptionTest_ReturnsCustomMessage_WhenIsPassed()
    {
        var exceptionMessage = "Custom message";

        Action action = () => throw new DatabaseConnectionProblemException(exceptionMessage);
        
        action.Should().Throw<DatabaseConnectionProblemException>().WithMessage(exceptionMessage);
    }

    [Fact]
    public void DatabaseConnectionProblemExceptionTest_ReturnsCustomMessageAndInnerException_WhenIsPassed()
    {
        var exceptionMessage = "Custom message";
        var innerException = new ArgumentNullException();

        Action action = () => throw new DatabaseConnectionProblemException(exceptionMessage, innerException);

        using (new AssertionScope())
        {
            action.Should()
                .Throw<DatabaseConnectionProblemException>()
                .WithMessage(exceptionMessage, innerException.Message);
            action.Should()
                .Throw<DatabaseConnectionProblemException>()
                .WithInnerException<ArgumentNullException>();
        }
    }
}