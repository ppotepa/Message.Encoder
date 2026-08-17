using Message.Encoder.Extensions;
using Message.Encoder.Messages;
using Message.Encoder.Metadata.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Message.Encoder.Serializers.Default
{
    public class DefaultPayloadSerializer : IPayloadSerializer
    {
        private const int PropertyHeaderLength = 4;
        private static readonly byte[] EmptyBytesResult = { 0, 0 };

        private static readonly Dictionary<Type, SerializationMetadata[]> PayloadMetadata = AppDomain.CurrentDomain
                .GetSubclassesOf<Payload>()
                .ToDictionary(type => type, SerializationMetadata.Create);

        public Payload Deserialize(Type payloadType, ReadOnlySpan<byte> payloadSpan)
        {
            Payload payload = Activator.CreateInstance(payloadType) as Payload;

            if (payloadSpan.Length > 0)
            {
                DeserializeProperties(payloadSpan, payload);
            }

            return payload;
        }

        public ReadOnlySpan<byte> Serialize<TPayload>(TPayload payload)
            where TPayload : Payload
        {
            List<byte> listBytes = new List<byte>();

            if (payload is null)
                return EmptyBytesResult;

            var metadata = PayloadMetadata[payload.GetType()];

            foreach (var data in metadata)
            {
                var value = data.PropertyInfo.GetValue(payload);
                var array = value.ToByteArray();
                listBytes.AddRange((array.Length).ToByteArray(), array);
            }

            return listBytes.ToArray();
        }

        private void DeserializeProperties(ReadOnlySpan<byte> payloadBytes, Payload payload)
        {
            int start = 0;
            IEnumerable<SerializationMetadata> metadata = PayloadMetadata[payload.GetType()];

            foreach (SerializationMetadata data in metadata)
            {
                var currentPropertyLength = payloadBytes.Slice(start, PropertyHeaderLength).ToInt32();
                var currentPropertyBytes = payloadBytes.Slice(start + PropertyHeaderLength, currentPropertyLength);

                if (data.IsNullable)
                {
                    switch (data.Preactivated)
                    {
                        case bool: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToNullableBoolean()); break;
                        case long: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToNullableInt64()); break;
                        case int: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToNullableInt32()); break;
                        case short: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToNullableInt16()); break;
                        case byte: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToNullableInt8()); break;
                        case float: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToNullableSingle()); break;
                        case double: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToNullableDouble()); break;
                    }
                }
                else
                {
                    switch (data.Preactivated)
                    {
                        case bool: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToBoolean()); break;
                        case long: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToInt64()); break;
                        case int: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToInt32()); break;
                        case short: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToInt16()); break;
                        case byte: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToInt8()); break;
                        case float: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToSingle()); break;
                        case double: data.PropertyInfo.SetValue(payload, currentPropertyBytes.ToDouble()); break;
                        case string: data.PropertyInfo.SetValue(payload, currentPropertyBytes.GetString()); break;
                    }
                }

                start += PropertyHeaderLength + currentPropertyLength;
            }
        }
    }
}