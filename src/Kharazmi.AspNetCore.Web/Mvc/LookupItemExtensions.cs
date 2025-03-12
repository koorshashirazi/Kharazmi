using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Application.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kharazmi.AspNetCore.Web.Mvc
{
    public static class LookupItemExtensions
    {
        public static IReadOnlyList<SelectListItem> ToSelectListItem<TValue>(this IEnumerable<LookupItem<TValue>> items)
        {
            return items.Select(i => new SelectListItem { Value = i.Value.ToString(), Text = i.Text, Selected = i.Selected }).ToList();
        }
    }
}