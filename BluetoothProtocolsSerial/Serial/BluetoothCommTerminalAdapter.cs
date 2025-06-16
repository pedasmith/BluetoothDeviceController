#define zzzSERIAL_DEVICE_WORKS // 2025-06-07: Serial port stuff compiles but fails at run-time

using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace BluetoothDeviceController.SerialPort
{
    /// <summary>
    /// Connects the Windows RFCOMM serial port service to an "ITerminal" which can handle it.
    /// </summary>
    class BluetoothCommTerminalAdapter
    {
        public enum DiscoveryType {  BluetoothDevice, BluetoothRfcommDevice,
#if SERIAL_DEVICE_WORKS
            SerialDevice, // Does not work; error 80070079
#endif
        };
        public DiscoveryType CurrDiscoveryType = DiscoveryType.BluetoothDevice;
        public BluetoothCommTerminalAdapter(ITerminal terminal, DeviceInformation input_di, DiscoveryType discoveryType=DiscoveryType.BluetoothDevice)
        {
            Terminal = terminal;
            ChosenDeviceInfo = input_di; // BluetoothDevice or RfcommDevice
            CurrDiscoveryType = discoveryType;
        }
        ITerminal Terminal;
        DeviceInformation ChosenDeviceInfo { get; set; } = null;
        public BluetoothDevice BluetoothDevice { get; internal set; } = null;
        public RfcommDeviceService RfcommDeviceService { get; internal set; } = null;
#if SERIAL_DEVICE_WORKS
        public SerialDevice SerialDevice { get; internal set; } = null;
#endif
        public StreamSocket Socket { get; internal set; } = null;
        public DataWriter dw { get; internal set; } = null;
        private Task ReadAllTask { get; set; } = null;
        public CancellationTokenSource cts { get; set; } = null;

        public static string GetDeviceSelector(DiscoveryType discoveryType)
        {
            switch (discoveryType)
            {
                case DiscoveryType.BluetoothDevice: return BluetoothDevice.GetDeviceSelectorFromPairingState(true);
                case DiscoveryType.BluetoothRfcommDevice: return RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
#if SERIAL_DEVICE_WORKS
                case DiscoveryType.SerialDevice: return Windows.Devices.SerialCommunication.SerialDevice.GetDeviceSelector();
#endif
                default:
                    return "";
            }
        }

        /// <summary>
        /// Says "EnsureRfcommService"
        /// </summary>
        /// <returns></returns>
        public async Task EnsureRfcommService()
        {
            if (RfcommDeviceService == null)
            {
                switch (CurrDiscoveryType)
                {
                    case DiscoveryType.BluetoothDevice:
                        //Terminal?.SetDeviceStatus($"About to get device from id={ChosenDeviceInfo.Id}");
                        //Terminal?.SetDeviceStatus(ChosenDeviceInfo.Id);
                        Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcStarted, ChosenDeviceInfo.Name);

                        if (BluetoothDevice == null)
                        {
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcGettingDevice, ChosenDeviceInfo.Id);
                            BluetoothDevice = await Windows.Devices.Bluetooth.BluetoothDevice.FromIdAsync(ChosenDeviceInfo.Id);
                            //Terminal?.SetDeviceStatus($"Result: {BluetoothDevice}");
                            if (BluetoothDevice == null)
                            {
                                Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcNoDevice, ChosenDeviceInfo.Name);
                                return;
                            }
                            else
                            {
                                Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcGotDevice, ChosenDeviceInfo.Name);
                            }
                        }
                        else
                        {
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcReusingDevice, BluetoothDevice.ToString());
                            //Terminal?.SetDeviceStatus($"Reusing BluetoothDevice: {BluetoothDevice}");
                        }

                        //Terminal?.SetDeviceStatus($"Got device: connection status={BluetoothDevice.ConnectionStatus} address={BluetoothDevice.BluetoothAddress}");

                        try
                        {
                            var rfcommDeviceServices = await BluetoothDevice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort, BluetoothCacheMode.Uncached);
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcUncachedServiceCount, "", rfcommDeviceServices.Services.Count);

                            // bt is BluetoothDevice
                            rfcommDeviceServices = await BluetoothDevice.GetRfcommServicesAsync(BluetoothCacheMode.Cached);
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcCachedServiceCount, "", rfcommDeviceServices.Services.Count);
                            foreach (var rfcommDeviceService in rfcommDeviceServices.Services)
                            {
                                //if (RfcommDeviceService == null) RfcommDeviceService = rfcommDeviceService;
                                //Terminal?.SetDeviceStatus($"ServiceId={rfcommDeviceService.ServiceId.Uuid}");
                                //Terminal?.SetDeviceStatus($"Service: ConnectionHostName={rfcommDeviceService.ConnectionHostName}");
                                //Terminal?.SetDeviceStatus($"Service: ConnectionServiceName={rfcommDeviceService.ConnectionServiceName}");
                                //Terminal?.SetDeviceStatus($" ");
                                //Terminal?.SetDeviceStatus(rfcommDeviceService.ServiceId.Uuid.ToString());
                            }

                            rfcommDeviceServices = await BluetoothDevice.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort, BluetoothCacheMode.Cached);
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcCachedServiceCount, "", rfcommDeviceServices.Services.Count);

                            //Terminal?.SetDeviceStatus($"Cached Serial Port Services");

                            foreach (var rfcommDeviceService in rfcommDeviceServices.Services)
                            {
                                if (RfcommDeviceService == null) RfcommDeviceService = rfcommDeviceService;
                                //Terminal?.SetDeviceStatus($"ServiceId={rfcommDeviceService.ServiceId.Uuid}");
                                //Terminal?.SetDeviceStatus($"Service: ConnectionHostName={rfcommDeviceService.ConnectionHostName}");
                                //Terminal?.SetDeviceStatus($"Service: ConnectionServiceName={rfcommDeviceService.ConnectionServiceName}");
                                //Terminal?.SetDeviceStatus($" ");
                                //Terminal?.SetDeviceStatus(rfcommDeviceService.ServiceId.Uuid.ToString());
                            }

                            if (RfcommDeviceService == null)
                            {
                                Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcNoServices);
                            }
                            else
                            {
                                Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcCompletedOk);
                            }
                        }
                        catch (Exception ex)
                        {
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcException, ex.Message);
                            return;
                        }
                        break;

                    case DiscoveryType.BluetoothRfcommDevice:
                        // 
                        Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcStarted, ChosenDeviceInfo.Name);
                        RfcommDeviceService = await RfcommDeviceService.FromIdAsync(ChosenDeviceInfo.Id);
                        if (RfcommDeviceService == null)
                        {
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcNoServices);
                        }
                        else
                        {
                            Terminal?.SetDeviceStatusEx(ConnectionState.VerifyDeviceCapabilities, ConnectionSubstate.VdcCompletedOk);
                        }

                        break;
