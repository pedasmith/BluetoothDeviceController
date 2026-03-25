using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothConversions
{
    static class Ad_types
    {
        public static string Decode(UInt16 value)
        {
            // From https://btprodspecificationrefs.blob.core.windows.net/assigned-values/16-bit%20UUID%20Numbers%20Document.pdf
            // the items marked GATT Characterisic and Object Type
            switch (value)
            {
                // updatefile:
                // url:https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/core/ad_types.yaml
                // file:ad_types.yaml
                // startupdatefile:
				case 0x01: return "System.Func`1[System.String]"; // 
				case 0x02: return "System.Func`1[System.String]"; // 
				case 0x03: return "System.Func`1[System.String]"; // 
				case 0x04: return "System.Func`1[System.String]"; // 
				case 0x05: return "System.Func`1[System.String]"; // 
				case 0x06: return "System.Func`1[System.String]"; // 
				case 0x07: return "System.Func`1[System.String]"; // 
				case 0x08: return "System.Func`1[System.String]"; // 
				case 0x09: return "System.Func`1[System.String]"; // 
				case 0x0A: return "System.Func`1[System.String]"; // 
				case 0x0D: return "System.Func`1[System.String]"; // 
				case 0x0E: return "System.Func`1[System.String]"; // 
				case 0x0F: return "System.Func`1[System.String]"; // 
				case 0x10: return "System.Func`1[System.String]"; // 
				case 0x10: return "System.Func`1[System.String]"; // 
				case 0x11: return "System.Func`1[System.String]"; // 
				case 0x12: return "System.Func`1[System.String]"; // 
				case 0x14: return "System.Func`1[System.String]"; // 
				case 0x15: return "System.Func`1[System.String]"; // 
				case 0x16: return "System.Func`1[System.String]"; // 
				case 0x17: return "System.Func`1[System.String]"; // 
				case 0x18: return "System.Func`1[System.String]"; // 
				case 0x19: return "System.Func`1[System.String]"; // 
				case 0x1A: return "System.Func`1[System.String]"; // 
				case 0x1B: return "System.Func`1[System.String]"; // 
				case 0x1C: return "System.Func`1[System.String]"; // 
				case 0x1D: return "System.Func`1[System.String]"; // 
				case 0x1E: return "System.Func`1[System.String]"; // 
				case 0x1F: return "System.Func`1[System.String]"; // 
				case 0x20: return "System.Func`1[System.String]"; // 
				case 0x21: return "System.Func`1[System.String]"; // 
				case 0x22: return "System.Func`1[System.String]"; // 
				case 0x23: return "System.Func`1[System.String]"; // 
				case 0x24: return "System.Func`1[System.String]"; // 
				case 0x25: return "System.Func`1[System.String]"; // 
				case 0x26: return "System.Func`1[System.String]"; // 
				case 0x27: return "System.Func`1[System.String]"; // 
				case 0x28: return "System.Func`1[System.String]"; // 
				case 0x29: return "System.Func`1[System.String]"; // 
				case 0x2A: return "System.Func`1[System.String]"; // 
				case 0x2B: return "System.Func`1[System.String]"; // 
				case 0x2C: return "System.Func`1[System.String]"; // 
				case 0x2D: return "System.Func`1[System.String]"; // 
				case 0x2E: return "System.Func`1[System.String]"; // 
				case 0x2F: return "System.Func`1[System.String]"; // 
				case 0x30: return "System.Func`1[System.String]"; // 
				case 0x31: return "System.Func`1[System.String]"; // 
				case 0x32: return "System.Func`1[System.String]"; // 
				case 0x34: return "System.Func`1[System.String]"; // 
				case 0x3D: return "System.Func`1[System.String]"; // 
				case 0xFF: return "System.Func`1[System.String]"; // 
                // endupdatefile:

            }
            return $"{value:X2}";
        }
    }
}
