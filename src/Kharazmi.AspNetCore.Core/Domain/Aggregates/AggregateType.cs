using System;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates;

public readonly record struct AggregateType 
{
    public AggregateType()
    {
        Value = string.Empty;
    }

    [System.Text.Json.Serialization.JsonConstructor, Newtonsoft.Json.JsonConstructor]
    public AggregateType(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value;
    }

    public static AggregateType From<T>(Func<Type?, Type>? validationHandler = null) where T : class, IAggregateRoot
        => From(typeof(T), validationHandler);


    public static AggregateType From(Type? type, Func<Type?, Type>? validationHandler = null)
    {
        var validateType = validationHandler is not null ? validationHandler(type) : ValidateType(type);
        if (validateType == null) throw new InvalidCastException($"Unbale to get a valid type for :{type}");

        return new AggregateType(validateType.GetTypeFullName());
    }

    public static Type ValidateType(Type? type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!typeof(IAggregateRoot).IsAssignableFrom(type) || !type.IsClassType())
            throw new InvalidCastException(
                $"The {type} is not assignable from {typeof(IAggregateRoot).GetTypeFullName()}");

        return type;
    }


    [JsonProperty, JsonInclude] public string Value { get; }

    public Type ToType()
    {
        var type = Type.GetType(Value);

        return type ?? throw new InvalidCastException($"the {nameof(Value)} is not a valid type");
    }
    public override string ToString()
    {
        return Value;
    }
 
    public static implicit operator string(AggregateType aggregateType) => aggregateType.ToString();
}