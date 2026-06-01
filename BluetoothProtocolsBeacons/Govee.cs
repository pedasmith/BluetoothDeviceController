using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3;
using System;
using System.Collections.ObjectModel;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothProtocols.AdvertisementDataSectionParser;
using static BluetoothProtocols.Nordic_Thingy;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    public class Govee : SensorDataRecord // SensorDataRecord is INotifyPropertyChanged. 
    {

        public bool IsValid { get; set; } = true;
        /// <summary>
        /// CompanyId is from the advertisement in the manufacturer-specific section. It's supposed to only be one of the values 
        /// from the Bluetooth SIG. See the BluetoothCompanyIdentifier for details.
        /// https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
        /// </summary>
        public UInt16 CompanyId { get; set; } // will by 0xEC88=60552 for the Govee H5074 H5075
        public enum SensorType { Other, H5074, H5075, H5106, NotGovee };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }

        private double _BatteryInPercent;
        /// <summary>
        /// BatteryInPercent isn't available for all devices.
        /// </summary>
        public double BatteryInPercent { get { return _BatteryInPercent; } set { if (value == _BatteryInPercent) return; _BatteryInPercent = value; OnPropertyChanged(); } }

        /// <summary>
        /// Message created by the Parse method; it's a handy user-readable string for the temp / humidity
        /// </summary>
        public string EncodeMessage { get; set; }

        public Govee Clone()
        {
            return this.MemberwiseClone() as Govee;
        }

        public static Govee CopyAndUpdateUnits(Govee source, Govee dest, UserPreferences CurrUserPrefs)
        {
            dest ??= source.Clone();
            dest.EventTime = source.EventTime;
            dest.Temperature = BluetoothWatcher.Units.Temperature.Convert(source.Temperature, BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius, CurrUserPrefs.Temperature);
            dest.Pressure = BluetoothWatcher.Units.Pressure.Convert(source.Pressure, BluetoothWatcher.Units.Pressure.PressureUnit.hectoPascal_milliBar, CurrUserPrefs.Pressure);
            dest.Humidity = source.Humidity; // Humidity is always in percent, so no conversion needed.
            dest.PM25 = source.PM25;

            dest.IsValid = source.IsValid;
            dest.CompanyId= source.CompanyId;
            dest.TagType = source.TagType;
            dest.BatteryInPercent = source.BatteryInPercent;
            return dest;
        }

        public void CopyFrom(Govee value)
        {
            EventTime = value.EventTime;
            Temperature = value.Temperature;
            Pressure = value.Pressure;
            Humidity = value.Humidity; // Humidity is always in percent, so no conversion needed.
            PM25 = value.PM25;

            IsValid = value.IsValid;
            CompanyId = value.CompanyId;
            TagType = value.TagType;
            BatteryInPercent = value.BatteryInPercent;
        }


        /// <summary>
        /// Returns true if the local name OR the original name matches Govee_H5074_ or GVH5075_
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static SensorType AdvertIsGovee(WatcherData wrapper)
        {
            var retval = NameToSensorType(wrapper.CompleteLocalName);
            if (retval == SensorType.NotGovee && wrapper.OriginalArgs != null)
            {
                retval = NameToSensorType(wrapper.OriginalArgs.Advertisement.LocalName);
            }
            return retval;
        }

        private static SensorType NameToSensorType(string name)
        {
            SensorType retval = SensorType.NotGovee;
            if (name != null)
            {
                if (name.StartsWith("Govee_H5074_")) retval = SensorType.H5074;
                if (name.StartsWith("GVH5075_")) retval = SensorType.H5075;
                if (name.StartsWith("GVH5106_")) retval = SensorType.H5106;
            }
            return retval;
        }
        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns a Govee data record. Return might be null or might be Invalid.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static Govee Parse(SensorType sensorType, WatcherData wrapper, Govee source)
        {
            if (sensorType == SensorType.NotGovee) return null;

            Govee retval = null;
            var ble = wrapper.OriginalArgs;
            foreach (var section in ble.Advertisement.DataSections)
            {
                DataTypeValue dtv = ConvertDataTypeValue(section.DataType); // get the enum value
                switch (dtv)
                {
                    case DataTypeValue.ManufacturerData:
                        retval = Parse(sensorType, section, source);
                        break;
                }
            }

            return retval;
        }

        /// <summary>
        /// Will parse a Govee sensor. Must be given the sensor type which is from AdvertIsGovee (parsing depends on 
        /// the type)
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static Govee Parse(SensorType sensorType, BluetoothLEAdvertisementDataSection section, Govee source)
        {
            if (sensorType == SensorType.NotGovee) return null;
            var retval = source ?? new Govee();
            retval.TagType = sensorType;

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 0xEC88=60552 but that's explicitly not enforced here
                var expectedCompanyId = 0xEC88;
                if (sensorType == SensorType.H5106) expectedCompanyId = 0x01; // Nokia??
                if (dr.UnconsumedBufferLength > 16 || retval.CompanyId != expectedCompanyId)
                {
                    var pre = dr.ReadInt16();
                    var (strName, nameOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength - 4);
                    var (strPost, postOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength);
                    retval.EncodeMessage = $"Pre={pre} Str={strName} Post={strPost} ";
                    retval.IsValid = false;
                }
                else
                {
                    if (sensorType == SensorType.Other)
                    {
                        switch (dr.UnconsumedBufferLength)
                        {
                            case 9: sensorType = SensorType.H5075; break;
                            case 10:
                                {
                                    var drtype = DataReader.FromBuffer(section.Data);
                                    drtype.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                                    var typecompany = dr.ReadInt16(); // TODO: why isn't this reading from drtype?
                                    var b0 = dr.ReadByte();
                                    var b1 = dr.ReadByte();
                                    if (b0 == 0x01 && b1 == 0x01)
                                    {
                                        sensorType = SensorType.H5106;
                                    }
                                    else
                                    {
                                        // Check to make sure that b0==0?
                                        sensorType = SensorType.H5074; break; // or H5106 :-(
                                    }
                                }
                                break;
                        }
                    }
                    // At this point, we've read in two bytes from dr.
                    switch (sensorType)
                    {
                        case SensorType.H5074:
                            var junk = dr.ReadByte();
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity;
                            retval.Temperature = dr.ReadInt16() / 100.0;
                            retval.Humidity = dr.ReadInt16() / 100.0;
                            retval.BatteryInPercent = dr.ReadByte();
                            retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}% (junk={junk}) ";
                            break;
                        case SensorType.H5075:
                            var junk2 = dr.ReadByte();
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity;
                            // Yes, this encoding is horrible for no good reason.
                            var b1 = dr.ReadByte();
                            var b2 = dr.ReadByte();
                            var b3 = dr.ReadByte();
                            var isneg = (b1 & 0x80) != 0;
                            var value = ((b1 & 0x7F) << 16) + (b2 << 8) + b3;
                            retval.Temperature = ((double)(value / 1000)) / 10.0;
                            retval.Humidity = ((double)(value % 1000)) / 10.0;
                            retval.BatteryInPercent = dr.ReadByte();
                            retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}% (junk={junk2}) ";
                            break;
                        case SensorType.H5106:
                            var junk3 = dr.ReadInt16();
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.PM25;
                            dr.ByteOrder = ByteOrder.BigEndian; // Surprise! It's big endian!
                            var value3 = dr.ReadUInt32();
                            retval.Temperature = ((double)(value3 / 1_000_000)) / 10.0;
                            retval.Humidity = ((double)((value3 / 1_000) % 1000)) / 10.0;
                            retval.PM25 = (double)(value3 % 1_000);
                            retval.BatteryInPercent = 100.0; // it's line powered.
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                retval.EncodeMessage = $"Unable to parse Govee: {ex.Message}";
                retval.IsValid = false;
            }
            return retval; // if there was an exception, the beacon is invalid...
        }
        public override string ToString()
        {
            return $"Temperature={Temperature} Humidity={Humidity}% PM2.5={PM25} "
                + $"Battery={BatteryInPercent}";
        }
    }



    ///<summary>
    ///TODO:
    ///Environment_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Environment_Data group from Environment.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class GoveeCollection
    {
        public enum Verb { Add, ReplaceMostRecent };

        public int Count { get { return Timestamps.Count; } }

        public void Update(Govee value, Verb verb)
        {
            switch (verb)
            {
                case Verb.Add: Add(value); break;
                case Verb.ReplaceMostRecent: ReplaceMostRecent(value); break;
            }
        }

        public void Add(Govee value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add(value.Clone());
            Timestamps.Add(value.TimestampMostRecent);
            TimestampsDT.Add(value.TimestampMostRecent.DateTime);
            Temperature.Add(value.Temperature);
            Pressure.Add(value.Pressure);
            Humidity.Add(value.Humidity);
            PM25.Add(value.PM25);
        }
        public void ReplaceMostRecent(Govee value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index].CopyFrom(value);  // was value.Clone(); switching to reduce flickering.
            Temperature[index] = value.Temperature;
            Pressure[index] = value.Pressure;
            Humidity[index] = value.Humidity;
            PM25[index] = value.PM25;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Temperature (c)
        public ObservableCollection<double> Temperature { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Pressure (hpa)
        public ObservableCollection<double> Pressure { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Humidity (%)
        public ObservableCollection<double> Humidity { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Air Quality eCOS TVOC
        public ObservableCollection<double> PM25 { get; } = new ObservableCollection<double>();
        public ObservableCollection<Govee> Data { get; } = new ObservableCollection<Govee>();
    }
}
