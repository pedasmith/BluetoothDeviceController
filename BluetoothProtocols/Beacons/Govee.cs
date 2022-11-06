using BluetoothDeviceController;
using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using System;
using System.Collections.Generic;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothDeviceController.BluetoothDefinitionLanguage.AdvertisementDataSectionParser;

namespace BluetoothProtocols.Beacons
{
    public class Govee : SensorDataRecord
    {

        public bool IsValid { get; set; } = true;
        public UInt16 CompanyId { get; set; } // will by 0xEC88 for the Govee HS5074
        public enum SensorType { Other, HS5074 };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }
        public double BatteryInPercent { get; set; }
        public string EncodeMessage { get; set; }

        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns a Govee data record. Return might be null or might be Invalid.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static Govee Parse (BleAdvertisementWrapper wrapper)
        {
            Govee retval = null;
            BluetoothCompanyIdentifier.CommonManufacturerType parseAs = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
            bool isGovee = AdvertIsGovee(wrapper); 
            if (isGovee)
            {
                //Future work: be more generic here. Right now this is super specific.
                parseAs = BluetoothCompanyIdentifier.CommonManufacturerType.Govee;
            }
            if (parseAs == BluetoothCompanyIdentifier.CommonManufacturerType.Govee)
            {
                var ble = wrapper.BleAdvert;
                foreach (var section in ble.Advertisement.DataSections)
                {
                    DataTypeValue dtv = ConvertDataTypeValue(section.DataType); // get the enum value
                    switch (dtv)
                    {
                        case DataTypeValue.ManufacturerData:
                            retval = Parse(section);
                            break;
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Returns true if the local name OR the original name is Govee_H5074
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static bool AdvertIsGovee(BleAdvertisementWrapper wrapper)
        {
            bool retval = false;
            if (wrapper.BleAdvert.Advertisement.LocalName.StartsWith("Govee_H5074")) retval = true;
            if (wrapper.BleOriginalAdvert != null)
            {
                if (wrapper.BleOriginalAdvert.Advertisement.LocalName.StartsWith("Govee_H5074")) retval = true;
            }
            return retval;
        }

        public static Govee Parse(BluetoothLEAdvertisementDataSection section)
        {
            var retval = new Govee();

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 0xEC88 but that's explicitly not enforced here

                // original code (not sure why I'm keeping it around; it's pretty weird)
                //var padding = dr.ReadByte();
                //var encode = dr.ReadUInt32();
                //var temp = ((double)(encode / 1000)) / 10.0; // result is degrees c
                //var hum = ((double)(encode % 1000)) / 10.0; // now it's percent
                if (dr.UnconsumedBufferLength > 16 || retval.CompanyId != 0xEC88)
                {
                    var pre = dr.ReadInt16();
                    var (strName, nameOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength - 4);
                    var (strPost, postOk) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength);
                    retval.EncodeMessage = $"Pre={pre} Str={strName} Post={strPost} ";
                    retval.IsValid = false;
                }
                else
                {
                    var junk = dr.ReadByte();
                    retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity;
                    retval.Temperature = dr.ReadInt16() / 100.0;
                    retval.Humidity = dr.ReadInt16() / 100.0;
                    retval.BatteryInPercent = dr.ReadByte();
                    retval.EncodeMessage = $"Temp={retval.Temperature}℃ ({retval.TemperatureInDegreesF}℉) Hum={retval.Humidity}% Bat={retval.BatteryInPercent}% (junk={junk}) ";
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
            return $"Temperature={Temperature} Humidity={Humidity}% "
                + $"Battery={BatteryInPercent}";
        }

    }
}
