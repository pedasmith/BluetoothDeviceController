using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.SerialPort
{
    /// <summary>
    /// Connects the Windows RFCOMM serial port service to an "ITerminal" which can handle it.
    /// </summary>
    class BluetoothCommTerminalAdapter
    {
        public BluetoothCommTerminalAdapter(ITerminal terminal, DeviceInformation input_di)
        {
            Terminal = terminal;
            di = input_di;
        }
        ITerminal Terminal;
        DeviceInformation di { get; set; } = null;
        public RfcommDeviceService serviceRfcomm { get; internal set; } = null;
        public StreamSocket socket { get; internal set; } = null;
        public DataWriter dw { get; internal set; } = null;
        private Task readAll { get; set; } = null;
        private CancellationTokenSource cts { get; set; } = null; 
        
        public async Task InitAsync()
        {
            if (di == null) return;
            if (serviceRfcomm == null)
            {
                Terminal?.SetDeviceStatus($"Getting Rfcomm Device Service");
                serviceRfcomm = await RfcommDeviceService.FromIdAsync(di.Id);
            }
            if (serviceRfcomm == null)
            {
                Terminal?.ErrorFromDevice($"Unable to create the RFCOMM service from {di.Id}\r");
                Terminal?.SetDeviceStatus($"Unable to connect");
                return;
            }
            if (socket == null)
            {
                socket = new StreamSocket();
                try
                {
                    Terminal?.SetDeviceStatus($"Connecting to device");
                    await socket.ConnectAsync(serviceRfcomm.ConnectionHostName, serviceRfcomm.ConnectionServiceName);
                    Terminal?.SetDeviceStatus($"Connection OK");
                }
                catch (Exception ex)
                {
                    Terminal?.ErrorFromDevice($"Exception connecting to Rfcomm/Spp. Exception {ex.Message}\r");
                    Terminal?.SetDeviceStatus($"Failed to connect");
                    socket = null;
                }
            }
            if (socket == null) return;
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
                dw = new DataWriter(socket.OutputStream);
            }
            if (readAll == null)
            {
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
                    readAll = null; // The task is going away; null it out.
                });
            }
        }
    }
}
