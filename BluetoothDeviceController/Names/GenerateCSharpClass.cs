using BluetoothDeviceController.BleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BluetoothDeviceController.Names
{
    public static class GenerateCSharpClass
    {
        public static void Error(string str)
        {
            ;
        }
        public static string DotNetSafe(this string name)
        {
            // "  foo  " --> "foo"
            // "foo bar" --> foo_bar
            // "temp (c)" --> "temp_c"
            // "temp(c)" --> "temp_c"
            name = name.Trim();
            name = name.Replace(" ", "_");
            name = name.Replace("(", "_");
            name = name.Replace(")", "");
            name = name.Replace("  ", " ");
            var sb = new StringBuilder();
            // We made all the exact fixes we want; now let's replace
            // everything else that might be a problem.
            foreach (var ch in name)
            {
                var isOk = char.IsLetterOrDigit(ch) || ch == '_';
                sb.Append(isOk ? ch : '_');
            }
            name = sb.ToString();
            name = name.Replace("__", "_"); // remove unsightly extra bits.
            name = name.Trim('_'); // remove leading and trailing underscores. Otherwise "Humidity %" --> Humidity__

            if (name.Contains ("IR_Temp_Conf")) // just a debugging hook.
            {
                ;
            }
            return name;
        }
        public static string GeneratePageCSharp(NameDevice nameDevice)
        {
            var classname = nameDevice.GetClassName().DotNetSafe();
            var name = nameDevice.Name ?? "";

            var replace = new SortedDictionary<string, string>()
            {
                {"[[CLASSNAME]]", classname },
                {"[[DESCRIPTION]]", nameDevice.Description ?? ""},
                {"[[DEVICENAME]]", name.DotNetSafe() },
                {"[[DEVICENAMEUSER]]", name },
                {"[[DOREADDEVICE_NAME]]", nameDevice.HasReadDevice_Name() ? "DoReadDevice_Name();" : "" },
                {"[[SERVICE_LIST]]",Generate_PageCSharp_SERVICE_LIST(nameDevice) },
            };
            var Retval = Replace(PageCSharp_BodyTemplate, replace);
            return Retval;
        }

        private static string Generate_PageCSharp_SERVICE_LIST(NameDevice nameDevice)
        {
            var serviceList = "";
            var replace = new SortedDictionary<string, string>()
            {

            };

            foreach (var service in nameDevice.Services)
            {
                if (service.Suppress) continue;

                var functionlist = "";
                replace["[[SERVICENAME]]"] = service.Name.DotNetSafe();
                replace["[[SERVICENAMEUSER]]"] = service.Name;

                foreach (var characteristic in service.Characteristics)
                {
                    replace["[[CHARACTERISTICNAME]]"] = characteristic.Name.DotNetSafe();
                    replace["[[CHARACTERISTICNAMEUSER]]"] = characteristic.Name;
                    replace["[[UICHARTCOMMAND]]"] = characteristic.UI?.chartCommand ?? "";
                    var dbgspec = characteristic.UI?.AsCSharpString() ?? "";
                    replace["[[UISPECS]]"] = characteristic.UI?.AsCSharpString() ?? "";
                    replace["[[READCONVERT]]"] = characteristic.ReadConvert ?? "";
                    replace["[[NOTIFYCONVERT]]"] = characteristic.NotifyConvert ?? "";
                    replace["[[NOTIFYCONFIGURE]]"] = characteristic.NotifyConfigure ?? "";

                    var split = ValueParserSplit.ParseLine(characteristic.Type);
                    if (characteristic.IsWrite || characteristic.IsWriteWithoutResponse)
                    {
                        var function = Generate_PageCSharp_function(replace, split, PageCSharp_CharacteristicWriteTemplate, PageCSharp_CharacteristicWrite_DataTemplates);
                        functionlist += function;
                    }
                    if (characteristic.IsRead || characteristic.IsNotify)
                    {
                        var function = Generate_PageCSharp_function(replace, split, PageCSharp_CharacteristicRecordTemplate, PageCSharp_CharacteristicRecord_DataTemplates);

                        // Handle the wierd case of ReadDevice_Name to support the auto-read capability.
                        if (service.Name == "Common Configuration" && characteristic.Name == "Device Name" && characteristic.IsRead)
                        {
                            // Add in the public DoReadDevice_Name function
                            function += PageCSharp_DoReadDevice_Name;
                        }

                        functionlist += function;
                    }
                    if (characteristic.IsRead)
                    {
                        var function = Generate_PageCSharp_function(replace, split, PageCSharp_CharacteristicReadTemplate, PageCSharp_CharacteristicRead_DataTemplates);
                        functionlist += function;
                    }
                    if (characteristic.IsNotify)
                    {
                        replace["[[NOTIFYVALUELIST]]"] = "";
                        if (characteristic.IsNotify)
                        {
                            // Act like there might be more than two possible entries into the notify array.
                            // Maybe at some point I'll also do an indicate?
                            // NOTE: maybe also have a live indicator on the button?
                            replace["[[NOTIFYVALUELIST]]"] += "            GattClientCharacteristicConfigurationDescriptorValue.Notify,\n";
                        }
                        var function = Generate_PageCSharp_function(replace, split, PageCSharp_CharacteristicNotifyTemplate, PageCSharp_CharacteristicNotify_DataTemplates);
                        functionlist += function;
                    }
                }
                replace["[[CHARACTERISTIC_LIST]]"] = functionlist;
                serviceList += Replace(PageCSharp_ServiceTemplate, replace);
            }
            return serviceList;
        }

        private static string ByteFormatToCSharp(string format)
        {
            switch (format)
            {
                case "F32": return "Single";
                case "F64": return "Double";
                case "U8": return "Byte";
                case "U16": return "UInt16";
                case "U32": return "UInt32";
                case "I8": return "SByte";
                case "I16": return "Int16";
                case "I24": return "Int32";
                case "I32": return "Int32";
                case "BYTES": return "Bytes";
                case "STRING": return "String";
            }
            if (format.StartsWith ("/") || format.StartsWith("Q"))
            {
                return "double";
            }
            Error($"ByteFormatToCSharp: unknown format {format}");
            return $"OtherType{format}";
        }
        private static string ByteFormatToCSharpDefault(string format)
        {
            switch (format)
            {
                case "F32": return "0.0";
                case "F64": return "0.0";
                case "U8": return "0";
                case "U16": return "0";
                case "U24": return "0";
                case "U32": return "0";
                case "I8": return "0";
                case "I16": return "0";
                case "I24": return "0";
                case "I32": return "0";
                case "BYTES": return "null";
                case "STRING": return "\"\"";
            }
            if (format.StartsWith("/") || format.StartsWith("Q"))
            {
                return "0.0";
            }
            Error($"ByteFormatToCSharpDefault: unknown format {format}");
            return $"OtherType{format}";
        }

        /// <summary>
        /// Converts a byte format (I8, /P8/U8, etc) into either AsString or AsDouble (i.e., BC BASIC types)
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        private static string ByteFormatToCSharpAsDouble(string format)
        {
            switch (format)
            {
                case "BYTES": return "AsString";
                case "STRING": return "AsString";
            }
            return $"AsDouble";
        }

        private static string ByteFormatToCSharpStringOrDouble(string format)
        {
            switch (format)
            {
                case "BYTES": return "string";
                case "STRING": return "string";
            }
            return $"double";
        }
        private static string Generate_PageCSharp_function(SortedDictionary<string, string> replace, IList<ValueParserSplit> split, string characteristicTemplate, string[] dataTemplates)
        {
            string[] datalist = new string[dataTemplates.Length];
            for (int ii=0; ii<dataTemplates.Length; ii++)
            {
                datalist[ii] = "";
            }
            var propertyTemplate = @"var [[DATANAME]]Property = typeof([[CHARACTERISTICNAME]]Record).GetProperty(""[[DATANAME]]"");\n";
            replace["[[DATANAME]]"] = "EventTime";
            var propertylist = Replace (propertyTemplate, replace);
            var arglist = "";
            for (int i = 0; i < split.Count; i++)
            {
                var item = split[i];
                var dataname = item.NamePrimary;
                // Special case: items whose type starts with O (like OEL or OEB for little and big endian)
                // should be skipped
                if (item.ByteFormatPrimary.StartsWith('O')) continue; // skip OEL and OEB (little and big endian indicators) and all other options

                if (dataname == "") dataname = $"param{i}";
                replace["[[DATANAME]]"] = dataname.DotNetSafe();
                replace["[[DATANAMEUSER]]"] = dataname.Replace ("_", " ");
                replace["[[VARIABLETYPE]]"] = ByteFormatToCSharp(item.ByteFormatPrimary);
                replace["[[VARIABLETYPE_DS]]"] = ByteFormatToCSharpStringOrDouble(item.ByteFormatPrimary);
                replace["[[AS_DOUBLE_OR_STRING]]"] = ByteFormatToCSharpAsDouble(item.ByteFormatPrimary); // e.g.  ".AsDouble";
                switch (item.DisplayFormatPrimary)
                {
                    case "DEC":
                        replace["[[DEC_OR_HEX]]"] = "System.Globalization.NumberStyles.None";
                        break;
                    case "HEX":
                        replace["[[DEC_OR_HEX]]"] = "System.Globalization.NumberStyles.AllowHexSpecifier";
                        break;
                    default:
                        replace["[[DEC_OR_HEX]]"] = "System.Globalization.NumberStyles.AllowHexSpecifier";
                        break;
                }
                for (int ii = 0; ii < dataTemplates.Length; ii++)
                {
                    datalist[ii] += Replace(dataTemplates[ii], replace);
                }
                propertylist += Replace(propertyTemplate, replace);

                if (arglist != "") arglist += ", ";
                arglist += dataname;
            }
            replace["[[ARG_LIST]]"] = arglist;
            for (int ii = 0; ii < dataTemplates.Length; ii++)
            {
                replace[$"[[DATA{ii+1}_LIST]]"] = datalist[ii];
            }
            replace["[[PROPERTY_LIST]]"] = propertylist;
            string chartCommand = "";
            string chartUpdateCommand = "";
            string chartSetupCommand = "";
            var haveChartCommand = replace.TryGetValue("[[UICHARTCOMMAND]]", out chartCommand);
            if (haveChartCommand && string.IsNullOrWhiteSpace(chartCommand)) haveChartCommand = false; // only a real chart command counts!
            if (haveChartCommand)
            {
                chartCommand = "[[CHARACTERISTICNAME]]Chart." + chartCommand + ";";
                chartUpdateCommand = "[[CHARACTERISTICNAME]]Chart.RedrawYTime<[[CHARACTERISTICNAME]]Record>([[CHARACTERISTICNAME]]RecordData);";
                // Note: this is starting to become unweildy :-(
                if (chartCommand.Contains ("AddLineYTime"))
                {
                    chartUpdateCommand = chartUpdateCommand.Replace("RedrawYTime", "RedrawLineYTime");
                }
                chartSetupCommand = PageCSharp_Characteristic_ChartSetup_Template;
            }
            replace["[[CHART_COMMAND]]"] = Replace (chartCommand, replace).Replace("\\n", "\n"); // e.g. [[CHARACTERISTICNAME]]Chart." + chartcommand; // SetCursorPosition(-N2, Angle);
            replace["[[CHART_UPDATE_COMMAND]]"] = Replace (chartUpdateCommand, replace);
            replace["[[CHART_SETUP]]"] = Replace(chartSetupCommand, replace);
            var retval = Replace(characteristicTemplate, replace);
            return retval;
        }




        public static string GeneratePageXaml(NameDevice nameDevice)
        {
            var classname = nameDevice.GetClassName().DotNetSafe();
            var name = nameDevice.Name ?? "";

            var replace = new SortedDictionary<string, string>()
            {
                {"[[CLASSNAME]]", classname },
                {"[[DESCRIPTION]]", nameDevice.Description ?? ""},
                {"[[DEVICENAME]]", name.DotNetSafe() },
                {"[[DEVICENAMEUSER]]", name },
                {"[[SERVICE_LIST]]",Generate_PageXaml_SERVICE_LIST(nameDevice) },
            };
            var Retval = Replace(PageXaml_BodyTemplate, replace);
            return Retval;
        }

        private static string Generate_PageXaml_SERVICE_LIST(NameDevice nameDevice)
        {
            var serviceList = "";
            var replace = new SortedDictionary<string, string>()
            {

            };
            var sortedServices = nameDevice.Services.OrderByDescending(x => x.Priority).ToList();

            foreach (var service in sortedServices)
            {
                if (service.Suppress) continue;

                var characteristicList = "";
                foreach (var characteristic in service.Characteristics)
                {
                    replace["[[CHARACTERISTICNAME]]"] = characteristic.Name.DotNetSafe();
                    replace["[[CHARACTERISTICNAMEUSER]]"] = characteristic.Name;

                    // Need the datalist and buttonlist
                    var datalist = "";
                    var buttonlist = "";
                    var split = ValueParserSplit.ParseLine(characteristic.Type);

                    var isReadOnly = true;
                    int nbutton = 0;
                    if (characteristic.IsWrite || characteristic.IsWriteWithoutResponse)
                    {
                        replace["[[BUTTONTYPE]]"] = "Write";
                        buttonlist += Replace(PageXamlCharacteristicButtonTemplate, replace);
                        nbutton++;
                        isReadOnly = false;
                    }
                    if (characteristic.IsRead)
                    {
                        replace["[[BUTTONTYPE]]"] = "Read";
                        buttonlist += Replace(PageXamlCharacteristicButtonTemplate, replace);
                        nbutton++;
                    }
                    if (characteristic.IsNotify)
                    {
                        replace["[[BUTTONTYPE]]"] = "Notify";
                        buttonlist += Replace(PageXamlCharacteristicButtonTemplate, replace);
                        nbutton++;
                    }
                    var table = ""; // default is no table
                    if (characteristic.IsRead || characteristic.IsNotify)
                    {
                        var xamlChart = "";
                        if (!String.IsNullOrEmpty(characteristic.UI?.chartType))
                        {
                            xamlChart = Replace (@"<charts:ChartControl Height=""200"" Width=""500"" x:Name=""[[CHARACTERISTICNAME]]Chart"" />", replace);
                        }
                        replace["[[XAMLCHART]]"] = xamlChart;
                        if (!String.IsNullOrEmpty (characteristic.UI?.tableType))
                        {
                            table = Replace(PageXamlCharacteristicTableTemplate, replace);
                        }
                    }
                    const int maxItemsPerRow = 6;
                    // Correctly calculate the number of buttons and fields.
                    int nitems = 0;
                    for (int i = 0; i < split.Count; i++)
                    {
                        var item = split[i];
                        if (item.ByteFormatPrimary.StartsWith('O')) continue; // skip OEL and OEB (little and big endian indicators)
                        nitems++;
                    }
                    nitems += nbutton;
                    int requiredRows = (int)Math.Ceiling((double)nitems / (double)maxItemsPerRow);
                    int splitCount = (int)Math.Ceiling((double)nitems / (double)requiredRows);
                    // The idea here is to split the datalist every so often. The result should
                    // be pleasing -- so that if there are a total of 7 items, and I only allow
                    // up to 6 items total, there must be at least 2 rows.
                    // If there are 2 rows, then there should be a split every ceil(7/2)==4 items
                    // so the last row has whatever orphans are left.
                    // I want to split at 4, 8, 12, etc.
                    // i % splitCount == 0 && i>0

                    for (int i=0; i<split.Count; i++)
                    {
                        var item = split[i];
                        if (item.ByteFormatPrimary.StartsWith('O')) continue; // skip OEL and OEB (little and big endian indicators)

                        replace["[[IS_READ_ONLY]]"] = isReadOnly ? "True" : "False";
                        var dataname = item.NamePrimary;
                        if (string.IsNullOrEmpty(dataname)) dataname = $"param{i}";
                        replace["[[DATANAME]]"] = dataname.DotNetSafe();
                        replace["[[DATANAMEUSER]]"] = dataname.Replace("_", " ");

                        bool shouldSplit = (i % splitCount == 0 && i > 0);
                        if (shouldSplit) datalist += @"</StackPanel><StackPanel Orientation=""Horizontal"">"+"\n";
                        datalist += Replace(PageXamlCharacteristicDataTemplate, replace);
                    }
                    replace["[[DATA1_LIST]]"] = datalist;
                    replace["[[BUTTON_LIST]]"] = buttonlist;
                    replace["[[TABLE]]"] = table;
                    var c = Replace(PageXaml_CharacteristicTemplate, replace);
                    characteristicList += c;
                }
                replace["[[CHARACTERISTIC_LIST]]"] = characteristicList;
                replace["[[SERVICENAME]]"] = service.Name.DotNetSafe();
                replace["[[SERVICENAMEUSER]]"] = service.Name;
                replace["[[SERVICEISEXPANDED]]"] = (service.Priority >= 10) ? "true" : "false";
                serviceList += Replace(PageXaml_ServiceTemplate, replace);
            }
            return serviceList;
        }


        public static string GenerateProtocol (NameDevice nameDevice)
        {
            var classname = nameDevice.GetClassName().DotNetSafe();
            var classmodifiers = nameDevice.ClassModifiers;
            var name = nameDevice.Name ?? "";
            var now = DateTime.Now;

            var replace = new SortedDictionary<string, string>()
            {
                {"[[CLASSMODIFIERS]]", classmodifiers ?? "" },
                {"[[CLASSNAME]]", classname },
                {"[[METHOD_LIST]]", Generate_Protocol_METHOD_LIST(nameDevice) },
                {"[[NAME]]", name.DotNetSafe() },
                {"[[DEVICENAME]]", name.DotNetSafe() },
                {"[[DESCRIPTION]]", nameDevice.Description ?? "" },
                {"[[CURRTIME]]", $"{now.ToShortDateString()} {now.ToShortTimeString()}" },
                {"[[SERVICE_CHARACTERISTIC_LIST]]",Generate_Protocol_SERVICE_CHARACTERISTIC_LIST(nameDevice) },
            };
            var Retval = Replace(Protocol_BodyTemplate, replace);
            return Retval;
        }

        private static string Generate_Protocol_SERVICE_CHARACTERISTIC_LIST(NameDevice nameDevice)
        {
            var template = Protocol_ServiceListTemplate;

            var serviceGuidList = "";
            var serviceNameList = "";
            var serviceList = "";

            var characteristicGuidList = "";
            var characteristicNameList = "";
            var characteristicList = "";

            var hashList = "";

            int characteristicIndex = 0;
            foreach (var service in nameDevice.Services)
            {
                if (service.Suppress) continue; // seems to be needed for Protocentral Sensything for generic service

                serviceGuidList += $"            Guid.Parse(\"{service.UUID}\"),\n";
                serviceNameList += $"            \"{service.Name}\",\n";
                serviceList += $"            null,\n";

                int startingCharacteristicIndex = characteristicIndex;
                for (var i=0; i<service.Characteristics.Count; i++)
                {
                    var characteristic = service.Characteristics[i];
                    characteristicGuidList += $"            Guid.Parse(\"{characteristic.UUID}\"), // #{i} is {characteristic.Name}\n";
                    characteristicNameList += $"            \"{characteristic.Name}\", // #{i} is {characteristic.UUID}\n";
                    characteristicList += $"            null,\n";

                    characteristicIndex++;
                }

                var indexlist = "";
                for (int i=startingCharacteristicIndex; i<characteristicIndex; i++)
                {
                    indexlist += $"{i}, ";
                }
                hashList += $"            new HashSet<int>(){{ {indexlist} }},\n";
            }

            template = template.Replace("[[SERVICE_GUID_LIST]]", serviceGuidList);
            template = template.Replace("[[SERVICE_NAME_LIST]]", serviceNameList);
            template = template.Replace("[[SERVICE_LIST]]", serviceList);
            template = template.Replace("[[CHARACTERISTIC_GUID_LIST]]", characteristicGuidList);
            template = template.Replace("[[CHARACTERISTIC_NAME_LIST]]", characteristicNameList);
            template = template.Replace("[[CHARACTERISTIC_LIST]]", characteristicList);
            template = template.Replace("[[HASH_LIST]]", hashList);

            return template;
        }


        private static string Generate_Protocol_METHOD_LIST(NameDevice nameDevice)
        {
            var Retval = "";
            int characteristicIndex = 0;
            foreach (var service in nameDevice.Services)
            {
                if (service.Suppress) continue;

                foreach (var characteristic in service.Characteristics)
                {
                    var paramlist = "";
                    var arglist = "";
                    var vpwlist = ValueParserSplit.ParseLine(characteristic.Type);
                    int paramCount = 0;
                    foreach (var param in vpwlist)
                    {
                        var name = param.NamePrimary.DotNetSafe();
                        if (name == "") name = $"param{paramCount}";
                        if (paramlist != "") paramlist += ", ";
                        var format = param.ByteFormatPrimary;
                        if (format == "") format = "BYTES";
                        switch (format)
                        {
                            case "BYTES":
                                paramlist += $"byte[] {name}";
                                arglist += $"            dw.WriteBytes({name});\n";
                                break;
                            case "STRING":
                                paramlist += $"String {name}";
                                arglist += $"            dw.WriteString({name});\n";
                                break;
                            case "I8":
                                paramlist += $"sbyte {name}";
                                arglist += $"            dw.WriteByte((byte){name});\n";
                                break;
                            case "U8":
                                paramlist += $"byte {name}";
                                arglist += $"            dw.WriteByte({name});\n";
                                break;
                            case "I16":
                                paramlist += $"Int16 {name}";
                                arglist += $"            dw.WriteInt16({name});\n";
                                break;
                            case "U16":
                                paramlist += $"UInt16 {name}";
                                arglist += $"            dw.WriteUInt16({name});\n";
                                break;
                            case "I32":
                                paramlist += $"Int32 {name}";
                                arglist += $"            dw.WriteInt32({name});\n";
                                break;
                            case "U32":
                                paramlist += $"UInt32 {name}";
                                arglist += $"            dw.WriteUInt32({name});\n";
                                break;
                            default:
                                if (format[0] == 'O')
                                {
                                    ; // Do nothing for options like OEL or OEB (option endian big and option endian little)
                                }
                                else
                                {
                                    Error($"GenerateCShartClass: unknown type {format} / {name}");

                                    paramlist += $"UNKNOWN_TYPE_{format} {name}";
                                    arglist += $"            dw.WriteUNKNOWN_TYPE_{format}({name});\n";
                                }
                                break;
                        }
                        paramCount++;
                    }
                    var replace = new SortedDictionary<string, string>()
                    {
                        { "[[ARGS]]", arglist },
                        { "[[CHARACTERISTICINDEX]]", characteristicIndex.ToString() }
,                       { "[[CHARACTERISTICTYPE]]", characteristic.Type }, // e.g. the "U8|HEX|Red U8|HEX|Green" string
                        { "[[CHARACTERISTICNAME]]", characteristic.Name.DotNetSafe() },
                        { "[[NAME]]", characteristic.Name.DotNetSafe() }, //NOTE: not needed??
                        { "[[PARAMS]]", paramlist },
                    };
                    bool hasProperty = characteristic.IsRead || characteristic.IsNotify;
                    var propertySet = "";
                    if (hasProperty)
                    {
                        var split = ValueParserSplit.ParseLine(characteristic.Type);

                        // Properties are per-data which is finer grained than just per-characteristic.
                        for (int i = 0; i < split.Count; i++)
                        {
                            var item = split[i];
                            var dataname = item.NamePrimary;
                            if (dataname == "") dataname = $"param{i}";
                            if (item.ByteFormatPrimary.StartsWith('O')) continue; // skip OEL and OEB (little and big endian indicators)

                            // CHDATANAME is either char_data (to make it more unique) or just plain characteristicname.
                            // If a charateristic has just a single data item, that one item doesn't need a seperate name here.
                            // so characteristic "temparature" with a single data value "temperature" will get a chdataname
                            // of "temperature". If there were two data value (temp and humidity) they would get unique names
                            // (temperature_temp and temperature_humidity)
                            replace["[[CHDATANAME]]"] = split.Count == 1 
                                ? characteristic.Name.DotNetSafe() 
                                : characteristic.Name.DotNetSafe()+ "_" + dataname.DotNetSafe();
                            replace["[[DATANAME]]"] = dataname.DotNetSafe();
                            replace["[[DATANAMEUSER]]"] = dataname.Replace("_", " ");
                            replace["[[VARIABLETYPE]]"] = ByteFormatToCSharp(item.ByteFormatPrimary);
                            replace["[[VARIABLETYPE_DS]]"] = ByteFormatToCSharpStringOrDouble(item.ByteFormatPrimary);
                            replace["[[AS_DOUBLE_OR_STRING]]"] = ByteFormatToCSharpAsDouble(item.ByteFormatPrimary); // e.g.  ".AsDouble";
                            replace["[[DOUBLE_OR_STRING_DEFAULT]]"] = ByteFormatToCSharpDefault(item.ByteFormatPrimary);
                            Retval += Replace(Protocol_DataPropertyTemplate, replace);
                            propertySet += Replace(Protocol_DataPropertySetTemplate, replace);
                        }
                        replace["[[SET_PROPERTY_VALUES]]"] = propertySet;
                    }

                    if (characteristic.IsRead)
                    {
                        Retval += Replace(Protocol_ReadMethodTemplate, replace);
                    }
                    if (characteristic.IsNotify)
                    {
                        Retval += Replace(Protocol_NotifyMethodTemplate, replace);
                    }
                    if (characteristic.IsWrite || characteristic.IsWriteWithoutResponse)
                    {
                        // For devices with both Write and WriteWithoutResponse, I prefer
                        // using plain WriteWithoutResponse because it's faster.
                        replace["[[WRITEMODE]]"] = characteristic.IsWriteWithoutResponse 
                            ? "GattWriteOption.WriteWithoutResponse" 
                            : "GattWriteOption.WriteWithResponse";
                        Retval += Replace(Protocol_WriteMethodTemplate, replace);
                    }


                    characteristicIndex++;
                }
            }
            return Retval;
        }

        private static string Replace(string template, SortedDictionary<string, string> replace)
        {
            if (template == null) return "";
            var Retval = template;
            foreach (var item in replace)
            {
                Retval = Retval.Replace(item.Key, item.Value);
            }
            return Retval;
        }

        private static string Protocol_BodyTemplate = @"using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothProtocols
{
    /// <summary>
    /// [[DESCRIPTION]].
    /// This class was automatically generated [[CURRTIME]]
    /// </summary>

    public [[CLASSMODIFIERS]] class [[CLASSNAME]] : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // [[LINKS]]TODO: create LINKS

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
[[SERVICE_CHARACTERISTIC_LIST]]

        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;

            GattCharacteristicsResult lastResult = null;
            if (forceReread) 
            {
                readCharacteristics = false;
            }
            if (readCharacteristics == false)
            {
                for (int serviceIndex = 0; serviceIndex < MapServiceToCharacteristic.Count; serviceIndex++)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($""Unable to get service {ServiceNames[serviceIndex]}"", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($""Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}"", serviceStatus);
                        continue; //return false;
                    }
                    var service = serviceStatus.Services[0];
                    var characteristicIndexSet = MapServiceToCharacteristic[serviceIndex];
                    foreach (var characteristicIndex in characteristicIndexSet)
                    {
                        var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[characteristicIndex]);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            Status.ReportStatus($""unable to get characteristic for {CharacteristicNames[characteristicIndex]}"", characteristicsStatus);
                            return false;
                        }
                        if (characteristicsStatus.Characteristics.Count == 0)
                        {
                            Status.ReportStatus($""unable to get any characteristics for {CharacteristicNames[characteristicIndex]}"", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else if (characteristicsStatus.Characteristics.Count != 1)
                        {
                            Status.ReportStatus($""unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[characteristicIndex]}"", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else
                        {
                            Characteristics[characteristicIndex] = characteristicsStatus.Characteristics[0];
                            lastResult = characteristicsStatus;
                        }
                        lastResult = characteristicsStatus;
                    }
                }
                // Do not call ReportStatus on OK -- the actual read/write/etc. call will
                // call ReportStatus for them. It's important that for any one actual call
                // (public method) that there's only one ReportStatus.
                //Status.ReportStatus(""OK: Connected to device"", lastResult);
                readCharacteristics = true;
            }
            return readCharacteristics;
        }

        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name=""method"" ></param>
        /// <param name=""command"" ></param>
        /// <returns></returns>
        private async Task WriteCommandAsync(int characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
            }
            catch (Exception)
            {
                result = GattCommunicationStatus.Unreachable;
            }
            Status.ReportStatus(method, result);
            if (result != GattCommunicationStatus.Success)
            {
                // NOTE: should add a way to reset
            }
        }
        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name=""characteristicIndex"">Index number of the characteristic</param>
        /// <param name=""method"" >Name of the actual method; is just used for logging</param>
        /// <param name=""cacheMode"" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(int characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[characteristicIndex].ReadValueAsync(cacheMode);
                if (readResult.Status == GattCommunicationStatus.Success)
                {
                    buffer = readResult.Value;
                }
                else
                {
                    // NOTE: reset the characteristics array?
                }
                Status.ReportStatus(method, readResult.Status);
            }
            catch (Exception)
            {
                Status.ReportStatus(method, GattCommunicationStatus.Unreachable);
                // NOTE: reset the characteristics array?
            }
            return buffer;
        }

        /// <summary>
        /// Delegate for all Notify events
        /// </summary>
        /// <param name=""data""></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);



        [[METHOD_LIST]]
    }
}
";

        private static string Protocol_ServiceListTemplate = @"
        Guid[] ServiceGuids = new Guid[] {
[[SERVICE_GUID_LIST]]
        };
        String[] ServiceNames = new string[] {
[[SERVICE_NAME_LIST]]
        };
        GattDeviceService[] Services = new GattDeviceService[] {
[[SERVICE_LIST]]
        };
        Guid[] CharacteristicGuids = new Guid[] {
[[CHARACTERISTIC_GUID_LIST]]
        };
        String[] CharacteristicNames = new string[] {
[[CHARACTERISTIC_NAME_LIST]]
        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
[[CHARACTERISTIC_LIST]]
        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
[[HASH_LIST]]
        };
