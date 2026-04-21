using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothCodeGenerator
{
    class ProgramArgs
    {
        public enum CommandType { Error, Help, Run }
        public CommandType Command { get; internal set; } = CommandType.Help;
        public string ErrorMessage { get; internal set; } = "";
        public string InputJsonFile { get; internal set; } = "";
        public string InputJsonDirectory { get; internal set; } = "";
        public string InputTemplateDirectory { get; internal set; } = ".\\Templates";
        public List<string> InputTemplateFiles { get; internal set; } = new List<string>(); // empty list means all the templates
        /// <summary>
        /// potentialFile might be Templates/CSharp_BT_template.md and will match -inputtemplatefile csharp_bt_template.md
        /// </summary>
        /// <param name="potentialFile"></param>
        /// <returns></returns>
        public bool InputTemplateFileMatches (string potentialFile)
        {
            bool retval = (InputTemplateFiles.Count == 0); // if no filter is specified, everything matches.
            foreach (var file in InputTemplateFiles)
            {
                if (potentialFile.EndsWith(file, StringComparison.OrdinalIgnoreCase))
                {
                    retval = true;
                    break;
                }
            }

            return retval;
        }
        public string OutputDirectory { get; internal set; } = ".\\output";
        public static ProgramArgs ParseFromArgs(string[] args)
        {
            var retval = new ProgramArgs();
            retval.Command = CommandType.Run;
            for (int i = 0; i < args.Length; i++)
            {
                //Console.WriteLine($"Verbose: reading argument {args[i]}");
                switch (args[i])
                {
                    case "/?":
                    case "--help":
                        retval.Command = CommandType.Help;
                        break;
                    case "-inputJsonDirectory":
                        if (i + 1 >= args.Length)
                        {
                            retval.Command = CommandType.Error;
                            retval.ErrorMessage = "Error: missing {args[i]} argument at {args[i]}";
                        }
                        else
                        {
                            retval.InputJsonDirectory = args[++i];
                        }
                        break;
                    case "-inputJsonFile":
                        if (i + 1 >= args.Length)
                        {
                            retval.Command = CommandType.Error;
                            retval.ErrorMessage = "Error: missing {args[i]} argument at {args[i]}";
                        }
                        else
                        {
                            retval.InputJsonFile = args[++i];
                        }
                        break;
                    case "-inputTemplates":
                        if (i + 1 >= args.Length)
                        {
                            retval.Command = CommandType.Error;
                            retval.ErrorMessage = "Error: missing {args[i]} argument at {args[i]}";
                        }
                        else
                        {
                            retval.InputTemplateDirectory = args[++i];
                        }
                        break;
                    case "-inputTemplateFile":
                        if (i + 1 >= args.Length)
                        {
                            retval.Command = CommandType.Error;
                            retval.ErrorMessage = "Error: missing {args[i]} argument at {args[i]}";
                        }
                        else
                        {
                            retval.InputTemplateFiles.Add(args[++i]);
                        }
                        break;
                    case "-output":
                        if (i + 1 >= args.Length)
                        {
                            retval.Command = CommandType.Error;
                            retval.ErrorMessage = "Error: missing {args[i]} argument at {args[i]}";
                        }
                        else
                        {
                            retval.OutputDirectory = args[++i];
                        }
                        break;
                    default:
                        retval.Command = CommandType.Error;
                        retval.ErrorMessage += $"Error: unknown argument {args[i]}\n";
                        break;
                }
            }
            return retval;
        }
    }

}
