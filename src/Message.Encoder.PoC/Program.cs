using Message.Encoder.Builders;
using Message.Encoder.Factories.Messages;
using Message.Encoder.Factories.Serialization;
using Message.Encoder.Messages;
using Message.Encoder.Messages.Default.Text;
using Message.Encoder.PoC.Diagnostics;
using Message.Encoder.Serializers;
using System;
using System.Linq;

namespace Message.Encoder.PoC
{
    internal class Program
    {
        private static readonly Action Measure_1 = () =>
        {
            ReadOnlySpan<byte> binaryObject = GetBinaryObject();
            Message message = MessageFactory.Create(binaryObject);
            ReadOnlySpan<byte> serialized = MessageFactory.Serialize(message);
            Console.WriteLine($"-------------------------------------");
        };

        private static readonly Action Measure_2 = () =>
        {
            var message = new DefaultTextMessage
            {
                Headers = new()
                {
                    From = 1,
                    To = 2,
                    Timestamp = 12312312,
                    MessageType = 1
                },

                Payload = new()
                {
                    TextMessageBody = SHORT_STRING
                }
            };

            var res = Message.ToBinary(message);
            _ = Message.FromBytes(res);

            _ = SerializersFactory.CreateSerializer<IPayloadSerializer>(typeof(DefaultTextMessagePayload));
            _ = SerializersFactory.CreateSerializer<IHeadersSerializer>(typeof(DefaultTextMessageHeaders));

            Console.WriteLine($"-------------------------------------");
        };

        private static string LONG_STRING
        {
            get
            {
                var str = string.Join("", Enumerable.Range(1, new Random().Next(1, 1024 * 1024 * 16)).Select(x => "A"));
                Console.WriteLine(str.Length);
                return str;
            }
        }

        private static string SHORT_STRING
        {
            get
            {
                var str = string.Join("", Enumerable.Range(1, new Random().Next(1, 1024 * 1024 * 4)).Select(x => "A"));
                Console.WriteLine(str.Length);
                return str;
            }
        }

        private static ReadOnlySpan<byte> GetBinaryObject()
        {
            return MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>.CreateBuilder()
                .From(1)
                .To(2)
                .Timestamp(DateTime.Now.Ticks)
                .MsgType(1)
                .AddHeader("recipient-name", "Joe")
                .AddHeader("sender-name", "Anne")
                .AddHeader("is-message-unread", true)
                .EndHeaders()
                .AddPayloadProperty(nameof(DefaultTextMessagePayload.TextMessageBody), LONG_STRING)
                .GetBinary();
        }

        static void Main(string[] args)
        {
            var builder = MessageBuilder<DefaultTextMessageHeaders, DefaultTextMessagePayload>.CreateBuilder();

            var longStringMeasure = new DelegateStopwatch(Measure_1, Console.WriteLine);
            var shortStringMeasure  = new DelegateStopwatch(Measure_2, Console.WriteLine);
            
            var executions =25;

            var average1 = longStringMeasure.Execute(executions);
            var average2 = shortStringMeasure .Execute(executions);

            Console.WriteLine($"Average for {longStringMeasure} measure 1 {average1} ms. for {executions}");
            Console.WriteLine($"Average for {shortStringMeasure} measure 2 {average2} ms. for {executions}");
        }
    }
}

