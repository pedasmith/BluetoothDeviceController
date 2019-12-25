using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Names
{
    public class NameAllDevices
    {
        public IList<NameDevice> AllDevices { get; } = new List<NameDevice>();

        public int GetIndex (string name)
        {
            for (int i=0; i<AllDevices.Count; i++)
            {
                if (AllDevices[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }
        public override string ToString()
        {
            return $"AllDevices [{AllDevices.Count}]";
        }
    }
}
