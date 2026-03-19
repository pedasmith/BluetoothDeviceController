using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilities;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher AdvertisementWatcher = new BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher();
        int NAdvertisements = 0;
        public MainWindow()
        {
            UIThreadHelper.DQueue = this.DispatcherQueue;
            AdvertisementWatcher.WatcherEvent += AdvertisementWatcher_WatcherEvent;
            InitializeComponent();
        }

        private void AdvertisementWatcher_WatcherEvent(BluetoothLEAdvertisementWatcher sender, BluetoothWatcher.AdvertismentWatcher.WatcherData e)
        {
            NAdvertisements++;
            UIThreadHelper.CallOnUIThread(() =>
            {
                uiAdvertCount.Text = NAdvertisements.ToString();
                uiAdvertRaw.Text = e.ToString();
            });
        }

        private void OnAdvertisementStart(object sender, RoutedEventArgs e)
        {
            AdvertisementWatcher.Start();
        }
        private void OnAdvertisementStop(object sender, RoutedEventArgs e)
        {
            AdvertisementWatcher.Stop();
        }
    }
}
