using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Utilities
{
    class DataReaderReadStringRobust
    {
        public enum ReadStatus {  OK, Hex };
        public static (string, ReadStatus) ReadString(DataReader dr, uint len)
        {
            string stritem;
            var readStatus = ReadStatus.Hex;
            try
            {
                stritem = dr.ReadString(len); // len is often dr.UnconsumedBufferLength
                readStatus = ReadStatus.OK;
            }
            catch (Exception)
            {
                // The string can't be read. Let's try reading as a buffer instead.
                stritem = "";
                IBuffer buffer = dr.ReadBuffer(dr.UnconsumedBufferLength);
                for (uint ii = 0; ii < buffer.Length; ii++)
                {
                    if (ii != 0) stritem += " ";
                    stritem += buffer.GetByte(ii).ToString("X2");
                }
            }
            return (stritem, readStatus);
        }
        public static (string, ReadStatus) ReadStringEntire(DataReader dr)
        {
            return ReadString(dr, dr.UnconsumedBufferLength);
        }
    }
}
