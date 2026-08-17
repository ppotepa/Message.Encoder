using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Message.Encoder.Extensions
{
    public static class BinaryExtensions
    {
        public static IEnumerable<byte> GetBytes(this object data)
        {
            return data switch
            {
                // Out of some reason BitConverter interprets single byte as 2-byte digit ?? 
                byte @byte => BitConverter.GetBytes((short)@byte).Take(1),
                short @short => BitConverter.GetBytes(@short),
                int @int => BitConverter.GetBytes(@int),
                long @long => BitConverter.GetBytes(@long),
                float @float => BitConverter.GetBytes(@float),
                double @double => BitConverter.GetBytes(@double),
                bool @bool => BitConverter.GetBytes(@bool),
                string @string => System.Text.Encoding.ASCII.GetBytes(@string),
                null => Array.Empty<byte>(),
                _ => throw new ArgumentException($"Argument was invalid. {data.GetType().Name} is not supported.", $"{nameof(data)}", null)
            };
        }

        public static byte[] ToByteArray(this object @object)
        {
            return @object switch
            {
                long @long => __toBytearrayLong(@long),
                int @int => __toByteArrayInt(@int),
                short @short => __toByteArrayShort(@short),
                byte @byte => __toByteArrayByte(@byte),
                string @string => __toByteArrayString(@string),
                float @float => __toByteArraySingle(@float),
                double @double => __toByteArrayDouble(@double),
                bool @boolean => new[] { @boolean ? (byte)1 : (byte)0 },
                null => Array.Empty<byte>(),
                _ => throw new ArgumentException($"Argument was invalid. {@object.GetType().Name} is not supported.", $"{nameof(@object)}", null)
            };
        }

        public static short ToInt16(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 2) return (short)(@object[0] | (@object[1] << 8));
            throw new ArgumentException($"Required Span Length is 2");
        }

        public static short ToInt16(this byte[] @object)
        {
            if (@object.Length is 2) return (short)(@object[0] | (@object[1] << 8));
            throw new ArgumentException($"Required Span Length is 2");
        }

        public static short? ToNullableInt16(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 2) return (short)(@object[0] | (@object[1] << 8));
            if (@object.Length is 0) return null;
            throw new ArgumentException($"Required Span Length is 2");
        }

        public static int ToInt32(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 4) return @object[0] | @object[1] << 8 | @object[2] << 16 | (@object[3] << 24);
            throw new ArgumentException($"Required Span Length is 4");
        }

        public static int ToInt32(this byte[] @object)
        {
            if (@object.Length is 4) return @object[0] | @object[1] << 8 | @object[2] << 16 | (@object[3] << 24);
            throw new ArgumentException($"Required Span Length is 4");
        }

        public static int? ToNullableInt32(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 4) return @object[0] | @object[1] << 8 | @object[2] << 16 | (@object[3] << 24);
            if (@object.Length is 0) return null;
            throw new ArgumentException($"Required Span Length is 4");
        }

        public static long ToInt64(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 8) return BitConverter.ToInt64(@object);
            throw new ArgumentException($"Required Span Length is 8");
        }

        public static long ToInt64(this byte[] @object)
        {
            if (@object.Length is 8) return BitConverter.ToInt64(@object);
            throw new ArgumentException($"Required Span Length is 8");
        }

        public static long? ToNullableInt64(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 8) return BitConverter.ToInt64(@object);
            if (@object.Length is 0) return null;
            throw new ArgumentException($"Required Span Length is 8");
        }

        public static string GetString(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length is 0) return string.Empty;
            return Encoding.ASCII.GetString(@object);
        }

        public static float ToSingle(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length == 4) return BitConverter.ToSingle(@object);
            throw new ArgumentException("Required Span Length is 4");
        }

        public static float? ToNullableSingle(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length == 4) return BitConverter.ToSingle(@object);
            if (@object.Length is 0) return null;
            throw new ArgumentException("Required Span Length is 4");
        }

        public static bool ToBoolean(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length == 1) return BitConverter.ToBoolean(@object);
            throw new ArgumentException("Required Span Length is 1");
        }

        public static bool? ToNullableBoolean(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length == 1) return BitConverter.ToBoolean(@object);
            if (@object.Length is 0) return null;
            throw new ArgumentException("Required Span Length is 1");
        }

        public static double ToDouble(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length == 8) return BitConverter.ToDouble(@object);
            throw new ArgumentException("Required Span Length is 8");
        }

        public static double? ToNullableDouble(this ReadOnlySpan<byte> @object)
        {
            if (@object.Length == 8) return BitConverter.ToDouble(@object);
            if (@object.Length is 0) return null;
            throw new ArgumentException("Required Span Length is 8");
        }

        public static byte ToInt8(this ReadOnlySpan<byte> span)
        {
            if (span.Length == 1) return span[0];
            throw new ArgumentException("Required Span Length is 1");
        }

        public static byte? ToNullableInt8(this ReadOnlySpan<byte> span)
        {
            if (span.Length == 1) return span[0];
            if (span.Length is 0) return null;
            throw new ArgumentException("Required Span Length is 1");
        }

        private static byte[] __toByteArrayByte(this byte @object)
        {
            return new[] { @object };
        }

        private static byte[] __toByteArrayInt(this int @object)
        {
            return new[]
            {
                (byte)@object,
                (byte)(@object >> 8),
                (byte)(@object >> 16),
                (byte)(@object >> 24)
            };
        }

        private static byte[] __toByteArraySingle(this float @object)
        {
            return BitConverter.GetBytes(@object);
        }

        private static byte[] __toByteArrayDouble(this double @object)
        {
            return BitConverter.GetBytes(@object);
        }

        private static byte[] __toBytearrayLong(this long @object)
        {
            return new[] {
                (byte)@object,
                (byte)(@object >> 8),
                (byte)(@object >> 16),
                (byte)(@object >> 24),
                (byte)(@object >> 32),
                (byte)(@object >> 40),
                (byte)(@object >> 48),
                (byte)(@object >> 54)
            };
        }

        private static byte[] __toByteArrayShort(this short @object)
        {
            return new[] {
                (byte)@object,
                (byte)(@object >> 8)
            };
        }

        private static byte[] __toByteArrayString(this string @object)
        {
            void Void() {
                //this is void, intentionally left empty
            }

            var bytes = new byte[@object.Length];
            for (var index = 0; index < @object.Length; bytes[index] = (byte)@object[index++]) Void();
            return bytes;
        }
    }
}
