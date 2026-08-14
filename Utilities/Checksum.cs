using System;

namespace Utilities
{
    public static class Checksum
    {
        /// <summary>
        /// Compute CRC-8 (poly 0x07) over the supplied data. Default init 0x00.
        /// </summary>
        public static byte Crc8(byte[] data, int startIndex = 0, int endIndex = -1, byte poly = 0x07, byte init = 0x00)
        {
            if (data == null) return init;
            byte crc = init;
            //foreach (var b in data)
            if (endIndex < 0) endIndex = data.Length + endIndex; // -1 means go to end -2 means go to end except not the last
            for (int index = startIndex; index < endIndex; index++)
            {
                var b = data[index];
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    if ((crc & 0x80) != 0)
                        crc = (byte)((crc << 1) ^ poly);
                    else
                        crc <<= 1;
                }
            }
            return crc;
        }

        /// <summary>
        /// Simple 8-bit sum checksum (mod 256)
        /// </summary>
        public static byte Sum8(byte[] data)
        {
            if (data == null) return 0;
            int sum = 0;
            foreach (var b in data) sum += b;
            return (byte)(sum & 0xFF);
        }
    }
}
