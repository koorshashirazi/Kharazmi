using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Kharazmi.AspNetCore.Web.Mvc
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumHelper
    {
        /// <summary>
        /// Convert Enum Type To <see cref="T:Microsoft.AspNetCore.Mvc.Rendering.SelectList" />
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<SelectListItem> ToSelectListItem<T>() where T : struct, IComparable
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => new SelectListItem(x.ToString(), Convert.ToInt16(x).ToString())).ToList();
        }
    }
}