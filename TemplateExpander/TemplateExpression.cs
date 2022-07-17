using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateExpander
{
    internal class TemplateExpression
    {
        string Left { get; set; } = "";
        string Opcode { get; set; } = "";
        string Right { get; set; } = "";
        public string Error { get; internal set; } = "";

        public override string ToString()
        {
            return $"{Left} {Opcode} {Right}";
        }
        private Dictionary<string, string> BuiltInValues = new Dictionary<string, string>();
        /// <summary>
        /// Normally used to set the Source.Length value
        /// </summary>
        public void SetBuiltInValue(string name, string value)
        {
            if (BuiltInValues.ContainsKey(name)) BuiltInValues[name] = value;
            else BuiltInValues[name] = value;
        }
        public bool Eval(TemplateSnippet macros)
        {
            bool retval = false;
            var left = Expander.ExpandMacroAll(Left, macros);
            var right = Expander.ExpandMacroAll(Right, macros);
            if (BuiltInValues.ContainsKey(left)) left = BuiltInValues[left];
            if (BuiltInValues.ContainsKey(right)) right = BuiltInValues[right];

            // if the left is a number, calculate as a number;
            int leftVal, rightVal;
            bool leftIsNumber = int.TryParse(left, out leftVal);
            bool rightIsNumber = int.TryParse(left, out rightVal);
            if (leftIsNumber && rightIsNumber)
            {
                switch (Opcode)
                {
                    case "<": return leftVal < rightVal;
                    case "<=": return leftVal <= rightVal;
                    case "==": return leftVal == rightVal;
                    case "!=": return leftVal != rightVal;
                    case ">=": return leftVal >= rightVal;
                    case ">": return leftVal > rightVal;
                }
            }
            else
            {
                switch (Opcode)
                {
                    case "==": return left == right;
                    case "!=": return left != right;
                    case "contains": 
                        retval = left.ToLower().Contains(right.ToLower()); 
                        return retval;
                    case "!contains": 
                        if (right == ":")
                        {
                            ; // handy place to hang a debugger.
                        }
                        return !left.ToLower().Contains(right.ToLower());
                    case "length>":
                        return left.Length > rightVal;
                }

            }
            Console.Write($"ERROR: unknown opcode <<{Opcode}>>");
            return false;

        }
        public static TemplateExpression Parse(string expression)
        {
            var retval = new TemplateExpression();
            var list = expression.Split(new char[] { ' ' });
            if (list.Length != 3)
            {
                retval.Error = $"ERROR: invalid expression <<{expression}>>";
            }
            else
            {
                retval.Left = list[0];
                retval.Opcode = list[1];
                retval.Right = list[2];
            }
            return retval;
        }
    }
}
