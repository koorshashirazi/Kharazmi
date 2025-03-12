using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Kharazmi.AspNetCore.Core.Extensions;
using Kharazmi.AspNetCore.Core.Helpers;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.ValueObjects
{
    public interface IIdentity
    {
        string Value { get; }
    }

    public static class Identity
    {
        public static readonly Regex ValueValidation = new Regex(
            @"^[^\-]+\-(?<guid>[a-f0-9]{8}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{4}\-[a-f0-9]{12})$",
            RegexOptions.Compiled);

        public static string NameWithDash<T>() where T : Identity<T>
        {
            return new Regex("Id$").Replace(typeof(T).Name, string.Empty).ToLowerInvariant() + "-";
        }

        public static T New<T>() where T : Identity<T>
        {
            return With<T>(Guid.NewGuid());
        }

        public static T NewDeterministic<T>(Guid namespaceId, string name) where T : Identity<T>
        {
            var guid = GuidFactories.Deterministic.Create(namespaceId, name);
            return With<T>(guid);
        }

        public static T NewDeterministic<T>(Guid namespaceId, byte[] nameBytes) where T : Identity<T>
        {
            var guid = GuidFactories.Deterministic.Create(namespaceId, nameBytes);
            return With<T>(guid);
        }

        public static T NewComb<T>() where T : Identity<T>
        {
            var guid = GuidFactories.Comb.CreateForString();
            return With<T>(guid);
        }

        public static T With<T>(string value) where T : Identity<T>
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), value);
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }

        public static T With<T>(Guid guid) where T : Identity<T>
        {
            var value = $"{NameWithDash<T>()}{guid:D}";
            return With<T>(value);
        }

        public static bool IsValid<T>(string value) where T : Identity<T>
        {
            return !Validate<T>(value).Any();
        }

        public static IEnumerable<string> Validate<T>(string value) where T : Identity<T>
        {
            var nameWithDash = NameWithDash<T>();
            if (string.IsNullOrEmpty(value))
            {
                yield return $"Identity of type '{typeof(T).PrettyPrint()}' is null or empty";
                yield break;
            }

            if (!string.Equals(value.Trim(), value, StringComparison.OrdinalIgnoreCase))
                yield return
                    $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' contains leading and/or trailing spaces";
            if (!value.StartsWith(nameWithDash, StringComparison.InvariantCulture))
                yield return
                    $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not start with '{nameWithDash}'";
            if (!ValueValidation.IsMatch(value))
                yield return
                    $"Identity '{value}' of type '{typeof(T).PrettyPrint()}' does not follow the syntax '[NAME]-[GUID]' in lower case";
        }
    }

    public abstract class Identity<T> : SingleValueObject<Identity<T>, string>, IIdentity
        where T : Identity<T>
    {
        protected Identity(string value) : base(value)
        {
            var validationErrors = Identity.Validate<T>(value).ToList();
            if (validationErrors.Count != 0)
            {
                throw new ArgumentException($"Identity is invalid: {string.Join(", ", validationErrors)}");
            }

            _lazyGuid = new Lazy<Guid>(() => Guid.Parse(Identity.ValueValidation.Match(Value).Groups["guid"].Value));
        }

        private readonly Lazy<Guid> _lazyGuid;


        public Guid GetGuid() => _lazyGuid.Value;
    }


    /// <summary>
    /// Represents an interface for objects with an identifier.
    /// </summary>
    public interface IId : ISingleValueObject, IEquatable<IId>
    {
        bool IsDefault();
    }

    /// <summary>
    /// Represents an interface with a generic value.
    /// </summary>
    /// <typeparam name="TKey">The type of the value.</typeparam>
    public interface IId<TKey> : ISingleValueObject<TKey>, IId, IEquatable<IId<TKey>>
        where TKey : IEquatable<TKey>;

    public static class Id
    {
        #region Generators

        public static readonly IReadOnlyDictionary<Type, Delegate> Generators = new Dictionary<Type, Delegate>
        {
            { typeof(string), (Func<string, string>)(_ => Guid.NewGuid().ToString("N")) },
            { typeof(Guid), (Func<Guid, Guid>)(_ => Guid.NewGuid()) },
            { typeof(int), (Func<int, int>)(current => current + 1) },
            { typeof(long), (Func<long, long>)(current => current + 1) },
            { typeof(uint), (Func<uint, uint>)(current => current + 1) },
            { typeof(ulong), (Func<ulong, ulong>)(current => current + 1) },
            { typeof(short), (Func<short, short>)(current => (short)(current + 1)) },
            { typeof(ushort), (Func<ushort, ushort>)(current => (ushort)(current + 1)) },
            { typeof(float), (Func<float, float>)(current => current + 1f) },
            { typeof(double), (Func<double, double>)(current => current + 1d) },
            { typeof(decimal), (Func<decimal, decimal>)(current => current + 1m) }
        }.AsReadOnly();

        public static readonly IReadOnlyDictionary<Type, Delegate> DefaultGenerators = new Dictionary<Type, Delegate>
        {
            { typeof(string), (Func<string>)(() => Guid.Empty.ToString("N")) },
            { typeof(Guid), (Func<Guid>)(() => Guid.Empty) },
            { typeof(int), (Func<int>)(() => 0) },
            { typeof(long), (Func<long>)(() => 0) },
            { typeof(uint), (Func<uint>)(() => 0) },
            { typeof(ulong), (Func<ulong>)(() => 0) },
            { typeof(short), (Func<short>)(() => 0) },
            { typeof(ushort), (Func<ushort>)(() => 0) },
            { typeof(float), (Func<float>)(() => 0f) },
            { typeof(double), (Func<double>)(() => 0d) },
            { typeof(decimal), (Func<decimal>)(() => 0m) }
        }.AsReadOnly();

        #endregion

        public static TKey Default<TKey>() where TKey : IEquatable<TKey>
        {
            if (!DefaultGenerators.TryGetValue(typeof(TKey), out var idGenerator))
            {
                throw new NotSupportedException($"No generator defined for type {typeof(TKey)}");
            }

            return ((Func<TKey>)idGenerator).Invoke();
        }

        public static Id<TKey>? ValueOrNull<TKey>(TKey? value) where TKey : IEquatable<TKey>
        {
            return value is null ? null : new Id<TKey>(value);
        }

        public static TKey New<TKey>() where TKey : IEquatable<TKey>
        {
            if (!Generators.TryGetValue(typeof(TKey), out var idGenerator))
            {
                throw new NotSupportedException($"No generator defined for type {typeof(TKey)}");
            }

            return ((Func<TKey, TKey>)idGenerator).Invoke(Default<TKey>());
        }


        /// <summary>
        /// Generates a new unique identifier.
        /// </summary>
        /// <returns>A new unique identifier as a TValue.</returns>
        public static TKey IncrementValue<TKey>(TKey currentValue)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            if (!Id.Generators.TryGetValue(typeof(TKey), out var idGenerator))
            {
                throw new NotSupportedException($"No generator defined for type {typeof(TKey)}");
            }

            var newValue = ((Func<TKey?, TKey>)idGenerator).Invoke(currentValue);

            return newValue;
        }


        /// <summary>
        /// Converts an object implementing the <see cref="IId{TValue}"/> interface to an <see cref="Id{TValue}"/> object.
        /// </summary>
        /// <typeparam name="TKey">The type of value held by the Id.</typeparam>
        /// <param name="value">The object implementing the <see cref="IId{TValue}"/> interface to convert.</param>
        /// <returns>
        /// An <see cref="Id{TValue}"/> object created from the specified <paramref name="value"/>.
        /// </returns>
        public static Id<TKey> From<TKey>(IId<TKey> value)
            where TKey : IEquatable<TKey>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            return From(value.Value);
        }

        /// <summary>
        /// Creates a new instance of Id<TKey> from the specified value.</TKey>
        /// </summary>
        /// <typeparam name="TKey">The type of the value.</typeparam>
        /// <param name="value">The value to create the Id from.</param>
        /// <returns>A new instance of Id<TKey>.</TKey></returns>
        public static Id<TKey> From<TKey>(TKey value) where TKey : IEquatable<TKey>
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            return new Id<TKey>(value);
        }

        public static Id<string> From(Guid value) =>
            new(value.ConvertToString(formatProvider: CultureInfo.InvariantCulture));

        public static Id<string> From(ushort value) =>
            new(value.ConvertToString(formatProvider: CultureInfo.InvariantCulture));

        public static Id<string> From(short value) =>
            new(value.ConvertToString(formatProvider: CultureInfo.InvariantCulture));

        public static Id<string> From(uint value) =>
            new(value.ConvertToString(formatProvider: CultureInfo.InvariantCulture));

        public static Id<string> From(int value) =>
            new(value.ConvertToString(formatProvider: CultureInfo.InvariantCulture));

        public static Id<string> From(ulong value) =>
            new(value.ConvertToString(formatProvider: CultureInfo.InvariantCulture));

        public static Id<string> From(long value) =>
            new(value.ConvertToString(formatProvider: CultureInfo.InvariantCulture));

        public static Id<string> From(Type type, ulong version) => new($"{type.GetTypeName()}:v{version}");

        public static Guid ToGuid(Id<string> id) => new(id.Value);
        public static ushort ToUshort(Id<string> id) => Convert.ToUInt16(id.Value, CultureInfo.InvariantCulture);
        public static short ToShort(Id<string> id) => Convert.ToInt16(id.Value, CultureInfo.InvariantCulture);
        public static uint ToUint(Id<string> id) => Convert.ToUInt32(id.Value, CultureInfo.InvariantCulture);
        public static int ToInt(Id<string> id) => Convert.ToInt32(id.Value, CultureInfo.InvariantCulture);
        public static long ToLong(Id<string> id) => Convert.ToInt64(id.Value, CultureInfo.InvariantCulture);
        public static ulong ToUlong(Id<string> id) => Convert.ToUInt64(id.Value, CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Represents a type-safe identifier with a generic value.
    /// </summary>
    /// <typeparam name="TValue">The type of the identifier value.</typeparam>
    public readonly partial record struct Id<TValue> : IId<TValue>
        where TValue : IEquatable<TValue>
    {
        public Id()
        {
            if (!Id.Generators.TryGetValue(typeof(TValue), out var idGenerator))
            {
                throw new NotSupportedException($"No generator defined for type {typeof(TValue)}");
            }

            Value = ((Func<TValue?, TValue>)idGenerator).Invoke(default);
        }

        [System.Text.Json.Serialization.JsonConstructor, Newtonsoft.Json.JsonConstructor]
        public Id(TValue value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Id(TValue value, IEqualityComparer<TValue> equalityComparer) : this(value)
        {
            DefaultEqualityComparer = equalityComparer;
        }

        [JsonInclude, JsonProperty] public TValue Value { get; }

        [System.Text.Json.Serialization.JsonIgnore, Newtonsoft.Json.JsonIgnore]
        public IEqualityComparer<TValue> DefaultEqualityComparer { get; } = EqualityComparer<TValue>.Default;

        public object GetObjectValue() => Value;

        /// <summary>
        /// Converts the value of the class to the specified type TValueKey.
        /// </summary>
        /// <typeparam name="TValueKey">The type to convert the value to.</typeparam>
        /// <returns>The converted value.</returns>
        public readonly TValueKey ValueAs<TValueKey>() where TValueKey : IEquatable<TValueKey>
            => Value.ChangeTypeTo<TValueKey>();

        public readonly TValue ToTKey()
        {
            return Value;
        }

        public readonly Guid ToGuid()
        {
            return ValueAs<Guid>();
        }

        public readonly ushort ToUInt16()
        {
            return ValueAs<ushort>();
        }

        public readonly short ToInt16()
        {
            return ValueAs<short>();
        }

        public readonly uint ToUInt32()
        {
            return ValueAs<uint>();
        }

        public readonly int ToInt32()
        {
            return ValueAs<int>();
        }

        public readonly long ToInt64()
        {
            return ValueAs<long>();
        }

        public readonly ulong ToUInt64()
        {
            return ValueAs<ulong>();
        }

        public bool IsDefault() => DefaultEqualityComparer.Equals(Value, Id.Default<TValue>());

        public readonly bool Equals(ISingleValueObject? other) => other is Id<TValue> value && Equals(value);

        /// <summary>
        /// Determines whether the current object is equal to another object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public readonly bool Equals(ISingleValueObject<TValue>? other) => other is Id<TValue> value && Equals(value);

        /// <summary>
        /// Determines whether the current object is equal to another object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public readonly bool Equals(IId? other) => other is Id<TValue> value && Equals(value);


        /// <summary>
        /// Determines whether the current object is equal to another object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public readonly bool Equals(IId<TValue>? other) => other is Id<TValue> value && Equals(value);

        /// <summary>
        /// Determines whether the current object is equal to another object.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>
        /// true if the current object is equal to the other parameter; otherwise, false.
        /// </returns>
        public bool Equals(Id<TValue>? other) =>
            other is not null && DefaultEqualityComparer.Equals(Value, other.Value.Value);

        /// <summary>
        /// Implicitly converts an Id&lt;TValue&gt; object to a string.
        /// </summary>
        /// <param name="id">The Id&lt;TValue&gt; object to be converted.</param>
        /// <returns>A string representation of the Id&lt;TValue&gt; object.</returns>
        public static implicit operator string(Id<TValue> id) => id.ToString();

        /// <summary>
        /// Implicitly converts an instance of Id<TValue/> to Guid.
        /// </summary>
        /// <param name="id">The Id<TValue/> instance to be converted.</param>
        /// <returns>The Guid value obtained from the Id<TValue/> instance.</returns>
        public static implicit operator Guid(Id<TValue> id) => id.ToGuid();

        /// <summary>
        /// This method allows for an implicit conversion of Id<TValue/> to ushort.
        /// </summary>
        /// <param name="id">The Id<TValue/> object to convert to ushort.</param>
        /// <returns>The Id<TValue/> value converted to ushort.</returns>
        public static implicit operator ushort(Id<TValue> id) => id.ToUInt16();

        /// <summary>
        /// Implicitly converts an Id<TValue/> to a short value.
        /// </summary>
        /// <param name="id">The Id<TValue/> instance to convert.</param>
        /// <returns>The short value of the Id.</returns>
        public static implicit operator short(Id<TValue> id) => id.ToInt16();

        /// <summary>
        /// Defines an implicit conversion from an Id<TValue/> to an unsigned integer (uint) value.
        /// </summary>
        /// <param name="id">The Id<TValue/> object to convert.</param>
        /// <returns>The unsigned integer value represented by the Id<TValue/> object.</returns>
        public static implicit operator uint(Id<TValue> id) => id.ToUInt32();

        /// <summary>
        /// Returns the integer representation of the Id object.
        /// </summary>
        /// <typeparam name="TValue">The type of value contained in the Id object.</typeparam>
        /// <param name="id">The Id object to convert to integer.</param>
        /// <returns>The integer representation of the Id object.</returns>
        public static implicit operator int(Id<TValue> id) => id.ToInt32();

        /// <summary>
        /// Implicitly converts an Id<TValue/> object to a long value.
        /// </summary>
        /// <param name="id">The Id<TValue/> object to convert.</param>
        /// <returns>The converted long value.</returns>
        public static implicit operator long(Id<TValue> id) => id.ToInt64();

        /// <summary>
        /// Converts an instance of the Id<TValue/> class to an unsigned long.
        /// </summary>
        /// <param name="id">The Id<TValue/> object to be converted.</param>
        /// <returns>The value of the Id<TValue/> object as an unsigned long.</returns>
        public static implicit operator ulong(Id<TValue> id) => id.ToUInt64();
    }
}