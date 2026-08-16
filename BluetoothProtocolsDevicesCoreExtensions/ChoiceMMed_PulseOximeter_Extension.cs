using BluetoothWatcher.AdvertismentWatcher;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocolsDevicesCoreExtensions
{
    internal class ChoiceMMed_PulseOximeter_Extension
    {
        public enum SensorType { Other, PulseOximeter, NotThisSensorFamily };
        public static SensorType AdvertIsSensorFamily(WatcherData advertisement)
        {
            if (advertisement.BestName.StartsWith("500E-B"))
            {
                return SensorType.PulseOximeter;
            }
            if (advertisement.BestName.StartsWith("iP900BPB"))
            {
                return SensorType.PulseOximeter;
            }
            return SensorType.NotThisSensorFamily;
        }
    }
}
