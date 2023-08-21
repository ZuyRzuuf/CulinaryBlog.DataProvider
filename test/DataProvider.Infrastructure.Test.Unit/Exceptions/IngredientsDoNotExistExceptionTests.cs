using System;
using DataProvider.Infrastructure.Exceptions;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace DataProvider.Infrastructure.Test.Unit.Exceptions;

public class IngredientsDoNotExistExceptionTests
{
    [Fact]
    public void IngredientsDoNotExistException_ThrowsExceptionWithoutAnyMessage()
    {
        Action action = () => throw new NoIngredientsFoundException();
        
        action.Should().Throw<NoIngredientsFoundException>();
    }

    [Fact]
    public void IngredientsDoNotExistException_ReturnsCustomMessage_WhenIsPassed()
    {
        const string exceptionMessage = "Custom message";

        Action action = () => throw new NoIngredientsFoundException(exceptionMessage);
        
        action.Should().Throw<NoIngredientsFoundException>().WithMessage(exceptionMessage);
    }

    [Fact]
    public void IngredientsDoNotExistException_ReturnsCustomMessageAndInnerException_WhenIsPassed()
    {
        const string exceptionMessage = "Custom message";
        var innerException = new ArgumentNullException();

        Action action = () => throw new NoIngredientsFoundException(exceptionMessage, innerException);

        using (new AssertionScope())
        {
            action.Should()
                .Throw<NoIngredientsFoundException>()
                .WithMessage(exceptionMessage, innerException.Message);
            action.Should()
                .Throw<NoIngredientsFoundException>()
                .WithInnerException<ArgumentNullException>();
        }
    }
}