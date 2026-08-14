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
    public class HealthDataRecord : BTCommonMetaData<HealthDataRecord>
    {
        /* Adding more fields? Here's a quick guide:
         * 1. When you add a new field, you must also bump the .All value. 
         */
        [Flags]
        public enum SensorPresent
        {
            None = 0x00,
            PulseRate = 0x01, OxygenSaturationInPercent = 0x02, PerfusionIndexInPercent = 0x04,

            Battery = 0x80, // TODO: handle like SensorDataRecord??
            All = 0x07,
        };
        public SensorPresent IsSensorPresent { get; set; } = SensorPresent.All;

        public HealthDataRecord()
        {
            PulseRate = double.NaN;
            OxygenSaturationInPercent = double.NaN;
            PerfusionIndexInPercent = double.NaN;
            TimestampMostRecent = DateTimeOffset.Now;
        }
        public HealthDataRecord(double pulseRate, double oxygenSaturationInPercent, double perfusionIndexInPercent, DateTimeOffset? eventTime)
        {
            PulseRate = pulseRate;
            OxygenSaturationInPercent = oxygenSaturationInPercent;
            PerfusionIndexInPercent = perfusionIndexInPercent;
            TimestampMostRecent = eventTime ?? DateTimeOffset.Now;
            IsSensorPresent = SensorPresent.PulseRate | SensorPresent.OxygenSaturationInPercent | SensorPresent.PerfusionIndexInPercent;
        }

        public override HealthDataRecord Clone(string name = null)
        {
            var retval = this.MemberwiseClone() as HealthDataRecord;
            if (name != null)
            {
                retval.Name = name;
            }
            return retval;
        }

        public override void CopyFrom(HealthDataRecord value)
        {
            this.TimestampMostRecent = value.TimestampMostRecent;
            this.Name = value.Name;
            this.PulseRate = value.PulseRate;
            this.OxygenSaturationInPercent = value.OxygenSaturationInPercent;
            this.PerfusionIndexInPercent = value.PerfusionIndexInPercent;
        }

        // CopyFrom, but convert the doubles as appropriate
        public static HealthDataRecord CopyToWithConvertAndCreate(HealthDataRecord source, HealthDataRecord dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                dest = source.Clone(name);
            }
            dest.TimestampMostRecent = source.TimestampMostRecent;
            if (string.IsNullOrEmpty(dest.Name)) dest.Name = source.Name;
            dest.PulseRate = convert(source.PulseRate, "");
            dest.OxygenSaturationInPercent = convert(source.OxygenSaturationInPercent, "");
            dest.PerfusionIndexInPercent = convert(source.PerfusionIndexInPercent, "");
            return dest;
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            List<string> headers = new List<string>();
            if (IsSensorPresent.HasFlag(SensorPresent.PulseRate)) headers.Add("PulseRate");
            if (IsSensorPresent.HasFlag(SensorPresent.OxygenSaturationInPercent)) headers.Add("OxygenSaturationInPercent");
            if (IsSensorPresent.HasFlag(SensorPresent.PerfusionIndexInPercent)) headers.Add("PerfusionIndexInPercent");

            if (IsSensorPresent.HasFlag(SensorPresent.Battery)) headers.Add("Battery");

            return headers.ToArray();
        }

        public override void ExportRow(IExportData exporter)
        {
            if (IsSensorPresent.HasFlag(SensorPresent.PulseRate)) exporter.CellSet(PulseRate);
            if (IsSensorPresent.HasFlag(SensorPresent.OxygenSaturationInPercent)) exporter.CellSet(OxygenSaturationInPercent);
            if (IsSensorPresent.HasFlag(SensorPresent.PerfusionIndexInPercent)) exporter.CellSet(PerfusionIndexInPercent);
            if (IsSensorPresent.HasFlag(SensorPresent.Battery)) exporter.CellSet(BatteryInPercent);
        }



        public const string PulseRatePropertyChangedName = "PulseRate";
        public const string OxygenSaturationInPercentPropertyChangedName = "OxygenSaturationInPercent";
        public const string PerfusionIndexInPercentPropertyChangedName = "PerfusionIndexInPercent";
        public const string BatteryPropertyChangedName = "BatteryInPercent";


        private double _PulseRate;
        /// <summary>
        /// Temperature in degrees C
        /// </summary>
        public double PulseRate { get { return _PulseRate; } set { if (value == _PulseRate) return; _PulseRate = value; OnPropertyChanged(); } }

        private double _OxygenSaturationInPercent;
        /// <summary>
        /// Pressure in hPA. To convert to Pascal, multiply by 100 (hPA = hecto pascal)
        /// </summary>
        public double OxygenSaturationInPercent { get { return _OxygenSaturationInPercent; } set { if (value == _OxygenSaturationInPercent) return; _OxygenSaturationInPercent = value; OnPropertyChanged(); } }

        private double _PerfusionIndexInPercent;
        /// <summary>
        /// Humidity in percent
        /// </summary>
        public double PerfusionIndexInPercent { get { return _PerfusionIndexInPercent; } set { if (value == _PerfusionIndexInPercent) return; _PerfusionIndexInPercent = value; OnPropertyChanged(); } }



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
            if (IsSensorPresent.HasFlag(SensorPresent.PulseRate)) retval += " {PulseRate} bpm";
            if (IsSensorPresent.HasFlag(SensorPresent.OxygenSaturationInPercent)) retval += " {OxygenSaturationInPercent}%";
            if (IsSensorPresent.HasFlag(SensorPresent.PerfusionIndexInPercent)) retval += " {PerfusionIndexInPercent}";
            return retval;
        }
    }
}
