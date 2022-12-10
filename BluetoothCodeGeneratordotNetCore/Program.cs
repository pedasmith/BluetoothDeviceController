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

        private static void ReadJsonFile(string filename,string logfname, List<TemplateSnippet> outputList)
        {
            if (filename != "")
            {
                try
                {
                    // Read in the Default device. 
                    //DefaultDevice = await InitBleDefault(dname);

                    var contents = File.ReadAllText(filename);
                    var newlist = Newtonsoft.Json.JsonConvert.DeserializeObject<NameAllBleDevices>(contents);
                    if (newlist.AllDevices.Count == 0)
                    {
                        Log($"Error: {NAME}: JSON file has no devices for file {logfname}");
                        return;
                    }
                    var ndevicesOk = 0;
                    var ndevicesNotOk = 0;
                    var ndevices = 0;
                    foreach (var nameDevice in newlist.AllDevices)
                    {
                        ndevices++;
                        if (filename.Contains ("Gems"))
                        {
                            ; // handy place to put a debugger.
                        }
                        if (nameDevice.CompletionStatus == NameDevice.CompletionStatusEnum.Unusable)
                        {
                            Log($"Note: {logfname}\tUNUSABLE");
                            ndevicesNotOk++;
                            continue;
                        }
                        outputList.Add(BtJsonToMacro.Convert(nameDevice));
                        ndevicesOk++;
                    }
                    if (ndevices != (ndevicesNotOk + ndevicesOk))
                    {
                        Log($"Error: {NAME}: incorrect error count in file {logfname} ");
                    }
                    else if (ndevicesOk > 0)
                    {
                        Log($"Verbose: read in file {logfname}");
                    }
                    // otherwise there were errors and they have already been displayed

                }
                catch (Exception e)
                {
                    Log($"Error: {NAME}: unable to read JSON file from {logfname}; reason {e.Message}");
                    return;
                }
            }
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
                        List<TemplateSnippet> jsonData = new List<TemplateSnippet>();
                        if (args.InputJsonFile != "")
                        {
                            ReadJsonFile(args.InputJsonFile, args.InputJsonFile, jsonData);
                        }
                        if (args.InputJsonDirectory != "")
                        {
                            try
                            {
                                files = Directory.EnumerateFiles(args.InputJsonDirectory);
                                foreach (var file in files)
                                {
                                    if (file.EndsWith(".json"))
                                    {
                                        var logfname = file;
                                        var idx = logfname.IndexOf("..");
                                        if (idx > 0) logfname = logfname.Substring(idx); 
                                        ReadJsonFile(file, logfname, jsonData);
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                Log($"Error: {NAME}: unable to get files from {args.InputJsonDirectory}");
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
                                // Log($"Verbose: jsonData length {jsonData.Count}");
                                foreach (var btdata in jsonData)
                                {
                                    if (string.IsNullOrEmpty (btdata.Name)
                                        || btdata.Name.StartsWith("##"))
                                    {
                                        continue; // skip it. Can legit be this way (e.g., default is like this)
                                    }
                                    //var btdata = jsonData; // Switch to closer to the real thing! CreateMockBt.Create();
                                    Expander.ExpandChildTemplatesIntoMacros(child, btdata);

                                    var fname = Expander.ExpandMacroAll(child.OptionFileName, btdata);
                                    var dirname = string.IsNullOrEmpty(child.OptionDirName) ? "" : "\\" + child.OptionDirName;
                                    var outfilename = args.OutputDirectory + dirname + "\\" + fname;
                                    Directory.CreateDirectory(args.OutputDirectory + dirname);
                                    //Log($"Writing file {fname} as {outfilename}");
                                    if (fname == ".cs")
                                    {
                                        ; // handy place to hang a debugger.
                                    }

                                    var codeTemplate = child.Code;
                                    if (codeTemplate == null)
                                    {
                                        Log($"Error: was unable to expand the Code template for {fname}");
                                    }
                                    else
                                    {
                                        var code = Expander.ExpandMacroAll(codeTemplate, btdata);
                                        Log($"Writing file {fname} as {outfilename}; length {codeTemplate.Length}==>{code.Length}");
                                        File.WriteAllText(outfilename, code, Encoding.UTF8);
                                    }
                                }
                            }
                        }
                    }
                    break;
            } // end switch on the command type (run, error, help, etc.)
        }
    }
}