";

        private static string Protocol_NotifyMethodTemplate = @"
        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; [[CHARACTERISTICNAME]]Event += _my function_
        /// </summary>
        public event BluetoothDataEvent [[CHARACTERISTICNAME]]Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>
        
        private bool Notify[[CHARACTERISTICNAME]]_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name=""notifyType""></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> Notify[[CHARACTERISTICNAME]]Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[[[CHARACTERISTICINDEX]]];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!Notify[[CHARACTERISTICNAME]]_ValueChanged_Set)
                {
                    // Only set the event callback once
                    Notify[[CHARACTERISTICNAME]]_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = ""[[CHARACTERISTICTYPE]]"";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
[[SET_PROPERTY_VALUES]]
                        [[CHARACTERISTICNAME]]Event?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($""Notify[[CHARACTERISTICNAME]]: {e.Message}"", result);
                return false;
            }
            Status.ReportStatus($""Notify[[CHARACTERISTICNAME]]: set notification"", result);

            return true;
        }
";

        private static string Protocol_ReadMethodTemplate = @"
        /// <summary>
        /// Reads data 
        /// </summary>
        /// <param name=""cacheMode"">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> Read[[CHARACTERISTICNAME]](BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync()) return null;
            IBuffer result = await ReadAsync([[CHARACTERISTICINDEX]], ""[[CHARACTERISTICNAME]]"", cacheMode);
            if (result == null) return null;

            var datameaning = ""[[CHARACTERISTICTYPE]]"";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
