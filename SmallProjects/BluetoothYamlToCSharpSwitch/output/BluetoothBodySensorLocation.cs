using System;


namespace BluetoothConversions
{
    // From https://btprodspecificationrefs.blob.core.windows.net/assigned-values/16-bit%20UUID%20Numbers%20Document.pdf
    // the items marked GATT Unit
    // 2026: or look in the YAML file: https://bitbucket.org/bluetooth-SIG/public/src/main/assigned_numbers/uuids/units.yaml
    static class BluetoothBodySensorLocation
    {
        public static string Decode(Byte value)
        {
            switch (value)
            {
                // updatefile:
                // url:https://bitbucket.org/bluetooth-SIG/public/raw/main/gss/org.bluetooth.characteristic.body_sensor_location.yaml
                // file:org.bluetooth.characteristic.body_sensor_location.yaml
                // startupdatefile:
				case 0x00: /* 0 */ return "Other";
				case 0x01: /* 1 */ return "Chest";
				case 0x02: /* 2 */ return "Wrist";
				case 0x03: /* 3 */ return "Finger";
				case 0x04: /* 4 */ return "Hand";
				case 0x05: /* 5 */ return "Ear Lobe";
				case 0x06: /* 6 */ return "Foot";
				default: /* case 0x07–0xFF */ return $"Reserved for Future Use ({value:X2})";
                // endupdatefile:
            }
        }
    }
}

