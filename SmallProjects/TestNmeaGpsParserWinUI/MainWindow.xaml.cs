using BluetoothDeviceController.SerialPort;
using BluetoothProtocolsSerial.Serial;
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
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Diagnostics;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestNmeaGpsParserWinUI
{
    interface IHandleStreamSocketLine
    {
        void Log(string str);
        void HandleLine(string line);
    }
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window, IHandleStreamSocketLine
    {

        RfcommOptions Options = new RfcommOptions()
        {
            Match = "xgps150"
        };
        BluetoothCommTerminalAdapter? BtTerminalAdapter = null;

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

        public void Log(string str)
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
            DoAll(uiGps, Options);
        }

        private void OnListComm(object sender, RoutedEventArgs e)
        {
            DoAll(uiGps, Options);
        }

        private async void DoAll(ITerminal terminal, RfcommOptions options)
        {
            terminal.SetDeviceStatusEx(TerminalSupport.ConnectionState.ScanningForDevices, TerminalSupport.ConnectionSubstate.ScanningForDevicesStarted);
            LogClear();
            int nNotMatch = 0;
            int nMatch = 0;
            DeviceInformation? deviceInfoSelected = null;

            var discoveryType = BluetoothCommTerminalAdapter.DiscoveryType.BluetoothDevice;
            //var discoveryType = BluetoothCommTerminalAdapter.DiscoveryType.SerialDevice; // Does not work; error 80070079


            // this is the working one: 2025-06-04 DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelectorFromPairingState(true));
            // There actually three ways to make this. I haven't tried the third way yet
            var aqs = BluetoothCommTerminalAdapter.GetDeviceSelector(discoveryType);

            var picker = new DevicePicker();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(this));
            picker.Filter.SupportedDeviceSelectors.Add(aqs);
            var selected = await picker.PickSingleDeviceAsync(new Rect(50, 200, 10, 10)); // Rect is where it flie out from

            DeviceInformationCollection PairedBluetoothDevices = await DeviceInformation.FindAllAsync(aqs);
            foreach (DeviceInformation? device in PairedBluetoothDevices)
            {
                // BluetoothDevice will be e.g. XGPS150-HEXHEX and BluetoothRfcommDevice will be e.g. SPP Dev
                if (options.Matches(device.Name))
                {
                    nMatch++;
                    Log($"Info: device name={device.Name}");
                    LogCopyable(device.Name);
                    if (nMatch == 1) deviceInfoSelected = device; // first one
                }
                else
                {
                    nNotMatch++;

                    Log($"Info: Not matching: device name={device.Name}");
                    LogCopyable(device.Name);

                    // what the heck, select it anyway.
                    if (deviceInfoSelected == null) deviceInfoSelected = device;
                }
            }
            Log($"List complete. N. Match={nMatch} Not matching={nNotMatch}");

            // Now let's try to connect
            if (deviceInfoSelected == null) return;

            BtTerminalAdapter = new BluetoothCommTerminalAdapter(terminal, deviceInfoSelected, discoveryType);


            await BtTerminalAdapter.EnsureRfcommService();
            if (BtTerminalAdapter.RfcommDeviceService == null)
            {
                Log($"There are no serial services avaiable.");
                return;
            }

            BtTerminalAdapter.cts = new CancellationTokenSource();

            IInputStream? inputStream = null;
#if NEVER_EVER_DEFINED
            // I can't get SerialDevice to work
            if (BtTerminalAdapter.SerialDevice != null)
            {
                istream = BtTerminalAdapter.SerialDevice.InputStream;
            }
#endif
            if (inputStream == null)
            {
                // Log some date from the RfcommDeviceService
                Log($"ConnectionHostName: {BtTerminalAdapter.RfcommDeviceService.ConnectionHostName}");
                Log($"ConnectionServiceName: {BtTerminalAdapter.RfcommDeviceService.ConnectionServiceName}");
                LogCopyable($"{BtTerminalAdapter.RfcommDeviceService.ConnectionHostName}");
                LogCopyable($"{BtTerminalAdapter.RfcommDeviceService.ConnectionServiceName}");

                await BtTerminalAdapter.EnsureConnectedSocket();

                if (BtTerminalAdapter.Socket == null) return;

                inputStream = BtTerminalAdapter.Socket.InputStream;
            }

            readAll = Task.Run( async () => 
            {
                // await BluetoothSocketHelper.ReadLines(this, BtTerminalAdapter.Socket, inputStream, BtTerminalAdapter.cts.Token);
                await BluetoothSocketHelper.ReadLines(this, inputStream, BtTerminalAdapter.cts.Token);
            });


            Log("All steps completed");
        }



        public void HandleLine(string line)
        {
            Log(line);
        }

        Task? readAll;
        CancellationTokenSource cts = new CancellationTokenSource();

    }
}

