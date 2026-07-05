#if NET8_0_OR_GREATER
#nullable disable
#endif


using SharpYaml;
using System.Globalization;


namespace BluetoothYamlToCSharpSwitch
{
    public static class StringExtensions
    {
        public static string RemoveQuotes(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace("\"", "'");
            return s;
        }
    }
    internal class Program
    {
        const string VERSION = "12:41";

        interface NameValue
        {
            public string value { get; set; }
            public string name { get; set; }

        }
        class BTUnit : NameValue
        {
            public string uuid { get { return value; } set { this.value = value; } }
            public string value { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public override string ToString()
            {
                return $"case {value}: return \"{name.RemoveQuotes()}\"; // {id}";
            }

        }


        class BTValueName : NameValue
        {
            public string value { get; set; }
            public string name { get; set; }
            public override string ToString()
            {
                uint asdecimal = 0xFFF0;
                string noleading = value;
                if (noleading.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) noleading = noleading.Substring(2);
                var ishex = uint.TryParse(noleading, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out asdecimal);
                if (value.Contains("FF"))
                {
                    ; // handy place for a debugger.
                }
                if (value.Contains("–") || value.Contains ("-") )
                {
                    // – EN DASH U+2013 found in e.g. org.bluetooth.characteristic.body_sensor_location.yaml
                    // for example, org.bluetooth.characteristic.body_sensor_location.yaml has a
                    // value which is 0x7--0xFF
                    return $"default: /* case {value} */ return $\"{name.RemoveQuotes()} ({{value:X2}})\";";

                }
                return $"case {value}: /* {asdecimal} */ return \"{name.RemoveQuotes()}\";";
            }
        }

        class BTValueDescription : NameValue
        {
            public string value { get; set; }
            public string name { get; set; }
            public string description { get { return name; } set { name = value; } } // "value" here is the parameter, not the value field :-)
            public override string ToString()
            {
                uint asdecimal = 0xFFF0;
                string noleading = value;
                if (noleading.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) noleading = noleading.Substring(2);
                var ishex = uint.TryParse(noleading, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out asdecimal);
                if (value.Contains("FF"))
                {
                    ; // handy place for a debugger.
                }
                if (value.Contains("–") || value.Contains("-"))
                {
                    // – EN DASH U+2013 found in e.g. org.bluetooth.characteristic.body_sensor_location.yaml
                    // for example, org.bluetooth.characteristic.body_sensor_location.yaml has a
                    // value which is 0x7--0xFF
                    return $"default: /* case {value} */ return $\"{name.RemoveQuotes()} ({{value:X2}})\";";
                }
                return $"case {value}: /* {asdecimal} */ return \"{name.RemoveQuotes()}\";";
            }
        }

        class BTNameDescriptionValues 
        {
            public string name { get; set; }
            public string description { get; set; }
            public List<BTValueDescription> values { get; set; } = new();
            public override string ToString()
            {
                return $"// {name} {description} count={values.Count}";
            }
        }



        interface Root
        {
            public List<NameValue> values { get; }
        }
        class BTuuids_Root : Root
        {
            public List<NameValue> values {  get { return uuids.Cast<NameValue>().ToList();  } }
            public List<BTUnit> uuids { get; set; } = new ();
        }

        class BTad_types_Root : Root
        {
            public List<NameValue> values { get { return ad_types.Cast<NameValue>().ToList(); } }
            public List<BTUnit> ad_types { get; set; } = new ();
        }


        class BTcompany_identifiers_Root : Root
        {
            public List<NameValue> values { get { return company_identifiers.Cast<NameValue>().ToList(); } }
            public List<BTValueName> company_identifiers { get; set; } = new ();
        }

        class BTfields_Root : Root
        {
            public List<NameValue> values { get { return fields[0].values.Cast<NameValue>().ToList(); } }
            public string name { get; set; }
            public string description { get; set; }
            public List<BTNameDescriptionValues> fields { get; set; } = new (); // always a list of one :-)
        }

        class BTcharacteristic_fields_Root : Root
        {
            public BTfields_Root characteristic { get; set; }
            public List<NameValue> values { get { return characteristic.fields[0].values.Cast<NameValue>().ToList(); } }
        }


