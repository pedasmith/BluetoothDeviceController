using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.BluetoothDefinitionLanguage
{
    public static class Eddystone
    {
        public static string EddystoneUrlToString (string url)
        {
            // Takes in an eddystone-encoded URL
            // char 0 = 0=http://www. 1=https://www. 2=http:// 3=https://
            string result = "";
            char b1 = url[0];
            switch (b1)
            {
                case '\x00': result = "http://www."; break;
                case '\x01': result = "https://www."; break;
                case '\x02': result = "http://"; break;
                case '\x03': result = "https://"; break;
            }
            for (int i=1; i<url.Length; i++)
            {
                var ch = url[i];
                switch (ch)
                {
                    case '\x00': result += ".com/"; break;
                    case '\x01': result += ".org/"; break;
                    case '\x02': result += ".edu/"; break;
                    case '\x03': result += ".net/"; break;
                    case '\x04': result += ".info/"; break;
                    case '\x05': result += ".biz/"; break;
                    case '\x06': result += ".gov/"; break;
                    case '\x07': result += ".com"; break;
                    case '\x08': result += ".org"; break;
                    case '\x09': result += ".edu"; break;
                    case '\x0a': result += ".net"; break;
                    case '\x0b': result += ".info"; break;
                    case '\x0c': result += ".biz"; break;
                    case '\x0d': result += ".gov"; break;
                    default:
                        result += ch; //NOTE: technically wrong. The spec says that 14..32 and 127..255 are reserved.
                        break;
                }
            }

            return result;
        }
    }
}
