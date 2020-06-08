using BluetoothDeviceController.BleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Windows.ApplicationModel.Activation;

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

            var autonotify = nameDevice.HasReadDevice_Name() ? "\t\t\t\tawait DoReadDevice_Name();\n" : "";
            foreach (var service in nameDevice.Services)
            {
                foreach (var characteristic in service.Characteristics)
                {
                    if (characteristic.AutoNotify)
                    {
                        autonotify += $"\t\t\t\tawait DoNotify{characteristic.Name}();\n";
                    }
                }
            }

            var replace = new SortedDictionary<string, string>()
            {
                {"[[CLASSNAME]]", classname },
                {"[[DESCRIPTION]]", nameDevice.Description ?? ""},
                {"[[DEVICENAME]]", name.DotNetSafe() },
                {"[[DEVICENAMEUSER]]", name },
                {"[[DOREADDEVICE_NAME]]", autonotify },
                {"[[SERVICE_LIST]]",Generate_PageCSharp_SERVICE_LIST(nameDevice) },
            };
            var Retval = Replace(Generate_CSharp_Templates.PageCSharp_BodyTemplate, replace);
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
                    if (characteristic.Suppress)
                    {
                        continue;
                    }
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
                        var function = Generate_PageCSharp_function(replace, split, Generate_CSharp_Templates.PageCSharp_CharacteristicWriteTemplate, Generate_CSharp_Templates.PageCSharp_CharacteristicWrite_DataTemplates);
                        functionlist += function;
                    }
                    if (characteristic.IsRead || characteristic.IsNotify)
                    {
                        var function = Generate_PageCSharp_function(replace, split, Generate_CSharp_Templates.PageCSharp_CharacteristicRecordTemplate, Generate_CSharp_Templates.PageCSharp_CharacteristicRecord_DataTemplates);
                        functionlist += function;
                    }
                    if (characteristic.IsRead)
                    {
                        var function = Generate_PageCSharp_function(replace, split, Generate_CSharp_Templates.PageCSharp_CharacteristicReadTemplate, Generate_CSharp_Templates.PageCSharp_CharacteristicRead_DataTemplates);
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
                        var function = Generate_PageCSharp_function(replace, split, Generate_CSharp_Templates.PageCSharp_CharacteristicNotifyTemplate, Generate_CSharp_Templates.PageCSharp_CharacteristicNotify_DataTemplates);
                        functionlist += function;
                    }
                    // All of the string-oriented commands
                    if (characteristic.UIList.Count > 0)
                    {
                        var liststr = Generate_PageCSharp_UIList(characteristic, replace);
                        functionlist += liststr;
                    }
                }
                replace["[[CHARACTERISTIC_LIST]]"] = functionlist;
                serviceList += Replace(Generate_CSharp_Templates.PageCSharp_ServiceTemplate, replace);
            }
            return serviceList;
        }

        private static string Generate_PageCSharp_UIList(NameCharacteristic characteristic, SortedDictionary<string, string> replace)
        {
            var liststr = "";
            foreach (var simpleUI in characteristic.UIList)
            {
                Command target = null;
                string[] targetlist = null;
                if (!string.IsNullOrEmpty(simpleUI.Target))
                {
                    targetlist = simpleUI.Target.Split(new char[] { ' ' });
                    target = characteristic.Commands[targetlist[0]];
                    replace["[[COMMAND]]"] = DotNetSafe(targetlist[0]);
                    replace["[[LABEL]]"] = string.IsNullOrEmpty(simpleUI.Label) ? target.Label : simpleUI.Label;
                    // Is, e.g., "RGB R" -- the red parameter in the RGB command.
                    if (targetlist.Length > 1)
                    {
                        var parameter = target.Parameters[targetlist[1]];
                        replace["[[PARAM]]"] = DotNetSafe(targetlist[1]);
                        replace["[[MIN]]"] = parameter.Min.ToString();
                        replace["[[MAX]]"] = parameter.Max.ToString();
                        replace["[[INIT]]"] = parameter.Init.ToString();
                        if (!string.IsNullOrEmpty(parameter.Label))
                        {
                            replace["[[LABEL]]"] = parameter.Label;
                        }
                    }
                }
                else
                {
                    replace["[[LABEL]]"] = simpleUI.Label;
                }

                // E.G. A slider can set a value (the target like RGB R) and can also trigger a compute.
                // The compute is technically given a different name, just because we can.
                if (!string.IsNullOrEmpty (simpleUI.ComputeTarget))
                {
                    replace["[[TARGETCOMMAND]]"] = simpleUI.ComputeTarget;
                }
                else
                {
                    replace["[[SLIDERCHANGE_COMPUTETARGET]]"] = "";
                }
                switch (simpleUI.UIType)
                {
                    case "Blank":
                        break;
                    case "ButtonFor":
                        break;
                    case "SliderFor":
                        {
                            var needCompute = string.IsNullOrEmpty(simpleUI.ComputeTarget);
                            if (needCompute)
                            {
                                var ct = Replace(Generate_CSharp_Templates.PageCSharp_Characteristic_SliderChangeComputeTarget, replace);
                                replace["[[ASYNC]]"] = "async ";
                                replace["[[SLIDERCHANGE_COMPUTETARGET]]"] = ct;
                            }
                            else
                            {
                                replace["[[ASYNC]]"] = " ";
                                replace["[[SLIDERCHANGE_COMPUTETARGET]]"] = "";
                            }
                            liststr += Replace(Generate_CSharp_Templates.PageCSharp_Characteristic_SliderChange, replace);
                        }
                        break;
                    case "RowEnd":
                        break;
                    case "RowStart":
                        break;
                }

            }
            return liststr;
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
                chartSetupCommand = Generate_CSharp_Templates.PageCSharp_Characteristic_ChartSetup_Template;
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
            var Retval = Replace(Generate_CSharp_Templates.PageXaml_BodyTemplate, replace);
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
                    if (characteristic.Suppress)
                    {
                        continue;
                    }
                    replace["[[CHARACTERISTICNAME]]"] = characteristic.Name.DotNetSafe();
                    replace["[[CHARACTERISTICNAMEUSER]]"] = characteristic.Name;

                    // Need the datalist and buttonlist
                    var datalist = "";
                    var readwriteButtonList = "";
                    var enumButtonList = "";
                    var split = ValueParserSplit.ParseLine(characteristic.Type);

                    var isReadOnly = true;
                    int nbutton = 0;
                    if (characteristic.IsWrite || characteristic.IsWriteWithoutResponse)
                    {
                        replace["[[BUTTONTYPE]]"] = "Write";
                        readwriteButtonList += Replace(Generate_CSharp_Templates.PageXamlCharacteristicButtonTemplate, replace);
                        nbutton++;
                        isReadOnly = false;
                    }
                    if (characteristic.IsRead)
                    {
                        replace["[[BUTTONTYPE]]"] = "Read";
                        readwriteButtonList += Replace(Generate_CSharp_Templates.PageXamlCharacteristicButtonTemplate, replace);
                        nbutton++;
                    }
                    if (characteristic.IsNotify)
                    {
                        replace["[[BUTTONTYPE]]"] = "Notify";
                        readwriteButtonList += Replace(Generate_CSharp_Templates.PageXamlCharacteristicButtonTemplate, replace);
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
                            table = Replace(Generate_CSharp_Templates.PageXamlCharacteristicTableTemplate, replace);
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
                        datalist += Replace(Generate_CSharp_Templates.PageXamlCharacteristicDataTemplate, replace);
                    }
                    replace["[[DATA1_LIST]]"] = datalist;

                    // For the e.g. William Wiler Skoobot
                    var buttonUI = characteristic.UI?.buttonUI;
                    enumButtonList = Generate_PageXaml_UI_ButtonList(characteristic, replace);
                    replace["[[ENUM_BUTTON_LIST]]"] = enumButtonList;
                    replace["[[MAXCOLUMNS]]"] = (buttonUI==null ? 5 : buttonUI.MaxColumns).ToString();
                    replace["[[ENUM_BUTTON_LIST_PANEL]]"] = String.IsNullOrEmpty(enumButtonList)
                        ? ""
                        : Replace(Generate_CSharp_Templates.PageXamlCharacteristicEnumButtonPanelTemplate, replace);
                    ;

                    // All of the string-oriented commands
                    if (characteristic.UIList.Count > 0)
                    {
                        var liststr = Generate_PageXaml_UIList(characteristic, replace);
                        replace["[[FUNCTIONUILIST]]"] = liststr;
                        var panelstr = Replace(Generate_CSharp_Templates.PageXamlFunctionUIListPanelTemplate, replace);
                        replace["[[FUNCTIONUI_LIST_PANEL]]"] = panelstr;
                    }
                    else
                    {
                        replace["[[FUNCTIONUI_LIST_PANEL]]"] = "";
                    }


                    replace["[[READWRITE_BUTTON_LIST]]"] = readwriteButtonList;
                    replace["[[TABLE]]"] = table;
                    var c = Replace(Generate_CSharp_Templates.PageXaml_CharacteristicTemplate, replace);
                    characteristicList += c;
                }
                replace["[[CHARACTERISTIC_LIST]]"] = characteristicList;
                replace["[[SERVICENAME]]"] = service.Name.DotNetSafe();
                replace["[[SERVICENAMEUSER]]"] = service.Name;
                replace["[[SERVICEISEXPANDED]]"] = (service.Priority >= 10) ? "true" : "false";
                serviceList += Replace(Generate_CSharp_Templates.PageXaml_ServiceTemplate, replace);
            }
            return serviceList;
        }

        private static string Generate_PageXaml_UIList(NameCharacteristic characteristic, SortedDictionary<string, string> replace)
        {
            var liststr = "";
            foreach (var simpleUI in characteristic.UIList)
            {
                Command target = null;
                string[] targetlist = null;
                if (!string.IsNullOrEmpty(simpleUI.Target))
                {
                    targetlist = simpleUI.Target.Split(new char[] { ' ' });
                    target = characteristic.Commands[targetlist[0]];
                    replace["[[COMMAND]]"] = DotNetSafe(targetlist[0]);
                    replace["[[LABEL]]"] = string.IsNullOrEmpty(simpleUI.Label) ? target.Label : simpleUI.Label;
                    // Is, e.g., "RGB R" -- the red parameter in the RGB command.
                    if (targetlist.Length > 1)
                    {
                        var parameter = target.Parameters[targetlist[1]];
                        replace["[[PARAM]]"] = DotNetSafe(targetlist[1]);
                        replace["[[MIN]]"] = parameter.Min.ToString();
                        replace["[[MAX]]"] = parameter.Max.ToString();
                        replace["[[INIT]]"] = parameter.Init.ToString();
                        if (!string.IsNullOrEmpty(parameter.Label))
                        {
                            replace["[[LABEL]]"] = parameter.Label;
                        }
                    }
                }
                else
                {
                    replace["[[LABEL]]"] = simpleUI.Label;
                }
                switch (simpleUI.UIType)
                {
                    case "Blank":
                        // It doesn't take much to make a blank item.
                        liststr += "<Rectangle />\n";
                        break;
                    case "ButtonFor":
                        liststr += Replace(Generate_CSharp_Templates.PageXamlFunctionButtonTemplate, replace);
                        break;
                    case "SliderFor":
                        liststr += Replace(Generate_CSharp_Templates.PageXamlFunctionSliderTemplate, replace);
                        break;
                    case "RowEnd":
                        liststr += "</VariableSizedWrapGrid>\n";
                        break;
                    case "RowStart":
                        liststr += $"<VariableSizedWrapGrid Orientation=\"Horizontal\" MaximumRowsOrColumns=\"{simpleUI.GetN()}\">\n";
                        break;
                }

            }
            return liststr;
        }
        private static string Generate_PageXaml_UI_ButtonList(NameCharacteristic characteristic, SortedDictionary<string, string> replace)
        {
            string enumButtonList = "";
            var buttonUI = characteristic.UI?.buttonUI;
            if (characteristic.UI?.buttonType == "standard")
            {
                if (buttonUI == null)
                {
                    foreach (var (enumcommandname, enumlist) in characteristic.EnumValues)
                    {
                        foreach (var (enumname, enumvalue) in enumlist)
                        {
                            replace["[[ENUM_NAME]]"] = enumname;
                            replace["[[ENUM_VALUE]]"] = enumvalue.ToString(); // values are e.g. 17 as an int.
                            enumButtonList += Replace(Generate_CSharp_Templates.PageXamlCharacteristicEnumButtonTemplate, replace);
                        }
                    }
                }
                else
                {
                    var defaultEnum = String.IsNullOrEmpty(buttonUI.DefaultEnum)
                        ? characteristic.EnumValues.Keys.First()
                        : buttonUI.DefaultEnum;
                    foreach (var item in buttonUI.Buttons)
                    {
                        try
                        {
                            // NOTE: in the future, allow this to be set. For now, we only ever allow
                            // enums when there's a single command, so there will only ever be a single
                            // item in the EnumValues list.
                            var enumList = characteristic.EnumValues[defaultEnum];
                            switch (item.Type)
                            {
                                case ButtonPerButtonUI.ButtonType.Blank:
                                    enumButtonList += "                    <Rectangle />\n"; // have to make some kind of XAML object!
                                    break;
                                case ButtonPerButtonUI.ButtonType.Button:
                                    replace["[[ENUM_NAME]]"] = String.IsNullOrEmpty(item.Label) ? item.Enum : item.Label;
                                    replace["[[ENUM_VALUE]]"] = enumList[item.Enum].ToString(); // values are e.g. 17 as an int.
                                    enumButtonList += Replace(Generate_CSharp_Templates.PageXamlCharacteristicEnumButtonTemplate, replace);
                                    break;
                            }
                        }
                        catch (Exception)
                        {
                            enumButtonList += $"\nERROR: while processing button {item}\n";
                        }
                    }
                }
            }
            return enumButtonList;
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
            // Create the [[LINKS]]
            var links = "";
            foreach (var link in nameDevice.Links)
            {
                links += $"\t\t// Link: {link}\n";
            }
            replace.Add("[[LINKS]]", links);
            var Retval = Replace(Generate_CSharp_Templates.Protocol_BodyTemplate, replace);
            return Retval;
        }

        private static string Generate_Protocol_SERVICE_CHARACTERISTIC_LIST(NameDevice nameDevice)
        {
            var template = Generate_CSharp_Templates.Protocol_ServiceListTemplate;

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
                            Retval += Replace(Generate_CSharp_Templates.Protocol_DataPropertyTemplate, replace);
                            propertySet += Replace(Generate_CSharp_Templates.Protocol_DataPropertySetTemplate, replace);
                        }
                        replace["[[SET_PROPERTY_VALUES]]"] = propertySet;
                    }

                    if (characteristic.IsRead)
                    {
                        Retval += Replace(Generate_CSharp_Templates.Protocol_ReadMethodTemplate, replace);
                    }
                    if (characteristic.IsNotify)
                    {
                        Retval += Replace(Generate_CSharp_Templates.Protocol_NotifyMethodTemplate, replace);
                    }
                    if (characteristic.IsWrite || characteristic.IsWriteWithoutResponse)
                    {
                        // For devices with both Write and WriteWithoutResponse, I prefer
                        // using plain WriteWithoutResponse because it's faster.
                        replace["[[WRITEMODE]]"] = characteristic.IsWriteWithoutResponse 
                            ? "GattWriteOption.WriteWithoutResponse" 
                            : "GattWriteOption.WriteWithResponse";
                        Retval += Replace(Generate_CSharp_Templates.Protocol_WriteMethodTemplate, replace);
                    }
                    foreach (var (functionname, command) in characteristic.Commands)
                    {
                        // Set these up before replacing in the per-param values.
                        replace["[[FUNCTION_COMPUTE]]"] = command.Compute;
                        replace["[[FUNCTIONNAME]]"] = functionname;

                        var fparams = "";
                        var addvars = "";
                        var setvars = "";
                        var fenums = "";

                        foreach (var (paramname, param) in command.Parameters)
                        {
                            if (fparams.Length > 0) fparams += ", ";
                            replace["[[FUNCTIONPARAMNAME]]"] = paramname;

                            var enumname = "";
                            if (param.ValueNames.Count > 0)
                            {
                                var fenuminit = "";
                                foreach (var (varname, varvalue) in param.ValueNames)
                                {
                                    var varnamedn = DotNetSafe(varname);
                                    fenuminit += $"            {varnamedn} = {varvalue},\n";
                                }
                                replace["[[FUNCTION_ENUM_INITS]]"] = fenuminit.TrimEnd();
                                fenums += Replace(Generate_CSharp_Templates.Protocol_Function_Enum, replace);
                                enumname = Replace("[[CHARACTERISTICNAME]]_[[FUNCTIONNAME]]_[[FUNCTIONPARAMNAME]]", replace);
                            }

                            var issigned = param.Min < 0;
                            var sbytestr = issigned ? "s" : "";
                            var sintstr = issigned ? "" : "u";
                            if (enumname != "") fparams += enumname + " ";
                            else if (param.Max <= 255) fparams += $"{sbytestr}byte ";
                            else if (param.Max <= 65535) fparams += $"{sintstr}short ";
                            else fparams += $"{sintstr}int ";
                            fparams += paramname;

                            replace["[[FUNCTIONPARAMINIT]]"] = param.Init.ToString();
                            addvars += Replace(Generate_CSharp_Templates.Protocol_Function_AddVariableTemplate, replace);
                            setvars += Replace(Generate_CSharp_Templates.Protocol_Function_SetVariableTemplate, replace);
                        }

                        replace["[[PROTOCOL_FUNCTION_ENUMS]]"] = fenums;
                        replace["[[FUNCTION_ADDVARIABLES]]"] = addvars;
                        replace["[[FUNCTION_SETVARIABLES]]"] = setvars;
                        replace["[[FUNCTION_PARAMS]]"] = fparams;
                        Retval += Replace(Generate_CSharp_Templates.Protocol_FunctionTemplate, replace);
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

    }
}
