using System;
using System.Diagnostics;

namespace Message.Encoder.PoC.Diagnostics
{
    internal class DelegateStopwatch : Stopwatch
    {
        private readonly Action Action;
        private readonly Action<long> OutputAction;

        public DelegateStopwatch(Action action, Action<long> outputAction)
        {
            this.Action = action;
            this.OutputAction = outputAction;
        }

        public long Execute(int times = 0)
        {
            long total = 0;

            switch (times)
            {
                case < 0: ExecuteForever(); break;
                case >= 0: ExecuteNthTimes(times, out total); break;
            }

            return times is not 0 ? total / times : 0;
        }

        private void __void() { }

        private void ExecuteForever()
        {
            Start();

            while (true)
            {
                Action();
                Stop();
                OutputAction(ElapsedMilliseconds);
                Restart();
            }
        }

        private void ExecuteNthTimes(int times, out long total)
        {
            total = 0;
            for (var currentTick = 0; currentTick <= times; currentTick++, Restart(), Tick(ref total), Reset()) __void();
        }

        private void Tick(ref long total)
        {
            Action();
            total += ElapsedMilliseconds;
            OutputAction(ElapsedMilliseconds);
        }
    }
}
