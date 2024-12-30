using BluetoothDeviceController;
using BTControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
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

namespace BTUniversalKeyboard
{
    public interface IReconnect
    {
        Task ReconnectAsync();
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IReconnect
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            uiKeyboard.Reconnect = this;
            await ReconnectAsync();
        }

        public async Task ReconnectAsync()
        {
            uiAnnunciator.Activity(AnnunciatorActivity.Initial);
            await Connect();
        }

        private void Log(string str)
        {
            Utilities.UIThreadHelper.CallOnUIThread(() => { uiLog.Text += str + "\n"; });
            System.Diagnostics.Debug.WriteLine(str);
        }

        BluetoothProtocols.Keyboard_BTUnicode Keyboard_BTUnicode { get; set; } = null;
        DeviceWatcher MenuDeviceInformationWatcher = null;
        DispatcherTimer Timeout_BluetoothLEAdvertisementWatcher = null;
        UserPreferences Preferences = new UserPreferences();
        private async Task Connect()
        {
            await Task.Delay(0);

            // Query for extra properties you want returned
            // See https://docs.microsoft.com/en-us/windows/desktop/properties/devices-bumper
            string[] requestedProperties = {
                "System.Devices.Aep.DeviceAddress",
                "System.Devices.Aep.CanPair",
                "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.IsPresent", // Because I often only want devices that are here right now.
                "System.Devices.Aep.SignalStrength",
                "System.Devices.Aep.Bluetooth.Le.Appearance",
                "System.Devices.Aep.Bluetooth.Le.IsConnectable",
                "System.Devices.GlyphIcon",
                "System.Devices.Icon",
            };
            string qstr = "System.Devices.Aep.ProtocolId:=\"{BB7BB05E-5972-42B5-94FC-76EAA7084D49}\"";
            MenuDeviceInformationWatcher = DeviceInformation.CreateWatcher(
                qstr,
                requestedProperties,
                DeviceInformationKind.AssociationEndpoint);

            if (Timeout_BluetoothLEAdvertisementWatcher == null)
            {
                var dt = new DispatcherTimer();
                dt.Tick += Timeout_BluetoothLEAdvertisementWatcher_Tick;
                dt.Interval = TimeSpan.FromMilliseconds(Preferences.AdvertisementScanTimeInMilliseconds);
                Timeout_BluetoothLEAdvertisementWatcher = dt;
            }
            // Always make sure timeout is set (might have been set by automation for long adverts)
            Timeout_BluetoothLEAdvertisementWatcher.Interval = TimeSpan.FromMilliseconds(Preferences.AdvertisementScanTimeInMilliseconds);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            MenuDeviceInformationWatcher.Added += DeviceWatcher_Added;
            MenuDeviceInformationWatcher.Updated += DeviceWatcher_Updated;
            MenuDeviceInformationWatcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            MenuDeviceInformationWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            MenuDeviceInformationWatcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            uiAnnunciator.Activity(AnnunciatorActivity.ScanStarted);
            MenuDeviceInformationWatcher.Start();
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            uiAnnunciator.Activity(AnnunciatorActivity.ScanStopped);
            //uiAnnunciator.SetStatus(AnnunciatorStatus.NoDeviceFound, "Scanner: Stopped");
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            uiAnnunciator.Activity(AnnunciatorActivity.ScanComplete);
            //uiAnnunciator.SetStatus(AnnunciatorStatus.NoDeviceFound, "Scanner: Completed");
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            uiAnnunciator.Activity(AnnunciatorActivity.ScanItemRemoved);
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            uiAnnunciator.Activity(AnnunciatorActivity.ScanItemUpdated);
        }
        DeviceInformation CurrKeyboardDevice = null;
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            // Works OK. Log($"Got device: {args.Name}");
            uiAnnunciator.Activity(AnnunciatorActivity.ScanItemAdded);
            if (args.Name.StartsWith("BTUnicode Keyboard"))
            {
                // Low value logging: Log($"Found the keyboard; stopping the scan");
                MenuDeviceInformationWatcher.Stop();
                //uiAnnunciator.SetStatus(AnnunciatorStatus.Connecting, "Found Keyboard");
                uiAnnunciator.Activity(AnnunciatorActivity.ScanItemFound);


                // Connect to the device.
                CurrKeyboardDevice = args;
                UIThreadHelper.CallOnUIThread(
                    async () =>
                    {
                        await uiKeyboard.DoInitializeAsync(CurrKeyboardDevice);
                    });
            }
        }


        private void Timeout_BluetoothLEAdvertisementWatcher_Tick(object sender, object e)
        {
            uiAnnunciator.Activity(AnnunciatorActivity.ScanTimerTick);
        }
    }

    class UserPreferences
    {
        public double AdvertisementScanTimeInMilliseconds { get; set; } = 5_000;
    }
}
