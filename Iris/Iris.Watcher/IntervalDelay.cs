using System;
using System.Threading.Tasks;

namespace Iris.Watcher
{
    internal class IntervalDelay
    {
        private readonly TimeSpan _interval;
        private readonly DateTime _start;
        private int _index = 0;

        public IntervalDelay(TimeSpan interval)
        {
            _interval = interval;
            _start = DateTime.Now;
        }

        public async Task DelayTillNext()
        {
            TimeSpan expectedNext = _interval * ++_index;
            TimeSpan now = DateTime.Now - _start;
            TimeSpan delay = expectedNext - now;
            
            if (delay.TotalMilliseconds > 0)
            {
                await Task.Delay(delay);
            }
        }
    }
}