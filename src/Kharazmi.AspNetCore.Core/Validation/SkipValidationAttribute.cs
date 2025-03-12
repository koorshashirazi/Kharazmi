using System;

namespace Kharazmi.AspNetCore.Core.Validation
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class )]
    public sealed class SkipValidationAttribute : Attribute
    {
    }
}