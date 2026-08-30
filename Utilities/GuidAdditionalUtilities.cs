using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    internal static class GuidAdditionalUtilities
    {
        public static string AsAscii(this Guid value)
        {
            var bytes = value.ToByteArray();
            StringBuilder sb = new();
            int nnotascii = 0;
            int nnulatend = 0; // number of nul chars in a row at the end of the bytes.
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] >= 32 && bytes[i] <= 126)
                {
                    sb.Append((char)bytes[i]);
                    nnulatend = 0;
                }
                else
                {
                    if (bytes[i] == 0) nnulatend++;
                    else nnulatend = 0;
                    sb.Append($"0X{bytes[i]:X2}");
                    nnotascii++;
                }
            }
            if (nnotascii != nnulatend) return ""; // 
            return sb.ToString();    
        }
    }
}
