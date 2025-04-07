using FluentAssertions;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;
using System.Diagnostics.CodeAnalysis;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Dispatchers
{
    [Collection("DomainCommandDispatcherTests")]
    [ExcludeFromCodeCoverage]
    public class DomainCommandDispatcherTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDomainCommandHandlerTypeFactory _handlerTypeFactory;
        private readonly DomainCommandDispatcher _dispatcher;

        public DomainCommandDispatcherTests()
        {
            // Arrange
            _serviceProvider = Substitute.For<IServiceProvider>();
            _handlerTypeFactory = Substitute.For<IDomainCommandHandlerTypeFactory>();
            _dispatcher = new DomainCommandDispatcher(_serviceProvider, _handlerTypeFactory);
        }

        [Fact]
        public void Constructor_NullServiceProvider_ThrowsArgumentNullException()
        {
            // Arrange
            IServiceProvider serviceProvider = null;
            IDomainCommandHandlerTypeFactory handlerTypeFactory = Substitute.For<IDomainCommandHandlerTypeFactory>();

            // Act
            Action act = () => new DomainCommandDispatcher(serviceProvider, handlerTypeFactory);

            // Assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("serviceProvider");
        }

        [Fact]
        public void Constructor_NullHandlerTypeFactory_ThrowsArgumentNullException()
        {
            // Arrange
            IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
            IDomainCommandHandlerTypeFactory handlerTypeFactory = null;

            // Act
            Action act = () => new DomainCommandDispatcher(serviceProvider, handlerTypeFactory);

            // Assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("handlerTypeFactory");
        }

        [Fact]
        public async Task SendAsync_Generic_SuccessfulCommandExecution()
        {
            // Arrange
            var command = new TestCommand();
            
            var handler = Substitute.For<IDomainCommandHandler<TestCommand>>();
            handler.HandleAsync(command, CancellationToken.None).Returns(Task.FromResult(Result.Ok()));
            
            _serviceProvider.GetService(typeof(IDomainCommandHandler<TestCommand>)).Returns(handler);
            _serviceProvider.GetRequiredService<IDomainCommandHandler<TestCommand>>().Returns(handler);

            // Act
            var result = await _dispatcher.SendAsync(command);

            // Assert
            result.Failed.Should().BeFalse();
            await handler.Received(1).HandleAsync(command, CancellationToken.None);
        }

        [Fact]
        public async Task SendAsync_Generic_CommandHandlerThrowsException_ReturnsFailedResultWithException()
        {
            // Arrange
            var command = new TestCommand();
            var exception = new InvalidOperationException("Test exception");

            var handler = Substitute.For<IDomainCommandHandler<TestCommand>>();
            handler.HandleAsync(command, CancellationToken.None).Returns(Task.FromException<Result>(exception));
            
            _serviceProvider.GetService(typeof(IDomainCommandHandler<TestCommand>)).Returns(handler);
            _serviceProvider.GetRequiredService<IDomainCommandHandler<TestCommand>>().Returns(handler);

            // Act
            var result = await _dispatcher.SendAsync(command);

            // Assert
            result.Failed.Should().BeTrue();
            result.Exception.Should().Be(exception);
        }

        [Fact]
        public async Task SendAsync_NonGeneric_SuccessfulCommandExecution()
        {
            // Arrange
            var command = new TestCommand();
            var handlerType = typeof(TestCommandHandler);
            var handler = Substitute.For<IDomainCommandHandler<TestCommand>>();

            _handlerTypeFactory.GetHandlerType(command.CommandType.ToType()).Returns(handlerType);
            _serviceProvider.GetService(typeof(IDomainCommandHandler<TestCommand>)).Returns(handler);
            _serviceProvider.GetRequiredService<IDomainCommandHandler<TestCommand>>().Returns(handler);
            handler.HandleAsync(command, CancellationToken.None).Returns(Task.FromResult(Result.Ok()));

            // Act
            var result = await _dispatcher.SendAsync(command);

            // Assert
            result.Failed.Should().BeFalse();
            await handler.Received(1).HandleAsync(command, CancellationToken.None);
        }

        [Fact]
        public async Task SendAsync_NonGeneric_CommandHandlerThrowsException_ReturnsFailedResultWithException()
        {
            // Arrange
            var command = new TestCommand();
            var exception = new InvalidOperationException("Test exception");
            
            var handlerType = typeof(TestCommandHandler);
            var handler = Substitute.For<IDomainCommandHandler<TestCommand>>();
            handler.HandleAsync(command, CancellationToken.None).Returns(Task.FromException<Result>(exception));

            _handlerTypeFactory.GetHandlerType(command.CommandType.ToType()).Returns(handlerType);
            _serviceProvider.GetService(typeof(IDomainCommandHandler<TestCommand>)).Returns(handler);
            _serviceProvider.GetRequiredService<IDomainCommandHandler<TestCommand>>().Returns(handler);

            // Act
            var result = await _dispatcher.SendAsync(command);

            // Assert
            result.Failed.Should().BeTrue();
        }
    }
}