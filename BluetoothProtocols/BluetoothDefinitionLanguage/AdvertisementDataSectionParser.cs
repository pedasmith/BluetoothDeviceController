using BluetoothDeviceController.BleEditor;
using BluetoothProtocols.Beacons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.BluetoothDefinitionLanguage
{
    public class AdvertisementDataSectionParser
    {
        public enum DataTypeValue
        {
            Unknown = -1,
            Flags = 0x01,
            IncompleteListOf16BitServiceUuids = 0x02,
            CompleteListOf16BitServiceUuids = 0x03,
            IncompleteListOf32BitServiceUuids = 0x04,
            CompleteListOf32BitServiceUuids = 0x05,
            IncompleteListOf128BitServiceUuids = 0x06,
            CompleteListOf128BitServiceUuids = 0x07,
            ShortenedLocalName = 0x08,
            CompleteLocalName = 0x09,
            TxPowerLevel = 0x0a,
            ClassOfDevice = 0x0d,
            SimplePairingHashC = 0x0e,
            SimpleParingRandomizer = 0x0f,
            DeviceId = 0x10,
            SecurityManagerOobFlags = 0x11,
            SlConnectionIntervalRange = 0x12,
            ListOf16BitServiceSolicitationUuids = 0x14,
            ListOf128BitServiceSolicitationUuids = 0x15,
            ServiceData = 0x16,
            PublicTargerAddress = 0x17,
            RandomTargetAddress = 0x18,
            Appearance = 0x19,
            AdvertisingInterval = 0x1A,
            LEBluetoothDeviceAddress = 0x1B,
            LERole = 0x1C,
            SimplePairingHashC256 = 0x1D,
            ListOf32BitServiceSolicitationUuids = 0x1E,
            ServiceData32BitUuid = 0x20,
            ServiceData128BitUuid = 0x21,
            LESecureConnectionConfirmationValue = 0x22,
            LESecureConnectionRandomValue = 0x23,
            Uri = 0x24,
            
            ManufacturerData = 0xFF,
        }
        /// <summary>
        /// Handy routine converts the incoming byte from a bluetooth advert into a DataTypeValue enum
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static DataTypeValue ConvertDataTypeValue (byte b)
        {
            try
            {
                DataTypeValue dtv = (DataTypeValue)b; // get the enum value
                return dtv;
            }
            catch (Exception)
            {
                return DataTypeValue.Unknown;
            }
        }

        public static string ParseAppearance(BluetoothLEAdvertisementDataSection section)
        {
            var dr = DataReader.FromBuffer(section.Data);
            dr.ByteOrder = ByteOrder.LittleEndian; // bluetooth is little-endian by default
            if (dr.UnconsumedBufferLength < 2)
            {
                return $"?len={dr.UnconsumedBufferLength}";
            }
            var appearance = dr.ReadUInt16();
            var retval = BluetoothAppearance.AppearanceToString(appearance);
            return retval;
        }

        public static sbyte  ParseTxPowerLevel(BluetoothLEAdvertisementDataSection section)
        {
            var db = (sbyte)(section.Data.ToArray()[0]);
            return db;
        }

        public static (string result, BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType, UInt16 companyId) Parse(BluetoothLEAdvertisementDataSection section, sbyte txPower, BluetoothCompanyIdentifier.CommonManufacturerType parseAs, string indent)
        {
            string str = "??";
            byte b = section.DataType;
            UInt16 companyId = 0xFFFF;
            DataTypeValue dtv = ConvertDataTypeValue(b); // get the enum value
            object speciality = null;
            var manufacturerType = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
            var printAsHex = false;
            var hexPrefix = "";
            try
            {
                switch (dtv)
                {
                    case DataTypeValue.ManufacturerData:
                        (str, manufacturerType, companyId, speciality) = BluetoothCompanyIdentifier.ParseManufacturerData(section, txPower, parseAs);
                        break;
                    case DataTypeValue.Flags:
                        str = ParseFlags(section);
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.IncompleteListOf128BitServiceUuids:
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteListOf128BitServiceUuids:
                        {
                            // Viatom pulse oximeter has these
                            // Correct output: 6e400001-b5a3-f393-e0a9-e50e24dcca9e
                            // Actual  output: 6E400001-B5A3-F393-E0A9-E50E24DCCA9E
                            str = "";
                            var pre = dtv == DataTypeValue.CompleteListOf128BitServiceUuids ? "Service UUIDs (complete): " : "Service UUIDs (incomplete)";
                            var dr = DataReader.FromBuffer(section.Data);
                            dr.ByteOrder = ByteOrder.LittleEndian;
                            while (dr.UnconsumedBufferLength >= 16)
                            {
                                var addr0 = dr.ReadUInt16();
                                var addr1 = dr.ReadUInt16();
                                var addr2 = dr.ReadUInt16();
                                var addr3 = dr.ReadUInt16();
                                var addr4 = dr.ReadUInt16();
                                var addr5 = dr.ReadUInt16();
                                var addr6 = dr.ReadUInt16();
                                var addr7 = dr.ReadUInt16();
                                //var service = $"{addr0:X02}:{addr1:X02}:{addr2:X02}:{addr3:X02}:{addr4:X02}:{addr5:X02}:{addr6:X02}:{addr7:X02}";
                                var service = $"{addr7:X04}{addr6:X04}-{addr5:X04}-{addr4:X04}-{addr3:X04}-{addr2:X04}{addr1:X04}{addr0:X04}";
                                if (str != "") str += ", ";
                                str += service;
                            }
                            if (str == "")
                            {
                                str = pre + "\n";
                            }
                            else
                            {
                                str = pre + str + "\n";
                            }
                        }
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.IncompleteListOf16BitServiceUuids:
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteListOf16BitServiceUuids:
                        {
                            str = "";
                            var pre = dtv == DataTypeValue.CompleteListOf16BitServiceUuids ? "Service UUIDs (complete): " : "Service UUIDs (incomplete)";
                            var dr = DataReader.FromBuffer(section.Data);
                            dr.ByteOrder = ByteOrder.LittleEndian;
                            while (dr.UnconsumedBufferLength >= 2)
                            {
                                var addr = dr.ReadUInt16();
                                var service = BluetoothServiceUuid16Bit.Decode(addr);
                                if (service == "") service = $"{addr:X02}";
                                else service += $"={addr:X02}";
                                if (str != "") str += ", ";
                                str += service;
                            }
                            if (str == "")
                            {
                                str = pre + "\n";
                            }
                            else
                            {
                                str = pre + str + "\n";
                            }
                        }
                        break;
                    case DataTypeValue.ShortenedLocalName:
                        {
                            var buffer = section.Data.ToArray();
                            var allNul = true;
                            foreach (var namebyte in buffer)
                            {
                                if (namebyte != 0) allNul = false;
                            }
                            if (allNul)
                            {
                                str = "";
                            }
                            else
                            {
                                printAsHex = true;
                            }
                        }
                        break;
                    case DataTypeValue.TxPowerLevel:
                        var db = ParseTxPowerLevel(section);
                        str = $"{db}";
                        break;
                    case DataTypeValue.ServiceData:
                        SwitchBot switchbot = null;
                        switch (parseAs)
                        {
                            case BluetoothCompanyIdentifier.CommonManufacturerType.SwitchBot:
                                switchbot = SwitchBot.ParseScanResponseServiceData(section);
                                str = switchbot.ToString();
                                break;
                        }
                        if (switchbot == null)
                        {
                            var servicedatastr = ValueParser.Parse(section.Data, "U16|HEX BYTES|HEX");
                            str = $"{hexPrefix}section {dtv.ToString()} data={servicedatastr.AsString}\n";
                        }
                        break;
                }
            }
            catch (Exception)
            {
                printAsHex = true;
                hexPrefix = "error ";
            }
            if (printAsHex)
            {
                var hexstr = ValueParser.Parse(section.Data, "BYTES|HEX");
                str = $"{hexPrefix}section {dtv.ToString()} data={hexstr.AsString}\n";
            }
            if (!string.IsNullOrWhiteSpace(str)) str = indent + str;
            return (str, manufacturerType, companyId);
        }
        public static BluetoothCompanyIdentifier.CommonManufacturerType ParseManufacturerType(BluetoothLEAdvertisementDataSection section, sbyte txPower, string indent)
        {
            byte b = section.DataType;
            DataTypeValue dtv = ConvertDataTypeValue(b); // get the enum value
            BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
            try
            {
                switch (dtv)
                {
                    case DataTypeValue.ManufacturerData:
                        manufacturerType = BluetoothCompanyIdentifier.ParseManufacturerDataType(section, txPower);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
            return manufacturerType;
        }
        /// <summary>
        /// Gotta say, the flags data is pretty uninteresting. Maybe only show flags
        /// when there's a special switch set?
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public static string ParseFlags(BluetoothLEAdvertisementDataSection section)
        {
            try
            {
                var bytes = section.Data.ToArray();
                var b0 = bytes[0];
                var str = "";
                if ((b0 & 0x01) != 0)
                {
                    if (str != "") str += "+";
                    str += "LE Limited Discoverable Mode";
                }
                if ((b0 & 0x02) != 0)
                {
                    if (str != "") str += "+";
                    str += "LE General Discoverable Mode";
                }
                // It just says if classic bluetooth is supported or not.
                if ((b0 & 0x04) != 0)
                {
                    if (str != "") str += "+";
                    str += "BR/EDR Not Supported";
                }
                // BDR/EDR is not interesting.
                if (str != "") str += "\n";
                return str;
            }
            catch (Exception)
            {
                return "??\n";
            }
        }
    }
}
