using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate
{
    public class MediaFile : Entity<Guid>
    {
        public UrlValue File { get; private set; }

        //EF Core need a parameterless constructor
        private MediaFile()
        {
        }

        public MediaFile(Guid id, UrlValue file)
        {
            Id = id;
            File = file ?? throw new ArgumentNullAggregateException(nameof(file));
        }
    }
}