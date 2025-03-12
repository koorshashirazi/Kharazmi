using System;

 namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModificationTracking
    {
        /// <summary>
        /// 
        /// </summary>
        DateTime? ModifiedDateTime { get; set; }

    }
}