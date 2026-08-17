using Message.Encoder.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Message.Encoder.Extensions;

internal static class AppDomainExtensions
{
    private static readonly IEnumerable<Type> Types = default;
    private static readonly Type MessageOpenGeneric = default;

    static AppDomainExtensions()
    {
        Types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        MessageOpenGeneric = typeof(Message<,>);
    }

    public static IEnumerable<Type> GetSubclassesOf<TType>(this AppDomain @this) where TType : class
        => Types.Where(type => type.IsSubclassOf(typeof(TType)) && type.IsAbstract is false);

    public static TCollection GetSubclassesOf<TType, TCollection>(this AppDomain @this, Func<IEnumerable<Type>, TCollection> factory)
        where TType : class
        where TCollection : class, ICollection
    {
        return factory(@this.GetSubclassesOf<TType>());
    }

    public static Dictionary<Type, Type> GetSubclassesOfOpenGeneric(this AppDomain _, Type openGenericType) =>
        Types.Where(type => type.IsGenericTypeCandidate(openGenericType))
            .Select(type => type)
            .ToDictionary(KeySelector, type => type);

    private static Type KeySelector(Type type)
    {
        var result = MessageOpenGeneric.MakeGenericType(type.BaseType!.GenericTypeArguments[0]!,
            type.BaseType!.GenericTypeArguments[1]!);

        return result;
    }
}