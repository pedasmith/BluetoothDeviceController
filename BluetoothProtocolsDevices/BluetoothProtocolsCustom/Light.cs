using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothProtocolsDevices.BluetoothProtocolsCustom
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
        public abstract void SetRGB(int R, int G, int B);
        public abstract Task TurnOnOffAsync(bool on);
    }
}
