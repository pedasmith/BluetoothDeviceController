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
        public string InputTemplateDirectory { get; internal set; } = ".\\";
        public string OutputDirectory { get; internal set; } = ".\\output";
        public static ProgramArgs ParseFromArgs(string[] args)
        {
            var retval = new ProgramArgs();
            retval.Command = CommandType.Run;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "/?":
                    case "--help":
                        retval.Command = CommandType.Help;
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
                        retval.ErrorMessage = $"Error: unknown argument {args[i]}";
                        break;
                }
            }
            return retval;
        }
    }

}
