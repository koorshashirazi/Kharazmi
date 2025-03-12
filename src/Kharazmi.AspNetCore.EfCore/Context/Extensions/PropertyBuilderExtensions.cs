using Kharazmi.AspNetCore.EFCore.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kharazmi.AspNetCore.EFCore.Context.Extensions
{
    public static class PropertyBuilderExtensions {

        /// <summary>
        /// Serializes field as JSON blob in database.
        /// </summary>
        public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder) where T : class {

            propertyBuilder
                .HasConversion(new JsonValueConverter<T>())
                .Metadata.SetValueComparer(new JsonValueComparer<T>());

            return propertyBuilder;

        }

    }
}