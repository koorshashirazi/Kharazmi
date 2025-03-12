using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest
{
    [CollectionDefinition(nameof(UrlValueObjectUnitTest), DisableParallelization = true)]
    public class UrlValueObjectUnitTestCollection
    {
    }

    [Collection(nameof(UrlValueObjectUnitTest))]
    public class UrlValueObjectUnitTest
    {
        [Fact]
        public void EqualityIsTrueWhenObjectsAreSameValuesTest()
        {
            //Arrange
            var url1 = new UrlValue("http://url.com");
            var url2 = new UrlValue("http://url.com");

            Assert.Equal(url1, url2);
            Assert.True(url1.Equals(url2));
            Assert.True(url1.Equals((object)url2));
            Assert.Equal(url1.GetHashCode(), url2.GetHashCode());
        }

        [Fact]
        public void EqualityIsFalseWhenObjectsAreDifferentValuesTest()
        {
            //Arrange
            var url1 = new UrlValue("http://url1.com");
            var url2 = new UrlValue("http://url2.com");

            Assert.False(url1 == url2);
        }
    }
}