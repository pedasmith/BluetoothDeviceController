using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.BluetoothDefinitionLanguage
{
    static class BluetoothServiceUuid16Bit
    {
        // From https://btprodspecificationrefs.blob.core.windows.net/assigned-values/16-bit%20UUID%20Numbers%20Document.pdf
        // the items marked GATT Service
        public static string Decode (UInt16 value)
        {
            switch (value)
            {
                case 0x1800: return "Generic Access";
                case 0x1801: return "Generic Attribute";
                case 0x1802: return "Immediate Alert";
                case 0x1803: return "Link Loss";
                case 0x1804: return "Tx Power";
                case 0x1805: return "Current Time";
                case 0x1806: return "Reference Time Update ";
                case 0x1807: return "Next DST Change ";
                case 0x1808: return "Glucose";
                case 0x1809: return "Health Thermometer";
                case 0x180A: return "Device Information";
                case 0x180D: return "Heart Rate";
                case 0x180E: return "Phone Alert Status ";
                case 0x180F: return "Battery";
                case 0x1810: return "Blood Pressure";
                case 0x1811: return "Alert Notification ";
                case 0x1812: return "Human Interface Device";
                case 0x1813: return "Scan Parameters";
                case 0x1814: return "Running Speed and Cadence";
                case 0x1815: return "Automation IO";
                case 0x1816: return "Cycling Speed and Cadence";
                case 0x1818: return "Cycling Power";
                case 0x1819: return "Location and Navigation";
                case 0x181A: return "Environmental Sensing";
                case 0x181B: return "Body Composition";
                case 0x181C: return "User Data";
                case 0x181D: return "Weight Scale";
                case 0x181E: return "Bond Management ";
                case 0x181F: return "Continuous Glucose Monitoring ";
                case 0x1820: return "Internet Protocol Support";
                case 0x1821: return "Indoor Positioning ";
                case 0x1822: return "Pulse Oximeter";
                case 0x1823: return "HTTP Proxy";
                case 0x1824: return "Transport Discovery ";
                case 0x1825: return "Object Transfer ";
                case 0x1826: return "Fitness Machine";
                case 0x1827: return "Mesh Provisioning ";
                case 0x1828: return "Mesh Proxy";
                case 0x1829: return "Reconnection Configuration ";
                case 0x183A: return "Insulin Delivery ";
                case 0x183B: return "Binary Sensor";
                case 0x183C: return "Emergency Configuration";
                case 0x183E: return "Physical Activity Monitor ";
                case 0x1843: return "Audio Input Control ";
                case 0x1844: return "Volume Control";
                case 0x1845: return "Volume Offset Control";
                case 0x1846: return "Coordinated Set Identification Service";
                case 0x1847: return "Device Time";
                case 0x1848: return "Media Control Service ";
                case 0x1849: return "Generic Media Control Service ";
                case 0x184A: return "Constant Tone Extension";
                case 0x184B: return "Telephone Bearer Service";
                case 0x184C: return "Generic Telephone Bearer Service";
                case 0x184D: return "Microphone Control";
                case 0x184E: return "Audio Stream Control Service ";
                case 0x184F: return "Broadcast Audio Scan Service ";
                case 0x1850: return "Published Audio Capabilities Service ";
                case 0x1851: return "Basic Audio Announcement Service ";
                case 0x1852: return "Broadcast Audio Announcement Service ";
                case 0x1853: return "Common Audio Service*";
            }
            return $"";
        }
    }
}
