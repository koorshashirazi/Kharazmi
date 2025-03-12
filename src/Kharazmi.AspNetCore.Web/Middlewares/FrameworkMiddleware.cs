using System;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.Middlewares
{
    public class FrameworkMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _provider;

        public FrameworkMiddleware(RequestDelegate next, IServiceProvider provider)
        {
            _next = next;
            _provider = provider;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await Bootstrapper.RunOnBeginRequest(_provider).ConfigureAwait(false);

                await _next(context).ConfigureAwait(false);

                await Bootstrapper.RunOnEndRequest(_provider).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                await Bootstrapper.RunOnError(_provider, e).ConfigureAwait(false);
                
                throw;
            }
        }
    }
}