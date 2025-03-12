using Microsoft.Extensions.DependencyInjection;

 namespace Kharazmi.AspNetCore.Web.Mail
{
    /// <summary>
    /// 
    /// </summary>
    public static class EmailServiceExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serverOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddEmailServices(this IServiceCollection services,
            EmailServerOptions serverOptions)
        {
            services.AddSingleton(provider => serverOptions);
            services.AddScoped<IEmailService, EmailService>();
            return services;
        }
    }
}