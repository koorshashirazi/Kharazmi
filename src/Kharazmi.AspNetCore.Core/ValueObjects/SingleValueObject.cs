using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.ValueObjects
{
    public interface ISingleValueObject : IEquatable<ISingleValueObject>
    {
        object GetObjectValue();
    }

    public interface ISingleValueObject<TValue> : ISingleValueObject, IEquatable<ISingleValueObject<TValue>>
        where TValue : IEquatable<TValue>
    {
        /// <summary>
        /// Gets the value of the property.
        /// </summary>
        [JsonProperty, JsonInclude]
        TValue Value { get; }
    }

    public abstract class SingleValueObject<T, TValue> : ISingleValueObject<TValue>
        where T : SingleValueObject<T, TValue>
        where TValue : IEquatable<TValue>
    {
        private readonly Type _type = typeof(T);
        private readonly TypeInfo _typeInfo = typeof(T).GetTypeInfo();

        protected SingleValueObject(TValue value)
        {
            if (_typeInfo.IsEnum && !Enum.IsDefined(_type, value))
            {
                throw new ArgumentException($"The value '{value}' isn't defined in enum '{_type.PrettyPrint()}'");
            }

            Value = value;
        }

        public TValue Value { get; }

        public object GetObjectValue()
        {
            return Value;
        }

        [System.Text.Json.Serialization.JsonIgnore, Newtonsoft.Json.JsonIgnore]
        public IEqualityComparer<TValue> DefaultEqualityComparer { get; set; } = EqualityComparer<TValue>.Default;

        public bool Equals(ISingleValueObject? other) => other is T value && Equals(value);

        public bool Equals(ISingleValueObject<TValue>? other) => other is T value && Equals(value);

        public bool Equals(T? other) =>
            other is not null && DefaultEqualityComparer.Equals(Value, other.Value);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}