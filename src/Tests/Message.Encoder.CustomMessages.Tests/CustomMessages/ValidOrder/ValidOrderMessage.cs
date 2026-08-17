using Message.Encoder.Attributes;
using Message.Encoder.Messages;

namespace Message.Encoder.CustomMessages.Tests.CustomMessages.ValidOrder
{
    [MessageType(98)]
    internal class ValidOrderMessage : Message<ValidOrderHeader, ValidOrderPayload>
    {
        public ValidOrderMessage(ValidOrderHeader headers, ValidOrderPayload payload) : base(headers, payload)
        {
        }
    }
}
