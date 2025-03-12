namespace Kharazmi.AspNetCore.Core.Contracts
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}