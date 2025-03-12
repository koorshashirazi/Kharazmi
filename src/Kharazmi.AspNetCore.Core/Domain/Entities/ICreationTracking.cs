using System;

namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public interface ICreationTracking
    {
        /// <summary>
        /// 
        /// </summary>
        DateTime CreatedDateTime { get; set; }
    }
}