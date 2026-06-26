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
    public partial class SensorDataRecord : BTCommonMetaData<SensorDataRecord>
    {
        /* Adding more fields? Here's a quick guide:
         * 1. When you add a new field, you must also bump the .All value. Do a search in the code
         *    for .All; as of 2023-01-02, there's exactly one.
         * 2. Update the RuuvitagPage.xaml.cs with the new value (about line 50, // SensorDataRecord: Update)
         */
        [Flags]
        public enum SensorPresent {  None=0x00, Temperature = 0x01, Pressure = 0x02, Humidity = 0x04, PM25 = 0x08, Battery = 0x10, All=0x1F };
        public SensorPresent IsSensorPresent { get; set; } = SensorPresent.All;
        public SensorDataRecord()
        {
            Temperature = double.NaN;
            Pressure = double.NaN;
            Humidity = double.NaN;
            PM25 = double.NaN;
            TimestampMostRecent = DateTimeOffset.Now;
        }
        public SensorDataRecord(double temperature, double pressure, double humidity, DateTimeOffset? eventTime)
        {
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            PM25 = double.NaN;
            TimestampMostRecent = eventTime ?? DateTimeOffset.Now;
            IsSensorPresent = SensorPresent.Temperature | SensorPresent.Pressure | SensorPresent.Humidity;
        }

        public SensorDataRecord(double temperature, double humidity, DateTimeOffset? eventTime)
        {
            Temperature = temperature;
            Humidity = humidity;
            PM25 = double.NaN;
            TimestampMostRecent = eventTime ?? DateTimeOffset.Now;
            IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity;
        }

        public override SensorDataRecord Clone(string name = null)
        {
            var retval = this.MemberwiseClone() as SensorDataRecord;
            if (name != null)
            {
                retval.Name = name;
            }
            return retval;
        }

        public override void CopyFrom(SensorDataRecord value)
        {
            this.TimestampMostRecent = value.TimestampMostRecent;
            this.Name = value.Name;
            this.Temperature = value.Temperature;
            this.Pressure = value.Pressure;
            this.Humidity = value.Humidity;
            this.PM25 = value.PM25;
        }
        public override string[] ExportGetHeaders(IExportData _)
        {
            List<string> headers = new List<string>();
            if (IsSensorPresent.HasFlag(SensorPresent.Temperature)) headers.Add("Temperature");
            if (IsSensorPresent.HasFlag(SensorPresent.Pressure)) headers.Add("Pressure");
            if (IsSensorPresent.HasFlag(SensorPresent.Humidity)) headers.Add("Humidity");
            if (IsSensorPresent.HasFlag(SensorPresent.PM25)) headers.Add("PM25");
            if (IsSensorPresent.HasFlag(SensorPresent.Battery)) headers.Add("Battery");

            return headers.ToArray();
        }

        public override void ExportRow(IExportData exporter)
        {
            if (IsSensorPresent.HasFlag(SensorPresent.Temperature)) exporter.CellSet(Temperature);
            if (IsSensorPresent.HasFlag(SensorPresent.Pressure)) exporter.CellSet(Pressure);
            if (IsSensorPresent.HasFlag(SensorPresent.Humidity)) exporter.CellSet(Humidity);
            if (IsSensorPresent.HasFlag(SensorPresent.PM25)) exporter.CellSet(PM25);
            if (IsSensorPresent.HasFlag(SensorPresent.Battery)) exporter.CellSet(BatteryInPercent);
        }



        public const string TemperaturePropertyChangedName = "Temperature";
        public const string PM25PropertyChangedName = "PM25";
        public const string PressurePropertyChangedName = "Pressure";
        public const string HumidityPropertyChangedName = "Humidity";
        public const string BatteryPropertyChangedName = "BatteryInPercent";


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


        private double _BatteryInPercent;
        /// <summary>
        /// Battery in percent
        /// </summary>
        public double BatteryInPercent { get { return _BatteryInPercent; } set { if (value == _BatteryInPercent) return; _BatteryInPercent = value; OnPropertyChanged(); } }

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
