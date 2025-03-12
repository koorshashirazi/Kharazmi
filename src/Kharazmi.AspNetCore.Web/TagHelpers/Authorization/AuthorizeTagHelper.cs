using System;
using Kharazmi.AspNetCore.Core.Runtime;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kharazmi.AspNetCore.Web.TagHelpers.Authorization
{
    [HtmlTargetElement(Attributes = AuthorizeAttributeName)]
    [HtmlTargetElement(Attributes = PermissionAttributeName)]
    public class AuthorizeTagHelper : TagHelper
    {
        private const string PermissionAttributeName = "asp-permission";
        private const string AuthorizeAttributeName = "asp-authorize";
        private readonly IUserSession _session;
        public override int Order => -1000;

        [HtmlAttributeName(PermissionAttributeName)]
        public string PermissionName { get; set; }

        public AuthorizeTagHelper(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (!_session.IsAuthenticated || (!string.IsNullOrEmpty(PermissionName) && !_session.IsGranted(PermissionName)))
            {
                output.SuppressOutput();
            }
        }
    }
}