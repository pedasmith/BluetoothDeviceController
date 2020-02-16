using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace BluetoothDeviceController
{
    public class DeviceInformationWrapper
    {
        public DeviceInformationWrapper (DeviceInformation deviceInformation)
        {
            di = deviceInformation;
        }
        public DeviceInformation di { get; set; } = null;
        public UserSerialPortPreferences SerialPortPreferences = null;

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