        class UserOptions
        {
            public enum UpdateWithEnum { file }
            public UpdateWithEnum UpdateWith { get; set; } = UpdateWithEnum.file;
            public enum RunEnum { makecase, updatefile }
            public RunEnum RunType { get; set; } = RunEnum.makecase;
            public string Filename = @"c:\temp\2026\BluetoothSpecs\units.yaml";
            public string OutputDir = ""; // blank means write to STDOUT. Most common is "output"

            private static void Err(string s)
            {
                Console.Error.WriteLine(s);
            }
            public static UserOptions Parse(string[] args)
            {
                var retval = new UserOptions();
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-f":
                        case "--file":
                            if (i + 1 < args.Length)
                            {
                                retval.Filename = args[++i];
                            }
                            else
                            {
                                Err("Error: Missing filename after --file");
                                Err($"Expected --file <filename>");
                                Environment.Exit(1);
                            }
                            break;
                        case "--outputdir":
                            if (i + 1 < args.Length)
                            {
                                var value = args[++i];
                                retval.OutputDir = value;
                            }
                            else
                            {
                                Err("Error: Missing directory after --outputdir");
                                Err($"Expected --outputdir output");
                                Environment.Exit(1);
                            }
                            break;

                        case "--type":
                            if (i + 1 < args.Length)
                            {
                                var value = args[++i];
                                switch (value)
                                {
                                    case "makecase": retval.RunType = RunEnum.makecase; break;
                                    case "updatefile": retval.RunType = RunEnum.updatefile; break;
                                    default:
                                        Err($"Error: Unknown runtype {value} after --type");
                                        Err($"Expected --type <runtype>");
                                        Environment.Exit(1);
                                        break;
                                }
                            }
                            else
                            {
                                Err("Error: Missing runtype after --type");
                                Err($"Expected --type makecase|updatefile");
                                Environment.Exit(1);
                            }
                            break;
                        case "--updatewith":
                            if (i + 1 < args.Length)
                            {
                                var value = args[++i];
                                switch (value)
                                {
                                    case "file": retval.UpdateWith = UpdateWithEnum.file; break;
                                    default:
                                        Err($"Error: Unknown --updatewith {value}");
                                        Err($"Expected --updatewith file");
                                        Environment.Exit(1);
                                        break;
                                }
                            }
                            else
                            {
                                Err("Error: Missing runtype after --updatewith");
                                Err($"Expected --updatewith file");
                                Environment.Exit(1);
                            }
                            break;
                        case "--version":
                            Console.WriteLine($"Version: {VERSION}");
                            Environment.Exit(0);
                            break;
                        default:
                            Err($"Error: Unknown option {args[i]}");
                            Err($"Expected --file <filename>");
                            Environment.Exit(1);
                            break;
                    }
                }
                return retval;
            }
        }

        static List<NameValue> GetYamlList(string yaml)
        {
            var units = YamlSerializer.Deserialize<BTuuids_Root>(yaml);
            if (units != null && units.values != null && units.values.Count > 0) return units.values;

            var ad_types = YamlSerializer.Deserialize<BTad_types_Root>(yaml);
            if (ad_types != null && ad_types.values != null && ad_types.values.Count > 0) return ad_types.values;

            var company_identifiers = YamlSerializer.Deserialize<BTcompany_identifiers_Root>(yaml);
            if (company_identifiers != null && company_identifiers.values != null && company_identifiers.values.Count > 0) return company_identifiers.values;

            var characteristic_fields = YamlSerializer.Deserialize<BTcharacteristic_fields_Root>(yaml);
            if (characteristic_fields != null && characteristic_fields.values != null && characteristic_fields.values.Count > 0) return characteristic_fields.values;

            return null;
        }

        /// <summary>
        /// Old method; I used to just generate the inner CASE statements. The new way is that 
        /// each file includes a set of commands for where in the generated code should go.
        /// </summary>
        /// <param name="options"></param>
        static void DoMakeCase(UserOptions options)
        {
            string yaml = File.ReadAllText(options.Filename);
            var list = GetYamlList(yaml);
            if (list == null)
            {
                Console.Error.WriteLine($"Error: unable to parse YAML {yaml}");
                Environment.Exit(2);
            }
            var result = "";
            foreach (var value in list)
            {
                result += $"\t\t\t\t{value}\r\n";
            }
            if (string.IsNullOrEmpty(options.OutputDir))
            {
                Console.WriteLine(result);
            }
            else
            {
                var fname = System.IO.Path.GetFileName(options.Filename);
                var outf = $"{options.OutputDir}\\{fname}";
                System.IO.File.WriteAllText(outf, result);
            }
        }

