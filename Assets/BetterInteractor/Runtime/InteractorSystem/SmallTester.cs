using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Better.Interactor.Runtime
{
    public class SmallTester
    {
        Stopwatch stopwatch = new Stopwatch();
        private TimeSpan sessionElapsed;
        private TimeSpan totalElapsed;

        public void Start()
        {
            stopwatch.Start();
        }

        public void Stop()
        {
            stopwatch.Stop();
            sessionElapsed += stopwatch.Elapsed;
        }

        public void Report(string testName, long numberOfTests)
        {
            var averageElapsed = sessionElapsed / numberOfTests;
            totalElapsed += averageElapsed;
            Debug.Log(
                $"[{testName}] Average execution time for {numberOfTests} tests: {averageElapsed.TotalMilliseconds:F3}ms with ticks: {averageElapsed.Ticks}");
        }

        public void ReportTotal()
        {
            Debug.Log($"[Total] Average execution time tests: {totalElapsed.TotalMilliseconds:F3}ms with ticks: {totalElapsed.Ticks}");
        }

        public void Reset()
        {
            stopwatch.Reset();
            sessionElapsed = TimeSpan.Zero;
        }
    }
}