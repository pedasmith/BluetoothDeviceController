using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BluetoothProtocols;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{
    public partial class Govee_H6005 : Light
    {
        public override Capability GetDeviceCapability()
        {
            return Capability.OnOff | Capability.SetBrightness | Capability.SetColorRGB;
        }
        /// <summary>
        /// Brightness is a value 0..1 inclusive
        /// </summary>
        public override async Task<GattCommunicationStatus> SetBrightnessAsync(double brightness)
        {
            byte value = (byte)(brightness * 100.0);
            var retval = await this.WriteLED_Write(0x33, (byte)CMD.BRIGHT, value, 0x00, 0x00, 0x00, new byte[13] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);
            return retval;
        }
        /// <summary>
        /// RGB values are all 0..255
        /// </summary>
        public override async Task<GattCommunicationStatus> SetRGBAsync(byte R, byte G, byte B)
        {
            var retval = await this.WriteLED_Write(0x33, (byte)CMD.RGB, 0x0D, R, G, B, new byte[13] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0);
            return retval;
        }
        /// <summary>
        /// Warmth is a value 0..1 inclusive. 0=cold 1=warm
        /// </summary>
        public override async Task<GattCommunicationStatus> SetWarmthAsync(double warmth)
        {
            await Task.Delay(0);
            return GattCommunicationStatus.AccessDenied;
        }
        public enum CMD : byte
        {
            POWER = 0x01,
            BRIGHT = 0x04,
            RGB = 0x05,
        };

        public override async Task<GattCommunicationStatus> TurnOnOffAsync(bool on)
        {
            byte value = (byte)(on ? 1 : 0);
            var retval = await this.WriteLED_Write(0x33, (byte)CMD.POWER, value, 0x00, 0x00, 0x00, new byte[13] { 0,0,0,0,0,0,0,0,0,0,0,0,0}, 0);
            return retval;
        }
    }
}
