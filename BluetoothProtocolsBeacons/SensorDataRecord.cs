using BluetoothWinUI3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using static BluetoothProtocols.Nordic_Thingy;

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace BluetoothProtocols
{
    public class SensorDataRecord : INotifyPropertyChanged
    {
        /* Adding more fields? Here's a quick guide:
         * 1. When you add a new field, you must also bump the .All value. Do a search in the code
         *    for .All; as of 2023-01-02, there's exactly one.
         * 2. Update the RuuvitagPage.xaml.cs with the new value (about line 50, // SensorDataRecord: Update)
         */
        [Flags]
        public enum SensorPresent {  None=0x00, Temperature = 0x01, Pressure = 0x02, Humidity = 0x04, PM25 = 0x08, All=0x0F };
        public SensorPresent IsSensorPresent { get; set; } = SensorPresent.All;
        public SensorDataRecord()
        {
            Temperature = double.NaN;
            Pressure = double.NaN;
            Humidity = double.NaN;
            PM25 = double.NaN;
            EventTime = DateTimeOffset.Now;
        }
        public SensorDataRecord(double temperature, double pressure, double humidity, DateTimeOffset? eventTime)
        {
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            PM25 = double.NaN;
            EventTime = eventTime ?? DateTimeOffset.Now;
            IsSensorPresent = SensorPresent.Temperature | SensorPresent.Pressure | SensorPresent.Humidity;
        }
        public SensorDataRecord(double temperature, double humidity, DateTimeOffset? eventTime)
        {
            Temperature = temperature;
            Humidity = humidity;
            PM25 = double.NaN;
            EventTime = eventTime ?? DateTimeOffset.Now;
            IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity;
        }


        public void ExportHeaders(IExportData exporter)
        {
            exporter.HeadersSet(["Date", "Time", "Temperature", "Humidity", "PM25"]);
        }

        public void ExportRow(IExportData exporter)
        {
            exporter.RowStart();
            exporter.CellSet(TimestampMostRecentDT.ToString("yyyy-MM-dd"));
            exporter.CellSet(TimestampMostRecentDT.ToString("HH:mm:ss"));
            exporter.CellSet(Temperature);
            exporter.CellSet(Humidity);
            exporter.CellSet(PM25);
            exporter.RowEnd();
        }



        public const string TemperaturePropertyChangedName = "Temperature";
        public const string PM25PropertyChangedName = "PM25";
        public const string PressurePropertyChangedName = "Pressure";
        public const string HumidityPropertyChangedName = "Humidity";

        // For the INPC INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        // Uggg -- this is not set correctly? Should be set from the value in the stack,
        // which is more accurate.
        private DateTimeOffset _EventTime;
        public DateTimeOffset EventTime { get { return _EventTime; } 
            set { if (value == _EventTime) return; _EventTime = value; 
                OnPropertyChanged(); 
                OnPropertyChanged("TimestampMostRecent"); 
                OnPropertyChanged("TimestampMostRecentDT");  
            } 
        }

        public DateTimeOffset TimestampMostRecent {  get { return EventTime; } }
        public DateTime TimestampMostRecentDT { get { return TimestampMostRecent.DateTime; } }


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

        /// <summary>
        /// PM2.5 in ug/m3 µg/㎥
        /// </summary>
        private double _PM25;
        public double PM25 { get { return _PM25; } set { if (value == _PM25) return; _PM25 = value; OnPropertyChanged(); } }
        private String _Note;
        public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }

        public override string ToString()
        {
            var retval = $"Sensor";
            if (IsSensorPresent.HasFlag(SensorPresent.Temperature)) retval += " {Temperature}℃";
            if (IsSensorPresent.HasFlag(SensorPresent.Pressure)) retval += " {Pressure}mb";
            if (IsSensorPresent.HasFlag(SensorPresent.Humidity)) retval += " {Humidity}%";
            if (IsSensorPresent.HasFlag(SensorPresent.PM25)) retval += " {PM25}µg/㎥";
            return retval;
        }
    }
}
