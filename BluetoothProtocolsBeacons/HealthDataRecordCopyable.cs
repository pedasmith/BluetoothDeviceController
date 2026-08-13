using BluetoothProtocols;
using BluetoothWatcher.Units;
using BluetoothWinUI3;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    /// <summary>
    /// Based on HealthDataRecord but also knows about CurrUserPrefs for updating units.
    /// </summary>
    public class HealthDataRecordCopyable : HealthDataRecord
    {
        public bool IsValid { get; set; } = true;
        /// <summary>
        /// Special case: when IsValid is false, we often want to log that we got invalid data.
        /// But that's not true for the Ruuvi Air
        /// </summary>
        public bool IsIgnored { get; set; } = false; // when IsValid==false, IsIgnored sets whether we shouldn't even log it.

        public virtual HealthDataRecordCopyable Clone()
        {
            return this.MemberwiseClone() as HealthDataRecordCopyable;
        }

        public virtual void CopyFrom(HealthDataRecordCopyable value)
        {
            base.CopyFrom(value);
            // In reality, IsValid is always true here and IsIgnored false.
            IsValid = value.IsValid;
            IsIgnored = value.IsIgnored;
        }

        public virtual HealthDataRecordCopyable CopyToAndUpdateUnits(HealthDataRecordCopyable dest, UserPreferences CurrUserPrefs, string knownDeviceName)
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
            dest.PulseRate = PulseRate;
            dest.OxygenSaturationInPercent = OxygenSaturationInPercent;
            dest.PerfusionIndexInPercent = PerfusionIndexInPercent;
            dest.BatteryInPercent = BatteryInPercent;
            dest.Name = Name;

            return dest;
        }
    }
}
