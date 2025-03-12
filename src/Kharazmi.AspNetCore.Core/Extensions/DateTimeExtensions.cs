using System;

 namespace Kharazmi.AspNetCore.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class DateTimeExtensions
    {
        private static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static double ToUnixTime(this DateTimeOffset dateTime)
        {
            return dateTime.Subtract(_epoch).TotalMilliseconds;
        }
    }
}