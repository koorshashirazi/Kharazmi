using Kharazmi.AspNetCore.Core.ValueObjects;
using Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.SpeechAggregate;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates
{
    [CollectionDefinition(nameof(SpeechEntityUnitTest), DisableParallelization = true)]
    public class SpeechEntityUnitTestCollection { }

    [Collection(nameof(SpeechEntityUnitTest))]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class SpeechEntityUnitTest
    {
        [Fact]
        public void EqualityIsTrueWhenObjectsAreSameIdentitiesTest()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var id = Id.New<string>();
            //Act
            var speech1 = new Speech(id, title, url, description, SpeechType.Conferences);
            var speech2 = new Speech(id, title, url, description, SpeechType.SelfPacedLabs);

            //Assert
            Assert.Equal(speech1, speech2);
            Assert.True(speech1.Equals(speech2));
            Assert.True(speech1.Equals((object)speech2));
            Assert.Equal(speech1.GetHashCode(), speech2.GetHashCode());
        }

        [Fact]
        public void EqualityIsFalseWhenObjectsAreDifferentIdentitiesTest()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            //Act
            var speech1 = new Speech(Id.New<string>(), title, url, description, SpeechType.Conferences);
            var speech2 = new Speech(Id.New<string>(), title, url, description, SpeechType.Conferences);

            Assert.False(speech1 == speech2);
        }
    }
}