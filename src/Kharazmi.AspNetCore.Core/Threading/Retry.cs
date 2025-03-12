using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;

namespace Kharazmi.AspNetCore.Core.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public class Retry : ValueObject
    {
        /// <summary>
        /// 
        /// </summary>
        public static Retry Yes { get; } = new Retry(true, TimeSpan.Zero);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="retryAfter"></param>
        /// <returns></returns>
        public static Retry YesAfter(TimeSpan retryAfter) => new Retry(true, retryAfter);
        /// <summary>
        /// 
        /// </summary>
        public static Retry No { get; } = new Retry(false, TimeSpan.Zero);

        /// <summary>
        /// 
        /// </summary>
        public bool ShouldBeRetried { get; }
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan RetryAfter { get; }

        private Retry(bool shouldBeRetried, TimeSpan retryAfter)
        {
            if (retryAfter != TimeSpan.Zero && retryAfter != retryAfter.Duration())
                throw new ArgumentOutOfRangeException(nameof(retryAfter));
            if (!shouldBeRetried && retryAfter != TimeSpan.Zero)
                throw new ArgumentException("Invalid combination. Should not be retried and retry after set");

            ShouldBeRetried = shouldBeRetried;
            RetryAfter = retryAfter;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return ShouldBeRetried;
                yield return RetryAfter;
            }
        }
    }
}