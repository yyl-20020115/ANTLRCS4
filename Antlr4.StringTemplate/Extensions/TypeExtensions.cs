#if NETSTANDARD

namespace Antlr4.StringTemplate.Extensions;

using System;
using System.Collections.Generic;
using System.Reflection;

internal static class TypeExtensions
{
    public static FieldInfo GetField(this Type type, string name) 
        => type.GetRuntimeField(name);

    public static MethodInfo GetMethod(this Type type, string name, Type[] parameters) 
        => type.GetRuntimeMethod(name, parameters);

    public static IEnumerable<PropertyInfo> GetProperties(this Type type) 
        => type.GetRuntimeProperties();

    public static PropertyInfo GetProperty(this Type type, string name) 
        => type.GetRuntimeProperty(name);

    public static bool IsAssignableFrom(this Type type, Type otherType)
        => type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
}

#endif
