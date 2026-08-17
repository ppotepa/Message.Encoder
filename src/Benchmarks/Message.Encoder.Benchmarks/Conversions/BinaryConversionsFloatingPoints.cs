using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Message.Encoder.Extensions;
using System;

namespace Message.Encoder.Benchmarks.Conversions
{
    [SimpleJob(RuntimeMoniker.Net70, baseline: true)]
    [MaxIterationCount(15)]
    [MinIterationCount(10)]
    [RPlotExporter]
    public class BinaryConversionsFloatingPoints
    {
        [Params(4, 8)]
        public int N;
        private byte[] _data;

        [GlobalSetup]
        public void Setup()
        {
            _data = new byte[N];
            new Random(1000).NextBytes(_data);
        }

        [Benchmark]
        public object UsingBitConverter()
        {
            return _data.Length switch
            {
                4 => BitConverter.ToSingle(_data),
                8 => BitConverter.ToDouble(_data),
                _ => throw new InvalidOperationException("Invalid params.")
            };
        }

        [Benchmark]
        public object UsingBitShifting()
        {
            return _data.Length switch
            {
                4 => BitConverter.Int32BitsToSingle(_data.ToInt32()),
                8 => BitConverter.Int64BitsToDouble(_data.ToInt64()),
                _ => throw new InvalidOperationException("Invalid params.")
            };
        }
    }
}
