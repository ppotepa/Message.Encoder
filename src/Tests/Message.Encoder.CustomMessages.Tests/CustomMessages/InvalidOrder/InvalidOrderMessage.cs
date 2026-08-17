using Message.Encoder.Attributes;
using Message.Encoder.Messages;

namespace Message.Encoder.CustomMessages.Tests.CustomMessages.InvalidOrder
{
    [MessageType(99)]
    internal class InvalidOrderMessage : Message<InvalidOrderHeader, InvalidOrderPayload>
    {
        public InvalidOrderMessage(InvalidOrderHeader headers, InvalidOrderPayload payload) : base(headers, payload)
        {
        }
    }
}
