using Kharazmi.AspNetCore.Core.IntegrationTests.Settings;
using Kharazmi.AspNetCore.Web.Configuration;

namespace Kharazmi.AspNetCore.Core.IntegrationTests
{
    
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppSettings<AppSettings>()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
    
    
}