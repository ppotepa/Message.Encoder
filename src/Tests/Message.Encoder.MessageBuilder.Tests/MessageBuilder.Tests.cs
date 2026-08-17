using NUnit.Framework;
using Message.Encoder.Builders;
using Message.Encoder.Exceptions;
using Message.Encoder.Extensions;
using Message.Encoder.MessageBuilder.Tests.CustomMessages.AllProperties;
using Message.Encoder.MessageBuilder.Tests.CustomMessages.Default;
using Message.Encoder.Messages;
using Message.Encoder.Messages.Default.Text;
using Message.Encoder.Messages.Transport;
using Message.Encoder.Serializers.Default;
using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming
namespace Message.Encoder.MessageBuilder.Tests
{
    public class MessageBuilderTests
    {
        const long BILLION = 1_000_000_000;
        const int MILLION = 1_000_000;
        const byte ONE = 1;
        private const string STRING_32_BYTES = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
        const short THOUSAND = 1_000;
        const long TRILLION = 1_000_000_000_000;

        private readonly byte[] assertBytes =
        {
            1, 0, 0, 0, 0, 0, 0, 0,                     //  FROM                [8] bytes - translates to [1]
            2, 0, 0, 0, 0, 0, 0, 0,                     //  TO                  [8] bytes - translates to [2]
            134, 1, 114, 100, 0, 0, 0, 0,               //  TIMESTAMP           [8] bytes - translates to { 01.01.0001 00:02:48 }
            100,                                          //  MSG_TYPE            [1] byte  - translates to [1] 
            76, 0, 0, 0, 0, 0, 0, 0,                    //  HEADERS_LENGTH      [8] bytes -translates to  [57]
            1, 0,   100,                                //  HEADER_1            [2 bytes, 1 byte] - translates to - 100 as (byte)
            2, 0,   100, 0,                             //  HEADER_2            [2 bytes, 2 bytes] - translates to - 100 as (short)
            4, 0,   100, 0, 0, 0,                       //  HEADER_3            [2 bytes, 4 bytes] - translates to - 100 as (int)
            8, 0,   100, 0, 0, 0, 0, 0, 0, 0,           //  HEADER_4            [2 bytes, 8 bytes] - translates to - 100 as (long)
            32, 0,  97, 97, 97, 97, 97, 97, 97, 97,     //  HEADER_5            [2 bytes, 32 bytes (string)] - translates to string "a..."[32]
                    97, 97, 97, 97, 97, 97, 97, 97,
                    97, 97, 97, 97, 97, 97, 97, 97,
                    97, 97, 97, 97, 97, 97, 97, 97,
            4, 0, 0, 0, 128, 63,
            8, 0, 0, 0,   0,  0, 0,  0, 240, 63,
            1, 0, 1
        };

        private readonly byte[] assertBytesHeaderMissing =
        {
            1, 0, 0, 0, 0, 0, 0, 0,                     //  FROM                [8] bytes - translates to [1]
            2, 0, 0, 0, 0, 0, 0, 0,                     //  TO                  [8] bytes - translates to [2]
            134, 1, 114, 100, 0, 0, 0, 0,               //  TIMESTAMP           [8] bytes - translates to { 01.01.0001 00:02:48 }
            100,                                          //  MSG_TYPE            [1] byte  - translates to [1] 
            53, 0, 0, 0, 0, 0, 0, 0,                    //  HEADERS_LENGTH      [8] bytes -translates to  [57]
            1, 0,   100,                                //  HEADER_1            [2 bytes, 1 byte] - translates to - 100 as (byte)
            //2, 0,   100, 0,                               HEADER_2            [2 bytes, 2 bytes] - translates to - 100 as (short)
            4, 0,   100, 0, 0, 0,                       //  HEADER_3            [2 bytes, 4 bytes] - translates to - 100 as (int)
            8, 0,   100, 0, 0, 0, 0, 0, 0, 0,           //  HEADER_4            [2 bytes, 8 bytes] - translates to - 100 as (long)
            32, 0,  97, 97, 97, 97, 97, 97, 97, 97,     //  HEADER_5            [2 bytes, 32 bytes (string)] - translates to string "a..."[32]
            97, 97, 97, 97, 97, 97, 97, 97,
            97, 97, 97, 97, 97, 97, 97, 97,
            97, 97, 97, 97, 97, 97, 97, 97
        };

