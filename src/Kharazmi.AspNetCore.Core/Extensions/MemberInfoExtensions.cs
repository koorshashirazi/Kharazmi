using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        /// <summary>
        /// Returns DisplayFormatAttribute data.
        /// </summary>
        /// <param name="info">Property metadata info</param>
        /// <returns>NullDisplayText</returns>
        public static string GetNullDisplayTextAttribute(this MemberInfo info)
        {
            var displayFormat = info.GetCustomAttributes(true).OfType<DisplayFormatAttribute>().FirstOrDefault();
            return displayFormat == null ? string.Empty : displayFormat.NullDisplayText;
        }
    }
}