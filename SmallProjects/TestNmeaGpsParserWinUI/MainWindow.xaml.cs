using BluetoothDeviceController.SerialPort;
using BluetoothProtocolsSerial.Serial;
using Microsoft.UI.Xaml;
using Parsers.Nmea;
using System;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Enumeration;
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
        public static MainWindow? MainWindowWindow = null;
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
                uiGps.SetDeviceStatus(str);
                //uiLog.Text += str + "\n";
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
            ;
        }

        public bool MainWindowIsClosed = false;
        public MainWindow()
        {
            MainWindowWindow = this;
            MainWindowWindow.Closed += (s, e) => { MainWindowIsClosed = true; };
            InitializeComponent();
            Test();

            DoAll(uiGps, Options);
        }

        private void OnListComm(object sender, RoutedEventArgs e)
        {
            DoAll(uiGps, Options);
        }

        /* Don't need the picker
var picker = new DevicePicker();
WinRT.Interop.InitializeWithWindow.Initialize(picker, WinRT.Interop.WindowNative.GetWindowHandle(this));
picker.Filter.SupportedDeviceSelectors.Add(aqs);
var selected = await picker.PickSingleDeviceAsync(new Rect(50, 200, 10, 10)); // Rect is where it flies out from
*/

        private async void DoAll(ITerminal terminal, RfcommOptions options) // terminal (ITerminal) is a GpsControl
        {
            terminal.SetDeviceStatusEx(ConnectionState.UX, ConnectionSubstate.UXReset);

            DeviceInformation? deviceInfoSelected = null;
            var discoveryType = BluetoothCommTerminalAdapter.DiscoveryType.BluetoothDevice;

            // Find a device to connect to
            try
            {
                terminal.SetDeviceStatusEx(ConnectionState.ScanningForDevices, ConnectionSubstate.SfdStarted);

                int nNotMatch = 0;
                int nMatch = 0;
                var aqs = BluetoothCommTerminalAdapter.GetDeviceSelector(discoveryType);
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

                        //Log($"Info: Not matching: device name={device.Name}");
                        //LogCopyable(device.Name);
                    }
                }
                Log($"List complete. N. Match={nMatch} Not matching={nNotMatch}");

                // Now let's try to connect
                if (deviceInfoSelected == null)
                {
                    terminal.SetDeviceStatusEx(ConnectionState.ScanningForDevices, ConnectionSubstate.SfdNoDeviceFound);
                    return;
                }
                terminal.SetDeviceStatusEx(ConnectionState.ScanningForDevices, ConnectionSubstate.SfdCompletedOk, "", nMatch);
            }
            catch (Exception ex)
            {
                terminal.SetDeviceStatusEx(ConnectionState.ScanningForDevices, ConnectionSubstate.SfdException, ex.Message);
                return;
            }



            BtTerminalAdapter = new BluetoothCommTerminalAdapter(terminal, deviceInfoSelected, discoveryType);
            await BtTerminalAdapter.EnsureRfcommService();
            if (BtTerminalAdapter.RfcommDeviceService == null)
            {
                terminal.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcNoServices);
                return;
            }
            terminal.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcCompletedOk);


            BtTerminalAdapter.cts = new CancellationTokenSource();

            IInputStream? inputStream = null;

            await BtTerminalAdapter.EnsureConnectedSocket(); // Duplicates Handles all the ConnectingToDevice state

            if (BtTerminalAdapter.Socket == null)
            {
                return;
            }

            inputStream = BtTerminalAdapter.Socket.InputStream;

            terminal.SetDeviceStatusEx(ConnectionState.SendingAndReceiving, ConnectionSubstate.SRStarted);
            readAll = Task.Run(async () =>
            {
                // await BluetoothSocketHelper.ReadLines(this, BtTerminalAdapter.Socket, inputStream, BtTerminalAdapter.cts.Token);
                await BluetoothSocketHelper.ReadLines(uiGps, inputStream, BtTerminalAdapter.cts.Token);
            });
                //Log("All steps completed");
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

#if NEVER_EVER_DEFINED
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
#endif
        public static async Task ReadLines(ITerminal terminal, /* StreamSocket socket, */ IInputStream inputStream, CancellationToken ct)
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
                    terminal.SetDeviceStatusEx(ConnectionState.SendingAndReceiving, ConnectionSubstate.SRWaitingForData);
                    n = await dr.LoadAsync(READBUFFER).AsTask(ct);
                    terminal.SetDeviceStatusEx(ConnectionState.SendingAndReceiving, ConnectionSubstate.SRGotData, "", n);
                }
                catch (TaskCanceledException)
                {
                    n = 0;
                    keepGoing = false;
                    terminal.SetDeviceStatusEx(ConnectionState.SendingAndReceiving, ConnectionSubstate.SRCancelled);
                }
                catch (Exception ex)
                {
                    terminal.SetDeviceStatusEx(ConnectionState.SendingAndReceiving, ConnectionSubstate.SRException, ex.Message);
                    n = 0;
                    keepGoing = false;
                }
                if (n > 0)
                {
                    // Got some data from the device
                    var str = oldLine + dr.ReadString(dr.UnconsumedBufferLength);
                    oldLine = "";
                    if (str.Contains("\r\n"))
                    {
                        bool lastHasCRLF = str.EndsWith("\r\n");

                        var lines = str.Split("\r\n");
                        int lastIndex = lines.Length - 1;
                        terminal.ReceivedData(lines[0]);
                        if (lines.Length > 1)
                        {
                            for (int i = 1; i <= lastIndex; i++)
                            {
                                bool isLast = i == lastIndex;
                                if (!isLast || lastHasCRLF)
                                {
                                    terminal.ReceivedData(lines[i]);
                                }
                            }
                        }
                        oldLine = lastHasCRLF ? "" : lines[lastIndex];
                    }
                    else
                    {
                        oldLine = str;
                        // Pause a little bit and let the GPS buffer a little bit
                        await Task.Delay(250); // time is in milliseconds
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
            if (w == null || w.DispatcherQueue == null) return;

            bool isQueued = w.DispatcherQueue.TryEnqueue(priority,() => { f(); } );
        }
    }
}
