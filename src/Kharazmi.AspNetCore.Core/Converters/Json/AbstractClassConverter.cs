using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kharazmi.AspNetCore.Core.Converters.Json
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TAbstract"></typeparam>
    public class AbstractClassConverter<TAbstract> : DefaultContractResolver
        where TAbstract : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(TAbstract).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null; // pretend TableSortRuleConvert is not specified (thus avoiding a stack overflow)
            return base.ResolveContractConverter(objectType);
        }
    }
}