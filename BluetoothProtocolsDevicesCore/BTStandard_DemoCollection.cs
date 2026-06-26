// Collections to support BTStandard_Demo data
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


namespace BluetoothProtocols.NS_BTStandard_Demo
{

    /// <summary>
    /// Used to demonstrate adding new Bluetooth devices that require connecting to a device.
    /// This code was automatically generated 2026-06-25::17:39
    /// </summary>

    ///<summary>
    ///Common Configuration_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Common Configuration_Data group from Common Configuration.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Common_Configuration_DataCollection : DataCollection<BTStandard_Demo.Common_Configuration_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(BTStandard_Demo.Common_Configuration_Data value, Verb verb)
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

        public void Add(BTStandard_Demo.Common_Configuration_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(BTStandard_Demo.Common_Configuration_Data value)
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
        public ObservableCollection<BTStandard_Demo.Common_Configuration_Data> Data { get; } = new ObservableCollection<BTStandard_Demo.Common_Configuration_Data>();

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
    public class Battery_DataCollection : DataCollection<BTStandard_Demo.Battery_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(BTStandard_Demo.Battery_Data value, Verb verb)
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

        public void Add(BTStandard_Demo.Battery_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(BTStandard_Demo.Battery_Data value)
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
        public ObservableCollection<BTStandard_Demo.Battery_Data> Data { get; } = new ObservableCollection<BTStandard_Demo.Battery_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif

}