using System;

namespace Kharazmi.AspNetCore.Core.Pipelines
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PipelineBehaviorAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="behaviorTypes"></param>
        public PipelineBehaviorAttribute(params Type[] behaviorTypes)
        {
            BehaviorTypes = behaviorTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        public Type[] BehaviorTypes { get; }
    }
}