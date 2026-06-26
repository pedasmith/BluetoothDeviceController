// Collections to support TAOPE_CyclingSpeedCadence data
//From template: Protocol_Core_Body v2026-04-21 11:40

using System;
using System.Collections.ObjectModel; // Needed for ObservableCollection

using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Needed for CallerMemberNameAttribute
using System.Runtime.InteropServices.WindowsRuntime; // Needed for IBuffer.ToArray extension method
using BluetoothProtocolsDevicesCore;  // Needed for DataCollection

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace BluetoothProtocols.NS_TAOPE_CyclingSpeedCadence
{

    /// <summary>
    /// .
    /// This code was automatically generated 2026-06-25::17:39
    /// </summary>

    ///<summary>
    ///SpeedCadence_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the SpeedCadence_Data group from Cycling Speed and Cadence.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class SpeedCadence_DataCollection : DataCollection<TAOPE_CyclingSpeedCadence.SpeedCadence_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(TAOPE_CyclingSpeedCadence.SpeedCadence_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Data.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(TAOPE_CyclingSpeedCadence.SpeedCadence_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.SpeedCadence_Data value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            // Old code: used to include all the elements as their own array.
            // Timestamps[index] = value.TimestampMostRecent;
            // and everything from  [ [ DataGroupMemberCollectionReplaceMostRecent ] ]
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<TAOPE_CyclingSpeedCadence.SpeedCadence_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.SpeedCadence_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///Feature_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Feature_Data group from Cycling Speed and Cadence.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Feature_DataCollection : DataCollection<TAOPE_CyclingSpeedCadence.Feature_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(TAOPE_CyclingSpeedCadence.Feature_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Data.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(TAOPE_CyclingSpeedCadence.Feature_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Feature_Data value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            // Old code: used to include all the elements as their own array.
            // Timestamps[index] = value.TimestampMostRecent;
            // and everything from  [ [ DataGroupMemberCollectionReplaceMostRecent ] ]
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Feature_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Feature_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///Service_FD00_OTA_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Service_FD00_OTA_Data group from Service_FD00_OTA.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Service_FD00_OTA_DataCollection : DataCollection<TAOPE_CyclingSpeedCadence.Service_FD00_OTA_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(TAOPE_CyclingSpeedCadence.Service_FD00_OTA_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Data.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(TAOPE_CyclingSpeedCadence.Service_FD00_OTA_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Service_FD00_OTA_Data value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            // Old code: used to include all the elements as their own array.
            // Timestamps[index] = value.TimestampMostRecent;
            // and everything from  [ [ DataGroupMemberCollectionReplaceMostRecent ] ]
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Service_FD00_OTA_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Service_FD00_OTA_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///Battery_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Battery_Data group from Battery.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Battery_DataCollection : DataCollection<TAOPE_CyclingSpeedCadence.Battery_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(TAOPE_CyclingSpeedCadence.Battery_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Data.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
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
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Battery_Data value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            // Old code: used to include all the elements as their own array.
            // Timestamps[index] = value.TimestampMostRecent;
            // and everything from  [ [ DataGroupMemberCollectionReplaceMostRecent ] ]
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Battery_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Battery_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///Device Information_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Device Information_Data group from Device Information.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Device_Information_DataCollection : DataCollection<TAOPE_CyclingSpeedCadence.Device_Information_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(TAOPE_CyclingSpeedCadence.Device_Information_Data value, Verb verb)
        {
            if (verb == Verb.ReplaceMostRecent && Data.Count == 0)
            {
                verb = Verb.Add; // Can't replace
            }
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
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(TAOPE_CyclingSpeedCadence.Device_Information_Data value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom (value);  // was value.Clone(); switching to reduce flickering.
            // Old code: used to include all the elements as their own array.
            // Timestamps[index] = value.TimestampMostRecent;
            // and everything from  [ [ DataGroupMemberCollectionReplaceMostRecent ] ]
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<TAOPE_CyclingSpeedCadence.Device_Information_Data> Data { get; } = new ObservableCollection<TAOPE_CyclingSpeedCadence.Device_Information_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif

}