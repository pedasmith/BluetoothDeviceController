using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3;
using System;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothProtocols.AdvertisementDataSectionParser;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    public class Aranet : SensorDataRecordCopyable // SensorDataRecord // SensorDataRecord is INotifyPropertyChanged. 
    {
        /// <summary>
        /// CompanyId is from the advertisement in the manufacturer-specific section. It's supposed to only be one of the values 
        /// from the Bluetooth SIG. See the BluetoothCompanyIdentifier for details.
        /// https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
        /// </summary>
        public UInt16 CompanyId { get; set; } // will by 0x0702 for Aranet
        public enum SensorType { NotThisSensorFamily, Other, Aranet4 };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }



        /// <summary>
        /// Message created by the Parse method; it's a handy user-readable string for the temp / humidity
        /// </summary>
        public string EncodeMessage { get; set; }

        public override Aranet Clone()
        {
            return this.MemberwiseClone() as Aranet;
        }

        public Aranet CopyAndUpdateUnits(Aranet source, Aranet dest, UserPreferences CurrUserPrefs, string knownDeviceName)
        {
            if (dest == null)
            {
                dest = source.Clone();
                dest.Name = knownDeviceName;
                // the protocol Name is the "SupportedDevice" name. It's not unique to each one.
                // What we need for our data is the name that the user might have given the 
                // device (the "known device" name). It's set in the UpdateUX from SaveData
            }
            base.CopyToAndUpdateUnits(dest, CurrUserPrefs, knownDeviceName);

            dest.IsValid = IsValid;
            dest.CompanyId = CompanyId;
            dest.TagType = TagType;
            return dest;
        }

        public void CopyFrom(Aranet value)
        {
            base.CopyFrom(value);

            IsValid = value.IsValid;
            CompanyId = value.CompanyId;
            TagType = value.TagType;
        }


        /// <summary>
        /// Returns true if the local name OR the original name matches Aranet4
        /// All of these parsers happen to have an AdvertIsSensorFamily :-)
        /// </summary>
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
                if (name.StartsWith("Aranet4 ")) retval = SensorType.Aranet4;
            }
            return retval;
        }

        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns an Aranet data record. Return might be null or might be Invalid.
        /// The source will be overwritten! Null is never returned!
        /// </summary>
        public static Aranet Parse(SensorType sensorType, WatcherData wrapper, Aranet source)
        {
            var retval = source ?? new Aranet();
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
                            retval = Parse(sensorType, section, retval);
                            break;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Will parse a Aranet sensor. Must be given the sensor type which is from AdvertIsAranet (parsing depends on 
        /// the type)
        /// </summary>
        public static Aranet Parse(SensorType sensorType, BluetoothLEAdvertisementDataSection section, Aranet source)
        {
            var retval = source ?? new Aranet();
            retval.TagType = sensorType;
            retval.IsValid = false; // will be set true if the data is valid.

            if (sensorType == SensorType.NotThisSensorFamily) return retval;

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 0x0702 but that's explicitly not enforced here
                var expectedCompanyId = 0x0702;
                if (retval.CompanyId != expectedCompanyId)
                {
                    // Somewhat random check that the Govee sensor family has to do.
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
                    case SensorType.Aranet4: 
                        {
                            // Official decode at https://github.com/SAF-Tehnika-Developer/com.aranet4/blob/master/app.js
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity | SensorPresent.Battery | SensorPresent.CO2 | SensorPresent.Pressure;
                            var unknown1 = dr.ReadInt16();
                            var unknown2 = dr.ReadInt16();
                            var unknown3 = dr.ReadInt16();
                            var unknown4 = dr.ReadInt16();
                            retval.CO2 = dr.ReadInt16();
                            retval.Temperature = dr.ReadInt16() / 20.0;
                            retval.Pressure = dr.ReadInt16() / 10.0;
                            retval.Humidity = dr.ReadByte();
                            retval.BatteryInPercent = dr.ReadByte();
                            retval.EncodeMessage = $"CO₂={retval.CO2} Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}%) ";
                            retval.IsValid = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                retval.EncodeMessage = $"Unable to parse Aranet: {ex.Message}";
                retval.IsValid = false;
            }
            return retval; // if there was an exception, the beacon is invalid...
        }
        public override string ToString()
        {
            return $"Temperature={Temperature} Humidity={Humidity}% CO₂={PM25} "
                + $"Battery={BatteryInPercent}";
        }
    }
}
