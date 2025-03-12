using System.Threading.Tasks;
using Kharazmi.AspNetCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Kharazmi.AspNetCore.Web.Results
{
    /// <summary>
    /// An AJAX aware version of <see cref="Microsoft.AspNetCore.Mvc.RedirectResult"/>
    /// </summary>
    public class AjaxAwareRedirectResult : RedirectResult
    {
        public AjaxAwareRedirectResult(string url, bool permanent = false) : base(url, permanent)
        {
        }
        
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                await new JavaScriptRedirectResult(UrlHelper.Content(Url)).ExecuteResultAsync(context).ConfigureAwait(false);
            }
            else
            {
                await base.ExecuteResultAsync(context).ConfigureAwait(false);
            }
        }
    }
}