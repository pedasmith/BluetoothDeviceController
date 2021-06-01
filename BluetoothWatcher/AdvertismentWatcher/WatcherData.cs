using BluetoothDeviceController.BluetoothDefinitionLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using static BluetoothDeviceController.BluetoothDefinitionLanguage.BluetoothCompanyIdentifier;

namespace BluetoothWatcher.AdvertismentWatcher
{
    public class WatcherData
    {
        public String CompleteLocalName { get; set; } = "";
        public String ParsedCompanyData { get; set; } = "";
        public String ParsedCompanyDataTrim { get { return ParsedCompanyData.Trim('\n'); } }
        public ushort CompanyId { get; set; } = 65534; // is unknown. Is kind of a valid value?
        public CommonManufacturerType ManufacturerType { get; set; } = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
        public sbyte TransmitPower { get; set; } = 0;
        public object SpecializedDecodedData { get; set; } = null;
        public ulong Addr {  get { return OriginalArgs?.BluetoothAddress ?? 0; } }
        public BluetoothLEAdvertisementReceivedEventArgs OriginalArgs { get; set; }
    }
}
