using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothDeviceController.BleEditor
{
    public interface IWriteCharacteristic
    {
        Task<GattWriteResult> DoWriteString(string str);
    }

}
