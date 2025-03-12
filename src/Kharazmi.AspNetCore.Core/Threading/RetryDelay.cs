using System;

namespace Kharazmi.AspNetCore.Core.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public class RetryDelay
    {
        private static readonly Random Random = new Random();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static RetryDelay Between(TimeSpan min, TimeSpan max)
        {
            return new RetryDelay(min, max);
        }

        private RetryDelay(
            TimeSpan min,
            TimeSpan max)
        {
            if (min.Ticks < 0) throw new ArgumentOutOfRangeException(nameof(min), "Minimum cannot be negative");
            if (max.Ticks < 0) throw new ArgumentOutOfRangeException(nameof(max), "Maximum cannot be negative");

            Min = min;
            Max = max;
        }

        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Max { get; }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan Min { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TimeSpan PickDelay()
        {
            return Min.Add(TimeSpan.FromMilliseconds((Max.TotalMilliseconds - Min.TotalMilliseconds) * Random.NextDouble()));
        }
    }
}