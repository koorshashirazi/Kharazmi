using System;
using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Domain.Aggregates;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Events;
using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Exceptions;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate
{
    public class Speech : AggregateRoot<Speech, string>
    {
        public Title Title => new Title(_title);
        private string _title;

        private string _url;
        public UrlValue Url => new UrlValue(_url);

        private string _description;
        public Description Description => new Description(_description);

        private int _type;
        public SpeechType Type => new SpeechType(_type);

        private readonly List<MediaFile> _mediaFileItems;

        public IReadOnlyCollection<MediaFile> MediaFileItems => _mediaFileItems;

        //EF Core need a parameterless constructor
        private Speech()
        {
            _mediaFileItems = [];
        }

        public Speech(Title title, UrlValue urlValue,
            Description description, SpeechType type)
            : this(ValueObjects.Id.New<string>(), 0, title, urlValue, description, type)
        {
        }
        public Speech(string id, Title title, UrlValue urlValue,
            Description description, SpeechType type)
            : this(id, 0, title, urlValue, description, type)
        {
        }

        public Speech(string id, ulong version, Title title, UrlValue urlValue,
            Description description, SpeechType type) : base(id, version)
        {
            ArgumentNullException.ThrowIfNull(title);
            ArgumentNullException.ThrowIfNull(urlValue);
            ArgumentNullException.ThrowIfNull(description);
            ArgumentNullException.ThrowIfNull(type);

            Id = id;
            _title = title.Value;
            _url = urlValue.Value;
            _description = description.Value;
            _type = type.IntValue;
            _mediaFileItems = [];

            RegisterApplier<SpeechTypeChangedEvent>(Apply);
            RegisterApplier<SpeechUrlChangedEvent>(Apply);
            RegisterApplier<SpeechDescriptionChangedEvent>(Apply);
            RegisterApplier<SpeechTitleChangedEvent>(Apply);
            RegisterApplier<SpeechCreatedEvent>(Apply);
            RegisterApplier<MediaFileCreatedEvent>(Apply);

            Emit(new SpeechCreatedEvent(Id, Title.Value, Url.Value, Description.Value,
                Type.Value.ToString()));
        }


        public void CreateMedia(MediaFile mediaFile)
        {
            ArgumentNullException.ThrowIfNull(mediaFile);

            if (_mediaFileItems.Contains(mediaFile))
            {
                throw new MediaFileAlreadyExisteDomainException(nameof(mediaFile));
            }

            var domainEvent = new MediaFileCreatedEvent(Id, mediaFile.Id, mediaFile.File.Value);
            Emit(domainEvent);
        }

        public void Apply(SpeechCreatedEvent ev)
        {
            ArgumentNullException.ThrowIfNull(ev);
            Id = ev.EventMetadata.AggregateId;
            _title = ev.Title;
            _url = ev.Url;
            _description = ev.Description;
            _type = new SpeechType(ev.Type).IntValue;
        }

        public void Apply(MediaFileCreatedEvent ev)
        {
            ArgumentNullException.ThrowIfNull(ev);
            if (_mediaFileItems.All(c => c.File.Value != ev.File))
            {
                _mediaFileItems.Add(new MediaFile(ev.MediaFileId, new UrlValue(ev.File)));
            }
        }

        #region - update title

        public void ChangeTitle(Title title)
        {
            ArgumentNullException.ThrowIfNull(title);
            var domainEvent = new SpeechTitleChangedEvent(title.Value);
            Emit(domainEvent);
        }

        public void ChangeDescription(Description description)
        {
            ArgumentNullException.ThrowIfNull(description);
            var domainEvent = new SpeechDescriptionChangedEvent(description.Value);
            Emit(domainEvent);
        }

        public void ChangeUrl(UrlValue url)
        {
            ArgumentNullException.ThrowIfNull(url);
            var domainEvent = new SpeechUrlChangedEvent(Id, url.Value);
            Emit(domainEvent);
        }

        public void ChangeType(SpeechType type)
        {
            ArgumentNullException.ThrowIfNull(type);
            var domainEvent = new SpeechTypeChangedEvent(Id, type.StringValue);
            Emit(domainEvent);
        }

        public void Apply(SpeechTitleChangedEvent ev)
        {
            ArgumentNullException.ThrowIfNull(ev);

            if (ev.EventMetadata.AggregateId != Id)
            {
                throw new InvalidDomainEventException(
                    $"Cannot apply event : Speech Id ({Id}) is not equals to AggregateId ({ev.EventMetadata.AggregateId}) of the event , {nameof(SpeechTitleChangedEvent)}");
            }

            _title = ev.Title;
        }

        public void Apply(SpeechDescriptionChangedEvent ev)
        {
            ArgumentNullException.ThrowIfNull(ev);
            if (ev.EventMetadata.AggregateId != Id)
            {
                throw new InvalidDomainEventException(
                    $"Cannot apply event : Speech Id ({Id}) is not equals to AggregateId ({ev.EventMetadata.AggregateId}) of the event , {nameof(SpeechDescriptionChangedEvent)}");
            }

            _description = ev.Description;
        }

        public void Apply(SpeechUrlChangedEvent ev)
        {
            ArgumentNullException.ThrowIfNull(ev);
            if (ev.EventMetadata.AggregateId != Id)
            {
                throw new InvalidDomainEventException(
                    $"Cannot apply event : Speech Id ({Id}) is not equals to AggregateId ({ev.EventMetadata.AggregateId}) of the event , {nameof(SpeechUrlChangedEvent)}");
            }

            _url = ev.Url;
        }

        public void Apply(SpeechTypeChangedEvent ev)
        {
            ArgumentNullException.ThrowIfNull(ev);
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