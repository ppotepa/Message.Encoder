using Message.Encoder.Attributes;
using Message.Encoder.Messages;

namespace Message.Encoder.MessageBuilder.Tests.CustomMessages.Default
{
    [MessageType(100)]
    internal class TestMessage : Message<TestMessageHeader, TestMessagePayload>
    {
        public TestMessage(TestMessageHeader headersFromTransports, TestMessagePayload payload)
            : base(headersFromTransports, payload) { }

        public TestMessage() { }

    }
}