[[SET_PROPERTY_VALUES]]
            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue(""LightRaw"").AsDouble;
            return parseResult.ValueList;
        }
";

        private static string Protocol_WriteMethodTemplate = @"
        /// <summary>
        /// Writes data for [[CHARACTERISTICNAME]]
        /// </summary>
        /// <param name=""Period""></param>
        /// <returns></returns>
        public async Task Write[[CHARACTERISTICNAME]]([[PARAMS]])
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
[[ARGS]]
            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync([[CHARACTERISTICINDEX]], ""[[CHARACTERISTICNAME]]"", command, [[WRITEMODE]]);
        }
";

        private static string Protocol_DataPropertySetTemplate = @"
            [[CHDATANAME]] = parseResult.ValueList.GetValue(""[[DATANAME]]"").[[AS_DOUBLE_OR_STRING]];
";

        private static string Protocol_DataPropertyTemplate = @"
        private [[VARIABLETYPE_DS]] _[[CHDATANAME]] = [[DOUBLE_OR_STRING_DEFAULT]];
        private bool _[[CHDATANAME]]_set = false;
        public [[VARIABLETYPE_DS]] [[CHDATANAME]]
        {
            get { return _[[CHDATANAME]]; }
            internal set { if (_[[CHDATANAME]]_set && value == _[[CHDATANAME]]) return; _[[CHDATANAME]] = value; _[[CHDATANAME]]_set = true; OnPropertyChanged(); }
        }
