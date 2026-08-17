using Message.Encoder.Attributes;
using Message.Encoder.Messages;

namespace Message.Encoder.MessageBuilder.Tests.CustomMessages.Default
{
    internal class TestMessagePayload : Payload
    {
        [SerializationOrder(Order = 1, PropertyName = nameof(TestTextBody))]
        public string TestTextBody { get; set; }
    }
}