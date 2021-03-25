using BluetoothDeviceController.BleEditor;
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
            var retval = BluetoothAppearance.AppearaceToString(appearance);
            return retval;
        }

        public static sbyte  ParseTxPowerLevel(BluetoothLEAdvertisementDataSection section)
        {
            var db = (sbyte)(section.Data.ToArray()[0]);
            return db;
        }

        public static (string result, BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType, UInt16 companyId) Parse(BluetoothLEAdvertisementDataSection section, sbyte txPower, string indent)
        {
            string str = "??";
            byte b = section.DataType;
            UInt16 companyId = 0xFFFF;
            DataTypeValue dtv = ConvertDataTypeValue(b); // get the enum value
            BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
            try
            {
                var printAsHex = false;
                switch (dtv)
                {
                    case DataTypeValue.ManufacturerData:
                        (str, manufacturerType, companyId) = BluetoothCompanyIdentifier.ParseManufacturerData(section, txPower);
                        break;
                    case DataTypeValue.Flags:
                        str = ParseFlags(section);
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
                    default:
                        printAsHex = true;
                        break;
                }
                if (printAsHex)
                {
                    var result = ValueParser.Parse(section.Data, "BYTES|HEX");
                    str = $"section {dtv.ToString()} data={result.AsString}\n";
                }
            }
            catch (Exception)
            {
                var result = ValueParser.Parse(section.Data, "BYTES|HEX");
                str = $"error section {section.DataType} data={result.AsString}\n";
            }
            if (!string.IsNullOrWhiteSpace(str)) str = indent + str;
            return (str, manufacturerType, companyId);
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
