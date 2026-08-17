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
            return type.GetCustomAttribute<UseSerializerAttribute>()?.Serializer
                   ?? typeof(DefaultHeadersSerializer);
        }

        public static Type ObtainPayloadSerializer(this Type type)
        {
            return type.GetCustomAttribute<UseSerializerAttribute>()?.Serializer
                   ?? typeof(DefaultPayloadSerializer);
        }

        public static byte GetMessageTypeCode(this Type type)
        {
            var attribute = type.GetCustomAttribute<MessageTypeAttribute>()
                ?? throw new InvalidOperationException(
                    $"Message type {type.FullName} must be decorated with {nameof(MessageTypeAttribute)}."
                );

            return attribute.MessageTypeCode;
        }
    }
}
