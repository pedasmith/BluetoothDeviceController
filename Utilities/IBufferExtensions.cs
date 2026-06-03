using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Streams;

namespace Utilities
{
    public static class IBufferExtensions
    {
        /// <summary>
        /// Converts an IBuffer to a byte[] byte array
        /// </summary>
        public static byte[] ToByteArray(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                return byteArray;
            }
        }

        /// <summary>
        /// Creates a comma-seperated hexademical string of the buffer contents. Each hex digit includes a "0x" in front.
        /// See also ToHex is you don't want the "0x" in front of each digit.
        /// Example: 0x00,0x0F,0xF3,0x22
        /// </summary>
        public static string ToCsv(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                StringBuilder retval = new StringBuilder();
                foreach (var b in byteArray)
                {
                    if (retval.Length > 0) retval.Append(",");
                    retval.Append($"0x{b:X02}");
                }
                return retval.ToString();
            }
        }

        /// <summary>
        /// Creates a space-seperated hexademical string of the buffer contents. Each hex digit includes a "0x" in front.
        /// See also ToHex is you don't want the "0x" in front of each digit.
        /// Example: 0x00 0x0F 0xF3 0x22
        /// </summary>
        public static string ToSsv(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                StringBuilder retval = new StringBuilder();
                foreach (var b in byteArray)
                {
                    if (retval.Length > 0) retval.Append(" ");
                    retval.Append($"0x{b:X02}");
                }
                return retval.ToString();
            }
        }

        /// <summary>
        /// Creates a space-seperated hexademical string of the buffer contents. The hex digits are not prefixed with 0x.
        /// See also the ToSsv if you want the 0x for each digit.
        /// Example: 00 0F F3 22
        /// </summary>
        public static string ToHex(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                StringBuilder retval = new StringBuilder();
                foreach (var b in byteArray)
                {
                    if (retval.Length > 0) retval.Append(" ");
                    retval.Append($"{b:X02}");
                }
                return retval.ToString();
            }
        }
    }
}