";

        private static string PageXaml_BodyTemplate = @"
<Page
    x:Class=""BluetoothDeviceController.SpecialtyPages.[[CLASSNAME]]Page""
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:local=""using:BluetoothDeviceController.SpecialtyPages""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls""
    xmlns:charts=""using:BluetoothDeviceController.Charts""
    xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006""
    mc:Ignorable=""d""
    Background=""{ThemeResource ApplicationPageBackgroundThemeBrush}"">
    <Page.Resources>
        <Style TargetType=""Button"">
            <Setter Property=""VerticalAlignment"" Value=""Bottom"" />
            <Setter Property=""Margin"" Value=""10,0,0,0"" />
        </Style>
        <Style TargetType=""Line"">
            <Setter Property=""Margin"" Value=""0,15,0,0"" />
            <Setter Property=""Stroke"" Value=""ForestGreen"" />
        </Style>
        <Style x:Key=""TitleStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""30"" />
        </Style>
        <Style x:Key=""HeaderStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""20"" />
        </Style>
        <Style x:Key=""HeaderStyleExpander"" TargetType=""controls:Expander"">
            <Setter Property=""MinWidth"" Value=""550"" />
            <Setter Property=""HorizontalAlignment"" Value=""Left"" />
            <Setter Property=""HorizontalContentAlignment"" Value=""Left"" />
        </Style>
        <Style x:Key=""SubheaderStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""16"" />
        </Style>
        <Style x:Key=""AboutStyle"" TargetType=""TextBlock"">
            <Setter Property=""FontSize"" Value=""12"" />
            <Setter Property=""TextWrapping"" Value=""Wrap"" />
        </Style>
        <Style x:Key=""ChacteristicListStyle"" TargetType=""StackPanel"">
            <Setter Property=""Background"" Value=""WhiteSmoke"" />
            <Setter Property=""Margin"" Value=""18,0,0,0"" />
        </Style>
        <Style x:Key=""HEXStyle"" TargetType=""TextBox"">
            <Setter Property=""MinWidth"" Value=""90"" />
            <Setter Property=""FontSize"" Value=""12"" />
            <Setter Property=""Margin"" Value=""5,0,0,0"" />
        </Style>
        <Style x:Key=""TableStyle"" TargetType=""controls:DataGrid"">
            <Setter Property = ""Background"" Value=""BlanchedAlmond"" />
            <Setter Property = ""FontSize"" Value=""12"" />
            <Setter Property = ""Height"" Value=""200"" />
            <Setter Property = ""HorizontalAlignment"" Value=""Center"" />
            <Setter Property = ""Width"" Value=""500"" />
        </Style>
    </Page.Resources>
    
    <StackPanel>
        <Grid MaxWidth=""550"" HorizontalAlignment=""Left"">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width=""*"" />
                <ColumnDefinition Width=""auto"" />
            </Grid.ColumnDefinitions>
            <StackPanel Grid.Column=""0"">
                <TextBlock Style=""{StaticResource TitleStyle}"">[[DEVICENAMEUSER]] device</TextBlock>
                <TextBlock Style=""{StaticResource AboutStyle}"">
                    [[DESCRIPTION]]
                </TextBlock>
            </StackPanel>
            <controls:ImageEx Grid.Column=""1"" Style=""{StaticResource ImageStyle}""  Source=""/Assets/DevicePictures/[[CLASSNAME]].PNG"" />
        </Grid>
        <ProgressRing x:Name=""uiProgress"" />
        <TextBlock x:Name=""uiStatus"" />
