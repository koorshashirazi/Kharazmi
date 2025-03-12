namespace Kharazmi.AspNetCore.Core.HandlerRetry
{
    internal class HandlerRetryConfig : IHandlerRetryConfig
    {
        public int Attempt { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}