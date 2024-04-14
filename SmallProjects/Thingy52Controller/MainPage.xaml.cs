using BluetoothDeviceController.Beacons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ThingyController
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }
        private void Log(string str)
        {
            UIThreadHelper.CallOnUIThread(() => { uiLog.Text += str + "\n"; });
        }

        BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            watcher.Received += OnAdvertisementReceived;
            watcher.Start();
        }

        private void OnAdvertisementReceived(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var addr = BluetoothAddress.AsString(args.BluetoothAddress);
            Log($"DBG: BT: Watcher: address={addr} name={args.Advertisement.LocalName} type={args.AdvertisementType}");
            if (args.Advertisement.LocalName.StartsWith("Thingy"))
            {
                // Found it. Cancel the watcher and power up the Thingy52Control
                watcher.Stop();
                UIThreadHelper.CallOnUIThread(() => { uiThingy.DataContext = args.BluetoothAddress; });
            }
        }


    }
}
