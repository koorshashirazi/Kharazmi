using System;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain
{
    [Serializable]
    public class DomainContext 
    {
        public string Id { get; private set; }
        public string ActionName { get; }
        public string UserId { get; }
        public string ResourceId { get; private set; }
        public string Resource { get; private set; }
        public string TraceId { get; }
        public string ConnectionId { get; }
        public string Origin { get; }
        public string RequestPath { get; }
        public string Culture { get; }
        public int Retries { get; private set; }
        public DateTime CreatedAt { get; }

        public DomainContext() : this("")
        {
        }

        private DomainContext(string id, string actionName = "")
        {
            Id = id.IsEmpty() ? Guid.NewGuid().ToString("N") : id;
            ActionName = actionName;
            Culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        [JsonConstructor]
        private DomainContext(
            string id,
            string name,
            string userId,
            string resourceId,
            string resource,
            string culture,
            string traceId,
            string connectionId,
            string origin,
            string requestPath,
            int retries)
        {
            Id = id.IsEmpty() ? Guid.NewGuid().ToString("N") : id;
            if (name.IsNotEmpty())
                ActionName = name;
            UserId = userId;
            ResourceId = resourceId;
            Resource = resource;
            TraceId = traceId;
            ConnectionId = connectionId;
            Culture = culture;
            Origin = origin;
            RequestPath = requestPath;
            Retries = retries;
            CreatedAt = DateTime.UtcNow;
        }

        public static DomainContext Empty
            => new DomainContext();

        public static DomainContext Create<T>(string id, string userId, string resourceId, string resource,
            string culture,
            string traceId, string connectionId, string origin, string requestPath, int retries = 0)
            => new DomainContext(id, typeof(T).Name, userId, resourceId, resource, culture, traceId, connectionId,
                origin, requestPath,
                retries);

        public static DomainContext FromId(string id)
            => new DomainContext(id);

        public static DomainContext From<T>(DomainContext context)
            => Create<T>(context.Id, context.UserId, context.ResourceId, context.Resource, context.Culture,
                context.TraceId, context.ConnectionId, context.Origin, context.RequestPath,
                context.Retries);

        public static DomainContext For(string id, string actionName)
            => new DomainContext(id, actionName);


        public DomainContext UpdateId(string id)
        {
            Id = id;
            return this;
        }

        public DomainContext UpdateResourceId(string resourceId)
        {
            ResourceId = resourceId;
            return this;
        }

        public DomainContext UpdateResource(string resource)
        {
            Resource = resource;
            return this;
        }

        public DomainContext UpdateRetrying(int retry)
        {
            Retries =retry;
            return this;
        }   
        
        public DomainContext IncreaseRetrying()
        {
            Retries += 1;
            return this;
        }

        private static string GetName(string name)
            => name.Underscore().ToLowerInvariant();

        public Guid GlobalRequestId { get; set; }
    }
}