[[SERVICE_LIST]]
        <Button Content=""REREAD"" Click=""OnRereadDevice"" />
    </StackPanel>
</Page>
";

        private static string PageXaml_ServiceTemplate = @"
        <controls:Expander Header=""[[SERVICENAMEUSER]]"" IsExpanded=""[[SERVICEISEXPANDED]]"" Style=""{StaticResource HeaderStyleExpander}"">
            <StackPanel Style=""{StaticResource ChacteristicListStyle}"">
[[CHARACTERISTIC_LIST]]
            </StackPanel>
        </controls:Expander>
";

        private static string PageXaml_CharacteristicTemplate = @"
                <TextBlock Style=""{StaticResource SubheaderStyle}"">[[CHARACTERISTICNAMEUSER]]</TextBlock>
                <StackPanel Orientation=""Horizontal"">
[[DATA1_LIST]]
[[BUTTON_LIST]]
                </StackPanel>
[[TABLE]]
";
        private static string PageXamlCharacteristicDataTemplate = @"
                    <TextBox IsReadOnly=""[[IS_READ_ONLY]]"" x:Name=""[[CHARACTERISTICNAME]]_[[DATANAME]]"" Text=""*"" Header=""[[DATANAMEUSER]]"" Style=""{StaticResource HEXStyle}""/>
