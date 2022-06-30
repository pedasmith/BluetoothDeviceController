using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// See https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance
// See https://blogs.windows.com/windowsdeveloper/2020/09/03/calling-windows-apis-in-net5/
namespace BluetoothCodeGenerator
{
    class Args
    {
        public enum CommandType { Error, Help, Run }
        public CommandType Command { get; internal set; } = CommandType.Help;
        public string ErrorMessage { get; internal set; } = "";

        public string MarkdownDirectory { get; internal set; } = ".\\";
        public string OutputDirectory { get; internal set; } = ".\\output";
        public static Args ParseFromArgs(string[] args)
        {
            var retval = new Args();
            retval.Command = CommandType.Run;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "/?":
                    case "--help":
                        retval.Command = CommandType.Help;
                        break;
                    case "-input":
                        if (i + 1 >= args.Length)
                        {
                            retval.Command = CommandType.Error;
                            retval.ErrorMessage = "Error: missing {args[i]} argument at {args[i]}";
                        }
                        else
                        {
                            retval.MarkdownDirectory = args[++i];
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

    class Program
    {
        const string NAME = "Bluetooth Code Generator";

        enum Verbose { Verbose, Normal };
        private static Verbose Verbosity = Verbose.Normal;
        private static void Log(string text, Verbose verbose = Verbose.Normal)
        {
            if (verbose >= Verbosity)
            {
                Console.WriteLine(text);
            }
        }
        private static void LogVerbose(string text)
        {
            Log(text, Verbose.Verbose);
        }


        static void Main(string[] rawargs)
        {
            var args = Args.ParseFromArgs(rawargs);
            switch (args.Command)
            {
                case Args.CommandType.Help:
                    Log($"{NAME}");
                    Log("--help or /? for help");
                    Log("-input <directory> to select directory with template files in Markdown format");
                    break;
                case Args.CommandType.Error:
                    Log($"Error: {NAME}: {args.ErrorMessage}");
                    break;
                case Args.CommandType.Run:
                    {
                        IEnumerable<string> files = null;
                        try
                        {
                            files = Directory.EnumerateFiles(args.MarkdownDirectory);
                        }
                        catch (Exception)
                        {
                            Log($"Error: {NAME}: unable to get files from {args.MarkdownDirectory}");
                            return;
                        }


                    }
                    break;
            }
        }
    }
}
