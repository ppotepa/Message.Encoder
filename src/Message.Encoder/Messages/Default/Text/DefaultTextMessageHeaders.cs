using Message.Encoder.Attributes;
using Message.Encoder.Serializers.Default;

namespace Message.Encoder.Messages.Default.Text
{
    [UseSerializer(typeof(DefaultHeadersSerializer))]
    public class DefaultTextMessageHeaders : MessageHeader
    {
        [SerializationOrder(Order = 1, PropertyName = "recipient-name")]
        public string RecipientName { get; set; }

        [SerializationOrder(Order = 2, PropertyName = "sender-name")]
        public string SenderName { get; init; }

        [SerializationOrder(Order = 3, PropertyName = "is-message-unread")]
        public bool IsMessageUnread { get; set; }
    }
}
