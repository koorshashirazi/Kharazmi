using Kharazmi.AspNetCore.Core.Cryptography;
using Kharazmi.AspNetCore.Security.Authentication;
using Kharazmi.AspNetCore.Security.Cryptography;
using Kharazmi.AspNetCore.Security.Firewall;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Kharazmi.AspNetCore.Security
{
    public class SecurityBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public SecurityBuilder(IServiceCollection services)
        {
            Services = services;
        }
       
        public SecurityBuilder WithPasswordHashAlgorithm()
        {
            Services.AddSingleton<IUserPasswordHashAlgorithm, UserPasswordHashAlgorithm>();
            return this;
        }
       
        public SecurityBuilder WithAntiforgery()
        {
            Services.AddScoped<IAntiforgeryService, AntiforgeryService>();
            return this;
        }
        
        /// <summary>
        /// Register IAntiDosFirewall
        /// </summary>
        /// <returns></returns>
        public SecurityBuilder WithAntiDosFirewall()
        {
            Services.AddAntiDosFirewall();
            return this;
        }
        
        public SecurityBuilder WithDataProtection()
        {
            Services.AddSingleton<IProtectionProvider, ProtectionProvider>();
            Services.AddSingleton<IXmlRepository, XmlRepository>();
            Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(provider =>
            {
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    using var scope = provider.CreateScope();
                    options.XmlRepository = scope.ServiceProvider.GetService<IXmlRepository>();
                });
            });
            return this;
        }

        public SecurityBuilder WithDistributeCacheSessionStore()
        {
            Services.AddScoped<ITicketStore, DistributedCacheTicketStore>();
            return this;
        }
        
        public SecurityBuilder WithMemoryCacheSessionStore()
        {
            Services.AddScoped<ITicketStore, MemoryCacheTicketStore>();
            return this;
        }
    }
}