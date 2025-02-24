using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation.Diagnostics;
using Windows.Storage.Streams;

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

                int bytesUsed = 0;
                int charsUsed = 0;
                bool completed = true;
                Utf8NonThrowingDecoder.Convert(buffer, 0, buffer.Length, outbuffer, 0, outbuffer.Length, true, out bytesUsed, out charsUsed, out completed);
                retval = new string(outbuffer, 0, charsUsed);
                readStatus = retval.Contains(EncoderFallbackDetectionString) ? ReadStatus.Hex : ReadStatus.OK;
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

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}
