using Message.Encoder.Extensions;
using System;

// ReSharper disable InconsistentNaming

namespace Message.Encoder.Messages.Transport
{
    public ref struct MessageTransport
    {
        public const int DEFAULT_UNCOMPRESSED_PAYLOAD_SIZE_TRANSPORT = 1024 * 1024 * 16;
        public ReadOnlySpan<byte> BinaryPayload = default;
        public MessageHeaderTransport HeaderTransportInfo = new();

        public MessageTransport() { }

        public static MessageTransport FromSpan(ReadOnlySpan<byte> messageSpan)
        {
            long headersLength = messageSpan.GetMessageHeadersLength();

            MessageTransport result = new()
            {
                HeaderTransportInfo = MessageHeaderTransport.FromSpan(messageSpan, headersLength),
                BinaryPayload = messageSpan[(int)(25 + 8 + headersLength)..messageSpan.Length]
            };

            return result;
        }
    }
}