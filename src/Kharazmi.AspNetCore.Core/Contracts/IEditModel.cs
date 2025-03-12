namespace Kharazmi.AspNetCore.Core.Contracts
{
    public interface IEditModel : IModel
    {
        byte[] RowVersion { get; set; }
    }
}