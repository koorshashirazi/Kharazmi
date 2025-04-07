using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Functional;
using NSubstitute;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Dispatchers
{
    [Collection(nameof(DomainDispatcherTests))]
    [ExcludeFromCodeCoverage]
    public class DomainDispatcherTests
    {
        private readonly IDomainCommandDispatcher _domainCommandDispatcher;
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly IDomainQueryDispatcher _domainQueryDispatcher;
        private readonly DomainDispatcher _domainDispatcher;

        public DomainDispatcherTests()
        {
            _domainCommandDispatcher = Substitute.For<IDomainCommandDispatcher>();
            _domainEventDispatcher = Substitute.For<IDomainEventDispatcher>();
            _domainQueryDispatcher = Substitute.For<IDomainQueryDispatcher>();
            _domainDispatcher = new DomainDispatcher(_domainCommandDispatcher, _domainEventDispatcher, _domainQueryDispatcher);
        }

        [Fact]
        public async Task QueryAsync_ValidQuery_ReturnsResultFromQueryDispatcher()
        {
            // Arrange
            var query = Substitute.For<IDomainQuery>();
            var expectedResult = Result.Ok<string>("Query Result");
            _domainQueryDispatcher.QueryAsync<IDomainQuery, string>(query, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _domainDispatcher.QueryAsync<IDomainQuery, string>(query);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainQueryDispatcher.Received(1).QueryAsync<IDomainQuery, string>(query, CancellationToken.None);
        }

        [Fact]
        public async Task QueryAsync_NullQuery_DoesNotThrowException()
        {
            // Arrange
            IDomainQuery query = null;
            var expectedResult = Result.Ok<string>("Query Result");
            _domainQueryDispatcher.QueryAsync<IDomainQuery, string>(null, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _domainDispatcher.QueryAsync<IDomainQuery, string>(query);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainQueryDispatcher.Received(1).QueryAsync<IDomainQuery, string>(null, CancellationToken.None);
        }

        [Fact]
        public async Task QueryStreamAsync_ValidQuery_ReturnsStreamFromQueryDispatcher()
        {
            // Arrange
            var query = Substitute.For<IDomainQuery>();
            var expectedStream = new List<string> { "Stream Item 1", "Stream Item 2" }.ToAsyncEnumerable();
            _domainQueryDispatcher.QueryStreamAsync<IDomainQuery, string>(query, CancellationToken.None).Returns(expectedStream);

            // Act
            var resultStream = _domainDispatcher.QueryStreamAsync<IDomainQuery, string>(query);

            // Assert
            resultStream.Should().BeEquivalentTo(expectedStream);
            _domainQueryDispatcher.Received(1).QueryStreamAsync<IDomainQuery, string>(query, CancellationToken.None);
        }

        [Fact]
        public async Task QueryStreamAsync_NullQuery_DoesNotThrowException()
        {
            // Arrange
            IDomainQuery query = null;
            var expectedStream = new List<string> { "Stream Item 1", "Stream Item 2" }.ToAsyncEnumerable();
            _domainQueryDispatcher.QueryStreamAsync<IDomainQuery, string>(null, CancellationToken.None).Returns(expectedStream);

            // Act
            var resultStream = _domainDispatcher.QueryStreamAsync<IDomainQuery, string>(query);

            // Assert
            resultStream.Should().BeEquivalentTo(expectedStream);
            _domainQueryDispatcher.Received(1).QueryStreamAsync<IDomainQuery, string>(null, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_DomainEvent_ReturnsResultFromEventDispatcher()
        {
            // Arrange
            var domainEvent = Substitute.For<IDomainEvent>();
            var expectedResult = Result.Ok();
            _domainEventDispatcher.RaiseAsync(domainEvent, CancellationToken.None).Returns(Task.FromResult(expectedResult));

            // Act
            var result = await _domainDispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainEventDispatcher.Received(1).RaiseAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_GenericDomainEvent_ReturnsResultFromEventDispatcher()
        {
            // Arrange
            var domainEvent = Substitute.For<IDomainEvent>();
            var expectedResult = Result.Ok();
            _domainEventDispatcher.RaiseAsync(domainEvent, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _domainDispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainEventDispatcher.Received(1).RaiseAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_NullDomainEvent_DoesNotThrowException()
        {
            // Arrange
            IDomainEvent domainEvent = null;
            var expectedResult = Result.Ok();
            _domainEventDispatcher.RaiseAsync(domainEvent, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _domainDispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainEventDispatcher.Received(1).RaiseAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task SendAsync_DomainCommand_ReturnsResultFromCommandDispatcher()
        {
            // Arrange
            var command = Substitute.For<IDomainCommand>();
            var expectedResult = Result.Ok();
            _domainCommandDispatcher.SendAsync(command, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _domainDispatcher.SendAsync(command);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainCommandDispatcher.Received(1).SendAsync(command, CancellationToken.None);
        }

        [Fact]
        public async Task SendAsync_GenericDomainCommand_ReturnsResultFromCommandDispatcher()
        {
            // Arrange
            var command = Substitute.For<IDomainCommand>();
            var expectedResult = Result.Ok();
            _domainCommandDispatcher.SendAsync(command, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _domainDispatcher.SendAsync(command);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainCommandDispatcher.Received(1).SendAsync(command, CancellationToken.None);
        }

        [Fact]
        public async Task SendAsync_NullDomainCommand_DoesNotThrowException()
        {
            // Arrange
            IDomainCommand command = null;
            var expectedResult = Result.Ok();
            _domainCommandDispatcher.SendAsync(command, CancellationToken.None).Returns(expectedResult);

            // Act
            var result = await _domainDispatcher.SendAsync(command);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            await _domainCommandDispatcher.Received(1).SendAsync(command, CancellationToken.None);
        }
    }
}