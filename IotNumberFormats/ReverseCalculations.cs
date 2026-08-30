using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IotNumberFormats
{
    internal static class ReverseCalculations
    {
        /// <summary>
        /// Given a compiled ParserFieldList and a string, return a list of bytes. The ParserFieldList
        /// supports only a subset of the full parserFieldList values
        /// As of 2026-08-25, supported values are only U8 and BYTES
        /// The input string will have a format like "FF 567 32.23 __" (U8|BYTE U16|DEC F32|FIXED U8|BYTE||CheckSum^iHealth)
        /// ParserFields which have checksums will replace as apppropriate. 
        /// </summary>
        public static List<byte> StringToBytes(ParserFieldList compiled, string str)
        {
            var retval = new List<byte>();
            var values = str.Split(" ");
            int valueIndex = 0;
            for (int fieldIndex=0; fieldIndex<compiled.Fields.Count; fieldIndex++)
            {
                var field = compiled.Fields[fieldIndex];
                if (field.DefaultValuePrimary.StartsWith("Update"))
                {
                    switch (field.DefaultValuePrimary)
                    {
                        case "UpdateiHealthChecksumAtEnd":
                            {
                                retval.Add(0); // The UpdateiHealthChecksumAtEnd assumes we've already added the byte.
                                var bytes = retval.ToArray();
                                Utilities.CrcCalculations.UpdateiHealthChecksumAtEnd(bytes);
                                retval = bytes.ToList();
                            }
                            break;
                        default:
                            Log($"Error: ReverseCalculation: unknown default {field.DefaultValuePrimary}");
                            break;
                    }
                }
                else
                {
                    var nextValue = values[valueIndex++];
                    if (nextValue == "__")
                    {
                        nextValue = field.DefaultValuePrimary;
                        if (nextValue == "")
                        {
                            Log($"Error: ReverseCalculation: user provided __ but default is blank");
                        }
                    }
                    switch (field.ByteFormatPrimary)
                    {
                        case "BYTES":
                            // We can't just add everything because there might be somethingn else at the end
                            {
                                // remember that valueIndex has already been incremented
                                // Example: values.Length is 7 and valueIndex = 5.
                                // If BYTES is the last item, we should add in values indexed 4, 5, 6
                                // 4 is the first byte (before valueIndex was incremented) and 6 is the last index
                                // If MaxBytesRemaining is > 0, decrement 1-to-1

                                var ntofill = values.Length - (valueIndex - 1);
                                if (field.MaxBytesRemaining > 0) ntofill -= field.MaxBytesRemaining;
                                for (int bytecount = 0; bytecount < ntofill; bytecount++)
                                {
                                    switch (field.DisplayFormatPrimary)
                                    {
                                        case "HEX":
                                            retval.Add(Convert.ToByte(nextValue, 16));
                                            break;
                                        case "DEC":
                                            retval.Add(Convert.ToByte(nextValue));
                                            break;
                                    }
                                    if (valueIndex <  values.Length) nextValue = values[valueIndex++];
                                }
                            }
                            break;
                        case "U8":
                            switch (field.DisplayFormatPrimary)
                            {
                                case "HEX":
                                    retval.Add(Convert.ToByte(nextValue, 16));
                                    break;
                                case "DEC":
                                    retval.Add(Convert.ToByte(nextValue));
                                    break;
                            }
                            break;
                    }
                }
            }
            return retval;
        }

        static void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str); 
        }

        static int TestOne(string toBeCompiled, string original, string expected = "")
        {
            int nerror = 0;
            if (expected == "") expected = original;

            // Step one: turn the original string into bytes
            var compiled = ParserFieldList.ParseLine(toBeCompiled);
            var bytes = StringToBytes(compiled, original);

            // Step two: turn the bytes back into a string.
            var vpr = new ValueParser(toBeCompiled);
            var actual = vpr.Parse(bytes.ToArray());

            var actualList = actual.UserString.Split(" ");
            var expectedList = expected.Split(" ");

            if (actualList.Length != expectedList.Length)
            {
                Log($"Error: IOT: Original string '{expected}' has {expectedList.Length} elements, but actual string '{actual.UserString}' has {actualList.Length} elements");
                nerror++;
            }
            else
            {
                for (int i = 0; i < actualList.Length; i++)
                {
                    if (actualList[i] != expectedList[i])
                    {
                        Log($"Error: IOT: Original[{i}]'{expectedList[i]}' does not match actual string element '{actualList[i]}' from {expected}");
                        nerror++;
                    }
                }
            }

            return nerror;
        }

        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne("U8|HEX", "FF");
            nerror += TestOne("U8|HEX U8|DEC", "FF 10");
            nerror += TestOne("U8|HEX|TransmitHead||B0 U8|HEX|Length U8|HEX|Zero||0 U8|HEX|Sequence BYTES|HEX|Command U8|HEX|Checksum||UpdateiHealthChecksumAtEnd",
                "__ 04 __ 01 F0 D1 __", "B0 04 00 01 F0 D1 C2");
            return nerror;
        }



    }
}
