using System;
using System.Runtime.InteropServices;
// ReSharper disable InconsistentNaming

namespace Message.Encoder.Extensions
{
    internal static class MessageSpanExtensions
    {
        private const int LONG_LENGTH = 8;
        private const int MSG_TYPE_INDEX = 24;
        private const int HEADERS_LENGTH_FIRST_BYTE_INDEX = 25;

        public static long GetMessageFrom(this ReadOnlySpan<byte> messageSpan)
            => MemoryMarshal.Read<long>(messageSpan[..LONG_LENGTH]);

        public static long GetMessageTo(this ReadOnlySpan<byte> messageSpan) =>
            MemoryMarshal.Read<long>(messageSpan[LONG_LENGTH..(LONG_LENGTH * 2)]);


        public static long GetMessageTimestamp(this ReadOnlySpan<byte> messageSpan) =>
            MemoryMarshal.Read<long>(messageSpan[(LONG_LENGTH * 2)..MSG_TYPE_INDEX]);

        public static long GetMessageHeadersLength(this ReadOnlySpan<byte> messageSpan) =>
            MemoryMarshal.Read<long>(messageSpan[HEADERS_LENGTH_FIRST_BYTE_INDEX..(HEADERS_LENGTH_FIRST_BYTE_INDEX + 8)]);

        public static byte GetMessageType(this ReadOnlySpan<byte> messageSpan)
            => messageSpan[MSG_TYPE_INDEX];

        public static ReadOnlySpan<byte> GetAllHeaders(this ReadOnlySpan<byte> messageSpan, long headersLength)
        {
            try
            {
                return messageSpan[(HEADERS_LENGTH_FIRST_BYTE_INDEX + 8)..((int)headersLength + (HEADERS_LENGTH_FIRST_BYTE_INDEX + 8))];
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new InvalidHeadersLengthException(
                    $"Headers length was invalid. Message length : {messageSpan.Length}, headersLengthSupplied : {headersLength}", ex);
            }
        }
    }
}
