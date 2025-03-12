using Microsoft.Extensions.Logging;

 namespace Kharazmi.AspNetCore.Core.Test.Settings
{
    public class Logging
    {
        public bool IncludeScopes { get; set; }
        public LogLevel LogLevel { get; set; }
    }
}