using BluetoothWinUI3.BluetoothWinUI3Registration;
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
using System.Text;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public class UserPreferences
    {
        bool AutostartAdvertisementWatcher { get; set; } = true;
    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher AdvertisementWatcher = new BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher();
        int NAdvertisements = 0;
        UserPreferences CurrUserPrefs = new UserPreferences();
        public MainWindow()
        {

            UIThreadHelper.DQueue = this.DispatcherQueue;
            AdvertisementWatcher.WatcherEvent += AdvertisementWatcher_WatcherEvent;
            InitializeComponent();
            this.Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            AdvertisementWatcher.Start();
        }

        StringBuilder AllAdvertisements = new StringBuilder();
        Dictionary<string, string> UniqueAdvertisements = new Dictionary<string, string>();

        UserControl ThingyControl = null;
        private void AdvertisementWatcher_WatcherEvent(BluetoothLEAdvertisementWatcher sender, BluetoothWatcher.AdvertismentWatcher.WatcherData e)
        {
            // A little bit of logging and storing stuff for debugging
            NAdvertisements++;
            AllAdvertisements.AppendLine($"{NAdvertisements}, " + e.ToStringFull());
            var CanCompare = BluetoothWatcher.AdvertismentWatcher.WatcherData.AdvertisementStringFormat.CanCompare;
            UniqueAdvertisements[e.ToStringFull(CanCompare)] = e.ToStringFull();


            // Update the UI as needed and create the next device
            UIThreadHelper.CallOnUIThread(() =>
            {
                uiAdvertCount.Text = NAdvertisements.ToString();
                uiAdvertRaw.Text = e.ToString();



                // If this is a new advert, see if it's a known type
                var supportedDevice = BluetoothWinUI3.BluetoothWinUI3Registration.SupportedDevices.GetSupported(e);
                if (supportedDevice != null)
                {
                    // TODO: see if it's a known device AKA we already made the control.
                    // For now, we just support a single Thingy, so all those complications are short-circuited.
                    if (ThingyControl == null)
                    {
                        ThingyControl = Activator.CreateInstance(supportedDevice.FactoryInterface) as UserControl;
                        uiKnownDevices.Items.Add(ThingyControl);

                        var known = new KnownDevice(e, ThingyControl, supportedDevice);
                        KnownDevices.Devices.Add(known);
                        ThingyControl.DataContext = known;
                    }
                }
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

        private void OnAdvertisementCopy(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + AllAdvertisements.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }
        private void OnAdvertisementCopyUnique(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var item in UniqueAdvertisements.Values)
            {
                sb.AppendLine(item);
            }

            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + sb.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }

        private void OnFileExit(object sender, RoutedEventArgs e)
        {

        }

        private void OnHelpAbout(object sender, RoutedEventArgs e)
        {

        }

        private void OnDebugLoadDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Restore();

        }

        private void OnDebugSaveDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Save();

        }
    }
}
