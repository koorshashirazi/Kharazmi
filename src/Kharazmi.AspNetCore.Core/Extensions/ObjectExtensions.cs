using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Kharazmi.AspNetCore.Core.ValueObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    /// <summary>
    ///     Extension methods for all objects.
    /// </summary>
    public static partial class Core
    {
        public static Dictionary<string, TValue> ToDictionary<TValue>(this object obj)
        {
            var json = JsonConvert.SerializeObject(obj);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, TValue>>(json);
            return dictionary;
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                someObjectType
                    .GetProperty(item.Key)
                    ?.SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        public static IDictionary<string, object> AsDictionary(this object source,
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        /// <summary>
        ///     Used to simplify and beautify casting an object to a type.
        /// </summary>
        /// <typeparam name="T">Type to be casted</typeparam>
        /// <param name="obj">Object to cast</param>
        /// <returns>Casted object</returns>
        public static T As<T>(this object obj)
            where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Converts given object to a value or enum type using <see cref="Convert.ChangeType(object,TypeCode)"/> or <see cref="Enum.Parse(Type,string)"/> method.
        /// </summary>
        /// <param name="value">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object value)
            where T : IConvertible
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (typeof(T) == typeof(Guid))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value.ToString());
            }

            if (!typeof(T).IsEnum) return (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);

            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.Parse(typeof(T), value.ToString());
            }

            throw new ArgumentException($"Enum type undefined '{value}'.");
        }

        /// <summary>
        ///     Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }

        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetTypeInfo().GetGenericTypeName();
        }


        /// <summary>
        /// Converts the provided <paramref name="value"/> to a strongly typed value object.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An instance of <typeparamref name="T"/> representing the provided <paramref name="value"/>.</returns>
        public static T FromString<T>(this string value)
        {
            if (value == null)
            {
                return default;
            }

            return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value);
        }

        /// <summary>
        /// Converts the provided <paramref name="value"/> to a strongly typed value object.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>An instance of <typeparamref name="T"/> representing the provided <paramref name="value"/>.</returns>
        public static T To<T>(this string value)
        {
            return FromString<T>(value);
        }

        /// <summary>
        ///     Converts given object to a value or enum type using <see cref="Convert.ChangeType(object,TypeCode)" /> or
        ///     <see cref="Enum.Parse(Type,string)" /> method.
        /// </summary>
        /// <param name="valueObject">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T ChangeTypeTo<T>(this object valueObject)
        {
            if (valueObject is T valueResult)
            {
                return valueResult;
            }

            var valueType = typeof(T);
            if (valueObject is null)
            {
                throw new InvalidCastException($"Can't change the given value to {valueType}, The given value is null");
            }

            if (TryConvertToPrimitive<T>(valueObject, out var value))
            {
                return value;
            }

            throw new InvalidCastException($"Can't change the given value to {valueType}");
        }

        public static string ConvertToString(this object? value, string? format = null,
            IFormatProvider? formatProvider = null)
        {
            return ExceptionHandler.ExecuteAs(() =>
            {
                return value switch
                {
                    null => string.Empty,
                    string str => str,
                    int i => i.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    long l => l.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    uint ui => ui.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    ulong ul => ul.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    float f => f.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    double d => d.ToString(format, formatProvider),
                    byte b => b.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    decimal dc => dc.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    sbyte sb => sb.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    ushort us => us.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    short s => s.ToString(format, formatProvider ?? NumberFormatInfo.InvariantInfo),
                    bool bo => $"{bo}".ToLower(CultureInfo.InvariantCulture)
                        .ToString(formatProvider ?? CultureInfo.InvariantCulture),
                    JValue jValue => ConvertToString(jValue.Value, format, formatProvider),
                    JObject jObject => jObject.ToString(),
                    ISingleValueObject valueObject => ConvertToString(valueObject.GetObjectValue(), format, formatProvider),
                    Guid guid => guid.ToString(format),
                    TimeSpan timeSpan => timeSpan.ToString(format,
                        formatProvider ?? CultureInfo.InvariantCulture),
                    DateTime dateTime => dateTime.ToString(format,
                        formatProvider ?? CultureInfo.InvariantCulture),
                    DateTimeOffset offset => offset.ToString(format,
                        formatProvider ?? CultureInfo.InvariantCulture),
                    _ => Convert.ToString(value, formatProvider ?? CultureInfo.InvariantCulture) ?? string.Empty
                };
            }, onError: _ => string.Empty) ?? string.Empty;
        }

        public static object? TryConvertToPrimitive(this object? value, TypeCode typeCode)
        {
            try
            {
                if (value is null)
                {
                    return value;
                }

                return typeCode switch
                {
                    TypeCode.Object when value is ISingleValueObject valueObject =>
                        valueObject.GetObjectValue()
                            .TryConvertToPrimitive(Type.GetTypeCode(valueObject.GetObjectValue().GetType())),
                    TypeCode.Object when value is JValue jValue =>
                        jValue.Value.TryConvertToPrimitive(jValue.Type.GetTypeCode()),
                    TypeCode.Object => value,
                    TypeCode.Boolean when value is string stringValue => ConvertToBoolean(stringValue),
                    TypeCode.Boolean => Convert.ToBoolean(value,CultureInfo.InvariantCulture),
                    TypeCode.Char => Convert.ToChar(value, CultureInfo.InvariantCulture),
                    TypeCode.SByte => Convert.ToSByte(value, CultureInfo.InvariantCulture),
                    TypeCode.Byte => Convert.ToByte(value, CultureInfo.InvariantCulture),
                    TypeCode.Int16 => Convert.ToInt16(value, CultureInfo.InvariantCulture),
                    TypeCode.UInt16 => Convert.ToUInt16(value, CultureInfo.InvariantCulture),
                    TypeCode.Int32 => Convert.ToInt32(value, CultureInfo.InvariantCulture),
                    TypeCode.UInt32 => Convert.ToUInt32(value, CultureInfo.InvariantCulture),
                    TypeCode.Int64 => Convert.ToInt64(value, CultureInfo.InvariantCulture),
                    TypeCode.UInt64 => Convert.ToUInt64(value, CultureInfo.InvariantCulture),
                    TypeCode.Single => Convert.ToSingle(value, CultureInfo.InvariantCulture),
                    TypeCode.Double => Convert.ToDouble(value, CultureInfo.InvariantCulture),
                    TypeCode.Decimal => Convert.ToDecimal(value, CultureInfo.InvariantCulture),
                    TypeCode.DateTime => Convert.ToDateTime(value, CultureInfo.InvariantCulture),
                    TypeCode.String => ConvertToString(value, formatProvider:CultureInfo.InvariantCulture),
                    _ => value
                };
            }
            catch
            {
                return value;
            }
        }

  
        public static bool TryConvertToPrimitive<T>(this object? valueObject, out T? value)
        {
            value = default;
            if (valueObject is null) return false;

            try
            {
                if (valueObject is T resultType)
                {
                    value = resultType;
                    return true;
                }

                var conversionType = typeof(T);

                if (conversionType == typeof(Guid) && valueObject is string valueString)
                {
                    var guid = Guid.Parse(valueString);
                    if (guid is T guidValue)
                    {
                        value = guidValue;
                        return true;
                    }
                }
                
                var typeCode = Type.GetTypeCode(conversionType);
                var convertedValue = TryConvertToPrimitive(valueObject, typeCode);

                if (convertedValue is T result)
                {
                    value = result;
                    return true;
                }

                value = (T?)Convert.ChangeType(valueObject, conversionType, CultureInfo.InvariantCulture);
                return value is not null;
            }
            catch
            {
                return false;
            }
        }
        
        public static bool? ConvertToBoolean(string stringValue)
        {
            if (stringValue == null) throw new ArgumentNullException(nameof(stringValue));
            if (stringValue.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (stringValue.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;

            return null;
        }

    }
}