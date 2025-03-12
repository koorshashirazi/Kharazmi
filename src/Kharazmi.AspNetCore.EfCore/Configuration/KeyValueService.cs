using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Application.Services;
using Kharazmi.AspNetCore.Core.Configuration;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;

namespace Kharazmi.AspNetCore.EFCore.Configuration
{
    internal sealed class KeyValueService<TDbContext> : ApplicationService, IKeyValueService where TDbContext : DbContext
    {
        private readonly IUnitOfWork<TDbContext> _uow;
        private readonly DbSet<KeyValue> _values;

        public KeyValueService(IUnitOfWork<TDbContext> uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _values = _uow.Set<KeyValue>();
        }

        public async Task SaveValueAsync(string key, string value)
        {
            var record = await _values.SingleOrDefaultAsync(v => v.Key == key).ConfigureAwait(false);
            if (record == null)
            {
                _values.Add(new KeyValue
                {
                    Key = key,
                    Value = value
                });
            }
            else
            {
                record.Value = value;
            }

            await _uow.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<Maybe<string>> FindValueAsync(string key)
        {
            var record = await _values.SingleOrDefaultAsync(v => v.Key == key).ConfigureAwait(false);
            return record == null ? Maybe<string>.None : record.Value;
        }
    }
}