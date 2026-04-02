using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.Utilities;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            MatchingName = match;
            FactoryInterface = factory;
        }
        public string MatchingName { get; set; } = "Thingy*"; // For example 

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type FactoryInterface { get; set; } = null;
        public bool Matches(WatcherData advertisement)
        {
            var name = advertisement.CompleteLocalName;
            var retval = name.StarMatch(MatchingName);
            return retval;
        }
    }

    /// <summary>
    /// List of all supported devices. A supported device is one that we know how to connect to and which has a corresponding Control
    /// </summary>
    public static class SupportedDevices
    {
        public static List<SupportedDevice> Devices { get; set; } = new List<SupportedDevice>()
        {
            new SupportedDevice("Thingy*", typeof(BTNordic_ThingyControl)),
        };
        public static SupportedDevice GetSupported(BluetoothWatcher.AdvertismentWatcher.WatcherData advertisement)
        {
            var name = advertisement.CompleteLocalName;
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
