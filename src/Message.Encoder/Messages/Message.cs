using Message.Encoder.Factories.Messages;
using System;

namespace Message.Encoder.Messages
{
    public abstract class Message
    {
        protected object _headers;
        protected object _payload;

        protected Message(object headersFromTransports, object payload)
        {
            this._headers = headersFromTransports ?? throw new ArgumentNullException(nameof(headersFromTransports));
            this._payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        protected Message()
        {
        }

        public virtual object Headers
        {
            get => _headers;
            set => _headers = value;
        }

        public virtual object Payload
        {
            get => _payload;
            set => _payload = value;
        }

        public static Message FromBytes(byte[] messageBinary)
        {
            return MessageFactory.Create(messageBinary);
        }

        public static Message FromBytes(ReadOnlySpan<byte> messageBinary)
        {
            return MessageFactory.Create(messageBinary);
        }

        public static ReadOnlySpan<byte> ToBinary(Message message)
        {
            return MessageFactory.Serialize(message);
        }
    }

    public abstract class Message<THeadersType, TPayloadType> : Message
        where TPayloadType : Payload, new()
        where THeadersType : MessageHeader, new()
    {
        protected Message(THeadersType headersFromTransports, TPayloadType payload) : base(headersFromTransports, payload) { }

        protected Message() { }

        public override THeadersType Headers => base.Headers as THeadersType;
        public override TPayloadType Payload => base.Payload as TPayloadType;
    }
}
