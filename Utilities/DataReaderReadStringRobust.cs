using System;
using System.Runtime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation.Diagnostics;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace Utilities
{
    class DataReaderReadStringRobust
    {
        static Decoder Utf8NonThrowingDecoder = null;
        static string EncoderFallbackDetectionString = "#E?^%${ERR561:";

        public enum ReadStatus {  OK, Hex };
        /// <summary>
        /// Do the equivilent of dr.ReadString(len) but without throwing and will always return a string. If the bytes
        /// can't be read as a UTF8 string, show them as HEX instead. LEN is often dr.UnconsumedBufferLength.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static (string, ReadStatus) ReadString(DataReader dr, uint len)
        {
            if (Utf8NonThrowingDecoder == null)
            {
                Utf8NonThrowingDecoder = System.Text.Encoding.UTF8.GetDecoder();
                Utf8NonThrowingDecoder.Fallback = new DecoderReplacementFallback(EncoderFallbackDetectionString);
            }
            string retval = "";
            var readStatus = ReadStatus.Hex;

            byte[] buffer = new byte[0];
            try
            {
                if (len > dr.UnconsumedBufferLength)
                {
                    Log($"ERROR: Robust string reader: len {len} is longer than the unread buffer {dr.UnconsumedBufferLength}");
                    len = dr.UnconsumedBufferLength;
                }
                buffer = dr.ReadBuffer(len).ToArray();
                char[] outbuffer = new char [len * 50]; // should be big enough :-)

                if (ArrayHasControlChars(buffer))
                {
                    readStatus = ReadStatus.Hex;
                }
                else
                {
                    int bytesUsed = 0;
                    int charsUsed = 0;
                    bool completed = true;
                    Utf8NonThrowingDecoder.Convert(buffer, 0, buffer.Length, outbuffer, 0, outbuffer.Length, true, out bytesUsed, out charsUsed, out completed);
                    retval = new string(outbuffer, 0, charsUsed);
                    readStatus = retval.Contains(EncoderFallbackDetectionString) ? ReadStatus.Hex : ReadStatus.OK;
                }
            }
            catch (Exception)
            {
                readStatus = ReadStatus.Hex;
                // The original code used a plain dr.ReadString(). That worked well except that it would throw
                // exceptions which cluttered up my output log. The new code, AFAICT, doesn't throw exceptions, but I'm
                // keeping the try/catch for now. Change 2025-02-24.
                //  Sample exception: WinRT originate error - 0x80070459 : 'No mapping for the Unicode character exists in the target multi-byte code page.'.
            }
            if (readStatus == ReadStatus.Hex)
            {
                retval = "";
                for (uint ii = 0; ii < buffer.Length; ii++)
                {
                    if (ii != 0) retval += " ";
                    retval += buffer[ii].ToString("X2");
                }

            }
            return (retval, readStatus);
        }
        public static (string, ReadStatus) ReadStringEntire(DataReader dr)
        {
            return ReadString(dr, dr.UnconsumedBufferLength);
        }

        /// <summary>
        /// Returns true if the buffer has control chars (bytes with vaules 0..31 and 127=DEL) including NUL.
        /// But not including
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static bool ArrayHasControlChars(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                var b = value[i];
                switch (b)
                {
                    case 0: case 1: case 2: case 3: case 4: case 5: case 6: case 7: case 8:
                        return true;
                    // 9=HT 10=LF
                    case 11: case 12:
                        return true;
                    // 13=CR
                    case 14: case 15: case 16: case 17: case 18: case 19: case 20:
                    case 21: case 22: case 23: case 24: case 25: case 26:case 27: case 28: case 29:
                    case 30: case 31:
                        return true;
                    case 127: //DEL
                        return true;
                }
            }
            return false;
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}
