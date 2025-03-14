using System;

 namespace Kharazmi.AspNetCore.Core.Common
{
    /// <summary>
    /// Used to represent a named type selector.
    /// </summary>
    public class NamedTypeSelector
    {
        /// <summary>
        /// AggregateType of the selector.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Predicate.
        /// </summary>
        public Func<Type, bool> Predicate { get; set; }

        /// <summary>
        /// Creates new <see cref="NamedTypeSelector"/> object.
        /// </summary>
        /// <param name="name">AggregateType</param>
        /// <param name="predicate">Predicate</param>
        public NamedTypeSelector(string name, Func<Type, bool> predicate)
        {
            Name = name;
            Predicate = predicate;
        }
    }
}