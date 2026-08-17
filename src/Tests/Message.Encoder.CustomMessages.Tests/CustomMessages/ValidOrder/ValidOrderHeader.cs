using Message.Encoder.Attributes;
using Message.Encoder.Messages;

namespace Message.Encoder.CustomMessages.Tests.CustomMessages.ValidOrder
{
    internal class ValidOrderHeader : MessageHeader
    {
        [SerializationOrder(Order = 1, PropertyName = "header-1")]
        public string Header1 { get; init; }
    }
}