using BluetoothWatcher.AdvertismentWatcher;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.BluetoothWinUI3Registration
{
    /// <summary>
    /// A known device is a supported device + per instance data include user-entered data. 
    /// For example, a user with two Thingys will have a single supported device (a thingy) 
    /// and two known devices (for each actual thingy). The known device includes the WatcherData 
    /// from the advertisement and the SupportedDevice and also the UserControl for good measure.
    /// 
    /// KnownDevice is transient: it's not saved between sessions at all. Saved data should be in
    /// SaveData. SaveData is linked to KnownDevice by the Id which is a BluetoothLEDevice DeviceId.
    /// </summary>
    public class KnownDevice
    {
        public KnownDevice(WatcherData advertisement, UserControl control, ZoomableDeviceControl container, SupportedDevice supported)
        {
            Advertisement = advertisement;
            Control = control;
            Container = container;
            Supported = supported;
        }
        public WatcherData Advertisement { get; set;  }
        public UserControl Control { get; set; }
        public ZoomableDeviceControl Container { get; set; }
        public SupportedDevice Supported { get; set; }

        public BluetoothLEDevice BTLEDevice { get; set; }
        /// <summary>
        /// Id is the BluetoothLEDevice DeviceId as assigned by Windows. The claim is that it's stable
        /// across multiple connections / sessions / reboots. This is what links a KnownDevice to the
        /// SaveData.
        /// This is set in the UserControl when the DataContext is set and the device is connected. In case of 
        /// failures, it will be a blank string (not null).

        /// </summary>
        public string Id { get; set; } = "";
    }

    public static class KnownDevices
    {
        public static KnownDevice Add(WatcherData advertisement, UserControl control, ZoomableDeviceControl container, SupportedDevice supported)
        {
            var known = new KnownDevice(advertisement, control, container, supported);
            AllDevices.Add(known);
            return known;
        }
        public static KnownDevice Get(WatcherData advertisement)
        {
            var existing = AllDevices.FirstOrDefault(kd => kd.Advertisement.Addr == advertisement.Addr);
            return existing; // might be null when it cannot be found
        }
        private static List<KnownDevice> AllDevices { get; } = new List<KnownDevice>();
    }
}
