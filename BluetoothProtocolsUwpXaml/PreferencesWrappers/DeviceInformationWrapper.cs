using BluetoothDeviceController.Beacons;
using BluetoothProtocols;
using BluetoothProtocols.Beacons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;

namespace BluetoothDeviceController
{
    public class DeviceInformationWrapper
    {
        public DeviceInformationWrapper(DeviceInformation deviceInformation)
        {
            di = deviceInformation;
        }
        public DeviceInformationWrapper(BleAdvertisementWrapper bleAdvert)
        {
            BleAdvert = bleAdvert;
        }
        public DeviceInformation di { get; set; } = null;
        public UserSerialPortPreferences SerialPortPreferences = null;
        public UserBeaconPreferences BeaconPreferences = null;
        public UserPreferences UserPreferences = null;
        public BleAdvertisementWrapper BleAdvert = null;

        bool _IsNordicUart = false;
        bool _IsNordicUartChecked = false;
        public BluetoothProtocolsCustom.Nordic_Uart AsNordicUart { get; internal set; } = null;

        public async Task<bool> IsNordicUartAsync()
        {
            if (di == null) return false; // but don't update the IsNordicUartChecked
            if (_IsNordicUartChecked) return _IsNordicUart;
            _IsNordicUartChecked = true;
            AsNordicUart = new BluetoothProtocolsCustom.Nordic_Uart(di);
            var retval = await AsNordicUart.EnsureCharacteristicAsync();
            return retval;
        }
    }
}
