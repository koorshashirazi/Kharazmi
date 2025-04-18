using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ICustomTypeDescriptor td)
            where TAttribute : Attribute
        {
            var attributes = td.GetAttributes().OfType<TAttribute>();
            return attributes;
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this PropertyDescriptor pd)
            where TAttribute : Attribute
        {
            var attributes = pd.Attributes.OfType<TAttribute>();
            return attributes;
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this PropertyDescriptor pd,
            Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            Ensure.ArgumentIsNotNull(predicate, nameof(predicate));

            var attributes = pd.Attributes.OfType<TAttribute>().Where(predicate);
            return attributes;
        }

        public static PropertyDescriptor GetProperty(this ICustomTypeDescriptor td, string name)
        {
            Ensure.ArgumentIsNotEmpty(name, nameof(name));
            return td.GetProperties().Find(name, true);
            //.Cast<PropertyDescriptor>()
            //.FirstOrDefault(p => p.AggregateType.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public static IEnumerable<PropertyDescriptor> GetPropertiesWith<TAttribute>(this ICustomTypeDescriptor td)
            where TAttribute : Attribute
        {
            return td.GetPropertiesWith<TAttribute>(x => true);
        }

        public static IEnumerable<PropertyDescriptor> GetPropertiesWith<TAttribute>(
            this ICustomTypeDescriptor td,
            Func<TAttribute, bool> predicate)
            where TAttribute : Attribute
        {
            Ensure.ArgumentIsNotNull(predicate, nameof(predicate));

            return td.GetProperties()
                .Cast<PropertyDescriptor>()
                .Where(p => p.GetAttributes<TAttribute>().Any(predicate));

            //Expression<Func<TAttribute, bool>> expression = (TAttribute a) => typeof(TAttribute).IsAssignableFrom(a.GetType());

            //return td.GetProperties()
            //        .Cast<PropertyDescriptor>()
            //        .Where(p => p.Attributes.Cast<TAttribute>()
            //            .Any(expression.And(predicate).Compile()));

            //var query = from p in this.EntityTypeDescriptor.GetProperties().Cast<PropertyDescriptor>()
            //            let attributes = p.Attributes.Cast<TAttribute>()
            //            where attributes.Any(a => typeof(TAttribute).IsAssignableFrom(a.GetType()))
            //            //where attributes.Any(a => typeof(TAttribute).IsAssignableFrom(a.GetType()))
            //            select p;

            //return query 
            //        .Select<PropertyDescriptor, EntityProperty>(p => new EntityProperty(p, this))
            //        .AsReadOnly();
        }
    }
}