﻿using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateExpander;

// See https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance
// See https://blogs.windows.com/windowsdeveloper/2020/09/03/calling-windows-apis-in-net5/
namespace BluetoothCodeGenerator
{
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
            var args = ProgramArgs.ParseFromArgs(rawargs);
            TemplateSnippet.Verbosity = (TemplateSnippet.Verbose)Verbosity;//Must keep in sync.
            TemplateSnippet.NAME = NAME;

            switch (args.Command)
            {
                case ProgramArgs.CommandType.Help:
                    Log($"{NAME}");
                    Log("--help or /? for help");
                    Log("-inputTemplates <directory> to select directory with template files in Markdown format");
                    Log("-inputBtFile <file> to select a single BT JSON file");
                    break;
                case ProgramArgs.CommandType.Error:
                    Log($"Error: {NAME}: {args.ErrorMessage}");
                    break;
                case ProgramArgs.CommandType.Run:
                    {
                        IEnumerable<string> files = null;
                        try
                        {
                            files = Directory.EnumerateFiles(args.InputTemplateDirectory);
                        }
                        catch (Exception)
                        {
                            Log($"Error: {NAME}: unable to get files from {args.InputTemplateDirectory}");
                            return;
                        }

                        TemplateSnippet allSnippets = new TemplateSnippet();

                        int nerror = TemplateSnippet.ReadAllFiles(files, allSnippets);
                        if (nerror > 0)
                        {
                            Log($"ERROR: {NAME}: total parse errors={nerror}");
                            return;
                        }

                        // Now write all files
                        foreach (var item in allSnippets.Children)
                        {
                            var child = item.Value;
                            if (string.IsNullOrEmpty(child.OptionFileName))
                            {
                                Log($"NOTE: can't write {child.Name} without a FileName=___ value");
                            }
                            else
                            {
                                // For each BT info, make the needed macros.
                                // This is just stubbed out for now.
                                var btdata = CreateMockBt.Create();

                                var fname = Expander.ExpandMacroTextOne(child.OptionFileName, 0, btdata);
                                Log($"Writing file {fname}");
                            }
                        }
                    }
                    break;
            }
        }
    }
}