using System;

namespace Kharazmi.AspNetCore.Core.Timing
{
    /// <summary>
    /// 
    /// </summary>
    public static class SystemTime
    {
        /// <summary>
        /// 
        /// </summary>
        public static Func<DateTime> Now = () => DateTime.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        public static Func<DateTime, DateTime> Normalize = (dateTime) =>
            DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}