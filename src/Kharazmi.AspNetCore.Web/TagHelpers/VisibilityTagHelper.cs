using System.Threading.Tasks;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kharazmi.AspNetCore.Web.TagHelpers
{
    [HtmlTargetElement("li")]
    [HtmlTargetElement("ul")]
    [HtmlTargetElement("div")]
    [HtmlTargetElement("nav")]
    public class VisibilityTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-is-visible")] public bool IsVisible { get; set; } = true;

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            context.CheckArgumentIsNull(nameof(context));
            output.CheckArgumentIsNull(nameof(output));

            if (!IsVisible) output.SuppressOutput();

            return base.ProcessAsync(context, output);
        }
    }
}