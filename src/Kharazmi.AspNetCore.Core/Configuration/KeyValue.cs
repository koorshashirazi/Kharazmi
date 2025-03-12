using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.Configuration
{
    public class KeyValue : Entity<int>, IModificationTracking, ICreationTracking, IHasRowIntegrity
    {
        public KeyValue()
        {
        }

        public KeyValue(int id) : base(id)
        {
        }

        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime? ModifiedDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
        public string Hash { get; set; } = string.Empty;
    }
}