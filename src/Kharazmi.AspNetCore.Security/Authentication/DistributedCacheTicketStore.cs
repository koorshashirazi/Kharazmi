using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Distributed;

namespace Kharazmi.AspNetCore.Security.Authentication
{
    /// <summary>
    /// Implementation of  ITicketStore
    /// </summary>
    public class DistributedCacheTicketStore : ITicketStore
    {
        private const string KeyPrefix = "TicketStore-";
        private readonly IDistributedCache _cache;
        private readonly IDataSerializer<AuthenticationTicket> _ticketSerializer = TicketSerializer.Default;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public DistributedCacheTicketStore(IDistributedCache cache)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// Store AuthenticationTicket
        /// </summary>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var key = $"{KeyPrefix}{Guid.NewGuid():N}";
            await RenewAsync(key, ticket).ConfigureAwait(false);
            return key;
        }

        /// <summary>
        /// Renew AuthenticationTicket
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            var options = new DistributedCacheEntryOptions();

            var expiresUtc = ticket.Properties.ExpiresUtc;
            if (expiresUtc.HasValue)
            {
                options.SetAbsoluteExpiration(expiresUtc.Value);
            }

            if (ticket.Properties.AllowRefresh.GetValueOrDefault(false))
            {
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30)); // TODO: configurable.
            }

            return _cache.SetAsync(key, _ticketSerializer.Serialize(ticket), options);
        }

        /// <summary>
        /// Retrieve AuthenticationTicket by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            var value = await _cache.GetAsync(key).ConfigureAwait(false);
            return value != null ? _ticketSerializer.Deserialize(value) : null;
        }

        /// <summary>
        /// Remove AuthenticationTicket cache by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task RemoveAsync(string key)
        {
            return _cache.RemoveAsync(key);
        }
    }
}