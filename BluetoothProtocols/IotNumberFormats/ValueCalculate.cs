using System;
using System.Collections.Generic;

// I16^*500/65536.FIXED.GyroX.dps

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// Implements a tiny calculator. It's got just enough power to handle common IOT scenarios
    /// </summary>
    public static class ValueCalculate
    {
        /// <summary>
        /// Commands (like AN for an AND) are always exactly 2 uppercase letters. String constants are not commands.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool IsCommand(string cmd)
        {
            if (cmd == null) return false;
            char c1 = cmd[0];
            if (cmd.Length != 2) return false;
            char c2 = cmd[1];
            var retval = (c1 >= 'A' && c1 <= 'Z' && c2 >= 'A' && c2 <= 'Z');
            return retval;
        }

        /// <summary>
        /// Operands are always exactly 1 character + - * /
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool IsOp (string cmd)
        {
            if (cmd == null) return false;
            if (cmd.Length != 1) return false;
            char c1 = cmd[0];
            var retval = c1 == '+' || c1 == '-' || c1 == '*' || c1 == '/';
            return retval;
        }

        /// <summary>
        /// Replaces chars in the string with the escaped version
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeString (string value)
        {
            var retval = value;
            retval = retval.Replace("\\", "\\\\");

            // All of the 'C' style chars
            retval = retval.Replace("\a", "\\a"); // NOT ALLOWED IN JSON
            retval = retval.Replace("\b", "\\b");
            retval = retval.Replace("\f", "\\f");
            retval = retval.Replace("\n", "\\n");
            retval = retval.Replace("\r", "\\r");
            retval = retval.Replace("\t", "\\t");
            retval = retval.Replace("\v", "\\v"); // NOT ALLOWED IN JSON
            retval = retval.Replace("'", "\\'"); // NOT ALLOWED IN JSON
            retval = retval.Replace("\"", "\\\"");
            retval = retval.Replace("?", "\\?"); // NOT ALLOWED IN JSON
            retval = retval.Replace("\0", "\\00"); // JSON REQUIRES THIS TO BE \u0000
            // JSON ALSO CAN ESCAPE /
            // JSON ALSO ALLOWS \uXXXX for arbitrary hex
            // LASTLY, JSON DISALLOWS E.G. ^A to be in the string; it must be escaped.

            // All of the special chars TODO: JSON does not permit the use of weird escapes like \s \p \c
            // TODO: Recommend using $ instead. And following exactly the JSON escape syntax.
            retval = retval.Replace(" ", "\\s");
            retval = retval.Replace("|", "\\p");
            retval = retval.Replace("^", "\\c");

            return retval;
        }
        public static string ReverseReplace(this string str, string arg1, string arg2)
        {
            return str.Replace(arg2, arg1);
        }
        public static string UnescapeString(string value)
        {
            var retval = value;
            // All of the special chars
            retval = retval.ReverseReplace("^", "\\c");
            retval = retval.ReverseReplace("|", "\\p");
            retval = retval.ReverseReplace(" ", "\\s");

            // All of the 'C' style chars
            retval = retval.ReverseReplace("\0", "\\00");
            retval = retval.ReverseReplace("?", "\\?");
            retval = retval.ReverseReplace("\"", "\\\"");
            retval = retval.ReverseReplace("'", "\\'");
            retval = retval.ReverseReplace("\v", "\\v");
            retval = retval.ReverseReplace("\t", "\\t");
            retval = retval.ReverseReplace("\r", "\\r");
            retval = retval.ReverseReplace("\n", "\\n");
            retval = retval.ReverseReplace("\f", "\\f");
            retval = retval.ReverseReplace("\b", "\\b");
            retval = retval.ReverseReplace("\a", "\\a");

            retval = retval.ReverseReplace("\\", "\\\\");

            return retval;
        }

        public class CalculateResult
        {
            public CalculateResult (double d, string s) => (D, S) = (d, s);
            public double D { get; internal set; }
            public string S { get; internal set; }
            public void Deconstruct (out double d, out string s)
            {
                (d, s) = (D, S);
            }
        }

        /// <summary>
        /// Given a string like 1000_/ do a calculation. The string is split by underscore and the 'words'
        /// are RPN with values (1000), operations (AN) or opcodes (+). 1000_/ will divide the value on the 
        /// stack by 1000.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startValue"></param>
        /// <returns></returns>
        public static CalculateResult Calculate(string str, double startValue, string startStringValue = "", VariableSet variables = null)
        {
            int index = 0;
            try
            {
                var stack = new Stack<double>();
                stack.Push(startValue);
                var stringstack = new Stack<string>();
                stringstack.Push(startStringValue); 

                var commands = str.Split(new char[] { '_' });
                index = 0;
                while (index < commands.Length)
                {
                    var command = commands[index];
                    var nextindex = index + 1;
                    if (IsCommand(command) || IsOp (command))
                    {
                        double d1;
                        double d2;
                        double value;
                        switch (command) // or op...
                        {
                            case "+":
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 + d1;
                                stack.Push(value);
                                break;
                            case "-": // 10_2_- should do 10-2
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 - d1;
                                stack.Push(value);
                                break;
                            case "*":
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 * d1;
                                stack.Push(value);
                                break;
                            case "/":
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 / d1;
                                stack.Push(value);
                                break;
                            case "AN": // bitwise AND
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) & ((int)d1);
                                stack.Push(value);
                                break;
                            case "DU": // dup
                                d1 = stack.Pop();
                                stack.Push(d1);
                                stack.Push(d1);
                                break;
                            case "GO": // go to a location
                                d1 = stack.Pop();
                                nextindex = (int)d1;
                                break;


                            case "GN": // Get a name, look up in current (new)
                                {
                                    string name = stringstack.Pop();
                                    value = variables.GetCurrDouble(name); 
                                    stack.Push(value);
                                }
                                break;
                            case "GP": // Get a name, look up in previous dictionaries
                                {
                                    string name = stringstack.Pop();
                                    value = variables.GetPreviousDouble(name);
                                    stack.Push(value);
                                }
                                break;
                            case "GS": // Get a name, look up in current (new) as a string TODO: document
                                {
                                    string name = stringstack.Pop();
                                    var valuestr = variables.GetCurrString(name);
                                    stringstack.Push(valuestr);
                                }
                                break;
                            case "GD": // Get a name, look up in current and previous dictionaries. If same, return blank; otherwise return new
                                {
                                    string name = stringstack.Pop();
                                    double prev = variables.GetPreviousDouble(name);
                                    double curr = variables.GetCurrDouble(name);
                                    if (prev == curr)
                                    {
                                        stringstack.Push("");
                                    }
                                    else
                                    {
                                        stack.Push(curr);
                                    }
                                }
                                break;
                            case "IV": // negate (inverse)
                                d1 = stack.Pop();
                                value = -d1;
                                stack.Push(value);
                                break;
                            case "JN": // Jump conditional on non-zero skip the given number of instructions 0_2_JZ jumps 2 because the zero
                                d1 = stack.Pop();
                                d2 = stack.Peek();
                                if (d2 != 0)
                                {
                                    nextindex = index + (int)d1;
                                }
                                break;
                            case "JZ": // Jump conditional on zero skip the given number of instructions 0_2_JZ jumps 2 because the zero
                                d1 = stack.Pop();
                                d2 = stack.Peek();
                                if (d2 == 0)
                                {
                                    nextindex = index + (int)d1;
                                }
                                break;
                            case "JU": // skip the given number of instructions 1_JU is like a no-op! (0_JU is infinite loop!)
                                d1 = stack.Pop();
                                nextindex = index + (int)d1;
                                break;
                            case "LS": // left shift 3_1_LS is 6 (11 -> 110)
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) << ((int)d1);
                                stack.Push(value);
                                break;
                            case "NO": // no-op
                                break;
                            case "PO": // pop
                                d1 = stack.Pop();
                                break;
                            case "RS": // right shift 5_1_RS is 2 (101 -> 10)
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) >> ((int)d1);
                                stack.Push(value);
                                break;
                            case "SW": // swap
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                stack.Push(d1);
                                stack.Push(d2);
                                break;

                            case "XY": // 10_2_XY is 2**10
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = Math.Pow(d1, d2);
                                stack.Push(value);
                                break;
                            case "YX": // 10_2_YX is 10**2
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = Math.Pow(d2, d1);
                                stack.Push(value);
                                break;
                            case "ZE": // ZEro is an easy masking operation; if <n> is on the stack then 3_ZE will clear the bottom 2 bits
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) & ~((int)d1);
                                stack.Push(value);
                                break;
                        }
                    }
                    else if (command.Length >= 1 && command[0] == '$')
                    {
                        // Is a string constant. 
                        // String constants include escaped chars like \s=SP \p=BAR (pipe) \c=CARET
                        // TODO: that's not how space etc is escaped after re-checking the JSON spec :-(
                        // and also escaped chars for \r\n
                        // They include all C escapes except for octal and \uXXXX is the code for Unicode
                        var constString = UnescapeString (command.Substring(1));
                        stringstack.Push(constString);
                    }
                    else
                    {
                        double value = 0.0;
                        var parseOk = double.TryParse(command, out value);
                        if (!parseOk)
                        {
                            Error($"ERROR: ValueCalculate: not a constant {command} index {index} command {str}");
                            return new CalculateResult(double.NaN, null);
                        }
                        stack.Push(value);
                    }
                    index = nextindex;

                    // This is for small calculations, not giant ones!
                    if (stack.Count > 10)
                    {
                        Error($"ERROR: ValueCalculate: stack too large {stack.Count} index {index} command {str}");
                        return new CalculateResult(double.NaN, null);
                    }
                }

                double d = stack.Count > 0 ? stack.Pop() : double.NaN;
                string s = stringstack.Count > 0 ? stringstack.Pop() : null;
                return new CalculateResult(d, s);
            }
            catch (Exception)
            {
                Error($"ERROR: ValueCalculate: not enough params index {index} command {str}");
                return new CalculateResult(double.NaN, null);
            }
        }

        private static void Error (string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
            ;
        }
    }
}
