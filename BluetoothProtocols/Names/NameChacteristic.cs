using BluetoothProtocols.IotNumberFormats;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
// Location: BluetoothProtocols/Names
namespace BluetoothDeviceController.Names
{
    public class SimpleUI
    {
        public string UIType { get; set; }
        public string Target { get; set; }
        public string ComputeTarget { get; set; }
        public string Label { get; set; }
        public string FunctionName { get; set; }
        public List<string> Set { get; } = new List<string>();
        public double N { get; set; } = double.NaN;
        public int GetN(int defaultN = 4)
        {
            if (double.IsNaN(N)) return defaultN;
            return (int)N;
        }
        //public override string ToString()
        //{
        //    return $"SimpleUI: type={UIType} target={Target} label={Label} fname={FunctionName}";
        //}
    }

    public class ButtonPerButtonUI
    {
        public enum ButtonType { Button, Blank };
        public ButtonType Type { get; set; } = ButtonType.Button;
        public string Enum { get; set; }
        public string Label { get; set; }
        public override string ToString()
        {
            switch (Type)
            {
                case ButtonType.Blank:
                    return "Type=Blank";
                case ButtonType.Button:
                    return $"Type=Button Enum={Enum} Label={Label}";
            }
            return "??Unknown type";
        }
    }
    public class ButtonUI
    {
        public string DefaultEnum { get; set; } = null;
        public int MaxColumns { get; set; } = 5; // Use a reasonable default here!
        public IList<ButtonPerButtonUI> Buttons { get; } = new List<ButtonPerButtonUI>();
    }

    public class ChartLineDefaults
    {
        public string stroke = "Green";

    }

    public class UISpecifications
    {
        /// <summary>
        /// Allowed values: "" and "standard"; says whether there will be a panel of buttons or not.
        /// As of Feb 2020, only characteristics with a set of Enums can have buttons.
        /// </summary>
        public string buttonType { get; set; } = "";
        /// <summary>
        /// If there's supposed to be buttons, but there's no buttonUI, then we will make buttons 
        /// straight from the set of enums. This is a workable first pass for the resuts, but makes
        /// a mediocre set of buttons (bad labels, etc).
        /// </summary>
        public ButtonUI buttonUI { get; set; } 
        /// <summary>
        /// TableType is either blank or "standard"
        /// </summary>
        public string tableType { get; set; } = "";
        public string chartType { get; set; } = "";
        /// <summary>
        /// UICHARTCOMMAND
        /// </summary>
        public string chartCommand { get; set; } = "";
        /// <summary>
        /// When TRUE, the extra UI (Chart, Table) will be automatically expanded. Default is false for compatibility.
        /// </summary>
        public bool Expand { get; set; } = false; // default is false; must be set true in the JSON.
        /// <summary>
        /// Is UI.TitleSuffix; is the title of the expander for tables and charts
        /// </summary>
        public string TitleSuffix { get; set; } = "Data tracker";

        public double? chartDefaultMaxY { get; set; } = null;
        public double? chartMaxY { get; set; } = null;
        public double? chartDefaultMinY { get; set; } = null;
        public double? chartMinY { get; set; } = null;

        public enum YMinMaxCombined { Combined, Separate };
        public YMinMaxCombined chartYAxisCombined { get; set; } = YMinMaxCombined.Combined;
        public double? chartDefaultMinY0 { get; set; } = null;
        public double? chartDefaultMaxY0 { get; set; } = null;
        public double? chartDefaultMinY1 { get; set; } = null;
        public double? chartDefaultMaxY1 { get; set; } = null;
        public double? chartDefaultMinY2 { get; set; } = null;
        public double? chartDefaultMaxY2 { get; set; } = null;
        public double? chartDefaultMinY3 { get; set; } = null;
        public double? chartDefaultMaxY3 { get; set; } = null;
        public double? chartDefaultMinY4 { get; set; } = null;
        public double? chartDefaultMaxY4 { get; set; } = null;
        public double? GetChartDefaultMinY(int lineIndex)
        {
            switch (lineIndex)
            {
                case 0: return chartDefaultMinY0 ?? chartDefaultMinY;
                case 1: return chartDefaultMinY1 ?? chartDefaultMinY;
                case 2: return chartDefaultMinY2 ?? chartDefaultMinY;
                case 3: return chartDefaultMinY3 ?? chartDefaultMinY;
                case 4: return chartDefaultMinY4 ?? chartDefaultMinY;
            }
            return chartDefaultMinY;
        }
        public double? GetChartDefaultMaxY(int lineIndex)
        {
            switch (lineIndex)
            {
                case 0: return chartDefaultMaxY0 ?? chartDefaultMaxY;
                case 1: return chartDefaultMaxY1 ?? chartDefaultMaxY;
                case 2: return chartDefaultMaxY2 ?? chartDefaultMaxY;
                case 3: return chartDefaultMaxY3 ?? chartDefaultMaxY;
                case 4: return chartDefaultMaxY4 ?? chartDefaultMaxY;
            }
            return chartDefaultMaxY;
        }

