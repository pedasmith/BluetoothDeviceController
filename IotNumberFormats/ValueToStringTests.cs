using BluetoothDeviceController.BleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace IotNumberFormats
{
    public class ValueToStringTest
    {
        public static int Test()
        {
            int NError = 0;
            NError += TestSimple();
            NError += TestStringEscape();
            NError += TestBytes();
            return NError;
        }

        private static int TestSimple()
        {
            int NError = 0;


            NError += TestOk("U8", "01", "01");
            NError += TestOk("BYTES", "01", "01");
            NError += TestOk("U8 U8|DEC U8|DEC I8|DEC", "1F 1F 82 82", "1F 31 130 -126"); // I82-->

            NError += TestOk("I24^100_/|FIXED", "DB 07 00", "20.11"); // TI 1350 Barometer temp reading 70F-->20.11C-->2011-->7DB hex -> 00 07 DB

            NError += TestOk("U8 BYTES|HEX U16|HEX", "1F 31 32 33 20 30", "1F 31 32 33 3020");
            NError += TestOk("U8 STRING|ASCII U16|HEX", "1F 31 32 33 20 30", "1F 123 3020");

            NError += TestOk("I8 I16 I24 I32", "01 02 03 04 05 06 07 08 09 0A", "01 0302 060504 0A090807");
            NError += TestOk("I8 I16S I8 I8 I8", "01 02 03 04 05 06 07 08 09 0A", "01 0302 0504 0706 08 09 0A");
            NError += TestOk("U8 U16 U24 U32", "01 02 03 04 05 06 07 08 09 0A", "01 0302 060504 0A090807");
            // Explicitly do not test e.g., a I16 with X4 format when the value is negative; it fails in weird ways.
            NError += TestOk("U16 OEL U16 OEB U16", "01 02 03 04 05 06", "0201 0403 0506"); // Testing OEL OEB default is little endian for BT

            NError += TestOk("F32 F64", "00 00 46 41 C3 F5 28 5C 8F 42 34 40", "12.375 20.260"); // Testing F32 F64
            NError += TestOk("F32S I32", "00 00 46 41 00 00 46 41 50 51 52 53", "12.375 12.375 53525150"); // Testing F32S

            NError += TestOk("U8 U8 OOPT U8", "01 02 03", "01 02 03"); // Testing OOPT
            NError += TestOk("U8 U8 OOPT U8", "01 02", "01 02"); // Testing OOPT

            NError += TestOk("Q8Q8", "80 01", "1.500"); // Testing Q
            NError += TestOk("Q12Q4", "04 01", "16.25"); // Testing Q

            NError += TestOk("/I8/P8", "07 40", "7.64"); // Testing /_P  . Unlike the Q types, the two value are read one after the other

            // TESTS: 
            // I8 I16 I16S I24 I32 U8 U16 U24 U32
            // F32 F32S F64 
            // Q /
            // STRING BYTES
            // OEL OEB
            // OOPT
            // MISSING*** ODE ODR XR


            return NError;
        }

        private static int TestBytes()
        {
            int nerror = 0;
            var actualResult = ParserFieldList.ParseLine("U8 U16 BYTES|HEX|MyBytes U8 U8 U8 F32");
            var b = actualResult.Fields[2];
            if (b.NamePrimary != "MyBytes")
            {
                nerror += 1;
                Log($"ERROR: TestBytes: name is {b.NamePrimary} but expected MyBytes");
            }
            if (b.NBytes != -1)
            {
                nerror += 1;
                Log($"ERROR: TestBytes: NBytes is {b.NBytes} but expected -1");
            }
            if (b.MaxBytesRemaining != 7)
            {
                nerror += 1;
                Log($"ERROR: TestBytes: MaxBytesRemaining is {b.MaxBytesRemaining} but expected 7");
            }
            return nerror;
        }

        /// <summary>
        /// Test ValueToString providing a string of bytes in hex format (e.g. "01 02 03") and the expected string results and the decode commands.
        /// </summary>
        /// <param name="commands"></param>
        /// <param name="value"></param>
        /// <param name="expected"></param>
        /// <returns></returns>
        private static int TestOk(string commands, string value, string expected)
        {
            int NError=0;
            var valueb = Hex(value);
            var actualResult = ValueParser.Parse(valueb, commands);
            if (actualResult.Result != ValueParserResult.ResultValues.Ok)
            {
                Log($"ERROR: ValueToStringTest:TestOk ({commands}, {value}) failed to parse at all; expected OK");
                NError++;
            }
            else
            {
                if (actualResult.AsString != expected)
                {
                    Log($"ERROR: ValueToStringTest:TestOk ({commands}, {value}) actual is {actualResult.AsString} expected {expected}");
                    NError++;
                }
            }
            return NError;
        }

        private static void Log(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            Console.Write(message);
        }

        private static byte[] Hex(string value)
        {
            var result = ValueParserHelpers.ConvertToBuffer(value, "HEX");
            if (result.Result != ValueParserResult.ResultValues.Ok)
            {
                Log($"ERROR: ValueToStringTest ({value}) failed to parse");
                return new byte[0];
            }
            return result.ByteResult.ToArray();
        }

        private static int TestStringEscape()
        {
            int nerror = 0;
            nerror += TestStringEscapeOne("abc");
            nerror += TestStringEscapeOne("");
            nerror += TestStringEscapeOne("U8 U16 U32");
            nerror += TestStringEscapeOne("STRING^$BERO");
            for (char ch = char.MinValue; ch < (char)127; ch++)
            {
                var str = ch.ToString();
                nerror += TestStringEscapeOne(str);
            }
            return nerror;
        }

        private static int TestStringEscapeOne(string str)
        {
            int nerror = 0;
            var escape = ValueCalculate.EscapeString(str);
            if (escape.Contains('\0'))
            {
                nerror++;
                Log($"ERROR in TestStringEscape({str}) escape {escape} contains NULL");
            }
            if (escape.Contains(' '))
            {
                nerror++;
                Log($"ERROR in TestStringEscape({str}) escape {escape} contains SPACE");
            }
            if (escape.Contains('|'))
            {
                nerror++;
                Log($"ERROR in TestStringEscape({str}) escape {escape} contains BAR");
            }
            if (escape.Contains('^'))
            {
                nerror++;
                Log($"ERROR in TestStringEscape({str}) escape {escape} contains CARET");
            }
            if (escape.Contains('\r'))
            {
                nerror++;
                Log($"ERROR in TestStringEscape({str}) escape {escape} contains CR");
            }
            if (escape.Contains('\n'))
            {
                nerror++;
                Log($"ERROR in TestStringEscape({str}) escape {escape} contains LF");
            }
            var reverse = ValueCalculate.UnescapeString(escape);
            if (reverse != str)
            {
                nerror++;
                Log($"ERROR in TestStringEscape({str}) escape {escape} reverse {reverse} isn't the same!");
            }

            return nerror;
        }
    }
}