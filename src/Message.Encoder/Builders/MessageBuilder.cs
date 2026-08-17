using Message.Encoder.Attributes;
using Message.Encoder.Exceptions;
using Message.Encoder.Extensions;
using Message.Encoder.Messages;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Message.Encoder.Builders
{
    public class MessageBuilder<THeaders, TPayload> : ISetFromBuildingStep<THeaders, TPayload>,
                                                        ISetTimeStampBuildingStep<THeaders, TPayload>,
                                                        ISetToBuildingStep<THeaders, TPayload>,
                                                        ISetMsgTypeBuildingStep<THeaders, TPayload>,
                                                        IAddHeaderBuildingStep<THeaders, TPayload>,
                                                        IAddPayloadPropertyStep<THeaders, TPayload>
        where THeaders : MessageHeader, new()
        where TPayload : Payload, new()
    {
        private readonly List<byte> _defaultBytes = default;
        private readonly List<KeyValuePair<string, object>> _additionalHeaders = default;
        private readonly List<KeyValuePair<string, object>> _payloadProperties = default;

        private long _additionalHeadersLength = 0;
        private long _payloadPropertiesLength = 0;

        private readonly int _maxProperties = default;
        private readonly int _maxHeaders = default;

        private readonly SerializationOrderAttribute[] _headersAttributes = typeof(THeaders).GetProperties()
            .Where(property => property.GetCustomAttribute<SerializationOrderAttribute>() is not null)
            .Select(property => property.GetCustomAttribute<SerializationOrderAttribute>())
            .OrderBy(attr => attr!.Order)
            .ToArray();

        private readonly SerializationOrderAttribute[] _payloadAttributes = typeof(TPayload).GetProperties()
            .Where(property => property.GetCustomAttribute<SerializationOrderAttribute>() is not null)
            .Select(property => property.GetCustomAttribute<SerializationOrderAttribute>())
            .OrderBy(attr => attr!.Order)
            .ToArray();

        private MessageBuilder()
        {
            this._maxHeaders = _headersAttributes.Length;
            this._maxProperties = _payloadAttributes.Length;

            this._defaultBytes = new List<byte>(28);
            this._additionalHeaders = new List<KeyValuePair<string, object>>();
            this._payloadProperties = new List<KeyValuePair<string, object>>();
        }

        public static ISetFromBuildingStep<THeaders, TPayload> CreateBuilder()
        {
            return new MessageBuilder<THeaders, TPayload>();
        }

        public ISetToBuildingStep<THeaders, TPayload> From(long from)
        {
            this._defaultBytes.AddRange(from.ToByteArray());
            return this;
        }

        public ISetTimeStampBuildingStep<THeaders, TPayload> To(long to)
        {
            this._defaultBytes.AddRange(to.ToByteArray());
            return this;
        }

        public IAddHeaderBuildingStep<THeaders, TPayload> AddHeader<THeaderType>(string name, THeaderType value)
        {
            if (_additionalHeaders.Count < this._maxHeaders)
            {
                string expectedHeaderName = _headersAttributes[_additionalHeaders.Count].PropertyName;

                if (expectedHeaderName == name)
                {
                    this._additionalHeaders.Add(new KeyValuePair<string, object>(name, value));
                    var currentValueByteLength = value.ToByteArray().Length;
                    this._additionalHeadersLength += (2 + currentValueByteLength);
                    return this;
                }

                throw new InvalidHeaderNameException($"Header name was invalid. Expected {expectedHeaderName} was {name}.");

            }

            throw new HeadersCountExceededException(
                $"Header count exceeded. Allowed {_maxHeaders}, current was {_maxHeaders + 1}");
        }

        public IAddPayloadPropertyStep<THeaders, TPayload> EndHeaders()
        {
            return this;
        }

        public ISetMsgTypeBuildingStep<THeaders, TPayload> Timestamp(long timestamp)
        {
            this._defaultBytes.AddRange(timestamp.ToByteArray());
            return this;
        }

        public IAddHeaderBuildingStep<THeaders, TPayload> MsgType(byte msgType)
        {
            this._defaultBytes.AddRange(msgType.ToByteArray());
            return this;
        }

        public IAddPayloadPropertyStep<THeaders, TPayload> AddPayloadProperty<TType>(string propertyName, TType property)
        {
            if (_payloadProperties.Count < this._maxProperties)
            {
                this._payloadProperties.Add(new KeyValuePair<string, object>(propertyName, property));
                var currentValueByteLength = property.ToByteArray().Length;
                this._payloadPropertiesLength += (2 + currentValueByteLength);
                return this;
            }

            throw new PayloadPropertiesCountExceeded(
                $"Invalid amount of properties supplied. Allowed {_maxProperties}, current was {_maxProperties + 1}");
        }

        public Message Build()
        {
            if (_headersAttributes.Length > _additionalHeaders.Count)
            {
                throw new InvalidAmountOfHeadersSuppliedException($"Invalid amount of headers supplied for {typeof(THeaders).Name}. " +
                    $"Expected {_headersAttributes.Length} found {_additionalHeaders.Count}"
                );
            }

            if (_payloadAttributes.Length > _payloadProperties.Count)
            {
                throw new InvalidAmountOfPayloadPropertiesSuppliedException($"Invalid amount of payload properties supplied for {typeof(TPayload).Name}. " +
                                                                  $"Expected {_payloadAttributes.Length} found {_payloadProperties.Count}");
            }

            byte[] bytes = GetBinary();
            return Message.FromBytes(bytes);
        }

        public byte[] GetBinary()
        {
            byte[] additionalHeaderBytes = _additionalHeaders.Select(pair =>
                {
                    byte[] valueBytes = pair.Value.ToByteArray();
                    byte[] valueByteLengthArray = ((short)valueBytes.Length).ToByteArray();
                    return valueByteLengthArray.Concat(valueBytes);
                })
                .SelectMany(bytes => bytes)
                .ToArray();

            byte[] payloadBytes = _payloadProperties.Select(pair =>
                {
                    byte[] valueBytes = pair.Value.ToByteArray();
                    byte[] valueByteLengthArray = ((int)valueBytes.Length).ToByteArray();
                    return valueByteLengthArray.Concat(valueBytes);
                })
                .SelectMany(bytes => bytes)
                .ToArray();

            this._defaultBytes.AddRange(_additionalHeadersLength.ToByteArray());
            this._defaultBytes.AddRange(additionalHeaderBytes);
            this._defaultBytes.AddRange(payloadBytes);

            return _defaultBytes.ToArray();
        }
    }

    public interface ISetMsgTypeBuildingStep<TH, TP>
        where TH : MessageHeader, new()
        where TP : Payload, new()
    {
        IAddHeaderBuildingStep<TH, TP> MsgType(byte msgType);
    }

    public interface ISetTimeStampBuildingStep<TH, TP>
        where TH : MessageHeader, new()
        where TP : Payload, new()
    {
        ISetMsgTypeBuildingStep<TH, TP> Timestamp(long timestamp);
    }

    public interface ISetFromBuildingStep<TH, TP>
        where TH : MessageHeader, new()
        where TP : Payload, new()
    {
        ISetToBuildingStep<TH, TP> From(long from);
    }

    public interface ISetToBuildingStep<TH, TP>
        where TH : MessageHeader, new()
        where TP : Payload, new()
    {
        ISetTimeStampBuildingStep<TH, TP> To(long to);
    }

    public interface IAddHeaderBuildingStep<TH, TP>
        where TH : MessageHeader, new()
        where TP : Payload, new()
    {
        IAddHeaderBuildingStep<TH, TP> AddHeader<THeaderType>(string name, THeaderType value);
        IAddPayloadPropertyStep<TH, TP> EndHeaders();
    }

    public interface IAddPayloadPropertyStep<THeaders, TPayload>
        where THeaders : MessageHeader, new()
        where TPayload : Payload, new()
    {
        IAddPayloadPropertyStep<THeaders, TPayload> AddPayloadProperty<TType>(string propertyName, TType property);
        Message Build();
        byte[] GetBinary();
    }
}