        private readonly byte[] assertBytesInvalidHeadersLength =
        {
            1, 0, 0, 0, 0, 0, 0, 0,                     //  FROM                [8] bytes - translates to [1]
            2, 0, 0, 0, 0, 0, 0, 0,                     //  TO                  [8] bytes - translates to [2]
            134, 1, 114, 100, 0, 0, 0, 0,               //  TIMESTAMP           [8] bytes - translates to { 01.01.0001 00:02:48 }
            100,                                        //  MSG_TYPE            [1] byte  - translates to [1] 
            255, 0, 0, 0, 0, 0, 0, 0,                   //  HEADERS_LENGTH      [8] bytes -translates to  [57]
            1, 0,   100,                                //  HEADER_1            [2 bytes, 1 byte] - translates to - 100 as (byte)
            2, 0,   100, 0,                             //  HEADER_2            [2 bytes, 2 bytes] - translates to - 100 as (short)
            4, 0,   100, 0, 0, 0,                       //  HEADER_3            [2 bytes, 4 bytes] - translates to - 100 as (int)
            8, 0,   100, 0, 0, 0, 0, 0, 0, 0,           //  HEADER_4            [2 bytes, 8 bytes] - translates to - 100 as (long)
            32, 0,  97, 97, 97, 97, 97, 97, 97, 97,     //  HEADER_5            [2 bytes, 32 bytes (string)] - translates to string "a..."[32]
            97, 97, 97, 97, 97, 97, 97, 97,
            97, 97, 97, 97, 97, 97, 97, 97,
            97, 97, 97, 97, 97, 97, 97, 97
        };

        readonly byte[] assertHeaderBytes =
        {
            1, 0, 0, 0, 0, 0, 0, 0,                     //  FROM                [8] bytes - translates to [1]
            2, 0, 0, 0, 0, 0, 0, 0,                     //  TO                  [8] bytes - translates to [2]
            134, 1, 114, 100, 0, 0, 0, 0,               //  TIMESTAMP           [8] bytes - translates to { 01.01.0001 00:02:48 }
            1,                                          //  MSG_TYPE            [1] byte  - translates to [1] 
            0, 0, 0, 0, 0, 0, 0, 0                      //  HEADERS_LENGTH      [8] bytes - translates to [0] 
        };

        private readonly object[] defaultSerializerInput = {
            (long)1,
            (long)2,
            (long)1685193094,
            (byte)100,
            (long)76,
            (short)1, (byte)100,
            (short)2, (short)100,
            (short)4, 100,
            (short)8, (long)100,
            (short)32, STRING_32_BYTES,
            (short)4, 1f,
            (short)8, 1d,
            (short)1, true,
        };

        [Test]
        public void BinarySerializer_Works_Against_DefaultTextMessage()
        {
            byte[] builderMessageBinary = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>.CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(1)
                .AddHeader("recipient-name", "AAAAA")
                .AddHeader("sender-name", "AAAAA")
                .AddHeader("is-message-unread", true)
                .EndHeaders()
                .AddPayloadProperty("TextMessageBody", "AAAAA")
                .GetBinary();

            var myTestMessage = new DefaultTextMessage
            {
                Headers = new DefaultTextMessageHeaders
                {
                    From = 1,
                    To = 2,
                    MessageType = 1,
                    Timestamp = 1685193094,
                    IsMessageUnread = true,
                    RecipientName = "AAAAA",
                    SenderName = "AAAAA"
                },

                Payload = new DefaultTextMessagePayload
                {
                    TextMessageBody = "AAAAA"
                }
            };

            var myTestMessageArray = Message.ToBinary(myTestMessage).ToArray();

            Console.WriteLine($"BuilderMessage Length {builderMessageBinary.Length} {string.Join(",", builderMessageBinary.ToArray())}");
            Console.WriteLine($"Message.Create Length {myTestMessageArray.Length} {string.Join(",", myTestMessageArray)}");

            var myTestMessageBinary = Message.ToBinary(myTestMessage).ToArray();
            Assert.AreEqual(builderMessageBinary, myTestMessageBinary);
        }

