using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Kharazmi.AspNetCore.Core.ReflectionToolkit
{
    /// <summary>
    /// Fast property access, using Reflection.Emit.
    /// </summary>
    public class FastReflection
    {
        /// <summary>
        /// It's a lazy loaded thread-safe singleton.
        /// </summary>
        private static readonly Lazy<FastReflection> LazyInstance =
            new(() => new FastReflection(), LazyThreadSafetyMode.ExecutionAndPublication);

        // 'GetOrAdd' call on the dictionary is not thread safe and we might end up creating the GetterInfo more than
        // once. To prevent this Lazy<> is used. In the worst case multiple Lazy<> objects are created for multiple
        // threads but only one of the objects succeeds in creating a GetterInfo.
        private readonly ConcurrentDictionary<Type, Lazy<List<PropertyGetterInfo>>> _gettersCache = new();

        private FastReflection()
        {
        }

        /// <summary>
        /// Singleton instance of FastReflection.
        /// </summary>
        public static FastReflection Instance { get; } = LazyInstance.Value;

        /// <summary>
        /// Fast property access, using Reflection.Emit.
        /// </summary>
        public IList<PropertyGetterInfo> GetGetterDelegates(Type type)
        {
            var getterDelegates = _gettersCache.GetOrAdd(type, propType => new Lazy<List<PropertyGetterInfo>>(
                () =>
                {
                    var gettersList = new List<PropertyGetterInfo>();
                    var properties = propType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    int index = 0;
                    foreach (var property in properties)
                    {
                        var getterDelegate = CreateGetterPropertyDelegate(propType, property, index);
                        index++;

                        if (getterDelegate == null)
                            continue;

                        var info = new PropertyGetterInfo
                        {
                            Name = property.Name,
                            GetterFunc = getterDelegate,
                            PropertyType = property.PropertyType,
                            MemberInfo = property
                        };
                        gettersList.Add(info);
                    }

                    var fields = propType.GetFields(BindingFlags.Instance | BindingFlags.Public);
                    foreach (var field in fields)
                    {
                        var getterDelegate = CreateGetterFieldDelegate(propType, field);
                        if (getterDelegate == null)
                            continue;

                        var info = new PropertyGetterInfo
                        {
                            Name = field.Name,
                            GetterFunc = getterDelegate,
                            PropertyType = field.FieldType,
                            MemberInfo = field
                        };
                        gettersList.Add(info);
                    }

                    return gettersList;
                }));
            return getterDelegates.Value;
        }

        private static Func<object, object> CreateGetterFieldDelegate(Type type, FieldInfo fieldInfo)
        {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var field = Expression.Field(Expression.TypeAs(instanceParam, type), fieldInfo);
            var convertField = Expression.TypeAs(field, typeof(object));
            return Expression.Lambda<Func<object, object>>(convertField, instanceParam).Compile();
        }

        private static Func<object, object> CreateGetterPropertyDelegate(Type type, PropertyInfo propertyInfo,
            int index)
        {
#if NET40
            var getMethod = propertyInfo.GetGetMethod();
#else
            var getMethod = propertyInfo.GetMethod;
#endif
            if (getMethod == null)
                throw new InvalidOperationException($"Couldn't get the GetMethod of {type}");

            var hasParameter = getMethod.GetParameters().Length != 0;

            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var getterExpression = Expression.Convert(
                hasParameter
                    ? Expression.Call(Expression.Convert(instanceParam, type), getMethod, Expression.Constant(index))
                    : Expression.Call(Expression.Convert(instanceParam, type), getMethod)
                , typeof(object));
            return Expression.Lambda<Func<object, object>>(getterExpression, instanceParam).Compile();
        }
    }
    
    /// <summary>
    /// Getter method's info.
    /// </summary>
    public class PropertyGetterInfo
    {
        /// <summary>
        /// Property/Field's Getter method.
        /// </summary>
        public Func<object, object> GetterFunc { set; get; }

        /// <summary>
        /// Obtains information about the attributes of a member and provides access.
        /// </summary>
        public MemberInfo MemberInfo { set; get; }

        /// <summary>
        /// Property/Field's name.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Property/Field's Type.
        /// </summary>
        public Type PropertyType { set; get; }
    }
}