using System;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.HandlerRetry
{
    internal class HandlerRetryStrategy : IHandlerRetryStrategy
    {
        private readonly IHandlerRetryConfig _config;
        private readonly IHandlerTransientRetryDelay _retryDelay;

        public HandlerRetryStrategy(IHandlerRetryConfig config, IHandlerTransientRetryDelay retryDelay)
        {
            _config = config;
            _retryDelay = retryDelay;
        }

        public Threading.Retry ShouldBeRetied(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount)
        {
            if (!(exception is MessageBusException)  ||
                currentRetryCount > _config.Attempt)
            {
                return Threading.Retry.No;
            }

            return _config.Attempt >= currentRetryCount
                ? Threading.Retry.YesAfter(_retryDelay.TransientRetryDelay.PickDelay())
                : Threading.Retry.No;
        }
    }
}