";
        private static string PageXamlCharacteristicButtonTemplate = @"
                    <Button Content=""[[BUTTONTYPE]]"" Click=""On[[BUTTONTYPE]][[CHARACTERISTICNAME]]"" />
";

        private static string PageXamlCharacteristicTableTemplate = @"
                    <controls:Expander Header=""[[CHARACTERISTICNAMEUSER]] Data tracker"" IsExpanded=""false"" MinWidth=""550"" HorizontalAlignment=""Left"">
                        <StackPanel MinWidth=""550"">
                            [[XAMLCHART]]
                            <controls:DataGrid Style=""{StaticResource TableStyle}"" x:Name=""[[CHARACTERISTICNAME]]Grid"" ItemsSource=""{Binding [[CHARACTERISTICNAME]]RecordData}"" />
                            <TextBox  x:Name=""[[CHARACTERISTICNAME]]_Notebox"" KeyDown=""On[[CHARACTERISTICNAME]]_NoteKeyDown"" />
                            <StackPanel Orientation=""Horizontal"" HorizontalAlignment=""Left"">
                                <ComboBox SelectionChanged=""OnKeepCount[[CHARACTERISTICNAME]]"" Header=""Keep how many items?"" SelectedIndex=""2"">
                                    <ComboBoxItem Tag=""10"">10</ComboBoxItem>
                                    <ComboBoxItem Tag=""100"">100</ComboBoxItem>
                                    <ComboBoxItem Tag=""1000"">1,000</ComboBoxItem>
                                    <ComboBoxItem Tag=""10000"">10K</ComboBoxItem>
                                </ComboBox>
                                <Rectangle Width=""5"" />
                                <ComboBox SelectionChanged=""OnAlgorithm[[CHARACTERISTICNAME]]"" Header=""Remove algorithm?"" SelectedIndex=""0"">
                                    <ComboBoxItem Tag=""1"" ToolTipService.ToolTip=""Keep a random sample of data"">Keep random sample</ComboBoxItem>
                                    <ComboBoxItem Tag=""0"" ToolTipService.ToolTip=""Keep the most recent data"">Keep latest data</ComboBoxItem>
                                </ComboBox>
                                <Button Content = ""Copy"" Click=""OnCopy[[CHARACTERISTICNAME]]"" />
                            </StackPanel>
                        </StackPanel>
                    </controls:Expander>
