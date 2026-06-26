using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocolsDevicesCore
{
    ///<summary>
    /// DataCollection contains an ObservableCollection of sensor data. This is used
    /// instead of the (old) mechanism that made per-sensor data with collections for
    /// each individual variable (some graphs require this)
    /// The Data list is used when displaying historical graphs of the data, tables,
    /// and for exporting data.
    ///</summary>
    public class DataCollection<SensorData> where SensorData : BTCommonMetaData<SensorData>
    {
        public enum Verb { Add, ReplaceMostRecent };

        public int Count { get { return Data.Count; } }

        public void Update(SensorData value, Verb verb)
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

        public void Add(SensorData value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add(value.Clone() as SensorData);
        }
        public void ReplaceMostRecent(SensorData value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom(value);  // was value.Clone(); switching to reduce flickering.
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<SensorData> Data { get; } = new ObservableCollection<SensorData>();
    }
}
