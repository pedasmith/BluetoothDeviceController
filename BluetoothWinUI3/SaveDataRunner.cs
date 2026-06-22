using System;
using System.Collections.Generic;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    /// <summary>
    /// Does a save data every so often. There's a little stack of periods and how long to run them for.
    /// </summary>
    internal class SaveDataRunner
    {
        class TimerLength
        {
            public TimerLength(int timeInMinutes, int nTimes)
            {
                TimeInMinutes = timeInMinutes;
                NTimes = nTimes;
            }
            public int TimeInMinutes;
            public double TimeInMilliseconds {  get { return TimeInMinutes * 60_000.0; } }
            public int NTimes;
            public bool RunInfiniteTimes { get { return NTimes == int.MaxValue; } }
            /// <summary>
            /// Decrement NTimes; when zero (or negative) return true
            /// </summary>
            /// <returns></returns>
            public bool DecrementNTimes()
            {
                if (RunInfiniteTimes) return false;
                NTimes--;
                var retval = NTimes <= 0;
                return retval;
            }
        }
        Stack<TimerLength> TimerLengths = new Stack<TimerLength>(
            new List<TimerLength>() {
                new TimerLength(60, int.MaxValue),
                new TimerLength(10, 5),
                new TimerLength(1, 10),
        });

        System.Timers.Timer PeriodicTimer;
        int NElapsedCalls = 0;
        int NExceptions = 0;
        public void Start()
        {
            PeriodicTimer = new System.Timers.Timer();
            var tl = TimerLengths.Peek();
            tl.DecrementNTimes();
            PeriodicTimer.Interval = tl.TimeInMilliseconds;
            PeriodicTimer.Elapsed += PeriodicTimer_Elapsed;
            PeriodicTimer.Enabled = true;
        }

        private void PeriodicTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var tl = TimerLengths.Peek();
            var isLast = tl.DecrementNTimes();
            if (isLast)
            {
                TimerLengths.Pop();
                tl = TimerLengths.Peek();
                PeriodicTimer.Interval = tl.TimeInMilliseconds;
            }
            NElapsedCalls++;
            try
            {
                AllSaveData.Save();
            }
            catch (Exception)
            {
                NExceptions++; // just fail and don't worry the user. 
            }
        }
    }
}
