using System;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain;

public readonly record struct DomainEventType
{
    public DomainEventType()
    {
        Value = string.Empty;
    }

    [System.Text.Json.Serialization.JsonConstructor, Newtonsoft.Json.JsonConstructor]
    public DomainEventType(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value;
    }

    public static DomainEventType From<T>(Func<Type?, Type>? validationHandler = null) where T : class, IDomainEvent
        => From(typeof(T), validationHandler);

    /// <summary>
    /// Converts the specified <paramref name="type"/> to a <see cref="DomainEventType"/> instance.
    /// </summary>
    /// <param name="type">The type to be converted.</param>
    /// <param name="validationHandler">A function that performs custom validation logic. Optional.</param>
    /// <returns>A new instance of <see cref="DomainEventType"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the type is null.</exception>
    public static DomainEventType From(Type? type, Func<Type?, Type>? validationHandler = null)
    {
        var validateType = validationHandler is not null ? validationHandler(type) : ValidateType(type);
        if (validateType == null) throw new InvalidCastException($"Invalid {nameof(DomainEventType)}");

        return new DomainEventType(validateType.GetTypeFullName());
    }

    public static Type ValidateType(Type? type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!typeof(IDomainEvent).IsAssignableFrom(type) || !type.IsClassType())
            throw new InvalidCastException(
                $"The {type} is not assignable from {typeof(IDomainEvent).GetTypeFullName()}");

        return type;
    }

    [JsonProperty, JsonInclude] public string Value { get; }


    public Type ToType()
    {
        var type = Type.GetType(Value);

        return type ?? throw new InvalidCastException($"The {Value} is not a valid {nameof(DomainEventType)}");
    }

    public override string ToString()
    {
        return Type.GetType(Value)?.GetTypeFullName() ?? Value;
    }

    public static implicit operator string(DomainEventType domainEventType) => domainEventType.ToString();
}