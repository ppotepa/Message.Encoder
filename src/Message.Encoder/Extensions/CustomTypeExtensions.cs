using Message.Encoder.Attributes;
using Message.Encoder.Serializers.Default;
using System;
using System.Reflection;

namespace Message.Encoder.Extensions
{
    internal static class CustomTypeExtensions
    {
        public static Type ObtainHeaderSerializer(this Type type)
        {
            return (type.GetCustomAttribute(typeof(UseSerializerAttribute)) as UseSerializerAttribute)?.Serializer
                   ?? typeof(DefaultHeadersSerializer);
        }

        public static Type ObtainPayloadSerializer(this Type type)
        {
            return (type.GetCustomAttribute(typeof(UseSerializerAttribute)) as UseSerializerAttribute)?.Serializer
                   ?? typeof(DefaultPayloadSerializer);
        }

        public static bool IsGenericTypeCandidate(this Type type, Type openGenericType)
        {
            return type.IsAbstract is false && type.IsInterface is false &&
                   type.BaseType is { IsGenericType: true } &&
                   type.BaseType.GetGenericTypeDefinition() == openGenericType;
        }

        public static byte GetMessageTypeCode(this Type type) =>
            type.GetCustomAttribute<MessageTypeAttribute>()!.MessageTypeCode;

    }

}