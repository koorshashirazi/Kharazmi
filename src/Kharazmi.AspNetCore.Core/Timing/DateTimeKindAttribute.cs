using System;

namespace Kharazmi.AspNetCore.Core.Timing
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class DateTimeKindAttribute : Attribute
    {
        public DateTimeKind Kind { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kind"></param>
        public DateTimeKindAttribute(DateTimeKind kind)
        {
            Kind = kind;
        }
    }
}