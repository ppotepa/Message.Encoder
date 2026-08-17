using BenchmarkDotNet.Running;
using Message.Encoder.Benchmarks.Conversions;

namespace Message.Encoder.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summaryBinaryConversionsIntegers = BenchmarkRunner.Run<BinaryConversionsIntegers>();
            var summaryBinaryConversionsFloatingPoints = BenchmarkRunner.Run<BinaryConversionsFloatingPoints>();
        }
    }
}

