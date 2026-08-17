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
        public static Message Create(ReadOnlySpan<byte> messageBinary)
        {
            var messageTransport = MessageTransport.FromSpan(messageBinary);
            var messageTypes = GetMessageTypes();

            if (!messageTypes.TryGetValue(messageTransport.HeaderTransportInfo.MSG_TYPE, out var targetType))
            {
                throw new InvalidOperationException(
                    $"Unknown message type code {messageTransport.HeaderTransportInfo.MSG_TYPE}."
                );
            }

            var targetBase = targetType.BaseType
                ?? throw new InvalidOperationException($"Message type {targetType.Name} does not have a valid base type.");

            var serializers = CreateSerializers(targetBase);

            var instance = Activator.CreateInstance
            (
                type: targetType,
                args: new object[]
                {
                    serializers.headers.Deserialize(targetBase.GenericTypeArguments[0], messageTransport.HeaderTransportInfo),
                    serializers.payload.Deserialize(targetBase.GenericTypeArguments[1], messageTransport.BinaryPayload)
                }
            );

            return instance as Message
                ?? throw new InvalidOperationException($"Unable to create message type {targetType.Name}.");
        }

        public static Message Create(byte[] messageBinary)
            => Create(new ReadOnlySpan<byte>(messageBinary));

        public static byte[] Serialize<TMessage>(TMessage message)
            where TMessage : Message
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            var targetBase = message.GetType().BaseType
                ?? throw new InvalidOperationException($"Message type {message.GetType().Name} does not have a valid base type.");

            var serializers = CreateSerializers(targetBase);

            var headers = serializers.headers.Serialize(message.Headers as MessageHeader).ToArray();
            var payload = serializers.payload.Serialize(message.Payload as Payload).ToArray();

            return headers.Concat(payload).ToArray();
        }

        private static (IHeadersSerializer headers, IPayloadSerializer payload) CreateSerializers(Type targetBase) => (
            headers: SerializersFactory.CreateSerializer<IHeadersSerializer>(targetBase.GenericTypeArguments[0]),
            payload: SerializersFactory.CreateSerializer<IPayloadSerializer>(targetBase.GenericTypeArguments[1])
        );

        private static Dictionary<byte, Type> GetMessageTypes()
        {
            var candidates = AppDomain.CurrentDomain
                .GetSubclassesOf<Message>()
                .Select(type => new { Type = type, Code = type.GetMessageTypeCode() })
                .ToArray();

            var duplicate = candidates
                .GroupBy(candidate => candidate.Code)
                .FirstOrDefault(group => group.Count() > 1);

            if (duplicate is not null)
            {
                var types = string.Join(", ", duplicate.Select(candidate => candidate.Type.FullName));
                throw new InvalidOperationException(
                    $"Multiple message types use message type code {duplicate.Key}: {types}."
                );
            }

            return candidates.ToDictionary(candidate => candidate.Code, candidate => candidate.Type);
        }
    }
}
