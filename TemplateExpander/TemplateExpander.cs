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
        private static (string, bool) ExpandMacroOne (string macro, TemplateSnippet macros)
        {
            string retval;
            bool retstatus = true;
            if (macros == null)
            {
                retval = $"<<ERROR: unable to expand {macro}; no macros to expand with>>";
                retstatus = false;
            }
            else if (macro.StartsWith("../"))
            {
                // Expand as parent
                (retval,retstatus) = ExpandMacroOne(macro.Substring(3), macros.Parent);
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
                    retstatus = false;
                }
            }
            return (retval, retstatus);
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
            var (expand, expandok) = ExpandMacroOne(macroName, macros);
            if (!expandok)
            {
                startIndex = nextMacro+1; // find all the things that don't expand
                // In the case where the text is [[[COUNT]]], we need to expand [[COUNT]]
                // first, though, we will try to expand [COUNT from the first [[.
            }
            else
            {
                retval = retval.Substring(0, nextMacro) + expand + retval.Substring(endIndex + 2);
                startIndex = nextMacro;
            }
            return retval;
        }



        private static string ExpandListRecursive (string[] sourceList, int sourceIndex, TemplateSnippet template, TemplateSnippet macros, ref int count, ref int countAll)
        {
            var itemTemplate = template.CodeWithTrim;
            var itemWrapTemplate = template.CodeWrapWithTrim;
            var itemListSubZero = template.CodeListSubZeroWithTrim;

            var source = sourceList[sourceIndex];
            var expand = "";

            var data = macros.Children[source];
            // Not at the bottom; must keep going deeper (and will return from this code block)
            if (sourceIndex < (sourceList.Length - 1))
            {
                foreach (var item in data.Children)
                {
                    var childMacros = item.Value;
                    var itemExpand = ExpandListRecursive(sourceList, sourceIndex + 1, template, childMacros, ref count, ref countAll);
                    expand += itemExpand;
                }
                // If the non-bottom items have no expansion, that's neither an error nor wrapped.
                return expand;
            }

            // We're at the bottom of the sourceList, so do the expansion for real.
            int childCount = 0;
            int childCountAll = 0;
            if (data.Macros.Count > 0)
            {
                foreach (var item in data.Macros)
                {
                    macros.AddMacro("NAME", item.Key);
                    macros.AddMacro("TEXT", item.Value);
                    var itemExpand = Expander.ExpandMacroAll(itemTemplate, macros);
                    if (expand != "") expand += template.CodeListSeparator;
                    expand += itemExpand;
                }
                count += data.Macros.Count;
                countAll += data.Macros.Count;
                childCount += data.Macros.Count;
                childCountAll += data.Macros.Count;
            }
            else // Loop through the children, not the macros
            {
                foreach (var item in data.Children)
                {
                    var childMacros = item.Value;
                    childMacros.AddMacro("COUNT", count.ToString());
                    childMacros.AddMacro("Count.Child", childCount.ToString());
                    childMacros.AddMacro("COUNTALL", countAll.ToString());
                    childMacros.AddMacro("CountAll.Child", childCountAll.ToString());
                    bool matchesIf = true;

                    var itemExpand = Expander.ExpandMacroAll(itemTemplate, childMacros);

                    if (itemTemplate.Contains ("BREAK:"))
                    {
                        ; // handy place to hang a debugger.
                    }

                    if (!string.IsNullOrEmpty(template.OptionIf))
                    {
                        var optionIfExpression = TemplateExpression.Parse(template.OptionIf);
                        if (optionIfExpression.Error != "")
                        {
                            expand = optionIfExpression.Error;
                            matchesIf = true;
                        }
                        else
                        {
                            optionIfExpression.SetBuiltInValue("Source.Length", count.ToString());
                            matchesIf = optionIfExpression.Eval(childMacros);
                        }
                    }

                    if (matchesIf)
                    {
                        if (childCount > 0) expand += template.CodeListSeparator;
                        // Wait to bump these; items removed by the If aren't counted (except for countAll)
                        count++;
                        countAll++;
                        childCount++;
                        childCountAll++;

                        expand += itemExpand;

                        // If this is a ListOutput=child expansion, then add the expansion to the macro here.
                        if (template.OptionListOutput == TemplateSnippet.TypeOfListOutput.Child)
                        {
                            childMacros.AddMacro(template.Name, itemExpand);
                            expand = ""; // reset it
                        }
                    }
                    else
                    {
                        // countAll includes items removed from the If.
                        countAll++;
                        childCountAll++;

                        // If this is a ListOutput=child expansion, then add the expansion to the macro here.
                        if (template.OptionListOutput == TemplateSnippet.TypeOfListOutput.Child)
                        {
                            if (!string.IsNullOrEmpty(template.CodeListSubZeroWithTrim))
                            {
                                itemExpand = Expander.ExpandMacroAll(template.CodeListSubZeroWithTrim, childMacros);
                                childMacros.AddMacro(template.Name, itemExpand);
                                expand = ""; // reset it
                            }
                        }
                    }
                } // end for each child

                // If we're at the end of the bottom expansion, apply the code wrap.
                if (!string.IsNullOrEmpty(itemWrapTemplate))
                {
                    macros.AddMacro("TEXT", expand);
                    var wrapExpand = Expander.ExpandMacroAll(itemWrapTemplate, macros);
                    expand = wrapExpand;
                }
            } // end if should loop through child (not macro)

            // Handle the case of having no children. By default this is an error,
            // but the template author can fix it with CodeListSubZero
            // Child expansions don't consider this an error.
            if (childCountAll == 0 && template.OptionListOutput != TemplateSnippet.TypeOfListOutput.Child)
            {
                if (itemListSubZero == null)
                {
                    expand = $"[[ERROR: {source} has no macros and no children; parent={data.Parent.Name} template={template.Name}]]. Add CodeListSubZero item to fix?";
                }
                else
                {
                    expand = ExpandMacroAll(itemListSubZero, macros);
                }
            }

            // If this is a ListOutput=parent expansion, then add the expansion to the macro here.
            if (template.OptionListOutput == TemplateSnippet.TypeOfListOutput.Parent)
            {
                macros.AddMacro(template.Name, expand);
                expand = ""; // reset it
            }

            return expand;
        }

        private static string ZZZOrigExpandListRecursive(string[] sourceList, int sourceIndex, TemplateSnippet template, TemplateSnippet macros, ref int count, ref int countAll)
        {
            var itemTemplate = template.CodeWithTrim;
            var itemWrapTemplate = template.CodeWrapWithTrim;
            var itemListSubZero = template.CodeListSubZeroWithTrim;

            var source = sourceList[sourceIndex];
            var expand = "";

            var data = macros.Children[source];
            if (data.Macros.Count > 0)
            {
                foreach (var item in data.Macros)
                {
                    macros.AddMacro("NAME", item.Key);
                    macros.AddMacro("TEXT", item.Value);
                    var itemExpand = Expander.ExpandMacroAll(itemTemplate, macros);
                    if (expand != "") expand += template.CodeListSeparator;
                    expand += itemExpand;

                    if (item.Key == "Right")
                    {
                        ; // handy hook for a debugger.
                    }
                }
                count += data.Macros.Count;
                countAll += data.Macros.Count;
            }
            else // Loop through the children.
            {
                // are we still in the middle of the list?
                if (sourceIndex < (sourceList.Length - 1))
                {
                    // Must keep going deeper.
                    foreach (var item in data.Children)
                    {
                        var childMacros = item.Value;
                        var itemExpand = ExpandListRecursive(sourceList, sourceIndex + 1, template, childMacros, ref count, ref countAll);
                        expand += itemExpand;
                    }
                    // If the non-bottom items have no expansion, that's neither an error nor wrapped.
                }
                else // At the bottom; do the real expansion
                {
                    if (itemTemplate.Contains("BREAK:"))
                    {
                        ; // Handy place for a debugger.
                    }

                    int childCount = 0;
                    int childCountAll = 0;
                    foreach (var item in data.Children)
                    {
                        var childMacros = item.Value;
                        childMacros.AddMacro("COUNT", count.ToString());
                        childMacros.AddMacro("Count.Child", childCount.ToString());
                        childMacros.AddMacro("COUNTALL", countAll.ToString());
                        childMacros.AddMacro("CountAll.Child", childCountAll.ToString());
                        bool matchesIf = true;
                        if (childMacros.Name.Contains("Press") && childMacros.Name.Contains("Key"))
                        {
                            ;// Handy place to hang the debugger.
                        }
                        var itemExpand = Expander.ExpandMacroAll(itemTemplate, childMacros);

                        if (!string.IsNullOrEmpty(template.OptionIf))
                        {
                            var exp = TemplateExpression.Parse(template.OptionIf);
                            if (exp.Error != "")
                            {
                                expand = exp.Error;
                                matchesIf = true;
                            }
                            else
                            {
                                exp.SetBuiltInValue("Source.Length", count.ToString());
                                matchesIf = exp.Eval(childMacros);
                            }
                        }

                        if (matchesIf)
                        {
                            if (childCount > 0) expand += template.CodeListSeparator;
                            // Wait to bump these; items removed by the If aren't counted (except for countAll)
                            count++;
                            countAll++;
                            childCount++;
                            childCountAll++;

                            expand += itemExpand;

                            // If this is a ListOutput=child expansion, then add the expansion to the macro here.
                            if (template.OptionListOutput == TemplateSnippet.TypeOfListOutput.Child)
                            {
                                childMacros.AddMacro(template.Name, itemExpand);
                                expand = ""; // reset it
                            }
                        }
                        else
                        {
                            // countAll includes items removed from the If.
                            countAll++;
                            childCountAll++;

                            // If this is a ListOutput=child expansion, then add the expansion to the macro here.
                            if (template.OptionListOutput == TemplateSnippet.TypeOfListOutput.Child)
                            {
                                if (!string.IsNullOrEmpty(template.CodeListSubZeroWithTrim))
                                {
                                    itemExpand = Expander.ExpandMacroAll(template.CodeListSubZeroWithTrim, childMacros);
                                    childMacros.AddMacro(template.Name, itemExpand);
                                    expand = ""; // reset it
                                }
                            }
                        }

                    }

                    // If we're at the end of the bottom expansion, apply the code wrap.
                    if (!string.IsNullOrEmpty(itemWrapTemplate))
                    {
                        macros.AddMacro("TEXT", expand);
                        var wrapExpand = Expander.ExpandMacroAll(itemWrapTemplate, macros);
                        expand = wrapExpand;
                    }

                    // Handle the case of having no children. By default this is an error,
                    // but the template author can fix it with CodeListSubZero
                    // Child expansions don't consider this an error.
                    if (data.Children.Count == 0 && template.OptionListOutput != TemplateSnippet.TypeOfListOutput.Child)
                    {
                        if (itemListSubZero == null)
                        {
                            expand = $"[[ERROR: {source} has no macros and no children; parent={data.Parent.Name} template={template.Name}]]";
                        }
                        else
                        {
                            expand = ExpandMacroAll(itemListSubZero, macros);
                        }
                    }

                    // If this is a ListOutput=parent expansion, then add the expansion to the macro here.
                    if (template.OptionListOutput == TemplateSnippet.TypeOfListOutput.Parent)
                    {
                        macros.AddMacro(template.Name, expand);
                        expand = ""; // reset it
                    }
                }
            }

            return expand;
        }
        /// <summary>
        /// Primary template expander: given a bunch of templates, expand them into the macro using data from the macros
        /// </summary>
        /// <param name="templateParent"></param>
        /// <param name="macros"></param>
        public static void ExpandChildTemplatesIntoMacros(TemplateSnippet templateParent, TemplateSnippet macros)
        {
            foreach (var kv in templateParent.Children)
            {
                var child = kv.Value;
                var template = child.Code;

                if(child.Name.StartsWith("ZZZ"))
                {
                    continue; // ignore all templates that start with ZZZ.
                }
                if (child.Name.Contains("LINKS"))
                {
                    ;
                }
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
                            var sourceList = child.OptionSource.Split(new char[] { '/' });
                            var count = 0;
                            var countAll = 0;
                            expand = ExpandListRecursive(sourceList, 0, child, macros, ref count, ref countAll);

                            if (!string.IsNullOrEmpty(child.OptionIf) && child.OptionListOutput == TemplateSnippet.TypeOfListOutput.Parent)
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
            }
        }

    }
}
