using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.BleEditor
{
    public class ValueToStringTest
    {
        public static int Test()
        {
            int NError = 0;
            NError += TestSimple();
            return NError;
        }

        private static int TestSimple()
        {
            int NError = 0;
            NError += TestOk("U8", "01", "01");
            NError += TestOk("BYTES", "01", "01");
            NError += TestOk("U8 U8|DEC U8|DEC I8|DEC", "1F 1F 82 82", "1F 31 130 -126"); // I82-->

            NError += TestOk("I24^100_/|FIXED", "DB 07 00", "20.11"); // TI 1350 Barometer temp reading 70F-->20.11C-->2011-->7DB hex -> 00 07 DB

            return NError;
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
            var expectedb = Hex(value);
            var actualResult = ValueParser.Parse(valueb.AsBuffer(), commands);
            if (actualResult.Result != ValueParserResult.ResultValues.Ok)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: ValueToStringTest:TestOk ({commands}, {value}) failed to parse at all; expected OK");
                NError++;
            }
            else
            {
                if (actualResult.AsString != expected)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: ValueToStringTest:TestOk ({commands}, {value}) actual is {actualResult.AsString} expected {expected}");
                    NError++;
                }
            }
            return NError;
        }

        private static byte[] Hex(string value)
        {
            var result = ValueParser.ConvertToBuffer(value, "HEX");
            if (result.Result != ValueParserResult.ResultValues.Ok)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: ValueToStringTest ({value}) failed to parse");
                return new byte[0];
            }
            return result.ByteResult.ToArray();
        }


    }

}