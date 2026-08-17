using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Message.Encoder.Extensions;

internal static class AppDomainExtensions
{
    public static IEnumerable<Type> GetSubclassesOf<TType>(this AppDomain @this) where TType : class
        => @this.GetAssemblies()
            .SelectMany(GetLoadableTypes)
            .Where(type => type.IsSubclassOf(typeof(TType)) && type.IsAbstract is false);

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}
