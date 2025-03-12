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
        /// <typeparam name="TDomain"></typeparam>
        /// <returns></returns>
        Task<DomainContext> CreateAsync<TDomain>(string domainId = null, string resourceId = "", string resource = "");

        Task<DomainContext> GetAsync<TDomain>(string resourceId = "", string resource = "");
        Task<DomainContext> UpdateAsync<TDomain>(DomainContext domainContext);
    }

    internal class DomainContextService : IDomainContextService
    {
        private DomainContext _domainContext;

        public Task<DomainContext> CreateAsync<TDomain>(string domainId = null, string resourceId = "",
            string resource = "")
        {
            return Task.FromResult(DomainContext.Empty);
        }


        public async Task<DomainContext> GetAsync<TDomain>(string resourceId = "", string resource = "")
        {
            if (_domainContext == null)
                _domainContext = await CreateAsync<TDomain>(null, resourceId, resource).ConfigureAwait(false);

            return await Task.FromResult(_domainContext).ConfigureAwait(false);
        }

        public async Task<DomainContext> UpdateAsync<TDomain>(DomainContext domainContext)
        {
            _domainContext = await GetAsync<TDomain>().ConfigureAwait(false);
            _domainContext = DomainContext.From<TDomain>(domainContext);
            return _domainContext;
        }
    }
}