using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController
{
    interface HasId
    {
        /// <summary>
        /// For example, BBC micro:bit
        /// </summary>
        /// <returns></returns>
        string GetDeviceNameUser();
        string GetId();
        string GetPicturePath();
    }
}
