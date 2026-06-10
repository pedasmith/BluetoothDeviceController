// Collections to support TAOPE_CyclingSpeedCadence data
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


namespace BluetoothProtocols.NS_TAOPE_CyclingSpeedCadence
{

    /// <summary>
    /// .
    /// This code was automatically generated 2026-06-09::12:02
    /// </summary>

    ///<summary>
    ///TODO:
    ///Cycling Speed and Cadence_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Cycling Speed and Cadence_Data group from Cycling Speed and Cadence.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Cycling_Speed_and_Cadence_DataCollection 
    {
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Timestamps.Count; } } 

        public void Update(TAOPE_CyclingSpeedCadence.Cycling_Speed_and_Cadence_Data value, Verb verb)
        {
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(TAOPE_CyclingSpeedCadence.Cycling_Speed_and_Cadence_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Flags.Add (value.Flags);
            RevolutionA.Add (value.RevolutionA);
            TimeA.Add (value.TimeA);
            RevolutionB.Add (value.RevolutionB);
            TimeB.Add (value.TimeB);
            FeatureFlags.Add (value.FeatureFlags);
            SensorLocation.Add (value.SensorLocation);
            Unknown3.Add (value.Unknown3);
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Cycling_Speed_and_Cadence_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            Flags[index] = value.Flags;
            RevolutionA[index] = value.RevolutionA;
            TimeA[index] = value.TimeA;
            RevolutionB[index] = value.RevolutionB;
            TimeB[index] = value.TimeB;
            FeatureFlags[index] = value.FeatureFlags;
            SensorLocation[index] = value.SensorLocation;
            Unknown3[index] = value.Unknown3;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic CSC Measurement
        public ObservableCollection<double> Flags { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> RevolutionA { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> TimeA { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> RevolutionB { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> TimeB { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic CSC Feature
        public ObservableCollection<double> FeatureFlags { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Sensor Location
        public ObservableCollection<double> SensorLocation { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic SC Control Point
        public ObservableCollection<byte[]> Unknown3 { get; } = new ObservableCollection<byte[]>();
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Cycling_Speed_and_Cadence_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Cycling_Speed_and_Cadence_Data>();
    }
    ///<summary>
    ///TODO:
    ///Service_FD00_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Service_FD00_Data group from Service_FD00.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Service_FD00_DataCollection 
    {
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Timestamps.Count; } } 

        public void Update(TAOPE_CyclingSpeedCadence.Service_FD00_Data value, Verb verb)
        {
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(TAOPE_CyclingSpeedCadence.Service_FD00_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Unknown0.Add (value.Unknown0);
            Unknown1.Add (value.Unknown1);
            Unknown2.Add (value.Unknown2);
            Unknown3.Add (value.Unknown3);
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Service_FD00_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            Unknown0[index] = value.Unknown0;
            Unknown1[index] = value.Unknown1;
            Unknown2[index] = value.Unknown2;
            Unknown3[index] = value.Unknown3;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic FD09
        public ObservableCollection<byte[]> Unknown0 { get; } = new ObservableCollection<byte[]>();
        // Data values (properties) from characteristic FD0A
        public ObservableCollection<byte[]> Unknown1 { get; } = new ObservableCollection<byte[]>();
        // Data values (properties) from characteristic FD19
        public ObservableCollection<byte[]> Unknown2 { get; } = new ObservableCollection<byte[]>();
        // Data values (properties) from characteristic FD1A
        public ObservableCollection<byte[]> Unknown3 { get; } = new ObservableCollection<byte[]>();
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Service_FD00_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Service_FD00_Data>();
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

        public void Update(TAOPE_CyclingSpeedCadence.Battery_Data value, Verb verb)
        {
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(TAOPE_CyclingSpeedCadence.Battery_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Unknown0.Add (value.Unknown0);
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Battery_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            Unknown0[index] = value.Unknown0;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Battery Level
        public ObservableCollection<byte[]> Unknown0 { get; } = new ObservableCollection<byte[]>();
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Battery_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Battery_Data>();
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

        public void Update(TAOPE_CyclingSpeedCadence.Device_Information_Data value, Verb verb)
        {
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(TAOPE_CyclingSpeedCadence.Device_Information_Data value)
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
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Device_Information_Data value)
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
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Device_Information_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Device_Information_Data>();
    }

}