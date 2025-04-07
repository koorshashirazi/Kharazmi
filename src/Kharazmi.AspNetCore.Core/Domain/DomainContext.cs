using System;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain
{
    [Serializable]
    public class DomainContext
    {
        public string Id { get; private set; }
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

        private DomainContext(string id)
        {
            Id = id.IsEmpty() ? Guid.NewGuid().ToString("N") : id;
            Culture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        [JsonConstructor]
        private DomainContext(
            string id,
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

        public static DomainContext Empty => new();

        public static DomainContext Create(string id, string userId, string resourceId, string resource,
            string culture,
            string traceId, string connectionId, string origin, string requestPath, int retries = 0)
            => new(id, userId, resourceId, resource, culture, traceId, connectionId,
                origin, requestPath,
                retries);

        public static DomainContext FromId(string id) => new(id);

        public static DomainContext From(DomainContext context)
            => Create(context.Id, context.UserId, context.ResourceId, context.Resource, context.Culture,
                context.TraceId, context.ConnectionId, context.Origin, context.RequestPath,
                context.Retries);

        public static DomainContext For(string id) => new(id);


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
            Retries = retry;
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