using Kharazmi.AspNetCore.Core.Threading;

 namespace Kharazmi.AspNetCore.Core.Test.Settings
{
    public class DomainHandlerRetryOptions: IRetryConfig
    {
        public int Attempt { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}