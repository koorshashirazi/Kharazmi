using System;
using Kharazmi.AspNetCore.Core.Threading;

namespace Kharazmi.AspNetCore.Core.HandlerRetry
{
    internal class HandlerTransientRetryDelay : IHandlerTransientRetryDelay
    {
        private readonly IHandlerRetryConfig _config;

        public HandlerTransientRetryDelay(IHandlerRetryConfig config)
        {
            _config = config;
        }

        public RetryDelay TransientRetryDelay =>
            RetryDelay.Between(TimeSpan.FromSeconds(_config.Min), TimeSpan.FromSeconds(_config.Max));
    }
}