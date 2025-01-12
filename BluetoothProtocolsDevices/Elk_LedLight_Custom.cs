using BluetoothProtocolsDevices.BluetoothProtocolsCustom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{ 
    public partial class Elk_LedLight : Light
    {
        public override Capability GetDeviceCapability()
        {
            var retval = Capability.OnOff | Capability.SetBrightness | Capability.SetWarm;
            return retval;
        }
        public override async Task<GattCommunicationStatus> SetBrightnessAsync(double value)
        {
            byte bvalue = (byte)(value * 100.0);
            var result = await WriteCommand(0x7E, Counter, 0x01, bvalue, 0x00, 0x00, 0x00, 0x00, 0xEF);
            return result;
        }
        public override async Task<GattCommunicationStatus> SetRGBAsync(int R, int G, int B)
        {
            await Task.Delay(0);
            return GattCommunicationStatus.Unreachable;
        }
        public override async Task<GattCommunicationStatus> SetWarmthAsync(double value)
        {
            byte warm = (byte)(value * 100.0);
            byte cold = (byte)(100-warm);
            var result = await WriteCommand(0x7E, Counter, 0x05, 0x02, warm, cold, 0x00, 0x00, 0xEF);
            return result;
        }

        byte Counter = 0;
        public override async Task<GattCommunicationStatus> TurnOnOffAsync(bool On)
        {
            var result = await WriteCommand(0x7E, Counter, 0x04, 0xFF, 0x00, (On ? (byte)0x01 : (byte)0x00), 0x00, 0x00, 0xEF);
            return result;
        }
    }
}
