using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

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
            UIThreadHelper.CallOnUIThread(this, () =>
            {
                uiLog.Text += str + "\n";
            });
        }
        private void LogCopyable(string str)
        {
            UIThreadHelper.CallOnUIThread(this, () =>
            {
                uiLogImportantData.Text += str + "\n";
            });
       }
        private void LogClear(string header = "")
        {
            UIThreadHelper.CallOnUIThread(this, () =>
            {
                uiLog.Text = header;
            });
        }
        public MainWindow()
        {
            InitializeComponent();
            Test();
            ListBluetooth(Options);
        }

        private void OnListComm(object sender, RoutedEventArgs e)
        {
            ListBluetooth(Options);
        }

        private async void ListBluetooth(UserOptions options)
        {
            LogClear();
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
                    LogCopyable(device.Name);
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
                LogCopyable(firstMatch.Id);
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

            switch (bt.ConnectionStatus)
            {
                case BluetoothConnectionStatus.Disconnected:
                    Log("Device is unconnected. Press the power button to connect?");
                    break; ;
            }
            Log("Step 80: Get the RfcommServces");
            // All serial services are:
            // SPP: 00001101-0000-1000-8000-00805f9b34fb
            // 00000000-deca-fade-deca-deafdecacaff	Accessory-side MFi/iAP(2) protocol https://wiomoc.de/misc/posts/mfi_iap.html
            // See https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.rfcomm.rfcommserviceid?view=winrt-26100 for sample list.

            RfcommDeviceService? serviceRfcomm = null;
            // RfcommDeviceService API: https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.rfcomm.rfcommdeviceservice?view=winrt-26100
            try
            {
                var rfcommServices = await bt.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort, BluetoothCacheMode.Uncached);
                Log($"RfcommServices uncached count={rfcommServices.Services.Count}");

                rfcommServices = await bt.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort, BluetoothCacheMode.Cached);
                //var rfcommServices = await bt.GetRfcommServicesAsync(BluetoothCacheMode.Cached);
                Log($"RfcommServices cached count={rfcommServices.Services.Count}");


                foreach (var service in rfcommServices.Services)
                {
                    if (serviceRfcomm == null) serviceRfcomm = service;
                    Log($"Service: name={service.ConnectionServiceName}");
                    Log($"    id={service.ServiceId.Uuid} host={service.ConnectionHostName}");
                    LogCopyable(service.ServiceId.Uuid.ToString());
                }
            }
            catch (Exception ex)
            {
                Log($"Can't get BT comm services: reason={ex.Message}");
                return;
            }

            if (serviceRfcomm == null)
            {
                Log($"There are no serial services avaiable.");
                return;
            }
            cts = new CancellationTokenSource();
            StreamSocket? socket = await MakeConnectedSocket(serviceRfcomm);
            if (socket == null) return;

            var istream = socket.OutputStream;

            readAll = Task.Run(async () =>
            {
                var ct = cts.Token;
                var dr = new DataReader(socket.InputStream);
                dr.InputStreamOptions = InputStreamOptions.Partial;
                const int READBUFFER = 2000;
                bool keepGoing = true;
                while (!ct.IsCancellationRequested && keepGoing)
                {
                    uint n = 0;
                    try
                    {
                        Log($"trying to read");
                        n = await dr.LoadAsync(READBUFFER).AsTask(ct);
                        Log($"read OK");
                    }
                    catch (TaskCanceledException)
                    {
                        n = 0;
                        keepGoing = false;
                    }
                    catch (Exception ex)
                    {
                        Log($"ERROR: Receiving: {ex.Message}\r");
                        Log($"Socket read failed");
                        n = 0;
                        keepGoing = false;
                    }
                    if (n > 0)
                    {
                        // Got some data from the device
                        var str = dr.ReadString(dr.UnconsumedBufferLength);

                        // Give the string to the terminal
                        Log(str);
                    }
                    else
                    {
                        keepGoing = false;
                    }
                }
                readAll = null; // The task is going away; null it out.
            });

            Log("All steps completed");
        }

        /// <summary>
        /// Requires the Bluetooth capability (otherwise it dies at the ConnectAsync with HResult 8007277c which is barely documented)
        /// </summary>
        /// <param name="serviceRfcomm"></param>
        /// <returns></returns>
        private async Task<StreamSocket?> MakeConnectedSocket(RfcommDeviceService serviceRfcomm)
        {
            StreamSocket? socket = new StreamSocket();
            var istream = socket.OutputStream;
            try
            {
                Log($"Connecting to device");
                await socket.ConnectAsync(serviceRfcomm.ConnectionHostName, serviceRfcomm.ConnectionServiceName);
                Log($"Connection OK");
            }
            catch (Exception ex)
            {
                Log($"Exception connecting to Rfcomm/Spp. Exception {ex.Message}\r");
                Log($"Failed to connect");
                socket = null;
            }
            return socket;
        }

        Task? readAll;
        CancellationTokenSource cts = new CancellationTokenSource();

    }
}

namespace Utilities
{
    static class UIThreadHelper
    {
        /// <summary>
        /// Returns TRUE iff the current thread is the UI thread
        /// </summary>
        /// <returns></returns>
        public static bool IsOnUIThread()
        {
            var dispather = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            var retval = dispather.HasThreadAccess;
            return retval;
        }

        /// <summary>
        /// Calls the given function either directly or on the UI thread the TryRunAsync(). The resulting task is just thrown away.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="priority"></param>
        public static void CallOnUIThread(Window w, Action f, Microsoft.UI.Dispatching.DispatcherQueuePriority priority = Microsoft.UI.Dispatching.DispatcherQueuePriority.Low)
        {
            bool isQueued = w.DispatcherQueue.TryEnqueue(priority,() => { f(); } );
        }
    }
}
