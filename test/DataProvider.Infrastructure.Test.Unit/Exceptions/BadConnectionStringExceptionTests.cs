using System;
using FluentAssertions;
using FluentAssertions.Execution;
using DataProvider.Infrastructure.Exceptions;
using Xunit;

namespace DataProvider.Infrastructure.Test.Unit.Exceptions;

public class BadConnectionStringExceptionTests
{
    [Fact]
    public void BadConnectionStringException_ThrowsExceptionWithoutAnyMessage()
    {
        Action action = () => throw new BadConnectionStringException();
        
        action.Should().Throw<BadConnectionStringException>();
    }

    [Fact]
    public void BadConnectionStringException_ReturnsCustomMessage_WhenIsPassed()
    {
        var exceptionMessage = "Custom message";

        Action action = () => throw new BadConnectionStringException(exceptionMessage);
        
        action.Should().Throw<BadConnectionStringException>().WithMessage(exceptionMessage);
    }

    [Fact]
    public void BadConnectionStringException_ReturnsCustomMessageAndInnerException_WhenIsPassed()
    {
        var exceptionMessage = "Custom message";
        var innerException = new ArgumentNullException();

        Action action = () => throw new BadConnectionStringException(exceptionMessage, innerException);

        using (new AssertionScope())
        {
            action.Should()
                .Throw<BadConnectionStringException>()
                .WithMessage(exceptionMessage, innerException.Message);
            action.Should()
                .Throw<BadConnectionStringException>()
                .WithInnerException<ArgumentNullException>();
        }
    }
}