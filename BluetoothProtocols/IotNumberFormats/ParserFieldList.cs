using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols.IotNumberFormats
{
    /// <summary>
    /// Represents a parsed list of IotNumber Formats. In string form, they look like "U8|HEX|Opcode U16|DEC|Value U16|HEX|Options||0"
    /// Where it's a list of fields seperated by spaces; each field has multiple sections (type, display format, name, units, default value) 
    /// seperated by vertical bars (|); each section can have multiple sub-fields seperated by carets (^). For a small number of sub-fields,
    /// there are multiple items which are seperated by underscores (_).
    /// 
    /// see: https://shipwrecksoftware.wordpress.com/2019/10/13/modern-iot-number-formats/
    /// </summary>
    public class ParserFieldList
    {
        public IList<ParserField> Fields = new List<ParserField>();
        public Dictionary<string, ParserField> ODEs = new Dictionary<string, ParserField>();
        public static ParserFieldList ParseLine(string value)
        {
            var Retval = new ParserFieldList();


            var split = value.Split(ParserField.Space);
            for (int i = 0; i < split.Length; i++)
            {
                var item = new ParserField(split[i]);
                switch (item.FieldTypePrimary)
                {
                    case ParserField.FieldType.Regular:
                        Retval.Fields.Add(item);
                        break;
                    case ParserField.FieldType.ODE:
                        // Add to the ODEs using the field's name
                        Retval.ODEs.Add(item.NamePrimary, item);
                        break;
                }
            }

            // Now go back and fill in the RemainingSize value for the last BYTES or STRING item.
            var remainder = 0;
            for (int i = Retval.Fields.Count - 1; i >= 0; i--)
            {
                var item = Retval.Fields[i];
                if (item.NBytes >= 0) remainder += item.NBytes;
                else if (item.NBytes <= -2) remainder = -2; // meaning we couldn't calculate the amount
                else if (item.NBytes == -1)
                {
                    if (remainder <= -2)
                    {
                        // There was a error; spit it out.
                        ParserField.Log($"ERROR: ValueParserSplit:ParseLine: remainder is {remainder} at index {i}; that's a failure");
                    }
                    else if (remainder == -1)
                    {
                        // Also a error, but a different one
                        ParserField.Log($"ERROR: ValueParserSplit:ParseLine: remainder is {remainder} at index {i}");
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
    }
}
