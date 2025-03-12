﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Kharazmi.AspNetCore.Web.Middlewares
{
    public sealed class SpaRoutingMiddleware
    {
        private readonly RequestDelegate _next;

        public SpaRoutingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context).ConfigureAwait(false);
            var requestPath = context.Request.Path.Value;
            if (requestPath != null &&
                context.Response.StatusCode == 404 &&
                !Path.HasExtension(requestPath) &&
                !requestPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                context.Request.Path = "/index.html";
                //context.Request.Path = "/"; // since we are using views/shared/_layout.cshtml now.
                context.Response.StatusCode = 200;
                await _next(context).ConfigureAwait(false);
            }
        }
    }
}