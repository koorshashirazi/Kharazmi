using System;
using System.Threading.Tasks;

namespace Kharazmi.AspNetCore.Core.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDomainContextService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="domainId"></param>
        /// <param name="resourceId"></param>
        /// <param name="resource"></param>
        /// <returns></returns>
        Task<DomainContext> CreateAsync(string? domainId = null, string resourceId = "", string resource = "");

        Task<DomainContext> GetAsync(string resourceId = "", string resource = "");
        Task<DomainContext> UpdateAsync(Action<DomainContext> domainContext);
    }

    internal class DomainContextService : IDomainContextService
    {
        private DomainContext? _domainContext;

        public Task<DomainContext> CreateAsync(string? domainId = null, string resourceId = "",
            string resource = "")
        {
            return Task.FromResult(DomainContext.Empty);
        }

        public async Task<DomainContext> GetAsync(string resourceId = "", string resource = "")
        {
            _domainContext ??= await CreateAsync(null, resourceId, resource).ConfigureAwait(false);

            return await Task.FromResult(_domainContext).ConfigureAwait(false);
        }

        public async Task<DomainContext> UpdateAsync(Action<DomainContext> domainContext)
        {
            _domainContext = await GetAsync().ConfigureAwait(false);

            domainContext.Invoke(_domainContext);

            return _domainContext;
        }
    }
}