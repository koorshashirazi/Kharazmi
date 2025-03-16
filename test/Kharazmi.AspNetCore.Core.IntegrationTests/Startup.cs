using System;
using System.Security.Claims;
using Kharazmi.AspNetCore.Core.Bus;
using Kharazmi.AspNetCore.Core.Domain.Events;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.IntegrationTests.Models;
using Kharazmi.AspNetCore.Core.Pipelines;
using Kharazmi.AspNetCore.Core.Threading;
using Kharazmi.AspNetCore.Validation;
using Kharazmi.AspNetCore.Web;
using Kharazmi.AspNetCore.Web.Bus;
using Kharazmi.EventSourcing.EfCore;
using Kharazmi.MessageBroker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kharazmi.AspNetCore.Core.IntegrationTests
{
    public class DomainHandlerRetryOptions : IRetryConfig
    {
        public int Attempt { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            var validationBuilder = services.AddValidatorsFromAssembly();

            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<EventStoreDbContext>(
                    (serviceProvider, optionsBuilder) =>
                    {
                        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString("N"));
                    }, ServiceLifetime.Singleton);

            // using Scrutor DI and more features
            services.AddCoreFramework()
                .AddEventBus()
                .WithCommandValidation(validationBuilder)
                .WithRetryPipeline(new DomainHandlerRetryOptions
                {
                    Attempt = 3,
                    Min = 5,
                    Max = 15
                })
                .WithValidationCommandPipeline(validationBuilder)
                .WithTransientFaultCommandPipeline()
                .WithLoggerCommandPipeline()
                .WithTransientFaultEventPipeline()
                .Builder
                .AddRabbitMq()
                .AddEventSourcing(new EventSourcingOptions
                {
                    EnableStoreEvent = true
                })
                .WithDefaultEventStore()
                .WithRequestInfo(option => option.IncludeFormVariables = true)
                .WithUserInfo(x =>
                {
                    x.UserIdClaimType = ClaimTypes.Name;
                    x.UserNameClaimType = ClaimTypes.NameIdentifier;
                })
                .WithUserClaimsInfo(x =>
                {
                    x.SpecifiedClaimTypes =
                    [
                        ClaimTypes.Name,
                        ClaimTypes.NameIdentifier,
                        "custom-claim"
                    ];
                });


            services.AddWebApp()
                .WithUserContext();

//            services.AddTransient(typeof(ICommandHandler<>), typeof(LoggerCommandPipeline<>));
            //  using reflection to only register handler and pipeline
//            services.RegisterHandlersWithPipeline(Assembly.GetEntryAssembly());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseRabbitMq()
                .SubscribeEventFrom<RejectEvent>()
                .SubscribeEvent<AddUserDomainEvent>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}