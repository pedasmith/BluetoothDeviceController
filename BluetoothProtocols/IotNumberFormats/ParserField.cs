using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols.IotNumberFormats
{

    /// <summary>
    /// Class to perfectly parse the one field binary value descriptor strings.
    /// Simple example: "U8 U8" "U8|DEC|Temp|C U8|HEX|Mode"
    /// Complex example: "Q12Q4^_125_/|FIXED|Pressure|mbar"
    /// ODE example: ODE^Temp|U8|DEC|Temp|C
    /// Default example: "U8|HEX|Mode||FF"
    /// Three levels of splitting using space, vertical-bar (|) and caret (^) and sometimes underscore (_)
    /// Fields are ByteFormat (U8) DisplayFormat (DEC) Name (Temp) Units (c) DefaultValue (0)
    /// </summary>
    public class ParserField
    {
        public ParserField(string value)
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

        public enum FieldType {  Regular, ODE };
        public FieldType FieldTypePrimary { get; internal set; } = ParserField.FieldType.Regular;

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



        public IList<ParserField> GetXRFields(ParserFieldList pfl)
        {
            // Example: XR^Temperature_Humidity||Data would return the 'Temperature' and 'Humidity' fields
            // Those fields must have been defined by the ODE command.

            List<ParserField> retval = new List<ParserField>();
            if (FieldTypePrimary != FieldType.ODE)
            {
                Log($"ERROR: ParserField for {this.ToString()} via {pfl.ToString()} is not an ODE");
                return retval; // always an error.
            }
            var list = Get(0, 1).Split(new char[] { '_' });
            foreach (var name in list)
            {
                ParserField field;
                var hasfield = pfl.ODEs.TryGetValue(name, out field);
                if (!hasfield)
                {
                    Log($"ERROR: ParserField can't find {name} for {this.ToString()} via {pfl.ToString()} is not defined as an ODE");
                    retval.Clear();
                    return retval; // never progress with this kind of error.
                }
                else
                {
                    retval.Add(field);
                }
            }
            return retval;
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
        /// <summary>
        /// Split an entire command into fields by spaces. EG: "U8|HEX U8|HEX U16^100_/|DEC" is three fields.
        /// </summary>
        public static char[] Space = new char[] { ' ' };

        /// <summary>
        /// Split a field into sections with a bar. E.G., splitting U16^100_/|DEC into two sections 16^100_/ and DEC
        /// </summary>
        private static char[] Bar = new char[] { '|' };
        /// <summary>
        /// Split a section into subsections with a caret (^). E.G., splitting U16^100_/ into subsetions U16 and 100_/
        /// </summary>
        private static char[] Caret = new char[] { '^' };

        /// <summary>
        /// Contains all of the raw data for the fields and sub-fields. Each field is divided by a bar and then 
        /// by a caret into sub-fields.
        /// </summary>
        private string[][] SplitData = null;
        /// <summary>
        /// ParseScanResponseServiceData after the string is split into units.
        /// Example is U8|DEC|Temp|c
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private void Parse(string value)
        {
            if (value.StartsWith ("ODE")) // ODE = Option Define Element
            {
                // An ODE looks like ODE|U8|DEC|Humidity|Percent. To parse, set value to be everything
                // after the first bar and parse normally except that we also set the FieldTypePrimary to
                // ODE. That way the ParserFieldList can handle it correctly.

                FieldTypePrimary = FieldType.ODE;

                var idx = value.IndexOf(Bar[0]);
                if (idx == -1)
                {
                    // total error.
                    value = "";
                }
                else
                {
                    value = value.Substring(idx + 1);
                    // And now parse like normal!
                }
            }


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


        public override string ToString()
        {
            return $"{ByteFormatPrimary}|{DisplayFormatPrimary}|{NamePrimary}|{UnitsPrimary}|{DefaultValuePrimary}";
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
