using System.Diagnostics.CodeAnalysis;
using Bogus;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Validation;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Pipelines
{
    public class LoggerDomainEventPipelineLoggerWrapper : ILogger<LoggerDomainEventPipeline<TestDomainEvent>>
    {
        private readonly ILogger<LoggerDomainEventPipeline<TestDomainEvent>> _logger;

        public LoggerDomainEventPipelineLoggerWrapper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LoggerDomainEventPipeline<TestDomainEvent>>();
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

    public class LoggerDomainEventPipelineTestProxy : DomainEventHandler<TestDomainEvent>, IPipelineHandler
    {
        private readonly LoggerDomainEventPipeline<TestDomainEvent> _pipeline;
        internal LoggerDomainEventPipelineLoggerWrapper Logger { get; }

        public LoggerDomainEventPipelineTestProxy()
        {
            Handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            Logger = Substitute.For<LoggerDomainEventPipelineLoggerWrapper>(NullLoggerFactory.Instance);
            _pipeline = new LoggerDomainEventPipeline<TestDomainEvent>(Handler, Logger);
        }

        public IDomainEventHandler<TestDomainEvent> Handler { get; }

        public override Task<Result> HandleAsync(TestDomainEvent domainEvent, CancellationToken token = default)
        {
            return _pipeline.HandleAsync(domainEvent, token);
        }
    }

    [Collection("LoggerDomainEventPipelineTests")]
    [ExcludeFromCodeCoverage]
    public class LoggerDomainEventPipelineTests
    {
        private readonly IDomainEventHandler<TestDomainEvent> _handler;
        private readonly ILogger<LoggerDomainEventPipeline<TestDomainEvent>> _logger;
        private readonly LoggerDomainEventPipelineTestProxy _pipeline;

        public LoggerDomainEventPipelineTests()
        {
            _pipeline = new LoggerDomainEventPipelineTestProxy();
            _handler = _pipeline.Handler;
            _logger = _pipeline.Logger;
        }

        [Fact]
        public async Task HandleAsync_NullDomainEvent_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _pipeline.HandleAsync(null, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'domainEvent')");
        }

        [Fact]
        public async Task HandleAsync_ValidDomainEvent_CallsHandlerAndLogsCorrectly()
        {
            // Arrange
            var domainEvent = new Faker<TestDomainEvent>().Generate();
            var expectedResult = Result.Ok();
            _handler.HandleAsync(domainEvent, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _pipeline.HandleAsync(domainEvent, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);

            //Verify logger was called. Due to the complexity of verifying the exact log messages, we're verifying at least that it was called at Debug level
            _logger.Received().Log(
                LogLevel.Debug,
                new EventId(),
                string.Empty,
                null, (_,_) => string.Empty);
        }

        [Fact]
        public async Task HandleAsync_HandlerReturnsFailedResult_LogsErrorsAndFailures()
        {
            // Arrange
            var domainEvent = new Faker<TestDomainEvent>().Generate();
            var expectedResult = Result.Fail( "ErrorDescription", 10)
                .WithValidationMessages(ValidationFailure.For("PropertyName", "ErrorMessage"));

            _handler.HandleAsync(domainEvent, CancellationToken.None).Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _pipeline.HandleAsync(domainEvent, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);

            //Verify logger was called. Due to the complexity of verifying the exact log messages, we're verifying at least that it was called at Debug level
            _logger.Received().Log(
                LogLevel.Debug,
                new EventId(),
                string.Empty,
                null, (_,_) => string.Empty);
        }

        [Fact]
        public async Task HandleAsync_HandlerReturnsSuccessfulResult_LogsSuccess()
        {
            // Arrange
            var domainEvent = new Faker<TestDomainEvent>().Generate();
            var expectedResult = Result.Ok();
            _handler.HandleAsync(domainEvent, CancellationToken.None).Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _pipeline.HandleAsync(domainEvent, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);
            //Verify logger was called. Due to the complexity of verifying the exact log messages, we're verifying at least that it was called at Debug level
            _logger.Received().Log(
                LogLevel.Debug,
                new EventId(),
                string.Empty,
                null, (_,_) => string.Empty);
        }
    }

    public class TestDomainEvent : IDomainEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

        public DomainEventId EventId => DomainEventId.New();

        public EventMetadata EventMetadata => EventMetadata.Empty;

        public DomainEventType EventType => DomainEventType.From<TestDomainEvent>();
    }
}