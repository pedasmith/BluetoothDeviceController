using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothProtocols.AdvertisementDataSectionParser;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{

    public class Govee : CopyableSensorDataRecord // SensorDataRecord // SensorDataRecord is INotifyPropertyChanged. 
    {
        /// <summary>
        /// CompanyId is from the advertisement in the manufacturer-specific section. It's supposed to only be one of the values 
        /// from the Bluetooth SIG. See the BluetoothCompanyIdentifier for details.
        /// https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
        /// </summary>
        public UInt16 CompanyId { get; set; } // will by 0xEC88=60552 for the Govee H5074 H5075
        public enum SensorType { Other, H5074, H5075, H5103, H5106, H5171, H5179, NotThisSensorFamily };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }



        /// <summary>
        /// Message created by the Parse method; it's a handy user-readable string for the temp / humidity
        /// </summary>
        public string EncodeMessage { get; set; }

        public override Govee Clone()
        {
            return this.MemberwiseClone() as Govee;
        }

        public Govee CopyAndUpdateUnits(Govee source, Govee dest, UserPreferences CurrUserPrefs)
        {
            dest ??= source.Clone();
            base.CopyToAndUpdateUnits(dest, CurrUserPrefs);

            dest.IsValid = IsValid;
            dest.CompanyId= CompanyId;
            dest.TagType = TagType;
            return dest;
        }

        public void CopyFrom(Govee value)
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
                if (name.StartsWith("Govee_H5074_")) retval = SensorType.H5074;
                if (name.StartsWith("GVH5075_")) retval = SensorType.H5075;
                if (name.StartsWith("GVH5103_")) retval = SensorType.H5103;
                if (name.StartsWith("GVH5106_")) retval = SensorType.H5106;
                if (name.StartsWith("V5171")) retval = SensorType.H5171;
                if (name.StartsWith("GV5171")) retval = SensorType.H5171;
                if (name.StartsWith("GV5179_")) retval = SensorType.H5179;
            }
            return retval;
        }

        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns a Govee data record. Return might be null or might be Invalid.
        /// The source will be overwritten! Null is never returned!
        /// </summary>
        public static Govee Parse(SensorType sensorType, WatcherData wrapper, Govee source)
        {
            var retval = source ?? new Govee();
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

        private static SensorType SpecializedSensorTypeFromData(BluetoothLEAdvertisementDataSection section)
        {
            SensorType sensorType = SensorType.Other;
            var dr = DataReader.FromBuffer(section.Data);
            switch (dr.UnconsumedBufferLength)
            {
                case 9: sensorType = SensorType.H5075; break;
                case 10:
                    {
                        dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                        var typecompany = dr.ReadInt16(); 
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
            return sensorType;
        }

        /// <summary>
        /// Converts the remarkably idiotic three-byte combined temp/humidity values
        /// </summary>
        public static (double temperature, double humidity) ConvertThreeBytes(byte b1, byte b2, byte b3)
        {
            double temperature = double.NaN;
            double humidity = double.NaN;

            var isneg = (b1 & 0x80) != 0;
            var value = ((b1 & 0x7F) << 16) + (b2 << 8) + b3;

            var bottomRaw = (value % 1000);
            temperature = ((double)((value - bottomRaw) / 1000)) / 10.0;
            if (isneg) temperature = -temperature;
            humidity = ((double)bottomRaw) / 10.0;

            return (temperature, humidity);
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
            var retval = source ?? new Govee();
            retval.TagType = sensorType;
            retval.IsValid = false; // will be set true if the data is valid.

            if (sensorType == SensorType.NotThisSensorFamily) return retval;

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 0xEC88=60552 but that's explicitly not enforced here
                var expectedCompanyId = 0xEC88;
                switch (sensorType)
                {
                    case SensorType.H5103:
                    case SensorType.H5106:
                    case SensorType.H5171:
                    case SensorType.H5179:
                        expectedCompanyId = 0x01;
                        break;
                }
                if (dr.UnconsumedBufferLength > 16 || retval.CompanyId != expectedCompanyId)
                {
                    var pre = dr.ReadInt16();
                    var (strName, nameOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength - 4);
                    var (strPost, postOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength);
                    retval.EncodeMessage = $"Pre={pre} Str={strName} Post={strPost} ";
                    retval.IsValid = false;
                    return retval;
                }
                if (sensorType == SensorType.Other)
                {
                    sensorType = SpecializedSensorTypeFromData(section);
                }

                // At this point, we've read in two bytes from dr (the company ID)
                switch (sensorType)
                {
                    default:
                        retval.IsValid = false;
                        break;
                    case SensorType.H5074: // Example: 88 EC 00 F1 07 A2 13 64 02
                        {
                            // 88 EC -- TL TH HL HH BB --
                            // 88 EC 00 F1 07 A2 13 64 02
                            var junk = dr.ReadByte();
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.Battery;
                            retval.Temperature = dr.ReadInt16() / 100.0;
                            retval.Humidity = dr.ReadInt16() / 100.0;
                            retval.BatteryInPercent = dr.ReadByte();
                            retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}% (junk={junk}) ";
                            retval.IsValid = true;
                        }
                        break;
                    case SensorType.H5075: // Example: 88 EC 00 02 EF E0 0B 00
                        {
                            // 88 EC -- b1 b2 b3 BB
                            // 88 EC 00 02 EF E0 0B 00
                            //

                            var junk2 = dr.ReadByte();
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.Battery;
                            // Yes, this encoding is horrible for no good reason.
                            var b1 = dr.ReadByte();
                            var b2 = dr.ReadByte();
                            var b3 = dr.ReadByte();
                            var (temperature, humidity) = ConvertThreeBytes(b1, b2, b3);
                            retval.Temperature = temperature;
                            retval.Humidity = humidity;
                            //var isneg = (b1 & 0x80) != 0;
                            //var value = ((b1 & 0x7F) << 16) + (b2 << 8) + b3;
                            //retval.Temperature = ((double)(value / 1000)) / 10.0;
                            //retval.Humidity = ((double)(value % 1000)) / 10.0;
                            retval.BatteryInPercent = dr.ReadByte();
                            retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}% (junk={junk2}) ";
                            retval.IsValid = true;
                        }
                        break;
                    case SensorType.H5106: // Example: 01 00 01 01 0B 3C D0 D9
                                           //
                                           // 01 00 q1 q2 q3 q4
                                           // 01 00 01 01 0B 3C D0 D9

                        {
                            var junk3 = dr.ReadInt16();
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.PM25;
                            dr.ByteOrder = ByteOrder.BigEndian; // Surprise! It's big endian!
                            var value3 = dr.ReadUInt32();
                            retval.Temperature = ((double)(value3 / 1_000_000)) / 10.0;
                            retval.Humidity = ((double)((value3 / 1_000) % 1000)) / 10.0;
                            retval.PM25 = (double)(value3 % 1_000);
                            retval.BatteryInPercent = 100.0; // it's line powered.
                            retval.IsValid = true;
                        }
                        break;
                    case SensorType.H5171:
                        //          01 00 -- -- b1 b2 b3 BB
                        // Example: 01 00 01 01 03 35 D7 64 00 00 is 21C 39%
                        // Example: 01 00 01 01 01 78 C1 64 00 00 is 9C 44%
                        // The first two  bytes have already been read in.
                        // Example: 01 00 [company] 01 01 01 78 C1 64 00 00 is 9C 44%
                        {
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.Battery;
                            var junk1 = dr.ReadByte();
                            var junk2 = dr.ReadByte();
                            var b1 = dr.ReadByte();
                            var b2 = dr.ReadByte();
                            var b3 = dr.ReadByte();
                            var (temperature, humidity) = ConvertThreeBytes(b1, b2, b3);
                            retval.Temperature = temperature;
                            retval.Humidity = humidity;
                            retval.BatteryInPercent = dr.ReadByte(); // 2026-06-04 confirmed this is battery
                            retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}%  ";
                            retval.IsValid = true;
                        }
                        break;
                    case SensorType.H5103: // Example: 01 00 01 01 03 51 41 2C
                    case SensorType.H5179: // Example: 01 00 01 01 02 C8 B1 64
                                           // Example: 01 00 01 01 02 C8 B1 64 is 
                                           // The first two  bytes have already been read in.
                                           // Example: 01 00 [company] 01 01 01 78 C1 64 is 9C 44%
                                           // Is just like the H5171 but without the last 2 bytes
                                           // 
                                           // 01 00 -- -- b1 b2 b3 BB
                                           // 01 00 01 01 03 51 41 2C
                        {
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.Battery;
                            var junk1 = dr.ReadByte();
                            var junk2 = dr.ReadByte();
                            var b1 = dr.ReadByte();
                            var b2 = dr.ReadByte();
                            var b3 = dr.ReadByte();
                            var (temperature, humidity) = ConvertThreeBytes(b1, b2, b3);
                            retval.Temperature = temperature;
                            retval.Humidity = humidity;
                            retval.BatteryInPercent = dr.ReadByte();
                            retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}%  ";
                            retval.IsValid = true;
                        }
                        break;
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



}
