using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateExpander
{
    class Expander
    {
        /// <summary>
        /// Given a string to expand (like CLASSNAME), expand it.
        /// </summary>
        private static string ExpandMacroOne (string macro, TemplateSnippet macros)
        {
            string retval;
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
            return retval;
        }

        /// <summary>
        /// Given a string with "some [[MACRO]] text", expand the MACRO.Only one macro will be expanded.
        /// </summary>
        /// <param name="text">Text to expand</param>
        /// <param name="startIndex">starting index; will be updated as needed</param>
        /// <param name="macros"></param>
        /// <returns></returns>
        public static string ExpandMacroTextOne(string text, int startIndex, TemplateSnippet macros)
        {
            var retval = text;
            var nextMacro = retval.IndexOf("[[", startIndex);
            if (nextMacro < 0) return retval; // no macro to expand
            var endIndex = retval.IndexOf("]]", nextMacro); // where does it end?
            if (endIndex < 0) return retval; // no end is technically an error
            var macroLen = endIndex - startIndex - 2;
            var macroName = retval.Substring(startIndex + 2, macroLen);
            var expand = ExpandMacroOne(macroName, macros);
            retval = retval.Substring(0, nextMacro) + expand + retval.Substring(endIndex + 2);
             
            return retval;
        }
    }
}
