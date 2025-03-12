namespace Kharazmi.AspNetCore.Core.Contracts
{
    public interface IDeleteModel : IModel
    {
        byte[] RowVersion { get; set; }
    }
}
