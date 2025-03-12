using System;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Web.AuditLogging
{
    /// <summary>
    /// 
    /// </summary>
    public static class AuditLoggingExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="subjectOptions"></param>
        /// <param name="actionOptions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IAuditLoggingBuilder WithHttpEventData(
            this IAuditLoggingBuilder builder,
            Action<AuditHttpSubjectOptions> subjectOptions = null,
            Action<AuditHttpActionOptions> actionOptions = null)
        {
            if (actionOptions == null) throw new ArgumentNullException(nameof(actionOptions));
            var implementationInstance1 = new AuditHttpSubjectOptions();
            subjectOptions?.Invoke(implementationInstance1);
            builder.Services.AddSingleton(implementationInstance1);
            
            var implementationInstance2 = new AuditHttpActionOptions();
            actionOptions?.Invoke(implementationInstance2);
            builder.Services.AddSingleton(implementationInstance2);
            
            builder.Services.AddTransient<IAuditSubject, HttpAuditSubject>();
            builder.Services.AddTransient<IAuditAction, HttpAuditAction>();
            return builder;
        }
    }
}