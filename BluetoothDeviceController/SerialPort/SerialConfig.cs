using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.SerialPort
{

    static class AllShortcuts
    {
        public static IList<SerialConfig> All = null;
        public static void Init()
        {
            if (All == null)
            {
                All = BleNames.AllSerialDevices.AllSerialDevices;
            }
        }

        public static IList<SerialConfig> GetShortcuts(string deviceName, string id="")
        {
            var retval = new List<SerialConfig>();
            Init();
            if (All == null)
            {
                return retval; // just in case we try this before the init finishes.
            }
            foreach (var shortcut in All)
            {
                if (shortcut.IsMatch (deviceName, id))
                {
                    retval.Add(shortcut);
                }
            }
            return retval;
        }
    }
}
