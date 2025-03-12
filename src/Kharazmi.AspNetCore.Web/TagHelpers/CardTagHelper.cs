using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kharazmi.AspNetCore.Web.TagHelpers
{
    [HtmlTargetElement("card")]
    public class CardTagHelper : TagHelper
    {
        [HtmlAttributeName("header-text")] public string Header { get; set; }
        [HtmlAttributeName("header-class")] public string HeaderClass { get; set; }
        [HtmlAttributeName("title")] public string Title { get; set; }
        [HtmlAttributeName("title-icon")] public string TitleIcon { get; set; }
        [HtmlAttributeName("body-content-class")] public string BodyContentClass { get; set; }
        [HtmlAttributeName("body-class")] public string BodyClass { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "card", StringComparison.OrdinalIgnoreCase))
            {
                output.TagMode = TagMode.StartTagAndEndTag;
                output.TagName = "div";

                var content = await output.GetChildContentAsync().ConfigureAwait(false);

                var htmlcontent = string.Empty;
                if (!string.IsNullOrWhiteSpace(Header))
                    htmlcontent += $"<div class=\"card-header {HeaderClass}\">{Header}</div>";

                htmlcontent +=
                    $"<div class=\"card {BodyContentClass}\"><div class=\"card-body {BodyClass}\">";

                if (!string.IsNullOrWhiteSpace(Title))
                {
                    if (!string.IsNullOrWhiteSpace(TitleIcon))
                        htmlcontent +=
                            $"<h4 class=\"card-title\"><i class=\"{TitleIcon}\"></i>{Title}</h4>";
                    else
                        htmlcontent += $"<h4 class=\"card-title\">{Title}</h4>";
                }

                htmlcontent += $"{content.GetContent()}</div></div>";

                output.Content.SetHtmlContent(htmlcontent);
                await Task.CompletedTask.ConfigureAwait(false);
            }
            else
            {
                await base.ProcessAsync(context, output).ConfigureAwait(false);
            }
        }
    }
}