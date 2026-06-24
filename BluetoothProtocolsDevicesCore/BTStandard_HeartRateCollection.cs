// Collections to support BTStandard_HeartRate data
//From template: Protocol_Core_Body v2026-04-21 11:40

using System;
using System.Collections.ObjectModel; // Needed for ObservableCollection

using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Needed for CallerMemberNameAttribute
using System.Runtime.InteropServices.WindowsRuntime; // Needed for IBuffer.ToArray extension method

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace BluetoothProtocols.NS_BTStandard_HeartRate
{

    /// <summary>
    /// .
    /// This code was automatically generated 2026-06-23::19:15
    /// </summary>

    ///<summary>
    ///TODO:
    ///Heart Rate_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Heart Rate_Data group from Heart Rate.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Heart_Rate_DataCollection 
    {
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Timestamps.Count; } } 

        public void Update(BTStandard_HeartRate.Heart_Rate_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Timestamps.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(BTStandard_HeartRate.Heart_Rate_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Flags.Add (value.Flags);
            PulseRate.Add (value.PulseRate);
            PulseRateHighRes.Add (value.PulseRateHighRes);
            EnergyExpended.Add (value.EnergyExpended);
            RRInterval.Add (value.RRInterval);
            Unknown1.Add (value.Unknown1);
        }
        public void ReplaceMostRecent(BTStandard_HeartRate.Heart_Rate_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            Flags[index] = value.Flags;
            PulseRate[index] = value.PulseRate;
            PulseRateHighRes[index] = value.PulseRateHighRes;
            EnergyExpended[index] = value.EnergyExpended;
            RRInterval[index] = value.RRInterval;
            Unknown1[index] = value.Unknown1;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Heart Rate Measurement
        public ObservableCollection<double> Flags { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> PulseRate { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> PulseRateHighRes { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> EnergyExpended { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> RRInterval { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Body Sensor Location
        public ObservableCollection<byte[]> Unknown1 { get; } = new ObservableCollection<byte[]>();
        public ObservableCollection<BTStandard_HeartRate.Heart_Rate_Data> Data { get; } = new ObservableCollection<BTStandard_HeartRate.Heart_Rate_Data>();
    }
    ///<summary>
    ///TODO:
    ///GAP_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the GAP_Data group from GAP.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class GAP_DataCollection 
    {
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Timestamps.Count; } } 

        public void Update(BTStandard_HeartRate.GAP_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Timestamps.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(BTStandard_HeartRate.GAP_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            DeviceName.Add (value.DeviceName);
            Appearance.Add (value.Appearance);
            ConnectionParameters.Add (value.ConnectionParameters);
        }
        public void ReplaceMostRecent(BTStandard_HeartRate.GAP_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            DeviceName[index] = value.DeviceName;
            Appearance[index] = value.Appearance;
            ConnectionParameters[index] = value.ConnectionParameters;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Device Name
        public ObservableCollection<string> DeviceName { get; } = new ObservableCollection<string>();
        // Data values (properties) from characteristic Appearance
        public ObservableCollection<double> Appearance { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Peripheral Preferred Connection Parameters
        public ObservableCollection<byte[]> ConnectionParameters { get; } = new ObservableCollection<byte[]>();
        public ObservableCollection<BTStandard_HeartRate.GAP_Data> Data { get; } = new ObservableCollection<BTStandard_HeartRate.GAP_Data>();
    }
    ///<summary>
    ///TODO:
    ///Battery_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Battery_Data group from Battery.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Battery_DataCollection 
    {
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Timestamps.Count; } } 

        public void Update(BTStandard_HeartRate.Battery_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Timestamps.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(BTStandard_HeartRate.Battery_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            TransmitPower.Add (value.TransmitPower);
            BatteryLevel.Add (value.BatteryLevel);
        }
        public void ReplaceMostRecent(BTStandard_HeartRate.Battery_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            TransmitPower[index] = value.TransmitPower;
            BatteryLevel[index] = value.BatteryLevel;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Transmit Power
        public ObservableCollection<double> TransmitPower { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic BatteryLevel
        public ObservableCollection<double> BatteryLevel { get; } = new ObservableCollection<double>();
        public ObservableCollection<BTStandard_HeartRate.Battery_Data> Data { get; } = new ObservableCollection<BTStandard_HeartRate.Battery_Data>();
    }
    ///<summary>
    ///TODO:
    ///Device Information_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Device Information_Data group from Device Information.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Device_Information_DataCollection 
    {
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Timestamps.Count; } } 

        public void Update(BTStandard_HeartRate.Device_Information_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Timestamps.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(BTStandard_HeartRate.Device_Information_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Manufacturer.Add (value.Manufacturer);
            ModelNumber.Add (value.ModelNumber);
            HardwareRevision.Add (value.HardwareRevision);
            FirmwareRevision.Add (value.FirmwareRevision);
            SoftwareRevision.Add (value.SoftwareRevision);
            SystemID.Add (value.SystemID);
        }
        public void ReplaceMostRecent(BTStandard_HeartRate.Device_Information_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            Manufacturer[index] = value.Manufacturer;
            ModelNumber[index] = value.ModelNumber;
            HardwareRevision[index] = value.HardwareRevision;
            FirmwareRevision[index] = value.FirmwareRevision;
            SoftwareRevision[index] = value.SoftwareRevision;
            SystemID[index] = value.SystemID;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Manufacturer Name String
        public ObservableCollection<string> Manufacturer { get; } = new ObservableCollection<string>();
        // Data values (properties) from characteristic Model Number String
        public ObservableCollection<string> ModelNumber { get; } = new ObservableCollection<string>();
        // Data values (properties) from characteristic Hardware Revision String
        public ObservableCollection<string> HardwareRevision { get; } = new ObservableCollection<string>();
        // Data values (properties) from characteristic Firmware Revision String
        public ObservableCollection<string> FirmwareRevision { get; } = new ObservableCollection<string>();
        // Data values (properties) from characteristic Software Revision String
        public ObservableCollection<string> SoftwareRevision { get; } = new ObservableCollection<string>();
        // Data values (properties) from characteristic System ID
        public ObservableCollection<byte[]> SystemID { get; } = new ObservableCollection<byte[]>();
        public ObservableCollection<BTStandard_HeartRate.Device_Information_Data> Data { get; } = new ObservableCollection<BTStandard_HeartRate.Device_Information_Data>();
    }

}