#if SERIAL_DEVICE_WORKS
                    case DiscoveryType.SerialDevice:
                        // https://learn.microsoft.com/en-us/uwp/api/windows.devices.serialcommunication.serialdevice?view=winrt-26100
                        // Error 80070079 when no serialDevice
                        Terminal?.SetDeviceStatus($"Getting Serial Port");
                        try
                        {
                            SerialDevice = await SerialDevice.FromIdAsync(ChosenDeviceInfo.Id);
                        }
                        catch (Exception ex)
                        {
                            Terminal?.SetDeviceStatus($"Error: {ex.Message}  {ex.HResult:X}");
                        }
                        break;
#endif
                }
            }
        }

        public async Task EnsureConnectedSocket()
        {
            if (Socket == null)
            {
                Socket = new StreamSocket();
                try
                {
                    Terminal?.SetDeviceStatusEx(ConnectionState.ConnectingToDevice, ConnectionSubstate.CtdStarted);
                    Terminal?.SetDeviceStatusEx(ConnectionState.ConnectingToDevice, ConnectionSubstate.CtdHostName, RfcommDeviceService.ConnectionHostName.ToString());
                    Terminal?.SetDeviceStatusEx(ConnectionState.ConnectingToDevice, ConnectionSubstate.CtdServiceName, RfcommDeviceService.ConnectionServiceName.ToString());

                    await Socket.ConnectAsync(RfcommDeviceService.ConnectionHostName, RfcommDeviceService.ConnectionServiceName);
                    Terminal?.SetDeviceStatusEx(ConnectionState.ConnectingToDevice, ConnectionSubstate.CtdCompletedOk);
                }
                catch (Exception ex)
                {
                    Terminal?.SetDeviceStatusEx(ConnectionState.ConnectingToDevice, ConnectionSubstate.CtdException, ex.Message + $"\tHRESULT={ex.HResult}");
                    Socket = null;
                }
            }
        }
        
        /// <summary>
        /// Will connect up a device. The "ChosenDeviceInfo" must already be set
        /// </summary>
        /// <returns></returns>
        public async Task InitAsync()
        {
            if (ChosenDeviceInfo == null)
            {
                return;
            }
            await EnsureRfcommService();

            if (RfcommDeviceService == null)
            {
                Terminal?.ErrorFromDevice($"Unable to create the RFCOMM service from {ChosenDeviceInfo.Id}\r");
                Terminal?.SetDeviceStatus($"Unable to connect");
                return;
            }


            await EnsureConnectedSocket();
            if (Socket == null)
            {
                return;
            }
            //
            // DO THE READING AND WRITING
            //

            if (Terminal != null)
            {
                // Terminal has an event which it triggers every time the user enters a new line of text.
                Terminal.OnSendData += async (sender, data) =>
                {
                    if (dw != null)
                    {
                        try
                        {
                            dw.WriteString(data);
                            await dw.StoreAsync();
                            await dw.FlushAsync();
                            Terminal?.SetDeviceStatus($"Send OK");
                        }
                        catch (Exception ex)
                        {
                            Terminal?.ErrorFromDevice($"ERROR: Sending: {ex.Message}\r");
                            Terminal?.SetDeviceStatus($"Failed to send");
                            //NOTE: cancel the socket completely
                        }
                    }
                };
            }

            if (cts == null)
            {
                cts = new CancellationTokenSource();
            }
            if (dw == null)
            {
                dw = new DataWriter(Socket.OutputStream);
            }
            if (ReadAllTask == null)
            {
                ReadAllTask = Task.Run(async () =>
                {
                    var ct = cts.Token;
                    var dr = new DataReader(Socket.InputStream);
                    dr.InputStreamOptions = InputStreamOptions.Partial;
                    const int READBUFFER = 2000;
                    bool keepGoing = true;
                    while (!ct.IsCancellationRequested && keepGoing)
                    {
                        uint n = 0;
                        try
                        {
                            Terminal?.SetDeviceStatus($"trying to read");
                            n = await dr.LoadAsync(READBUFFER).AsTask(ct);
                            Terminal?.SetDeviceStatus($"read OK");
                        }
                        catch (TaskCanceledException)
                        {
                            n = 0;
                            keepGoing = false;
                        }
                        catch (Exception ex)
                        {
                            Terminal?.ErrorFromDevice($"ERROR: Receiving: {ex.Message}\r");
                            Terminal?.SetDeviceStatus($"Socket read failed");
                            n = 0;
                            keepGoing = false;
                        }
                        if (n > 0)
                        {
                            // Got some data from the device
                            var str = dr.ReadString(dr.UnconsumedBufferLength);

                            // Give the string to the terminal
                            Terminal?.ReceivedData(str);
                        }
                        else
                        {
                            keepGoing = false;
                        }
                    }
                    ReadAllTask = null; // The task is going away; null it out.
                });
            }
        }
    }
}
