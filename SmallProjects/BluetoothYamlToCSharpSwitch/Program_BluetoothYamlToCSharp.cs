#if NET8_0_OR_GREATER
#nullable disable
#endif


using SharpYaml;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace BluetoothYamlToCSharpSwitch
{
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
                return $"case {value}: return \"{name}\"; // {id}";
            }

        }

        interface Root
        {
            public List<NameValue> values { get; }
        }
        class BTUnitRoot : Root
        {
            public List<NameValue> values {  get { return uuids.Cast<NameValue>().ToList();  } }
            public List<BTUnit> uuids { get; set; } = new List<BTUnit>();
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
                return $"case {value}: /* {asdecimal} */ return \"{name}\";";
            }
        }


        class BTCompanyIdentifiers : Root
        {
            public List<NameValue> values { get { return company_identifiers.Cast<NameValue>().ToList(); } }
            public List<BTValueName> company_identifiers { get; set; } = new List<BTValueName>();
        }

        class UserOptions
        {
            public enum UpdateWithEnum { file }
            public UpdateWithEnum UpdateWith { get; set; } = UpdateWithEnum.file;
            public enum RunEnum { makecase, updatefile }
            public RunEnum RunType { get; set; } = RunEnum.makecase;
            public string Filename = @"c:\temp\2026\BluetoothSpecs\units.yaml";

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
            var units = YamlSerializer.Deserialize<BTUnitRoot>(yaml);
            if (units != null && units.values != null && units.values.Count > 0) return units.values;

            var company_identifiers = YamlSerializer.Deserialize<BTCompanyIdentifiers>(yaml);
            if (company_identifiers != null && company_identifiers.values != null && company_identifiers.values.Count > 0) return company_identifiers.values;

            return null;
        }

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
            Console.WriteLine(result);
        }

        enum UpdateStates {  Copy, InUpdateFile }
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
                        }
                        else if (line.StartsWith("// startupdatefile:"))
                        {
                            results.Add(rawline);
                            // Dump them all in!
                            foreach (var item in list)
                            {
                                results.Add($"\t\t\t\t{item}");
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
            foreach (var result in results)
            {
                Console.WriteLine(result);
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
