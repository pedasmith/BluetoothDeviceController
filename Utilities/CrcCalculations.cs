using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class CrcCalculations
    {
        /// <summary>
        /// Given a byte array, calcuate the XOR CRC of each byte except for the last byte. Then
        /// set the last byte to the calculated value.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static void UpdateXorAtEnd(byte[] command)
        {
            byte value = 0;
            for (int i = 0; i < command.Length-1; i++)
            {
                value ^= command[i];
            }
            command[command.Length - 1] = value;
        }
    }
}
