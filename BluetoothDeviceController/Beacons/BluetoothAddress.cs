using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Beacons
{
    class BluetoothAddress
    {
        /// <summary>
        /// Converts a bluetooth address provided as a long into a useful string
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string AsString(ulong address)
        {
            var b5 = (byte)((address >> 40) & 0xFF);
            var b4 = (byte)((address >> 32) & 0xFF);
            var b3 = (byte)((address >> 24) & 0xFF);
            var b2 = (byte)((address >> 16) & 0xFF);
            var b1 = (byte)((address >>  8) & 0xFF);
            var b0 = (byte)((address >>  0) & 0xFF);
            var retval = $"{b5:X}:{b4:X}:{b3:X}:{b2:X}:{b1:X}:{b0:X}";
            return retval;
        }
    }
}
