using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{
    public   partial class TI_beLight_2540 : Light
    {
        public override Capability GetDeviceCapability()
        {
            return Capability.SetColorRGB | Capability.SetBrightness;
        }

        byte savedW = 0;
        public override Task<GattCommunicationStatus> SetBrightnessAsync(double brightness)
        {
            savedW = (byte)(brightness * 255);
            return WriteWhite(savedW);
        }

        public override Task<GattCommunicationStatus> SetRGBAsync(byte R, byte G, byte B)
        {
            return WriteSetColor(R, G, B, savedW);
        }

        public override Task<GattCommunicationStatus> SetWarmthAsync(double warmth)
        {
            throw new NotImplementedException();
        }

        public override Task<GattCommunicationStatus> TurnOnOffAsync(bool on)
        {
            throw new NotImplementedException();
        }
    }
}