        public double? SetChartDefaultMinY(int lineIndex, double value)
        {
            switch (lineIndex)
            {
                case 0: chartDefaultMinY0 = value; break;
                case 1: chartDefaultMinY1 = value; break;
                case 2: chartDefaultMinY2 = value; break;
                case 3: chartDefaultMinY3 = value; break;
                case 4: chartDefaultMinY4 = value; break;
            }
            return chartDefaultMaxY;
        }
        public double? SetChartDefaultMaxY(int lineIndex, double value)
        {
            switch (lineIndex)
            {
                case 0: chartDefaultMaxY0 = value; break;
                case 1: chartDefaultMaxY1 = value; break;
                case 2: chartDefaultMaxY2 = value; break;
                case 3: chartDefaultMaxY3 = value; break;
                case 4: chartDefaultMaxY4 = value; break;
            }
            return chartDefaultMaxY;
        }

        public Dictionary<string, ChartLineDefaults> chartLineDefaults = new Dictionary<string, ChartLineDefaults>()
        {
        };

        /// <summary>
        /// Returns the correct min y for a chart. If there's a specified min, use that. If there's a default min, use that iff it's less than the potential min. Otherwise, use the potential min value.
        /// </summary>
        public double ChartMinY(int lineIndex, double potentialMinY)
        {
            // This is a definitive value and overrides all others.
            if (chartMinY.HasValue)
            {
                return chartMinY.Value;
            }
            double? value = GetChartDefaultMinY(lineIndex);
            if (value.HasValue)
            {
                return Math.Min(value.Value, potentialMinY);
            }
            return potentialMinY;
        }
        /// <summary>
        /// Returns the correct max y for a chart. If there's a specified max, use that. If there's a default max, use that iff it's less than the potential max. Otherwise, use the potential max value.
        /// </summary>
        public double ChartMaxY(int lineIndex, double potentialMaxY)
        {
            if (chartMaxY.HasValue)
            {
                return chartMaxY.Value;
            }
            double? value = GetChartDefaultMaxY(lineIndex);
            if (value.HasValue)
            {
                return Math.Max(value.Value, potentialMaxY);
            }
            return potentialMaxY;
        }

        public string AsCSharpString()
        {
            var sb = new StringBuilder();
            sb.Append("{\n");
            if (!string.IsNullOrEmpty(tableType)) sb.Append($"tableType=\"{tableType}\",\n");
            if (!string.IsNullOrEmpty(chartType)) sb.Append($"chartType=\"{chartType}\",\n");
            if (!string.IsNullOrEmpty(chartCommand)) sb.Append($"chartCommand=\"{chartCommand}\",\n");

            if (chartDefaultMaxY.HasValue) sb.Append($"chartDefaultMaxY={chartDefaultMaxY.Value},\n");
            if (chartMaxY.HasValue) sb.Append($"chartMaxY={chartMaxY.Value},\n");
            if (chartDefaultMinY.HasValue) sb.Append($"chartDefaultMinY={chartDefaultMinY.Value},\n");
            if (chartMinY.HasValue) sb.Append($"chartMinY={chartMinY.Value},\n");

            if (chartLineDefaults.Count > 0)
            {
                sb.Append($"        chartLineDefaults={{\n");
                foreach (var (name, value) in chartLineDefaults)
                {
                    sb.Append($"                        {{ \"{name}\", new ChartLineDefaults() {{\n");
                    sb.Append($"                            stroke=\"{value.stroke}\",\n");
                    sb.Append($"                            }}\n");
                    sb.Append($"                        }},\n");
                }
                sb.Append($"                    }},\n");
            }
            //sb.Append($"\"REM_created\"=\"{DateTimeOffset.Now.ToString()}\"\n"); //Last line 
            sb.Append("}");
            var retval = sb.ToString();
            return retval;
        }
    }

