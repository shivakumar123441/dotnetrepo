namespace InvestTrackerWebApi.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

internal static class TypeExtensions
{
    public static List<string> GetNestedClassesStaticStringValues(this Type type)
    {
        var values = new List<string>();
        foreach (var prop in type.GetNestedTypes().SelectMany(c =>
        c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            object? propertyValue = prop.GetValue(null);
            if (propertyValue?.ToString() is string propertyString)
            {
                values.Add(propertyString);
            }
        }

        return values;
    }
}
