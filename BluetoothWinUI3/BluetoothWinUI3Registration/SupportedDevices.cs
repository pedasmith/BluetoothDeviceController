using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.Utilities;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.BluetoothWinUI3Registration
{
    public interface IBTControl
    {
        public void Initialize(WatcherData advertisement);
    }

    public class SupportedDevice
    {
        public SupportedDevice(string match, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type factory)
        {
            Guid guid;
            var isguid = Guid.TryParse(match, out guid);
            if (isguid)
            {
                MatchingServiceGuid = guid;
            }
            else
            {
                MatchingName = match;
            }
            FactoryInterface = factory;
        }
        public string MatchingName { get; set; } = "Thingy*"; // For example 
        public Guid MatchingServiceGuid { get; set; } = Guid.Empty;

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type FactoryInterface { get; set; } = null;
        public bool Matches(WatcherData advertisement)
        {
            var retval = false;
            if (MatchingServiceGuid != Guid.Empty)
            {
                retval = advertisement.ServiceUuids.Contains(MatchingServiceGuid);
                if (retval)
                {
                    ; // handy place for a debugger
                }
            }
            else
            {
                var name = advertisement.BestName;
                retval = name.StarMatch(MatchingName);
            }
            return retval;
        }
    }

    /// <summary>
    /// List of all supported devices. A supported device is one that we know how to connect to and which has a corresponding Control
    /// </summary>
    public static class SupportedDevices
    {
        private static List<SupportedDevice> Devices { get; set; } = new List<SupportedDevice>()
        {
            //new SupportedDevice("BK6*", typeof(BTStandard_DemoControl)),
            new SupportedDevice("JBL*", typeof(BTStandard_DemoControl)),
            //new SupportedDevice("Thingy*", typeof(BTStandard_DemoControl)),



            // Govee Environmental Thermometer devices
            new SupportedDevice("Govee_H5074_*", typeof(BTCommon_EnvironmentalControl)),
            new SupportedDevice("GVH5075_*", typeof(BTCommon_EnvironmentalControl)),
            new SupportedDevice("GVH5103_*", typeof(BTCommon_EnvironmentalControl)),
            new SupportedDevice("GVH5106_*", typeof(BTCommon_EnvironmentalControl)),
            new SupportedDevice("GV5171*", typeof(BTCommon_EnvironmentalControl)),
            new SupportedDevice("GV5179_*", typeof(BTCommon_EnvironmentalControl)),

            // Nordic
            new SupportedDevice("Thingy*", typeof(BTNordic_ThingyControl)),

            // SensorPro devices
            new SupportedDevice("T201", typeof(BTCommon_EnvironmentalControl)),

            // ThermPro devices
            new SupportedDevice("TP351*", typeof(BTCommon_EnvironmentalControl)),
            new SupportedDevice("TP357*", typeof(BTCommon_EnvironmentalControl)),
            new SupportedDevice("TP359*", typeof(BTCommon_EnvironmentalControl)),

            // Bike Cycle Cadence and Speed
            //new SupportedDevice("BK6*", typeof(BTTAOPE_CyclingSpeedCadenceControl)),
            new SupportedDevice("00001816-0000-1000-8000-00805F9B34FB", typeof(BTStandard_CyclingSpeedCadenceControl)),
            new SupportedDevice("0000180d-0000-1000-8000-00805F9B34FB", typeof(BTStandard_HeartRateControl)),

        };
        public static SupportedDevice GetSupported(BluetoothWatcher.AdvertismentWatcher.WatcherData advertisement)
        {
            var name = advertisement.BestName;
            foreach (var device in Devices)
            {
                if (device.Matches(advertisement))
                {
                    return device;
                }
            }

            return null;
        }
    }
}
