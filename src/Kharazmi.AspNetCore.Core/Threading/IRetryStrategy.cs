using System;

namespace Kharazmi.AspNetCore.Core.Threading
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRetryStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="totalExecutionTime"></param>
        /// <param name="currentRetryCount"></param>
        /// <returns></returns>
        Retry ShouldBeRetied(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount);
    }
}