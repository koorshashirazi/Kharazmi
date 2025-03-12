namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IHasRowVersion
    {
        /// <summary>
        /// 
        /// </summary>
        byte[] Version { get; set; }
    }
}