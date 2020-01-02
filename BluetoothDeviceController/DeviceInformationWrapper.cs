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
    }
}
