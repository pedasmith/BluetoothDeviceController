// Collections to support BTStandard_Demo data
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


namespace BluetoothProtocols.NS_BTStandard_Demo
{

    /// <summary>
    /// Used to demonstrate adding new Bluetooth devices that require connecting to a device.
    /// This code was automatically generated 2026-06-15::11:34
    /// </summary>

    ///<summary>
    ///TODO:
    ///Common Configuration_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Common Configuration_Data group from Common Configuration.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Common_Configuration_DataCollection 
    {
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Timestamps.Count; } } 

        public void Update(BTStandard_Demo.Common_Configuration_Data value, Verb verb)
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

        public void Add(BTStandard_Demo.Common_Configuration_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Device_Name.Add (value.Device_Name);
            Interval_Min.Add (value.Interval_Min);
            Interval_Max.Add (value.Interval_Max);
            Latency.Add (value.Latency);
            Timeout.Add (value.Timeout);
        }
        public void ReplaceMostRecent(BTStandard_Demo.Common_Configuration_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            Device_Name[index] = value.Device_Name;
            Interval_Min[index] = value.Interval_Min;
            Interval_Max[index] = value.Interval_Max;
            Latency[index] = value.Latency;
            Timeout[index] = value.Timeout;
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
        public ObservableCollection<string> Device_Name { get; } = new ObservableCollection<string>();
        // Data values (properties) from characteristic Connection Parameter
        public ObservableCollection<double> Interval_Min { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Interval_Max { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Latency { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Timeout { get; } = new ObservableCollection<double>();
        public ObservableCollection<BTStandard_Demo.Common_Configuration_Data> Data { get; } = new ObservableCollection<BTStandard_Demo.Common_Configuration_Data>();
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

        public void Update(BTStandard_Demo.Battery_Data value, Verb verb)
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

        public void Add(BTStandard_Demo.Battery_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            BatteryLevel.Add (value.BatteryLevel);
        }
        public void ReplaceMostRecent(BTStandard_Demo.Battery_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
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
        // Data values (properties) from characteristic BatteryLevel
        public ObservableCollection<double> BatteryLevel { get; } = new ObservableCollection<double>();
        public ObservableCollection<BTStandard_Demo.Battery_Data> Data { get; } = new ObservableCollection<BTStandard_Demo.Battery_Data>();
    }

}