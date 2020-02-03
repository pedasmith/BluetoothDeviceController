using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.SerialPort
{
    class nRFUartTerminalAdapter
    {
        /// <summary>
        /// Don't forget to call InitAsync() to start reading from device+terminal and passing data from one to the other.
        /// </summary>
        /// <param name="terminal"></param>
        /// <param name="input_nrf"></param>
        public nRFUartTerminalAdapter(ITerminal terminal, CraftyRobot_Smartibot input_nrf)
        {
            Terminal = terminal;
            nrf = input_nrf;
        }
        ITerminal Terminal;
        CraftyRobot_Smartibot nrf { get; set; } = null;
        public RfcommDeviceService serviceRfcomm { get; internal set; } = null;
        public DataWriter dw { get; internal set; } = null;
        private Task readAll { get; set; } = null;
        private CancellationTokenSource cts { get; set; } = null;
        bool connected = false;

        public async Task InitAsync()
        {
            if (nrf == null) return;

            try {
                Terminal?.SetDeviceStatus($"Connecting to device");
                Terminal?.SetDeviceStatus($"Connection OK");
            }
            catch (Exception ex)
            {
                Terminal?.ErrorFromDevice($"Exception connecting to Rfcomm/Spp. Exception {ex.Message}\r");
                Terminal?.SetDeviceStatus($"Failed to connect");
            }
            //
            // DO THE READING AND WRITING
            //

            if (Terminal != null)
            {
                // Terminal has an event which it triggers every time the user enters a new line of text.
                Terminal.OnSendData += async (sender, data) =>
                {
                    if (nrf != null)
                    {
                        try
                        {
                            await nrf.WriteTransmit(data);
                            Terminal?.SetDeviceStatus($"Send OK");
                        }
                        catch (Exception ex)
                        {
                            Terminal?.ErrorFromDevice($"ERROR: Sending: {ex.Message}\r");
                            Terminal?.SetDeviceStatus($"Failed to send");
                        }
                    }
                };
            }

            if (cts == null)
            {
                cts = new CancellationTokenSource();
            }
            await nrf.NotifyReceiveAsync();
            nrf.ReceiveEvent += Nrf_ReceiveEvent;
            await Task.Delay(0); // make the compiler be quiet!
        }

        private void Nrf_ReceiveEvent(BleEditor.ValueParserResult data)
        {
            var str = data.UserString;
            Terminal?.ReceivedData(str);
            Terminal?.SetDeviceStatus($"read OK");
        }
    }
}
