using BluetoothWinUI3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Text;


#if NET8_0_OR_GREATER
#nullable disable
#endif


#if NEVER_EVER_DEFINED

Replace with DataCollection

namespace BluetoothProtocols
{

    ///<summary>
    ///SensorDataCollection is an ObservableCollection of sensor data
    ///</summary>
    public class SensorDataCollection<CopyableDataRecord> where CopyableDataRecord : SensorDataRecordCopyable // e.g., SensorDataRecordCopyable
    {
        public enum Verb { Add, ReplaceMostRecent };

        public int Count { get { return Data.Count; } }

        public void Update(CopyableDataRecord value, Verb verb)
        {
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(CopyableDataRecord value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add(value.Clone() as CopyableDataRecord);
        }
        public void ReplaceMostRecent(CopyableDataRecord value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom(value);  // was value.Clone(); switching to reduce flickering.
        }
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; }
        public ObservableCollection<CopyableDataRecord> Data { get; } = new ObservableCollection<CopyableDataRecord>();
    }
}

#endif