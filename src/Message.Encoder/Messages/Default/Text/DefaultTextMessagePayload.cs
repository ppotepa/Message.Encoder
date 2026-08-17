using Message.Encoder.Attributes;
using Message.Encoder.Serializers.Default;

namespace Message.Encoder.Messages.Default.Text
{
    [UseSerializer(typeof(DefaultPayloadSerializer))]
    public class DefaultTextMessagePayload : Payload
    {
        public DefaultTextMessagePayload()
        {
        }

        [SerializationOrder(Order = 1, PropertyName = nameof(TextMessageBody))]
        public string TextMessageBody { get; set; }
    }
}