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
    public class SensorPro : CopyableSensorDataRecord // SensorDataRecord is INotifyPropertyChanged. 
    {
        /// <summary>
        /// CompanyId is from the advertisement in the manufacturer-specific section. SensorPro devices
        /// completely mess it up; the CompanyId includes actual data.
        /// 
        /// It's supposed to only be one of the values 
        /// from the Bluetooth SIG. See the BluetoothCompanyIdentifier for details.
        /// https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
        /// </summary>
        public UInt16 CompanyId { get; set; } // will by 0xEC88=60552 for the Govee H5074 H5075
        public enum SensorType { Other, T201, NotThisSensorFamily };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }



        /// <summary>
        /// Message created by the Parse method; it's a handy user-readable string for the temp / humidity
        /// </summary>
        public string EncodeMessage { get; set; }

        public override SensorPro Clone()
        {
            return this.MemberwiseClone() as SensorPro;
        }

        public SensorPro CopyToAndUpdateUnits(SensorPro dest, UserPreferences CurrUserPrefs)
        {
            dest ??= this.Clone();
            base.CopyToAndUpdateUnits(dest, CurrUserPrefs);

            dest.IsValid = IsValid;
            dest.CompanyId = CompanyId;
            dest.TagType = TagType;
            return dest;
        }

        public void CopyFrom(SensorPro value)
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
                if (name.StartsWith("T201")) retval = SensorType.T201;
            }
            return retval;
        }

        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns a Govee data record. Return might be null or might be Invalid.
        /// The source will be overwritten! Null is never returned!
        /// </summary>
        public static SensorPro Parse(SensorType sensorType, WatcherData wrapper, SensorPro source)
        {
            var retval = source ?? new SensorPro();
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
        /// Will parse a SensorPro sensor. Must be given the sensor type which is from AdvertIsThermPro (parsing depends on 
        /// the type)
        /// </summary>
        /// <param name="sensorType"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public static SensorPro Parse(SensorType sensorType, BluetoothLEAdvertisementDataSection section, SensorPro source)
        {
            var retval = source ?? new SensorPro();
            retval.TagType = sensorType;
            retval.IsValid = false; // will be set true if the data is valid.

            if (sensorType == SensorType.NotThisSensorFamily) return retval;

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                if (dr.UnconsumedBufferLength > 19)
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
                    case SensorType.T201: // Example: 55 AA 01 01 A4 C1 38 E5 86 C9 01 07 08 1F 12 71 64 00 01  #18.9C 46%
                        {
                            //
                            // -- -- -- -- 05 -- -- -- -- 10 -- -- -- -- 15 -- -- -- -- 20
                            // -- -- -- -- -- -- -- -- -- -- -- le Th Tl Hh Hl Ba -- -- 
                            // 55 AA 01 01 A4 C1 38 E5 86 C9 01 07 07 56 15 23 64 00 01
                            // 55 AA 01 01 A4 C1 38 E5 86 C9 01 07 07 F9 12 E9 64 00 01
                            // 55 AA 01 01 A4 C1 38 E5 86 C9 01 07 08 1F 12 71 64 00 01  #18.9C 46%
                            //
                            // 081F is 2079 (big endian)
                            // 1271 is 4721 (also big endian)

                            dr.ByteOrder = ByteOrder.BigEndian; // Yeah, not ideal
                            var start = dr.ReadInt16(); // 55AA
                            byte[] junk = new byte[9];
                            dr.ReadBytes(junk);
                            var len = dr.ReadByte();

                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.Battery;
                            retval.Temperature = dr.ReadInt16() / 100.0;
                            retval.Humidity = dr.ReadInt16() / 100.0;
                            retval.BatteryInPercent = dr.ReadByte();
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