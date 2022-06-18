using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Bytes
    {
        //TODO: actually make the conversions work!
        //  var parsedparam0 = Bytes.TryParse(IOServiceData_param0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param0);
        public byte[] Data = null;
        public static bool TryParse(string text, System.Globalization.NumberStyles style, object obj, out Bytes result)
        {
            bool retval = true;
            var split = text.Split(new char[] { ' ' });
            var bytes = new List<byte>();
            if (text != "")
            {
                foreach (var item in split)
                {
                    byte b;
                    var parsedByte = Utilities.Parsers.TryParseByte(item, style, null, out b);
                    if (!parsedByte) retval = false;
                    else bytes.Add(b);
                }
            }

            result = new Bytes() { Data = bytes.ToArray() };
            return retval;
        }

        public static implicit operator byte[](Bytes v) { return v.Data; }
        public static implicit operator Bytes(String v) { return new Bytes(); }
    }

    /// <summary>
    /// Wrapper class to make a common set of parsers for bytes, ints etc.
    /// </summary>
    public class Parsers
    {
        public static bool TryParseString(string text, System.Globalization.NumberStyles style, IFormatProvider format, out string result)
        {
            // Turns out that parsing a string from a string is trivial :-)
            result = text;
            return true;
        }

        public static bool TryParseByte(string text, System.Globalization.NumberStyles style, IFormatProvider format, out Byte result)
        {
            var ok = Byte.TryParse(text, style, format, out result);
            return ok;
        }

        public static bool TryParseSByte(string text, System.Globalization.NumberStyles style, IFormatProvider format, out SByte result)
        {
            var ok = SByte.TryParse(text, style, format, out result);
            return ok;
        }

        public static bool TryParseBytes(string text, System.Globalization.NumberStyles style, IFormatProvider format, out Bytes result)
        {
            var ok = Bytes.TryParse(text, style, format, out result);
            return ok;
        }
        public static bool TryParseSingle(string text, System.Globalization.NumberStyles style, IFormatProvider format, out Single result)
        {
            var ok = Single.TryParse(text, style, format, out result);
            return ok;
        }
        public static bool TryParseDouble(string text, System.Globalization.NumberStyles style, IFormatProvider format, out Double result)
        {
            var ok = Double.TryParse(text, style, format, out result);
            return ok;
        }
        public static bool TryParseInt16(string text, System.Globalization.NumberStyles style, IFormatProvider format, out Int16 result)
        {
            var ok = Int16.TryParse(text, style, format, out result);
            return ok;
        }
        public static bool TryParseInt32(string text, System.Globalization.NumberStyles style, IFormatProvider format, out Int32 result)
        {
            var ok = Int32.TryParse(text, style, format, out result);
            return ok;
        }
        public static bool TryParseUInt16(string text, System.Globalization.NumberStyles style, IFormatProvider format, out UInt16 result)
        {
            var ok = UInt16.TryParse(text, style, format, out result);
            return ok;
        }
        public static bool TryParseUInt32(string text, System.Globalization.NumberStyles style, IFormatProvider format, out UInt32 result)
        {
            var ok = UInt32.TryParse(text, style, format, out result);
            return ok;
        }
    }
}
