using System;
using System.Diagnostics;

namespace Roslyn.Utilities
{
    internal readonly struct SharedStopwatch
    {
        private static readonly Stopwatch s_stopwatch = Stopwatch.StartNew();

        private readonly TimeSpan _started;

        public TimeSpan Elapsed => s_stopwatch.Elapsed - _started;

        private SharedStopwatch(TimeSpan started)
        {
            _started = started;
        }

        public static SharedStopwatch StartNew()
        {
            return new SharedStopwatch(s_stopwatch.Elapsed);
        }
    }
}
