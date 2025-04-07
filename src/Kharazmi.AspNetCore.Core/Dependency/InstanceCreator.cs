using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Kharazmi.AspNetCore.Core.Exceptions;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.Dependency;

[AttributeUsage(AttributeTargets.Constructor)]
public sealed class InstanceConstructorAttribute : Attribute;

public sealed record InstanceCreatorOptions(uint Capacity, bool EnableLru = true)
{
    public uint Capacity { get; set; } = Capacity;
    public bool EnableLru { get; set; } = EnableLru;
}

public interface IInstanceCreator
{
    /// <summary>
    /// Creates an instance of the specified type using the provided parameters.
    /// </summary>
    /// <param name="concreteType">The type of the object to create.</param>
    /// <param name="options">Optional instance creator options.</param>
    /// <param name="primitiveArguments">Optional primitive arguments for the constructor.</param>
    /// <returns>An object of the specified type.</returns>
    object CreateInstance(Type concreteType, Action<InstanceCreatorOptions>? options = null,
        params object[] primitiveArguments);

    /// <summary>
    /// Creates an instance of type T with the given options.
    /// </summary>
    /// <typeparam name="T">The type of the instance to create.</typeparam>
    /// <param name="options">The optional options for creating the instance.</param>
    /// <param name="primitiveArguments"></param>
    /// <returns>An instance of type T.</returns>
    T CreateInstance<T>(Action<InstanceCreatorOptions>? options = null,
        params object[] primitiveArguments);

    void Dispose();
}

/// <summary>
/// The InstanceCreator class provides methods for creating instances of types.
/// </summary>
public class InstanceCreator : IDisposable, IInstanceCreator
{
    /// <summary>
    /// Dictionary that contains constructor information for types.
    /// </summary>
    internal readonly ConcurrentDictionary<string, TypeConstructorInfo> ConstructorInfos = new();

    // Track circular dependencies during recursive calls
    private readonly ThreadLocal<HashSet<Type>> _recursionGuard = new(() => []);

    // For LRU cache implementation
    private readonly ConcurrentDictionary<string, long> _lastAccessTimes = new();
    private long _accessCounter;

    /// <summary>
    /// Creates an instance of the specified type using the provided parameters.
    /// </summary>
    /// <param name="concreteType">The type of the object to create.</param>
    /// <param name="options">Optional instance creator options.</param>
    /// <param name="primitiveArguments">Optional primitive arguments for the constructor.</param>
    /// <returns>An object of the specified type.</returns>
    public object CreateInstance(Type concreteType, Action<InstanceCreatorOptions>? options = null,
        params object[] primitiveArguments)
    {
        if (concreteType == null) throw new ArgumentNullException(nameof(concreteType));
        if (primitiveArguments == null) throw new ArgumentNullException(nameof(primitiveArguments));

        TypeValidation(concreteType);

        // Simple case for value types
        if (concreteType.IsValueType)
        {
            return Activator.CreateInstance(concreteType) ??
                   throw new InstanceException($"Can't create an instance of value type {concreteType}");
        }

        // Check for circular dependencies
        var recursionSet = _recursionGuard.Value!;
        if (recursionSet.Contains(concreteType))
        {
            throw new InstanceException($"Circular dependency detected for type {concreteType}");
        }

        try
        {
            recursionSet.Add(concreteType);

            InstanceCreatorOptions option = new(250);
            options?.Invoke(option);

            var key = concreteType.GetTypeFullName();

            // Get or add constructor info to cache
            var typeConstructorInfo = ConstructorInfos.GetOrAdd(key, k =>
            {
                var constructorInfo = new TypeConstructorInfo(k, concreteType);
                ConstructorInfos.TryAdd(k, constructorInfo);

                // Manage cache size using LRU if enabled
                ManageCache(option);

                return constructorInfo;
            });

            // Update access time for LRU
            if (option.EnableLru)
            {
                _lastAccessTimes[key] = Interlocked.Increment(ref _accessCounter);
            }

            // Match parameters by position and type
            var constructorParameters = typeConstructorInfo.ParameterInfos.ToArray();
            var parametersToPass = new object[constructorParameters.Length];
            var primitiveArgsUsed = 0;

            for (int i = 0; i < constructorParameters.Length; i++)
            {
                var param = constructorParameters[i];
                var paramType = param.ParameterType;

                // If parameter is primitive or string and we have primitive arguments left
                if ((paramType.IsPrimitive || paramType == typeof(string)) &&
                    primitiveArgsUsed < primitiveArguments.Length)
                {
                    // Check if the primitive argument type is compatible
                    var primitiveArg = primitiveArguments[primitiveArgsUsed];
                    if (paramType.IsInstanceOfType(primitiveArg))
                    {
                        parametersToPass[i] = primitiveArg;
                        primitiveArgsUsed++;
                        continue;
                    }
                }

                // If parameter has a default value and we're at the end of provided arguments
                if (param.HasDefaultValue && primitiveArgsUsed >= primitiveArguments.Length)
                {
                    parametersToPass[i] = param.DefaultValue!;
                    continue;
                }

                // For non-primitive types or when primitive arguments don't match
                if (!paramType.IsPrimitive && paramType != typeof(string))
                {
                    parametersToPass[i] = CreateInstance(paramType, options, primitiveArguments);
                }
                else if (primitiveArgsUsed < primitiveArguments.Length)
                {
                    // Use the next primitive argument even if types don't exactly match
                    // (Let the CLR handle type conversion)
                    parametersToPass[i] = primitiveArguments[primitiveArgsUsed++];
                }
                else
                {
                    // If we get here, we're missing a required parameter
                    throw new InstanceException(
                        $"Missing required parameter '{param.Name}' of type {paramType} for constructor of {concreteType}");
                }
            }

            return typeConstructorInfo.ConstructorInfo.Invoke(parametersToPass);
        }
        finally
        {
            // Always remove the type from the recursion set when done
            recursionSet.Remove(concreteType);
        }
    }

