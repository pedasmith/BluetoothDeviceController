using BluetoothWatcher.AdvertismentWatcher;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            FoundViaAdvertisement,
            Connecting,
            ConnectionFailed,
            Connected,
            Disconnecting,
            Disconnected,
        }

        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;
        public void OnConnectionChanged()
        {
            ConnectionChanged?.Invoke(this, new ConnectionChangedEventArgs(CurrLEDevice, CurrWatcherData, CurrState));
        }

        ConnectionState _CurrState = ConnectionState.Disconnected;
        /// <summary>
        /// The current connection state. Is often "Disconnected" or "FoundViaAdvertisement"
        /// </summary>
        public ConnectionState CurrState { 
            get { return _CurrState; } 
            internal set { if (value == _CurrState) return; _CurrState = value; UpdateIcon();  OnConnectionChanged(); } 
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
                case ConnectionState.FoundViaAdvertisement:
                    if (CurrWatcherData == null)
                    {
                        uiStatus.Text = $"Unable to connect; there isn't a Bluetooth advertisement";
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
                    CurrState = ConnectionState.Connected;

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
            uiBatteryLevelPercent.Text = $"{level}%";
        }

        private void UpdateIcon()
        {
            switch (CurrState)
            {
                case ConnectionState.FoundViaAdvertisement:
                    uiIcon.Text = "Adv";
                    break;
                case ConnectionState.Connecting:
                    uiIcon.Text = "..c";
                    break;
                case ConnectionState.Connected:
                    uiIcon.Text = "Con";
                    break;
                case ConnectionState.Disconnecting:
                    uiIcon.Text = "..d";
                    break;
                case ConnectionState.Disconnected:
                    uiIcon.Text = "Dis";
                    break;
            }
        }
    }
}
