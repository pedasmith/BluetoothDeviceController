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
    /// <summary>
    /// Based on the older UWP Ruuvi_Tag (which is uses for parsing)
    /// </summary>

    public class Ruuvi_TagCSDR : CopyableSensorDataRecord // SensorDataRecord // SensorDataRecord is INotifyPropertyChanged. 
    {
        /// <summary>
        /// CompanyId is from the advertisement in the manufacturer-specific section. It's supposed to only be one of the values 
        /// from the Bluetooth SIG. See the BluetoothCompanyIdentifier for details.
        /// https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/company_identifiers/company_identifiers.yaml
        /// </summary>
        public UInt16 CompanyId { get; set; } // will by 0x0499==1177 for standard Ruuvi tags
        public enum SensorType { Other, Air, NotThisSensorFamily };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }



        /// <summary>
        /// Message created by the Parse method; it's a handy user-readable string for the temp / humidity
        /// </summary>
        public string EncodeMessage { get; set; }

        public override Ruuvi_TagCSDR Clone()
        {
            return this.MemberwiseClone() as Ruuvi_TagCSDR;
        }

        public Ruuvi_TagCSDR CopyAndUpdateUnits(Ruuvi_TagCSDR source, Ruuvi_TagCSDR dest, UserPreferences CurrUserPrefs, string knownDeviceName)
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

        public void CopyFrom(Ruuvi_TagCSDR value)
        {
            base.CopyFrom(value);

            IsValid = value.IsValid;
            CompanyId = value.CompanyId;
            TagType = value.TagType;
        }


        /// <summary>
        /// Returns true if the local name OR the original name matches Ruuvi_* TODO: correct name
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
                if (name.StartsWith("Ruuvi")) retval = SensorType.Air; // TODO: correct name
            }
            return retval;
        }

        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns a Ruuvi_TagCSDR data record. Return might be null or might be Invalid.
        /// The source will be overwritten! Null is never returned!
        /// </summary>
        public static Ruuvi_TagCSDR Parse(SensorType sensorType, WatcherData wrapper, Ruuvi_TagCSDR source)
        {
            var retval = source ?? new Ruuvi_TagCSDR();
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
        /// Will parse a Ruuvi sensor. Must be given the sensor type which is from AdvertIsRuuvi (parsing depends on 
        /// the type)
        /// </summary>
        public static Ruuvi_TagCSDR Parse(SensorType sensorType, BluetoothLEAdvertisementDataSection section, Ruuvi_TagCSDR source)
        {
            var retval = source ?? new Ruuvi_TagCSDR();
            retval.TagType = sensorType;
            retval.IsValid = false; // will be set true if the data is valid.

            if (sensorType == SensorType.NotThisSensorFamily) return retval;

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 0x0499 but that's explicitly not enforced here
                var expectedCompanyId = 0x0499;

                if (retval.CompanyId != expectedCompanyId)
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
                        var rt = Ruuvi_Tag.Parse(section, 0); // No RSSI (and it's not used by Parse anyway?)
                        rt.ToSensorDataRecord(retval);
                        retval.IsValid = rt.IsValid;
                        break;
                }
            }
            catch (Exception ex)
            {
                retval.EncodeMessage = $"Unable to parse Ruuvi: {ex.Message}";
                retval.IsValid = false;
            }
            if (retval.Luminosity== 0)
            {
                ; // handy place for a debugger
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
