using Message.Encoder.Attributes;
using Message.Encoder.Builders;
using Message.Encoder.Serializers.Default;

namespace Message.Encoder.Messages
{
    public abstract class Payload : IBuildable
    {
    }

    [UseSerializer(typeof(DefaultPayloadSerializer))]
    internal class EmptyPayload : Payload
    {

    }
}