";
        private static string PageCSharp_BodyTemplate =
@"using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Names;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[NAME]] device
    /// </summary>
    public sealed partial class [[CLASSNAME]]Page : Page, HasId, ISetHandleStatus
    {
        public [[CLASSNAME]]Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = ""[[CLASSNAME]]"";
        private string DeviceNameUser = ""[[DEVICENAMEUSER]]"";

        int ncommand = 0;
        [[CLASSNAME]] bleDevice = new [[CLASSNAME]]();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive (true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive (false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            [[DOREADDEVICE_NAME]]
        }

        public string GetId()
        {
            return bleDevice?.ble?.DeviceId ?? """";
        }

        public string GetPicturePath()
        {
            return $""/Assets/DevicePictures/{DeviceName}-175.PNG"";
        }

        public string GetDeviceNameUser()
        {
            return $""{DeviceNameUser}"";
        }

        private IHandleStatus ParentStatusHandler = null;

        public void SetHandleStatus(IHandleStatus handleStatus)
        {
            ParentStatusHandler = handleStatus;
        }

        private void SetStatus(string status)
        {
            uiStatus.Text = status;
            ParentStatusHandler?.SetStatusText(status);
        }
        private void SetStatusActive (bool isActive)
        {
            uiProgress.IsActive = isActive;
            ParentStatusHandler?.SetStatusActive(isActive);
        }

        private async void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $""{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}"";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                SetStatus(nowstr + "": "" + status.AsStatusString);
                SetStatusActive (false);
            });
        }
[[SERVICE_LIST]]
        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus(""Reading device"");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(true);
            SetStatusActive(false);
        }
    }
}
";
        private static string PageCSharp_ServiceTemplate = @"
// Functions for [[SERVICENAMEUSER]]

[[CHARACTERISTIC_LIST]]
";

        private static string PageCSharp_CharacteristicRecordTemplate = @"
        public class [[CHARACTERISTICNAME]]Record: INotifyPropertyChanged
        {
            public [[CHARACTERISTICNAME]]Record()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }
[[DATA1_LIST]]
            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }
    
    public DataCollection<[[CHARACTERISTICNAME]]Record> [[CHARACTERISTICNAME]]RecordData { get; } = new DataCollection<[[CHARACTERISTICNAME]]Record>();
    private void On[[CHARACTERISTICNAME]]_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = """";
            // Add the text to the notes section
            if ([[CHARACTERISTICNAME]]RecordData.Count == 0)
            {
                [[CHARACTERISTICNAME]]RecordData.AddRecord(new [[CHARACTERISTICNAME]]Record());
            }
            [[CHARACTERISTICNAME]]RecordData[[[CHARACTERISTICNAME]]RecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCount[[CHARACTERISTICNAME]](object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        [[CHARACTERISTICNAME]]RecordData.MaxLength = value;

        [[CHART_UPDATE_COMMAND]]
    }

    private void OnAlgorithm[[CHARACTERISTICNAME]](object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        [[CHARACTERISTICNAME]]RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopy[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append(""EventDate,EventTime[[DATA2_LIST]],Notes\n"");
        foreach (var row in [[CHARACTERISTICNAME]]RecordData)
        {
            var time24 = row.EventTime.ToString(""HH:mm:ss.f"");
            sb.Append($""{row.EventTime.ToShortDateString()},{time24}[[DATA3_LIST]],{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n"");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }
";
        private static string[] PageCSharp_CharacteristicRecord_DataTemplates =
        {
@"
            private [[VARIABLETYPE_DS]] _[[DATANAME]];
            public [[VARIABLETYPE_DS]] [[DATANAME]] { get { return _[[DATANAME]]; } set { if (value == _[[DATANAME]]) return; _[[DATANAME]] = value; OnPropertyChanged(); } }
", // DATA1_LIST
@",[[DATANAME]]", // DATA2_LIST
@",{row.[[DATANAME]]}", // DATA3_LIST
@"
typeof([[CHARACTERISTICNAME]]Record).GetProperty(""[[DATANAME]]""),", // DATA4_LIST
@"
""[[DATANAME]]"",", // DATA5_LIST
};


        private static string PageCSharp_CharacteristicReadTemplate = @"
        private async void OnRead[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.Read[[CHARACTERISTICNAME]]();
                if (valueList == null)
                {
                    SetStatus ($""Error: unable to read [[CHARACTERISTICNAME]]"");
                    return;
                }
                [[READCONVERT]]
                var record = new [[CHARACTERISTICNAME]]Record();
[[DATA1_LIST]]

                [[CHARACTERISTICNAME]]RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($""Error: exception: {ex.Message}"");
            }
        }
";
        private static string[] PageCSharp_CharacteristicRead_DataTemplates = new string[] { @"
                var [[DATANAME]] = valueList.GetValue(""[[DATANAME]]"");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE_DS]])[[DATANAME]].[[AS_DOUBLE_OR_STRING]];
                    [[CHARACTERISTICNAME]]_[[DATANAME]].Text = record.[[DATANAME]].ToString(); // ""N0""); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
" };

        //TODO: fix the NOTE: about popping up a dialog box
        private static string PageCSharp_CharacteristicWriteTemplate = @"
        private async void OnWrite[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;
[[DATA1_LIST]]
                if (parseError == null)
                {
                    await bleDevice.Write[[CHARACTERISTICNAME]]([[ARG_LIST]]);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($""Error: could not parse {parseError}"");
                }
            }
            catch (Exception ex)
            {
                SetStatus($""Error: exception: {ex.Message}"");
            }
        }
";
        private static string[] PageCSharp_CharacteristicWrite_DataTemplates = new string[] { @"
                [[VARIABLETYPE]] [[DATANAME]];
                var parsed[[DATANAME]] = Utilities.Parsers.TryParse[[VARIABLETYPE]]([[CHARACTERISTICNAME]]_[[DATANAME]].Text, [[DEC_OR_HEX]], null, out [[DATANAME]]);
                if (!parsed[[DATANAME]])
                {
                    parseError = ""[[DATANAMEUSER]]"";
                }
"};

        private static string PageCSharp_CharacteristicNotifyTemplate = @"
        GattClientCharacteristicConfigurationDescriptorValue[] Notify[[CHARACTERISTICNAME]]Settings = {
[[NOTIFYVALUELIST]]
            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int [[CHARACTERISTICNAME]]NotifyIndex = 0;
        bool [[CHARACTERISTICNAME]]NotifySetup = false;
        private async void OnNotify[[CHARACTERISTICNAME]](object sender, RoutedEventArgs e)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (![[CHARACTERISTICNAME]]NotifySetup)
                {
                    [[CHARACTERISTICNAME]]NotifySetup = true;
                    bleDevice.[[CHARACTERISTICNAME]]Event += BleDevice_[[CHARACTERISTICNAME]]Event;
                }
                var notifyType = Notify[[CHARACTERISTICNAME]]Settings[[[CHARACTERISTICNAME]]NotifyIndex];
                [[CHARACTERISTICNAME]]NotifyIndex = ([[CHARACTERISTICNAME]]NotifyIndex + 1) % Notify[[CHARACTERISTICNAME]]Settings.Length;
                var result = await bleDevice.Notify[[CHARACTERISTICNAME]]Async(notifyType);
                [[NOTIFYCONFIGURE]]

[[CHART_SETUP]]
            }
            catch (Exception ex)
            {
                SetStatus($""Error: exception: {ex.Message}"");
            }
        }

        private async void BleDevice_[[CHARACTERISTICNAME]]Event(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                [[NOTIFYCONVERT]]
                var record = new [[CHARACTERISTICNAME]]Record();
[[DATA1_LIST]]
                var addResult = [[CHARACTERISTICNAME]]RecordData.AddRecord(record);
                [[CHART_COMMAND]]
                });
            }
        }";

        private static string PageCSharp_Characteristic_ChartSetup_Template = @"
                var EventTimeProperty = typeof([[CHARACTERISTICNAME]]Record).GetProperty(""EventTime"");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {[[DATA4_LIST]]
                };
                var names = new List<string>()
                {[[DATA5_LIST]]
                };
                [[CHARACTERISTICNAME]]Chart.SetDataProperties(properties, EventTimeProperty, names);
                [[CHARACTERISTICNAME]]Chart.SetTitle(""[[CHARACTERISTICNAMEUSER]] Chart"");
                [[CHARACTERISTICNAME]]Chart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
[[UISPECS]]
;
";

        private static string[] PageCSharp_CharacteristicNotify_DataTemplates = new string[] { @"
                var [[DATANAME]] = valueList.GetValue(""[[DATANAME]]"");
                if ([[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsDouble || [[DATANAME]].CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.[[DATANAME]] = ([[VARIABLETYPE_DS]])[[DATANAME]].[[AS_DOUBLE_OR_STRING]];
                    [[CHARACTERISTICNAME]]_[[DATANAME]].Text = record.[[DATANAME]].ToString(); // ""N0""); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
" };

        private static string PageCSharp_DoReadDevice_Name = @"
        public void DoReadDevice_Name()
        {
            OnReadDevice_Name(null, null);
        }
";
    }
}
