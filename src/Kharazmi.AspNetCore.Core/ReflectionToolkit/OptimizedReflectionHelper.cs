using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace Kharazmi.AspNetCore.Core.ReflectionToolkit;

public static class OptimizedReflectionHelper
{
    private static readonly Dictionary<string, object> Cache = [];

    #region Type Utilities

    /// <summary>
    /// Searching for types in an assembly by name pattern
    /// </summary>
    public static IEnumerable<Type> FindTypes(Assembly assembly, string? pattern = null)
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));
        var types = assembly.GetTypes();

        if (string.IsNullOrEmpty(pattern))
            return types;

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return types.Where(t => regex.IsMatch(t.FullName));
    }

    /// <summary>
    /// Find a type by full or partial name
    /// </summary>
    public static Type FindType(string typeName, bool searchAllAssemblies = false)
    {
        var cacheKey = $"Type_{typeName}_{searchAllAssemblies}";

        if (Cache.TryGetValue(cacheKey, out var cachedType))
            return (Type)cachedType;

        Type result = null;

        // First we search in the calling assembly
        result = Assembly.GetCallingAssembly().GetTypes()
            .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                                 t.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase));

        if (result == null && searchAllAssemblies)
        {
            // Search all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                result = assembly.GetTypes()
                    .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase) ||
                                         t.FullName.Equals(typeName, StringComparison.OrdinalIgnoreCase));

                if (result != null)
                    break;
            }
        }

        if (result != null)
            Cache[cacheKey] = result;

        return result;
    }

    #endregion

    #region Method Utilities

    /// <summary>
    /// Search for methods based on name pattern
    /// </summary>
    public static IEnumerable<MethodInfo> FindMethods(Type type, string pattern = null,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        var methods = type.GetMethods(bindingFlags);

        if (string.IsNullOrEmpty(pattern))
            return methods;

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return methods.Where(m => regex.IsMatch(m.Name));
    }

    /// <summary>
    /// Create a delegate for quick method invocation using Expression Trees
    /// </summary>
    public static T CreateMethodDelegate<T>(MethodInfo methodInfo) where T : Delegate
    {
        var cacheKey = $"MethodDelegate_{methodInfo.DeclaringType.FullName}_{methodInfo.Name}_{typeof(T).FullName}";

        if (Cache.TryGetValue(cacheKey, out var cachedDelegate))
            return (T)cachedDelegate;

        var parameters = methodInfo.GetParameters();
        var paramExpressions = new ParameterExpression[parameters.Length + 1];

        paramExpressions[0] = Expression.Parameter(methodInfo.DeclaringType, "instance");

        for (int i = 0; i < parameters.Length; i++)
        {
            paramExpressions[i + 1] = Expression.Parameter(parameters[i].ParameterType, $"param{i}");
        }

        var instanceExpression = paramExpressions[0];
        var argumentsExpressions = paramExpressions.Skip(1).Take(parameters.Length).ToArray();

        Expression callExpression;
        if (methodInfo.IsStatic)
        {
            callExpression = Expression.Call(methodInfo, argumentsExpressions);
        }
        else
        {
            callExpression = Expression.Call(instanceExpression, methodInfo, argumentsExpressions);
        }

        var lambdaExpression = Expression.Lambda(typeof(T), callExpression, paramExpressions);

        var result = (T)lambdaExpression.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Create a delegate to call a parameterless method generically
    /// </summary>
    public static Func<TInstance, TResult> CreateFastMethodCall<TInstance, TResult>(MethodInfo methodInfo)
    {
        var cacheKey = $"FastMethod_{methodInfo.DeclaringType.FullName}_{methodInfo.Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedFunc))
            return (Func<TInstance, TResult>)cachedFunc;

        var instanceParam = Expression.Parameter(typeof(TInstance), "instance");

        var callExpression = Expression.Call(
            Expression.Convert(instanceParam, methodInfo.DeclaringType),
            methodInfo);

        var lambdaExpression = Expression.Lambda<Func<TInstance, TResult>>(
            Expression.Convert(callExpression, typeof(TResult)),
            instanceParam);

        var result = lambdaExpression.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Create a delegate to call a method with a parameter in a generic way
    /// </summary>
    public static Func<TInstance, TParam, TResult> CreateFastMethodCall<TInstance, TParam, TResult>(
        MethodInfo methodInfo)
    {
        var cacheKey = $"FastMethod_{methodInfo.DeclaringType.FullName}_{methodInfo.Name}_{typeof(TParam).Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedFunc))
            return (Func<TInstance, TParam, TResult>)cachedFunc;

        var instanceParam = Expression.Parameter(typeof(TInstance), "instance");
        var methodParam = Expression.Parameter(typeof(TParam), "param");

        var callExpression = Expression.Call(
            Expression.Convert(instanceParam, methodInfo.DeclaringType),
            methodInfo,
            Expression.Convert(methodParam, methodInfo.GetParameters()[0].ParameterType));

        var lambdaExpression = Expression.Lambda<Func<TInstance, TParam, TResult>>(
            Expression.Convert(callExpression, typeof(TResult)),
            instanceParam,
            methodParam);

        var result = lambdaExpression.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Public delegate constructor for method invocation
    /// </summary>
    public static Delegate CreateFastInvoker(MethodInfo methodInfo)
    {
        var parameters = methodInfo.GetParameters();
        var paramTypes = new Type[parameters.Length + 1];

        paramTypes[0] = methodInfo.DeclaringType;
        for (int i = 0; i < parameters.Length; i++)
        {
            paramTypes[i + 1] = parameters[i].ParameterType;
        }

        var delegateType = Expression.GetDelegateType(
            [.. paramTypes, .. new[] { methodInfo.ReturnType }]);

        var paramExpressions = new ParameterExpression[paramTypes.Length];
        for (int i = 0; i < paramTypes.Length; i++)
        {
            paramExpressions[i] = Expression.Parameter(paramTypes[i], $"param{i}");
        }

        var instanceExpression = paramExpressions[0];
        var argumentsExpressions = paramExpressions.Skip(1).ToArray();

        Expression callExpression = Expression.Call(instanceExpression, methodInfo, argumentsExpressions);

        var lambdaExpression = Expression.Lambda(delegateType, callExpression, paramExpressions);

        return lambdaExpression.Compile();
    }

    #endregion

    #region Property Utilities

    /// <summary>
    /// Creating a fast property getter using Expression Trees
    /// </summary>
    public static Func<object, object> CreatePropertyGetter(PropertyInfo propertyInfo)
    {
        var cacheKey = $"PropertyGetter_{propertyInfo.DeclaringType.FullName}_{propertyInfo.Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedGetter))
            return (Func<object, object>)cachedGetter;

        var instanceParam = Expression.Parameter(typeof(object), "instance");

        var instanceCast = Expression.Convert(instanceParam, propertyInfo.DeclaringType);

        var propertyAccess = Expression.Property(instanceCast, propertyInfo);

        var resultCast = Expression.Convert(propertyAccess, typeof(object));

        var lambda = Expression.Lambda<Func<object, object>>(resultCast, instanceParam);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Create a quick getter for a property with a given type
    /// </summary>
    public static Func<TInstance, TProperty> CreatePropertyGetter<TInstance, TProperty>(PropertyInfo propertyInfo)
    {
        var cacheKey =
            $"PropertyGetter_{propertyInfo.DeclaringType.FullName}_{propertyInfo.Name}_{typeof(TInstance).Name}_{typeof(TProperty).Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedGetter))
            return (Func<TInstance, TProperty>)cachedGetter;

        var instanceParam = Expression.Parameter(typeof(TInstance), "instance");

        Expression instanceExpr = instanceParam;
        if (propertyInfo.DeclaringType != typeof(TInstance))
        {
            instanceExpr = Expression.Convert(instanceParam, propertyInfo.DeclaringType);
        }

        var propertyAccess = Expression.Property(instanceExpr, propertyInfo);

        Expression resultExpr = propertyAccess;
        if (propertyInfo.PropertyType != typeof(TProperty))
        {
            resultExpr = Expression.Convert(propertyAccess, typeof(TProperty));
        }

        var lambda = Expression.Lambda<Func<TInstance, TProperty>>(resultExpr, instanceParam);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Creating a quick setter for a property using Expression Trees
    /// </summary>
    public static Action<object, object> CreatePropertySetter(PropertyInfo propertyInfo)
    {
        var cacheKey = $"PropertySetter_{propertyInfo.DeclaringType.FullName}_{propertyInfo.Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedSetter))
            return (Action<object, object>)cachedSetter;

        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");

        var instanceCast = Expression.Convert(instanceParam, propertyInfo.DeclaringType);

        var valueCast = Expression.Convert(valueParam, propertyInfo.PropertyType);

        var propertyAccess = Expression.Property(instanceCast, propertyInfo);
        var assignExpression = Expression.Assign(propertyAccess, valueCast);

        var lambda = Expression.Lambda<Action<object, object>>(assignExpression, instanceParam, valueParam);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Create a quick setter for a property with a specific type
    /// </summary>
    public static Action<TInstance, TProperty> CreatePropertySetter<TInstance, TProperty>(PropertyInfo propertyInfo)
    {
        var cacheKey =
            $"PropertySetter_{propertyInfo.DeclaringType.FullName}_{propertyInfo.Name}_{typeof(TInstance).Name}_{typeof(TProperty).Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedSetter))
            return (Action<TInstance, TProperty>)cachedSetter;

        var instanceParam = Expression.Parameter(typeof(TInstance), "instance");
        var valueParam = Expression.Parameter(typeof(TProperty), "value");

        Expression instanceExpr = instanceParam;
        if (propertyInfo.DeclaringType != typeof(TInstance))
        {
            instanceExpr = Expression.Convert(instanceParam, propertyInfo.DeclaringType);
        }

        Expression valueExpr = valueParam;
        if (propertyInfo.PropertyType != typeof(TProperty))
        {
            valueExpr = Expression.Convert(valueParam, propertyInfo.PropertyType);
        }

        var propertyAccess = Expression.Property(instanceExpr, propertyInfo);
        var assignExpression = Expression.Assign(propertyAccess, valueExpr);

        var lambda = Expression.Lambda<Action<TInstance, TProperty>>(assignExpression, instanceParam, valueParam);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    #endregion

    #region Constructor Utilities

    /// <summary>
    /// Find all constructors of a type with filter support
    /// </summary>
    public static IEnumerable<ConstructorInfo> FindConstructors(Type type,
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
    {
        return type.GetConstructors(bindingFlags);
    }

    /// <summary>
    /// Fast instance creation using Expression Trees
    /// </summary>
    public static Func<object[], object> CreateActivator(ConstructorInfo constructorInfo)
    {
        var cacheKey =
            $"Constructor_{constructorInfo.DeclaringType.FullName}_{string.Join("_", constructorInfo.GetParameters().Select(p => p.ParameterType.FullName))}";

        if (Cache.TryGetValue(cacheKey, out var cachedActivator))
            return (Func<object[], object>)cachedActivator;

        var paramsArrayParam = Expression.Parameter(typeof(object[]), "parameters");

        var parameters = constructorInfo.GetParameters();
        var paramExpressions = new Expression[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            var paramType = parameters[i].ParameterType;
            var paramAccessExpression = Expression.ArrayIndex(paramsArrayParam, Expression.Constant(i));
            paramExpressions[i] = Expression.Convert(paramAccessExpression, paramType);
        }

        var newExpression = Expression.New(constructorInfo, paramExpressions);

        Expression resultExpression = constructorInfo.DeclaringType.IsValueType
            ? Expression.Convert(newExpression, typeof(object))
            : (Expression)newExpression;

        var lambda = Expression.Lambda<Func<object[], object>>(resultExpression, paramsArrayParam);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Fast instance constructor with no parameters
    /// </summary>
    public static Func<object> CreateActivator(Type type)
    {
        var cacheKey = $"Constructor_{type.FullName}_NoParams";

        if (Cache.TryGetValue(cacheKey, out var cachedActivator))
            return (Func<object>)cachedActivator;

        var constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor == null)
            throw new MissingMethodException($"Type {type.FullName} does not have a parameterless constructor");

        var newExpression = Expression.New(constructor);

        Expression resultExpression = type.IsValueType
            ? Expression.Convert(newExpression, typeof(object))
            : (Expression)newExpression;

        var lambda = Expression.Lambda<Func<object>>(resultExpression);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Fast generic instance creator
    /// </summary>
    public static Func<T> CreateActivator<T>() where T : new()
    {
        var cacheKey = $"Constructor_{typeof(T).FullName}_Generic";

        if (Cache.TryGetValue(cacheKey, out var cachedActivator))
            return (Func<T>)cachedActivator;

        var newExpression = Expression.New(typeof(T));

        var lambda = Expression.Lambda<Func<T>>(newExpression);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    #endregion

    #region Field Utilities

    /// <summary>
    /// Create a quick getter for a field using Expression Trees
    /// </summary>
    public static Func<object, object> CreateFieldGetter(FieldInfo fieldInfo)
    {
        if (fieldInfo == null) throw new ArgumentNullException(nameof(fieldInfo));
        var cacheKey = $"FieldGetter_{fieldInfo.DeclaringType?.FullName}_{fieldInfo.Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedGetter))
            return (Func<object, object>)cachedGetter;

        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var instanceCast = Expression.Convert(instanceParam, fieldInfo.DeclaringType);
        var fieldAccess = Expression.Field(instanceCast, fieldInfo);
        var resultCast = Expression.Convert(fieldAccess, typeof(object));
        var lambda = Expression.Lambda<Func<object, object>>(resultCast, instanceParam);

        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Create a quick getter for a field with a specific type
    /// </summary>
    public static Func<TInstance, TField> CreateFieldGetter<TInstance, TField>(FieldInfo fieldInfo)
    {
        if (fieldInfo == null) throw new ArgumentNullException(nameof(fieldInfo));
        var cacheKey =
            $"FieldGetter_{fieldInfo.DeclaringType?.FullName}_{fieldInfo.Name}_{typeof(TInstance).Name}_{typeof(TField).Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedGetter))
            return (Func<TInstance, TField>)cachedGetter;

        var instanceParam = Expression.Parameter(typeof(TInstance), "instance");

        Expression instanceExpr = instanceParam;
        if (fieldInfo.DeclaringType != typeof(TInstance))
        {
            instanceExpr = Expression.Convert(instanceParam, fieldInfo.DeclaringType);
        }

        var fieldAccess = Expression.Field(instanceExpr, fieldInfo);

        Expression resultExpr = fieldAccess;
        if (fieldInfo.FieldType != typeof(TField))
        {
            resultExpr = Expression.Convert(fieldAccess, typeof(TField));
        }

        var lambda = Expression.Lambda<Func<TInstance, TField>>(resultExpr, instanceParam);
        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Create a quick setter for a field using Expression Trees
    /// </summary>
    public static Action<object, object> CreateFieldSetter(FieldInfo fieldInfo)
    {
        if (fieldInfo == null) throw new ArgumentNullException(nameof(fieldInfo));
        var cacheKey = $"FieldSetter_{fieldInfo.DeclaringType?.FullName}_{fieldInfo.Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedSetter))
            return (Action<object, object>)cachedSetter;

        if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
            throw new ArgumentException($"Field {fieldInfo.Name} is readonly and cannot be modified");

        var instanceParam = Expression.Parameter(typeof(object), "instance");
        var valueParam = Expression.Parameter(typeof(object), "value");

        var instanceCast = Expression.Convert(instanceParam, fieldInfo.DeclaringType);
        var valueCast = Expression.Convert(valueParam, fieldInfo.FieldType);
        var fieldAccess = Expression.Field(instanceCast, fieldInfo);
        var assignExpression = Expression.Assign(fieldAccess, valueCast);
        var lambda = Expression.Lambda<Action<object, object>>(assignExpression, instanceParam, valueParam);
        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    /// <summary>
    /// Create a quick setter for a field with a specific type
    /// </summary>
    public static Action<TInstance, TField> CreateFieldSetter<TInstance, TField>(FieldInfo fieldInfo)
    {
        if (fieldInfo == null) throw new ArgumentNullException(nameof(fieldInfo));
        var cacheKey =
            $"FieldSetter_{fieldInfo.DeclaringType?.FullName}_{fieldInfo.Name}_{typeof(TInstance).Name}_{typeof(TField).Name}";

        if (Cache.TryGetValue(cacheKey, out var cachedSetter))
            return (Action<TInstance, TField>)cachedSetter;

        if (fieldInfo.IsInitOnly || fieldInfo.IsLiteral)
            throw new ArgumentException($"Field {fieldInfo.Name} is readonly and cannot be modified");

        var instanceParam = Expression.Parameter(typeof(TInstance), "instance");
        var valueParam = Expression.Parameter(typeof(TField), "value");

        Expression instanceExpr = instanceParam;
        if (fieldInfo.DeclaringType != typeof(TInstance))
        {
            instanceExpr = Expression.Convert(instanceParam, fieldInfo.DeclaringType);
        }

        Expression valueExpr = valueParam;
        if (fieldInfo.FieldType != typeof(TField))
        {
            valueExpr = Expression.Convert(valueParam, fieldInfo.FieldType);
        }

        var fieldAccess = Expression.Field(instanceExpr, fieldInfo);
        var assignExpression = Expression.Assign(fieldAccess, valueExpr);
        var lambda = Expression.Lambda<Action<TInstance, TField>>(assignExpression, instanceParam, valueParam);
        var result = lambda.Compile();

        Cache[cacheKey] = result;

        return result;
    }

    #endregion

    #region Advanced Utilities

    /// <summary>
    /// Method call with support for ref/out parameters
    /// </summary>
    public static object InvokeMethodWithRefParams(MethodInfo methodInfo, object instance, object[] parameters)
    {
        var methodParams = methodInfo.GetParameters();

        for (int i = 0; i < methodParams.Length; i++)
        {
            if (methodParams[i].IsOut && (parameters[i] == null || parameters[i] == DBNull.Value))
            {
                parameters[i] = methodParams[i].ParameterType.IsByRef
                    ? Activator.CreateInstance(methodParams[i].ParameterType.GetElementType())
                    : Activator.CreateInstance(methodParams[i].ParameterType);
            }
        }

        return methodInfo.Invoke(instance, parameters);
    }

    /// <summary>
    /// Advanced Reflection Metadata Search Using LINQ
    /// </summary>
    public static IEnumerable<MemberInfo> SearchMetadata(Type type, string pattern,
        MemberTypes memberTypes = MemberTypes.All)
    {
        var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        var members = type.GetMembers(bindingFlags).Where(m => (m.MemberType & memberTypes) != 0);

        if (string.IsNullOrEmpty(pattern))
            return members;

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return members.Where(m => regex.IsMatch(m.Name));
    }

    /// <summary>
    /// Freeing cache
    /// </summary>
    public static void ClearCache()
    {
        Cache.Clear();
    }

    #endregion
}

/// <summary>
/// Extension class to provide extended methods
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Extension for easy method invocation
    /// </summary>
    public static object InvokeMethod(this object instance, string methodName, params object[] parameters)
    {
        var type = instance.GetType();
        var method = type.GetMethod(methodName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
            throw new MissingMethodException(type.FullName, methodName);

        return method.Invoke(instance, parameters);
    }

    /// <summary>
    /// Extension for easy generic method calling
    /// </summary>
    public static T InvokeMethod<T>(this object instance, string methodName, params object[] parameters)
    {
        return (T)instance.InvokeMethod(methodName, parameters);
    }

    /// <summary>
    /// Extension for easy property value retrieval
    /// </summary>
    public static object GetPropertyValue(this object instance, string propertyName)
    {
        var type = instance.GetType();
        var property = type.GetProperty(propertyName,
                           BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ??
                       throw new MissingMemberException(type.FullName, propertyName);
        var getter = OptimizedReflectionHelper.CreatePropertyGetter(property);
        return getter(instance);
    }

    /// <summary>
    /// Extension for easy property value retrieval with type conversion
    /// </summary>
    public static T GetPropertyValue<T>(this object instance, string propertyName)
    {
        return (T)instance.GetPropertyValue(propertyName);
    }


    /// <summary>
    /// Extension for easy field value retrieval
    /// </summary>
    public static object GetFieldValue(this object instance, string fieldName)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        var type = instance.GetType();
        var field = type.GetField(fieldName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ??
                    throw new MissingMemberException(type.FullName, fieldName);
        var getter = OptimizedReflectionHelper.CreateFieldGetter(field);
        return getter(instance);
    }

    /// <summary>
    /// Extension for easy field value setting
    /// </summary>
    public static void SetFieldValue(this object instance, string fieldName, object value)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        var type = instance.GetType();
        var field = type.GetField(fieldName,
                        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) ??
                    throw new MissingMemberException(type.FullName, fieldName);
        var setter = OptimizedReflectionHelper.CreateFieldSetter(field);
        setter(instance, value);
    }

    /// <summary>
    /// Extension for easy property value setting
    /// </summary>
    public static void SetPropertyValue(this object instance, string propertyName, object value)
    {
        var type = instance.GetType();
        var property = type.GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (property == null)
            throw new MissingMemberException(type.FullName, propertyName);

        var setter = OptimizedReflectionHelper.CreatePropertySetter(property);
        setter(instance, value);
    }
}

/// <summary>
/// Helper class to provide flexible Fluent API
/// </summary>
/// <typeparam name="T"></typeparam>
public class ReflectionQuery<T>
{
    private readonly Type _type;
    private BindingFlags _bindingFlags = BindingFlags.Public | BindingFlags.Instance;

    public ReflectionQuery()
    {
        _type = typeof(T);
    }

    public ReflectionQuery<T> IncludeNonPublic()
    {
        _bindingFlags |= BindingFlags.NonPublic;
        return this;
    }

    public ReflectionQuery<T> IncludeStatic()
    {
        _bindingFlags |= BindingFlags.Static;
        return this;
    }

    public IEnumerable<PropertyInfo> Properties(string pattern = null)
    {
        var props = _type.GetProperties(_bindingFlags);

        if (string.IsNullOrEmpty(pattern))
            return props;

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return props.Where(p => regex.IsMatch(p.Name));
    }

    public IEnumerable<MethodInfo> Methods(string pattern = null)
    {
        return OptimizedReflectionHelper.FindMethods(_type, pattern, _bindingFlags);
    }

    public IEnumerable<FieldInfo> Fields(string pattern = null)
    {
        var fields = _type.GetFields(_bindingFlags);

        if (string.IsNullOrEmpty(pattern))
            return fields;

        var regex = new Regex(pattern, RegexOptions.IgnoreCase);
        return fields.Where(f => regex.IsMatch(f.Name));
    }

    public IEnumerable<ConstructorInfo> Constructors()
    {
        return OptimizedReflectionHelper.FindConstructors(_type, _bindingFlags);
    }
}