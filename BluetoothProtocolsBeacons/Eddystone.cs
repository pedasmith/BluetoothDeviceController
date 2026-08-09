using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;
using static BluetoothProtocols.Ruuvi_Tag_v1_Helper;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    class Eddystone
    {
        const int Eddystone_TypeURL = 0x10;

        /// <summary>
        /// Parse Eddystone data from a ServiceData buffer
        /// </summary>
        public static (bool Success, int Power, byte FrameType, string Url) ParseEddystoneUrlArgs(IBuffer buffer)
        {
            var dr = DataReader.FromBuffer(buffer);
            dr.ByteOrder = ByteOrder.LittleEndian;
            var service = dr.ReadUInt16();
            switch (service)
            {
                default:
                    return (false, 0, 0, null);
                case 0xFDDF: //Tile advertisement
                    return (false, 0, 0, null);
                case 0xFEAA: // Eddystone
                    var frameType = dr.ReadByte(); //  (byte)(0x0F & (dr.ReadByte() >> 4));
                    if (frameType != Eddystone_TypeURL) // 0x10
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

        public static (bool Success, int Power, byte FrameType, string Url) ParseEddystoneAdvertisement(BluetoothLEAdvertisementReceivedEventArgs ble)
        {
            // Lets's see if it's an Eddystone beacon...
            // https://github.com/google/eddystone
            // https://github.com/google/eddystone/blob/master/protocol-specification.md

            foreach (var section in ble.Advertisement.DataSections)
            {
                switch ((BluetoothProtocols.AdvertisementDataSectionParser.DataTypeValue)section.DataType)
                {
                    case BluetoothProtocols.AdvertisementDataSectionParser.DataTypeValue.ServiceData: // 0x16 == 22=service data
                        var dr = DataReader.FromBuffer(section.Data);
                        dr.ByteOrder = ByteOrder.LittleEndian;
                        var Service = dr.ReadUInt16();
                        // https://github.com/google/eddystone
                        if (Service == 0xFEAA) // An Eddystone type
                        {
                            //EddystoneFrameType = (byte)(0x0F & (dr.ReadByte() >> 4));
                            var EddystoneFrameType = dr.ReadByte();
                            switch (EddystoneFrameType)
                            {
                                case Eddystone_TypeURL: // 0x10: An Eddystone-URL
                                    // https://github.com/google/eddystone/tree/master/eddystone-url
                                    var result = Eddystone.ParseEddystoneUrlArgs(section.Data);
                                    return result;
                                    /*
                                    if (result.Success && result.Url.StartsWith("https://ruu.vi/#"))
                                    {
                                        //foundValues.Add(AdvertisementType.RuuviTag);
                                        var ruuvi = ParseRuuviTag(result.Url);
                                        ruuvi.Data.TimestampMostRecent = DateTimeOffset.Now;
                                        return ruuvi;
                                    }
                                    break;
                                    */
                            }
                        }
                        break;
                }
            }
            return (false, 0, 0, "");
        }
    }
}