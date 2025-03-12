using System;

namespace Kharazmi.AspNetCore.Core.Common
{

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Parameter)]
    public class SkipNormalizationAttribute : Attribute
    {
    }
}