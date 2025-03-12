namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    public interface IDeletedEntity
    {
        bool IsDeleted { get; }
    }
}