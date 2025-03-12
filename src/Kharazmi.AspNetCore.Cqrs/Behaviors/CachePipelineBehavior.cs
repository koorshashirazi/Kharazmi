

namespace Kharazmi.AspNetCore.Cqrs.Behaviors
{
    // https://anderly.com/2019/12/12/cross-cutting-concerns-with-mediatr-pipeline-behaviors/
//    public class CachePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
//    {
//        private readonly IDistributedCache cache;
//        private readonly ILogger<SGUKClassActionClaimantPortal> logger;
//
//        public CachePipelineBehavior(IDistributedCache cache, ILogger<SGUKClassActionClaimantPortal> logger)
//        {
//            Ensure.ThrowIfIsNull(cache, nameof(cache));
//            Ensure.ThrowIfIsNull(logger, nameof(logger));
//            this.cache = cache;
//            this.logger = logger;
//        }
//
//        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
//        {
//            if (request is ICacheableRequest<TResponse> cacheableRequest)
//            {
//                var key = cacheableRequest.GetCacheKey();
//                
//                return await cache.GetOrSet(
//                    key,
//                    miss: () => { Log.CacheMiss(logger, key); return next(); },
//                    hit: (data) => Log.CacheHit(logger, key),
//                    cacheableRequest.GetExpirationTime(),
//                    cancellationToken);
//            }
//
//            var response = await next();
//
//            if (request is ICacheInvalidationRequest cacheInvalidationRequest)
//            {
//                await cache.Remove(cacheInvalidationRequest.GetCacheKey(), cancellationToken);
//            }
//
//            return response;
//        }
//    }
}