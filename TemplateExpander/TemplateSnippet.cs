﻿using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateExpander
{
    /// <summary>
    /// Contains the code snippet for a # ## ### etc area and a tree of all sub-shippets
    /// </summary>
    public class TemplateSnippet
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool OptionTrim { get; set; } = false;
        public string OptionFileName { get; set; } = "";
        public Dictionary<string, string> Macros { get; } = new Dictionary<string, string>();
        public Dictionary<string, TemplateSnippet> Children { get; }  = new Dictionary<string, TemplateSnippet>();
        public void AddChildViaMacro(TemplateSnippet child, string childId="UUID")
        {
            string name;
            var gotName = child.Macros.TryGetValue(childId, out name);
            if (!gotName)
            {
                Log($"ERROR: {NAME}: unable to get ID {childId} in AddChild; value not added");
            }
            Children.Add(name, child);
            child.Parent = this;
        }
        public void AddChild(string name, TemplateSnippet child)
        {
            Children.Add(name, child);
            child.Parent = this;
        }
        public void AddMacroNumber(string text)
        {
            var name = (Macros.Count).ToString();
            Macros.Add(name, text);
        }

        public TemplateSnippet Parent { get; set; }
        public string Errors { get; internal set; } = "";


        public enum Verbose { Verbose, Normal };
        public static Verbose Verbosity = Verbose.Normal;
        public static string NAME = "TemplateSnippet";
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

        /// <summary>
        /// Create a TemplateSnippet from a # NAME OPTION=VALUE OPTION=VALUE markdown line. Does not handle the back-tck code snippet bits.
        /// </summary>
        /// <param name="md"></param>
        public static TemplateSnippet ParseFromMD(string md)
        {
            var retval = new TemplateSnippet();
            var items = md.Trim().Split(new char[] { ' ' });
            retval.Name = items[0];
            for (int i=1; i<items.Length; i++)
            {
                var opts = items[i].Split(new char[] { '=' });
                if (opts.Length != 2)
                {
                    retval.Errors += $"ERROR: option {items[i]} isn't in format NAME=OPTION";
                }
                else
                {
                    switch (opts[0])
                    {
                        case "FileName":
                            retval.OptionFileName = opts[1];
                            break;
                        case "Trim":
                            switch (opts[1])
                            {
                                case "true":
                                    retval.OptionTrim = true;
                                    break;
                                case "false":
                                    retval.OptionTrim = false;
                                    break;
                                default:
                                    retval.Errors += $"ERROR: value {opts[0]}={opts[1]} should be true of false\n";
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return retval;
        }

        public static int Parse(TemplateSnippet allSnippets, string text)
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

        public static int ReadAllFiles(IEnumerable<string> files, TemplateSnippet allSnippets)
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
                        var fileNError = TemplateSnippet.Parse(allSnippets, text);
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

        public override string ToString()
        {
            return $"{Name} nmacros={Macros.Count} nchildren={Children.Count}";
        }
    }
}