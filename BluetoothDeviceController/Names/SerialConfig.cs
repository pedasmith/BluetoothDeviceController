using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Names
{
    public class SerialConfig
    {
        public string Id { get; set; } = "(name)";
        public string DeviceName { get; set; } = ""; // e.g. SlantRobotics is "Dev B"
        public string Name { get; set; } = "(name)";
        public string ClassName { get; set; }
        public string GetClassName() { return String.IsNullOrEmpty(ClassName) ? (Name ?? "") : ClassName; }
        public string ClassModifiers { get; set; }
        public string Description { get; set; }
        public string DefaultPin { get; set; }
        public List<string> Aliases { get; set; } = new List<string>(); // Must not be null
        public IList<String> Links { get; } = new List<String>();

        public Dictionary<string, SerialConfigSetting> Settings = new Dictionary<string, SerialConfigSetting>();
        public Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public override string ToString()
        {
            return $"{Name} Settings #{Settings.Count} Commands #{Commands.Count}";
        }
        public bool IsMatch(string deviceName, string id)
        {
            // The matches are optimistic -- goal is I can ask for deviceName="" id="Slant-Robotics-LittleBot" and get
            // the exact one I want.
            //var btMatch = BluetoothName == "" || bluetoothName == "" || bluetoothName.StartsWith(BluetoothName);
            var idMatch = id == "" || (!String.IsNullOrEmpty(Id) && id == Id);
            if (id == "(name)") idMatch = false; // default names don't match!
            var devMatch = deviceName == "" || (!String.IsNullOrEmpty(DeviceName) && deviceName.StartsWith(DeviceName));
            var match = idMatch && devMatch;
            return match;
        }
    }

    public class SerialConfigSetting
    {
        public string Label { get; set; } = null;
        public enum UiType {  TextBox, Slider, Hide};

        public string Name { get; set; } = null;
        public double Min { get; set; } = 0.0;
        public double Max { get; set; } = 100.0;
        public double Init { get; set; } = 0.0;
        public UiType InputType { get; set; } = UiType.TextBox;
        public string OnChange { get; set; } = null;
        public string CmdName { get; set; }
        public override string ToString()
        {
            return $"Settings Min={Min} Init={Init} Max={Max}";
        }
    }

    public class Command
    {
        public string Label { get; set; } = "KEY";
        /// <summary>
        /// Compute is a computed value: e.g. STRING^$BERO1.0. Values like space must be escaped. Use Replace for simple scenarios that involve just sending a string.
        /// </summary>
        public string Compute { get; set; } = "";
        /// <summary>
        /// A simple string to send to the device.
        /// </summary>
        public string Replace { get; set; } = "";
        /// <summary>
        /// The name of a file like SerialFiles/Espruino_Modules_Smartibot.js
        /// The file will be in the Assets directory.
        /// </summary>
        public string ReplaceFile { get; set; } = "";
        public string Alt { get; set; } = "";
        /// <summary>
        /// When set to true, the command is hidden
        /// </summary>
        public bool IsHidden { get; set; } = false;
        public string OnChange { get; set; } = null;
        public Dictionary<string, double> Set { get; } = new Dictionary<string, double>();
    }
}
