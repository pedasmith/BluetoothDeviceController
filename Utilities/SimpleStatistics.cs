using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace Utilities
{
    internal class SimpleStatistics
    {
        int MaxCount = 1;
        public SimpleStatistics(int maxCount)
        {
            MaxCount = maxCount;
            Values = new(MaxCount);
        }
        Queue<double> Values;

        /// <summary>
        /// Alpha must be >0 and <=1. Larger value are "twitchier" and 1 is just the last value.
        /// </summary>
        public double Alpha { get; set; } = 0.25;
        private double _currentAverage = double.NaN;

        /// <summary>
        /// Updates the moving average with a new value.
        /// </summary>
        public void Update(double value)
        {
            if (Values.Count >= MaxCount)
            {
                Values.Dequeue();
            }
            Values.Enqueue(value);

            double total = 0.0;
            foreach (double v in Values)
            {
                total += v;
            }
            var average = total / (double)Values.Count;
            CurrentAverage = average;
        }
        /// <summary>
        /// The value to return when there's no value. Default is NaN
        /// </summary>
        public double DefaultValue { get; set; } = double.NaN;
        /// <summary>
        /// Gets the current moving average.
        /// </summary>
        public double CurrentAverage
        {
            get { return double.IsNaN(_currentAverage) ? DefaultValue : _currentAverage; }
            set { if (value == _currentAverage) return; _currentAverage = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Key event handler for the INPC (INotifyPropertyChanged) interface. 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SimpleStatistics Clone()
        {
            var retval = this.MemberwiseClone() as SimpleStatistics;
            return retval;
        }
    }
}
