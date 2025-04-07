using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    public static partial class Core
    {
        public static string GetColumns(this Type type)
        {
            var selectedColumn = type
                .GetProperties()
                .Select(p => p.Name).ToList();

            return string.Join(",", selectedColumn);
        }

        public static List<string> GetColumnList(this Type type)
        {
            return [.. type
                .GetProperties()
                .Select(p => p.Name)];
        }

        public static string GetColumns(this Type type, Func<PropertyInfo, bool> predicate)
        {
            var selectedColumn = type
                .GetProperties()
                .Where(predicate)
                .Select(p => p.Name).ToList();

            return string.Join(",", selectedColumn);
        }

        public static List<string> GetColumnList(this Type type, Func<PropertyInfo, bool> predicate)
        {
            return [.. type
                .GetProperties(BindingFlags.GetField)
                .Where(predicate)
                .Select(p => p.Name)];
        }

        public static string GetColumn(this Type type, string columnName)
        {
            return type
                .GetProperty(columnName, BindingFlags.Public)
                ?.Name;
        }
    }
}