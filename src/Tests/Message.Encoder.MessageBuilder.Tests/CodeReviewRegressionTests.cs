using Message.Encoder.Attributes;
using Message.Encoder.Builders;
using Message.Encoder.Exceptions;
using Message.Encoder.Extensions;
using Message.Encoder.Messages;
using Message.Encoder.Messages.Default.Text;
using Message.Encoder.Metadata.Serialization;
using NUnit.Framework;
using System.Linq;

namespace Message.Encoder.MessageBuilder.Tests
{
    public class CodeReviewRegressionTests
    {
        [Test]
        public void Long_Conversion_Preserves_The_Highest_Byte()
        {
            Assert.Multiple(() =>
            {
                Assert.That(long.MaxValue.ToByteArray(), Is.EqualTo(new byte[] { 255, 255, 255, 255, 255, 255, 255, 127 }));
                Assert.That(long.MinValue.ToByteArray(), Is.EqualTo(new byte[] { 0, 0, 0, 0, 0, 0, 0, 128 }));
                Assert.That(0x4000000000000000L.ToByteArray(), Is.EqualTo(new byte[] { 0, 0, 0, 0, 0, 0, 0, 64 }));
            });
        }

        [Test]
        public void Serialization_Metadata_Uses_Attribute_Order_Not_Declaration_Order()
        {
            var metadata = SerializationMetadata.Create(typeof(OutOfDeclarationOrderPayload));

            Assert.That(metadata.Select(item => item.Attribute.Order), Is.EqualTo(new[] { 1, 2 }));
            Assert.That(metadata.Select(item => item.PropertyInfo.Name), Is.EqualTo(new[] { nameof(OutOfDeclarationOrderPayload.First), nameof(OutOfDeclarationOrderPayload.Second) }));
        }

        [Test]
        public void Builder_Rejects_Invalid_Payload_Property_Name()
        {
            var payloadStep = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>.CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(3)
                .MsgType(1)
                .AddHeader("recipient-name", "Bob")
                .AddHeader("sender-name", "Alice")
                .AddHeader("is-message-unread", true)
                .EndHeaders();

            Assert.Throws<InvalidPayloadPropertyNameException>(
                () => payloadStep.AddPayloadProperty("wrong-property-name", "Hello")
            );
        }

        [Test]
        public void GetBinary_Is_Idempotent()
        {
            var finalStep = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>.CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(3)
                .MsgType(1)
                .AddHeader("recipient-name", "Bob")
                .AddHeader("sender-name", "Alice")
                .AddHeader("is-message-unread", true)
                .EndHeaders()
                .AddPayloadProperty(nameof(DefaultTextMessagePayload.TextMessageBody), "Hello");

            var first = finalStep.GetBinary();
            var second = finalStep.GetBinary();

            Assert.That(second, Is.EqualTo(first));
        }

        [Test]
        public void HeadersLength_Is_Consistent_For_Manual_And_Deserialized_Headers()
        {
            var original = new DefaultTextMessage
            {
                Headers = new DefaultTextMessageHeaders
                {
                    From = 1,
                    To = 2,
                    Timestamp = 3,
                    MessageType = 1,
                    RecipientName = "Bob",
                    SenderName = "Alice",
                    IsMessageUnread = true
                },
                Payload = new DefaultTextMessagePayload
                {
                    TextMessageBody = "Hello"
                }
            };

            var deserialized = Message.FromBytes(Message.ToBinary(original)) as DefaultTextMessage;

            Assert.That(deserialized, Is.Not.Null);
            Assert.That(deserialized!.Headers.HeadersLength, Is.EqualTo(original.Headers.HeadersLength));
        }

        private class OutOfDeclarationOrderPayload : Payload
        {
            [SerializationOrder(Order = 2, PropertyName = nameof(Second))]
            public string Second { get; set; }

            [SerializationOrder(Order = 1, PropertyName = nameof(First))]
            public string First { get; set; }
        }
    }
}
