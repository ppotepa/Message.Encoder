using Message.Encoder.Extensions;
using Message.Encoder.Messages;
using Message.Encoder.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Message.Encoder.Factories.Serialization
{
    internal static class SerializersFactory
    {
        internal static Dictionary<Type, Type> HeadersSerializers = default;
        internal static Dictionary<Type, Type> PayloadSerializers = default;

        static SerializersFactory()
        {
            HeadersSerializers ??= AppDomain.CurrentDomain
                .GetSubclassesOf<MessageHeader>()
                .ToDictionary(payload => payload, CustomTypeExtensions.ObtainHeaderSerializer);

            PayloadSerializers ??= AppDomain.CurrentDomain
                .GetSubclassesOf<Payload>()
                .ToDictionary(payload => payload, CustomTypeExtensions.ObtainPayloadSerializer);

        }

        private static ISerializer CreateSerializer(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (typeof(Payload).IsAssignableFrom(type))
                return Activator.CreateInstance(PayloadSerializers[type]) as ISerializer;

            if (typeof(MessageHeader).IsAssignableFrom(type))
                return Activator.CreateInstance(HeadersSerializers[type]) as ISerializer;

            throw new ArgumentException(
                $"{type.Name} is not assignable from {nameof(Payload)} nor from {nameof(MessageHeader)}."
            );
        }

        public static TSerializer CreateSerializer<TSerializer>(Type type)
            where TSerializer : ISerializer
        {
            return (TSerializer)CreateSerializer(type);
        }
    }
}
