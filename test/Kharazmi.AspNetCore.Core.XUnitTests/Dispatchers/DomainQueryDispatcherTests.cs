using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Dispatchers;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Handlers;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Dispatchers
{
    [Collection("DomainQueryDispatcherTests")]
    [ExcludeFromCodeCoverage]
    public class DomainQueryDispatcherTests
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DomainQueryDispatcher _dispatcher;

        public DomainQueryDispatcherTests()
        {
            _serviceProvider = Substitute.For<IServiceProvider>();
            _dispatcher = new DomainQueryDispatcher(_serviceProvider);
        }

        [Fact]
        public async Task QueryAsync_ValidQuery_ReturnsSuccessResult()
        {
            // Arrange
            var query = new TestQuery();
            var queryResult = new TestResult();

            var handler = Substitute.For<IDomainQueryHandler<TestQuery, TestResult>>();
            handler.HandleAsync(query, CancellationToken.None).Returns(Result.Ok(queryResult));

            _serviceProvider.GetService(typeof(IDomainQueryHandler<TestQuery, TestResult>)).Returns(handler);
            _serviceProvider.GetService<IDomainQueryHandler<TestQuery, TestResult>>().Returns(handler);

            // Act
            var result = await _dispatcher.QueryAsync<TestQuery, TestResult>(query);

            // Assert
            result.Failed.Should().BeFalse();
            result.Value.Should().Be(queryResult);
        }

        [Fact]
        public async Task QueryAsync_HandlerThrowsException_ReturnsFailResultWithException()
        {
            // Arrange
            var query = new TestQuery();
            var exception = new InvalidOperationException("Something went wrong");

            var handler = Substitute.For<IDomainQueryHandler<TestQuery, TestResult>>();
            handler.HandleAsync(query, CancellationToken.None)
                .Returns(Task.FromException<Result<TestResult>>(exception));

            _serviceProvider.GetService(typeof(IDomainQueryHandler<TestQuery, TestResult>)).Returns(handler);
            _serviceProvider.GetService<IDomainQueryHandler<TestQuery, TestResult>>().Returns(handler);

            // Act
            var result = await _dispatcher.QueryAsync<TestQuery, TestResult>(query);

            // Assert
            result.Failed.Should().BeTrue();
            result.FriendlyMessage.Description.Should().Be($"Unable to handle the query {query.QueryType}");
            result.Exception.Should().Be(exception);
        }

        [Fact]
        public async Task QueryAsync_HandlerReturnsFailResult_ReturnsFailResult()
        {
            // Arrange
            var query = new TestQuery();

            var handler = Substitute.For<IDomainQueryHandler<TestQuery, string>>();
            handler.HandleAsync(query, CancellationToken.None).Returns(Task.FromResult(Result.Fail<string>("Failed")));

            _serviceProvider.GetService(typeof(IDomainQueryHandler<TestQuery, string>)).Returns(handler);
            _serviceProvider.GetService<IDomainQueryHandler<TestQuery, string>>().Returns(handler);

            // Act
            var result = await _dispatcher.QueryAsync<TestQuery, string>(query);

            // Assert
            result.Failed.Should().BeTrue();
            result.FriendlyMessage.Description.Should().BeEquivalentTo("Failed");
        }

        [Fact]
        public async Task QueryAsync_GetServiceReturnsNull_ThrowsServiceNotFoundException()
        {
            // Arrange
            IServiceProvider serviceProvider = null;

            // Act
            Action act = () => new DomainQueryDispatcher(serviceProvider );

            // Assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("serviceProvider");

            // Assert
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be("serviceProvider");
        }

        [Fact]
        public async Task QueryAsync_QueryIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var queryType = DomainQueryType.From<TestQuery>();
            
            // Act
           var result = await _dispatcher.QueryAsync<TestQuery, string>(null);

            // Assert
            result.FriendlyMessage.Description.Should().Be($"Unable to handle the query {queryType}");
            result.Exception.Message.Should().Be("Value cannot be null. (Parameter 'domainQuery')");
        }

        [Fact]
        public void QueryStreamAsync_ValidQuery_ReturnsAsyncEnumerable()
        {
            // Arrange
            var query = new TestQuery();

            var handler = Substitute.For<IStreamDomainQueryHandler<TestQuery, string>>();
            var expectedStream = Enumerable.Range(1, 3).Select(i => i.ToString()).ToList().ToAsyncEnumerable();

            handler.HandleStreamAsync(query, CancellationToken.None).Returns(expectedStream);
            _serviceProvider.GetService<IStreamDomainQueryHandler<TestQuery, string>>().Returns(handler);
            _serviceProvider.GetService(typeof(IStreamDomainQueryHandler<TestQuery, string>)).Returns(handler);

            // Act
            var resultStream = _dispatcher.QueryStreamAsync<TestQuery, string>(query);

            // Assert
            resultStream.Should().BeEquivalentTo(expectedStream);
        }

        [Fact]
        public void QueryStreamAsync_GetServiceReturnsNull_ThrowsServiceNotFoundException()
        {
            // Arrange
            var query = new TestQuery();

            _serviceProvider.GetService(typeof(IStreamDomainQueryHandler<TestQuery, string>)).Returns(null);
            _serviceProvider.GetService<IStreamDomainQueryHandler<TestQuery, string>>().Returns((IStreamDomainQueryHandler<TestQuery, string>?)null);

            // Act
            Action act = () => _dispatcher.QueryStreamAsync<TestQuery, string>(query);

            // Assert
            Assert.Throws<InvalidOperationException>(act);
        }

        [Fact]
        public void QueryStreamAsync_QueryIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var query = new TestQuery();
            // Act
            Action act = () => _dispatcher.QueryStreamAsync<TestQuery, string>(null);

            // Assert
            Assert.Throws<ArgumentNullException>(act);
        }
    }
}