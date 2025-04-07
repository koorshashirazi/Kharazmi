using System.Diagnostics.CodeAnalysis;
using Bogus;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.Pipelines;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Microsoft.Extensions.Logging.Abstractions;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Pipelines
{
    public class LoggerDomainQueryPipelineLoggerWrapper : ILogger<LoggerDomainQueryPipeline<TestQuery, TestResult>>
    {
        private readonly ILogger<LoggerDomainQueryPipeline<TestQuery, TestResult>> _logger;

        public LoggerDomainQueryPipelineLoggerWrapper(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LoggerDomainQueryPipeline<TestQuery, TestResult>>();
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

    public class LoggerDomainQueryPipelineTestProxy : IDomainQueryHandler<TestQuery, TestResult>, IPipelineHandler
    {
        private readonly LoggerDomainQueryPipeline<TestQuery, TestResult> _pipeline;
        internal LoggerDomainQueryPipelineLoggerWrapper Logger { get; }

        public LoggerDomainQueryPipelineTestProxy()
        {
            Handler = Substitute.For<IDomainQueryHandler<TestQuery, TestResult>>();
            Logger = Substitute.For<LoggerDomainQueryPipelineLoggerWrapper>(NullLoggerFactory.Instance);
            _pipeline = new LoggerDomainQueryPipeline<TestQuery, TestResult>(Handler, Logger);
        }

        public IDomainQueryHandler<TestQuery, TestResult> Handler { get; }


        public Task<Result<TestResult>> HandleAsync(TestQuery query, CancellationToken token = default)
        {
            return _pipeline.HandleAsync(query, token);
        }
    }

    [Collection("LoggerDomainQueryPipelineTests")]
    [ExcludeFromCodeCoverage]
    public class LoggerDomainQueryPipelineTests
    {
        private readonly IDomainQueryHandler<TestQuery, TestResult> _handler;
        private readonly ILogger<LoggerDomainQueryPipeline<TestQuery, TestResult>> _logger;
        private readonly LoggerDomainQueryPipelineTestProxy _pipeline;

        public LoggerDomainQueryPipelineTests()
        {
            _pipeline = new LoggerDomainQueryPipelineTestProxy();
            _handler = _pipeline.Handler;
            _logger = _pipeline.Logger;
        }

        [Fact]
        public async Task HandleAsync_ValidQuery_ReturnsResultFromHandler()
        {
            // Arrange
            var query = new Faker<TestQuery>().Generate();
            var expectedResult = Result.Ok(new Faker<TestResult>().Generate());
            _handler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expectedResult);

            // Act
            var result = await _pipeline.HandleAsync(query);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _handler.Received(1).HandleAsync(query, Arg.Any<CancellationToken>());
        }


        [Fact]
        public async Task HandleAsync_NullQueryDomain_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _pipeline.HandleAsync(null, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>()
                .WithMessage("Value cannot be null. (Parameter 'query')");
        }

        [Fact]
        public async Task HandleAsync_FailedResult_LogsErrorMessages()
        {
            // Arrange
            var query = new Faker<TestQuery>().Generate();
            var expectedResult = Result.Fail<TestResult>("Error Description", 10);
            expectedResult.WithValidationMessages(Validation.ValidationFailure.For("PropertyName", "Error Message"));
            _handler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expectedResult);

            // Act
            var result = await _pipeline.HandleAsync(query);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _handler.Received(1).HandleAsync(query, Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task HandleAsync_SuccessfulResult_ReturnsSuccessfulResult()
        {
            // Arrange
            var query = new Faker<TestQuery>().Generate();
            var expectedResult = Result.Ok(new Faker<TestResult>().Generate());
            _handler.HandleAsync(query, Arg.Any<CancellationToken>()).Returns(expectedResult);

            // Act
            var result = await _pipeline.HandleAsync(query);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _handler.Received(1).HandleAsync(query, Arg.Any<CancellationToken>());
        }
    }

    public record TestQuery : IDomainQuery
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }

        public DomainQueryType QueryType => DomainQueryType.From<TestQuery>();

        public DomainQueryId QueryId => DomainQueryId.New();
    }

    public record TestResult
    {
        public string Value { get; set; } = string.Empty;
    }
}