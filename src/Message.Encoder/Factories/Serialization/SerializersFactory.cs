using Message.Encoder.Extensions;
using Message.Encoder.Messages;
using Message.Encoder.Serializers;
using System;

namespace Message.Encoder.Factories.Serialization
{
    internal static class SerializersFactory
    {
        private static ISerializer CreateSerializer(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (typeof(Payload).IsAssignableFrom(type))
                return Activator.CreateInstance(type.ObtainPayloadSerializer()) as ISerializer
                    ?? throw new InvalidOperationException($"Unable to create payload serializer for {type.Name}.");

            if (typeof(MessageHeader).IsAssignableFrom(type))
                return Activator.CreateInstance(type.ObtainHeaderSerializer()) as ISerializer
                    ?? throw new InvalidOperationException($"Unable to create header serializer for {type.Name}.");

            throw new ArgumentException(
                $"{type.Name} is not assignable from {nameof(Payload)} nor from {nameof(MessageHeader)}."
            );
        }

        public static TSerializer CreateSerializer<TSerializer>(Type type)
            where TSerializer : ISerializer
        {
            var serializer = CreateSerializer(type);

            if (serializer is not TSerializer typedSerializer)
            {
                throw new InvalidOperationException(
                    $"Serializer {serializer.GetType().Name} is not assignable to {typeof(TSerializer).Name}."
                );
            }

            return typedSerializer;
        }
    }
}
