using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3;
using System;
using System.Collections.ObjectModel;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothProtocols.AdvertisementDataSectionParser;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    public class ThermPro : CopyableSensorDataRecord // SensorDataRecord is INotifyPropertyChanged. 
    {
        /// <summary>
        /// CompanyId is from the advertisement in the manufacturer-specific section. ThermPro devices
        /// completely mess it up; the CompanyId includes actual data.
        /// 
        /// It's supposed to only be one of the values 
        /// from the Bluetooth SIG. See the BluetoothCompanyIdentifier for details.
        /// https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
        /// </summary>
        public UInt16 CompanyId { get; set; } 
        public enum SensorType { Other, TP351, TP357, TP359, NotThisSensorFamily };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }



        /// <summary>
        /// Message created by the Parse method; it's a handy user-readable string for the temp / humidity
        /// </summary>
        public string EncodeMessage { get; set; }

        public override ThermPro Clone()
        {
            return this.MemberwiseClone() as ThermPro;
        }

        public ThermPro CopyToAndUpdateUnits(ThermPro dest, UserPreferences CurrUserPrefs)
        {
            dest ??= this.Clone();
            base.CopyToAndUpdateUnits(dest, CurrUserPrefs);

            dest.IsValid = IsValid;
            dest.CompanyId = CompanyId;
            dest.TagType = TagType;
            return dest;
        }

        public void CopyFrom(ThermPro value)
        {
            base.CopyFrom(value);

            IsValid = value.IsValid;
            CompanyId = value.CompanyId;
            TagType = value.TagType;
        }



        /// <summary>
        /// Returns true if the local name OR the original name matches Govee_H5074_ or GVH5075_
        /// All of these parsers happen to have an AdvertIsSensorFamily :-)
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static SensorType AdvertIsSensorFamily(WatcherData wrapper)
        {
            var retval = NameToSensorType(wrapper.BestName);
            if (retval == SensorType.NotThisSensorFamily && wrapper.OriginalAdvertisement != null)
            {
                retval = NameToSensorType(wrapper.OriginalAdvertisement.Advertisement.LocalName);
            }
            return retval;
        }

        private static SensorType NameToSensorType(string name)
        {
            SensorType retval = SensorType.NotThisSensorFamily;
            if (name != null)
            {
                if (name.StartsWith("TP351")) retval = SensorType.TP351;
                if (name.StartsWith("TP357")) retval = SensorType.TP357;
                if (name.StartsWith("TP359")) retval = SensorType.TP357;
            }
            return retval;
        }

        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns a Govee data record. Return might be null or might be Invalid.
        /// The source will be overwritten! Null is never returned!
        /// </summary>
        public static ThermPro Parse(SensorType sensorType, WatcherData wrapper, ThermPro source)
        {
            var retval = source ?? new ThermPro();
            if (sensorType == SensorType.NotThisSensorFamily)
            {
                retval.IsValid = false;
                return retval;
            }
            foreach (var advert in wrapper.Advertisements)
            {
                if (advert == null) continue;
                foreach (var section in advert.Advertisement.DataSections)
                {
                    DataTypeValue dtv = ConvertDataTypeValue(section.DataType); // get the enum value
                    switch (dtv)
                    {
                        case DataTypeValue.ManufacturerData:
                            retval = Parse(sensorType, section, source);
                            break;
                    }
                }
            }

            return retval;
        }


        /// <summary>
        /// Will parse a ThermPro sensor. Must be given the sensor type which is from AdvertIsThermPro (parsing depends on 
        /// the type)
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static ThermPro Parse(SensorType sensorType, BluetoothLEAdvertisementDataSection section, ThermPro source)
        {
            var retval = source ?? new ThermPro();
            retval.TagType = sensorType;
            retval.IsValid = false; // will be set true if the data is valid.

            if (sensorType == SensorType.NotThisSensorFamily) return retval;

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                if (dr.UnconsumedBufferLength > 10)
                {
                    var pre = dr.ReadInt16();
                    var (strName, nameOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength - 4);
                    var (strPost, postOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength);
                    retval.EncodeMessage = $"Pre={pre} Str={strName} Post={strPost} ";
                    retval.IsValid = false;
                    return retval;
                }

                // At this point, we've read in two bytes from dr (the company ID)
                switch (sensorType)
                {
                    default:
                        retval.IsValid = false;
                        break;
                    case SensorType.TP351: // Example: C2 DA 00 2B 22 33 01 # 21.6C 45%
                    case SensorType.TP357: // Example: C2 C0 00 32 02 2C
                    case SensorType.TP359: // Example: C2 EA 00 29 22 13 01 # 24.2C 39%
                        {
                            // -- TL TH HU
                            // C2 C0 00 32 02 2C
                            //

                            var junk = dr.ReadByte();
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity;
                            retval.Temperature = dr.ReadInt16() / 10.0;
                            retval.Humidity = dr.ReadByte();
                            //retval.BatteryInPercent = dr.ReadByte();
                            retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% ";
                            retval.IsValid = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                retval.EncodeMessage = $"Unable to parse ThermPro: {ex.Message}";
                retval.IsValid = false;
            }
            return retval; // if there was an exception, the beacon is invalid...
        }
        public override string ToString()
        {
            return $"Temperature={Temperature} Humidity={Humidity}% ";
        }
    }

}