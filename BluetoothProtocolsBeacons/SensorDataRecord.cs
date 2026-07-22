using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

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
         * 2. OBSOLETE: Update the RuuvitagPage.xaml.cs with the new value (about line 50, // SensorDataRecord: Update)
         */
        [Flags]
        public enum SensorPresent 
        {  
            None=0x00, 
            Temperature = 0x01, Pressure = 0x02, Humidity = 0x04, 
            Battery = 0x80,
            PM25 = 0x10, CO2 = 0x20, VOC = 0x40, NOX = 0x80, Luminosity = 0x100,
            All=0x1FF };
        public SensorPresent IsSensorPresent { get; set; } = SensorPresent.All;
        public SensorDataRecord()
        {
            Temperature = double.NaN;
            Pressure = double.NaN;
            Humidity = double.NaN;
            PM25 = double.NaN;
            CO2 = 390;
            VOC = double.NaN;
            NOX = double.NaN;
            Luminosity = double.NaN;
            TimestampMostRecent = DateTimeOffset.Now;
        }
        public SensorDataRecord(double temperature, double pressure, double humidity, DateTimeOffset? eventTime)
        {
            Temperature = temperature;
            Pressure = pressure;
            Humidity = humidity;
            PM25 = double.NaN;
            CO2 = 390;
            VOC = double.NaN;
            NOX = double.NaN;
            Luminosity = double.NaN;
            TimestampMostRecent = eventTime ?? DateTimeOffset.Now;
            IsSensorPresent = SensorPresent.Temperature | SensorPresent.Pressure | SensorPresent.Humidity;
        }

        public SensorDataRecord(double temperature, double humidity, DateTimeOffset? eventTime)
        {
            Temperature = temperature;
            Humidity = humidity;
            PM25 = double.NaN;
            CO2 = 390;
            VOC = double.NaN;
            NOX = double.NaN;
            Luminosity = double.NaN;
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
            this.CO2 = value.CO2;
            this.VOC = value.VOC;
            this.NOX = value.NOX;
            this.Luminosity = value.Luminosity;
        }

        // CopyFrom, but convert the doubles as appropriate
        public static SensorDataRecord CopyToWithConvertAndCreate(SensorDataRecord source, SensorDataRecord dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                dest = source.Clone(name);
            }
            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = source.Name;
            dest.Temperature = convert(source.Temperature, "C");
            dest.Pressure = convert(source.Pressure, "hPA");
            dest.Humidity = convert(source.Humidity, "%");
            dest.PM25 = convert(source.PM25, "ppm");
            dest.CO2 = convert(source.CO2, ""); // TODO; what are the unit names? here and down to luminosity
            dest.VOC = convert(source.VOC, "");
            dest.NOX = convert(source.NOX, "");
            dest.Luminosity = convert(source.Luminosity, "");
            return dest;
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            List<string> headers = new List<string>();
            if (IsSensorPresent.HasFlag(SensorPresent.Temperature)) headers.Add("Temperature");
            if (IsSensorPresent.HasFlag(SensorPresent.Pressure)) headers.Add("Pressure");
            if (IsSensorPresent.HasFlag(SensorPresent.Humidity)) headers.Add("Humidity");
            if (IsSensorPresent.HasFlag(SensorPresent.PM25)) headers.Add("PM25");
            if (IsSensorPresent.HasFlag(SensorPresent.CO2)) headers.Add("CO2");
            if (IsSensorPresent.HasFlag(SensorPresent.VOC)) headers.Add("VOC");
            if (IsSensorPresent.HasFlag(SensorPresent.NOX)) headers.Add("NOX");
            if (IsSensorPresent.HasFlag(SensorPresent.Luminosity)) headers.Add("Luminosity");

            if (IsSensorPresent.HasFlag(SensorPresent.Battery)) headers.Add("Battery");

            return headers.ToArray();
        }

        public override void ExportRow(IExportData exporter)
        {
            if (IsSensorPresent.HasFlag(SensorPresent.Temperature)) exporter.CellSet(Temperature);
            if (IsSensorPresent.HasFlag(SensorPresent.Pressure)) exporter.CellSet(Pressure);
            if (IsSensorPresent.HasFlag(SensorPresent.Humidity)) exporter.CellSet(Humidity);
            if (IsSensorPresent.HasFlag(SensorPresent.PM25)) exporter.CellSet(PM25);
            if (IsSensorPresent.HasFlag(SensorPresent.CO2)) exporter.CellSet(CO2);
            if (IsSensorPresent.HasFlag(SensorPresent.VOC)) exporter.CellSet(VOC);
            if (IsSensorPresent.HasFlag(SensorPresent.NOX)) exporter.CellSet(NOX);
            if (IsSensorPresent.HasFlag(SensorPresent.Luminosity)) exporter.CellSet(Luminosity);
            if (IsSensorPresent.HasFlag(SensorPresent.Battery)) exporter.CellSet(BatteryInPercent);
        }



        public const string TemperaturePropertyChangedName = "Temperature";
        public const string PM25PropertyChangedName = "PM25";
        public const string CO2PropertyChangedName = "CO2";
        public const string VOCPropertyChangedName = "VOC";
        public const string NOXPropertyChangedName = "NOX";
        public const string LuminosityPropertyChangedName = "Luminosity";
        public const string PressurePropertyChangedName = "Pressure";
        public const string HumidityPropertyChangedName = "Humidity";
        public const string BatteryPropertyChangedName = "BatteryInPercent";


        private double _Temperature;
        /// <summary>
        /// Temperature in degrees C
        /// </summary>
        public double Temperature { get { return _Temperature; } set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged(); } }

        private double _Pressure;
        /// <summary>
        /// Pressure in hPA. To convert to Pascal, multiply by 100 (hPA = hecto pascal)
        /// </summary>
        public double Pressure { get { return _Pressure; } set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged(); } }

        private double _Humidity;
        /// <summary>
        /// Humidity in percent
        /// </summary>
        public double Humidity { get { return _Humidity; } set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged(); } }

        private double _PM25;
        /// <summary>
        /// PM2.5 in ug/m3 µg/㎥
        /// </summary>
        public double PM25 { get { return _PM25; } set { if (value == _PM25) return; _PM25 = value; OnPropertyChanged(); } }


        private double _CO2;
        /// <summary>
        /// PM2.5 in ug/m3 µg/㎥
        /// </summary>
        public double CO2 { get { return _CO2; } set { if (value == _CO2) return; _CO2 = value; OnPropertyChanged(); } }


        private double _VOC;
        /// <summary>
        /// PM2.5 in ug/m3 µg/㎥
        /// </summary>
        public double VOC { get { return _VOC; } set { if (value == _VOC) return; _VOC = value; OnPropertyChanged(); } }


        private double _NOX;
        /// <summary>
        /// PM2.5 in ug/m3 µg/㎥
        /// </summary>
        public double NOX { get { return _NOX; } set { if (value == _NOX) return; _NOX = value; OnPropertyChanged(); } }


        private double _Luminosity;
        /// <summary>
        /// PM2.5 in ug/m3 µg/㎥
        /// </summary>
        public double Luminosity { get { return _Luminosity; } set { if (value == _Luminosity) return; _Luminosity = value; OnPropertyChanged(); } }




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
            if (IsSensorPresent.HasFlag(SensorPresent.CO2)) retval += " {CO2}"; // TODO: add units
            if (IsSensorPresent.HasFlag(SensorPresent.VOC)) retval += " {VOC}";
            if (IsSensorPresent.HasFlag(SensorPresent.NOX)) retval += " {NOX}";
            if (IsSensorPresent.HasFlag(SensorPresent.Luminosity)) retval += " {Luminosity}";
            return retval;
        }
    }
}
