#if NETSTANDARD

namespace Antlr4.StringTemplate.Extensions;

using System.Reflection;

internal static class PropertyInfoExtensions
{
    public static MethodInfo GetGetMethod(this PropertyInfo propertyInfo) 
        => propertyInfo.GetMethod;
}

#endif
