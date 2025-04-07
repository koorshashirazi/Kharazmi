using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Bogus;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Validation;
using NSubstitute;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Functional
{
    [Collection("ResultTCollection")]
    [ExcludeFromCodeCoverage]
    public class ResultTTests
    {
        private readonly Faker _faker = new();

        [Fact]
        public void HasValue_WithValueAndNotFailed_ReturnsTrue()
        {
            // Arrange
            var value = _faker.Random.Int();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(value, false, message);

            // Act
            var hasValue = result.HasValue();

            // Assert
            hasValue.Should().BeTrue();
        }

        [Fact]
        public void HasValue_WithNullValueAndNotFailed_ReturnsFalse()
        {
            // Arrange
            int? value = null;
            var message = new FriendlyResultMessage();
            var result = new Result<int?>(value, false, message);

            // Act
            var hasValue = result.HasValue();

            // Assert
            hasValue.Should().BeFalse();
        }

        [Fact]
        public void HasValue_WithValueAndFailed_ReturnsFalse()
        {
            // Arrange
            var value = _faker.Random.Int();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(value, true, message);

            // Act
            var hasValue = result.HasValue();

            // Assert
            hasValue.Should().BeFalse();
        }

        [Fact]
        public void Value_HasValue_ReturnsValue()
        {
            // Arrange
            var value = _faker.Random.Int();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(value, false, message);

            // Act
            var retrievedValue = result.Value;

            // Assert
            retrievedValue.Should().Be(value);
        }

        [Fact]
        public void Value_NoValue_ThrowsInvalidOperationException()
        {
            // Arrange
            var message = new FriendlyResultMessage();
            var result = new Result<int>(default, true, message);

            // Act
            Action act = () =>
            {
                var _ = result.Value;
            };

            // Assert
            act.Should().Throw<InvalidOperationException>().WithMessage("There is no value for failure.");
        }

        [Fact]
        public void WithException_SetsException()
        {
            // Arrange
            var exception = new Exception(_faker.Lorem.Sentence());
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithException(exception);

            // Assert
            updatedResult.Exception.Should().Be(exception);
        }

        [Fact]
        public void WithInternalMessages_SetsInternalMessages()
        {
            // Arrange
            var messages = new List<InternalResultMessage>
                { new(new Faker().Commerce.Department(10), new Faker().Lorem.Sentence(10)), new() };
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithInternalMessages(messages);

            // Assert
            updatedResult.InternalMessages.Value.Should().BeEquivalentTo(messages);
        }  
        [Fact]
        public void WithInternalMessages_SetsSameInternalMessages()
        {
            // Arrange
            var messages = new List<InternalResultMessage> { new(), new() };
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithInternalMessages(messages);

            // Assert
            updatedResult.InternalMessages.Value.Count.Should().Be(1);
        }

        [Fact]
        public void WithMessages_SetsMessages()
        {
            // Arrange
            var messages = new List<FriendlyResultMessage> { new("description"), new() };
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithMessages(messages);

            // Assert
            updatedResult.Messages.Should().BeEquivalentTo(messages);
        }  
        
        [Fact]
        public void WithMessages_SetsSameMessages()
        {
            // Arrange
            var messages = new List<FriendlyResultMessage> { new(), new() };
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithMessages(messages);

            // Assert
            updatedResult.Messages.Count.Should().Be(1);
        }

        [Fact]
        public void WithValidationMessages_SetsValidationMessages()
        {
            // Arrange
            var validationFailures = new List<ValidationFailure>
                { new("Property1", "Error1"), new("Property2", "Error2") };
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);


            // Act
            var updatedResult = result.WithValidationMessages(validationFailures);

            // Assert
            updatedResult.ValidationMessages.Should().BeEquivalentTo(validationFailures);
        }

        [Fact]
        public void WithRedirectUrl_SetsRedirectUrl()
        {
            // Arrange
            var url = _faker.Internet.Url();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithRedirectUrl(url);

            // Assert
            updatedResult.RedirectToUrl.Should().Be(url);
        }

        [Fact]
        public void WithJsHandler_SetsJsHandler()
        {
            // Arrange
            var handler = _faker.Random.String();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithJsHandler(handler);

            // Assert
            updatedResult.JsHandler.Should().Be(handler);
        }

        [Fact]
        public void WithRequestPath_SetsRequestPath()
        {
            // Arrange
            var path = _faker.System.FilePath();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithRequestPath(path);

            // Assert
            updatedResult.RequestPath.Should().Be(path);
        }

        [Fact]
        public void WithTraceId_SetsTraceId()
        {
            // Arrange
            var traceId = Guid.NewGuid().ToString();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithTraceId(traceId);

            // Assert
            updatedResult.TraceId.Should().Be(traceId);
        }

        [Fact]
        public void WithStatus_SetsStatus()
        {
            // Arrange
            var status = _faker.Random.Int();
            var message = new FriendlyResultMessage();
            var result = new Result<int>(1, false, message);

            // Act
            var updatedResult = result.WithStatus(status);

            // Assert
            updatedResult.ResponseStatus.Should().Be(status);
        }
    }
}