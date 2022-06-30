using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Bluetooth.Advertisement;

#if NETCOREAPP
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Documents;
#else
using Windows.UI.Text;
using Windows.UI.Xaml.Documents;
#endif

namespace BluetoothProtocols.Beacons
{
    public class SimpleBeaconHistory
    {
        public static SimpleBeaconHistory MakeFromAdvertisement (BluetoothLEAdvertisementReceivedEventArgs bleAdvert, DateTime eventTime, string headerString, string displayString)
        {
            var retval = new SimpleBeaconHistory();
            retval.AdvertisementTime = eventTime;
            retval.Address = bleAdvert.BluetoothAddress;
            retval.Args = bleAdvert;
            retval.HeaderString = headerString;
            retval.DisplayString = displayString;


            return retval;
        }

        public Run[] MakeRun()
        {
            var run = new Run() { Text = DisplayString };
            if (HeaderString != null)
            {
                var bold = new Run() { Text = HeaderString, FontWeight = FontWeights.Bold };
                return new Run[] { bold, run };
            }
            return new Run[] { run };
        }
        public DateTimeOffset AdvertisementTime { get; set; }
        public ulong Address { get; set; }
        public string HeaderString { get; set; } = null;
        public string DisplayString { get; set; } = null;
        public BluetoothLEAdvertisementReceivedEventArgs Args { get; set; } = null;
    }
}
