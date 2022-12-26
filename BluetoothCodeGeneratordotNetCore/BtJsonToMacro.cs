using BluetoothDeviceController.BleEditor;
using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateExpander;
using Windows.ApplicationModel.Chat;

namespace BluetoothCodeGenerator
{
    internal static class BtJsonToMacro
    {
        static private bool ItemIsSuppressed(ValueParserSplit item)
        {
            bool retval = false;
            if (item.ByteFormatPrimary.StartsWith('O')) retval = true; // skip OEL and OEB (little and big endian indicators)
            if (item.NamePrimary.EndsWith("Unused") || item.NamePrimary.EndsWith("Internal")) retval = true;
            return retval;
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

            if (name.Contains("IR_Temp_Conf")) // just a debugging hook.
            {
                ;
            }
            return name;
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
            if (format.StartsWith("/") || format.StartsWith("Q"))
            {
                return "double";
            }
            Error($"ByteFormatToCSharp: unknown format={format} expected e.g. F32 or BYTES or STRING");
            return $"OtherType{format}";
        }

        private static string ByteFormatToCSharpParam(string format)
        {
            switch (format)
            {
                case "":
                case "BYTES": return $"byte[]";
                case "STRING": return $"String";
                case "I8": return $"sbyte"; 
                case "U8": return $"byte";
                case "I16": return $"Int16";
                case "U16": return $"UInt16";
                case "I32": return $"Int32";
                case "U32": return $"UInt32";
                case "F32": return $"float";
            }
            return $"UNKNOWN_TYPE_{format}";
        }

