//using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothConversions;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothConversions.BluetoothCompanyIdentifier;
using static BluetoothProtocols.AdvertisementDataSectionParser;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWatcher.AdvertismentWatcher
{

    /// <summary>
    /// WatcherData is a super-summary of the data in the original Bluetooth advertisement + scan response. 
    /// The original is available as "OriginalAdvertisement"; if there's a ScanResponse, it's in "ResponseAdvertisement".
    /// 
    /// The fields are filled in in AdvertisementWatcher.
    /// A key point is that a Bluetooth advertisement can optionally contain a bunch of fields; the
    /// AdvertisementWatcher will read in each field and then fill in the WatcherData as appropriate.
    /// </summary>
    public class WatcherData
    {
        /// <summary>
        /// Advertisement CompleteLocalName (BT 0x09) or ShortenedLocalName (BT 0x08) or ""
        /// OR RuuviTag ___ for Eddystone based RuuviTags
        /// </summary>
        public String BestName { get; set; } = "";
        /// <summary>
        /// Parsed manufacturer data from ManufacturerData (BT 0xFF).
        /// </summary>
        public String ParsedCompanyData { get; set; } = "";

        public string EddystoneUrl { get; set; } = string.Empty;
        /// <summary>
        /// Same as ParsedCompanyData but doesn't incude CR
        /// </summary>
        public String ParsedCompanyDataTrim { get { return ParsedCompanyData.Trim('\n'); } }
        /// <summary>
        /// From the ManufacturerData (BT 0xFF)
        /// </summary>
        public ushort CompanyId { get; set; } = 0xBABA; // is unknown. Is kind of a valid value? //TODO: make this nullable
        public static ushort CompanyIdInvalidValue = 0xBABA;
        /// <summary>
        /// ManufacturerType is my interpretation of the CompanyId and only includes companies that I can parse.
        /// </summary>
        public CommonManufacturerType ManufacturerType { get; set; } = CommonManufacturerType.Other;
        /// <summary>
        /// From the TxPowerLevel field
        /// </summary>
        public sbyte? TransmitPower { get; set; } = null;
        /// <summary>
        /// Can be one of many different possible objects including Apple_iBeacon or Ruuvi_Tag
        /// </summary>
        public object SpecializedDecodedData { get; set; } = null;
        /// <summary>
        /// ULONG version of the address. See AddressAsString for the nicely formatted version.
        /// </summary>
        public ulong Addr {  get { return OriginalAdvertisement?.BluetoothAddress ?? 0; } }
        public string AddressAsString { get { return BluetoothAddress.AsString(Addr); } }

        /// <summary>
        /// List of service UUIDs in the advertisement. Include the 16-bit ones and the 32-bit
        /// (although that code hasn't been tested)
        /// </summary>
        public List<Guid> ServiceUuids = new List<Guid>();


        public string TimeStampHHmmssfff
        {  
            get
            {
                return MostRecentAdvertisement.Timestamp.ToString("HH:mm:ss.fff");
            } 
        }
        /// <summary>
        /// Original advertisement data
        /// </summary>
        public BluetoothLEAdvertisementReceivedEventArgs OriginalAdvertisement { get; set; }
        public BluetoothLEAdvertisementReceivedEventArgs ResponseAdvertisement { get; set; }
        public BluetoothLEAdvertisementReceivedEventArgs ExtendedAdvertisement { get; set; }
        public int NResponseAdvertisement { get; set; } = 0;
        public BluetoothLEAdvertisementReceivedEventArgs MostRecentAdvertisement
            { 
            get 
            {
                var originalOrExtended = ExtendedAdvertisement ?? OriginalAdvertisement;
                var retval = ResponseAdvertisement ?? originalOrExtended; return retval;  
            } 
        } 
        public List<BluetoothLEAdvertisementReceivedEventArgs> Advertisements
        {
            get
            {
                var retval = new List<BluetoothLEAdvertisementReceivedEventArgs>();
                if (OriginalAdvertisement != null) retval.Add(OriginalAdvertisement);
                if (ResponseAdvertisement != null) retval.Add(ResponseAdvertisement);
                if (ExtendedAdvertisement != null) retval.Add(ExtendedAdvertisement);
                return retval;
            }
        }

        public enum AdvertisementStringFormat { Full, CanCompare, AddressOnly };

        public void FixupBestName()
        {
            foreach (var advertisement in Advertisements)
            {
                foreach (var section in advertisement.Advertisement.DataSections)
                {
                    var dsname = AdvertisementSection_types.Decode(section.DataType);
                    switch ((AdvertisementDataSectionParser.DataTypeValue)section.DataType)
                    {
                        case AdvertisementDataSectionParser.DataTypeValue.ServiceData:
                            // Just in case it's an Eddystone beacon
                            var eddystoneResult = Eddystone.ParseEddystoneUrlArgs(section.Data);
                            if (eddystoneResult.Success)
                            {
                                if (eddystoneResult.Url.Contains("https://ruu.vi/"))
                                {
                                    BestName = "RuuviTag " + AddressAsString;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                var ruuvilist = advertisement.Advertisement.GetManufacturerDataByCompanyId(1177); // 1177 = 0x0499 is Ruuvi
                if (BestName == "" && ruuvilist.Count > 0)
                {
                    BestName = "Ruuvi " + AddressAsString; // Artificially create this.
                }
            }
        }

        public enum DeviceType {  CantTell, LEOnly, BREDROnly, DualMode };

        public DeviceType CalculateDeviceType()
        {
            // Per Copilot, dual mode etc is determined only by the Flags section, and that's only
            // in regular advertisement. It's not supposed to be in a scan response or extended advertisement.
            if (OriginalAdvertisement == null) return DeviceType.CantTell;
            if (OriginalAdvertisement.IsScanResponse) return DeviceType.CantTell;
            if (OriginalAdvertisement.AdvertisementType == BluetoothLEAdvertisementType.Extended) return DeviceType.CantTell;

            var retval = DeviceType.CantTell;
            var flags = OriginalAdvertisement.Advertisement.Flags;
            if (flags != null && flags.HasValue)
            {
                var f = flags.Value;
                if (f.HasFlag(BluetoothLEAdvertisementFlags.ClassicNotSupported))
                {
                    retval = DeviceType.LEOnly;
                }
                else if (f.HasFlag(BluetoothLEAdvertisementFlags.DualModeControllerCapable) || f.HasFlag(BluetoothLEAdvertisementFlags.DualModeHostCapable))
                {
                    retval = DeviceType.DualMode;
                }
                else
                {
                    retval = DeviceType.BREDROnly;
                }
            }
            return retval;
        }

        public string ToStringFull(AdvertisementStringFormat format = AdvertisementStringFormat.Full)
        {
            if (format == AdvertisementStringFormat.AddressOnly)
            {
                // Super quick return!
                return BluetoothAddress.AsString(Addr);
            }

            var ts = (format == AdvertisementStringFormat.Full) ? MostRecentAdvertisement.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.ff") : "NoTimeStamp";
            var power = (format == AdvertisementStringFormat.Full)
                ? $"{MostRecentAdvertisement.RawSignalStrengthInDBm},{MostRecentAdvertisement.TransmitPowerLevelInDBm}" : ",";
            var ds = MostRecentAdvertisement.Advertisement.DataSections;
            string flags = "";
            if (MostRecentAdvertisement.IsAnonymous) flags += "+Anonymous";
            if (MostRecentAdvertisement.IsConnectable) flags += "+Connectable";
            if (MostRecentAdvertisement.IsScanResponse) flags += "+ScanResponse";
            if (MostRecentAdvertisement.IsDirected) flags += "+Directed";
            if (flags.Length > 0) flags = flags.Substring(1); // remove leading +

            string retval = $"{ts},{BluetoothAddress.AsString(Addr)},{BestName}";

            retval += $",{OriginalAdvertisement.BluetoothAddressType},{MostRecentAdvertisement.AdvertisementType},{flags},{power},{ds.Count()}";
            foreach (var advertisement in Advertisements)
            {
                foreach (var section in advertisement.Advertisement.DataSections)
                {
                    var dsname = AdvertisementSection_types.Decode(section.DataType);
                    switch ((AdvertisementDataSectionParser.DataTypeValue)section.DataType)
                    {
                        case AdvertisementDataSectionParser.DataTypeValue.ServiceData:
                            // Just in case it's an Eddystone beacon
                            var eddystoneResult = Eddystone.ParseEddystoneUrlArgs(section.Data);
                            if (eddystoneResult.Success)
                            {
                                retval += $",{dsname} (0x{section.DataType:X02})={section.Data.ToSsv()} EddystoneURL={eddystoneResult.Url}";
                            }
                            else
                            {
                                retval += $",{dsname} (0x{section.DataType:X02})={section.Data.ToSsv()}";
                            }
                            break;
                        default:
                            retval += $",{dsname} (0x{section.DataType:X02})={section.Data.ToSsv()}";
                            break;
                    }
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
            return $"{BluetoothAddress.AsString(Addr)} {BestName} {ParsedCompanyDataTrim}";
        }

        /// <summary>
        /// Very full details of the WatcherData. Is used by the Advertisements display
        /// Goal is to display all the data known about the advertisement
        /// Used by the BTServicesAndCharacteristicsControl display
        /// </summary>
        public string ToStringDetails()
        {
            BluetoothLEAdvertisementReceivedEventArgs args = MostRecentAdvertisement;

            string retval = ToStringDetailsOne(OriginalAdvertisement, "");
            retval += DataSectionDetailsOne(OriginalAdvertisement, "");
            if (ExtendedAdvertisement != null)
            {
                retval += "**Extended advertisement:**\n";
                retval += ToStringDetailsOne(ExtendedAdvertisement, "&nbsp;&nbsp;&nbsp;&nbsp;");
                retval += DataSectionDetailsOne(ExtendedAdvertisement, "&nbsp;&nbsp;&nbsp;&nbsp;");
            }
            if (ResponseAdvertisement != null)
            {
                retval += "**Response advertisement:**\n";
                retval += ToStringDetailsOne(ResponseAdvertisement, "&nbsp;&nbsp;&nbsp;&nbsp;");
                retval += DataSectionDetailsOne(ResponseAdvertisement, "&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            foreach (var advertisement in Advertisements)
            {
                // I don't want to use the ManufacturerData directly because that already has the two-byte
                // ManufacturerID pulled out. The AI tools that can handle BT ManufacturerData prefer clean
                // and complete sections of data
                foreach (var section in advertisement.Advertisement.DataSections)
                {
                    byte b = section.DataType;
                    DataTypeValue dtv = ConvertDataTypeValue(b); // get the enum value e.g. Flags (0x01) or IncompleteListOf16BitServiceUuids (0x02)
                    if (dtv == DataTypeValue.ManufacturerData)
                    {
                        var str = "Manufacturer Hex: " + section.Data.ToHex() + "\n";
                        retval += str;
                    }
                }
            }

            // 
            // Lastly: let's find special data like for Govee 
            //

            var goveeType = Govee.AdvertIsSensorFamily(this);
            if (goveeType != Govee.SensorType.NotThisSensorFamily)
            {
                var goveeData = Govee.Parse(goveeType, this, null);
                if (goveeData != null)
                {
                    retval += $"Govee data: Type={goveeData.TagType}, Temp={goveeData.TemperatureInDegreesF:F1}F, Humidity={goveeData.Humidity}%, Battery={goveeData.BatteryInPercent}%\n";
                }
            }
            retval += "\n\nMore info at [Novelbits.io](https://novelbits.io/bluetooth-address-privacy-ble/)\n";
            return retval;
        }

        private string ToStringDetailsOne(BluetoothLEAdvertisementReceivedEventArgs args, string indent)
        {
            string retval = "";
            retval += $"{indent}Event time: {MostRecentAdvertisement.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}\n";
            retval += $"{indent}Address: {BluetoothAddress.AsString(Addr)}\n";
            retval += $"{indent}Address type: {args.BluetoothAddressType}\n";
            retval += $"{indent}Advertisement type: {args.AdvertisementType}\n";

            var isstr = "";
            if (args.IsConnectable) isstr += ",Connectable";
            if (args.IsDirected) isstr += ",Directed";
            if (args.IsScannable) isstr += ",Scannable";
            if (args.IsAnonymous) isstr += ",Anonymous";
            if (args.IsScanResponse) isstr += ",ScanResponse";
            if (isstr.Length > 0)
            {
                isstr = isstr.Substring(1); // remove leading (,) comma
                retval += $"{indent}Flags: {isstr}\n";
            }

            retval += $"{indent}Signal strength (dBm): {args.RawSignalStrengthInDBm}\n";
            if (args.TransmitPowerLevelInDBm != null)
            {
                retval += $"{indent}Transmit power (dBm): {args.TransmitPowerLevelInDBm}\n";
            }
            retval += $"{indent}Timestamp: {args.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\n";

            // Primary/Secondary PHY may not exist on all SDK versions; use reflection if present.
            var primaryPhyProp = args.GetType().GetProperty("PrimaryPhy");
            if (primaryPhyProp != null)
            {
                var primaryPhyVal = primaryPhyProp.GetValue(args);
                retval += $"{indent}Primary PHY: {primaryPhyVal}\n";
            }

            var secondaryPhyProp = args.GetType().GetProperty("SecondaryPhy");
            if (secondaryPhyProp != null)
            {
                var secondaryPhyVal = secondaryPhyProp.GetValue(args);
                retval += $"{indent}Secondary PHY: {secondaryPhyVal}\n";
            }
            return retval;
        }

        private string DataSectionDetailsOne(BluetoothLEAdvertisementReceivedEventArgs advertisement, string indent)
        {
            string retval = "";
            sbyte txPower = (sbyte)(advertisement.TransmitPowerLevelInDBm ?? 0);
            foreach (var section in advertisement.Advertisement.DataSections)
            {
                var mtype = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
                var (str, manufacturerType, companyId) = AdvertisementDataSectionParser.Parse(section, advertisement.RawSignalStrengthInDBm, txPower, mtype, "");
                retval += indent + "Section: " + str;
            }
            return retval;
        }
        /// <summary>
        /// Returns a string with \n for CR (or the defaultValue). The strings are in 
        /// almost JSON format tabs at the start and with double-quotes
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string ToStringManufacturerData(string defaultValue)
        {
            var retval = "";
            foreach (var advertisement in Advertisements)
            {
                // I don't want to use the ManufacturerData directly because that already has the two-byte
                // ManufacturerID pulled out. The AI tools that can handle BT ManufacturerData prefer clean
                // and complete sections of data
                foreach (var section in advertisement.Advertisement.DataSections)
                {
                    byte b = section.DataType;
                    DataTypeValue dtv = ConvertDataTypeValue(b); // get the enum value e.g. Flags (0x01) or IncompleteListOf16BitServiceUuids (0x02)
                    if (dtv == DataTypeValue.ManufacturerData)
                    {
                        var str = $"\t\t\t\t\"{section.Data.ToHex()}\",\n";
                        retval += str;
                    }
                }
            }
            if (retval == "") retval = defaultValue;
            return retval;
        }
    }
}
