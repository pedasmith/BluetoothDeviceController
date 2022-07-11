using BluetoothDeviceController.Names;
using Microsoft.Toolkit.Parsers.Markdown;
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

                        TemplateSnippet allSnippets = new TemplateSnippet("allSnippets");

                        int nerror = TemplateSnippet.ReadAllFiles(files, allSnippets);
                        if (nerror > 0)
                        {
                            Log($"ERROR: {NAME}: total parse errors={nerror}");
                            return;
                        }

                        // Read in the JSON file
                        TemplateSnippet jsonData = null;
                        if (args.InputJsonFile != "")
                        {
                            try
                            {
                                // Read in the Default device. 
                                //DefaultDevice = await InitBleDefault(dname);

                                var contents = File.ReadAllText(args.InputJsonFile);
                                var newlist = Newtonsoft.Json.JsonConvert.DeserializeObject<NameAllBleDevices>(contents);
                                jsonData = BtJsonToMacro.Convert(newlist.AllDevices[0]); // TODO: for now, just convert the one
                                //InitSingleBleFile(AllDevices, file, DefaultDevice);
                                //InitSingleBleFile(AllRawDevices, file, null); // read in a device without adding in default services

                            }
                            catch (Exception)
                            {
                                Log($"Error: {NAME}: unable to read JSON files from {args.InputJsonFile}");
                                return;
                            }
                        }

                        // Now write all files based on the template snippets.
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
                                var btdata = jsonData; // Switch to closer to the real thing! CreateMockBt.Create();
                                Expander.ExpandChildTemplatesIntoMacros(child, btdata);

                                var fname = Expander.ExpandMacroAll(child.OptionFileName, btdata);
                                var outfilename = args.OutputDirectory + "\\" + fname;
                                Log($"Writing file {fname} as {outfilename}");

                                var codeTemplate = child.Code;
                                var code = Expander.ExpandMacroAll(codeTemplate, btdata);
                                Log($"Length starts as {codeTemplate.Length} and becomes {code.Length}");
                                File.WriteAllText(outfilename, code);

                            }
                        }
                    }
                    break;
            }
        }
    }
}
