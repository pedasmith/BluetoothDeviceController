using BluetoothDeviceController.BleEditor;
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

        public Dictionary<string, VariableDescription> Settings = new Dictionary<string, VariableDescription>();
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

    public class VariableDescription
    {
        public string Label { get; set; } = null;
        public enum UiType { TextBox, Slider, Hide, ComboBox };

        public string Name { get; set; } = null;
        public double Min { get; set; } = 0.0;
        public double Max { get; set; } = 100.0;
        public double Init { get; set; } = 0.0;
        public string InitString { get; set; } = "";
        public double CurrValue { get; set; } = 0.0;
        public string CurrValueString { get; set; } = "";
        public bool CurrValueIsString { get; set; } = false;
        /// <summary>
        /// Set of enum values e.g. ValueNames: { "All":0, "Lights_Off":1, "Stop":2, "Mute":3 }
        /// </summary>
        public Dictionary<string, double> ValueNames { get; } = new Dictionary<string, double>();
        /// <summary>
        /// Input type, one of TextBox (default), Slider, Hide, and ComboBox or ComboBoxOrSlider. The ComboBox
        /// will only be used when ValueNames are provided.
        /// </summary>
        public UiType InputType { get; set; } = UiType.TextBox;
        public string OnChange { get; set; } = null;
        public string CmdName { get; set; }
        public override string ToString()
        {
            return $"Settings {Name} Min={Min} Init={Init} Max={Max}";
        }
    }

    public class Command
    {
        public string Label { get; set; } = null;
        /// <summary>
        /// Compute is a computed value: e.g. STRING^$BERO1.0. Values like space must be escaped. Use Replace for simple scenarios that involve just sending a string.
        /// Values are seperated by spaces e.g "${CLEAR[ $ClearMode_GN $]}" is split into three values
        /// which will be concatenated together.
        /// </summary>
        public string Compute { get; set; } = "";
        /// <summary>
        /// A simple string to send to the device. Why replace? Because why not, that's why.
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
        /// <summary>
        /// Provides explicit values for the values needed by a command
        /// </summary>
        public Dictionary<string, double> Set { get; } = new Dictionary<string, double>();

        /// <summary>
        /// Variables that can be used with the command. These are per-command.
        /// </summary>
        public Dictionary<string, VariableDescription> Parameters { get; } = new Dictionary<string, VariableDescription>();

        public IWriteCharacteristic WriteCharacteristic { get; set; } = null;

        VariableSet Variables = new VariableSet();
        public void InitVariables()
        {
            Variables.Init(Set);
            Variables.Init(Parameters);
        }

        public string DoCompute()
        {
            Variables.SetCurrValue(Parameters);

            var list = Compute.Split(new char[] { ' ' });
            var cmd = "";
            foreach (var strcommand in list)
            {
                var calculateResult = BleEditor.ValueCalculate.Calculate(strcommand, double.NaN, null, Variables);
                cmd += calculateResult.S ?? calculateResult.D.ToString();
            }
            Variables.CopyToPrev();
            return cmd;
        }
        public async Task DoCommand()
        {
            if (WriteCharacteristic == null) return; // can happen at startup
            var cmd = DoCompute();
            var result = await WriteCharacteristic.DoWriteString(cmd);
            System.Diagnostics.Debug.WriteLine($"COMMAND: {cmd}");
            ;
        }

        public void SetCurrDouble(string name, double value)
        {
            Parameters[name].CurrValue = value;
        }
    }
}
