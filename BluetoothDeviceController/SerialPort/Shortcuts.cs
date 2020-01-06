using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.SerialPort
{
    class Shortcut
    {
        public string Label { get; set; } = "KEY";
        /// <summary>
        /// The Replace is what will be sent to the device. This is always the EXACT string; it's
        /// not interpreted, etc. 
        /// </summary>
        public string Replace { get; set; } = "REPLACE";
        public string Alt { get; set; } = "";
    }
    class Shortcuts
    {
        // No good way to get this.
        // public string BluetoothName { get; internal set; } = "";

        /// <summary>
        /// E.G. the SlantRobotics LittleBot is an "HC 06" for bluetooth but "Dev B" for device name.
        /// </summary>
        public string DeviceName { get; internal set; } = "";

        public string Description { get; internal set; } = "";

        public List<Shortcut> List { get; } = new List<Shortcut>();
        public void Add (string label, string replace, string alt)
        {
            var sc = new Shortcut() { Label = label, Replace = replace, Alt = alt };
            List.Add(sc);
        }

        public bool IsMatch (string deviceName)
        {
            //var btMatch = BluetoothName == "" || bluetoothName == "" || bluetoothName.StartsWith(BluetoothName);
            var devMatch = !String.IsNullOrEmpty(DeviceName) && deviceName.StartsWith(DeviceName);
            //var match = btMatch || devMatch;
            return devMatch;
        }

    }

    static class AllShortcuts
    {
        static List<Shortcuts> All = null;
        public static void Init()
        {
            if (All == null)
            {
                All = new List<Shortcuts>();
                All.Add(new SlantRobotics_LittleBot_Shortcuts()); // 2017 robot
                All.Add(new StandardRobotCar_Shortcuts()); 
            }
        }

        public static IList<Shortcuts> GetShortcuts(string deviceName)
        {
            var retval = new List<Shortcuts>();
            Init();
            foreach (var shortcut in All)
            {
                if (shortcut.IsMatch (deviceName))
                {
                    retval.Add(shortcut);
                }
            }
            return retval;
        }
    }
}
