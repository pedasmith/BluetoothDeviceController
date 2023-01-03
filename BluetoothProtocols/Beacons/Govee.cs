using BluetoothDeviceController;
using System;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothDeviceController.BluetoothDefinitionLanguage.AdvertisementDataSectionParser;

namespace BluetoothProtocols.Beacons
{
    public class Govee : SensorDataRecord
    {

        public bool IsValid { get; set; } = true;
        public UInt16 CompanyId { get; set; } // will by 0xEC88 for the Govee H5074 H5075
        public enum SensorType { Other, H5074, H5075, H5106, NotGovee };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }
        public double BatteryInPercent { get; set; }
        public string EncodeMessage { get; set; }



        /// <summary>
        /// Returns true if the local name OR the original name matches Govee_H5074_ or GVH5075_
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static SensorType AdvertIsGovee(BleAdvertisementWrapper wrapper)
        {
            var retval = NameToSensorType(wrapper.BleAdvert.Advertisement.LocalName);
            if (retval == SensorType.NotGovee && wrapper.BleOriginalAdvert != null)
            {
                retval = NameToSensorType(wrapper.BleOriginalAdvert.Advertisement.LocalName);
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
        public static Govee Parse(SensorType sensorType, BleAdvertisementWrapper wrapper)
        {
            if (sensorType == SensorType.NotGovee) return null;

            Govee retval = null;
            var ble = wrapper.BleAdvert;
            foreach (var section in ble.Advertisement.DataSections)
            {
                DataTypeValue dtv = ConvertDataTypeValue(section.DataType); // get the enum value
                switch (dtv)
                {
                    case DataTypeValue.ManufacturerData:
                        retval = Parse(sensorType, section);
                        break;
                }
            }

            return retval;
        }
        public static Govee Parse(SensorType sensorType, BluetoothLEAdvertisementDataSection section)
        {
            if (sensorType == SensorType.NotGovee) return null;
            var retval = new Govee();

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian; // BT is generally little endian.
                retval.CompanyId = dr.ReadUInt16(); // Will be 0xEC88 but that's explicitly not enforced here
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
                    }

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
}
