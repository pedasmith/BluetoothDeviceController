using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BluetoothWinUI3.Utilities
{
    public static class StringUtilities
    {
        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }
        private static int TestStarMatchOne(string str, string match, bool expected)
        {
            int nerror = 0;
            if (str.StarMatch(match) != expected)
            {
                Log($"Error: '{str}' should {(expected ? "" : "not ")}match '{match}'");
                nerror++;
            }
            return nerror;
        }
        public static int TestStarMatch()
        {
            int nerror = 0;
            nerror += TestStarMatchOne("hello world", "hello*", true);
            nerror += TestStarMatchOne("hello", "hello*", true);
            nerror += TestStarMatchOne("he hello", "hello*", false);

            // TODO: test more corner cases, such as multiple stars, stars at the beginning or end, etc.
            return nerror;
        }
        public static bool StarMatch(this string str, string pattern, StringComparison compare = StringComparison.OrdinalIgnoreCase)
        {
            if (pattern == "*")
            {
                return true;
            }
            var patternParts = pattern.Split('*');
            int currentIndex = 0;
            for (int partIndex = 0; partIndex < patternParts.Length; partIndex++)
            {
                var part = patternParts[partIndex];
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }
                int foundIndex = str.IndexOf(part, currentIndex, compare);
                if (foundIndex == -1)
                {
                    return false;
                }
                if (partIndex == 0 && !pattern.StartsWith("*") && foundIndex != 0)
                {
                    // StarMatch ("hello world", "hello*", true) should match, but StarMatch ("he hello", "hello*", false) should not match
                    return false;
                }
                currentIndex = foundIndex + part.Length;
            }
            return true;
        }
    }
}
