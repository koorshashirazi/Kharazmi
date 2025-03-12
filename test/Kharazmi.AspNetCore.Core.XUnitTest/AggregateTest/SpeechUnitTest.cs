using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Events;
using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Exceptions;
using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate;
using Moq;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest
{
    [CollectionDefinition(nameof(SpeechUnitTest), DisableParallelization = true)]
    public class SpeechUnitTestCollection
    {
    }

    [Collection(nameof(SpeechUnitTest))]
    public class SpeechUnitTest
    {
        [Fact]
        public void RegisterSpeechWithTitleLessThanNCaractersThrowsDomainException()
        {
            //Arrange
            var title = "abc";

            //Act
            //Assert
            Assert.Throws<InvalidLenghtAggregateException>(() => new Speech(
                Id.New<string>(),
                new Title(title),
                It.IsAny<UrlValue>(),
                It.IsAny<Description>(),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithTitleMoreThanNCaractersThrowsDomainException()
        {
            //Arrange
            var title =
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum";

            //Act
            //Assert
            Assert.Throws<InvalidLenghtAggregateException>(() => new Speech(
                Id.New<string>(),
                new Title(title),
                It.IsAny<UrlValue>(),
                It.IsAny<Description>(),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithTitleNullThrowsDomainException()
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Speech(
                Id.New<string>(),
                null, It.IsAny<UrlValue>(),
                It.IsAny<Description>(),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithDescriptionLessThanNCaractersThrowsDomainException()
        {
            //Arrange
            var description = "abc";

            //Act
            //Assert
            Assert.Throws<InvalidLenghtAggregateException>(() => new Speech(
                Id.New<string>(),
                It.IsAny<Title>(),
                It.IsAny<UrlValue>(),
                new Description(description),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithDescriptionMoreThanNCaractersThrowsDomainException()
        {
            //Arrange
            var description =
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum";

            //Act
            //Assert
            Assert.Throws<InvalidLenghtAggregateException>(() => new Speech(
                Id.New<string>(),
                It.IsAny<Title>(),
                It.IsAny<UrlValue>(),
                new Description(description),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithDescriptionNullThrowsDomainException()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Speech(
                Id.New<string>(),
                title, It.IsAny<UrlValue>(),
                null,
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithNullUrlThrowsDomainException()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Speech(
                Id.New<string>(),
                title, null,
                It.IsAny<Description>(),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithEmptyUrlThrowsDomainException()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Speech(
                Id.New<string>(),
                title, new UrlValue(string.Empty),
                It.IsAny<Description>(),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithInvalidUrlThrowsDomainException()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");

            //Act
            //Assert
            Assert.Throws<InvalidUrlAggregateException>(() => new Speech(
                Id.New<string>(),
                title, new UrlValue("thisIsnotAValiUrl"),
                It.IsAny<Description>(),
                It.IsAny<SpeechType>()));
        }

        [Fact]
        public void RegisterSpeechWithNullTypeThrowsDomainException()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var url = new UrlValue("http://url.com");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => new Speech(title, url, description, null));
        }

        [Fact]
        public void RegisterSpeechWithInvalidTypeThrowsDomainException()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");
            //Act
            //Assert
            Assert.Throws<InvalidEnumAggregateException>(() =>
                new Speech(title, url, description, new SpeechType("patati")));
        }

        [Theory]
        [ClassData(typeof(SpeechTypeTestData))]
        public void RegisterSpeechWithValidDataReturnsSuccess(SpeechType speechType)
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            //Act
            var speech = new Speech(title, url, description, speechType);

            //Assert
            Assert.Equal(title.Value, speech.Title.Value);
            Assert.Equal(description.Value, speech.Description.Value);
            Assert.Equal(url.Value, speech.Url.Value);
            Assert.Equal(speechType.Value, speech.Type.Value);
        }

        [Theory]
        [ClassData(typeof(SpeechTypeTestData))]
        public void RegisterSpeechWithValidDataRaiseDomainEvent(SpeechType speechType)
        {
            //Arrange
            var id = Guid.NewGuid().ToString("N");
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            //Act
            var speech = new Speech(id, title, url, description, speechType);
            var domainEvent = speech.GetUncommittedEvents().SingleOrDefault();
            var speechCreateEvent = (SpeechCreatedEvent)domainEvent;

            System.Globalization.CultureInfo.CurrentCulture.ClearCachedData();

            //Assert
            Assert.IsAssignableFrom<SpeechCreatedEvent>(domainEvent);
            Assert.NotNull(speechCreateEvent);
            Assert.Equal(id, speechCreateEvent.EventMetadata.AggregateId);
            Assert.Equal(url.Value, speechCreateEvent.Url);
            Assert.Equal(title.Value, speechCreateEvent.Title);
            Assert.Equal(description.Value, speechCreateEvent.Description);
            Assert.Equal(speechType, new SpeechType(speechCreateEvent.Type));
            DateTimeOffset.UtcNow.Should().BeOnOrAfter(speechCreateEvent.EventMetadata.Timestamp);
        }

        [Theory]
        [ClassData(typeof(SpeechTypeTestData))]
        public void CreateMediaWithNullMediaFile(SpeechType speechType)
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            //Act
            var speech = new Speech(title, url, description, speechType);
            //Assert
            Assert.Throws<ArgumentNullException>(() => speech.CreateMedia(null));
        }

        [Fact]
        public void CreateMediaWithValidMediaFileRetunrSuccess()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");
            var file = new MediaFile(Guid.NewGuid(), new UrlValue(
                "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RE2ybMU?ver=c5fc&q=90&m=6&h=201&w=358&b=%23FFFFFFFF&l=f&o=t&aim=true"));

            var id = Id.New<string>();
            var speech = new Speech(id, title, url, description, SpeechType.Conferences);

            //Act
            speech.CreateMedia(file);
            var domainEvent = speech.GetUncommittedEvents().Single(x => x is MediaFileCreatedEvent);
            var mediaFileCreatedEvent = (MediaFileCreatedEvent)domainEvent;

            //Assert
            Assert.Contains(file, speech.MediaFileItems);

            Assert.NotNull(speech.MediaFileItems.Select(f => f.File));
            Assert.NotNull(mediaFileCreatedEvent);
            Assert.Equal(mediaFileCreatedEvent.EventMetadata.AggregateId, speech.Id);
            Assert.Equal(mediaFileCreatedEvent.EventMetadata.AggregateVersion, speech.Version);
        }

        [Fact]
        public void CreateMediaWithValidVersionReturnSuccess()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");
            var file = new MediaFile(Guid.NewGuid(), new UrlValue(
                "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RE2ybMU?ver=c5fc&q=90&m=6&h=201&w=358&b=%23FFFFFFFF&l=f&o=t&aim=true"));

            //Act
            var speech = new Speech(title, url, description, SpeechType.Conferences);
            speech.CreateMedia(file);
            var domainEvent = speech.GetUncommittedEvents().SingleOrDefault(s => s is MediaFileCreatedEvent);
            var mediaFileCreatedEvent = (MediaFileCreatedEvent)domainEvent;

            //Assert
            Assert.Contains(file, speech.MediaFileItems);

            Assert.NotNull(speech.MediaFileItems.Select(f => f.File));
            Assert.NotNull(domainEvent);
            Assert.True(speech.Version == 2);
            Assert.NotNull(mediaFileCreatedEvent);
            Assert.NotNull(mediaFileCreatedEvent.File);
            Assert.NotNull(mediaFileCreatedEvent.File);
        }


        [Fact]
        public void CreateMediaWithExistingMedaiShouldNotApplyEvent()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description =
                new Description(
                    "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");
            var file = new MediaFile(Guid.NewGuid(), new UrlValue(
                "https://img-prod-cms-rt-microsoft-com.akamaized.net/cms/api/am/imageFileData/RE2ybMU?ver=c5fc&q=90&m=6&h=201&w=358&b=%23FFFFFFFF&l=f&o=t&aim=true"));

            var id = Id.New<string>();
            var speech = new Speech(id, title, url, description, SpeechType.Conferences);


            //Act
            speech.CreateMedia(file);

            Assert.Throws<MediaFileAlreadyExisteDomainException>(() => speech.CreateMedia(file));
            var domainEvent = speech.GetUncommittedEvents().SingleOrDefault(s => s is MediaFileCreatedEvent);
            var mediaFileCreatedEvent = (MediaFileCreatedEvent)domainEvent;

            //Assert
            Assert.Contains(file, speech.MediaFileItems);

            Assert.NotNull(speech.MediaFileItems.Select(f => f.File));
            Assert.NotNull(domainEvent);
            Assert.True(mediaFileCreatedEvent.EventMetadata.AggregateVersion == 2);
            Assert.Equal(mediaFileCreatedEvent.EventMetadata.AggregateId, id);
        }

        [Fact]
        public void InstanciatingMediaFileWithPrivateConstrcutorShouldReturnNotNullInstance()
        {
            var instance = (MediaFile)typeof(MediaFile)
                .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    new Type[0],
                    new ParameterModifier[0])
                ?.Invoke(new object[0]);
            Assert.NotNull(instance);
            Assert.IsType<MediaFile>(instance);
        }

        #region changeTitle

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeTitleWhenTitleIsNullOrEmptyShouldRaiseArgumentNullException(string newTitle)
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => speech.ChangeTitle(new Title(newTitle)));
        }

        [Fact]
        public void ChangeTitleWithValidArgumentsShouldApplySpeechTitleChangedEvent()
        {
            //Arrange
            string newTitle = "new Lorem Ipsum is simply dummy text of the printin";
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            speech.ChangeTitle(new Title(newTitle));

            //Assert
            Assert.Equal(newTitle, speech.Title.Value);
            Assert.Equal(description, speech.Description);
            Assert.Equal(url, speech.Url);
        }

        #endregion changeTitle

        #region changeDescription

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeDescriptionWhenDescriptionIsNullOrEmptyShouldRaiseArgumentNullAggregateException(
            string newDescription)
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => speech.ChangeDescription(new Description(newDescription)));
        }

        [Fact]
        public void ChangeDescriptionWhenExpectedVersionIsNotEqualsToAggregateVersionShouldRaiseConcurrencyException()
        {
            //Arrange
            const ulong currentVersion = 1;
            string newDescription =
                @" newLorem Ipsum is simply dummy text of the printing and typesetting industry.                                         Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took";
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");
            var aggregate = new Speech(
                Id.New<string>(), currentVersion, title, url, description, SpeechType.Conferences);
            
            //Act
            aggregate.ChangeDescription(new Description(newDescription));
            typeof(Speech).GetProperty("Version")?.SetValue(aggregate, 10ul);
            
            //Assert
            aggregate.Version.Should().Be(10ul);
            aggregate.UncommittedEvents.Count.Should().Be(2);

            Action validateVersion = () => aggregate.ValidateVersion();
            validateVersion.Should()
                .Throw<ConcurrencyException>()
                .WithMessage("Invalid version specified : expectedVersion = 3 but  originalVersion = 10.");
        }

        [Fact]
        public void ChangeDescriptionWithValidArgumentsShouldApplySpeechDescriptionChangedEvent()
        {
            //Arrange
            string newDescription =
                "Nex desc Lorem Ipsum is simply dummy text of the printing and typesetting industry.Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took";
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            speech.ChangeDescription(new Description(newDescription));

            //Assert
            Assert.Equal(title, speech.Title);
            Assert.Equal(newDescription, speech.Description.Value);
            Assert.Equal(url, speech.Url);
        }

        [Fact]
        public void
            ApplySpeechSpeechDescriptionChangedEventWithInvalidAggregateIdShouldRaiseInvalidDomainEventException()
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);
            var domainEvent = new SpeechDescriptionChangedEvent(description.Value);

            //Act
            domainEvent.EventMetadata.SetAggregateId(Id.New<string>());

            //Assert
            Assert.Throws<InvalidDomainEventException>(() => speech.Apply(domainEvent));
        }

        #endregion changeDescription

        #region changeType

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeTypeWhenTitleIsNullOrEmptyShouldRaiseArgumentNullAggregateException(string newType)
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            //Assert
            Assert.Throws<InvalidEnumAggregateException>(() => speech.ChangeType(new SpeechType(newType)));
        }


        [Fact]
        public void ChangeTypeWithValidArgumentsShouldApplySpeechTypeChangedEvent()
        {
            //Arrange
            string newType = "Conferences";
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            speech.ChangeType(new SpeechType(newType));

            //Assert
            Assert.Equal(newType, speech.Type.StringValue);
            Assert.Equal(description, speech.Description);
            Assert.Equal(url, speech.Url);
        }

        #endregion changeType

        #region changeUrl

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ChangeUrlWhenUrlIsNullOrEmptyShouldRaiseArgumentNullAggregateException(string newUrl)
        {
            //Arrange
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url_new.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(() => speech.ChangeUrl(new UrlValue(newUrl)));
        }


        [Fact]
        public void ChangeUrlWithValidArgumentsShouldApplySpeechUrlChangedEvent()
        {
            //Arrange
            string newUrl = "http://url_new.com";
            var title = new Title("Lorem Ipsum is simply dummy text of the printin");
            var description = new Description(
                @"Lorem Ipsum is simply dummy text of the printing and typesetting industry.
                                              Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took ");
            var url = new UrlValue("http://url.com");

            var speech = new Speech(title, url, description, SpeechType.Conferences);

            //Act
            speech.ChangeUrl(new UrlValue(newUrl));


            //Assert
            Assert.Equal(title, speech.Title);
            Assert.Equal(description, speech.Description);
            Assert.Equal(newUrl, speech.Url.Value);
        }

        #endregion changeUrl
    }
}