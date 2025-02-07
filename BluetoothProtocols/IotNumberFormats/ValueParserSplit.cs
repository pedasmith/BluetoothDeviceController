using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols.IotNumberFormats
{
    /// <summary>
    /// Class to perfectly parse the binary value descriptor strings.
    /// Simple example: "U8 U8" "U8|DEC|Temp|C U8|HEX|Mode"
    /// Complex example: "Q12Q4^_125_/|FIXED|Pressure|mbar"
    /// Default example: "U8|HEX|Mode||FF"
    /// Three levels of splitting using space, vertical-bar (|) and caret (^)
    /// </summary>
    public class ValueParserSplit
    {
        ValueParserSplit(string value)
        {
            Parse(value);
        }
        /// <summary>
        /// Examples: U8 F32 BYTES STRING
        /// </summary>
        public string ByteFormatPrimary { get; set; } = "U8";
        /// <summary>
        /// Examples: HEX DEC FIXED STRING 
        /// </summary>
        public string DisplayFormatPrimary { get; set; } = "";
        
        public string NamePrimary { get; set; } = "";
        public string UnitsPrimary { get; set; } = "";
        public string DefaultValuePrimary { get; set; } = "*";

        /// <summary>
        ///  Number of bytes this value uses or -1 for BYTES or STRING (and -2 when not set like the Q format)
        /// </summary>
        public int NBytes { get; set; } = 1;
        /// <summary>
        /// Set to >0 when the last BYTES or STRING is followed by a series of fixed-width fields (which is most of them). -1 for BYTES or STRING means to fill it in with the rest of the data.
        /// </summary>
        public int MaxBytesRemaining { get; set; } = -1; 
        /// <summary>
        /// Universal GET by index. Get (0,0) is the same as getting the ByteFormatPrimary. Invalid indexes return ""
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns>either the value or "" for indexes that are out of range</returns>
        public string Get(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0) return "";
            if (index1 == 0 && index2 == 0) return ByteFormatPrimary;
            if (index1 == 1 && index2 == 0) return DisplayFormatPrimary;
            if (index1 == 2 && index2 == 0) return NamePrimary;
            if (index1 == 3 && index2 == 0) return UnitsPrimary;
            if (index2 == 4 && index2 == 0) return DefaultValuePrimary;
            if (index1 >= SplitData.Length) return "";
            if (index2 >= SplitData[index1].Length) return "";
            return SplitData[index1][index2];
        }

        public bool IsHidden
        {
            get
            {
                var hidden = Get(1, 2); // e.g., U8|HEX^^HIDDEN|Opcode
                if (hidden == "HIDDEN") return true;
                else if (hidden == "") return false;
                Log("ERROR: unknown hidden type {hidden}");
                return false;
            }
        }
        private static char[] Space = new char[] { ' ' };
        private static char[] Bar = new char[] { '|' };
        private static char[] Caret = new char[] { '^' };


        private string[][] SplitData = null;
        /// <summary>
        /// ParseScanResponseServiceData after the string is split into units.
        /// Example is U8|DEC|Temp|c
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private void Parse(string value)
        {
            var barsplit = value.Split(Bar);
            SplitData = new string[barsplit.Length][];
            for (int i = 0; i < barsplit.Length; i++)
            {
                SplitData[i] = barsplit[i].Split(Caret);
                switch (i)
                {
                    case 0: ByteFormatPrimary = SplitData[i][0]; break;
                    case 1: DisplayFormatPrimary = SplitData[i][0]; break;
                    case 2: NamePrimary = SplitData[i][0]; break;
                    case 3: UnitsPrimary = SplitData[i][0]; break;
                    case 4: DefaultValuePrimary = SplitData[i][0]; break;
                }
            }

            NBytes = ParseNBytes(ByteFormatPrimary); 
        }

        private static int ParseNBytes(string byteFormatPrimary)
        {
            if (string.IsNullOrEmpty (byteFormatPrimary))
            {
                return 0; // does it matter what gets returned? This is actually a common case
                // when we're reading a new device with no specialization or JSON.
            }
            int retval = -2; // failure
            switch (byteFormatPrimary[0])
            {
                case 'F':
                case 'I':
                case 'U':
                    int len;
                    if (Int32.TryParse(byteFormatPrimary.Substring(1), out len))
                    {
                        retval = len / 8;
                    }
                    break;
                case 'O': // All the optional values are zero long
                    retval = 0;
                    break;
                default:
                    if (byteFormatPrimary == "STRING" || byteFormatPrimary == "BYTES")
                    {
                        retval = -1;
                    }
                    break;
            }
            return retval;
        }

        public static IList<ValueParserSplit> ParseLine(string value)
        {
            var Retval = new List<ValueParserSplit>();
            var split = value.Split(Space);
            for (int i = 0; i < split.Length; i++)
            {
                var item = new ValueParserSplit(split[i]);
                Retval.Add(item);
            }

            // Now go back and fill in the RemainingSize value for the last BYTES or STRING item.
            var remainder = 0;
            for (int i=Retval.Count - 1; i>= 0; i--)
            {
                var item = Retval[i];
                if (item.NBytes >= 0) remainder += item.NBytes;
                else if (item.NBytes <= -2) remainder = -2; // meaning we couldn't calculate the amount
                else if (item.NBytes == -1)
                {
                    if (remainder <= -2)
                    {
                        // There was a error; spit it out.
                        Log($"ERROR: ValueParserSplit:ParseLine: remainder is {remainder} at index {i}; that's a failure");
                    }
                    else if (remainder == -1)
                    {
                        // Also a error, but a different one
                        Log($"ERROR: ValueParserSplit:ParseLine: remainder is {remainder} at index {i}");
                    }
                    else if (remainder > 0)
                    {
                        item.MaxBytesRemaining = remainder; // if the BYTE is the last item, keep MaxBytesRemaining the same.
                    }
                    break; // Only fill in one value.
                }
            }

            return Retval;
        }

        public override string ToString()
        {
            return $"{ByteFormatPrimary}|{DisplayFormatPrimary}|{NamePrimary}|{UnitsPrimary}|{DefaultValuePrimary}";
        }

        private static void Log(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