    /// <summary>
    /// Creates an instance of type T with the given options.
    /// </summary>
    /// <typeparam name="T">The type of the instance to create.</typeparam>
    /// <param name="options">The optional options for creating the instance.</param>
    /// <param name="primitiveArguments"></param>
    /// <returns>An instance of type T.</returns>
    public T CreateInstance<T>(Action<InstanceCreatorOptions>? options = null,
        params object[] primitiveArguments)
    {
        var instance = CreateInstance(typeof(T), options, primitiveArguments);
        return instance is T value ? value : throw new InstanceException(typeof(T));
    }

    private void ManageCache(InstanceCreatorOptions options)
    {
        if (ConstructorInfos.Count < options.Capacity)
            return;

        if (options.EnableLru)
        {
            // LRU eviction strategy
            var leastRecentlyUsedKey = _lastAccessTimes
                .OrderBy(kvp => kvp.Value)
                .Select(kvp => kvp.Key)
                .FirstOrDefault();

            if (leastRecentlyUsedKey == null) return;
            ConstructorInfos.TryRemove(leastRecentlyUsedKey, out _);
            _lastAccessTimes.TryRemove(leastRecentlyUsedKey, out _);
        }
        else
        {
            // Simple FIFO strategy as before
            var keysToRemove = ConstructorInfos.Keys.First();
            ConstructorInfos.TryRemove(keysToRemove, out _);
        }
    }

    private static void TypeValidation(Type concreteType)
    {
        if (concreteType.IsAbstract || concreteType.IsInterface)
        {
            throw new InstanceException(
                $"Can't create an instance of type {concreteType}. The type {concreteType} is abstract or an interface");
        }

        if (concreteType is { IsGenericType: true, ContainsGenericParameters: true })
        {
            throw new InstanceException(
                $"Can't create an instance of open generic type {concreteType}. The type {concreteType} contains unresolved generic parameters");
        }
    }

    public void Dispose()
    {
        _recursionGuard.Dispose();
    }
}

/// <summary>
/// Represents information about a type constructor.
/// </summary>
internal readonly record struct TypeConstructorInfo
{
    public TypeConstructorInfo(string typeKey, Type targetType)
    {
        if (targetType == null) throw new ArgumentNullException(nameof(targetType));
        TypeKey = typeKey ?? throw new ArgumentNullException(nameof(typeKey));

        ConstructorInfo = targetType
                              .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                              .FirstOrDefault(x =>
                                  x.GetCustomAttribute<InstanceConstructorAttribute>() != null ||
                                  x.GetParameters().Length == 0) ??
                          throw new InstanceException(targetType);

        ParameterInfos = ConstructorInfo.GetParameters();
    }

    /// <summary>
    /// Gets the TypeKey of the property.
    /// </summary>
    /// <remarks>
    /// The TypeKey is a string representation of the type of the property.
    /// </remarks>
    public string TypeKey { get; }

    /// <summary>
    /// Gets the constructor information for a type.
    /// </summary>
    /// <remarks>
    /// This property returns an instance of the ConstructorInfo class, which provides information about a constructor of a type.
    /// ConstructorInfo is a reflection class that can be used to inspect and manipulate constructors at runtime.
    /// </remarks>
    public ConstructorInfo ConstructorInfo { get; }

    /// <summary>
    /// Gets the information about the parameters of the property.
    /// </summary>
    /// <value>
    /// A collection of ParameterInfo objects representing the parameters of the property.
    /// </value>
    public IReadOnlyCollection<ParameterInfo> ParameterInfos { get; }
}