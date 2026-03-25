using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif




namespace BluetoothWinUI3
{

    public sealed partial class BTConnectionControl : UserControl
    {
        public enum ConnectionState
        {
            FoundViaAdvertisement,
            Connecting,
            Connected,
            Disconnecting,
            Disconnected,
        }

        public ConnectionState CurrState { get; internal set; }
        BluetoothWatcher.AdvertismentWatcher.WatcherData Advertisement;

        public BTConnectionControl()
        {
            InitializeComponent();
        }


        public void SetAdvertisementData(BluetoothWatcher.AdvertismentWatcher.WatcherData data)
        {
            Advertisement = data;
            CurrState = ConnectionState.FoundViaAdvertisement;
        }

        private void OnConnect(object sender, RoutedEventArgs e)
        {
            switch (CurrState)
            {
                case ConnectionState.FoundViaAdvertisement:
                    CurrState = ConnectionState.Connecting;
                    break;
            }
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
