using System.Collections.Generic;

namespace BluetoothDeviceController.Names
{
    public class NameAllBleDevices
    {
        public IList<NameDevice> AllDevices { get; } = new List<NameDevice>();

        public int GetBleIndex(string name)
        {
            var list = AllDevices;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }
        public override string ToString()
        {
            return $"AllBleDevices [{AllDevices.Count}]";
        }
    }

    public class NameAllSerialDevices
    {
        public IList<SerialConfig> AllSerialDevices { get; } = new List<SerialConfig>();

        public int GetSerialIndex(string name)
        {
            var list = AllSerialDevices;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }
        public override string ToString()
        {
            return $"AllSerialDevices [{AllSerialDevices.Count}]";
        }
    }
}
