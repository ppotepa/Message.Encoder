using Message.Encoder.Builders;
using Message.Encoder.Extensions;
using Message.Encoder.Messages.Transport;
using System.Collections.Generic;
using System.Linq;

namespace Message.Encoder.Messages
{
    public abstract class MessageHeader : IBuildable
    {
        internal readonly Dictionary<string, object> Headers = new();

        protected MessageHeader(params object[] headers)
        {
        }

        public long From { get; set; }

        public byte MessageType { get; set; }

        public long Timestamp { get; set; }

        public long To { get; set; }

        public long HeadersLength
            => this.Headers.Values.Select(x => x.ToByteArray().Length).Sum() + (this.Headers.Count * 2);

        internal virtual byte[] DefaultBytes => new object[] {
            From, To, Timestamp, MessageType
        }
        .SelectMany(@object => @object.ToByteArray())
        .ToArray();

        public object this[string index]
        {
            get => Headers[index];
            set
            {
                if (!Headers.ContainsKey(index))
                {
                    Headers[index] = value;
                }
            }
        }

        public virtual void Apply(MessageHeaderTransport headersTransport)
        {
            this.From = headersTransport.MSG_FROM;
            this.To = headersTransport.MSG_TO;
            this.MessageType = headersTransport.MSG_TYPE;
            this.Timestamp = headersTransport.MSG_TIMESTAMP;
            this.MessageType = headersTransport.MSG_TYPE;
        }
    }
}