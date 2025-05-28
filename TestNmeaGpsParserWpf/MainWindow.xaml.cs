using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Parsers.Nmea;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;

namespace TestNmeaGpsParserWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        private void Log(string str)
        {
            uiLog.Text += str = "\n";
        }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnListComm(object sender, RoutedEventArgs e)
        {

        }

        private static async void ListBluetooth(UserOptions options)
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