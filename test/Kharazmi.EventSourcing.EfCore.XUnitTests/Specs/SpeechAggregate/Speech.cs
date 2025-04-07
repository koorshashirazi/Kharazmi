using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Events;
using Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.Exceptions;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.SpeechAggregate
{
    public class Speech : AggregateRoot<Speech, string>
    {
        public Title Title => new Title(_title);
        private string _title;

        private string _url;
        public UriValue Uri => new UriValue(_url);

        private string _description;
        public Description Description => new Description(_description);

        private int _type;
        public SpeechType Type => new SpeechType(_type);

        private readonly List<MediaFile> _mediaFileItems;

        public IReadOnlyCollection<MediaFile> MediaFileItems => _mediaFileItems;

        //EF Core need a parameterless constructor
        private Speech() : base(AspNetCore.Core.ValueObjects.Id.Default<string>())
        {
            _mediaFileItems = new List<MediaFile>();
        }
        
        public Speech(string id, Title title, UriValue uriValue, Description description, SpeechType type) : base(id)
        {
            Id = id;
            _title = title.Value ?? throw new ArgumentNullAggregateException(nameof(title));
            _url = uriValue.Value ?? throw new ArgumentNullAggregateException(nameof(uriValue));
            _description = description.Value ?? throw new ArgumentNullAggregateException(nameof(description));
            _type = type.IntValue;
            _mediaFileItems = new List<MediaFile>();

            RegisterApplier<SpeechTypeChangedEvent>(Apply);
            RegisterApplier<SpeechUrlChangedEvent>(Apply);
            RegisterApplier<SpeechDescriptionChangedEvent>(Apply);
            RegisterApplier<SpeechTitleChangedEvent>(Apply);
            RegisterApplier<SpeechCreatedEvent>(Apply);
            RegisterApplier<MediaFileCreatedEvent>(Apply);

            Emit(new SpeechCreatedEvent(Id, Title.Value, Uri.Value, Description.Value,
                Type.Value.ToString()));
        }
        
        public void CreateMedia(MediaFile mediaFile, ulong originalVersion)
        {
            if (mediaFile == null)
            {
                throw new ArgumentNullAggregateException(nameof(mediaFile));
            }

            if (_mediaFileItems.Contains(mediaFile))
            {
                throw new MediaFileAlreadyExisteDomainException(nameof(mediaFile));
            }

            var domainEvent = new MediaFileCreatedEvent(Id, mediaFile.Id, mediaFile.File.Value);
            domainEvent.EventMetadata.SetAggregateVersion(originalVersion);

            Emit(domainEvent);
        }

        public void Apply(SpeechCreatedEvent ev)
        {
            Id = ev.EventMetadata.AggregateId;
            _title = ev.Title;
            _url = ev.Url;
            _description = ev.Description;
            _type = new SpeechType(ev.Type).IntValue;
        }

        public void Apply(MediaFileCreatedEvent ev)
        {
            if (_mediaFileItems.All(c => c.File.Value != ev.File))
            {
                _mediaFileItems.Add(new MediaFile(ev.MediaFileId, new UriValue(ev.File)));
            }
        }

        #region - update title

        public void ChangeTitle(Title title, ulong originalVersion)
        {
            var domainEvent = new SpeechTitleChangedEvent(Id, title.Value);
            domainEvent.EventMetadata.SetAggregateVersion(originalVersion);
            Emit(domainEvent);
        }

        public void ChangeDescription(Description description, ulong originalVersion)
        {
            var domainEvent = new SpeechDescriptionChangedEvent(Id, description.Value);
            domainEvent.EventMetadata.SetAggregateVersion(originalVersion);
            Emit(domainEvent);
        }

        public void ChangeUrl(UriValue uri, ulong originalVersion)
        {
            var domainEvent = new SpeechUrlChangedEvent(Id, uri.Value);
            domainEvent.EventMetadata.SetAggregateVersion(originalVersion);
            Emit(domainEvent);
        }

        public void ChangeType(SpeechType type, ulong originalVersion)
        {
            var domainEvent = new SpeechTypeChangedEvent(Id, type.StringValue);
            domainEvent.EventMetadata.SetAggregateVersion(originalVersion);
            Emit(domainEvent);
        }

        public void Apply(SpeechTitleChangedEvent ev)
        {
            if (ev.EventMetadata.AggregateId != Id)
            {
                throw new InvalidDomainEventException(
                    $"Cannot apply event : Speech Id ({Id}) is not equals to AggregateId ({ev.EventMetadata.AggregateId}) of the event , {nameof(SpeechTitleChangedEvent)}");
            }

            _title = ev.Title;
        }

        public void Apply(SpeechDescriptionChangedEvent ev)
        {
            if (ev.EventMetadata.AggregateId != Id)
            {
                throw new InvalidDomainEventException(
                    $"Cannot apply event : Speech Id ({Id}) is not equals to AggregateId ({ev.EventMetadata.AggregateId}) of the event , {nameof(SpeechDescriptionChangedEvent)}");
            }

            _description = ev.Description;
        }

        public void Apply(SpeechUrlChangedEvent ev)
        {
            if (ev.EventMetadata.AggregateId != Id)
            {
                throw new InvalidDomainEventException(
                    $"Cannot apply event : Speech Id ({Id}) is not equals to AggregateId ({ev.EventMetadata.AggregateId}) of the event , {nameof(SpeechUrlChangedEvent)}");
            }

            _url = ev.Url;
        }

        public void Apply(SpeechTypeChangedEvent ev)
        {
            if (ev.EventMetadata.AggregateId != Id)
            {
                throw new InvalidDomainEventException(
                    $"Cannot apply event : Speech Id ({Id}) is not equals to AggregateId ({ev.EventMetadata.AggregateId}) of the event , {nameof(SpeechTypeChangedEvent)}");
            }

            _type = new SpeechType(ev.Type).IntValue;
        }

        #endregion - update title
    }
}