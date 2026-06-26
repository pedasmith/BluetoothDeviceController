// Collections to support Nordic_Thingy data
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


namespace BluetoothProtocols.NS_Nordic_Thingy
{

    /// <summary>
    /// The Nordic Thingy:52™ is an easy-to-use prototyping platform, designed to help in building prototypes and demos, without the need to build hardware or even write firmware. It is built around the nRF52832 Bluetooth 5 SoC.
    /// This code was automatically generated 2026-06-25::17:39
    /// </summary>

    ///<summary>
    ///Environment_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Environment_Data group from Environment.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Environment_DataCollection : DataCollection<Nordic_Thingy.Environment_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(Nordic_Thingy.Environment_Data value, Verb verb)
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

        public void Add(Nordic_Thingy.Environment_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(Nordic_Thingy.Environment_Data value)
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
        public ObservableCollection<Nordic_Thingy.Environment_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.Environment_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///EnvironmentColor_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the EnvironmentColor_Data group from Environment.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class EnvironmentColor_DataCollection : DataCollection<Nordic_Thingy.EnvironmentColor_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(Nordic_Thingy.EnvironmentColor_Data value, Verb verb)
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

        public void Add(Nordic_Thingy.EnvironmentColor_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(Nordic_Thingy.EnvironmentColor_Data value)
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
        public ObservableCollection<Nordic_Thingy.EnvironmentColor_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.EnvironmentColor_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///EnvironmentConfiguration_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the EnvironmentConfiguration_Data group from Environment.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class EnvironmentConfiguration_DataCollection : DataCollection<Nordic_Thingy.EnvironmentConfiguration_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(Nordic_Thingy.EnvironmentConfiguration_Data value, Verb verb)
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

        public void Add(Nordic_Thingy.EnvironmentConfiguration_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(Nordic_Thingy.EnvironmentConfiguration_Data value)
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
        public ObservableCollection<Nordic_Thingy.EnvironmentConfiguration_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.EnvironmentConfiguration_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///Common Configuration_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Common Configuration_Data group from Common Configuration.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Common_Configuration_DataCollection : DataCollection<Nordic_Thingy.Common_Configuration_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(Nordic_Thingy.Common_Configuration_Data value, Verb verb)
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

        public void Add(Nordic_Thingy.Common_Configuration_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(Nordic_Thingy.Common_Configuration_Data value)
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
        public ObservableCollection<Nordic_Thingy.Common_Configuration_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.Common_Configuration_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///Generic Service_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Generic Service_Data group from Generic Service.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Generic_Service_DataCollection : DataCollection<Nordic_Thingy.Generic_Service_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(Nordic_Thingy.Generic_Service_Data value, Verb verb)
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

        public void Add(Nordic_Thingy.Generic_Service_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(Nordic_Thingy.Generic_Service_Data value)
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
        public ObservableCollection<Nordic_Thingy.Generic_Service_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.Generic_Service_Data>();

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
    public class Battery_DataCollection : DataCollection<Nordic_Thingy.Battery_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(Nordic_Thingy.Battery_Data value, Verb verb)
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

        public void Add(Nordic_Thingy.Battery_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(Nordic_Thingy.Battery_Data value)
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
        public ObservableCollection<Nordic_Thingy.Battery_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.Battery_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif
    ///<summary>
    ///Configuration_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Configuration_Data group from Configuration.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Configuration_DataCollection : DataCollection<Nordic_Thingy.Configuration_Data>
    {
    }

    #if NEVER_EVER_DEFINED
        public enum Verb {  Add, ReplaceMostRecent };

        public int Count { get { return  Data.Count; } } 

        public void Update(Nordic_Thingy.Configuration_Data value, Verb verb)
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

        public void Add(Nordic_Thingy.Configuration_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            // Timestamps.Add (value.TimestampMostRecent);
            // TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            // Old code: used to include all the elements as their own array.
            // and everything from [ [ DataGroupMemberCollectionAdd ] ]
        }
        public void ReplaceMostRecent(Nordic_Thingy.Configuration_Data value)
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
        public ObservableCollection<Nordic_Thingy.Configuration_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.Configuration_Data>();

        // Old code: used to include all the elements as their own array.
        // public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        // public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // and everything from [ [ DataGroupMemberCollection ] ]

        #endif

}