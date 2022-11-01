using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace BluetoothProtocols
{


    public class SensorDataRecord : INotifyPropertyChanged
    {
        [Flags]
        public enum SensorPresent {  None=0x00, Temperature = 0x01, Pressure = 0x02, Humidity = 0x04, All=0x07 };
        public SensorPresent IsSensorPresent { get; set; } = SensorPresent.All;
        public SensorDataRecord()
        {
            Temperature = double.NaN;
            Pressure = double.NaN;
            Humidity = double.NaN;
            EventTime = DateTime.Now;
        }
        public SensorDataRecord(double temperature, double pressure, double humidity)
        {
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            EventTime = DateTime.Now;
            IsSensorPresent = SensorPresent.Temperature | SensorPresent.Pressure | SensorPresent.Humidity;
        }


        // For the INPC INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private DateTime _EventTime;
        public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

        private double _Temperature;
        /// <summary>
        /// Temperature in degrees C
        /// </summary>
        public double Temperature { get { return _Temperature; } set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged(); } }

        /// <summary>
        /// Pressure in hPA
        /// </summary>
        private double _Pressure;
        public double Pressure { get { return _Pressure; } set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged(); } }

        private double _Humidity;
        public double Humidity { get { return _Humidity; } set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged(); } }

        private String _Note;
        public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }

        public override string ToString()
        {
            var retval = $"Sensor";
            if (IsSensorPresent.HasFlag(SensorPresent.Temperature)) retval += " {Temperature}℃";
            if (IsSensorPresent.HasFlag(SensorPresent.Pressure)) retval += " {Pressure}mb";
            if (IsSensorPresent.HasFlag(SensorPresent.Humidity)) retval += " {Humidity}%";
            return retval;
        }
    }
}
