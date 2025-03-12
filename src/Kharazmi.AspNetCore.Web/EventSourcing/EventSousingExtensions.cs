using System;
using Kharazmi.AspNetCore.Core.Cqrs;
using Kharazmi.AspNetCore.Core.EventSourcing;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Web.EventSourcing
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventSousingExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static ICqrsBuilder WithUserInfo(this ICqrsBuilder builder,
            Action<UserInfoOptions>? options = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var userOptions = new UserInfoOptions();
            options?.Invoke(userOptions);
            builder.Services.AddSingleton(userOptions);
            builder.Services.AddService<IEventUserInfoService, EventUserInfoService>(serviceLifetime);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="options"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static ICqrsBuilder WithUserClaimsInfo(this ICqrsBuilder builder,
            Action<UserClaimInfoOptions>? options = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var userClaimsOptions = new UserClaimInfoOptions();
            options?.Invoke(userClaimsOptions);
            builder.Services.AddSingleton(userClaimsOptions);
            builder.Services.AddService<IEventUserClaimInfoService, EventUserClaimInfoService>(serviceLifetime);
            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static ICqrsBuilder WithRequestInfo(this ICqrsBuilder builder,
            Action<RequestInfoOptions>? options = null,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var requestInfoOptions = new RequestInfoOptions();
            options?.Invoke(requestInfoOptions);
            builder.Services.AddSingleton(requestInfoOptions);
            builder.Services.AddService<IEventRequestInfoService, EventRequestInfoService>(serviceLifetime);
            return builder;
        }
    }
}