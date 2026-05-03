using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Streams;

namespace Utilities
{
    public static class IBufferExtensions
    {
        public static byte[] ToByteArray(this IBuffer buffer)
        {
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                byte[] byteArray = new byte[buffer.Length];
                dataReader.ReadBytes(byteArray);
                return byteArray;
            }
        }
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
    }
}
