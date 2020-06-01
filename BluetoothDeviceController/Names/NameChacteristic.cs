using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothDeviceController.Names
{
    public class SimpleUI
    {
        public string UIType { get; set; }
        public string Target { get; set; }
        public string ComputeTarget { get; set; }
        public string PlaceAt { get; set; }
        public string Label { get; set; }
        public List<string> Set { get; } = new List<string>();
        public double N { get; set; } = double.NaN;
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
        public static Windows.UI.Color ConvertColor(string color)
        {
            foreach (var item in typeof(Windows.UI.Colors).GetProperties())
            {
                if (color == item.Name)
                {
                    return (Windows.UI.Color)item.GetValue(null, null);
                    ;
                }
            }
            return Windows.UI.Colors.Aquamarine;
        }
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
        public string tableType { get; set; } = "";
        public string chartType { get; set; } = "";
        public string chartCommand { get; set; } = "";

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
            if (!string.IsNullOrEmpty(chartType)) sb.Append($"chartType=\"{tableType}\",\n");
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

    public class NameCharacteristic
    {
        public NameCharacteristic()
        {

        }
        public NameCharacteristic(GattCharacteristic characteristic, NameCharacteristic defaultCharacteristic, int count = -1)
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
            Type = defaultCharacteristic == null ? $"BYTES|HEX|{Name}" : defaultCharacteristic.Type;
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

        public UISpecifications UI { get; set; } = null; // default to null
        public string UUID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Suppress { get; set; } = false; // when set, the UI doesn't need to see this characteristic.
        /// <summary>
        /// Says how to interpret the results -- e.g. "U8|HEX|Red U8|HEX|Green U8|HEX|Blue". Can be parsed with ValueParserSplit.
        /// </summary>
        public string Type { get; set; }

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


        public List<string> ExampleData { get; set; } = new List<string>();
        /// <summary>
        /// See WilliamWeilerEngineering_Skoobot for an example
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> EnumValues = new Dictionary<string, Dictionary<string, int>>();

        /// <summary>
        /// List of commands that use this one characterisic. Is used by e.g. the Elegoo MiniCar
        /// where there's one characteristic that uses a series of commands like {BEEP[10]}
        /// </summary>
        public Dictionary<string, Command> Commands { get; } = new Dictionary<string, Command>();
        public List<SimpleUI> UIList { get; } = new List<SimpleUI>();
        public override string ToString()
        {
            return $"{Name}:{UUID}";
        }
    }
}
