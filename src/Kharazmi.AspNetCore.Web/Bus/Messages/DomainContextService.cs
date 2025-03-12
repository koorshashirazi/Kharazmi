using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Web.Extensions;
using Kharazmi.AspNetCore.Web.Http;

namespace Kharazmi.AspNetCore.Web.Bus.Messages
{
    internal class DomainContextService : IDomainContextService
    {
        private readonly IUserContextService _userContextService;
        private DomainContext _domainContext;

        public DomainContextService(IUserContextService userContextService)
        {
            _userContextService = userContextService;
        }

        public Task<DomainContext> CreateAsync<TDomain>(
            string domainId = null,
            string resourceId = "",
            string resource = "")
        {
            if (_userContextService == null)
            {
                return Task.FromResult(DomainContext.Empty);
            }

            if (resource.IsNotEmpty())
            {
                resource = $"{resource}/{resourceId}";
            }

            resourceId = resourceId.IsEmpty() ? Guid.Empty.ToString("N") : resourceId;

            _domainContext = DomainContext.Create<TDomain>(
                domainId,
                _userContextService.UserId,
                resourceId,
                resource,
                _userContextService.CultureName ?? System.Threading.Thread.CurrentThread.CurrentCulture.Name,
                _userContextService.TraceId,
                _userContextService.ConnectionId,
                _userContextService.Request?.GetOrigin(),
                _userContextService.RequestPath
            );

            return Task.FromResult(_domainContext);
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