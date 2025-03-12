namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHasRowIntegrity
    {
        string Hash { get; set; }
    }
}