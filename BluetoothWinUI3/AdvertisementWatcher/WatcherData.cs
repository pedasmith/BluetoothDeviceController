//using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using static BluetoothConversions.BluetoothCompanyIdentifier;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWatcher.AdvertismentWatcher
{
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

        public override string ToString()
        {
            return $"{BluetoothAddress.AsString(Addr)} {CompleteLocalName} {ParsedCompanyDataTrim}";
        }
    }
}
