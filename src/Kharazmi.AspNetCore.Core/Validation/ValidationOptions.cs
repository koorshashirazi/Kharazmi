using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Threading;

namespace Kharazmi.AspNetCore.Core.Validation
{
    /// <summary>
    /// 
    /// </summary>
    public class ValidationOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public ISet<Type> IgnoredTypes { get; } = new HashSet<Type> { typeof(Type), typeof(Stream), typeof(Expression), typeof(CancellationToken) };
    }
}