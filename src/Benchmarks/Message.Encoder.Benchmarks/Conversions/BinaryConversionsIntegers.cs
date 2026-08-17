using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Message.Encoder.Extensions;

namespace Message.Encoder.Benchmarks.Conversions
{
    [SimpleJob(RuntimeMoniker.Net70, baseline: true)]
    [MaxIterationCount(15)]
    [MinIterationCount(10)]
    [RPlotExporter]
    public class BinaryConversionsIntegers
    {
        [Params(2, 4, 8)]
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
                2 => BitConverter.ToInt16(_data),
                4 => BitConverter.ToInt32(_data),
                8 => BitConverter.ToInt64(_data),
                _ => throw new InvalidOperationException("Invalid params.")
            };
        }

        [Benchmark]
        public object UsingBitShifting()
        {
            return _data.Length switch
            {
                2 => _data.ToInt16(),
                4 => _data.ToInt32(),
                8 => _data.ToInt64(),
                _ => throw new InvalidOperationException("Invalid params.")
            };
        }
    }
}