namespace Utilities
{
    static class BluetoothSocketHelper
    {
        /// <summary>
        /// Requires the Bluetooth capability (otherwise it dies at the ConnectAsync with HResult 8007277c which is barely documented)
        /// </summary>
        /// <param name="serviceRfcomm"></param>
        /// <returns></returns>
        public static async Task<StreamSocket?> MakeConnectedSocket(TestNmeaGpsParserWinUI.IHandleStreamSocketLine logger, RfcommDeviceService serviceRfcomm)
        {
            StreamSocket? socket = new StreamSocket();
            var istream = socket.OutputStream;
            try
            {
                logger.Log($"Connecting to device");
                await socket.ConnectAsync(serviceRfcomm.ConnectionHostName, serviceRfcomm.ConnectionServiceName);
                logger.Log($"Connection OK");
            }
            catch (Exception ex)
            {
                logger.Log($"Exception connecting to Rfcomm/Spp. Exception {ex.Message}\r");
                logger.Log($"Failed to connect");
                socket = null;
            }
            return socket;
        }

        public static async Task ReadLines(TestNmeaGpsParserWinUI.IHandleStreamSocketLine logger, /* StreamSocket socket, */ IInputStream inputStream, CancellationToken ct)
        {
            var oldLine = "";
            var dr = new DataReader(inputStream);
            dr.InputStreamOptions = InputStreamOptions.Partial;
            const int READBUFFER = 2000;
            bool keepGoing = true;
            while (!ct.IsCancellationRequested && keepGoing)
            {
                uint n = 0;
                try
                {
                    logger.Log($"trying to read");
                    n = await dr.LoadAsync(READBUFFER).AsTask(ct);
                    logger.Log($"read OK {n}");
                }
                catch (TaskCanceledException)
                {
                    n = 0;
                    keepGoing = false;
                }
                catch (Exception ex)
                {
                    logger.Log($"ERROR: Receiving: {ex.Message}\r");
                    logger.Log($"Socket read failed");
                    n = 0;
                    keepGoing = false;
                }
                if (n > 0)
                {
                    // Got some data from the device
                    var str = dr.ReadString(dr.UnconsumedBufferLength);
                    if (str.Contains("\r\n"))
                    {
                        bool lastHasCRLF = str.EndsWith("\r\n");

                        var lines = str.Split("\r\n");
                        int lastIndex = lines.Length - 1;
                        logger.HandleLine(oldLine + lines[0]);
                        if (lines.Length > 1)
                        {
                            for (int i = 1; i <= lastIndex; i++)
                            {
                                bool isLast = i == lastIndex;
                                if (!isLast || lastHasCRLF)
                                {
                                    logger.HandleLine(lines[i]);
                                }
                            }
                        }
                        oldLine = lastHasCRLF ? "" : lines[lastIndex];
                    }
                    else
                    {
                        oldLine += str;
                        // Pause a little bit and let the GPS buffer a little bit
                        await Task.Delay(50); // time is in milliseconds
                    }
                }
                else
                {
                    keepGoing = false;
                }
            }
        }
    }


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
