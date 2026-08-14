using System;
using System.Collections.Generic;
using System.Text;
#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace Utilities
{
    public static class HexUtilities
    {
        public static byte[] HexStringToByteArray(string hex)
        {
            if (hex == null) return null;
            hex = hex.Replace(" ", "").Replace("0x", "").Replace("0X", "");
            if (hex.Length % 2 != 0) throw new FormatException("Hex string must have an even length");
            int len = hex.Length / 2;
            var result = new byte[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }
    }
}