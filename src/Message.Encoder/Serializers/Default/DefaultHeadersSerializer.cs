using Message.Encoder.Exceptions;
using Message.Encoder.Extensions;
using Message.Encoder.Messages;
using Message.Encoder.Messages.Transport;
using Message.Encoder.Metadata.Serialization;
using System;
using System.Linq;
using System.Text;

namespace Message.Encoder.Serializers.Default
{
    public class DefaultHeadersSerializer : IHeadersSerializer
    {
        private const int PropertyHeaderLength = 2;

        public MessageHeader Deserialize(Type headersType, MessageHeaderTransport headersTransport)
        {
            int start = 0;

            if (Activator.CreateInstance(headersType) is MessageHeader headers)
            {
                headers.Apply(headersTransport);
                ReadOnlySpan<byte> headerBytes = headersTransport.ADDITIONAL_HEADERS_BYTES;

                var headersCount = CountHeaders(headerBytes);
                SerializationMetadata[] metadata = SerializationMetadata.Create(headersType);

                if (headersCount != metadata.Length)
                {
                    throw new InvalidAmountOfHeadersFound(
                        $"Invalid amount of headers found. Message does not match type signature. " +
                        $"Expected {metadata.Length}, found {headersCount}. Header type {headersType.Name}."
                    );
                }

                foreach (var data in metadata)
                {
                    var currentPropertyLength = headerBytes.Slice(start, PropertyHeaderLength).ToInt16();
                    var currentHeaderBytes = headerBytes.Slice(start + PropertyHeaderLength, currentPropertyLength);

                    object @value = null;

                    if (data.PropertyInfo.PropertyType == typeof(string))
                    {
                        value = Encoding.ASCII.GetString(currentHeaderBytes);
                    }

                    if (data.PropertyInfo.PropertyType == typeof(bool))
                    {
                        value = currentHeaderBytes.ToBoolean();
                    }

                    if (data.PropertyInfo.PropertyType == typeof(float))
                    {
                        value = currentHeaderBytes.ToSingle();
                    }

                    if (data.PropertyInfo.PropertyType == typeof(double))
                    {
                        value = currentHeaderBytes.ToDouble();
                    }

                    if (data.PropertyInfo.PropertyType == typeof(long))
                    {
                        value = currentHeaderBytes.ToInt64();
                    }

                    if (data.PropertyInfo.PropertyType == typeof(int))
                    {
                        value = currentHeaderBytes.ToInt32();
                    }

                    if (data.PropertyInfo.PropertyType == typeof(short))
                    {
                        value = currentHeaderBytes.ToInt16();
                    }

                    if (data.PropertyInfo.PropertyType == typeof(byte))
                    {
                        value = currentHeaderBytes.ToInt8();
                    }

                    data.PropertyInfo.SetValue(headers, value);

                    if (value is not null)
                    {
                        headers[data.Attribute.PropertyName] = value;
                    }

                    start += PropertyHeaderLength + currentPropertyLength;
                }
            }
            else
                throw new InvalidOperationException($"{headersType.Name} is not valid Header Type.");

            return headers;
        }

        private static int CountHeaders(ReadOnlySpan<byte> headerBytes)
        {
            var headersCount = 0;

            for (var propertyStart = 0; propertyStart < headerBytes.Length;)
            {
                var currentLength = headerBytes.Slice(propertyStart, PropertyHeaderLength).ToInt16();

                if (currentLength < 0)
                {
                    throw new InvalidHeadersLengthException(
                        $"Header property length cannot be negative: {currentLength}.",
                        new ArgumentOutOfRangeException(nameof(currentLength))
                    );
                }

                headersCount++;
                propertyStart += PropertyHeaderLength + currentLength;
            }

            return headersCount;
        }

        public ReadOnlySpan<byte> Serialize<THeaders>(THeaders headers)
            where THeaders : MessageHeader
        {
            long headersLength = 0;
            var metadata = SerializationMetadata.Create(headers.GetType());
            var result = metadata.Select(data =>
            {
                byte[] current = data.PropertyInfo.GetValue(headers).ToByteArray();

                if (current.Length > short.MaxValue)
                {
                    throw new InvalidHeadersLengthException(
                        $"Header property {data.PropertyInfo.Name} is too large. Maximum length is {short.MaxValue} bytes.",
                        new ArgumentOutOfRangeException(data.PropertyInfo.Name)
                    );
                }

                headersLength += PropertyHeaderLength + current.Length;
                return ((short)current.Length).ToByteArray().Concat(current);
            })
            .SelectMany(obj => obj)
            .ToArray();

            return headers.DefaultBytes.Concat(headersLength.ToByteArray()).Concat(result).ToArray();
        }
    }
}
