using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

// I16^*500/65536.FIXED.GyroX.dps

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// Implements a tiny calculator. It's got just enough power to handle common IOT scenarios
    /// </summary>
    public static class ValueCalculate
    {
        /// <summary>
        /// Commands (like AN for an AND) are always exactly 2 uppercase letters
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool IsCommand(string cmd)
        {
            if (cmd == null) return false;
            if (cmd.Length != 2) return false;
            char c1 = cmd[0];
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
        /// Given a string like 1000_/ do a calculation. The string is split by underscore and the 'words'
        /// are RPN with values (1000), operations (AN) or opcodes (+). 1000_/ will divide the value on the 
        /// stack by 1000.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startValue"></param>
        /// <returns></returns>
        public static double Calculate (string str, double startValue)
        {
            double Retval = double.NaN;
            int index = 0;
            try
            {
                var stack = new Stack<double>();
                stack.Push(startValue);

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
                        switch (command)
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
                    else
                    {
                        double value = 0.0;
                        var parseOk = double.TryParse(command, out value);
                        if (!parseOk)
                        {
                            Error($"ERROR: ValueCalculate: not a constant {command} index {index} command {str}");
                            return double.NaN;
                        }
                        stack.Push(value);
                    }
                    index = nextindex;

                    // This is for small calculations, not giant ones!
                    if (stack.Count > 10)
                    {
                        Error($"ERROR: ValueCalculate: stack too large {stack.Count} index {index} command {str}");
                        return double.NaN;
                    }
                }


                Retval = stack.Pop();
            }
            catch (Exception)
            {
                Error($"ERROR: ValueCalculate: not enough params index {index} command {str}");
            }
            return Retval;
        }

        private static void Error (string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
            ;
        }
    }
}
