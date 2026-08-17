using Message.Encoder.Extensions;
using Message.Encoder.Factories.Serialization;
using Message.Encoder.Messages;
using Message.Encoder.Messages.Transport;
using Message.Encoder.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Message.Encoder.Factories.Messages
{
    internal sealed class MessageFactory
    {
        private static readonly Dictionary<byte, Type> MessageTypes = default;
        private static readonly Dictionary<Type, Type> MessageTypesBinding = default;

        static MessageFactory()
        {
            MessageTypes = AppDomain.CurrentDomain.GetSubclassesOf<Message, Dictionary<byte, Type>>(Factory);
            MessageTypesBinding = AppDomain.CurrentDomain.GetSubclassesOfOpenGeneric(typeof(Message<,>));
        }

        public static Message Create(ReadOnlySpan<byte> messageBinary)
        {
            var serializers = GetSerializers
            (
                messageBinary: messageBinary,
                target: out var target,
                messageTransport: out var messageTransport
            );

            var targetType = MessageTypesBinding[typeof(Message<,>)
                    .MakeGenericType(target.GenericTypeArguments[0], target.GenericTypeArguments[1])];

            var instance = Activator.CreateInstance
            (
                type: targetType,
                args: new object[]
                {
                    serializers.headers.Deserialize(target.GenericTypeArguments[0], messageTransport.HeaderTransportInfo),
                    serializers.payload.Deserialize(target.GenericTypeArguments[1], messageTransport.BinaryPayload)
                }
            );

            return instance as Message;
        }

        public static Message Create(byte[] messageBinary)
            => Create(new ReadOnlySpan<byte>(messageBinary));

        public static byte[] Serialize<TMessage>(TMessage message)
            where TMessage : Message
        {
            var serializers = Create(message.GetType().BaseType);

            var headers = serializers.headers.Serialize(message.Headers as MessageHeader).ToArray();
            var payload = serializers.payload.Serialize(message.Payload as Payload).ToArray();

            return headers.Concat(payload).ToArray();
        }

        private static (IHeadersSerializer headers, IPayloadSerializer payload) Create(Type targetBase) => (
            headers: SerializersFactory.CreateSerializer<IHeadersSerializer>(targetBase!.GenericTypeArguments[0]),
            payload: SerializersFactory.CreateSerializer<IPayloadSerializer>(targetBase!.GenericTypeArguments[1])
        );

        private static Dictionary<byte, Type> Factory(IEnumerable<Type> types)
                                            => types.ToDictionary(type => type.GetMessageTypeCode(), type => type);

        private static (IHeadersSerializer headers, IPayloadSerializer payload) GetSerializers(ReadOnlySpan<byte> messageBinary,
            out Type target, out MessageTransport messageTransport)
        {
            messageTransport = MessageTransport.FromSpan(messageBinary);
            target = MessageTypes[messageTransport.HeaderTransportInfo.MSG_TYPE].BaseType!;

            return Create(target);
        }
    }
}
