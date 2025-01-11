using BluetoothProtocolsDevices.BluetoothProtocolsCustom;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothProtocols
{ 
    public partial class Elk_LedLight : Light
    {
        public override Capability GetDeviceCapability()
        {
            var retval = Capability.OnOff | Capability.SetColorRGB;
            return retval;
        }
        public override void SetRGB(int R, int G, int B)
        {
            throw new NotImplementedException();
        }

        byte Counter = 0;
        public override async Task TurnOnOffAsync(bool On)
        {
            var result = await WriteCommand(0x7E, Counter, 0x04, 0xFF, 0x00, (On ? (byte)0x01 : (byte)0x00), 0x00, 0x00, 0xEF);
        }
    }
}
