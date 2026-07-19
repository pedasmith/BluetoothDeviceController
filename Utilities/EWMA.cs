using System.ComponentModel;
using System.Runtime.CompilerServices;

#if NET8_0_OR_GREATER
#nullable disable
#endif
namespace Utilities
{
    /// <summary>
    /// Exponentially Weighted Moving Average (EWMA) class for calculating the moving average of a series of values with exponential weighting.
    /// </summary>
    public class EWMA : INotifyPropertyChanged
    {
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
            if (double.IsNaN(_currentAverage))
            {
                CurrentAverage = value;
            }
            else
            {
                CurrentAverage = Alpha * value + (1 - Alpha) * CurrentAverage;
            }
        }
        /// <summary>
        /// Gets the current moving average.
        /// </summary>
        public double CurrentAverage
        {
            get { return _currentAverage;  }
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

        public EWMA Clone()
        {
            var retval = this.MemberwiseClone() as EWMA;
            return retval;
        }

    }
}
