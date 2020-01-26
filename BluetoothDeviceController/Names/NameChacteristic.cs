using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothDeviceController.Names
{
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
        /// Allowed values: "" and "standard"
        /// </summary>
        public string buttonType { get; set; } = "";
        public string tableType { get; set; } = "";
        public string chartType { get; set; } = "";
        public string chartCommand { get; set; } = "";

        public double? chartDefaultMaxY { get; set; } = null;
        public double? chartMaxY { get; set; } = null;
        public double? chartDefaultMinY { get; set; } = null;
        public double? chartMinY { get; set; } = null;
        public Dictionary<string, ChartLineDefaults> chartLineDefaults = new Dictionary<string, ChartLineDefaults>()
        {
        };

        /// <summary>
        /// Returns the correct min y for a chart. If there's a specified min, use that. If there's a default min, use that iff it's less than the potential min. Otherwise, use the potential min value.
        /// </summary>
        public double ChartMinY(double potentialMinY)
        {
            if (chartMinY.HasValue)
            {
                return chartMinY.Value;
            }
            if (chartDefaultMinY.HasValue)
            {
                return Math.Min(chartDefaultMinY.Value, potentialMinY);
            }
            return potentialMinY;
        }
        /// <summary>
        /// Returns the correct max y for a chart. If there's a specified max, use that. If there's a default max, use that iff it's less than the potential max. Otherwise, use the potential max value.
        /// </summary>
        public double ChartMaxY(double potentialMaxY)
        {
            if (chartMaxY.HasValue)
            {
                return chartMaxY.Value;
            }
            if (chartDefaultMaxY.HasValue)
            {
                return Math.Max(chartDefaultMaxY.Value, potentialMaxY);
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
        /// <summary>
        /// Says how to interpret the results -- e.g. "U8|HEX|Red U8|HEX|Green U8|HEX|Blue". Can be parsed with ValueParserSplit.
        /// </summary>
        public string Type { get; set; }

        // These are used when generating C# classes
        public bool IsRead { get; set; }
        public string ReadConvert { get; set; }
        public bool IsWrite { get; set; }
        public bool SuppressRead { get; set; }
        public bool SuppressWrite { get; set; }
        public bool IsWriteWithoutResponse { get; set; }
        public bool IsNotify { get; set; }
        public string NotifyConfigure { get; set; }
        public string NotifyConvert { get; set; }
        public bool IsIndicate { get; set; }


        public List<string> ExampleData { get; set; } = new List<string>();
        /// <summary>
        /// See WilliamWeilerEngineering_Skoobot for an example
        /// </summary>
        public Dictionary<string, Dictionary<string, int>> EnumValues = new Dictionary<string, Dictionary<string, int>>();
        public override string ToString()
        {
            return $"{Name}:{UUID}";
        }
    }
}
