using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Domain;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Web.Extensions;
using Kharazmi.AspNetCore.Web.Http;

namespace Kharazmi.AspNetCore.Web.Bus.Messages
{
    internal sealed class DomainContextService : IDomainContextService
    {
        private readonly IUserContextService _userContextService;
        private DomainContext _domainContext = DomainContext.Empty;

        public DomainContextService(IUserContextService userContextService)
        {
            _userContextService = userContextService ?? throw new ArgumentNullException(nameof(userContextService));
        }

        public Task<DomainContext> CreateAsync(
            string? domainId = null,
            string resourceId = "",
            string resource = "")
        {
            if (resource.IsNotEmpty())
            {
                resource = $"{resource}/{resourceId}";
            }

            resourceId = resourceId.IsEmpty() ? Guid.Empty.ToString("N") : resourceId;

            _domainContext = DomainContext.Create(
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

        public async Task<DomainContext> GetAsync(string resourceId = "", string resource = "")
        {
            _domainContext = await CreateAsync(null, resourceId, resource).ConfigureAwait(false);
            return _domainContext;
        }

        public async Task<DomainContext> UpdateAsync(Action<DomainContext> domainContext)
        {
            _domainContext = await GetAsync().ConfigureAwait(false);
            domainContext.Invoke(_domainContext);
            return _domainContext;
        }
    }
}