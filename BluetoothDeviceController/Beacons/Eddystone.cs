using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.Beacons
{
    class Eddystone
    {
        public static (bool Success, int Power, byte FrameType, string Url) ParseEddystoneUrlArgs(IBuffer buffer)
        {
            var dr = DataReader.FromBuffer(buffer);
            dr.ByteOrder = ByteOrder.LittleEndian;
            var service = dr.ReadUInt16();

            var frameType = dr.ReadByte(); //  (byte)(0x0F & (dr.ReadByte() >> 4));
            if (frameType != 0x10)
            {
                // Only frame type 0x10 is allowed for Eddystone URL
                return (false, 0, 0, null);
            }

            var power = (int)(sbyte)dr.ReadByte();
            var UrlScheme = dr.ReadByte();
            var urlBuilder = new StringBuilder();
            switch (UrlScheme)
            {
                case 0: urlBuilder.Append("http://www."); break;
                case 1: urlBuilder.Append("https://www."); break;
                case 2: urlBuilder.Append("http://"); break;
                case 3: urlBuilder.Append("https://"); break;
                default:
                    // Invalid url scheme
                    return (false, 0, 0, null);
            }
            while (dr.UnconsumedBufferLength > 0)
            {
                var b = dr.ReadByte();
                if (b >= 0 && b <= 13)
                {
                    switch (b)
                    {
                        case 0: urlBuilder.Append(".com/"); break;
                        case 1: urlBuilder.Append(".org/"); break;
                        case 2: urlBuilder.Append(".edu/"); break;
                        case 3: urlBuilder.Append(".net/"); break;
                        case 4: urlBuilder.Append(".info/"); break;
                        case 5: urlBuilder.Append(".biz/"); break;
                        case 6: urlBuilder.Append(".gov/"); break;
                        case 7: urlBuilder.Append(".com"); break;
                        case 8: urlBuilder.Append(".org"); break;
                        case 9: urlBuilder.Append(".edu"); break;
                        case 10: urlBuilder.Append(".net"); break;
                        case 11: urlBuilder.Append(".info"); break;
                        case 12: urlBuilder.Append(".biz"); break;
                        case 13: urlBuilder.Append(".gov"); break;
                    }
                }
                else if (b >= 14 && b <= 32)
                {
                    return (false, 0, 0, null); // reserved for future use
                }
                else if (b >= 127 && b <= 255)
                {
                    return (false, 0, 0, null); // reserved for future use
                }
                else
                {
                    urlBuilder.Append((char)b);
                }
            }
            return (true, power, frameType, urlBuilder.ToString()); // Everything worked
        }
    }
}
