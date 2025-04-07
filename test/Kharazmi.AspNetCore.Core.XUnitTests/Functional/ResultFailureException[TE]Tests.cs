        using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Contracts;
using Kharazmi.AspNetCore.Core.Functional;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Functional
{
    [Collection(nameof(ResultFailureExceptionTests))]
    [ExcludeFromCodeCoverage]
    public class ResultFailureExceptionTests
    {
        [Fact]
        public void ResultFailureException_Generic_Constructor_SetsError()
        {
            // Arrange
            var expectedError = "Test Error";

            // Act
            var exception = new ResultFailureException<string>(expectedError);

            // Assert
            exception.Error.Should().Be(expectedError);
            exception.Message.Should().Be(ResultMessages.ValueIsInaccessibleForFailure);
        }
    }
}