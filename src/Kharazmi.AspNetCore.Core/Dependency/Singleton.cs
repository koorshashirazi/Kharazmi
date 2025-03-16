using System;

namespace Kharazmi.AspNetCore.Core.Dependency;

/// <summary>
/// Represents a singleton class that ensures only one instance of the derived class can be created.
/// </summary>
/// <typeparam name="T">The derived class.</typeparam>
public abstract class Singleton<T> where T : Singleton<T>
{
    private static readonly Lazy<T> Lazy = new(() => new InstanceCreator().CreateInstance<T>());


    /// <summary>
    /// Gets the instance of a generic type using lazy initialization. </summary>
    /// <typeparam name="T">The type of the instance.</typeparam> <returns>The instance of the specified type.</returns>
    /// /
    public static T Instance => Lazy.Value;

    /// <summary>
    /// Checks if an instance of the class has already been created.
    /// </summary>
    /// <returns>
    /// True if an instance has been created; otherwise, false.
    /// </returns>
    public static bool HasInstanced() => Lazy.IsValueCreated;
}