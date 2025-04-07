using System.Diagnostics.CodeAnalysis;
using Bogus;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;
using Kharazmi.AspNetCore.Core.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Pipelines
{
    public class LoggerDomainCommandPipelineLoggerWrapper : ILogger<LoggerDomainCommandPipeline<TestCommand>>
    {
        private readonly ILogger<LoggerDomainCommandPipeline<TestCommand>> _logger;

        public LoggerDomainCommandPipelineLoggerWrapper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LoggerDomainCommandPipeline<TestCommand>>();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return _logger.BeginScope(state);
        }
    }

    public class LoggerDomainCommandPipelineTestProxy : DomainCommandHandler<TestCommand>, IPipelineHandler
    {
        private readonly LoggerDomainCommandPipeline<TestCommand> _pipeline;
        internal LoggerDomainCommandPipelineLoggerWrapper Logger { get; }

        public LoggerDomainCommandPipelineTestProxy()
        {
            Handler = Substitute.For<IDomainCommandHandler<TestCommand>>();
            Logger = Substitute.For<LoggerDomainCommandPipelineLoggerWrapper>(NullLoggerFactory.Instance);
            _pipeline = new LoggerDomainCommandPipeline<TestCommand>(Handler, Logger);
        }

        public IDomainCommandHandler<TestCommand> Handler { get; }

        public override Task<Result> HandleAsync(TestCommand command, CancellationToken token = default)
        {
            return _pipeline.HandleAsync(command, token);
        }
    }
    

    [Collection("LoggerDomainCommandPipelineTests")]
    [ExcludeFromCodeCoverage]
    public class LoggerDomainCommandPipelineTests
    {
        private readonly IDomainCommandHandler<TestCommand> _handler;
        private readonly ILogger<LoggerDomainCommandPipeline<TestCommand>> _logger;
        private readonly LoggerDomainCommandPipelineTestProxy _pipeline;

        public LoggerDomainCommandPipelineTests()
        {
            _pipeline = new LoggerDomainCommandPipelineTestProxy();
            _logger = _pipeline.Logger;
            _handler = _pipeline.Handler;
        }

        [Fact]
        public async Task HandleAsync_ValidCommand_CallsHandlerAndLogsCorrectly()
        {
            // Arrange
            var command = new Faker<TestCommand>().Generate();
            var expectedResult = Result.Ok();
            _handler.HandleAsync(command, CancellationToken.None).Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _pipeline.HandleAsync(command);

            // Assert
            await _handler.Received(1).HandleAsync(command, CancellationToken.None);
            result.Should().BeEquivalentTo(expectedResult);
            _logger.Received().LogDebug(string.Empty, []);
        }

        [Fact]
        public async Task HandleAsync_HandlerReturnsFailedResult_LogsErrorsAndFailures()
        {
            // Arrange
            var command = new Faker<TestCommand>().Generate();

            var expectedResult = Result.Fail("ErrorCode", 10)
                .WithValidationMessages(ValidationFailure.For("PropertyName", "ErrorMessage"));
            _handler.HandleAsync(command, CancellationToken.None).Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _pipeline.HandleAsync(command);

            // Assert
            await _handler.Received(1).HandleAsync(command, CancellationToken.None);
            result.Should().BeEquivalentTo(expectedResult);
            _logger.Received().LogDebug(string.Empty, []);
        }

        [Fact]
        public async Task HandleAsync_CommandIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            TestCommand command = null;

            // Act
            Func<Task> act = async () => await _pipeline.HandleAsync(command);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task HandleAsync_CancellationTokenIsCancelled_PropagatesCancellation()
        {
            // Arrange
            var command = new Faker<TestCommand>().Generate();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            var cancellationToken = cancellationTokenSource.Token;
            _handler.HandleAsync(command, cancellationToken).Returns(Task.FromCanceled<Result>(cancellationToken));

            // Act
            Func<Task> act = async () => await _pipeline.HandleAsync(command, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<System.OperationCanceledException>();
        }
    }

    public class TestCommand : DomainCommand
    {
        public override bool IsValid()
        {
            return true;
        }
    }
}