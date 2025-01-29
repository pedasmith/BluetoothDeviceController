using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{
    /// <summary>
    /// LED strip lightswith Bluetooth control. Can also be controlled with a small remote. 
    /// This class was automatically generated 2025-01-29::14:30
    /// </summary>

    public partial class Daybetter_LedLight : Light
    {
        public override Capability GetDeviceCapability()
        {
            var retval = Capability.OnOff | Capability.SetBrightness | Capability.SetColorRGB;
            return retval;
        }
        public override async Task<GattCommunicationStatus> SetBrightnessAsync(double value)
        {
            byte bvalue = (byte)(value * 100.0);
            var result = await WriteModbusSend(0xA0, 0x13, new byte[] { 0x04, bvalue}, 0x00); // CRC will be set correctly
            return result;
        }
        public override async Task<GattCommunicationStatus> SetRGBAsync(byte R, byte G, byte B)
        {
            var result = await WriteModbusSend(0xA0, 0x15, new byte[] { 0x06, R, G, B}, 0x00); // CRC will be set correctly
            return result;
        }
        public override async Task<GattCommunicationStatus> SetWarmthAsync(double value)
        {
            await Task.Delay(0);
            return GattCommunicationStatus.Unreachable;
        }

        byte Counter = 0;
        public override async Task<GattCommunicationStatus> TurnOnOffAsync(bool On)
        {
            byte bvalue = On ? (byte)0x01 : (byte)0x00;
            var result = await WriteModbusSend(0xA0, 0x11, new byte[] { 0x04, bvalue }, 0x00); // CRC will be set correctly
            return result;
        }
    }
}
