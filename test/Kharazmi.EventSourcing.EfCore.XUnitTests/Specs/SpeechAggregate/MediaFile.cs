using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.SpeechAggregate
{
    public class MediaFile : Entity<Guid>
    {
        public UriValue File { get; private set; }

        //EF Core need a parameterless constructor
        private MediaFile()
        {
        }

        public MediaFile(Guid id, UriValue file)
        {
            Id = id;
            File = file;
        }
    }
}