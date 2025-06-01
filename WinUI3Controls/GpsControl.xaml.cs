using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BluetoothDeviceController.SerialPort;
using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinUI3Controls
{
    public sealed partial class GpsControl : UserControl, ITerminal
    {
        public GpsControl()
        {
            this.InitializeComponent();
        }

        public event TerminalSendDataEventHandler OnSendData;

        public void ErrorFromDevice(string error)
        {
            throw new NotImplementedException();
        }

        public void ReceivedData(string data)
        {
            throw new NotImplementedException();
        }

        public void SetDeviceStatus(string status)
        {
            throw new NotImplementedException();
        }
    }
}
