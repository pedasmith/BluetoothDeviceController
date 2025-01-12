using BluetoothProtocolsDevices.BluetoothProtocolsCustom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{
    public partial class Zengge_LedLight : Light
    {
        byte Counter = 0;

        public override Capability GetDeviceCapability()
        {
            var retval = Capability.OnOff | Capability.SetBrightness | Capability.SetWarm;
            return retval;
        }
        byte lastWarmth = 0x44; // arbitrary value
        byte lastBrightness = 0x77; // arbitrary value

        enum CMD : byte
        {
            POWER_ON = 0x23,
            POWER_OFF = 0x24,
            SET_RGB = 0xA1,
            SET_WHITE = 0xB1,
        };
        public override async Task<GattCommunicationStatus> SetBrightnessAsync(double value)
        {
            byte brightness = (byte)(value * 100.0);
            lastBrightness = brightness;
            var result = await WriteLED_Write(0x00, Counter, 0x80, 0x00, 0x000d, 0x0e, 0x0b3b, (byte)CMD.SET_WHITE, 0x00, 0x00, 0x00, lastWarmth, brightness, new byte[6] { 0, 0, 0, 0, 0, 0 });
            return result;
        }
        public override async Task<GattCommunicationStatus> SetRGBAsync(int R, int G, int B)
        {
            await Task.Delay(0);
            return GattCommunicationStatus.Unreachable;
        }
        public override async Task<GattCommunicationStatus> SetWarmthAsync(double value)
        {
            // Input: warmth is 0(blue/cold) to 1.0 (red/warm). The Zenggee is temperature: 0=lower temp/red = "warmer" to 100 higher temp/bluer = "colder"
            byte warmth = (byte)((1.0-value) * 100.0);
            lastWarmth = warmth;
            var result = await WriteLED_Write(0x00, Counter, 0x80, 0x00, 0x000d, 0x0e, 0x0b3b, (byte)CMD.SET_WHITE, 0x00, 0x00, 0x00, warmth, lastBrightness, new byte[6] { 0, 0, 0, 0, 0, 0 });
            return result;
        }

        public override async Task<GattCommunicationStatus> TurnOnOffAsync(bool On)
        {
            byte cmd = (byte)(On ? CMD.POWER_ON : CMD.POWER_OFF);
            var result = await WriteLED_Write(0x00, Counter, 0x80, 0x00, 0x000d, 0x0e, 0x0b3b, cmd, 0x00, 0x00, 0x00, 0x00, 0x00, new byte[6] { 0, 0, 0, 0, 0, 0 });
            return result;
        }
    }
}
