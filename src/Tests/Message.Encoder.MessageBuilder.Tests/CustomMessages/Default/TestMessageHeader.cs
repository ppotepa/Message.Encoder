using Message.Encoder.Attributes;
using Message.Encoder.Messages;

namespace Message.Encoder.MessageBuilder.Tests.CustomMessages.Default
{
    internal class TestMessageHeader : MessageHeader
    {
        [SerializationOrder(Order = 1, PropertyName = "byte")]
        public byte Byte { get; init; }

        [SerializationOrder(Order = 2, PropertyName = "short")]
        public short Short { get; set; }

        [SerializationOrder(Order = 3, PropertyName = "int")]
        public int Int { get; set; }

        [SerializationOrder(Order = 4, PropertyName = "long")]
        public long Long { get; set; }

        [SerializationOrder(Order = 5, PropertyName = "string")]
        public string String { get; set; }

        [SerializationOrder(Order = 6, PropertyName = "float")]
        public float Float { get; set; }

        [SerializationOrder(Order = 7, PropertyName = "double")]
        public double Double { get; set; }

        [SerializationOrder(Order = 8, PropertyName = "boolean")]
        public bool Boolean { get; set; }
    }
}