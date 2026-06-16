using BluetoothWinUI3;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;


#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace BluetoothProtocols
{
    public class CopyableSensorDataRecord : SensorDataRecord
    {
        public bool IsValid { get; set; } = true;

        public virtual CopyableSensorDataRecord Clone()
        {
            return this.MemberwiseClone() as Govee;
        }

        public virtual void CopyFrom(CopyableSensorDataRecord value)
        {
            TimestampMostRecent = value.TimestampMostRecent;
            Temperature = value.Temperature;
            Pressure = value.Pressure;
            Humidity = value.Humidity; // Humidity is always in percent, so no conversion needed.
            PM25 = value.PM25;
            BatteryInPercent = value.BatteryInPercent;
            Name = value.Name;
        }

        public virtual CopyableSensorDataRecord CopyToAndUpdateUnits(CopyableSensorDataRecord dest, UserPreferences CurrUserPrefs)
        {
            dest ??= this.Clone();
            dest.TimestampMostRecent = TimestampMostRecent;
            dest.Temperature = BluetoothWatcher.Units.Temperature.Convert(
                Temperature,
                BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius,
                CurrUserPrefs.Temperature);
            dest.Pressure = BluetoothWatcher.Units.Pressure.Convert(
                Pressure,
                BluetoothWatcher.Units.Pressure.PressureUnit.hectoPascal_milliBar,
                CurrUserPrefs.Pressure);
            dest.Humidity = Humidity; // Humidity is always in percent, so no conversion needed.
            dest.PM25 = PM25;
            dest.BatteryInPercent = BatteryInPercent;
            dest.Name = Name;

            return dest;
        }
    }

    ///<summary>
    ///SensorDataCollection is an ObservableCollection of sensor data
    ///</summary>
    public class SensorDataCollection
    {
        public enum Verb { Add, ReplaceMostRecent };

        public int Count { get { return Data.Count; } }

        public void Update(CopyableSensorDataRecord value, Verb verb)
        {
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(CopyableSensorDataRecord value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add(value.Clone());
        }
        public void ReplaceMostRecent(CopyableSensorDataRecord value)
        {
            var index = Data.Count - 1;
            Data[index].CopyFrom(value);  // was value.Clone(); switching to reduce flickering.
        }
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; }
        public ObservableCollection<CopyableSensorDataRecord> Data { get; } = new ObservableCollection<CopyableSensorDataRecord>();
    }
}
