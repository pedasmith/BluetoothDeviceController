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
            /// 0=heart rate uses the 1-byte format and is in the range 0..255
            /// 1=heart rate uses 2-bytes format and is 256 or higher
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
        private bool HeartRateIsHighRange {  get { return CurrFlagsDecoded.HasFlag(FlagsDecoded.HeartRateValueFormatHighRange); } }

        /// <summary>
        /// Returns the SensorLocation as a string per the YAML description from the 
        /// Bluetooth SIG.
        /// </summary>
        public string Sensor
        {
            get
            {
                var retval = BluetoothConversions.BluetoothBodySensorLocation.Decode((byte)SensorLocation);
                return retval;
            }
        }

        private double _HeartRate = 0;
        /// <summary>
        /// From Heart Rate and Heart Rate Measurement
        ///</summary>
        public double HeartRate
        {
            get { return _HeartRate; }
            set { if (value == _HeartRate) return; _HeartRate = value; OnPropertyChanged(); }
        }

        public RRRecent CurrRRRecent { get; set; } = new RRRecent();
        public class RRRecent
        {
            DateTimeOffset MostRecentUpdate = DateTimeOffset.MinValue;

            /// <summary>
            /// The newData's last value is the most recent, and started at the updateTime
            /// minus the number of milliseconds
            /// </summary>
            public void AddRRData(DateTimeOffset updateTime, List<double> newData)
            {
                if (MostRecentUpdate == updateTime)
                {
                    ; // handy place for a debugger
                    // Is a duplicate; ignore it.
                    return;
                }
                MostRecentUpdate = updateTime;
                DateTimeOffset curr = updateTime;
                for (int i=newData.Count-1; i>= 0; i--)
                {
                    double milliseconds = newData[i];
                    curr = curr.AddMilliseconds(-milliseconds); // subtract is "add the negative"
                    RRData.Add((curr, milliseconds));
                }
            }

            public void DoClearAccumulatedFineGrainedData()
            {
                RRData.Clear();
            }


            public string RRAsString()
            {
                if (RRData.Count == 0) return "[]";
                StringBuilder retval = new();
                foreach (var item in RRData)
                {
                    if (retval.Length != 0) retval.Append(", ");
                    retval.Append(item.RR.ToString());
                }
                return "[" + retval.ToString() + "]";
            }

            public override string ToString()
            {
                return RRAsString();
            }

            private List<(DateTimeOffset Time, double RR)> RRData = new();
        }

        #region From_Heart_Rate_Data
        //
        // copy-pasted from Heart_Rate_Data and then updated Clone, CopyFrom, CopyToWithConvertAndCreate
        // and radically change CopyToWithConvertAndCreate
        // and changed HeartRateLowRange and HeartRateHighRange to private


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
            this.HeartRate = value.HeartRate;
            this.EnergyExpended = value.EnergyExpended;
            this.RRInterval = new List<double>(value.RRInterval);
            this.SensorLocation = value.SensorLocation;
        }

        // List CopyFrom, but convert the doubles as appropriate
        public static Heart_Rate_Data_Facade CopyToWithConvertAndCreate(Heart_Rate_Data source, Heart_Rate_Data_Facade dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                dest = new(); // Can't clone from source because it's a different type.
                // UpdateFacade: if the source changes to have more fields, must update this method.
            }
            var heartRate = dest.CurrFlagsDecoded.HasFlag(FlagsDecoded.HeartRateValueFormatHighRange) ? source.HeartRateHighRange : source.HeartRateLowRange; 

            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
            dest.Flags = convert(source.Flags, "");
            dest.HeartRate = convert(heartRate, "bpm");
            dest.EnergyExpended = convert(source.EnergyExpended, "Joules");
            if (source.RRInterval != null)
            {
                dest.RRInterval = new List<double>();
                foreach (var value in source.RRInterval)
                {
                    double newvalue = convert(value, "");
                    dest.RRInterval.Add(newvalue);
                }
            }
            dest.SensorLocation = convert(source.SensorLocation, "");

            return dest;
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            return ["Sensor", "HeartRate", "RRInterval", "EnergyExpended"];
        }

        public override void ExportRow(IExportData exporter)
        {
            // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
            // RowEnd and add in the timestamps
            exporter.CellSet(Sensor);
            exporter.CellSet(HeartRate);
            exporter.CellSet(this?.CurrRRRecent?.RRAsString() ?? "[]");
            exporter.CellSet(EnergyExpended);
        }

        public override string ToString()
        {
            return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {CurrFlagsDecoded} {HeartRate} {EnergyExpended} {RRInterval} {SensorLocation}");
        }

        #endregion
    }
}
