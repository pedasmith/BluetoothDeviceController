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
    /// <summary>
    /// Based on SensorDataRecord but also knows about CurrUserPrefs for updating units.
    /// </summary>
    public class CopyableSensorDataRecord : SensorDataRecord
    {
        public bool IsValid { get; set; } = true;
        /// <summary>
        /// Special case: when IsValid is false, we often want to log that we got invalid data.
        /// But that's not true for the Ruuvi Air
        /// </summary>
        public bool IsIgnored { get; set; } = false; // when IsValid==false, IsIgnored sets whether we shouldn't even log it.

        public virtual CopyableSensorDataRecord Clone()
        {
            return this.MemberwiseClone() as Govee;
        }

        public virtual void CopyFrom(CopyableSensorDataRecord value)
        {
            base.CopyFrom(value);
            // In reality, IsValid is always true here and IsIgnored false.
            IsValid = value.IsValid;
            IsIgnored = value.IsIgnored;
#if NEVER_EVER_DEFINED
            TimestampMostRecent = value.TimestampMostRecent;
            Temperature = value.Temperature;
            Pressure = value.Pressure;
            Humidity = value.Humidity;
            PM25 = value.PM25; // TODO: add in all other for Ruuvi Air!
            BatteryInPercent = value.BatteryInPercent;
            Name = value.Name;
#endif
        }

        public virtual CopyableSensorDataRecord CopyToAndUpdateUnits(CopyableSensorDataRecord dest, UserPreferences CurrUserPrefs, string knownDeviceName)
        {
            if (dest == null)
            {
                dest = this.Clone();
                dest.Name = knownDeviceName;
                // the protocol Name is the "SupportedDevice" name. It's not unique to each one.
                // What we need for our data is the name that the user might have given the 
                // device (the "known device" name). It's set in the UpdateUX from SaveData
            }
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

            dest.PM10 = PM10;
            dest.PM25 = PM25;
            dest.PM40 = PM40;
            dest.PM100 = PM100;
            dest.CO2 = CO2;
            dest.NOXIndex = NOXIndex;
            dest.VOC = VOC;
            dest.Luminosity = Luminosity;
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