    /// <summary>
    /// Characteristic with common data (like the name) filled in
    /// </summary>
    public class NameCharacteristic
    {
        public NameCharacteristic()
        {

        }
        public NameCharacteristic(GattCharacteristic characteristic, NameService service, NameCharacteristic defaultCharacteristic, int count = -1)
        {
            UUID = characteristic.Uuid.ToString("D");
            Name = null;

            // Priority: default characteristic, user description, unknown
            if (defaultCharacteristic != null)
            {
                Name = defaultCharacteristic.Name;
                SuppressRead = defaultCharacteristic.SuppressRead;
                SuppressWrite = defaultCharacteristic.SuppressWrite;
                Suppress = defaultCharacteristic.Suppress;
            }
            if (String.IsNullOrEmpty(Name))
            {
                Name = characteristic.UserDescription;
            }
            if (String.IsNullOrEmpty(Name))
            {
                Name = count >= 0 ? $"Unknown{count}" : "Unknown";
            }
            Description = defaultCharacteristic?.Description ?? characteristic.UserDescription;
            if (Description == "") Description = null;
            if (characteristic.UserDescription != "")
            {
                ;
            }
            if (Name.Contains("20") || Name.Contains("humi_cmd"))
            {
                ; // something for the debugger to hook to.
            }
            Type = defaultCharacteristic == null ? $"BYTES|HEX|{Name}" : defaultCharacteristic.Type;
            TypePFL.Globals = service?.ServiceTypePFL;
            if (service != null && defaultCharacteristic != null)
            {
                if (service?.ServiceTypePFL != null || defaultCharacteristic.Type.Contains("XR^EnvironmentData"))
                {
                    ; // handy place to put a debugger to verify it's being set.
                }
            }
            IsRead = characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read);
            if (SuppressRead) IsRead = false;
            IsWrite = characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write);
            if (SuppressWrite) IsWrite = false;
            IsWriteWithoutResponse = characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);
            IsNotify = characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify);
            NotifyConfigure = defaultCharacteristic == null ? null : defaultCharacteristic.NotifyConfigure;
            ReadConvert = defaultCharacteristic == null ? null : defaultCharacteristic.ReadConvert;
            NotifyConvert = defaultCharacteristic == null ? null : defaultCharacteristic.NotifyConvert;
            IsIndicate = characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate);
            UI = defaultCharacteristic == null ? null : defaultCharacteristic.UI;

        }

        [DefaultValue("")]
        public string ExtraUI { get; set; } = ""; // allowed: "" or "LightControl"
        public UISpecifications UI { get; set; } = null; // default to null
        private string UuidRaw = "";
        /// <summary>
        /// Sets the UUID as a string. Will take in short version (like 1800) but will
        /// always return full UUID values.
        /// </summary>
        public string UUID
        {
            get
            {
                return UuidRaw.AsFullUuid();
            }
            set
            {
                UuidRaw = value;
            }
        }
        public string Name { get; set; }
        public string Description { get; set; }

        public bool Suppress { get; set; } = false; // when set, the UI doesn't need to see this characteristic.

        [JsonIgnore]
        public ParserFieldList TypePFL = null;
        string _Type = null;

        /// <summary>
        /// Says how to interpret the results -- e.g. "U8|HEX|Red U8|HEX|Green U8|HEX|Blue". Can be parsed with ParserFieldList.ParseLine.
        /// </summary>
        public string Type { get { return _Type; } 
            set 
            { 
                _Type = value; 
                if (value.Contains ("XR^EnvironmentData"))
                {
                    ; // Handy place to hang a debugger
                }
                TypePFL = ParserFieldList.ParseLine(value); 
            } 
        }

        // These are used when generating C# classes
        public bool IsRead { get; set; }
        public string ReadConvert { get; set; }
        public bool IsWrite { get; set; }
        public bool SuppressRead { get; set; } = false;
        public bool SuppressWrite { get; set; } = false;
        public bool IsWriteWithoutResponse { get; set; }
        public bool IsNotify { get; set; }
        public string NotifyConfigure { get; set; }
        public string NotifyConvert { get; set; }
        /// <summary>
        /// When true, the UI will automatically call the DoNotify()
        /// </summary>
        public bool AutoNotify { get; set; } = false;
        public bool IsIndicate { get; set; }
        public string Verbs
        {
            get
            {
                var retval =  ":" +
                    (IsRead ? "Read:" : "") +
                    (IsWrite ? "Write:" : "") +
                    (IsWriteWithoutResponse ? "WriteWithoutResponse:" : "") +
                    (IsIndicate ? "Indicate:" : "") +
                    (IsNotify ? "Notify:" : "") +
                    ((IsRead || IsIndicate || IsNotify) ? "RdInNo:" : "") + // Often has the same needs in generated code
                    ((IsIndicate || IsNotify) ? "InNo:" : "") + // Often has the same needs in generated code
                    ((IsWrite || IsWriteWithoutResponse) ? "WrWw:" : "") +
                    "";
                return retval;
            }
        }


        public List<string> ExampleData { get; set; } = new List<string>();
        /// <summary>
        /// See WilliamWeilerEngineering_Skoobot for an example
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> EnumValues = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>
        /// List of commands that use this one characterisic. Is used by e.g. the Elegoo MiniCar
        /// in the UIList where there's one characteristic that uses a series of commands like {BEEP[10]}
        /// </summary>
        public Dictionary<string, Command> UIList_Commands { get; } = new Dictionary<string, Command>();
        /// <summary>
        /// Used by Elegoo_MiniCar in combination with the Commands values
        /// </summary>
        public List<SimpleUI> UIList_UI { get; } = new List<SimpleUI>();
        public override string ToString()
        {
            return $"{Name}:{UUID}";
        }
    }
}
