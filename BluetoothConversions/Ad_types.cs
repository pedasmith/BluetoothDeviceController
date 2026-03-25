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
				case 0x01: return "Flags"; // 
				case 0x02: return "Incomplete List of 16-bit Service or Service Class UUIDs"; // 
				case 0x03: return "Complete List of 16-bit Service or Service Class UUIDs"; // 
				case 0x04: return "Incomplete List of 32-bit Service or Service Class UUIDs"; // 
				case 0x05: return "Complete List of 32-bit Service or Service Class UUIDs"; // 
				case 0x06: return "Incomplete List of 128-bit Service or Service Class UUIDs"; // 
				case 0x07: return "Complete List of 128-bit Service or Service Class UUIDs"; // 
				case 0x08: return "Shortened Local Name"; // 
				case 0x09: return "Complete Local Name"; // 
				case 0x0A: return "Tx Power Level"; // 
				case 0x0D: return "Class of Device"; // 
				case 0x0E: return "Simple Pairing Hash C-192"; // 
				case 0x0F: return "Simple Pairing Randomizer R-192"; // 
				case 0x10: return "Device ID"; // 
				//case 0x10: return "Security Manager TK Value"; // 
				case 0x11: return "Security Manager Out of Band Flags"; // 
				case 0x12: return "Peripheral Connection Interval Range"; // 
				case 0x14: return "List of 16-bit Service Solicitation UUIDs"; // 
				case 0x15: return "List of 128-bit Service Solicitation UUIDs"; // 
				case 0x16: return "Service Data - 16-bit UUID"; // 
				case 0x17: return "Public Target Address"; // 
				case 0x18: return "Random Target Address"; // 
				case 0x19: return "Appearance"; // 
				case 0x1A: return "Advertising Interval"; // 
				case 0x1B: return "LE Bluetooth Device Address"; // 
				case 0x1C: return "LE Role"; // 
				case 0x1D: return "Simple Pairing Hash C-256"; // 
				case 0x1E: return "Simple Pairing Randomizer R-256"; // 
				case 0x1F: return "List of 32-bit Service Solicitation UUIDs"; // 
				case 0x20: return "Service Data - 32-bit UUID"; // 
				case 0x21: return "Service Data - 128-bit UUID"; // 
				case 0x22: return "LE Secure Connections Confirmation Value"; // 
				case 0x23: return "LE Secure Connections Random Value"; // 
				case 0x24: return "URI"; // 
				case 0x25: return "Indoor Positioning"; // 
				case 0x26: return "Transport Discovery Data"; // 
				case 0x27: return "LE Supported Features"; // 
				case 0x28: return "Channel Map Update Indication"; // 
				case 0x29: return "PB-ADV"; // 
				case 0x2A: return "Mesh Message"; // 
				case 0x2B: return "Mesh Beacon"; // 
				case 0x2C: return "BIGInfo"; // 
				case 0x2D: return "Broadcast_Code"; // 
				case 0x2E: return "Resolvable Set Identifier"; // 
				case 0x2F: return "Advertising Interval - long"; // 
				case 0x30: return "Broadcast_Name"; // 
				case 0x31: return "Encrypted Advertising Data"; // 
				case 0x32: return "Periodic Advertising Response Timing Information"; // 
				case 0x34: return "Electronic Shelf Label"; // 
				case 0x3D: return "3D Information Data"; // 
				case 0xFF: return "Manufacturer Specific Data"; // 
                // endupdatefile:

            }
            return $"{value:X2}";
        }
    }
}
