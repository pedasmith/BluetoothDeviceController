using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateExpander
{
    class Expander
    {
        /// <summary>
        /// Given text with embedded macros, keep on expanding it until it's all expanded. Will stop after 
        /// a large preset value (5000?) and returns an error so we don't do an infinite loop.
        /// </summary>
        public static string ExpandMacroAll(string text, TemplateSnippet macros)
        {
            var retval = text;
            int startIndex = 0;
            int nloop = 0;
            while (startIndex >= 0 && nloop < 5000)
            {
                retval = ExpandMacroTextOne(retval, ref startIndex, macros);
                nloop++;
            }
            if (nloop >= 5000)
            {
                retval = $"ERROR: too many macro expansions at {startIndex}\n" + retval;
            }
            return retval;
        }
        
        
        
        /// <summary>
        /// Given a string of the name of a macro to expand (like CLASSNAME), expand it.
        /// </summary>
        private static string ExpandMacroOne (string macro, TemplateSnippet macros)
        {
            string retval;
            if (macros == null)
            {
                retval = $"<<ERROR: unable to expand {macro}; no macros to expand with>>";
            }
            else if (macro.StartsWith("../"))
            {
                // Expand as parent
                retval = ExpandMacroOne(macro.Substring(3), macros.Parent);
            }
            else
            {
                bool hasMacro = macros.Macros.TryGetValue(macro, out retval);
                if (!hasMacro)
                {
                    var parent = macros.Parent;
                    while (parent != null && !hasMacro)
                    {
                        hasMacro = parent.Macros.TryGetValue(macro, out retval);
                        parent = parent.Parent;
                    }
                }
                if (!hasMacro)
                {
                    // Not yet
                    //retval = $"<<ERROR: unable to find macro {macro}>>";
                }
            }
            return retval;
        }

        /// <summary>
        /// Given a string with "some [[MACRO]] text", expand the MACRO.Only one macro will be expanded.
        /// </summary>
        /// <param name="text">Text to expand</param>
        /// <param name="startIndex">starting index; will be updated as needed</param>
        /// <param name="macros"></param>
        /// <returns></returns>
        private static string ExpandMacroTextOne(string text, ref int startIndex, TemplateSnippet macros)
        {
            var retval = text;
            var nextMacro = retval.IndexOf("[[", startIndex);
            if (nextMacro < 0)
            {
                startIndex = -1;
                return retval; // no macro to expand
            }
            var endIndex = retval.IndexOf("]]", nextMacro); // where does it end?
            if (endIndex < 0)
            {
                startIndex = -2;
                return retval; // no end is technically an error
            }
            var macroLen = endIndex - nextMacro - 2;
            var macroName = retval.Substring(nextMacro + 2, macroLen);
            var expand = ExpandMacroOne(macroName, macros);
            if (expand == null || expand == "")
            {
                startIndex = endIndex; // find all the things that don't expand
            }
            else
            {
                retval = retval.Substring(0, nextMacro) + expand + retval.Substring(endIndex + 2);
                startIndex = nextMacro;
            }
            return retval;
        }



        private static string ExpandListRecursive (string[] sourceList, int sourceIndex, string itemTemplate, string itemWrapTemplate, string itemListSubZero, TemplateSnippet macros, ref int count)
        {
            var source = sourceList[sourceIndex];
            var expand = "";

            var data = macros.Children[source];
            if (data.Macros.Count > 0)
            {
                foreach (var item in data.Macros)
                {
                    macros.AddMacro("TEXT", item.Value);
                    var itemExpand = Expander.ExpandMacroAll(itemTemplate, macros);
                    expand += itemExpand;
                }
            }
            else // if (data.Children.Count > 0) // Loop through the children.
            {
                // are we still in the middle of the list?
                if (sourceIndex < (sourceList.Length - 1))
                {
                    // Must keep going deeper.
                    foreach (var item in data.Children)
                    {
                        var childMacros = item.Value;
                        var itemExpand = ExpandListRecursive(sourceList, sourceIndex + 1, itemTemplate, itemWrapTemplate, itemListSubZero, childMacros, ref count);
                        expand += itemExpand;
                    }
                    // If the non-bottom items have no expansion, that's neither an error nor wrapped.
                }
                else // At the bottom; do the real expansion
                {
                    foreach (var item in data.Children)
                    {
                        var childMacros = item.Value;
                        childMacros.AddMacro("COUNT", count.ToString());
                        count++;
                        var itemExpand = Expander.ExpandMacroAll(itemTemplate, childMacros);
                        expand += itemExpand;
                    }

                    // If we're at the end of the bottom expansion, apply the code wrap.
                    if (!string.IsNullOrEmpty(itemWrapTemplate))
                    {
                        macros.AddMacro("TEXT", expand);
                        var wrapExpand = Expander.ExpandMacroAll(itemWrapTemplate, macros);
                        expand = wrapExpand;
                    }

                    if (data.Children.Count == 0)
                    {
                        if (string.IsNullOrEmpty(itemListSubZero))
                        {
                            expand = $"[[ERROR: {source} has no macros and no children]]";
                        }
                        else
                        {
                            expand = ExpandMacroAll (itemListSubZero, macros);
                        }
                    }
                }
            }

            return expand;
        }

        /// <summary>
        /// Given a bunch of templates, expand them into the macro using data from the macros
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="macros"></param>
        public static void ExpandChildTemplatesIntoMacros(TemplateSnippet parent, TemplateSnippet macros)
        {
            foreach (var kv in parent.Children)
            {
                var child = kv.Value;
                var template = child.Code;
                // Type of expansion depends on the Type=list
                string expand = $"[[ERROR failed to expand {kv.Key}]]";
                bool matchesIf = true;
                switch (child.OptionType)
                {
                    case TemplateSnippet.TypeOfExpansion.List:
                        {
                            // The template comes from the templates and the data
                            // to fill in the template comes from the Macros. It could not
                            // work any other way; the data is per-device but the 
                            // templates are not.
                            var source = child.OptionSource.Split(new char[] { '/' });
                            var count = 0;
                            //expand = ExpandList(source, child.CodeWithTrim, child.CodeWrapWithTrim, macros, out count);
                            expand = ExpandListRecursive(source, 0, child.CodeWithTrim, child.CodeWrapWithTrim, child.CodeListSubZeroWithTrim, macros, ref count);

                            if (!string.IsNullOrEmpty(child.OptionIf))
                            {
                                var exp = TemplateExpression.Parse(child.OptionIf);
                                if (exp.Error != "")
                                {
                                    expand = exp.Error;
                                    matchesIf = true;
                                }
                                else
                                {
                                    exp.SetBuiltInValue("Source.Length", count.ToString());
                                    matchesIf = exp.Eval(macros);
                                }
                            }
                            if (count == 0) // top-level expansion resulted in no results
                            {
                                if (!string.IsNullOrEmpty(child.CodeListZero))
                                {
                                    expand = child.CodeListZero; // no trim?
                                }
                            }

                        }
                        break;
                    case TemplateSnippet.TypeOfExpansion.Normal:
                        expand = ExpandMacroAll(template, macros);
                        if (!string.IsNullOrEmpty(child.OptionIf))
                        {
                            var exp = TemplateExpression.Parse(child.OptionIf);
                            if (exp.Error != "")
                            {
                                expand = exp.Error;
                                matchesIf = true;
                            }
                            else
                            {
                                matchesIf = exp.Eval(macros);
                            }
                        }
                        break;
                }
                if (matchesIf)
                {
                    macros.AddMacro(kv.Key, expand);
                }
                else
                {
                    ; // handy place to hang a debugger.
                }
            }
        }

    }
}
