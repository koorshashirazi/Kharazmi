using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest
{
    [CollectionDefinition(nameof(TitleValueObjectUnitTest), DisableParallelization = true)]
    public class TitleValueObjectUnitTestCollection { }

    [Collection(nameof(TitleValueObjectUnitTest))]

    public class TitleValueObjectUnitTest
    {
        [Fact]
        public void EqualityIsTrueWhenObjectsAreSameValuesTest()
        {
            //Arrange
            var title1 = new Title("Lorem Ipsum is simply dummy text of the printin");
            var title2 = new Title("Lorem Ipsum is simply dummy text of the printin");

            Assert.Equal(title1, title2);
            Assert.True(title1.Equals(title2));
            Assert.True(title1.Equals((object)title2));
            Assert.Equal(title1.GetHashCode(), title2.GetHashCode());
        }

        [Fact]
        public void EqualityIsFalseWhenObjectsAreDifferentValuesTest()
        {
            //Arrange
            var title1 = new Title("Lorem Ipsum is simply dummy text of the printin");
            var title2 = new Title("defLorem Ipsum is simply dummy text of the printin");

            Assert.False(title1 == title2);
        }
    }
}