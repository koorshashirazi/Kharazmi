using Kharazmi.AspNetCore.Core.Domain.Entities;

 namespace Kharazmi.AspNetCore.Core.Cryptography
{
    /// <summary>
    /// 
    /// </summary>
    public class ProtectionKey : Entity<string>
    {
        /// <summary>
        /// 
        /// </summary>
        public string FriendlyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string XmlData { get; set; }
    }
}