using System;
using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.EntityHistory
{
    //TODO: under development
    public class EntityHistory : Entity<Guid>, ICreationTracking
    {
        public EntityHistory(Guid id) : base(id)
        {
        }

        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string JsonOriginalValue { get; set; } = string.Empty;
        public string JsonNewValue { get; set; } = string.Empty;
        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;
    }
}