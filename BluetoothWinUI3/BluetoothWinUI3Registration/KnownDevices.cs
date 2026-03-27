using BluetoothWatcher.AdvertismentWatcher;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.BluetoothWinUI3Registration
{
    /// <summary>
    /// A known device is a supported device. For example, a user with two Thingys will have
    /// a single supported device (a thingy) and two known devices (for each actual thingy).
    /// The known device includes the WatcherData from the advertisement and the SupportedDevice
    /// and also the UserControl for good measure.
    /// </summary>
    public class KnownDevice
    {
        public KnownDevice(WatcherData advertisement, UserControl control, SupportedDevice supported)
        {
            Advertisement = advertisement;
            Control = control;
            Supported = supported;
        }
        public WatcherData Advertisement { get; set;  }
        public UserControl Control { get; set; }
        public SupportedDevice Supported { get; set; }
    }

    public static class KnownDevices
    {
        public static List<KnownDevice> Devices { get; } = new List<KnownDevice>();
    }
}
