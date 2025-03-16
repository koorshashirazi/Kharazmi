using Microsoft.Extensions.Logging;

 namespace Kharazmi.AspNetCore.Core.IntegrationTests.Settings
{
    public class Logging
    {
        public bool IncludeScopes { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}