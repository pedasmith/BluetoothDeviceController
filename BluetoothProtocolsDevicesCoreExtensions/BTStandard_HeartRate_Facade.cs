using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static BluetoothProtocols.BTStandard_HeartRate;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    public class Heart_Rate_Data_Facade : BTCommonMetaData<Heart_Rate_Data_Facade> //, IExportDataSource
    {
        /// <summary>
        /// Decoded per specs at https://www.bluetooth.com/specifications/specs/html/?src=HRS_v1.0/out/en/index-en.html#UUID-5a90afed-6e38-b34e-199b-1eae1c2a4b95
        /// section 3.1.1.1
        /// </summary>
        [Flags]
        public enum FlagsDecoded { 
            /// <summary>
            /// 0=pulse uses the 1-byte format and is in the range 0..255
            /// 1=pulse uses 2-bytes format and is 256 or higher
            /// </summary>
            HeartRateValueFormatHighRange = 0x01, 
            /// <summary>
            /// 1=device supports detecting skin contact
            /// </summary>
            SensorContactSupported = 0x02, 
            /// <summary>
            /// 1=skin is detected
            /// </summary>
            SensorContactDetected = 0x04, 
            /// <summary>
            /// 1=the EnergyExpended field is filled in
            /// </summary>
            EnergyExpendedStatus = 0x08,
            /// <summary>
            /// 1=there are RRInterval status values present
            /// </summary>
            RRIntervalStatus = 0x10,
        }
        public FlagsDecoded CurrFlagsDecoded {  get { return (FlagsDecoded)Flags; } }
        public bool PulseRateIsHighRange {  get { return CurrFlagsDecoded.HasFlag(FlagsDecoded.HeartRateValueFormatHighRange); } }

        private double _PulseRate = 0;
        /// <summary>
        /// From Heart Rate and Heart Rate Measurement
        ///</summary>
        public double PulseRate
        {
            get { return _PulseRate; }
            set { if (value == _PulseRate) return; _PulseRate = value; OnPropertyChanged(); }
        }


        //
        // copy-pasted from Heart_Rate_Data and then updated Clone, CopyFrom, CopyToOrClone
        // and radically change CopyToOrClone
        // and changed PulseRateLowRange and PulseRateHighRange to private


        // Template is ServiceDataGroups
        private double _Flags = 0;
        /// <summary>
        /// From Heart Rate and Heart Rate Measurement
        ///</summary>
        public double Flags
        {
            get { return _Flags; }
            set { if (value == _Flags) return; _Flags = value; OnPropertyChanged(); }
        }

        private double _EnergyExpended = 0;
        /// <summary>
        /// From Heart Rate and Heart Rate Measurement
        ///</summary>
        public double EnergyExpended
        {
            get { return _EnergyExpended; }
            set { if (value == _EnergyExpended) return; _EnergyExpended = value; OnPropertyChanged(); }
        }
        private List<double> _RRInterval = new();
        /// <summary>
        /// From Heart Rate and Heart Rate Measurement
        ///</summary>
        public List<double> RRInterval
        {
            get { return _RRInterval; }
            set { if (value == _RRInterval) return; _RRInterval = value; OnPropertyChanged(); }
        }
        private double _SensorLocation = 0;
        /// <summary>
        /// From Heart Rate and Body Sensor Location
        ///</summary>
        public double SensorLocation
        {
            get { return _SensorLocation; }
            set { if (value == _SensorLocation) return; _SensorLocation = value; OnPropertyChanged(); }
        }
        public override Heart_Rate_Data_Facade Clone(string name = null)
        {
            var retval = this.MemberwiseClone() as Heart_Rate_Data_Facade;
            if (name != null)
            {
                retval.Name = name;
            }
            return retval;
        }

        public override void CopyFrom(Heart_Rate_Data_Facade value)
        {
            this.TimestampMostRecent = value.TimestampMostRecent;
            this.Name = value.Name;
            this.Flags = value.Flags;
            this.PulseRate = value.PulseRate;
            this.EnergyExpended = value.EnergyExpended;
            this.RRInterval = value.RRInterval;
            this.SensorLocation = value.SensorLocation;
        }

        // CopyFrom, but convert the doubles as appropriate
        public static Heart_Rate_Data_Facade CopyToOrClone(Heart_Rate_Data source, Heart_Rate_Data_Facade dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                dest = new(); // was: source.Clone(name);
            }
            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = source.Name;
            dest.Flags = convert(source.Flags, "");
            var pulse = dest.CurrFlagsDecoded.HasFlag(FlagsDecoded.HeartRateValueFormatHighRange) ? source.PulseRateHighRange : source.PulseRateLowRange;
            dest.PulseRate = convert(pulse, "bpm");
            dest.EnergyExpended = convert(source.EnergyExpended, "Joules"); dest.RRInterval = source.RRInterval;
            dest.SensorLocation = convert(source.SensorLocation, "");
            return dest;
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            return ["Flags", "PulseRateLowRange", "PulseRateHighRange", "EnergyExpended", "RRInterval", "SensorLocation"];
        }

        public override void ExportRow(IExportData exporter)
        {
            // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
            // RowEnd and add in the timestamps
            exporter.CellSet(CurrFlagsDecoded.ToString());
            exporter.CellSet(PulseRate);
            exporter.CellSet(EnergyExpended);
            exporter.CellSet(RRInterval);
            exporter.CellSet(SensorLocation);
        }

        public override string ToString()
        {
            return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {CurrFlagsDecoded} {PulseRate} {EnergyExpended} {RRInterval} {SensorLocation}");
        }
    }
}
