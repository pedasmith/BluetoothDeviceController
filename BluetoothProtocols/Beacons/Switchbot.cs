using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothDeviceController;
using System;
using System.Collections.Generic;
using System.Text;
using static BluetoothDeviceController.BluetoothDefinitionLanguage.AdvertisementDataSectionParser;
using static BluetoothProtocols.SensorDataRecord;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using BluetoothDeviceController.BleEditor;

namespace BluetoothProtocols.Beacons
{
    public class SwitchBot : SensorDataRecord
    {
        public bool IsValid { get; set; } = true;
        public UInt16 ServiceId { get; set; } // will bb 0x0xFD3D for the SwitchBot MeterTH S1 FCC 2AKXB-METERTH
        public enum SensorType { Other, MeterTH };
        public SensorType TagType { get; set; } = SensorType.Other;
        public double TemperatureInDegreesF { get { return (Temperature * 9.0 / 5.0) + 32.0; } }
        public double BatteryInPercent { get; set; }
        public string EncodeMessage { get; set; }

        /// <summary>
        /// Returns true if the advert matches a SwitchBot advert.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static bool AdvertIsSwitchBot(BleAdvertisementWrapper wrapper)
        {
            bool retval = false;
            var ble = wrapper.BleAdvert;
            if (!ble.IsScanResponse)
            {
                // the parent advert.
                foreach (var section in ble.Advertisement.DataSections)
                {
                    DataTypeValue dtv = ConvertDataTypeValue(section.DataType); // get the enum value
                    switch (dtv)
                    {
                        case DataTypeValue.ManufacturerData:
                            var dr = DataReader.FromBuffer(section.Data);
                            dr.ByteOrder = ByteOrder.LittleEndian;
                            if (dr.UnconsumedBufferLength >= 2)
                            {
                                var addr = dr.ReadUInt16();
                                switch (addr)
                                {
                                    case 0x0969: // SwitchBot
                                        // case 2409: /* hex=0x0969 */ return "Woan Technology (Shenzhen) Co., Ltd.";
                                        retval = true; // NOTE: this isn't a very precise way to tell
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            else if (ble.IsScanResponse)
            {
                // The original should be a SwitchBot, too
                if (wrapper.BleOriginalAdvert != null && wrapper.BleOriginalAdvert.Advertisement != null)
                {
                    retval = AdvertIsSwitchBot(new BleAdvertisementWrapper (wrapper.BleOriginalAdvert));
                    if (!retval)
                    {
                        return retval;
                    }
                }
                foreach (var section in ble.Advertisement.DataSections)
                {
                    DataTypeValue dtv = ConvertDataTypeValue(section.DataType); // get the enum value
                    switch (dtv)
                    {
                        case DataTypeValue.ServiceData:
                            var dr = DataReader.FromBuffer(section.Data);
                            dr.ByteOrder = ByteOrder.LittleEndian;
                            if (dr.UnconsumedBufferLength >= 2)
                            {
                                var addr = dr.ReadUInt16();
                                switch (addr)
                                {
                                    case 0xFD3D: // SwitchBot
                                        retval = true; // NOTE: this isn't a very precise way to tell
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// Parses a BleAdvertisementWrapper and returns a SwitchBot data record. Return might be null or might be Invalid.
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        public static SwitchBot Parse(BleAdvertisementWrapper wrapper)
        {
            SwitchBot retval = null;
            BluetoothCompanyIdentifier.CommonManufacturerType parseAs = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
            bool isSwitchBot = AdvertIsSwitchBot(wrapper);
            if (isSwitchBot)
            {
                parseAs = BluetoothCompanyIdentifier.CommonManufacturerType.SwitchBot;
            }
            if (parseAs == BluetoothCompanyIdentifier.CommonManufacturerType.SwitchBot)
            {
                var ble = wrapper.BleAdvert;
                foreach (var section in ble.Advertisement.DataSections)
                {
                    DataTypeValue dtv = ConvertDataTypeValue(section.DataType); // get the enum value
                    switch (dtv)
                    {
                        case DataTypeValue.ServiceData:
                            retval = ParseScanResponseServiceData(section);
                            break;
                    }
                }
            }

            return retval;
        }
        /// <summary>
        /// Main parser for the SwitchBot. All the other methods are merely wrappers for this one.
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static SwitchBot ParseScanResponseServiceData(BluetoothLEAdvertisementDataSection section)
        {
            var retval = new SwitchBot();

            try
            {
                var dr = DataReader.FromBuffer(section.Data);
                dr.ByteOrder = ByteOrder.LittleEndian;
                if (dr.UnconsumedBufferLength >= 2)
                {
                    var addr = dr.ReadUInt16();
                    switch (addr)
                    {
                        case 0xFD3D: // SwitchBot
                            byte modeRaw = dr.ReadByte();
                            char deviceType = (char)(modeRaw & 0x7F); // e.g 'T' for SwitchBit MeterTH
                            byte status = dr.ReadByte();
                            byte batteryRaw = dr.ReadByte();
                            uint batteryPercent = (uint)(batteryRaw & 0x7F);
                            byte fracRaw = dr.ReadByte();
                            uint temperatureFraction = (uint)(fracRaw & 0x0F);
                            byte tempRaw = dr.ReadByte();
                            bool tempNegative = (tempRaw & 0x80) == 0x00;
                            uint temp = (uint)(tempRaw & 0x7F);
                            double tempC = (double)temp + (double)temperatureFraction / 10.0;
                            double tempF = (tempC * 9.0 / 5.0) + 32.0;
                            if (tempNegative) tempC = -tempC;
                            byte humidityRaw = dr.ReadByte();
                            bool displayC = (humidityRaw & 0x80) == 0x00;
                            uint humidityPercent = (uint)(humidityRaw & 0x7F);

                            retval.ServiceId = addr;
                            retval.BatteryInPercent = (double)batteryPercent;
                            retval.Temperature = tempC;
                            retval.Humidity = (double)humidityPercent;
                            retval.IsValid = true;
                            retval.IsSensorPresent = SensorPresent.Temperature | SensorPresent.Humidity;
                            switch (deviceType)
                            {
                                case 'T': retval.TagType = SensorType.MeterTH; break;
                                default: retval.TagType = SensorType.Other; break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                retval.EncodeMessage = $"Unable to parse SwitchBot: {ex.Message}";
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
