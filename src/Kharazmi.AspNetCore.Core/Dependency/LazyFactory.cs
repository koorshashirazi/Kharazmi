using System;
using Microsoft.Extensions.DependencyInjection;

namespace Kharazmi.AspNetCore.Core.Dependency
{
    public sealed class LazyFactory<T> : Lazy<T> where T : class
    {
        public LazyFactory(IServiceProvider provider)
            : base(provider.GetRequiredService<T>)
        {
        }
    }
}