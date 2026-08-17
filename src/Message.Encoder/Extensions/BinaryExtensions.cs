using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Message.Encoder.Extensions
{
    public static class BinaryExtensions
    {
        public static IEnumerable<byte> GetBytes(this object data)
            => data.ToByteArray();

        public static byte[] ToByteArray(this object @object)
        {
            return @object switch
            {
                long value => WriteInt64(value),
                int value => WriteInt32(value),
                short value => WriteInt16(value),
                byte value => new[] { value },
                string value => Encoding.ASCII.GetBytes(value),
                float value => WriteInt32(BitConverter.SingleToInt32Bits(value)),
                double value => WriteInt64(BitConverter.DoubleToInt64Bits(value)),
                bool value => new[] { value ? (byte)1 : (byte)0 },
                null => Array.Empty<byte>(),
                _ => throw new ArgumentException(
                    $"Argument was invalid. {@object.GetType().Name} is not supported.",
                    nameof(@object)
                )
            };
        }

        public static short ToInt16(this ReadOnlySpan<byte> @object)
        {
            EnsureLength(@object, sizeof(short));
            return BinaryPrimitives.ReadInt16LittleEndian(@object);
        }

        public static short ToInt16(this byte[] @object)
            => new ReadOnlySpan<byte>(@object).ToInt16();

        public static short? ToNullableInt16(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 0) return null;
            return @object.ToInt16();
        }

        public static int ToInt32(this ReadOnlySpan<byte> @object)
        {
            EnsureLength(@object, sizeof(int));
            return BinaryPrimitives.ReadInt32LittleEndian(@object);
        }

        public static int ToInt32(this byte[] @object)
            => new ReadOnlySpan<byte>(@object).ToInt32();

        public static int? ToNullableInt32(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 0) return null;
            return @object.ToInt32();
        }

        public static long ToInt64(this ReadOnlySpan<byte> @object)
        {
            EnsureLength(@object, sizeof(long));
            return BinaryPrimitives.ReadInt64LittleEndian(@object);
        }

        public static long ToInt64(this byte[] @object)
            => new ReadOnlySpan<byte>(@object).ToInt64();

        public static long? ToNullableInt64(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 0) return null;
            return @object.ToInt64();
        }

        public static string GetString(this ReadOnlySpan<byte> @object)
            => @object.Length is 0 ? string.Empty : Encoding.ASCII.GetString(@object);

        public static float ToSingle(this ReadOnlySpan<byte> @object)
            => BitConverter.Int32BitsToSingle(@object.ToInt32());

        public static float? ToNullableSingle(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 0) return null;
            return @object.ToSingle();
        }

        public static bool ToBoolean(this ReadOnlySpan<byte> @object)
        {
            EnsureLength(@object, sizeof(byte));
            return @object[0] != 0;
        }

        public static bool? ToNullableBoolean(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 0) return null;
            return @object.ToBoolean();
        }

        public static double ToDouble(this ReadOnlySpan<byte> @object)
            => BitConverter.Int64BitsToDouble(@object.ToInt64());

        public static double? ToNullableDouble(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 0) return null;
            return @object.ToDouble();
        }

        public static byte ToInt8(this ReadOnlySpan<byte> span)
        {
            EnsureLength(span, sizeof(byte));
            return span[0];
        }

        public static byte? ToNullableInt8(this ReadOnlySpan<byte> span)
        {
            if (span.Length is 0) return null;
            return span.ToInt8();
        }

        private static byte[] WriteInt16(short value)
        {
            var bytes = new byte[sizeof(short)];
            BinaryPrimitives.WriteInt16LittleEndian(bytes, value);
            return bytes;
        }

        private static byte[] WriteInt32(int value)
        {
            var bytes = new byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(bytes, value);
            return bytes;
        }

        private static byte[] WriteInt64(long value)
        {
            var bytes = new byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(bytes, value);
            return bytes;
        }

        private static void EnsureLength(ReadOnlySpan<byte> bytes, int requiredLength)
        {
            if (bytes.Length != requiredLength)
            {
                throw new ArgumentException($"Required Span Length is {requiredLength}");
            }
        }
    }
}
