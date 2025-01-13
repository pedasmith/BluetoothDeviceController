using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{
    public abstract class Light
    {
        [Flags]
        public enum Capability
        {
            OnOff = 0x01,
            SetWarm = 0x02,
            SetBrightness = 0x04,
            SetColorRGB = 0x08,
        }
        public abstract Capability GetDeviceCapability();
        /// <summary>
        /// Brightness is a value 0..1 inclusive
        /// </summary>
        public abstract Task<GattCommunicationStatus> SetBrightnessAsync(double brightness);
        /// <summary>
        /// RGB values are all 0..255
        /// </summary>
        public abstract Task<GattCommunicationStatus> SetRGBAsync(byte R, byte G, byte B);
        /// <summary>
        /// Warmth is a value 0..1 inclusive. 0=cold 1=warm
        /// </summary>
        public abstract Task<GattCommunicationStatus> SetWarmthAsync(double warmth);
        public abstract Task<GattCommunicationStatus> TurnOnOffAsync(bool on);
    }
}
