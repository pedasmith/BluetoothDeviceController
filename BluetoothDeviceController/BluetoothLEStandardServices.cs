using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothDeviceController
{
    public class BluetoothLEStandardServices
    {
        public static Guid Battery = Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb");
        public static Guid BatteryPercent = Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb");

        public static Guid Display = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb");
        public static Guid Name = Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb");
        public static Guid Appearance = Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb");
        public static Guid Privacy = Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb");
        public static Guid ReconnectAddress = Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb");
        public static Guid ConnectionParameter = Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb");
        //public static Guid Name = Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb");

        // https://developer.bluetooth.org/gatt/services/Pages/ServiceViewer.aspx?u=org.bluetooth.service.device_information.xml


        // Add to CharacteristicData.AsString
        public enum Characteristic
        {
            Other,
            Appearance,
            BatteryPercent,
            Name,
            Privacy,
        };

        public enum DisplayType
        {
            Hex,
            Percent,
            String,

            AppearanceEnum,
            PrivacyEnum,
        };

        public class CharacteristicData
        {
            public CharacteristicData(Characteristic ch, DisplayType dt)
            {
                Characteristic = ch;
                DisplayType = dt;
            }
            public Characteristic Characteristic { get; internal set; }
            public DisplayType DisplayType { get; internal set; }

            public string AsString(IBuffer buffer)
            {
                string str = "";
                switch (Characteristic)
                {
                    case Characteristic.Appearance: str += "Appearance: "; break;
                    case Characteristic.BatteryPercent: str += "Battery: "; break;
                    case Characteristic.Name: str += "Name: "; break;
                    case Characteristic.Privacy: str += "Privacy: "; break;
                    default:
                        str += "Value: ";
                        break;
                }
                switch (DisplayType)
                {
                    default:
                    case DisplayType.Hex:
                        for (uint i = 0; i < buffer.Length; i++)
                        {
                            if (str != "") str += " ";
                            str += $"{buffer.GetByte(i):X2}";
                        }
                        break;
                    case DisplayType.Percent:
                        if (buffer.Length != 1) return AsErrorString(buffer, "Incorrect percent length:");
                        var b = buffer.GetByte(0);
                        str += $"{b}%";
                        break;
                    case DisplayType.String:
                        var dr = Windows.Storage.Streams.DataReader.FromBuffer(buffer);
                        var rawstr = dr.ReadString(dr.UnconsumedBufferLength);
                        rawstr = rawstr.Replace("\\", "\\\\"); // escape all back-slashes
                        rawstr = rawstr.Replace("\0", "\\0"); // replace a real NUL with an escaped null.
                        str += rawstr;
                        break;
                }
                return str;
            }

            private string AsErrorString(IBuffer buffer, string prefix)
            {
                string str = "";
                for (uint i = 0; i < buffer.Length; i++)
                {
                    if (str != "") str += " ";
                    str += $"{buffer.GetByte(i):X2}";
                }
                return prefix + str;
            }
        }


        public static CharacteristicData GetDisplayInfo(GattDeviceService service, GattCharacteristic characteristic)
        {
            if (service.Uuid == Battery)
            {
                if (characteristic.Uuid == BatteryPercent) return new CharacteristicData(Characteristic.BatteryPercent, DisplayType.Percent);
            }
            else if (service.Uuid == Display)
            {
                if (characteristic.Uuid == Name) return new CharacteristicData(Characteristic.Name, DisplayType.String);
                if (characteristic.Uuid == Appearance) return new CharacteristicData(Characteristic.Appearance, DisplayType.AppearanceEnum);
                if (characteristic.Uuid == Privacy) return new CharacteristicData(Characteristic.Privacy, DisplayType.PrivacyEnum);
                if (characteristic.Uuid == ReconnectAddress) return new CharacteristicData(Characteristic.Other, DisplayType.Hex);
                if (characteristic.Uuid == ConnectionParameter) return new CharacteristicData(Characteristic.Other, DisplayType.Hex);
            }
            return new CharacteristicData(Characteristic.Other, DisplayType.Hex);
        }
    }
}
