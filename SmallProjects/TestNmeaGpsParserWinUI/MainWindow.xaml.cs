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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

using Parsers.Nmea;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestNmeaGpsParserWinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        class UserOptions
        {
            public string Match = "*";
            public bool Matches(string devicename)
            {
                if (Match == "*") return true;

                var nameup = devicename.ToUpper();
                var matchup = Match.ToUpper();
                if (nameup.Contains(matchup)) return true;
                return false;
            }
        }
        UserOptions Options = new UserOptions()
        {
            Match = "xgps150"
        };

        /// <summary>
        /// Calls all of the internal static self-test methods.
        /// </summary>
        /// <returns>Number of errors; should always be 0</returns>
        static int Test()
        {
            int nerror = 0;
            nerror += Nmea_Data.Test();

            return nerror;
        }

        private void Log(string str)
        {
            uiLog.Text += str + "\n";
        }
        public MainWindow()
        {
            InitializeComponent();
            Test();
        }

        private void OnListComm(object sender, RoutedEventArgs e)
        {
            ListBluetooth(Options);
        }

        private async void ListBluetooth(UserOptions options)
        {
            int nNotMatch = 0;
            int nMatch = 0;
            DeviceInformation? firstMatch = null;
            DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            foreach (DeviceInformation? device in PairedBluetoothDevices)
            {
                if (options.Matches(device.Name))
                {
                    nMatch++;
                    Log($"Info: device name={device.Name}");
                    if (firstMatch == null) firstMatch = device;
                }
                else
                {
                    nNotMatch++;
                }
            }
            Log($"List complete. N. Match={nMatch} Not matching={nNotMatch}");

            // Now let's try to connect
            if (firstMatch == null) return;

            var accessStatus = DeviceAccessInformation.CreateFromId(firstMatch.Id);
            if (accessStatus.CurrentStatus != DeviceAccessStatus.Allowed)
            {
                Log($"Can't connect: access status={accessStatus.CurrentStatus}");
                return;
            }

            BluetoothDevice? bt = null;
            try
            {
                Log($"About to get device from id={firstMatch.Id}");
                bt = await BluetoothDevice.FromIdAsync(firstMatch.Id);
                Log($"Result: {bt}");
                if (bt == null)
                {
                    Log($"Unable to get BT device: FromId returned null");
                    return;
                }
                Log($"Got device: connection status={bt.ConnectionStatus} address={bt.BluetoothAddress}");
            }
            catch (Exception ex)
            {
                Log($"Can't get BT device: reason={ex.Message}");
                return;
            }
            return;
            try
            {
                var rfcommServices = await bt.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);
                Log($"RfcommServices count={rfcommServices.Services.Count}");
            }
            catch (Exception ex)
            {
                Log($"Can't get BT comm services: reason={ex.Message}");
                return;
            }

        }
    }
}
