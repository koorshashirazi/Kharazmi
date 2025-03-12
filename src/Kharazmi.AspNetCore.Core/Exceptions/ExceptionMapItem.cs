using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kharazmi.AspNetCore.Core.Exceptions
{
    /// <summary>
    /// 
    /// </summary>
    public class ExceptionMapItem
    {
        public ISet<string> Keywords { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        [Required] public string Message { get; set; }
        public string MemberName { get; set; }
    }
}