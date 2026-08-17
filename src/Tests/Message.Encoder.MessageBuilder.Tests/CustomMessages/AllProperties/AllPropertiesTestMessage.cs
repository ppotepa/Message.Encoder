using Message.Encoder.Attributes;
using Message.Encoder.Messages;

namespace Message.Encoder.MessageBuilder.Tests.CustomMessages.AllProperties
{
    [MessageType(150)]
    internal class AllPropertiesTestMessage : Message<AllPropertiesHeader, AllPropertiesPayload>
    {
        public AllPropertiesTestMessage(AllPropertiesHeader headersFromTransports, AllPropertiesPayload payload)
            : base(headersFromTransports, payload) { }

        public AllPropertiesTestMessage() { }
    }

    internal class AllPropertiesPayload : Payload
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

        [SerializationOrder(Order = 9, PropertyName = "nullable-byte")]
        public byte? NullableByte { get; init; }

        [SerializationOrder(Order = 10, PropertyName = "nullable-short")]
        public short? NullableShort { get; set; }

        [SerializationOrder(Order = 11, PropertyName = "nullable-int")]
        public int? NullableInt { get; set; }

        [SerializationOrder(Order = 12, PropertyName = "nullable-long")]
        public long? NullableLong { get; set; }

        [SerializationOrder(Order = 13, PropertyName = "nullable-float")]
        public float? NullableFloat { get; set; }

        [SerializationOrder(Order = 14, PropertyName = "nullable-double")]
        public double? NullableDouble { get; set; }

        [SerializationOrder(Order = 15, PropertyName = "nullable-boolean")]
        public bool? NullableBoolean { get; set; }

        [SerializationOrder(Order = 16, PropertyName = "nullable-null")]
        public int? NullProperty { get; set; }
    }

    internal class AllPropertiesHeader : MessageHeader
    {
        [SerializationOrder(Order = 1, PropertyName = "header-1")]
        public string Header1 { get; init; }
    }
}
