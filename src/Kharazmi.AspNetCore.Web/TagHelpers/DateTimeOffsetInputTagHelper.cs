using System;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Kharazmi.AspNetCore.Web.TagHelpers
{
    [HtmlTargetElement("input", Attributes = ForAttributeName, TagStructure = TagStructure.WithoutEndTag)]
    public class DateTimeOffsetInputTagHelper : TagHelper
    {
        private const string ForAttributeName = "asp-for";
        public override int Order => int.MinValue;


        [HtmlAttributeName("type")] public string InputTypeName { get; set; }

        [HtmlAttributeName("value")] public string Value { get; set; }

        [HtmlAttributeName(ForAttributeName)] public ModelExpression For { get; set; }

        [HtmlAttributeNotBound] [ViewContext] public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (For.Metadata.UnderlyingOrModelType == typeof(DateTimeOffset))
            {
                var fullName = ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(For.Name);

                var dateValue = (DateTimeOffset?) For.Model;

                var offset = dateValue.HasValue ? Convert.ToInt32(dateValue.Value.Offset.TotalMinutes) : 0;

                if (InputTypeName.IsEmpty())
                    InputTypeName = "datetime-local";

                if (!output.Attributes.ContainsName("type"))
                    output.Attributes.Add("type", InputTypeName);

                if (Value.IsEmpty())
                    Value = dateValue.HasValue
                        ? $"{dateValue.Value.Year:00}-{dateValue.Value.Month:00}-{dateValue.Value.Day:00}T{dateValue.Value.Hour:00}:{dateValue.Value.Minute:00}:{dateValue.Value.Second:00}"
                        : string.Empty;

                if (!output.Attributes.ContainsName("value"))
                    output.Attributes.Add("value", Value);

                output.Attributes.Add("data-has-offset", "true");

                fullName = fullName.Length > 0 ? fullName + ".O" : "O";
                output.PostElement.AppendHtml("<input name='" +
                                              fullName + "' type='hidden' value='" + offset + "' />");
            }
        }
    }
}