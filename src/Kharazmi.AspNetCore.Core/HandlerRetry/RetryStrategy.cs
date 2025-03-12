using System;
using Kharazmi.AspNetCore.Core.Threading;

namespace Kharazmi.AspNetCore.Core.HandlerRetry
{
    internal class RetryStrategy : IRetryStrategy
    {
        public Retry ShouldBeRetied(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount)
        {
            return Retry.No;
        }
    }
}