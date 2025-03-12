using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.AuditLogging;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.EFCore.AuditLogging
{
    public class AuditEventLogger : IAuditEventLogger
    {
        protected readonly IEnumerable<IAuditEventLoggerSink> Sinks;
        protected readonly IAuditSubject AuditSubject;
        protected readonly IAuditAction AuditAction;
        private readonly AuditLoggerOptions _auditLoggerOptions;

        public AuditEventLogger(
          IEnumerable<IAuditEventLoggerSink> sinks,
          IAuditSubject auditSubject,
          IAuditAction auditAction,
          AuditLoggerOptions auditLoggerOptions)
        {
            Sinks = sinks;
            Sinks.CheckArgumentIsNull(nameof(Sinks));
            
            AuditSubject = auditSubject;
            AuditSubject.CheckArgumentIsNull(nameof(AuditSubject));
            
            AuditAction = auditAction;
            AuditAction.CheckArgumentIsNull(nameof(AuditAction));
            
            _auditLoggerOptions = auditLoggerOptions;
            _auditLoggerOptions.CheckArgumentIsNull(nameof(_auditLoggerOptions));
        }

        protected virtual Task PrepareEventAsync(
          AuditEvent auditEvent,
          Action<AuditLoggerOptions> loggerOptions)
        {
            if (loggerOptions == null)
            {
                PrepareDefaultValues(auditEvent, _auditLoggerOptions);
            }
            else
            {
                var loggerOptions1 = new AuditLoggerOptions();
                loggerOptions(loggerOptions1);
                PrepareDefaultValues(auditEvent, loggerOptions1);
            }
            return Task.CompletedTask;
        }

        private void PrepareDefaultValues(AuditEvent auditEvent, AuditLoggerOptions loggerOptions)
        {
            if (loggerOptions.UseDefaultSubject)
                PrepareDefaultSubject(auditEvent);
            if (loggerOptions.UseDefaultAction)
                PrepareDefaultAction(auditEvent);
            PrepareDefaultConfiguration(auditEvent, loggerOptions);
        }

        private static void PrepareDefaultConfiguration(
          AuditEvent auditEvent,
          AuditLoggerOptions loggerOptions)
        {
            auditEvent.Source = loggerOptions.Source;
        }

        private void PrepareDefaultAction(AuditEvent auditEvent)
        {
            auditEvent.Action = AuditAction.Action;
        }

        private void PrepareDefaultSubject(AuditEvent auditEvent)
        {
            auditEvent.SubjectName = AuditSubject.SubjectName;
            auditEvent.SubjectIdentifier = AuditSubject.SubjectIdentifier;
            auditEvent.SubjectType = AuditSubject.SubjectType;
            auditEvent.SubjectAdditionalData = AuditSubject.SubjectAdditionalData;
        }

        public virtual async Task LogEventAsync(
          AuditEvent auditEvent,
          Action<AuditLoggerOptions> loggerOptions = null)
        {
            await PrepareEventAsync(auditEvent, loggerOptions).ConfigureAwait(false);
            foreach (var sink in Sinks)
                await sink.PersistAsync(auditEvent).ConfigureAwait(false);
        }
    }
    
}