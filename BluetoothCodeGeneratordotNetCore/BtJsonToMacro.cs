using BluetoothDeviceController.BleEditor;
using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateExpander;

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
            Error($"ByteFormatToCSharp: unknown format {format}");
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

        public static TemplateSnippet Convert(NameDevice bt)
        {
            TemplateSnippet retval = new TemplateSnippet(bt.Name);
            retval.Macros.Add("DeviceName", bt.Name);
            retval.Macros.Add("CLASSNAME", bt.ClassName ?? bt.Name?.DotNetSafe());
            retval.Macros.Add("DESCRIPTION", bt.Description);
            retval.Macros.Add("CURRTIME", DateTime.Now.ToString("yyyy-MM-dd::hh:mm"));
            retval.Macros.Add("CLASSMODIFIERS", bt.ClassModifiers);

            //Services
            var services = new TemplateSnippet("Services");
            retval.AddChild("Services", services);

            foreach (var btService in bt.Services)
            {
                if (btService.Suppress) continue;

                var service = new TemplateSnippet(btService.Name);
                service.Macros.Add("Name", btService.Name);
                service.Macros.Add("Name.dotNet", btService.Name.DotNetSafe());
                service.Macros.Add("ServiceName", btService.Name);
                service.Macros.Add("UUID", btService.UUID);
                services.AddChildViaMacro(service); // have to wait until the UUID macro is added

                var chs = new TemplateSnippet("Characteristics");
                service.AddChild("Characteristics", chs);

                foreach (var btCharacteristic in btService.Characteristics)
                {
                    if (btCharacteristic.Suppress) continue;

                    var ch = new TemplateSnippet(btCharacteristic.Name);
                    ch.Macros.Add("UUID", btCharacteristic.UUID);
                    ch.Macros.Add("Name", btCharacteristic.Name);
                    ch.Macros.Add("CharacteristicName", btCharacteristic.Name);
                    ch.Macros.Add("Name.dotNet", btCharacteristic.Name.DotNetSafe());
                    ch.Macros.Add("Type", btCharacteristic.Type);
                    ch.Macros.Add("CHARACTERISTICTYPE", btCharacteristic.Type);
                    ch.Macros.Add("Verbs", btCharacteristic.Verbs);

                    // For devices with both Write and WriteWithoutResponse, I prefer
                    // using plain WriteWithoutResponse because it's faster.
                    // Only set the macro for writable ones.
                    if (btCharacteristic.IsWrite || btCharacteristic.IsWriteWithoutResponse)
                    {
                        ch.Macros.Add("WRITEMODE", btCharacteristic.IsWriteWithoutResponse
                            ? "GattWriteOption.WriteWithoutResponse"
                            : "GattWriteOption.WriteWithResponse");
                    }

                    chs.AddChildViaMacro(ch); // uses the UUID to add correctly



                    var prs = new TemplateSnippet("Properties"); // Properties aka args 
                    ch.AddChild("Properties", prs); // always add, even if it's got nothing in it.
                    bool hasProperty = btCharacteristic.IsRead || btCharacteristic.IsNotify;
                    if (hasProperty)
                    {
                        var split = ValueParserSplit.ParseLine(btCharacteristic.Type);

                        // Properties are per-data which is finer grained than just per-characteristic.
                        for (int i = 0; i < split.Count; i++)
                        {
                            var item = split[i];
                            var dataname = item.NamePrimary;
                            if (dataname == "") dataname = $"param{i}";
                            if (ItemIsSuppressed(item)) continue; // skip OEL and OEB (little and big endian indicators)

                            var pr = new TemplateSnippet(dataname);
                            prs.AddChild(dataname, pr);

                            // CHDATANAME is either char_data (to make it more unique) or just plain characteristicname.
                            // If a charateristic has just a single data item, that one item doesn't need a seperate name here.
                            // so characteristic "temparature" with a single data value "temperature" will get a chdataname
                            // of "temperature". If there were two data value (temp and humidity) they would get unique names
                            // (temperature_temp and temperature_humidity)
                            var name = btCharacteristic.Name;
                            pr.AddMacro("NAME", name);
                            pr.AddMacro("CHDATANAME", split.Count == 1
                                ? btCharacteristic.Name.DotNetSafe()
                                : btCharacteristic.Name.DotNetSafe() + "_" + dataname.DotNetSafe());
                            pr.AddMacro("DATANAME", dataname.DotNetSafe());
                            pr.AddMacro("DATANAMEUSER", dataname.Replace("_", " "));
                            pr.AddMacro("VARIABLETYPE", ByteFormatToCSharp(item.ByteFormatPrimary));
                            pr.AddMacro("VARIABLETYPEPARAM", ByteFormatToCSharpParam(item.ByteFormatPrimary));
                            pr.AddMacro("VARIABLETYPE+DS", ByteFormatToCSharpStringOrDouble(item.ByteFormatPrimary));
                            pr.AddMacro("ARGDWCALL", ByteFormatToDataWriterCall(item.ByteFormatPrimary));
                            pr.AddMacro("ARGDWCALLCAST", ByteFormatToDataWriterCallCast(item.ByteFormatPrimary));
                            pr.AddMacro("AS+DOUBLE+OR+STRING", ByteFormatToCSharpAsDouble(item.ByteFormatPrimary)); // e.g.  ".AsDouble";
                            pr.AddMacro("DOUBLE+OR+STRING+DEFAULT", ByteFormatToCSharpDefault(item.ByteFormatPrimary));
                        }
                    }
                    prs = null;

                    var wprs = new TemplateSnippet("WriteParams"); // Properties aka args 
                    ch.AddChild("WriteParams", wprs); // always add, even if it's got nothing in it.
                    bool hasWrite = btCharacteristic.IsWrite || btCharacteristic.IsWriteWithoutResponse;
                    if (hasWrite)
                    {
                        var split = ValueParserSplit.ParseLine(btCharacteristic.Type);

                        // Properties are per-data which is finer grained than just per-characteristic.
                        for (int i = 0; i < split.Count; i++)
                        {
                            var item = split[i];
                            var dataname = item.NamePrimary;
                            if (dataname == "") dataname = $"param{i}";
                            if (ItemIsSuppressed(item)) continue; // skip OEL and OEB (little and big endian indicators)

                            var pr = new TemplateSnippet(dataname);
                            wprs.AddChild(dataname, pr);

                            // CHDATANAME is either char_data (to make it more unique) or just plain characteristicname.
                            // If a charateristic has just a single data item, that one item doesn't need a seperate name here.
                            // so characteristic "temparature" with a single data value "temperature" will get a chdataname
                            // of "temperature". If there were two data value (temp and humidity) they would get unique names
                            // (temperature_temp and temperature_humidity)
                            var name = btCharacteristic.Name;
                            pr.AddMacro("NAME", name);
                            pr.AddMacro("CHDATANAME", split.Count == 1
                                ? btCharacteristic.Name.DotNetSafe()
                                : btCharacteristic.Name.DotNetSafe() + "_" + dataname.DotNetSafe());
                            pr.AddMacro("DATANAME", dataname.DotNetSafe());
                            pr.AddMacro("DATANAMEUSER", dataname.Replace("_", " "));
                            pr.AddMacro("VARIABLETYPE", ByteFormatToCSharp(item.ByteFormatPrimary));
                            pr.AddMacro("VARIABLETYPEPARAM", ByteFormatToCSharpParam(item.ByteFormatPrimary));
                            pr.AddMacro("VARIABLETYPE+DS", ByteFormatToCSharpStringOrDouble(item.ByteFormatPrimary));
                            pr.AddMacro("ARGDWCALL", ByteFormatToDataWriterCall(item.ByteFormatPrimary));
                            pr.AddMacro("ARGDWCALLCAST", ByteFormatToDataWriterCallCast(item.ByteFormatPrimary));
                            pr.AddMacro("AS+DOUBLE+OR+STRING", ByteFormatToCSharpAsDouble(item.ByteFormatPrimary)); // e.g.  ".AsDouble";
                            pr.AddMacro("DOUBLE+OR+STRING+DEFAULT", ByteFormatToCSharpDefault(item.ByteFormatPrimary));
                        }
                    }

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
                                //TODO:: this was the old way andcan be deleted (it was removed before it was even used!)
                                //var enumvalue = new TemplateSnippet(vname);
                                //vns.AddChild(vname, enumvalue);
                                //enumvalue.AddMacro("EnumName", vname);
                                //enumvalue.AddMacro("EnumValue", vvalue.ToString());
                                vns.AddMacro(vname.DotNetSafe(), vvalue.ToString()); // should be simple, not the complexity of the first attempt.
                            }
                        }
                    }


                }
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
