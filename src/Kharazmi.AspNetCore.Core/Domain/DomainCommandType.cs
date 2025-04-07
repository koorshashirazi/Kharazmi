using System;
using System.Text.Json.Serialization;
using Kharazmi.AspNetCore.Core.Extensions;
using Newtonsoft.Json;

namespace Kharazmi.AspNetCore.Core.Domain;

public readonly record struct DomainCommandType
{
    public DomainCommandType()
    {
        Value = string.Empty;
    }

    [System.Text.Json.Serialization.JsonConstructor, Newtonsoft.Json.JsonConstructor]
    public DomainCommandType(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(value));
        }

        Value = value;
    }

    public static DomainCommandType From<T>(Func<Type?, Type>? validationHandler = null) where T : class, IDomainCommand
        => From(typeof(T), validationHandler);

    /// <summary>
    /// Converts the specified <paramref name="type"/> to a <see cref="DomainCommandType"/> instance.
    /// </summary>
    /// <param name="type">The type to be converted.</param>
    /// <param name="validationHandler">A function that performs custom validation logic. Optional.</param>
    /// <returns>A new instance of <see cref="DomainCommandType"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the type is null.</exception>
    public static DomainCommandType From(Type? type, Func<Type?, Type>? validationHandler = null)
    {
        var validateType = validationHandler is not null ? validationHandler(type) : ValidateType(type);
        if (validateType == null) throw new InvalidCastException($"Invalid {nameof(DomainCommandType)}");

        return new DomainCommandType(validateType.GetTypeFullName());
    }

    public static Type ValidateType(Type? type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!typeof(IDomainCommand).IsAssignableFrom(type) || !type.IsClassType())
            throw new InvalidCastException(
                $"The {type} is not assignable from {typeof(IDomainCommand).GetTypeFullName()}");

        return type;
    }

    [JsonProperty, JsonInclude] public string Value { get; }


    public Type ToType()
    {
        var type = Type.GetType(Value);

        return type ?? throw new InvalidCastException($"The {Value} is not a valid {nameof(DomainCommandType)}");
    }

    public override string ToString()
    {
        return Type.GetType(Value)?.GetTypeFullName() ?? Value;
    }
    
    public static implicit operator string(DomainCommandType domainCommandType) => domainCommandType.ToString();
}