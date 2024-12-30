using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Streams;

namespace Utilities
{
    public static class DataWriterExtension
    {
        public static void WriteInt24(this DataWriter dw, Int32 value)
        {
            byte msb = (byte)((value >> 16) & 0xFF);
            byte b1 = (byte)((value >> 8) & 0xFF);
            byte lsb = (byte)((value >> 0) & 0xFF);
            switch (dw.ByteOrder)
            {
                case ByteOrder.LittleEndian:
                    dw.WriteByte(lsb);
                    dw.WriteByte(b1);
                    dw.WriteByte(msb);
                    break;
                case ByteOrder.BigEndian:
                    dw.WriteByte(msb);
                    dw.WriteByte(b1);
                    dw.WriteByte(lsb);
                    break;
            }
        }
        public static void WriteUInt24(this DataWriter dw, UInt32 value)
        {
            byte msb = (byte)((value >> 16) & 0xFF);
            byte b1 = (byte)((value >> 8) & 0xFF);
            byte lsb = (byte)((value >> 0) & 0xFF);
            switch (dw.ByteOrder)
            {
                case ByteOrder.LittleEndian:
                    dw.WriteByte(lsb);
                    dw.WriteByte(b1);
                    dw.WriteByte(msb);
                    break;
                case ByteOrder.BigEndian:
                    dw.WriteByte(msb);
                    dw.WriteByte(b1);
                    dw.WriteByte(lsb);
                    break;
            }
        }
    }
}
