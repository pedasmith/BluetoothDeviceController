using BluetoothDeviceController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Windows.Storage.Streams;
using static Utilities.DataReaderReadStringRobust;
using BluetoothProtocols.IotNumberFormats;

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// Converts an IBuffer to a string using the given type. Default is HEX.
    /// </summary>
    /// 
    /// See https://shipwrecksoftware.wordpress.com/2019/10/13/modern-iot-number-formats/ for details
    /// 
    // Description is Field [SP Field]*
    // Field is Format|Display|Name|Units|DefaultValue  e.g. U8|HEX|Green
    // Format is format [^calculation]
    //     U<bitsize> or I<bitsize> or F<bitsize>
    //      bitsize is 8, 16, 24, 32 for U and I, 32 and 64 for F
    //     Q<intbits>Q<fractionalbits>  fixed point number e.g. Q6Q10|HEX|AccelX|G
    //         total of intbits + fractionalbits must be 8, 16, 32
    //     /[U|I]<intbits>/[U|I|P]<fractionalbits> fixed point number
    //         intbits and fractionalbits are each 8,16, or 32
    //         P means the number is a decimal e.g. for P8 the number is 0..99 
    //     BYTES
    //     STRING -- display is ASCII
    //     XRS^elementname1_elementname2_elementname3||name = array of records (defined by ODE Option Define Element)
    //     OEB OEL order endian; default is little-endian
    //     OOPT reset of fields are optional
    //
    //
    //  Display is DEC HEX FIXED STRING Speciality^Appearance
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
        /// The nice decoded value. If there was an error, this might be ??-??
        /// </summary>
        public string UserString = "??-??";
        /// <summary>
        /// The error string from any errors encountered while parsing; otherwise just blank.
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
            var vps = ParserFieldList.ParseLine(decodeCommands);
            return ConvertHelper(dr, vps, decodeCommands);
        }
        public static ValueParserResult Parse(IBuffer buffer, ParserFieldList decodeCommandsPFL)
        {
            var dr = DataReader.FromBuffer(buffer);
            dr.ByteOrder = ByteOrder.LittleEndian; // default to little endian because it's common for bluetooth/
            return ConvertHelper(dr, decodeCommandsPFL, decodeCommandsPFL.ToString());
        }
        enum ResultState { NoResult, IsString, IsDouble, IsDoubleArray, IsBytes };

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
                            return BluetoothDefinitionLanguage.BluetoothAppearance.AppearanceToString((ushort)dvalue);
                    }
                    break;
            }
            return null; // null means we couldn't do the conversion
        }

        /// <summary>
        /// Returns a ValueParserResult: a string and a ValueList of the result
        /// </summary>
        private static ValueParserResult ConvertHelper(DataReader dr, ParserFieldList vps, string logstr)
        {
            var str = new StringBuilder();
            var valueList = new BCBasic.BCValueList();
            bool isOptional = false;

            for (int i = 0; i < vps.Fields.Count; i++)
            {
                var command = vps.Fields[i];
                var err = ConvertHelperOne(str, valueList, dr, vps, logstr, i, ref isOptional, command);
                if (err != null)
                {
                    return err;
                }
                if (dr.UnconsumedBufferLength <= 0)
                {
                    break;
                }

            }
            var returnstr = str.ToString();
            return ValueParserResult.CreateOk(returnstr, valueList);
        }

        /// <summary>
        /// Does  single command (like U8) and updates a bunch of eventual return values (returnsb, valueList, etc.). Reads from DataReader dr. Returns a non-null value only on errors.
        /// </summary>
        /// <param name="returnsb"></param>
        /// <param name="valueList"></param>
        /// <param name="dr"></param>
        /// <param name="vps"></param>
        /// <param name="logstr"></param>
        /// <param name="defaultIndex"></param>
        /// <param name="isOptional"></param>
        /// <param name="command"></param>
        /// <returns>On error, returns a ValueParserResult with the error; otherwise returns null</returns>
        private static ValueParserResult ConvertHelperOne(StringBuilder returnsb, BCBasic.BCValueList valueList, DataReader dr, ParserFieldList vps, string logstr, int defaultIndex, ref bool isOptional, ParserField command)
        {
            var stritem = "";
            byte[] byteArrayItem = null;

            var readcmd = command.ByteFormatPrimary;
            var readindicator = readcmd[0];
            var displayFormat = command.DisplayFormatPrimary;
            var displayFormatSecondary = command.Get(1, 1);

            var name = command.NamePrimary;
            if (string.IsNullOrEmpty(name)) name = $"param{defaultIndex}";
            var units = command.UnitsPrimary;

            var resultState = ResultState.IsDouble; // the most common result
            double dvalue = double.NaN;
            List<double> dvalues = null;
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
                                return ValueParserResult.CreateError(logstr, $"Missing data with {readcmd} field {defaultIndex + 1}");
                            }
                        }
                        switch (readcmd)
                        {
                            case "F32":
                                {
                                    dvalue = dr.ReadSingle();
                                    switch (displayFormat)
                                    {
                                        case "":
                                        case "FIXED":
                                            displayFormat = (displayFormatSecondary == "") ? "N3" : displayFormatSecondary;
                                            break;
                                        case "DEC":
                                            displayFormat = (displayFormatSecondary == "") ? "N0" : displayFormatSecondary;
                                            break;
                                        case "HEX":
                                            return ValueParserResult.CreateError(logstr, $"Float displayFormat unrecognized; should be FIXED {displayFormat}");
                                    }
                                    stritem = dvalue.ToString(displayFormat); // e.g. N3 for 3 digits
                                }
                                break;
                            case "F64":
                                {
                                    dvalue = dr.ReadDouble();
                                    switch (displayFormat)
                                    {
                                        case "":
                                        case "FIXED":
                                            displayFormat = (displayFormatSecondary == "") ? "N3" : displayFormatSecondary;
                                            break;
                                        case "DEC":
                                            displayFormat = (displayFormatSecondary == "") ? "N0" : displayFormatSecondary;
                                            break;
                                        case "HEX":
                                            return ValueParserResult.CreateError(logstr, $"Float displayFormat unrecognized; should be FIXED {displayFormat}");
                                    }
                                    stritem = dvalue.ToString(displayFormat); // e.g. N3 for 3 digits
                                }
                                break;
                            case "F32S": // Array of F32
                                {
                                    resultState = ResultState.IsDoubleArray;
                                    dvalues = new List<double>();
                                    while (dr.UnconsumedBufferLength >= 2)
                                    {
                                        dvalue = dr.ReadSingle();
                                        dvalues.Add(dvalue);
                                        switch (displayFormat)
                                        {
                                            case "":
                                            case "FIXED":
                                                displayFormat = (displayFormatSecondary == "") ? "N3" : displayFormatSecondary;
                                                break;
                                            case "DEC":
                                                displayFormat = (displayFormatSecondary == "") ? "N0" : displayFormatSecondary;
                                                break;
                                            case "HEX":
                                                return ValueParserResult.CreateError(logstr, $"Float displayFormat unrecognized; should be FIXED {displayFormat}");
                                        }

                                        if (stritem != "") stritem += " ";
                                        stritem += dvalue.ToString(displayFormat); // e.g. N3 for 3 digits
                                    } // end while loop
                                }
                                break;
                            default:
                                return ValueParserResult.CreateError(logstr, $"Float command unrecognized; should be F32 or F64 not {readcmd}");
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
                                return ValueParserResult.CreateError(logstr, $"Missing data with {readcmd} field {defaultIndex + 1}");
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
                                        dvalue = ValueCalculate.Calculate(calculateCommand, dvalue).D;
                                        if (double.IsNaN(dvalue))
                                        {
                                            return ValueParserResult.CreateError(logstr, $"Calculation failed for {calculateCommand} in {readcmd}");
                                        }
                                        else
                                        {
                                            // Everything worked and got a value
                                            stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary);
                                            if (stritem == null)
                                            {
                                                return ValueParserResult.CreateError(logstr, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (displayFormat == "") displayFormat = "HEX";
                                        stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary, floatFormat, intFormat);
                                        if (stritem == null)
                                        {
                                            return ValueParserResult.CreateError(logstr, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                        }
                                    }

                                }
                                break;

                            case "I16S": // Array of I16
                                {
                                    if (displayFormat == "") displayFormat = "HEX";
                                    string floatFormat = "F2";
                                    string intFormat = "X6";

                                    resultState = ResultState.IsDoubleArray;
                                    dvalues = new List<double>();
                                    while (dr.UnconsumedBufferLength >= 2)
                                    {
                                        dvalue = dr.ReadInt16();
                                        dvalues.Add(dvalue);

                                        var intstr = DoubleToString(dvalue, displayFormat, displayFormatSecondary, floatFormat, intFormat);
                                        if (intstr == null)
                                        {
                                            return ValueParserResult.CreateError(logstr, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                        }

                                        if (stritem != "") stritem += " ";
                                        stritem += intstr;
                                    } // end while loop
                                }
                                break;


                            default:
                                return ValueParserResult.CreateError(logstr, $"Integer command unrecognized; should be I8/16/24/32 not {readcmd}");
                        }
                        break;
                    case 'O':
                        switch (readcmd)
                        {
                            case "ODE": // ODE = Option Define Element; ignored while building 
                                resultState = ResultState.NoResult;
                                break;
                            case "ODR": // ODR = Option Define Record; ignored while building 
                                resultState = ResultState.NoResult;
                                break;
                            case "OEB": // OEB = Option Endian Big-endian
                                resultState = ResultState.NoResult;
                                dr.ByteOrder = ByteOrder.BigEndian;
                                break;
                            case "OEL": // OEL = Option Endian Little-endian
                                resultState = ResultState.NoResult;
                                dr.ByteOrder = ByteOrder.LittleEndian;
                                break;
                            case "OOPT": // OOPT = rest of items are optional
                                isOptional = true;
                                break;
                            default:
                                return ValueParserResult.CreateError(logstr, $"Optional command unrecognized; should be OEL or OEB not {readcmd}");
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
                                return ValueParserResult.CreateError(logstr, $"Missing data with {readcmd} field {defaultIndex + 1}");
                            }
                        }
                        // e.g. Q12Q4.Fixed
                        {
                            var subtypes = readcmd.Split(new char[] { 'Q' });
                            if (subtypes.Length != 3) // Actually 2, but first is blank
                            {
                                return ValueParserResult.CreateError(logstr, $"Q command unrecognized; wrong number of Qs {readcmd}");
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
                                return ValueParserResult.CreateError(logstr, $"Missing data with {readcmd} field {defaultIndex + 1}");
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
                                    dvalue = ValueCalculate.Calculate(calculateCommand, dvalue).D;
                                    if (double.IsNaN(dvalue))
                                    {
                                        return ValueParserResult.CreateError(logstr, $"Calculation failed for {calculateCommand} in {readcmd}");
                                    }
                                    else
                                    {
                                        stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary);
                                        if (stritem == null)
                                        {
                                            return ValueParserResult.CreateError(logstr, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                        }
                                    }
                                }
                                else
                                {
                                    if (displayFormat == "") displayFormat = "HEX";
                                    stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary, "F2", xfmt);
                                    if (stritem == null)
                                    {
                                        return ValueParserResult.CreateError(logstr, $"Integer display format command unrecognized;\nshould be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                    }
                                }
                                break;
                            default:
                                return ValueParserResult.CreateError(logstr, $"UInteger command unrecognized;\nshould be U8/U16/U24/U32 not {readcmd}");
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
                                return ValueParserResult.CreateError(logstr, $"Missing data with {readcmd} field {defaultIndex + 1}");
                            }
                        }
                        {
                            var subtypes = readcmd.Split(new char[] { '/' });
                            if (subtypes.Length != 3) // Actually 2, but first is blank
                            {
                                return ValueParserResult.CreateError(logstr, $"/ command unrecognized; wrong number of slashes {readcmd}");
                            }
                            stritem = FixedFractionHelper(dr, subtypes[1], subtypes[2], displayFormat, out dvalue);
                            // NOTE: fail on failure
                        }
                        break;
                    case 'X':
                        switch (readcmd)
                        {
                            case "XR":
                                {
                                    var recordname = command.Get(0, 1); // XR^EnvironmentData||Data
                                    var list = vps.GetRecordDefinition(recordname);
                                    if (list == null)
                                    {
                                        return ValueParserResult.CreateError(logstr, $"XR command has unknown recordname={recordname} globals={vps.Globals}");
                                    }
                                    returnsb.Append($" XR={recordname} ");
                                    for (int i=0; i<list.Count; i++)
                                    {
                                        var item = list[i];
                                        var itemresult = ConvertHelperOne(returnsb, valueList, dr, vps, logstr, defaultIndex * 100 + i, ref isOptional, item);
                                        if (itemresult != null)
                                        {
                                            // Only get a non-null for errors.
                                            return itemresult;
                                        }
                                    }
                                }
                                break;
                            default:
                                return ValueParserResult.CreateError(logstr, $"X command unrecognized; should be XR^recordname not {readcmd}");
                        }
                        break;
                    default:
                        if (readcmd != readcmd.ToUpper())
                        {
                            Log("ERROR: readcmd {readcmd} but should be uppercase");
                        }
                        int skip = command.MaxBytesRemaining;
                        if (skip == -1)
                        {
                            skip = 0;
                        }
                        else if (skip <= -2)
                        {
                            return ValueParserResult.CreateError(logstr, $"Unable to get MaxBytesRemaining for {readcmd}");
                        }
                        switch (readcmd.ToUpper()) //NOTE: should be all-uppercase; any lowercase is wrong
                        {
                            case "STRING":
                                {
                                    ReadStatus readStatus;
                                    (stritem, readStatus) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength - (uint)skip);
                                    switch (readStatus)
                                    {
                                        case ReadStatus.OK:
                                            switch (displayFormat)
                                            {
                                                case "":
                                                case "ASCII":
                                                    stritem = EscapeString(stritem, displayFormatSecondary);
                                                    break;
                                                case "Eddystone":
                                                    stritem = BluetoothDefinitionLanguage.Eddystone.EddystoneUrlToString(stritem);
                                                    break;
                                                default:
                                                    return ValueParserResult.CreateError(logstr, $"Unknown string format type {displayFormat}");

                                            }
                                            break;
                                        case ReadStatus.Hex:
                                            break;
                                    }

                                    resultState = ResultState.IsString;
                                }
                                break;
                            case "BYTES":
                                {
                                    //You might want bytes, but this methods is explicitly generating a string.
                                    if (dr.UnconsumedBufferLength == 0)
                                    {
                                        stritem = "(no bytes)";
                                        resultState = ResultState.IsString;
                                    }
                                    else
                                    {
                                        IBuffer buffer = dr.ReadBuffer(dr.UnconsumedBufferLength - (uint)skip);
                                        byteArrayItem = buffer.ToArray();
                                        var format = "X2";
                                        switch (displayFormat)
                                        {
                                            case "DEC": format = ""; break;
                                            default:
                                            case "HEX": format = "X2"; break;
                                        }
                                        for (uint ii = 0; ii < byteArrayItem.Length; ii++)
                                        {
                                            if (ii != 0) stritem += " ";
                                            stritem += byteArrayItem[ii].ToString(format);
                                        }
                                        resultState = ResultState.IsBytes;
                                    }
                                }
                                break;
                            default:
                                return ValueParserResult.CreateError(logstr, $"Other command unrecognized; should be STRING or BYTES {readcmd}");
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                stritem = $"EXCEPTION reading data {e} index {defaultIndex} command {command} len {dr.UnconsumedBufferLength}";
                return ValueParserResult.CreateError(returnsb + stritem, stritem);
            }
            switch (resultState)
            {
                case ResultState.IsBytes:
                    valueList.SetProperty(name, new BCBasic.BCValue(byteArrayItem));
                    break;
                case ResultState.IsDouble:
                    valueList.SetProperty(name, new BCBasic.BCValue(dvalue));
                    break;
                case ResultState.IsDoubleArray:
                    var darray = dvalues.ToArray();
                    valueList.SetProperty(name, new BCBasic.BCValue(darray));
                    break;
                case ResultState.IsString:
                    valueList.SetProperty(name, new BCBasic.BCValue(stritem));
                    break;
            }

            if (returnsb.Length > 0) returnsb.Append(" ");
            returnsb.Append(stritem);
            return null; // only return a value on error. Yes, this is weird.
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


        private static string EscapeString(string rawstr, string displayFormatSecondary) // displayFormatSecondary is either "" or LONG
        {
            rawstr = rawstr.Replace("\\", "\\\\"); // escape all back-slashes
            rawstr = rawstr.Replace("\0", "\\0"); // replace a real NUL with an escaped null.
            switch (displayFormatSecondary)
            {
                default:
                    rawstr = rawstr.Replace("\r", "\\r");
                    rawstr = rawstr.Replace("\n", "\\n");
                    break;
                case "LONG":
                    // The LONG style display can easily display CR and LF 
                    // The concept where was to make it trivial to both get the CR LF and
                    // also see the values on the screen. The goal was to be easier to distinguish.
                    // The actual result looked cluttered and ugly.
                    //rawstr = rawstr.Replace("\r", "\\r\r");
                    //rawstr = rawstr.Replace("\n", "\\n\n");
                    break;
            }
            rawstr = rawstr.Replace("\v", "\\v");
            return rawstr;
        }

        public static string UnescapeString (string rawstr)
        {
            var retval = rawstr;
            // Do in reverse order of the EscapeString
            retval = retval.Replace("\\v", "\v").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\0", "\0").Replace("\\\\", "\\");
            return retval;
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

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Console.WriteLine(message);
        }
    }
}
