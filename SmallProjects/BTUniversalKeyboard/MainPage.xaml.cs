using BluetoothDeviceController;
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

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
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
            MenuDeviceInformationWatcher.Start();
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
        }
        DeviceInformationWrapper CurrKeyboardDevice = null;
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            // Works OK. Log($"Got device: {args.Name}");
            if (args.Name.StartsWith("BTUnicode Keyboard"))
            {
                Log($"Found the keyboard! Stopping!");
                MenuDeviceInformationWatcher.Stop();

                // Connect to the device.
                CurrKeyboardDevice = new DeviceInformationWrapper(args);
                UIThreadHelper.CallOnUIThread(
                    async () =>
                    {
                        await uiKeyboard.DoInitializeAsync(CurrKeyboardDevice);
                    });
                
#if NEVER_EVER_DEFINED
                var ble = await BluetoothLEDevice.FromIdAsync(args.Id);
                Keyboard_BTUnicode = new BluetoothProtocols.Keyboard_BTUnicode();
                Keyboard_BTUnicode.ble = ble;
                Keyboard_BTUnicode.PropertyChanged += Keyboard_BTUnicode_PropertyChanged;
                Keyboard_BTUnicode.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
                //await Keyboard_BTUnicode.EnsureCharacteristicAsync(BluetoothProtocols.Keyboard_BTUnicode.CharacteristicsEnum.All_enum);
                Keyboard_BTUnicode.KeyPressEvent += Keyboard_BTUnicode_KeyPressEvent;
                var status = await Keyboard_BTUnicode.NotifyKeyPressAsync();
                Log($"Notify status: status={status}");
#endif
            }
        }

        private void Keyboard_BTUnicode_KeyPressEvent(BluetoothDeviceController.BleEditor.ValueParserResult data)
        {
            Log($"Keypress: {data.UserString}");
        }

        private void Status_OnBluetoothStatus(object source, BluetoothProtocols.BluetoothCommunicationStatus status)
        {
            Log($"Status: {status.AsStatusString}");
        }

        private void Keyboard_BTUnicode_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // What property?
            Log($"Change: {e.PropertyName}");
        }

        private void Timeout_BluetoothLEAdvertisementWatcher_Tick(object sender, object e)
        {
        }
    }

    class UserPreferences
    {
        public double AdvertisementScanTimeInMilliseconds { get; set; } = 5_000;
    }
}