        private static string ByteFormatToDataWriterCall(string format)
        {
            switch (format)
            {
                case "":
                case "BYTES": return "dw.WriteBytes";
                case "STRING": return "dw.WriteString";
                case "I8": return "dw.WriteByte";
                case "U8": return "dw.WriteByte";
                case "I16": return "dw.WriteInt16";
                case "U16": return "dw.WriteUInt16";
                case "I32": return "dw.WriteInt32";
                case "U32": return "dw.WriteUInt32";
                case "F32": return "dw.WriteSingle";
            }
            return $"dw.WriteUNKNOWN_TYPE_{format}";
        }
        private static string ByteFormatToDataWriterCallCast(string format)
        {
            switch (format)
            {
                case "I8": return "(byte)";
            }
            return $"";
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
        private static void Error(string text)
        {
            Console.WriteLine(text);
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

        private static TemplateSnippet ServiceToTemplateSnippet(NameService btService)
        {
            var service = new TemplateSnippet(btService.Name);
            service.Macros.Add("Name", btService.Name); // [[SERVICENAMEUSER]
            service.Macros.Add("Name.dotNet", btService.Name.DotNetSafe()); // [[SERVICENAME]]
            service.Macros.Add("ServiceName", btService.Name); // Preferred name to "Name"
            service.Macros.Add("SERVICENAME", btService.Name.DotNetSafe()); // BAD: [[SERVICENAME]] in TI 1350 and 2541 for NOTIFYCONFIGURE
            service.Macros.Add("ServiceIsExpanded", (btService.Priority >= 10) ? "true" : "false");
            service.Macros.Add("UUID", btService.UUID);

            var chs = new TemplateSnippet("Characteristics");
            service.AddChild("Characteristics", chs);

            foreach (var btCharacteristic in btService.Characteristics)
            {
                if (btCharacteristic.Suppress) continue;

                var ch = new TemplateSnippet(btCharacteristic.Name);
                ch.Macros.Add("UUID", btCharacteristic.UUID);
                ch.Macros.Add("Name", btCharacteristic.Name);
                ch.Macros.Add("CharacteristicName", btCharacteristic.Name);
                ch.Macros.Add("CharacteristicName.dotNet", btCharacteristic.Name.DotNetSafe());
                ch.Macros.Add("Name.dotNet", btCharacteristic.Name.DotNetSafe());

                ch.Macros.Add("Type", btCharacteristic.Type);
                ch.Macros.Add("CHARACTERISTICTYPE", btCharacteristic.Type);
                ch.Macros.Add("Verbs", btCharacteristic.Verbs);
                ch.Macros.Add("TableType", btCharacteristic.UI?.tableType ?? "");
                ch.Macros.Add("AutoNotify", btCharacteristic.AutoNotify ? "true" : "false");
                ch.Macros.Add("UICHARTCOMMAND", btCharacteristic.UI?.chartCommand ?? "");

                var UI_AsCSharp = btCharacteristic.UI?.AsCSharpString() ?? "";
                ch.Macros.Add("UISPECS", UI_AsCSharp);
                if (UI_AsCSharp.Contains ("[[CHARACTERISTICNAME]]"))
                {
                    // Just a little bit of weirdness here :-)
                    ch.Macros.Add("CHARACTERISTICNAME", btCharacteristic.Name.DotNetSafe());
                }
                ch.Macros.Add("READCONVERT", btCharacteristic.ReadConvert ?? "");
                ch.Macros.Add("NOTIFYCONVERT", btCharacteristic.NotifyConvert ?? "");
                ch.Macros.Add("NOTIFYCONFIGURE", btCharacteristic.NotifyConfigure ?? "");

                var isReadOnly = true;

                // For devices with both Write and WriteWithoutResponse, I prefer
                // using plain WriteWithoutResponse because it's faster.
                // Only set the macro for writable ones.
                if (btCharacteristic.IsWrite || btCharacteristic.IsWriteWithoutResponse)
                {
                    ch.Macros.Add("WRITEMODE", btCharacteristic.IsWriteWithoutResponse
                        ? "GattWriteOption.WriteWithoutResponse"
                        : "GattWriteOption.WriteWithResponse");
                    isReadOnly = false;
                }

                chs.AddChildViaMacro(ch); // uses the UUID to add correctly
                ch.Macros.Add("IS+READ+ONLY", isReadOnly ? "True" : "False");
                AddVerbButtons(ch, btCharacteristic);

                AddButtonUIAndEnum(ch, btCharacteristic);
                AddUIList(ch, btCharacteristic);
                AddPropertiesPerCharacteristic(ch, btCharacteristic, isReadOnly);
                AddCommands(ch, btCharacteristic);
            }
            return service;
        }



        private static void AddCommands(TemplateSnippet ch, NameCharacteristic btCharacteristic)
        {
            // Commands
            var cmds = new TemplateSnippet("Commands");
            ch.AddChild("Commands", cmds); // always add, even if it's got nothing in it.
            foreach (var (dataname, command) in btCharacteristic.Commands)
            {
                var pr = new TemplateSnippet(dataname);
                cmds.AddChild(dataname, pr);

                pr.AddMacro("FUNCTIONNAME", dataname);
                pr.AddMacro("Name.dotNet", dataname.DotNetSafe());
                pr.AddMacro("Label", command.Label);
                pr.AddMacro("Alt", command.Alt); // e.g., "Obstacle Avoidance Mode"
                pr.AddMacro("Compute", command.Compute); // e.g., "${OA[?]}"

                var prms = new TemplateSnippet("Parameters");
                pr.AddChild("Parameters", prms);
                foreach (var (pname, param) in command.Parameters)
                {
                    var paramts = new TemplateSnippet(pname);
                    prms.AddChild(pname, paramts);
                    var name = param.Name;
                    if (string.IsNullOrWhiteSpace(name)) name = pname;

                    paramts.AddMacro("Name", name);
                    paramts.AddMacro("Name.dotNet", name.DotNetSafe());
                    paramts.AddMacro("FUNCTIONPARAMINIT", param.Init.ToString());

                    var variableType = "";
                    var issigned = param.Min < 0;
                    var sbytestr = issigned ? "s" : "";
                    var sintstr = issigned ? "" : "u";
                    if (param.ValueNames.Count > 0) variableType = "Command_[[FUNCTIONNAME]]_[[Name.dotNet]]";
                    else if (param.Max <= 255) variableType = $"{sbytestr}byte";
                    else if (param.Max <= 65535) variableType = $"{sintstr}short";
                    else variableType = $"{sintstr}int";
                    paramts.AddMacro("VARIABLETYPE", variableType);


                    // All of the "Enums" a.k.a. ValueNames
                    var vns = new TemplateSnippet("ValueNames");
                    paramts.AddChild("ValueNames", vns);
                    foreach (var (vname, vvalue) in param.ValueNames)
                    {
                        vns.AddMacro(vname.DotNetSafe(), vvalue.ToString()); // should be simple, not the complexity of the first attempt.
                    }
                }
            }

        }

        private static void AddButtonUIAndEnum(TemplateSnippet ch, NameCharacteristic btCharacteristic)
        {
            //
            // Use EnumValues + ButtonUI for simple UI (e.g., Skoobot). See also the UIList + Commands used by the Elegoo MiniCar.                
            // The robot commands are e.g. Left, Stop, Right, etc.
            // 
            var buttonSource = "None"; // or might be ButtonUI
            var commandEnums = new TemplateSnippet("Enums");
            ch.AddChild("Enums", commandEnums); // always add, even if it's got nothing in it.
            if (btCharacteristic.EnumValues.Count > 0)
            {
                buttonSource = "Enums";
            }
            foreach (var (enumcommandname, enumlist) in btCharacteristic.EnumValues)
            {
                // NOTE: technically this is wrong; there could be any number of Enum values
                foreach (var (enumname, enumvalue) in enumlist)
                {
                    var singleEnum = new TemplateSnippet(enumname);
                    commandEnums.AddChild(enumname, singleEnum);
                    singleEnum.Macros.Add("EnumCommandName", enumcommandname); // Maybe instead have a hierarchy?
                    singleEnum.Macros.Add("EnumName", enumname);
                    singleEnum.Macros.Add("EnumValue", enumvalue.ToString());
                }
            }

            //
            // All of the UI elements. Used by the Skoobot for the different
            // robot commands (Left, Stop, Right, etc.)
            //

            var uiCustom = new TemplateSnippet("ButtonUI");
            ch.AddChild("ButtonUI", uiCustom);
            var ui = btCharacteristic.UI;
            ch.AddMacro("buttonType", ui?.buttonType ?? "");
            ch.AddMacro("buttonMaxColumns", (ui?.buttonUI?.MaxColumns ?? 5).ToString());
            if (ui != null)
            {
                if (ui.buttonUI != null && ui.buttonUI.Buttons.Count > 0) // if buttonUI is null, we make mediocre buttons.
                {
                    buttonSource = "ButtonUI";

                    // Get the enum translations from the ../Enums macros
                    var enumSource = ui.buttonUI.DefaultEnum;
                    var enumList = commandEnums;

                    int index = 0;
                    foreach (var button in ui.buttonUI.Buttons)
                    {
                        var enumname = button.Label; //  "?enumname?button?";
                        if (string.IsNullOrEmpty(enumname)) enumname = $"button{index}";
                        var singleEnum = new TemplateSnippet(enumname);
                        uiCustom.AddChild(enumname, singleEnum);

                        // Find the enum value
                        var enumValue = "";
                        foreach (var (name, macros) in enumList.Children) // n**2 FTW!
                        {
                            if (macros.Macros["EnumCommandName"] == enumSource
                                && macros.Macros["EnumName"] == button.Enum)
                            {
                                enumValue = macros.Macros["EnumValue"];
                            }
                        }

                        singleEnum.Macros.Add("ButtonEnum", button.Enum);
                        singleEnum.Macros.Add("ButtonEnumValue", enumValue);
                        singleEnum.Macros.Add("ButtonLabel", button.Label);
                        singleEnum.Macros.Add("ButtonType", button.Type.ToString()); // "Blank" or "Button"
                        index++;
                    }
                }
            }
            ch.AddMacro("ButtonSource", buttonSource); // None Enums or ButtonUI
        }

        /// <summary>
        // All of the properties (args) for a characteristic. 
        // Source=Services/Characteristics/Properties or /ReadProperties or /WriteProperties

        /// </summary>
        /// <param name="ch"></param>
        /// <param name="btCharacteristic"></param>
        /// <param name="isReadOnly"></param>
        private static void AddPropertiesPerCharacteristic(TemplateSnippet ch, NameCharacteristic btCharacteristic, bool isReadOnly)
        {

            var allprs = new TemplateSnippet("Properties"); // Properties aka args 
            ch.AddChild("Properties", allprs); // always add, even if it's got nothing in it.

            var readprs = new TemplateSnippet("ReadProperties"); // Properties aka args -- e.g., ColorR, ColorG, ColorB
            ch.AddChild("ReadProperties", readprs); // always add, even if it's got nothing in it.
            bool hasRead = btCharacteristic.IsRead || btCharacteristic.IsNotify || btCharacteristic.IsIndicate;

            var writeprs = new TemplateSnippet("WriteProperties"); // Properties aka args 
            ch.AddChild("WriteProperties", writeprs); // always add, even if it's got nothing in it.
            bool hasWrite = btCharacteristic.IsWrite || btCharacteristic.IsWriteWithoutResponse;


            var split = ValueParserSplit.ParseLine(btCharacteristic.Type);

            // Properties are per-data which is finer grained than just per-characteristic.
            var isFirstProperty = true;
            for (int i = 0; i < split.Count; i++)
            {
                var item = split[i];
                var dataname = item.NamePrimary;
                if (dataname == "") dataname = $"param{i}";
                if (ItemIsSuppressed(item)) continue; // skip OEL and OEB (little and big endian indicators)


                // CHDATANAME is either char_data (to make it more unique) or just plain characteristicname.
                // If a charateristic has just a single data item, that one item doesn't need a seperate name here.
                // so characteristic "temparature" with a single data value "temperature" will get a chdataname
                // of "temperature". If there were two data value (temp and humidity) they would get unique names
                // (temperature_temp and temperature_humidity)
                var name = btCharacteristic.Name;
                var displayFormat = "System.Globalization.NumberStyles.None";
                // DataToString.dotNet
                var dotNetDisplayFormat = "ToString()";
                var isDouble = ByteFormatToCSharpAsDouble(item.ByteFormatPrimary) == "AsDouble";
                switch (item.DisplayFormatPrimary)
                {
                    case "DEC":
                        displayFormat = "System.Globalization.NumberStyles.None";
                        if (isDouble) dotNetDisplayFormat = "ToString(\"N0\")";
                        break;
                    case "HEX":
                        displayFormat = "System.Globalization.NumberStyles.AllowHexSpecifier";
                        if (isDouble) dotNetDisplayFormat = "ToString(\"N0\")";
                        break;
                    case "FIXED":
                        displayFormat = "System.Globalization.NumberStyles.AllowHexSpecifier";
                        if (isDouble) dotNetDisplayFormat = "ToString(\"F3\")";
                        break;
                    default:
                        displayFormat = "System.Globalization.NumberStyles.AllowHexSpecifier";
                        if (isDouble) dotNetDisplayFormat = "ToString()";
                        break;
                }

                if (hasRead)
                {
                    var datareadpr = new TemplateSnippet(dataname);
                    readprs.AddChild(dataname, datareadpr);

                    datareadpr.AddMacro("NAME", name);
                    datareadpr.AddMacro("CHDATANAME", split.Count == 1
                        ? btCharacteristic.Name.DotNetSafe()
                        : btCharacteristic.Name.DotNetSafe() + "_" + dataname.DotNetSafe());
                    datareadpr.AddMacro("DATANAME", dataname.DotNetSafe());
                    datareadpr.AddMacro("DataName", dataname);
                    datareadpr.AddMacro("DataName.dotNet", dataname.DotNetSafe());
                    datareadpr.AddMacro("IS+READ+ONLY", isReadOnly ? "True" : "False");
                    datareadpr.AddMacro("DATANAMEUSER", dataname.Replace("_", " "));
                    datareadpr.AddMacro("VARIABLETYPE", ByteFormatToCSharp(item.ByteFormatPrimary));
                    datareadpr.AddMacro("VARIABLETYPEPARAM", ByteFormatToCSharpParam(item.ByteFormatPrimary));
                    datareadpr.AddMacro("VARIABLETYPE+DS", ByteFormatToCSharpStringOrDouble(item.ByteFormatPrimary));
                    datareadpr.AddMacro("ARGDWCALL", ByteFormatToDataWriterCall(item.ByteFormatPrimary));
                    datareadpr.AddMacro("ARGDWCALLCAST", ByteFormatToDataWriterCallCast(item.ByteFormatPrimary));
                    datareadpr.AddMacro("AS+DOUBLE+OR+STRING", ByteFormatToCSharpAsDouble(item.ByteFormatPrimary)); // e.g.  ".AsDouble";
                    datareadpr.AddMacro("DOUBLE+OR+STRING+DEFAULT", ByteFormatToCSharpDefault(item.ByteFormatPrimary));
                    datareadpr.AddMacro("DEC+OR+HEX", displayFormat);
                    datareadpr.AddMacro("DataToString.dotNet", dotNetDisplayFormat);
                }
                if (hasWrite)
                {
                    var datawritepr = new TemplateSnippet(dataname);
                    writeprs.AddChild(dataname, datawritepr);

                    // CHDATANAME is either char_data (to make it more unique) or just plain characteristicname.
                    // If a charateristic has just a single data item, that one item doesn't need a seperate name here.
                    // so characteristic "temparature" with a single data value "temperature" will get a chdataname
                    // of "temperature". If there were two data value (temp and humidity) they would get unique names
                    // (temperature_temp and temperature_humidity)
                    datawritepr.AddMacro("NAME", name);
                    datawritepr.AddMacro("CHDATANAME", split.Count == 1
                        ? btCharacteristic.Name.DotNetSafe()
                        : btCharacteristic.Name.DotNetSafe() + "_" + dataname.DotNetSafe());
                    datawritepr.AddMacro("DATANAME", dataname.DotNetSafe());
                    datawritepr.AddMacro("DATANAMEUSER", dataname.Replace("_", " "));
                    datawritepr.AddMacro("DataName", dataname);
                    datawritepr.AddMacro("DataName.dotNet", dataname.DotNetSafe());
                    datawritepr.AddMacro("VARIABLETYPE", ByteFormatToCSharp(item.ByteFormatPrimary));
                    datawritepr.AddMacro("VARIABLETYPEPARAM", ByteFormatToCSharpParam(item.ByteFormatPrimary));
                    datawritepr.AddMacro("VARIABLETYPE+DS", ByteFormatToCSharpStringOrDouble(item.ByteFormatPrimary));
                    datawritepr.AddMacro("ARGDWCALL", ByteFormatToDataWriterCall(item.ByteFormatPrimary));
                    datawritepr.AddMacro("ARGDWCALLCAST", ByteFormatToDataWriterCallCast(item.ByteFormatPrimary));
                    datawritepr.AddMacro("AS+DOUBLE+OR+STRING", ByteFormatToCSharpAsDouble(item.ByteFormatPrimary)); // e.g.  ".AsDouble";
                    datawritepr.AddMacro("DOUBLE+OR+STRING+DEFAULT", ByteFormatToCSharpDefault(item.ByteFormatPrimary));
                    datawritepr.AddMacro("DEC+OR+HEX", displayFormat);
                    datawritepr.AddMacro("DataToString.dotNet", dotNetDisplayFormat);
                }

                if (true) // always do this
                {
                    var dataallpr = new TemplateSnippet(dataname);
                    allprs.AddChild(dataname, dataallpr);

                    dataallpr.AddMacro("NAME", name);
                    dataallpr.AddMacro("CHDATANAME", split.Count == 1
                        ? btCharacteristic.Name.DotNetSafe()
                        : btCharacteristic.Name.DotNetSafe() + "_" + dataname.DotNetSafe());
                    dataallpr.AddMacro("DATANAME", dataname.DotNetSafe());
                    dataallpr.AddMacro("DataName", dataname);
                    dataallpr.AddMacro("DataName.dotNet", dataname.DotNetSafe());
                    dataallpr.AddMacro("IS+READ+ONLY", isReadOnly ? "True" : "False");
                    dataallpr.AddMacro("DATANAMEUSER", dataname.Replace("_", " "));
                    dataallpr.AddMacro("VARIABLETYPE", ByteFormatToCSharp(item.ByteFormatPrimary));
                    dataallpr.AddMacro("VARIABLETYPEPARAM", ByteFormatToCSharpParam(item.ByteFormatPrimary));
                    dataallpr.AddMacro("VARIABLETYPE+DS", ByteFormatToCSharpStringOrDouble(item.ByteFormatPrimary));
                    dataallpr.AddMacro("ARGDWCALL", ByteFormatToDataWriterCall(item.ByteFormatPrimary));
                    dataallpr.AddMacro("ARGDWCALLCAST", ByteFormatToDataWriterCallCast(item.ByteFormatPrimary));
                    dataallpr.AddMacro("AS+DOUBLE+OR+STRING", ByteFormatToCSharpAsDouble(item.ByteFormatPrimary)); // e.g.  ".AsDouble";
                    dataallpr.AddMacro("DOUBLE+OR+STRING+DEFAULT", ByteFormatToCSharpDefault(item.ByteFormatPrimary));

                    dataallpr.AddMacro("DEC+OR+HEX", displayFormat);
                    dataallpr.AddMacro("DataToString.dotNet", dotNetDisplayFormat);

                    // Bad hack: the first item for write is also added to the characteristic
                    // This is needed for the write which should sweep up the different text boxes, but doesn't.
                    // Can't just be based on i==0 because the first property might be e.g. OOPT and be suppressed.
                    if (isFirstProperty)
                    {
                        ch.AddMacro("DataName.First.dotNetSafe", btCharacteristic.Name.DotNetSafe() + "_" + dataname.DotNetSafe());
                        ch.AddMacro("DEC+OR+HEX", displayFormat);
                    }
                }
                isFirstProperty = false;
            }
            readprs = null;
        }

        /// <summary>
        /// Adds everything from the UIList list. Note that this is different from the Commands + Enums.
        /// Used by the Elegoo where there needs to be e.g. a slider which sets a variable and a button which
        /// sends a commands which is parameterized by the variable. Super complex, and might not be worth the effort.
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="btCharacteristic"></param>
        private static void AddUIList(TemplateSnippet ch, NameCharacteristic btCharacteristic)
        {
            //
            // Use EnumValues + ButtonUI for simple UI (e.g., Skoobot). See also the UIList + Commands used by the Elegoo MiniCar.                
            // The robot commands are e.g. Left, Stop, Right, etc.
            // 
            var uiList = new TemplateSnippet("UIList");
            ch.AddChild("UIList", uiList); // always add, even if it's got nothing in it.
            var nui = 0;
            foreach (var simpleUI in btCharacteristic.UIList)
            {
                Command cmd = null;
                VariableDescription cmdsub = null;
                string cmdsubName = "";
                if (!string.IsNullOrEmpty(simpleUI.Target))
                {
                    var targetList = simpleUI.Target.Split(' '); // e.g. Beep2 Tone

                    cmd = GetCorrespondingCommand(btCharacteristic, targetList[0]);
                    if (targetList.Length > 1)
                    {
                        cmdsubName = targetList[1];
                        cmdsub = cmd.Parameters[cmdsubName];
                    }
                    if (targetList.Length > 2)
                    {
                        ; // Should never happen
                    }
                }

                var name = $"ui{nui}";
                var singleUI = new TemplateSnippet(name);
                uiList.AddChild(name, singleUI);
                singleUI.AddMacro("UIType", simpleUI.UIType); // ButtonFor RadioFor SliderFor RowStart RowEnd
                singleUI.AddMacro("Target", simpleUI.Target);
                singleUI.AddMacro("ComputeTarget", simpleUI.ComputeTarget);

                var label = simpleUI.Label;
                if (string.IsNullOrEmpty (label))
                {
                    if (cmd != null && !string.IsNullOrEmpty(cmd.Label))
                    {
                        label = cmd.Label;
                    }
                }
                singleUI.AddMacro("Label", label ?? "");
                var functionName = simpleUI.FunctionName; 
                if (string.IsNullOrEmpty(functionName))
                {
                    functionName = simpleUI.Target;
                }
                singleUI.AddMacro("FunctionName", functionName?.DotNetSafe() ?? "");
                singleUI.AddMacro("N", simpleUI.GetN().ToString());
                singleUI.AddMacro("Set0", simpleUI.Set.FirstOrDefault() ?? "");

                // Add in duration values for sliders. They are all prepended with e.g. "Duration_" or "Tone_"
                if (cmdsub != null)
                {
                    //singleUI.AddMacro($"{cmdsubName}_Min", cmdsub.Min.ToString());
                    //singleUI.AddMacro($"{cmdsubName}_Max", cmdsub.Max.ToString());
                    //singleUI.AddMacro($"{cmdsubName}_Value", cmdsub.Init.ToString());

                    singleUI.AddMacro("Slider_Min", cmdsub.Min.ToString());
                    singleUI.AddMacro("Slider_Max", cmdsub.Max.ToString());
                    singleUI.AddMacro("Slider_Init", cmdsub.Init.ToString());
                    singleUI.AddMacro("Slider_Label", cmdsub.Label);
                    singleUI.AddMacro("Sub_Label", cmdsub.Label);
                }

                // Add in the RadioFor list. Always make the list, even if it's empty.
                TemplateSnippet radioForList = new TemplateSnippet("RadioFor");
                singleUI.AddChild("RadioFor", radioForList);
                if (simpleUI.UIType == "RadioFor" || simpleUI.UIType == "ComboBoxFor")
                {
                    // Grab the items from the ValueNames
                    var vnames = cmdsub.ValueNames;
                    int n = 0;
                    foreach (var vname in vnames )
                    {
                        var value = new TemplateSnippet(vname.Key);
                        radioForList.AddChild(vname.Key, value);
                        value.AddMacro("RadioName", vname.Key);
                        value.AddMacro("RadioValue", vname.Value.ToString());
                        value.AddMacro("RadioChecked", n == 0 ? "True" : "False");
                        n++;
                    }
                }


                nui++;
            }
            ;

            var uiListType = nui > 0 ? "UIList" : "None";
            ch.AddMacro("UIListType", uiListType);
        }

        private static Command GetCorrespondingCommand(NameCharacteristic btCharacteristic, string target)
        {
            if (target == null) return null;
            if (btCharacteristic.Commands.TryGetValue (target, out Command value))
            {
                return value;
            }
            return null;
        }



        /// <summary>
        /// All of the verbs (read/write/notify) for a characteristic. They are called buttons because this
        // is used for generating the XAML buttons + textbox for each characteristic.
        // Source=Source=Services/Characteristics/Buttons
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="btCharacteristic"></param>
        private static void AddVerbButtons(TemplateSnippet ch, NameCharacteristic btCharacteristic)
        {
            var buttons = new TemplateSnippet("Buttons");
            ch.AddChild("Buttons", buttons); // always add, even if it's got nothing in it.
            var buttonList = new List<string>();
            if (btCharacteristic.IsWrite || btCharacteristic.IsWriteWithoutResponse)
            {
                buttonList.Add("Write");
            }
            if (btCharacteristic.IsRead)
            {
                buttonList.Add("Read");
            }
            if (btCharacteristic.IsNotify)
            {
                buttonList.Add("Notify");
            }
            foreach (var button in buttonList)
            {
                var buttonpr = new TemplateSnippet(button);
                buttons.AddChild(button, buttonpr);
                buttonpr.AddMacro("ButtonVerb", button); // Read Write Notify
            }
        }
        public static TemplateSnippet Convert(NameDevice bt)
        {
            TemplateSnippet retval = new TemplateSnippet(bt.Name);
            retval.Macros.Add("DeviceName", bt.Name);
            retval.Macros.Add("DeviceName.dotNet", bt.Name.DotNetSafe());
            retval.Macros.Add("CLASSNAME", bt.ClassName ?? bt.Name?.DotNetSafe());
            retval.Macros.Add("DESCRIPTION", bt.Description);
            retval.Macros.Add("CURRTIME", DateTime.Now.ToString("yyyy-MM-dd::hh:mm"));
            retval.Macros.Add("CLASSMODIFIERS", bt.ClassModifiers);

            //Services
            var servicesByPriority = new TemplateSnippet("Services");
            retval.AddChild("Services", servicesByPriority);
            retval.AddChild("ServicesByPriority", servicesByPriority);

            var servicesByOriginalOrder = new TemplateSnippet("ServicesByOriginalOrder");
            retval.AddChild("ServicesByOriginalOrder", servicesByOriginalOrder);

            var serviceListByOriginalOrder = bt.Services.ToList();
            var serviceListByPriority = bt.Services.OrderByDescending(bt => bt.Priority).ToList();
            foreach (var btService in serviceListByPriority)
            {
                if (btService.Suppress) continue;
                var service = ServiceToTemplateSnippet(btService);
                servicesByPriority.AddChildViaMacro(service); // have to wait until the UUID macro is added
            }
            foreach (var btService in serviceListByOriginalOrder)
            {
                if (btService.Suppress) continue;
                var service = ServiceToTemplateSnippet(btService);
                servicesByOriginalOrder.AddChildViaMacro(service); // have to wait until the UUID macro is added
            }

            // The LINKS
            var links = new TemplateSnippet("LINKS");
            retval.AddChild("LINKS", links);
            foreach (var link in bt.Links)
            {
                links.AddMacroNumber(link);
            }

            return retval;
        }
    }
}
