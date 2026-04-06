//using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothConversions;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothConversions.BluetoothCompanyIdentifier;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWatcher.AdvertismentWatcher
{
    public static class IBufferExtensions
    {
        public static byte[] ToByteArray(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                return byteArray;
            }
        }
        public static string ToCsv(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                StringBuilder retval = new StringBuilder();
                foreach (var b in byteArray)
                {
                    if (retval.Length > 0) retval.Append(",");
                    retval.Append($"0x{b:X02}");
                }
                return retval.ToString();
            }
        }
        public static string ToSsv(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                StringBuilder retval = new StringBuilder();
                foreach (var b in byteArray)
                {
                    if (retval.Length > 0) retval.Append(" ");
                    retval.Append($"0x{b:X02}");
                }
                return retval.ToString();
            }
        }
    }
    public class WatcherData
    {
        public String CompleteLocalName { get; set; } = "";
        public String ParsedCompanyData { get; set; } = "";
        public String ParsedCompanyDataTrim { get { return ParsedCompanyData.Trim('\n'); } }
        public ushort CompanyId { get; set; } = 65534; // is unknown. Is kind of a valid value? //TODO: make this nullable
        public CommonManufacturerType ManufacturerType { get; set; } = CommonManufacturerType.Other;
        public sbyte TransmitPower { get; set; } = 0;
        public object SpecializedDecodedData { get; set; } = null;
        public ulong Addr {  get { return OriginalArgs?.BluetoothAddress ?? 0; } }
        public BluetoothLEAdvertisementReceivedEventArgs OriginalArgs { get; set; }

        public enum AdvertisementStringFormat { Full, CanCompare, AddressOnly };
        public string ToStringFull(AdvertisementStringFormat format = AdvertisementStringFormat.Full)
        {
            if (format == AdvertisementStringFormat.AddressOnly)
            {
                // Super quick return!
                return BluetoothAddress.AsString(Addr);
            }

            var ts = (format == AdvertisementStringFormat.Full) ? OriginalArgs.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ff") : "NoTimeStamp";
            var power = (format == AdvertisementStringFormat.Full)
                ? $"{OriginalArgs.RawSignalStrengthInDBm},{OriginalArgs.TransmitPowerLevelInDBm}" : ",";
            var ds = OriginalArgs.Advertisement.DataSections;
            string flags = "";
            if (OriginalArgs.IsAnonymous) flags += "+Anonymous";
            if (OriginalArgs.IsConnectable) flags += "+Connectable";
            if (OriginalArgs.IsScanResponse) flags += "+ScanResponse";
            if (OriginalArgs.IsDirected) flags += "+Directed";
            if (flags.Length > 0) flags = flags.Substring(1); // remove leading +

            string retval = $"{ts},{BluetoothAddress.AsString(Addr)},{CompleteLocalName}";

            retval += $",{OriginalArgs.BluetoothAddressType},{OriginalArgs.AdvertisementType},{flags},{power},{ds.Count()}";
            foreach (var section in OriginalArgs.Advertisement.DataSections)
            {
                var dsname = Ad_types.Decode(section.DataType);
                switch (section.DataType)
                {
                    default:
                        retval += $",{dsname} (0x{section.DataType:X02})={section.Data.ToSsv()}";
                        break;
                }
            }
            return retval;
        }

        public static string ToHeaderString()
        {
            return "TimeStamp,Address,LocalName,AddressType,AdvertisementType,Flags,SignalStrength,TransmitPower,NSection,Section1,Section2,Section3,Section4,Section5";
        }
        public override string ToString()
        {
            return $"{BluetoothAddress.AsString(Addr)} {CompleteLocalName} {ParsedCompanyDataTrim}";
        }
    }
}
