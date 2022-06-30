using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateExpander;

namespace BluetoothCodeGenerator
{
    class Args
    {
        public enum CommandType {  Error, Help, Run }
        public CommandType Command { get; internal set; } = CommandType.Help;
        public string ErrorMessage { get; internal set; } = "";

        public string MarkdownDirectory { get; internal set; } = ".\\";
        public string OutputDirectory { get; internal set; } = ".\\output";
        public static Args ParseFromArgs(string[] args)
        {
            var retval = new Args();
            retval.Command = CommandType.Run;
            for (int i=0; i<args.Length; i++)
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

        enum Verbose {  Verbose, Normal};
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

        private static int Parse(TemplateSnippet allSnippets, string text)
        {
            var md = new MarkdownDocument();
            md.Parse(text);

            int nerror = 0;
            Stack<HeaderBlock> currHeaderStack = new Stack<HeaderBlock>();
            Stack<TemplateSnippet> currTemplateStack = new Stack<TemplateSnippet>();
            foreach (var element in md.Blocks)
            {
                if (element is HeaderBlock header)
                {
                    LogVerbose($"Template: {header} level {header.HeaderLevel}");
                    if (currHeaderStack.Count == 0)
                    {
                        if (header.HeaderLevel != 1)
                        {
                            Log($"ERROR: {NAME}: header {header} is first header in file; must be level 1 (# NAME)");
                            nerror++;
                            header.HeaderLevel = 1; // fix is up so the parsing can continue without totally dying.
                        }
                    }
                    while (currHeaderStack.Count >= 1 && currHeaderStack.Peek().HeaderLevel >= header.HeaderLevel)
                    {
                        // Example: file is header 1A, 2A, 3A, 4A, 2B, and we are just reading 2B. The start starts as
                        // 1A, 2A, 3A, 4A. Pop off the 2A, 3A, 4A and push 2B so the stack ends as 1A 2B.
                        currHeaderStack.Pop();
                        currTemplateStack.Pop();
                    }
                    currHeaderStack.Push(header);

                    // Now create a TemplateSnippet and add it to the 'parent'
                    var newts = TemplateSnippet.ParseFromMD(header.ToString());
                    if (!string.IsNullOrEmpty(newts.Errors))
                    {
                        Log($"ERROR: {NAME}: unable to handle header {newts.Name} with {newts.Errors}");
                        nerror++;
                    }
                    if (currTemplateStack.Count > 0)
                    {
                        // There's a parent; add this 
                        var parent = currTemplateStack.Peek();
                        if (parent.Children.ContainsKey(newts.Name))
                        {
                            Log($"ERROR: {NAME}: duplicate child name {newts.Name} for parent {parent.Name}");
                            nerror++;
                        }
                        parent.Children.Add(newts.Name, newts);
                    }
                    else
                    {
                        // This is a level-1 header; add to the allShippet
                        allSnippets.Children.Add(newts.Name, newts);
                    }

                    currTemplateStack.Push(newts);

                }
                else if (element is CodeBlock block)
                {
                    if (currHeaderStack.Count() < 1)
                    {
                        Log($"ERROR: {NAME}: got a code block before the first header in the file.");
                        nerror++;
                    }
                    else
                    {
                        var ts = currTemplateStack.Peek();
                        if (string.IsNullOrEmpty(block.CodeLanguage))
                        {
                            if (!string.IsNullOrEmpty(ts.Code))
                            {
                                Log($"ERROR: {NAME}: already have a code block for {ts.Name}");
                                nerror++;
                            }
                            else
                            {
                                ts.Code = element.ToString();
                            }
                        }
                        else if (block.CodeLanguage != "SAMPLE" && block.CodeLanguage != "TEST")
                        {
                            Log($"ERROR: {NAME}: code block language {block.CodeLanguage} must be SAMPLE or TEST for {ts.Name}");
                            nerror++;
                        }
                    }
                }
                else
                {
                    LogVerbose($"Template: other: {element.Type}");
                }
            }
            return nerror;
        }

        private static int ReadAllFiles(IEnumerable<string> files, TemplateSnippet allSnippets)
        {
            int nerror = 0;
            foreach (var file in files)
            {
                if (file.EndsWith(".md"))
                {
                    Log($"Reading file: {file}");
                    string text = null;
                    try
                    {
                        text = File.ReadAllText(file);
                        var fileNError = Parse(allSnippets, text);
                        if (fileNError > 0)
                        {
                            Log($"ERROR: {NAME}: File {file} had {fileNError} errors");
                        }
                        nerror += fileNError;
                    }
                    catch (Exception e)
                    {
                        Log($"Error: {NAME}: unable to read file {file} reason={e.Message}");
                        nerror++;
                        return nerror;
                    }

                }
            }
            if (nerror > 0)
            {
                Log($"ERROR: {NAME}: total parse errors={nerror}");
            }
            return nerror;
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

                        TemplateSnippet allSnippets = new TemplateSnippet();

                        int nerror = ReadAllFiles(files, allSnippets);
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
                                Log($"Writing file {child.OptionFileName}");
                            }
                        }
                    }
                    break;
            }
        }
    }
}
