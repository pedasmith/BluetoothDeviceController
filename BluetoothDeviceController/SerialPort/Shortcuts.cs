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

        public string Id { get; internal set; } = "no-id-set";
        public string Name { get; internal set; } = "no-name-set";

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

        public bool IsMatch (string deviceName, string id)
        {
            // The matches are optimistic -- goal is I can ask for deviceName="" id="Slant-Robotics-LittleBot" and get
            // the exact one I want.
            //var btMatch = BluetoothName == "" || bluetoothName == "" || bluetoothName.StartsWith(BluetoothName);
            var idMatch = id == "" || (!String.IsNullOrEmpty(Id) && id==Id);
            var devMatch = deviceName == "" || (!String.IsNullOrEmpty(DeviceName) && deviceName.StartsWith(DeviceName));
            var match = idMatch && devMatch;
            return match;
        }

    }

    static class AllShortcuts
    {
        public static List<Shortcuts> All = null;
        public static void Init()
        {
            if (All == null)
            {
                All = new List<Shortcuts>();
                All.Add(new SlantRobotics_LittleBot_Shortcuts()); // 2017 robot
                All.Add(new StandardRobotCar_Shortcuts()); 
            }
        }

        public static IList<Shortcuts> GetShortcuts(string deviceName, string id="")
        {
            var retval = new List<Shortcuts>();
            Init();
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
