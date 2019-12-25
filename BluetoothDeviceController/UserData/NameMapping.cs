using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.UserData
{
    public class NameMapping
    {
        /// <summary>
        /// The user-prefered name (e.g. DIG BLE for \0IG BLE CONTROLLER)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The bluetooth device id. Is, e.g., BluetoothLE#BluetoothLEbc:83:85:22:5a:70-00:a0:50:7c:b3:da 
        /// so that it includes a bunch of cruft in addition to the bluetooth address.
        /// </summary>
        public string Id { get; set; }
    }
}
