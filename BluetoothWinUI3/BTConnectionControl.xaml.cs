using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Utilities;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace BluetoothWinUI3
{
    public class ConnectionChangedEventArgs : EventArgs
    {
        public BluetoothLEDevice Device;
        public WatcherData CurrWatcherData;
        public BTConnectionControl.ConnectionState NewConnectionState;

        public ConnectionChangedEventArgs(BluetoothLEDevice le, WatcherData watcherData, BTConnectionControl.ConnectionState newConnectionState)
        {
            Device = le;
            CurrWatcherData = watcherData;
            NewConnectionState = newConnectionState;
        }
    }


    public sealed partial class BTConnectionControl : UserControl
    {
        public enum ConnectionState
        {
            /// <summary>
            /// Only used by the BTServicesCharacteristics control. Normal devices are either
            /// purely advertisement and just stay as Disconnected or are normal devices
            /// and are drive by the control
            /// </summary>
            FoundViaAdvertisement,
            Connecting,
            ConnectionFailed,
            Connected,
            Disconnecting,
            Disconnected,
        }
        public enum AutoReconnectType {  None, Advertisement, EveryMinuteFor5TimesAfterConnect, }
        public AutoReconnectType CurrAutoReconnectType { get; set; } = AutoReconnectType.Advertisement;
        public int NConnect { get; internal set; } = 0;
        public int NAutoRetryCount { get; internal set; } = 0;
        private Task RetryTask = null;


        /// <summary>
        /// Set by the different Controls to distribute the ConnectionChanged value.
        /// 2026-08-09: Actually is just used by the BTServicesCharacteristics display
        /// </summary>
        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;
        public void OnConnectionChanged()
        {
            ConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(CurrLEDevice, CurrWatcherData, CurrState));
        }

        ConnectionState _CurrState = ConnectionState.Disconnected;
        /// <summary>
        /// The current connection state. Is often "Disconnected" or "FoundViaAdvertisement"
        /// For many controls, the control will set this value
        /// </summary>
        public ConnectionState CurrState { 
            get { return _CurrState; } 
            internal set 
            { 
                if (value == _CurrState) return; 
                _CurrState = value;
                if (value == ConnectionState.Connected)
                {
                    NAutoRetryCount = 0;
                    NConnect++;
                }
                else if (value == ConnectionState.Disconnected)
                {
                    PotentiallyRetryConnect();
                }
                UpdateIcon();  
                OnConnectionChanged(); 
            } 
        }

        public void SetState(BluetoothConnectionStatus value)
        {
            switch (value)
            {
                case BluetoothConnectionStatus.Connected:
                    CurrState = ConnectionState.Connected;
                    break;
                case BluetoothConnectionStatus.Disconnected:
                    CurrState = ConnectionState.Disconnected;
                    break;
            }
        }

        public void SetState(BluetoothCommunicationStatus value)
        {
            switch (value.Status)
            {
                case Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success:
                    CurrState = ConnectionState.Connected;
                    break;
                default: // everything else is bad: Unreachable ProtocolError AccessDenied
                    CurrState = ConnectionState.Disconnected;
                    break;
            }
        }


        public void SetConnectVisibility(Visibility visibility)
        {
            uiConnectInfo.Visibility = visibility;
        }

        public void SetBatteryVisibility(Visibility visibility)
        {
            uiBatteryLevelIcon.Visibility = visibility;
            uiBatteryLevelPercent.Visibility = visibility;
        }

        /// <summary>
        /// Most recent advertisement with an address that will be connected to (or not!)
        /// </summary>
        BluetoothWatcher.AdvertismentWatcher.WatcherData CurrWatcherData;

        BluetoothLEDevice CurrLEDevice;

        public BTConnectionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Called by, e.g., BTServicesCharacteristics. Sets the advertisement that we
        /// might use later to do a connection.
        /// </summary>
        /// <param name="data"></param>
        public void SetAdvertisementData(BluetoothWatcher.AdvertismentWatcher.WatcherData data)
        {
            CurrWatcherData = data;
            CurrLEDevice = null;
            uiStatus.Text = $"Selected {CurrWatcherData.AddressAsString} {CurrWatcherData.BestName}";
            CurrState = ConnectionState.FoundViaAdvertisement; // will trigger events
        }

        private IDeviceControlBasic DeviceControlBasic = null;
        public void SetDeviceControl(IDeviceControlBasic value)
        {
            DeviceControlBasic = value;
        }

        /// <summary>
        /// Called by a device control when a device is disconnected. Disconnection is often 
        /// discovered via Device.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged
        /// TODO: hook this up??
        /// </summary>
        public async Task DeviceDisconnected()
        {
            CurrState = ConnectionState.Disconnected;
        }


        /// <summary>
        /// Called by the Connect button and will start a connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnConnect(object sender, RoutedEventArgs e)
        {
            CurrLEDevice = null;
            switch (CurrState)
            {
                // FoundViaAdvertisement is only used by the BTServicesCharacteristics control
                case ConnectionState.FoundViaAdvertisement:
                    if (CurrWatcherData == null)
                    {
                        uiStatus.Text = $"Unable to connect; there isn't a Bluetooth advertisement";
                        CurrState = ConnectionState.ConnectionFailed;
                        CurrState = ConnectionState.FoundViaAdvertisement;
                        return;
                    }

                    CurrState = ConnectionState.Connecting;
                    var addr = CurrWatcherData.Addr;
                    CurrLEDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(addr);
                    if (CurrLEDevice == null)
                    {
                        CurrState = ConnectionState.ConnectionFailed;
                        CurrState = ConnectionState.FoundViaAdvertisement;
                        Log($"Unable to connect to {CurrWatcherData.AddressAsString}");
                        return;
                    }
                    CurrState = ConnectionState.Connected; // It's not really connected at this point ... 

                    break;

                case ConnectionState.Connected:
                case ConnectionState.Disconnected:
                    await DeviceControlBasic?.ReconnectAsync();
                    break;
            }
        }

        private void Log(String str)
        {
            uiStatus.Text = str;
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }

        /// <summary>
        /// level is a percentage from 0 to 100 (inclusive).
        /// </summary>
        /// <param name="level"></param>
        public void SetBatteryLevel(double level)
        {
            var icon = BluetoothWinUI3.Units.BatteryLevelIcon.Icon(level);
            uiBatteryLevelIcon.Text = icon;
            uiBatteryLevelPercent.Text = $"{level:F0}%";
        }

        private void UpdateIcon()
        {
            UIThreadHelper.CallOnUIThread(() =>
            {
                switch (CurrState)
                {
                    case ConnectionState.FoundViaAdvertisement:
                        uiIcon.Text = "🔷"; // Adv
                        uiConnectRing.IsActive = false;
                        break;
                    case ConnectionState.Connecting:
                        uiIcon.Text = "⟳"; //  ..c";
                        uiConnectRing.IsActive = true;
                        break;
                    case ConnectionState.Connected:
                        uiIcon.Text = "✔"; //  Con";
                        uiConnectRing.IsActive = false;
                        break;
                    case ConnectionState.Disconnecting:
                        uiIcon.Text = "𝗑🗲";
                        break;
                    case ConnectionState.Disconnected:
                        uiIcon.Text = "🗙"; // Dis";
                        uiConnectRing.IsActive = false;
                        break;
                }
            });
        }

        public async Task GotAnotherAdvertisementAsync()
        {
            if (DeviceControlBasic == null) return;
            if (NConnect == 0) return;
            if (CurrAutoReconnectType != AutoReconnectType.Advertisement) return;
            if (CurrState != ConnectionState.Disconnected) return;
            await DeviceControlBasic.ReconnectAsync();
        }

        private void PotentiallyRetryConnect()
        {
            if (DeviceControlBasic == null) return; // Can't reconnect automatically if there's nobody to do it
            if (NConnect == 0) return; // we've never connected; let's not retry now
            switch (CurrAutoReconnectType)
            {
                case AutoReconnectType.None:
                    return; // our parent control thinks we should not retry
                case AutoReconnectType.Advertisement:
                    return; // wait for an advertisement to reconnect
                case AutoReconnectType.EveryMinuteFor5TimesAfterConnect:
                    RetryTask = new Task(async () =>
                    {
                        await Task.Delay(60_000); // wait one minute
                        //await Task.Delay(5_000); // wait one minute

                        // Maybe we have already reconnected -- e.g., the user clicked connect or the
                        // device sent an advert which triggers reconnect.
                        if (CurrState != ConnectionState.Disconnected) return; // user click connect?
                        NAutoRetryCount++;
                        if (NAutoRetryCount > 5) return; // no reason to retry
                        UIThreadHelper.CallOnUIThread(async () =>
                        {
                            await DeviceControlBasic.ReconnectAsync();
                        });
                    });
                    RetryTask.Start();
                    break;
            }
        }
    }
}
