using Message.Encoder.Extensions;
using System;

// ReSharper disable InconsistentNaming
namespace Message.Encoder.Messages.Transport
{
    public ref struct MessageHeaderTransport
    {
        public readonly long HEADERS_LENGTH = default;
        public readonly long MSG_FROM = default;
        public readonly long MSG_TIMESTAMP = default;
        public readonly long MSG_TO = default;
        public readonly byte MSG_TYPE = default;
        public ReadOnlySpan<byte> ADDITIONAL_HEADERS_BYTES = default;

        public MessageHeaderTransport()
        {
            MSG_FROM = 0;
            MSG_TIMESTAMP = 0;
            MSG_TO = 0;
            MSG_TYPE = 0;
            HEADERS_LENGTH = 0;
            ADDITIONAL_HEADERS_BYTES = default;
        }

        private MessageHeaderTransport
        (
            long msgFrom, long msgTo, long msgTimestamp,
            byte msgType, long headersLength, ReadOnlySpan<byte> additionalHeadersBytes) : this()
        {
            MSG_FROM = msgFrom;
            MSG_TO = msgTo;
            MSG_TIMESTAMP = msgTimestamp;
            MSG_TYPE = msgType;
            HEADERS_LENGTH = headersLength;
            ADDITIONAL_HEADERS_BYTES = additionalHeadersBytes;
        }

        public static MessageHeaderTransport FromSpan(ReadOnlySpan<byte> messageSpan, long headersLength)
        {
            return new MessageHeaderTransport
            (
                msgFrom: messageSpan.GetMessageFrom(),
                msgTo: messageSpan.GetMessageTo(),
                msgTimestamp: messageSpan.GetMessageTimestamp(),
                msgType: messageSpan.GetMessageType(),
                headersLength: headersLength,
                additionalHeadersBytes: messageSpan.GetAllHeaders(headersLength)
            );
        }
    }
}