using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Kharazmi.AspNetCore.Core.XUnitTests.Pipelines;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Dispatchers
{
    [Collection("DomainEventDispatcherTests")]
    [ExcludeFromCodeCoverage]
    public class DomainEventDispatcherTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DomainEventDispatcher _dispatcher;

        public DomainEventDispatcherTests()
        {
            _serviceProvider = Substitute.For<IServiceProvider>();
            _dispatcher = new DomainEventDispatcher(_serviceProvider);
        }

        [Fact]
        public async Task RaiseAsync_Generic_NullDomainEvent_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var result = await _dispatcher.RaiseAsync<TestDomainEvent>(null);

            // Assert
            result.Exception.Message.Should().Be("Value cannot be null. (Parameter 'domainEvent')");
        }

        [Fact]
        public async Task RaiseAsync_Generic_NoHandlers_ReturnsOkResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();
            var handlers = Array.Empty<IDomainEventHandler<TestDomainEvent>>();

            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeFalse();
        }

        [Fact]
        public async Task RaiseAsync_Generic_SingleHandler_ReturnsOkResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();
            var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler.HandleAsync(domainEvent, CancellationToken.None).Returns(Result.Ok());

            var handlers = new[] { handler };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeFalse();
            await handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_Generic_MultipleHandlers_ReturnsOkResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();

            var handler1 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler1.HandleAsync(domainEvent, CancellationToken.None).Returns(Result.Ok());

            var handler2 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler2.HandleAsync(domainEvent, CancellationToken.None).Returns(Result.Ok());

            var handlers = new[] { handler1, handler2 };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeFalse();
            await handler1.Received(1).HandleAsync(domainEvent, CancellationToken.None);
            await handler2.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_Generic_HandlerFails_ReturnsFailedResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();

            var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler.HandleAsync(domainEvent, CancellationToken.None).Returns(Result.Fail("Handler failed"));

            var handlers = new[] { handler };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeTrue();
            result.FriendlyMessage.Description.Should().Contain("Handler failed");
            await handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_Generic_HandlerThrowsException_ReturnsFailedResultWithException()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();
            var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            var expectedException = new InvalidOperationException("Handler exception");

            handler.HandleAsync(domainEvent, CancellationToken.None)
                .Returns(Task.FromException<Result>(expectedException));

            var handlers = new[] { handler };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeTrue();
            result.FriendlyMessage.Description.Should().Contain($"Unable to raise {domainEvent.EventType}");
            result.Exception.Should().NotBeNull();
            result.Exception.Message.Should().Be("Handler exception");
            await handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_NoHandlersFound_ReturnsOkResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();

            var handlers = Array.Empty<IDomainEventHandler<TestDomainEvent>>();
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeFalse();
        }

        [Fact]
        public async Task RaiseAsync_SingleHandlerFound_ReturnsOkResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();
            var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler.HandleAsync(domainEvent, CancellationToken.None).Returns(Result.Ok());

            var handlers = new[] { handler };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeFalse();
            await handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_MultipleHandlersFound_ReturnsOkResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();

            var handler1 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler1.HandleAsync(domainEvent, CancellationToken.None).Returns(Result.Ok());

            var handler2 = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler2.HandleAsync(domainEvent, CancellationToken.None).Returns(Result.Ok());

            var handlers = new[] { handler1, handler2 };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeFalse();
            await handler1.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_HandlerFails_ReturnsFailedResult()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();

            var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            handler.HandleAsync(domainEvent, CancellationToken.None)
                .Returns(Task.FromResult(Result.Fail("Handler failed")));

            var handlers = new[] { handler };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeTrue();
            result.FriendlyMessage.Description.Should().Be("Handler failed");
            await handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }

        [Fact]
        public async Task RaiseAsync_HandlerThrowsException_ReturnsFailedResultWithException()
        {
            // Arrange
            var domainEvent = new TestDomainEvent();
            var handler = Substitute.For<IDomainEventHandler<TestDomainEvent>>();
            var expectedException = new InvalidOperationException("Handler exception");

            handler.HandleAsync(domainEvent, CancellationToken.None)
                .Returns(Task.FromException<Result>(expectedException));

            var handlers = new[] { handler };
            _serviceProvider.GetService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetRequiredService<IEnumerable<IDomainEventHandler<TestDomainEvent>>>().Returns(handlers);
            _serviceProvider.GetServices<IDomainEventHandler<TestDomainEvent>>().Returns(handlers);

            // Act
            var result = await _dispatcher.RaiseAsync(domainEvent);

            // Assert
            result.Failed.Should().BeTrue();
            result.FriendlyMessage.Description.Should().Contain($"Unable to raise {domainEvent.EventType}");
            result.Exception.Should().NotBeNull();
            await handler.Received(1).HandleAsync(domainEvent, CancellationToken.None);
        }
    }
}