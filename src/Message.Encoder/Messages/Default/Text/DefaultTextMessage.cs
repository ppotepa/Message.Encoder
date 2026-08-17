using Message.Encoder.Attributes;

namespace Message.Encoder.Messages.Default.Text
{
    [MessageType(MessageTypeCode = 1)]
    public class DefaultTextMessage : Message<DefaultTextMessageHeaders, DefaultTextMessagePayload>
    {
        public DefaultTextMessage(DefaultTextMessageHeaders headersFromTransports, DefaultTextMessagePayload payload)
            : base(headersFromTransports, payload)
        {
        }

        public DefaultTextMessage() { }
    }
}
