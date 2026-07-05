using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocolsDevicesCore
{
    internal static class Nordic_Thingy_Extension
    {
        /// <summary>
        /// Returns true when the data has valid pressure, humidity data. Can't detect when the temperature is invalid
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsValidPH(this Nordic_Thingy.Environment_Data data)
        {
            // 0.0 is a valid temperature :-(
            var retval = (data.Pressure != 0.0 && data.Humidity != 0.0);
            return retval;
        }
    }
}
