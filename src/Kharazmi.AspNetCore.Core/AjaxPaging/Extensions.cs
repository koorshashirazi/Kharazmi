using System.Collections.Generic;
using System.Text;

namespace Kharazmi.AspNetCore.Core.AjaxPaging
{
    public static class Extensions
    {
        public static List<string> ToStringList(this char[] chars)
        {
            var list = new List<string>();
            foreach (var c in chars) list.Add(c.ToString());

            return list;
        }

        public static string ToCsv(this string[] arr)
        {
            var sb = new StringBuilder();
            var comma = string.Empty;
            foreach (var s in arr)
            {
                sb.Append(comma);
                sb.Append(s);
                comma = ",";
            }

            return sb.ToString();
            //return arr.
        }
    }
}