        [Test]
        public void Creating_A_Message_With_Null_Payload_And_Transport_Throws_Exception()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<ArgumentNullException>(() => new DefaultTextMessage(new DefaultTextMessageHeaders(), null));
                Assert.Throws<ArgumentNullException>(() => new DefaultTextMessage(null, new DefaultTextMessagePayload()));
                Assert.DoesNotThrow(() => new DefaultTextMessage(new DefaultTextMessageHeaders(), new DefaultTextMessagePayload()));
            });
        }

        [Test]
        public void Custom_Bit_Converter_Matches_Results_With_Built_In_One()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            Assert.Multiple(() =>
            {
                // Out of some reason BitConverter interprets single byte as 2-byte digit.
                Assert.AreEqual(BitConverter.GetBytes((short)ONE).Take(1), ONE.ToByteArray());
                Assert.AreEqual(BitConverter.GetBytes(THOUSAND), THOUSAND.ToByteArray());
                Assert.AreEqual(BitConverter.GetBytes(MILLION), MILLION.ToByteArray());
                Assert.AreEqual(BitConverter.GetBytes(BILLION), BILLION.ToByteArray());
                Assert.AreEqual(BitConverter.GetBytes(TRILLION), TRILLION.ToByteArray());

                Assert.Throws<ArgumentException>(() => dictionary.ToByteArray());
                Assert.Throws<ArgumentException>(() => dictionary.GetBytes());

                Assert.That(() => ((object)null).GetBytes(), Has.Exactly(0).Items.Empty);
                Assert.That(() => ((object)null).ToByteArray(), Has.Exactly(0).Items.Empty);

                Assert.That(() => true.ToByteArray(), Has.Length.EqualTo(1).And.ItemAt(0).EqualTo(1));
                Assert.That(() => false.ToByteArray(), Has.Length.EqualTo(1).And.ItemAt(0).EqualTo(0));

                Assert.That(() => new ReadOnlySpan<byte>(new byte[] { }).GetString(), Has.Exactly(0).Items.Empty);
                Assert.That(() => new ReadOnlySpan<byte>(new byte[] { 65, 65 }).GetString(), Has.All.EqualTo('A').And.Exactly(2).Items);


                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { }).ToBoolean());
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1, 1 }).ToBoolean());
                Assert.That(() => new ReadOnlySpan<byte>(new byte[] { }).ToNullableBoolean(), Is.Null);
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1, 1 }).ToNullableBoolean());

                Assert.That(new ReadOnlySpan<byte>(new byte[] { }).ToNullableSingle(), Is.EqualTo(null));
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToNullableSingle());
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToSingle());

                Assert.That(new ReadOnlySpan<byte>(new byte[] { }).ToNullableInt32(), Is.EqualTo(null));
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToInt32());
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToNullableInt32());

                Assert.That(new ReadOnlySpan<byte>(new byte[] { }).ToNullableInt64(), Is.EqualTo(null));
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToInt64());
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToNullableInt64());

                Assert.That(new ReadOnlySpan<byte>(new byte[] { }).ToNullableDouble(), Is.EqualTo(null));
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToNullableDouble());
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1 }).ToDouble());

                Assert.That(new ReadOnlySpan<byte>(new byte[] { }).ToNullableInt8(), Is.EqualTo(null));
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1, 1 }).ToNullableInt8());
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1, 1 }).ToInt8());

                Assert.That(new ReadOnlySpan<byte>(new byte[] { }).ToNullableInt16(), Is.EqualTo(null));
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1, 1, 1 }).ToNullableInt16());
                Assert.Throws<ArgumentException>(() => new ReadOnlySpan<byte>(new byte[] { 1, 1, 1 }).ToInt16());

            });
        }

        [Test]
        public void Default_Headers_Serializer_Should_Not_Accept_Wrong_Types()
        {
            Assert.Multiple(() =>
            {
                Assert.Throws<InvalidOperationException>(
                    () => new DefaultHeadersSerializer().Deserialize(typeof(TestMessage), new MessageHeaderTransport()));

                Assert.Throws<InvalidOperationException>(
                    () => new DefaultHeadersSerializer().Deserialize(typeof(TestMessage), new MessageHeaderTransport()));

            });
        }

        [Test]
        public void Default_Payload_Serializer_Returns_Empty_Array_If_Payload_Is_Null()
        {
            var result = new DefaultPayloadSerializer().Serialize<TestMessagePayload>(null).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Has.Exactly(2).Items.EqualTo(0));
            });
        }

        [Test]
        public void Deserialization_When_Headers_Are_Missing_Throws_An_Exception()
        {
            Assert.Throws<InvalidAmountOfHeadersFound>(() => Message.FromBytes(assertBytesHeaderMissing));
        }

        [Test]
        public void Deserialization_When_Headers_Length_Is_Invalid_Throws_An_Exception()
        {
            Assert.Throws<InvalidHeadersLengthException>(() => Message.FromBytes(assertBytesInvalidHeadersLength));
        }

        [Test]
        public void Message_Created_By_Builder_Equals_Instance_Message_Back_And_Forth()
        {
            var builderMessage = MessageBuilder<TestMessageHeader, TestMessagePayload>.CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(100)
                .AddHeader("byte", (byte)100)
                .AddHeader("short", (short)100)
                .AddHeader("int", 100)
                .AddHeader("long", (long)100)
                .AddHeader("string", STRING_32_BYTES)
                .AddHeader("float", 1f)
                .AddHeader("double", 1d)
                .AddHeader("boolean", true)
                .EndHeaders()
                .AddPayloadProperty(nameof(TestMessagePayload.TestTextBody), "Test message.")
                .Build() as Message<TestMessageHeader, TestMessagePayload>;

            var shortHandMessage = new TestMessage
            {
                Headers = new TestMessageHeader
                {
                    MessageType = 100,
                    Timestamp = 1685193094,
                    From = 1,
                    To = 2,
                    Byte = 100,
                    Int = 100,
                    Long = 100,
                    Short = 100,
                    String = STRING_32_BYTES,
                    Float = 1f,
                    Double = 1d,
                    Boolean = true
                },

                Payload = new TestMessagePayload
                {
                    TestTextBody = "Test message."
                }
            };

            var conditions = new[]
            {
                builderMessage.Payload.TestTextBody == shortHandMessage.Payload.TestTextBody,
                builderMessage.Headers.String == shortHandMessage.Headers.String,
                builderMessage.Headers.Int == shortHandMessage.Headers.Int,
                builderMessage.Headers.Byte == shortHandMessage.Headers.Byte,
                builderMessage.Headers.Long == shortHandMessage.Headers.Long,
                builderMessage.Headers.Boolean == shortHandMessage.Headers.Boolean,
                builderMessage.Headers.Double == shortHandMessage.Headers.Double,
                builderMessage.Headers.Short == shortHandMessage.Headers.Short,
                builderMessage.Headers.Float == shortHandMessage.Headers.Float,
                builderMessage.Headers.From == shortHandMessage.Headers.From,
                builderMessage.Headers.To == shortHandMessage.Headers.To,
                builderMessage.Headers.Timestamp == shortHandMessage.Headers.Timestamp,
                builderMessage.Headers.MessageType == shortHandMessage.Headers.MessageType,
                builderMessage.Headers.HeadersLength == shortHandMessage.Headers.HeadersLength,
            };

            Assert.That(conditions, Has.Exactly(13).EqualTo(true));
        }

        [Test]
        public void MessageBuilder_Serializes_Assertion_Bytes_Correctly()
        {
            Assert.Multiple(() =>
            {
                Message resultMessage = Message.FromBytes(assertBytes);

                Assert.That(resultMessage, Is.Not.Null);
                Assert.That(resultMessage, Is.TypeOf(typeof(TestMessage)));
            });
        }

        [Test]
        public void MessageBuilder_Serializes_Basic_Headers_Correctly()
        {
            var binary = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>.CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(1)
                .EndHeaders()
                .GetBinary();

            Assert.AreEqual(assertHeaderBytes, binary);
        }
        [Test]
        public void MessageBuilder_Serializes_Basic_Headers_Correctly_With_Additional_Headers()
        {
            var defaultSerializerResult = defaultSerializerInput.SelectMany(integer => integer.ToByteArray()).ToArray();
            var binarySerializerResult = defaultSerializerInput.SelectMany(BinaryExtensions.GetBytes).ToArray();

            var builderMessageBinary = MessageBuilder<TestMessageHeader, TestMessagePayload>.CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(100)
                .AddHeader("byte", (byte)100)
                .AddHeader("short", (short)100)
                .AddHeader("int", 100)
                .AddHeader("long", (long)100)
                .AddHeader("string", STRING_32_BYTES)
                .AddHeader("float", 1f)
                .AddHeader("double", 1d)
                .AddHeader("boolean", true)
                .EndHeaders()
                .GetBinary();

            Console.WriteLine($"defaultSerializerResult: {defaultSerializerResult.Length} {string.Join(",", defaultSerializerResult)}");
            Console.WriteLine($"binarySerializerResult:  {binarySerializerResult.Length} {string.Join(",", binarySerializerResult)}");
            Console.WriteLine($"builderMessageBinary:    {builderMessageBinary.Length} {string.Join(",", builderMessageBinary)}");

            bool[] testAssertionConditions = {
                assertBytes.SequenceEqual(builderMessageBinary),        // Check against BinaryBuilder.
                assertBytes.SequenceEqual(defaultSerializerResult),     // Check against ToByteArray() Extension Method.
                assertBytes.SequenceEqual(binarySerializerResult)       // Check against Built-In BitConverter.
            };

            Assert.That(testAssertionConditions, Has.Exactly(3).EqualTo(true));
        }
        [Test]
        public void MessageBuilder_Serializes_Message_Back_And_Forth()
        {
            var timestamp = 123_123_123;

            var message = new AllPropertiesTestMessage
            {
                Headers = new AllPropertiesHeader
                {
                    From = 1,
                    To = 2,
                    Timestamp = timestamp,
                    MessageType = 150,
                    Header1 = "Test header"
                },
                Payload = new AllPropertiesPayload
                {
                    Boolean = true,
                    Byte = 1,
                    Double = 1d,
                    Float = 1f,
                    Int = 1,
                    Long = 1,
                    Short = 1,
                    String = "string",

                    NullableBoolean = true,
                    NullableByte = 1,
                    NullableDouble = 1d,
                    NullableFloat = 1f,
                    NullableInt = 1,
                    NullableLong = 1,
                    NullableShort = 1,

                    NullProperty = null,
                }
            };

            var binary = Message.ToBinary(message);
            var fromBinary = Message.FromBytes(binary);
            var toBinaryAgain = Message.ToBinary(fromBinary);

            var sequencesAreEqual = binary.SequenceEqual(toBinaryAgain);

            Assert.Multiple(() =>
            {
                Assert.That(fromBinary.Headers, Is.TypeOf(typeof(AllPropertiesHeader)));
                Assert.That(fromBinary.Payload, Is.TypeOf(typeof(AllPropertiesPayload)));

                Assert.NotNull(fromBinary.Payload);
                Assert.NotNull(fromBinary.Headers);

                Assert.That(fromBinary, Is.TypeOf(typeof(AllPropertiesTestMessage)));
                Assert.That(fromBinary.GetType().BaseType, Is.EqualTo(typeof(Message<AllPropertiesHeader, AllPropertiesPayload>)));

                Assert.That(sequencesAreEqual, Is.True);
            });
        }

        [Test]
        public void MessageBuilder_Throws_An_Exception_When_Header_Is_Missing()
        {
            var builderMessageBinary = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>
                .CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(1)
                .AddHeader("recipient-name", "AAAAA")
                .EndHeaders();

            Assert.Throws<InvalidAmountOfHeadersSuppliedException>(() => builderMessageBinary.Build());
        }

        [Test]
        public void MessageBuilder_Throws_An_Exception_When_Header_Name_Is_Missing()
        {
            var builderMessageBinary = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>
                .CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(1)
                .AddHeader("recipient-name", "AAAAA");

            Assert.Throws<InvalidHeaderNameException>(() => builderMessageBinary.AddHeader<string>("bad-header-name", null));
        }
        [Test]
        public void MessageBuilder_Throws_An_Exception_When_Headers_Count_Is_To_High()
        {
            var builderMessageBinary = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>
                .CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(1)
                .AddHeader("recipient-name", "AAAAA")
                .AddHeader("sender-name", "AAAAA")
                .AddHeader("is-message-unread", "AAAAA");

            Assert.Throws<HeadersCountExceededException>(() => builderMessageBinary.AddHeader<string>("excessive-header-name", null));
        }

        [Test]
        public void MessageBuilder_Throws_An_Exception_When_Payload_Properties_Are_Missing()
        {
            var builderMessageBinary = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>
                .CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(1)
                .AddHeader("recipient-name", "AAAAA")
                .AddHeader("sender-name", "AAAAA")
                .AddHeader("is-message-unread", "AAAAA")
                .EndHeaders();


            Assert.Throws<InvalidAmountOfPayloadPropertiesSuppliedException>(() => builderMessageBinary.Build());
        }
        [Test]
        public void MessageBuilder_Throws_An_Exception_When_Payload_Properties_Count_Is_To_High()
        {
            var builderMessageBinary = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>
                .CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(1685193094)
                .MsgType(1)
                .AddHeader("recipient-name", "AAAAA")
                .AddHeader("sender-name", "AAAAA")
                .AddHeader("is-message-unread", "AAAAA")
                .EndHeaders()
                .AddPayloadProperty("TextMessageBody", "Some text.");

            Assert.Throws<PayloadPropertiesCountExceeded>(() => builderMessageBinary.AddPayloadProperty<string>("ExcessivePropertyName", null));
        }
        [SetUp]
        public void Setup()
        {
            // intentionally left empty
        }

        [Test]
        public void UserIsNotAbleToOverrideHeaders()
        {
            const string @message = "this is test header message";

            DefaultTextMessageHeaders headers = new DefaultTextMessageHeaders();
            headers["test-header"] = @message;
            var result = headers["test-header"];
            Assert.AreSame(@message, result);
        }
    }
}