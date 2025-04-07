using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Handlers;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Dispatchers
{
    [Collection(nameof(DomainCommandHandlerTypeFactoryTests))]
    [ExcludeFromCodeCoverage]
    public class DomainCommandHandlerTypeFactoryTests
    {
        private readonly DomainCommandHandlerTypeFactory _sut;

        public DomainCommandHandlerTypeFactoryTests()
        {
            _sut = new DomainCommandHandlerTypeFactory();
        }

        [Fact]
        public void GetHandlerType_Should_Return_Correct_HandlerType()
        {
            // Arrange
            Type commandType = typeof(TestCommand);

            // Act
            Type handlerType = _sut.GetHandlerType(commandType);

            // Assert
            handlerType.Should().Be(typeof(IDomainCommandHandler<TestCommand>));
        }

        [Fact]
        public void GetHandlerType_Should_Return_Same_HandlerType_For_Same_CommandType_From_Cache()
        {
            // Arrange
            Type commandType = typeof(TestCommand);

            // Act
            Type handlerType1 = _sut.GetHandlerType(commandType);
            Type handlerType2 = _sut.GetHandlerType(commandType);

            // Assert
            handlerType1.Should().Be(handlerType2);
        }

        [Fact]
        public void GetHandlerType_Should_Return_Different_HandlerType_For_Different_CommandType()
        {
            // Arrange
            Type commandType1 = typeof(TestCommand);
            Type commandType2 = typeof(AnotherTestCommand);

            // Act
            Type handlerType1 = _sut.GetHandlerType(commandType1);
            Type handlerType2 = _sut.GetHandlerType(commandType2);

            // Assert
            handlerType1.Should().NotBe(handlerType2);
        }
    }
}