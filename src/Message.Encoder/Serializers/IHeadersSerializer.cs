using Message.Encoder.Messages;
using Message.Encoder.Messages.Transport;
using System;

namespace Message.Encoder.Serializers
{
    public interface IHeadersSerializer : ISerializer
    {
        public MessageHeader Deserialize(Type headersType, MessageHeaderTransport headersTransport);
        public ReadOnlySpan<byte> Serialize<THeaders>(THeaders headers) where THeaders : MessageHeader;
    }
}