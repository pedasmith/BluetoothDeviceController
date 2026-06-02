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
        /// </summary>
        public String BestName { get; set; } = "";
        /// <summary>
        /// Parsed manufacturer data from ManufacturerData (BT 0xFF).
        /// </summary>
        public String ParsedCompanyData { get; set; } = "";
        /// <summary>
        /// Same as ParsedCompanyData but doesn't incude CR
        /// </summary>
        public String ParsedCompanyDataTrim { get { return ParsedCompanyData.Trim('\n'); } }
        /// <summary>
        /// From the ManufacturerData (BT 0xFF)
        /// </summary>
        public ushort CompanyId { get; set; } = 65534; // is unknown. Is kind of a valid value? //TODO: make this nullable
        /// <summary>
        /// ManufacturerType is my interpretation of the CompanyId and only includes companies that I can parse.
        /// </summary>
        public CommonManufacturerType ManufacturerType { get; set; } = CommonManufacturerType.Other;
        /// <summary>
        /// From the TxPowerLevel field
        /// </summary>
        public sbyte TransmitPower { get; set; } = 0;
        /// <summary>
        /// Can be one of many different possible objects including Apple_iBeacon or Ruuvi_Tag
        /// </summary>
        public object SpecializedDecodedData { get; set; } = null;
        /// <summary>
        /// ULONG version of the address. See AddressAsString for the nicely formatted version.
        /// </summary>
        public ulong Addr {  get { return OriginalAdvertisement?.BluetoothAddress ?? 0; } }
        public string AddressAsString { get { return BluetoothAddress.AsString(Addr); } }

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
        public BluetoothLEAdvertisementReceivedEventArgs MostRecentAdvertisement
            { get {  var retval = ResponseAdvertisement ?? OriginalAdvertisement; return retval;  } } 

        public enum AdvertisementStringFormat { Full, CanCompare, AddressOnly };
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
            if (OriginalAdvertisement != null)
            {
                foreach (var section in OriginalAdvertisement.Advertisement.DataSections)
                {
                    var dsname = AdvertisementSection_types.Decode(section.DataType);
                    switch (section.DataType)
                    {
                        default:
                            retval += $",{dsname} (0x{section.DataType:X02})={section.Data.ToSsv()}";
                            break;
                    }
                }
            }
            if (ResponseAdvertisement != null)
            {
                foreach (var section in ResponseAdvertisement.Advertisement.DataSections)
                {
                    var dsname = AdvertisementSection_types.Decode(section.DataType);
                    switch (section.DataType)
                    {
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

        public string ToStringDetails()
        {
            var args = MostRecentAdvertisement;
            string retval = "";
            retval += $"Address: {BluetoothAddress.AsString(Addr)}\n";
            retval += $"Address type: {args.BluetoothAddressType}\n";
            retval += $"Advertisement type: {args.AdvertisementType}\n";

            var isstr = "";
            if (args.IsAnonymous) isstr += ",Anonymous";
            if (args.IsConnectable) isstr += ",Connectable";
            if (args.IsScanResponse) isstr += ",ScanResponse";
            if (args.IsDirected) isstr += ",Directed";
            if (isstr.Length > 0) isstr = isstr.Substring(1); // remove leading +
            retval += $"Flags: {isstr}\n";

            retval += $"Signal strength (dBm): {args.RawSignalStrengthInDBm}\n";
            retval += $"Transmit power (dBm): {args.TransmitPowerLevelInDBm}\n";
            retval += $"Timestamp: {args.Timestamp:yyyy-MM-dd HH:mm:ss.fff}\n";

            // Primary/Secondary PHY may not exist on all SDK versions; use reflection if present.
            var primaryPhyProp = args.GetType().GetProperty("PrimaryPhy");
            if (primaryPhyProp != null)
            {
                var primaryPhyVal = primaryPhyProp.GetValue(args);
                retval += $"Primary PHY: {primaryPhyVal}\n";
            }

            var secondaryPhyProp = args.GetType().GetProperty("SecondaryPhy");
            if (secondaryPhyProp != null)
            {
                var secondaryPhyVal = secondaryPhyProp.GetValue(args);
                retval += $"Secondary PHY: {secondaryPhyVal}\n";
            }

            // And now, data directly from the advertisement!

            var adv = OriginalAdvertisement?.Advertisement;
            if (adv != null)
            {
                foreach (var section in adv.DataSections)
                {
                    sbyte txPower = (sbyte)(args.TransmitPowerLevelInDBm ?? 0);
                    var mtype = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
                    var (str, manufacturerType, companyId) = AdvertisementDataSectionParser.Parse(section, txPower, mtype, "");
                    //var dsname = AdvertisementSection_types.Decode(section.DataType);
                    //retval += $"Section: {dsname} (0x{section.DataType:X02})={section.Data.ToHex()}\n";
                    retval += str;
                }
            }

            adv = ResponseAdvertisement?.Advertisement;
            if (adv != null)
            {
                foreach (var section in adv.DataSections)
                {
                    sbyte txPower = (sbyte)(args.TransmitPowerLevelInDBm ?? 0);
                    var mtype = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
                    var (str, manufacturerType, companyId) = AdvertisementDataSectionParser.Parse(section, txPower, mtype, "");
                    //var dsname = AdvertisementSection_types.Decode(section.DataType);
                    //retval += $"Section: {dsname} (0x{section.DataType:X02})={section.Data.ToHex()}\n";
                    retval += str;
                }
            }

            // 
            // Lastly: let's find special data like for Govee 
            //

            var goveeType = Govee.AdvertIsGovee(this);
            if (goveeType != Govee.SensorType.NotGovee)
            {
                var goveeData = Govee.Parse(goveeType, this, null);
                if (goveeData != null)
                {
                    retval += $"Govee: Type={goveeData.TagType}, Temp={goveeData.TemperatureInDegreesF:F1}F, Humidity={goveeData.Humidity}%, Battery={goveeData.BatteryInPercent}%\n";
                }
            }

            return retval;
        }
    }
}
