using Kharazmi.AspNetCore.Core.Domain.Entities;

namespace Kharazmi.AspNetCore.Core.Numbering
{
    public class NumberedEntity : Entity<int>
    {
        public NumberedEntity(int id) : base(id)
        {
        }

        public string EntityName { get; set; } = string.Empty;
        public long NextNumber { get; set; }
    }
}