        enum UpdateStates {  Copy, InUpdateFile }
        /// <summary>
        /// The most common method! This is the sophisticed code that handles the updatefile command.
        /// </summary>
        /// <param name="options"></param>
        static void DoUpdateFile(UserOptions options)
        {
            string[] source = [];
            try
            {
                source = File.ReadAllLines(options.Filename);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error reading file {options.Filename}: {ex.Message}");
                Environment.Exit(1);
            }
            UpdateStates state = UpdateStates.Copy;
            List<NameValue> list = null;
            var results = new List<string>();
            for (int i = 0; i < source.Length; i++)
            {
                var rawline = source[i];
                var line = rawline.Trim();
                switch (state)
                {
                    case UpdateStates.Copy:
                        if (line.StartsWith("// updatefile:"))
                        {
                            results.Add(rawline);
                            state = UpdateStates.InUpdateFile;
                        }
                        else
                        {
                            results.Add(rawline);
                        }
                        break;
                    case UpdateStates.InUpdateFile:
                        if (line.StartsWith("// file:"))
                        {
                            results.Add(rawline);

                            var fname = line.Split(':')[1]; // file:filename.yaml
                            string yaml = "";
                            try
                            {
                                yaml = File.ReadAllText(fname);
                            }
                            catch (Exception ex)
                            {
                                Console.Error.WriteLine($"Error reading YAML file {fname}: {ex.Message}");
                                Environment.Exit(1);
                            }
                            // units = YamlSerializer.Deserialize<BTUnitRoot>(yaml);
                            list = GetYamlList(yaml);
                            if (list == null)
                            {
                                Console.Error.WriteLine($"Error: the YAML file {fname} for {options.Filename} had no values");
                                Environment.Exit(1);
                            }
                        }
                        else if (line.StartsWith("// startupdatefile:"))
                        {
                            results.Add(rawline);
                            // Dump them all in!

                            if (list == null)
                            {
                                Console.Error.WriteLine($"Error: no list of values to substitute in for {options.Filename}");
                                Environment.Exit(1);
                            }
                            HashSet<string> potentialDups = new HashSet<string>();
                            foreach (var item in list)
                            {
                                bool itemIsNew = potentialDups.Add(item.value);
                                if (!itemIsNew)
                                {
                                    Console.Error.WriteLine($"Warning: duplicate value {item} in {options.Filename}");
                                    results.Add($"\t\t\t\t//DUPLICATE: {item}");
                                }
                                else
                                {
                                    results.Add($"\t\t\t\t{item}");
                                }
                            }

                        }
                        else if (line.StartsWith("// endupdatefile:"))
                        {
                            results.Add(rawline);
                            state = UpdateStates.Copy;
                        }
                        else if (line.StartsWith("// ")) // Might be a command, but we don't know it, so just copy it
                        {
                            results.Add(rawline);
                        }
                        else // code line which we are replacing, so ignore it
                        {
                            ;
                        }
                        break;
                }
            }
            Console.Error.WriteLine($"Processed file {options.Filename} nline={source.Length} result nline={results.Count}");
            if (string.IsNullOrEmpty(options.OutputDir))
            {
                foreach (var result in results)
                {
                    Console.WriteLine(result);
                }
            }
            else
            {
                var fname = System.IO.Path.GetFileName(options.Filename);
                var outf = $"{options.OutputDir}\\{fname}";
                System.IO.File.WriteAllLines(outf, results);
            }
        }
        static void Main(string[] args)
        {
            UserOptions options = UserOptions.Parse(args);
            switch (options.RunType)
            {
                case UserOptions.RunEnum.makecase:
                    DoMakeCase(options);
                    break;
                case UserOptions.RunEnum.updatefile:
                    DoUpdateFile(options);
                    break;
            }
        }
    }
}
