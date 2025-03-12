using System;

 namespace Kharazmi.AspNetCore.Core.Domain.Events
{
    /// <summary> </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class EventAttribute : Attribute
    {
        /// <summary> </summary>
        public string Name { get; }

        /// <summary> </summary>
        /// <param name="name"></param>
        public EventAttribute(string name)
        {
            Name = name;
        }
    }
}