using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// Converts an IBuffer to a string using the given type. Default is HEX.
    /// </summary>
    /// 
    /// Decode Commands are
    /// String[^ASCII|^Eddystone]
    /// BYTES[|HEX]
    /// Numbers are:
    /// 
    /// {I|U}{8|16|24|32}[|HEX||DEC||OCT||BIN]
    /// /{I|U}{8|16|24|32}/{I|U|P}{8|16|32}.Fixed
    ///     When given a P, the /P8 (for example) is taken to be a decimal amount
    ///     P8 == 100 P16 == 10000 P32 = 100000000
    /// 
    /// Commands can be seperated with spaces
    /// U8 U16
    /// 
    /// All commands can include a name and type
    /// I8|DEC|ColorRed|%
    /// 
    /// OEL OEB for option endian little and big 
    /// 
    /// Not implemented:
    /// .ASCII versus .Eddystone
    /// .OCT and .BIN
    /// 

    public class ValueParserResult
    {
        public static ValueParserResult CreateError(string str, string error)
        {
            return new ValueParserResult()
            {
                Result = ResultValues.Error,
                UserString = str,
                ErrorString = error,
            };
        }
        public static ValueParserResult CreateOk(string str, BCBasic.BCValueList valueList = null)
        {
            return new ValueParserResult()
            {
                Result = ResultValues.Ok,
                UserString = str,
                ValueList = valueList,
            };
        }
        public enum ResultValues {  Ok, Error };
        public ResultValues Result = ResultValues.Ok;
        /// <summary>
        /// The nice decoded value. If there was an error, this might bee ??-??
        /// </summary>
        public string UserString = "??-??";
        /// <summary>
        /// The error string from any errors encountere while parsing; otherwise just blank.
        /// </summary>
        public string ErrorString = "";
        public IList<byte> ByteResult = null;
        /// <summary>
        /// Either the error string or the user string depending on whether there was an error or not.
        /// </summary>
        public string AsString {  get { if (ErrorString == "") return UserString; return ErrorString; } }
        //NOTE: can also add in e.g. 
        /// <summary>
        /// List of name/value pairs in BC BASIC format (strings/doubles)
        /// </summary>
        public BCBasic.BCValueList ValueList = new BCBasic.BCValueList();
    }

    /// <summary>
    /// Class to perfectly parse the binary value descriptor strings.
    /// Simple example: "U8 U8" "U8|DEC|Temp|C U8|HEX|Mode"
    /// Complex example: "Q12Q4^/125|FIXED|Pressure|mbar"
    /// Three levels of splitting using space, vertical-bar (|) and caret (^)
    /// </summary>
    public class ValueParserSplit
    {
        ValueParserSplit(string value)
        {
            Parse(value);
        }
        public string ByteFormatPrimary { get; set; } = "U8";
        /// <summary>
        /// Examples: HEX DEC FIXED STRING 
        /// </summary>
        public string DisplayFormatPrimary { get; set; } = "";
        public string NamePrimary { get; set; } = "";
        public string UnitsPrimary { get; set; } = "";
        /// <summary>
        /// Universal GET by index. Get (0,0) is the same as getting the ByteFormatPrimary
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public string Get(int index1, int index2)
        {
            if (index1 == 0 && index2 == 0) return ByteFormatPrimary;
            if (index1 == 1 && index2 == 0) return DisplayFormatPrimary;
            if (index1 == 2 && index2 == 0) return NamePrimary;
            if (index2 == 3 && index2 == 0) return UnitsPrimary;
            if (index1 >= SplitData.Length) return "";
            if (index2 >= SplitData[index1].Length) return "";
            return SplitData[index1][index2];
        }
        private static char[] Space = new char[] { ' ' };
        private static char[] Bar = new char[] { '|' };
        private static char[] Caret = new char[] { '^' };


        private string[][] SplitData = null;
        /// <summary>
        /// Parse after the string is split into units
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Parse (string value)
        {
            var barsplit = value.Split(Bar);
            SplitData = new string[barsplit.Length][];
            for (int i=0; i<barsplit.Length; i++)
            {
                SplitData[i] = barsplit[i].Split(Caret);
                switch (i)
                {
                    case 0: ByteFormatPrimary = SplitData[i][0]; break;
                    case 1: DisplayFormatPrimary = SplitData[i][0]; break;
                    case 2: NamePrimary = SplitData[i][0]; break;
                    case 3: UnitsPrimary = SplitData[i][0]; break;
                }
            }
        }

        public static IList<ValueParserSplit> ParseLine(string value)
        {
            var Retval = new List<ValueParserSplit>();
            var split = value.Split(Space);
            for (int i=0; i<split.Length; i++)
            {
                var item = new ValueParserSplit(split[i]);
                Retval.Add(item);
            }
            return Retval;
        }
    }
    public static class ValueParser
    {
        /// <summary>
        /// Given a byte buffer of data and a string of decode commands (like "U8|DEC I8|HEX") produce a string representation of the byte buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="decodeCommands"></param>
        /// <returns></returns>
        public static ValueParserResult Parse(IBuffer buffer, string decodeCommands)
        {
            var dr = DataReader.FromBuffer(buffer);
            dr.ByteOrder = ByteOrder.LittleEndian; // default to little endian because it's common for bluetooth/
            return ConvertHelper(dr, decodeCommands);
        }
        enum ResultState { NoResult, IsString, IsDouble };

        private static string DoubleToString(double dvalue, string displayFormat, string displayFormatSecondary, string fixedFormat="F2", string hexFormat="X6", string decFormat="D")
        {
            switch (displayFormat)
            {
                case "":
                case "FIXED":
                    if (!String.IsNullOrEmpty(displayFormatSecondary)) fixedFormat = displayFormatSecondary;
                    return dvalue.ToString(fixedFormat);
                case "HEX":
                    if (!String.IsNullOrEmpty(displayFormatSecondary)) hexFormat = displayFormatSecondary;
                    return ((int)dvalue).ToString(hexFormat);
                case "DEC":
                    if (!String.IsNullOrEmpty(displayFormatSecondary)) decFormat = displayFormatSecondary;
                    return ((int)dvalue).ToString(decFormat);
                case "Speciality":
                    switch (displayFormatSecondary)
                    {
                        case "Appearance":
                            return BluetoothDefinitionLanguage.BluetoothAppearance.AppearaceToString((ushort)dvalue);
                    }
                    break;
            }
            return null; // null means we couldn't do the conversion
        }

        private static ValueParserResult ConvertHelper(DataReader dr, string decodeCommands)
        {
            var str = "";
            var vps = ValueParserSplit.ParseLine(decodeCommands);
            var valueList = new BCBasic.BCValueList();
            bool isOptional = false;

            for (int i = 0; i < vps.Count; i++)
            {
                var stritem = "";

                var command = vps[i];
                var readcmd = command.ByteFormatPrimary;
                var readindicator = readcmd[0];
                var displayFormat = command.DisplayFormatPrimary;
                var displayFormatSecondary = command.Get(1, 1);

                var name = command.NamePrimary;
                if (string.IsNullOrEmpty(name)) name = $"param{i}";
                var units = command.UnitsPrimary;

                var resultState = ResultState.IsDouble; // the most common result
                double dvalue = double.NaN;
                try
                {
                    switch (readindicator)
                    {
                        case 'F': // FLOAT
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i+1}");
                                }
                            }
                            switch (readcmd)
                            {
                                case "F32":
                                    {
                                        dvalue = dr.ReadSingle();
                                        if (displayFormat == "") displayFormat = "N3";
                                        stritem = dvalue.ToString(displayFormat); // e.g. N3 for 3 digits
                                    }
                                    break;
                                case "F64":
                                    {
                                        dvalue = dr.ReadDouble();
                                        if (displayFormat == "") displayFormat = "N3";
                                        stritem = dvalue.ToString(displayFormat); // e.g. N3 for 3 digits
                                    }
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Float command unrecognized; should be F32 or F64 not {readcmd}");
                            }
                            break;
                        case 'I':
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            switch (readcmd)
                            {
                                case "I8":
                                case "I16":
                                case "I24":
                                case "I32":
                                    {
                                        string floatFormat = "F2";
                                        string intFormat = "X6";
                                        switch (readcmd)
                                        {
                                            case "I8":
                                                {
                                                    var value = (sbyte)dr.ReadByte();
                                                    dvalue = (double)value;
                                                }
                                                break;
                                            case "I16":
                                                {
                                                    var value = dr.ReadInt16();
                                                    dvalue = (double)value;
                                                }
                                                break;
                                            case "I24":
                                                {
                                                    var b0 = dr.ReadByte();
                                                    var b1 = dr.ReadByte();
                                                    var b2 = dr.ReadByte();
                                                    var msb = (sbyte)(dr.ByteOrder == ByteOrder.BigEndian ? b0 : b2);
                                                    var lsb = dr.ByteOrder == ByteOrder.BigEndian ? b2 : b0;
                                                    int value = (int)(msb << 16) + (b1 << 8) + (lsb);
                                                    dvalue = (double)value;
                                                }
                                                break;
                                            case "I32":
                                                {
                                                    var value = dr.ReadInt32();
                                                    dvalue = (double)value;
                                                    intFormat = "X8";
                                                }
                                                break;
                                        }
                                        string calculateCommand = command.Get(0, 1); // e.g. for I24^100_/ for TI 1350 barometer values
                                        if (!string.IsNullOrEmpty(calculateCommand))
                                        {
                                            dvalue = ValueCalculate.Calculate(calculateCommand, dvalue);
                                            if (double.IsNaN(dvalue))
                                            {
                                                return ValueParserResult.CreateError(decodeCommands, $"Calculation failed for {calculateCommand} in {readcmd}");
                                            }
                                            else
                                            {
                                                // Everything worked and got a value
                                                stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary);
                                                if (stritem == null)
                                                {
                                                    return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (displayFormat == "") displayFormat = "HEX";
                                            stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary, floatFormat, intFormat);
                                            if (stritem == null)
                                            {
                                                return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                            }
                                        }

                                    }
                                    break;

                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Integer command unrecognized; should be I8/16/24/32 not {readcmd}");
                            }
                            break;
                        case 'O':
                            switch (readcmd)
                            {
                                case "OEB":
                                    resultState = ResultState.NoResult;
                                    dr.ByteOrder = ByteOrder.LittleEndian;
                                    break;
                                case "OEL":
                                    resultState = ResultState.NoResult;
                                    dr.ByteOrder = ByteOrder.LittleEndian;
                                    break;
                                case "OOPT":
                                    isOptional = true;
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Optional command unrecognized; should be OEL or OEB not {readcmd}");
                            }
                            break;
                        case 'Q':
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            // e.g. Q12Q4.Fixed
                            {
                                var subtypes = readcmd.Split(new char[] { 'Q' });
                                if (subtypes.Length != 3) // Actually 2, but first is blank
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Q command unrecognized; wrong number of Qs {readcmd}");
                                }
                                stritem = FixedQuotientHelper(dr, subtypes[1], subtypes[2], displayFormat, out dvalue);
                                //NOTE: fail on failure
                            }
                            break;
                        case 'U':
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            switch (readcmd)
                            {
                                case "U8":
                                case "U16":
                                case "U24":
                                case "U32":
                                    string xfmt = "X2";
                                    switch (readcmd)
                                    {
                                        case "U8":
                                            {
                                                var value = dr.ReadByte();
                                                dvalue = (double)value;
                                                xfmt = "X2";
                                            }
                                            break;
                                        case "U16":
                                            {
                                                var value = dr.ReadUInt16();
                                                dvalue = (double)value;
                                                xfmt = "X4";
                                            }
                                            break;
                                        case "U24":
                                            {
                                                var b0 = dr.ReadByte();
                                                var b1 = dr.ReadByte();
                                                var b2 = dr.ReadByte();
                                                var msb = (byte)(dr.ByteOrder == ByteOrder.BigEndian ? b0 : b2);
                                                var lsb = dr.ByteOrder == ByteOrder.BigEndian ? b2 : b0;
                                                int value = (int)(msb << 16) + (b1 << 8) + (lsb);
                                                dvalue = (double)value;
                                            }
                                            break;
                                        case "U32":
                                            {
                                                var value = dr.ReadUInt32();
                                                dvalue = (double)value;
                                                xfmt = "X8";
                                            }
                                            break;
                                    }
                                    string calculateCommand = command.Get(0, 1); // e.g. for I24^100_/ for TI 1350 barometer values
                                    if (!string.IsNullOrEmpty(calculateCommand))
                                    {
                                        dvalue = ValueCalculate.Calculate(calculateCommand, dvalue);
                                        if (double.IsNaN(dvalue))
                                        {
                                            return ValueParserResult.CreateError(decodeCommands, $"Calculation failed for {calculateCommand} in {readcmd}");
                                        }
                                        else
                                        {
                                            stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary);
                                            if (stritem == null)
                                            {
                                                return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (displayFormat == "") displayFormat = "HEX";
                                        stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary, "F2", xfmt);
                                        if (stritem == null)
                                        {
                                            return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized;\nshould be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                        }
                                    }
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"UInteger command unrecognized;\nshould be U8/U16/U24/U32 not {readcmd}");
                            }
                            break;
                        case '/':
                            // e.g. /U8/I16|Fixed
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            {
                                var subtypes = readcmd.Split(new char[] { '/' });
                                if (subtypes.Length != 3) // Actually 2, but first is blank
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"/ command unrecognized; wrong number of slashes {readcmd}");
                                }
                                stritem = FixedFractionHelper(dr, subtypes[1], subtypes[2], displayFormat, out dvalue);
                                // NOTE: fail on failure
                            }
                            break;
                        default:
                            if (readcmd != readcmd.ToUpper())
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR: readcmd {readcmd} but should be uppercase");
                            }
                            switch (readcmd.ToUpper()) //NOTE: should be all-uppercase; any lowercase is wrong
                            {
                                case "STRING":
                                    {
                                        try
                                        {
                                            stritem = dr.ReadString(dr.UnconsumedBufferLength);
                                            switch (displayFormat)
                                            {
                                                case "":
                                                case "ASCII":
                                                    stritem = EscapeString(stritem);
                                                    break;
                                                case "Eddystone":
                                                    stritem = BluetoothDefinitionLanguage.Eddystone.EddystoneUrlToString(stritem);
                                                    break;
                                                default:
                                                    return ValueParserResult.CreateError(decodeCommands, $"Unknown string format type {displayFormat}");

                                            }
                                        }
                                        catch (Exception)
                                        {
                                            // The string can't be read. Let's try reading as a buffer instead.
                                            IBuffer buffer = dr.ReadBuffer(dr.UnconsumedBufferLength);
                                            for (uint ii = 0; ii < buffer.Length; ii++)
                                            {
                                                if (ii != 0) stritem += " ";
                                                stritem += buffer.GetByte(ii).ToString("X2");
                                            }
                                        }
                                        resultState = ResultState.IsString;
                                    }
                                    break;
                                case "BYTES":
                                    {
                                        IBuffer buffer = dr.ReadBuffer(dr.UnconsumedBufferLength);
                                        for (uint ii=0; ii<buffer.Length; ii++)
                                        {
                                            if (ii != 0) stritem += " ";
                                            stritem += buffer.GetByte(ii).ToString("X2");
                                        }
                                        resultState = ResultState.IsString;
                                    }
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Other command unrecognized; should be String or Bytes {readcmd}");
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    stritem = $"EXCEPTION reading data {e} index {i} command {command} len {dr.UnconsumedBufferLength}";
                    return ValueParserResult.CreateError(str + stritem, stritem);
                }
                switch (resultState)
                {
                    case ResultState.IsDouble:
                        valueList.SetProperty(name, new BCBasic.BCValue(dvalue));
                        break;
                    case ResultState.IsString:
                        valueList.SetProperty(name, new BCBasic.BCValue(stritem));
                        break;
                }

                if (str != "") str += " ";
                str += stritem;

                if (dr.UnconsumedBufferLength <= 0)
                {
                    break;
                }

            }
            return ValueParserResult.CreateOk(str, valueList);
        }

        // Command is e.g. U8 or I32 or P8
        private static double ReadValue(DataReader dr, string command)
        {
            double Retval = Double.NaN;
            switch (command)
            {
                case "I8":
                    Retval = (sbyte)dr.ReadByte();
                    break;
                case "I16":
                    Retval = dr.ReadInt16();
                    break;
                case "I32":
                    Retval = dr.ReadInt32();
                    break;

                case "P8":
                    Retval = (sbyte)dr.ReadByte();
                    break;
                case "P16":
                    Retval = dr.ReadInt16();
                    break;
                case "P32":
                    Retval = dr.ReadInt32();
                    break;

                case "U8":
                    Retval = dr.ReadByte();
                    break;
                case "U16":
                    Retval = dr.ReadUInt16();
                    break;
                case "U32":
                    Retval = dr.ReadUInt32();
                    break;
            }
            return Retval;
        }


        /// <summary>
        /// Handles /I16/U8.FIXED type requests. The I16 is the integer part and the U8 is the fraction. The fraction for U and I is binary and for P types is decimal (e.g. P8 is an out-of-100 portion)
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="integralType"></param>
        /// <param name="fractionType"></param>
        /// <param name="presentation"></param>
        /// <returns></returns>
        private static string FixedFractionHelper(DataReader dr, string integralType, string fractionType, string presentation, out double dvalue)
        {
            string stritem;
            var integral = ReadValue(dr, integralType);
            var fraction = ReadValue(dr, fractionType);
            var denominator = 1.0;
            switch (fractionType)
            {
                case "I8":
                case "U8":
                    denominator = 1 << 8;
                    break;
                case "P8":
                    denominator = 100;
                    break;
                case "I16":
                case "U16":
                    denominator = 1 << 16;
                    break;
                case "P16":
                    denominator = 10000; //  1 << 16;
                    break;
                case "I32":
                case "U32":
                    denominator = 1 << 32;
                    break;
                case "P32":
                    denominator = 1000000; //  1 << 32;
                    break;

            }
            switch (presentation)
            {
                default: // NOTE: error!
                case "FIXED":
                    if (fraction > denominator)
                    {
                        ; //TODO: failure
                    }
                    dvalue = integral + (fraction / denominator);
                    stritem = dvalue.ToString();
                    break;
                    // NOTE: need to add a failure path pretty much everywhere.
            }
            return stritem;
        }
        private static string FixedQuotientHelper(DataReader dr, string integralType, string fractionType, string presentation, out double dvalue)
        {
            dvalue = double.NaN;
            string stritem;
            var integralBits = Int32.Parse(integralType);
            var fractionBits = Int32.Parse(fractionType);
            var nbits = integralBits + fractionBits;
            Int32 rawValue = 0;
            switch (nbits)
            {
                default:
                    return $"ERROR: incorrect Qbit encoding {integralType}Q{fractionType}";
                case 8: rawValue = (sbyte)dr.ReadByte(); break;
                case 16: rawValue = dr.ReadInt16(); break;
                case 32: rawValue = dr.ReadInt32(); break;
            }
            string idealFormat = "N";
            // How many bits of precision?
            // 3--> 1 chars
            // 4-->2 chars
            var maxSize = 1<<fractionBits; // e.g. 10-->1024
            var displaybits = Math.Ceiling(Math.Log10(maxSize));
            idealFormat = $"N{displaybits}";

            // An example: we get a number in 12Q4 format.
            // That means that the top 12 bits are the integer part
            // and the bottom 4 are the fractional part.
            // This is actually a simple calculation!
            // The divideBy is 1<<4 (remember that 1<<0==1, 1<<1==2, 1<<2==4 1<<3=8 1<<4==16)
            double divideBy = (double)(1 << fractionBits);
            dvalue = ((double)rawValue) / divideBy;

            switch (presentation)
            {
                default: // TODO: error!
                case "FIXED":
                    stritem = dvalue.ToString(idealFormat);
                    break;
                    // TODO: need to add a failure path pretty much everywhere.
            }
            return stritem;
        }

        public static string ConvertToStringHex(IBuffer buffer)
        {
            var str = "";
            for (uint i = 0; i < buffer.Length; i++)
            {
                if (str != "") str += " ";
                str += $"{buffer.GetByte(i):X2}";
            }
            return str;
        }


        private static string EscapeString(string rawstr)
        {
            rawstr = rawstr.Replace("\\", "\\\\"); // escape all back-slashes
            rawstr = rawstr.Replace("\0", "\\0"); // replace a real NUL with an escaped null.
            rawstr = rawstr.Replace("\r", "\\r");
            rawstr = rawstr.Replace("\n", "\\n");
            rawstr = rawstr.Replace("\v", "\\v");
            return rawstr;
        }

        public static ValueParserResult ConvertToBuffer (this string value, string type)
        {
            var Retval = new ValueParserResult();
            Retval.ByteResult = new List<byte>() { Capacity = (value.Length + 1) / 3 }; // most likely size
            type = type.ToUpper();
            switch (type)
            {
                case "ASCII":
                    foreach (var ch in value)
                    {
                        Retval.ByteResult.Add((byte)(ch & 0xFF)); //just whack them.
                    }
                    break;
                case "DEC":
                case "HEX":
                    var items = value.Split(" ");
                    foreach (var item in items)
                    {
                        byte b = 0;
                        bool converted;

                        var specifier = System.Globalization.NumberStyles.None;
                        if (type == "HEX")
                        {
                            specifier = System.Globalization.NumberStyles.AllowHexSpecifier;
                        }
                        converted = byte.TryParse(item, specifier, null, out b);
                        if (converted)
                        {
                            Retval.ByteResult.Add(b);
                        }
                        else
                        {
                            Retval.Result = ValueParserResult.ResultValues.Error;
                            Retval.ErrorString += $"Item {item} could not be converted as {type}\n";
                        }
                    }
                    break;
            }
            return Retval;
        }
    }
}
