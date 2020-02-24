using BluetoothDeviceController.BleEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.Beacons
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimpleBeaconPage : Page
    {
        public SimpleBeaconPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformationWrapper;
            di.BleAdvert.UpdatedBleAdvertisement += BleAdvert_UpdatedBleAdvertisement;
        }

        private async void BleAdvert_UpdatedBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var eventTime = DateTime.Now;
                var time24 = eventTime.ToString("HH:mm:ss.f");

                var builder = new StringBuilder();
                builder.Append ($"{time24}\t{data.RawSignalStrengthInDBm}\n");
                foreach (var section in data.Advertisement.DataSections)
                {
                    var result = ValueParser.Parse(section.Data, "BYTES|HEX");
                    var str = $"section {section.DataType} data={result.AsString}\n";
                    builder.Append(str);
                }
                uiBeaconData.Text += builder.ToString();
            });
        }
    }
}
