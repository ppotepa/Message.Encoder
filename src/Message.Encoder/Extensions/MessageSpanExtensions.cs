using System;
// ReSharper disable InconsistentNaming

namespace Message.Encoder.Extensions
{
    internal static class MessageSpanExtensions
    {
        private const int LONG_LENGTH = 8;
        private const int MSG_TYPE_INDEX = 24;
        private const int HEADERS_LENGTH_FIRST_BYTE_INDEX = 25;

        public static long GetMessageFrom(this ReadOnlySpan<byte> messageSpan)
            => messageSpan[..LONG_LENGTH].ToInt64();

        public static long GetMessageTo(this ReadOnlySpan<byte> messageSpan)
            => messageSpan[LONG_LENGTH..(LONG_LENGTH * 2)].ToInt64();

        public static long GetMessageTimestamp(this ReadOnlySpan<byte> messageSpan)
            => messageSpan[(LONG_LENGTH * 2)..MSG_TYPE_INDEX].ToInt64();

        public static long GetMessageHeadersLength(this ReadOnlySpan<byte> messageSpan)
            => messageSpan[HEADERS_LENGTH_FIRST_BYTE_INDEX..(HEADERS_LENGTH_FIRST_BYTE_INDEX + LONG_LENGTH)].ToInt64();

        public static byte GetMessageType(this ReadOnlySpan<byte> messageSpan)
            => messageSpan[MSG_TYPE_INDEX];

        public static ReadOnlySpan<byte> GetAllHeaders(this ReadOnlySpan<byte> messageSpan, long headersLength)
        {
            try
            {
                return messageSpan[(HEADERS_LENGTH_FIRST_BYTE_INDEX + LONG_LENGTH)..
                    checked((int)headersLength + HEADERS_LENGTH_FIRST_BYTE_INDEX + LONG_LENGTH)];
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException or OverflowException)
            {
                throw new InvalidHeadersLengthException(
                    $"Headers length was invalid. Message length: {messageSpan.Length}, headers length supplied: {headersLength}", ex);
            }
        }
    }
}
