using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Drawing;
using Kharazmi.AspNetCore.Web.Caching;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.ActionResults
{
    /// <summary>
    /// An ASP.NET Core text to image renderer.
    /// </summary>
    public class TextToImageResult : ActionResult
    {
        private readonly TextToImageOptions _options;
        private readonly string _text;

        /// <summary>
        /// An ASP.NET Core text to image renderer.
        /// </summary>
        public TextToImageResult(string text, TextToImageOptions options)
        {
            _options = options;
            _text = text;
        }

        /// <summary>
        /// Executes the result operation of the action method asynchronously.
        /// </summary>
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            await WriteToResponse(context).ConfigureAwait(false);
        }

        private async Task WriteToResponse(ActionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = new MediaTypeHeaderValue("image/png").ToString();
            context.HttpContext.DisableBrowserCache();
            var data = _text.TextToImage(_options);
            await response.Body.WriteAsync(data, 0, data.Length).ConfigureAwait(false);
        }
    }
}