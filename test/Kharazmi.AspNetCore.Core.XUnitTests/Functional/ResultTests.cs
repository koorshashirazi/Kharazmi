using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Bogus;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Validation;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Functional
{
    [Collection("ResultTests")]
    [ExcludeFromCodeCoverage]
    public class ResultTests
    {
        private readonly Faker<ValidationFailure> _validationFailureFaker;

        public ResultTests()
        {
            _validationFailureFaker = new Faker<ValidationFailure>()
                .CustomInstantiator(f => new ValidationFailure(f.Lorem.Word().ToString(), f.Lorem.Sentence()));
        }

        [Fact]
        public void Result_Constructor_Sets_Properties_Correctly()
        {
            // Arrange
            var failed = true;
            var friendlyMessage = FriendlyResultMessage.With(new Faker().Lorem.Sentence(20), 10);

            // Act
            var result = new Result(failed, friendlyMessage);

            // Assert
            result.Failed.Should().Be(failed);
            result.FriendlyMessage.Should().BeEquivalentTo(friendlyMessage);
            result.ResultType.Should().Be(failed ? "Failed" : "Succeed");
            result.ResultId.Should().NotBeNullOrEmpty();
            result.CreateAt.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetInternalMessages_Returns_EmptyCollection_Initially()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);

            // Act
            var messages = result.GetInternalMessages();

            // Assert
            messages.Should().BeEmpty();
        }

        [Fact]
        public void WithInternalMessages_Adds_Messages_To_Internal_Collection()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            List<InternalResultMessage> internalMessages = [];
            for (int i = 0; i < 3; i++)
            {
                internalMessages.Add(InternalResultMessage.With(new Faker().Commerce.Department(10),
                    new Faker().Lorem.Sentence(30), new Faker().Lorem.Sentence(10)));
            }

            // Act
            result.WithInternalMessages(internalMessages);
            var messages = result.GetInternalMessages();

            // Assert
            messages.Should().BeEquivalentTo(internalMessages);
        }

        [Fact]
        public void WithInternalMessages_Throws_ArgumentNullException_When_Messages_Is_Null()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            IReadOnlyCollection<InternalResultMessage> messages = null;

            // Act
            Action act = () => result.WithInternalMessages(messages);

            // Assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("messages");
        }

        [Fact]
        public void WithException_Sets_Exception_Property()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var exception = new Exception("Test exception");

            // Act
            result.WithException(exception);

            // Assert
            result.Exception.Should().Be(exception);
        }

        [Fact]
        public void WithException_Aggregates_Exceptions()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var exception1 = new Exception("Test exception 1");
            var exception2 = new Exception("Test exception 2");

            // Act
            result.WithException(exception1);
            result.WithException(exception2);

            // Assert
            result.Exception.Should().BeOfType<AggregateException>();
            ((AggregateException)result.Exception).InnerExceptions.Should().Contain(exception1);
            ((AggregateException)result.Exception).InnerExceptions.Should().Contain(exception2);
        }

        [Fact]
        public void WithException_Handles_Null_Exception()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);

            // Act
            result.WithException(null);

            // Assert
            result.Exception.Should().BeNull();
        }

        [Fact]
        public void WithMessages_Adds_Messages_To_Collection()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            List<FriendlyResultMessage> messages = [];
            for (int i = 0; i < 3; i++)
            {
                messages.Add(FriendlyResultMessage.With(new Faker().Lorem.Sentence(20), 10));
            }

            // Act
            result.WithMessages(messages);

            // Assert
            result.Messages.Should().BeEquivalentTo(messages);
        }

        [Fact]
        public void WithMessages_Throws_ArgumentNullException_When_Messages_Is_Null()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            IReadOnlyCollection<FriendlyResultMessage> messages = null;

            // Act
            Action act = () => result.WithMessages(messages);

            // Assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("messages");
        }

        [Fact]
        public void WithValidationMessages_Adds_ValidationFailures_To_Collection()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var validationFailures = _validationFailureFaker.Generate(3).ToList().AsReadOnly();

            // Act
            result.WithValidationMessages(validationFailures);

            // Assert
            result.ValidationMessages.Should().BeEquivalentTo(validationFailures);
        }

        [Fact]
        public void WithValidationMessages_Throws_ArgumentNullException_When_Failures_Is_Null()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            IReadOnlyCollection<ValidationFailure> failures = null;

            // Act
            Action act = () => result.WithValidationMessages(failures);

            // Assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("failures");
        }

        [Fact]
        public void WithRedirectUrl_Sets_RedirectToUrl_Property()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var url = new Faker().Internet.Url();

            // Act
            result.WithRedirectUrl(url);

            // Assert
            result.RedirectToUrl.Should().Be(url);
        }

        [Fact]
        public void WithJsHandler_Sets_JsHandler_Property()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var handler = new Faker().Lorem.Word();

            // Act
            result.WithJsHandler(handler);

            // Assert
            result.JsHandler.Should().Be(handler);
        }

        [Fact]
        public void WithRequestPath_Sets_RequestPath_Property()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var path = new Faker().System.FilePath();

            // Act
            result.WithRequestPath(path);

            // Assert
            result.RequestPath.Should().Be(path);
        }

        [Fact]
        public void WithTraceId_Sets_TraceId_Property()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var traceId = Guid.NewGuid().ToString();

            // Act
            result.WithTraceId(traceId);

            // Assert
            result.TraceId.Should().Be(traceId);
        }

        [Fact]
        public void WithStatus_Sets_ResponseStatus_Property()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage);
            var status = new Faker().Random.Int(100, 599);

            // Act
            result.WithStatus(status);

            // Assert
            result.ResponseStatus.Should().Be(status);
        }

        [Fact]
        public void ToString_Returns_Correct_Json_Representation()
        {
            // Arrange
            var failed = true;
            var friendlyMessage = FriendlyResultMessage.With(new Faker().Lorem.Sentence(50), 100);
            var result = new Result(failed, friendlyMessage);
            List<FriendlyResultMessage> messages = [];
            for (int i = 0; i < 3; i++)
            {
                messages.Add(FriendlyResultMessage.With(new Faker().Lorem.Sentence(30), 11));
            }

            var validationMessages = _validationFailureFaker.Generate(2).ToList().AsReadOnly();

            List<InternalResultMessage> internalMessages = [];
            for (int i = 0; i < 3; i++)
            {
                internalMessages.Add(InternalResultMessage.With(new Faker().Commerce.Department(10),
                    new Faker().Lorem.Sentence(30), new Faker().Lorem.Sentence(10)));
            }

            var exception = new InvalidOperationException("Test Exception");
            var redirectUrl = new Faker().Internet.Url();
            var jsHandler = new Faker().Lorem.Word();
            var requestPath = new Faker().System.FilePath();
            var traceId = Guid.NewGuid().ToString();
            var responseStatus = new Faker().Random.Int(100, 599);

            result.WithMessages(messages)
                .WithValidationMessages(validationMessages)
                .WithInternalMessages(internalMessages)
                .WithException(exception)
                .WithRedirectUrl(redirectUrl)
                .WithJsHandler(jsHandler)
                .WithRequestPath(requestPath)
                .WithTraceId(traceId)
                .WithStatus(responseStatus);

            // Act
            var jsonString = result.ToString();

            // Assert
            jsonString.Should().Contain($"\"Failed\": {failed.ToString().ToLowerInvariant()}");
            jsonString.Should().Contain($"\"ResultType\": \"{result.ResultType}\"");
            jsonString.Should().Contain($"\"ResultId\": \"{result.ResultId}\"");
            jsonString.Should().Contain($"\"ResponseStatus\": {responseStatus}");
            jsonString.Should().Contain($"\"TraceId\":\"{traceId}\"");
            jsonString.Should().Contain($"\"RequestPath\":\"{requestPath}\"");
            jsonString.Should().Contain($"\"RedirectToUrl\":\"{redirectUrl}\"");
            jsonString.Should().Contain($"\"JsHandler\":\"{jsHandler}\"");

            foreach (var message in messages)
            {
                jsonString.Should().Contain(message.ToString());
            }

            foreach (var validationMessage in validationMessages)
            {
                jsonString.Should().Contain(validationMessage.ToString());
            }

            foreach (var internalMessage in internalMessages)
            {
                jsonString.Should().Contain(internalMessage.ToString());
            }

            jsonString.Should().Contain($"\"Exceptions\": {exception.AsJsonException()}");

            JsonConvert.DeserializeObject(jsonString).Should().NotBeNull();
        }

        [Fact]
        public void ToString_Returns_Valid_Json_When_Collections_Are_Empty()
        {
            // Arrange
            var result = new Result(false, Result.SucceedResultMessage)
                .WithTraceId(string.Empty)
                .WithRequestPath(string.Empty)
                .WithRedirectUrl(string.Empty)
                .WithJsHandler(string.Empty);

            // Act
            var jsonString = result.ToString();

            // Assert
            JsonConvert.DeserializeObject(jsonString).Should().NotBeNull();
        }
    }
}