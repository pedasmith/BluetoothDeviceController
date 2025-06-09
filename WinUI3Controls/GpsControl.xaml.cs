using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BluetoothDeviceController.SerialPort;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation.Diagnostics;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{
    public sealed partial class GpsControl : UserControl, ITerminal
    {
        public GpsControl()
        {
            this.InitializeComponent();
        }

        private void Log(string message)
        {
            uiLog.Text += message + "\n";
        }

        public event TerminalSendDataEventHandler OnSendData;

        public void ErrorFromDevice(string error)
        {
            Log(error);
        }

        public void ReceivedData(string data)
        {
            Log(data);
        }

        public void SetDeviceStatus(string status)
        {
            Log(status);
        }
        public void SetDeviceStatusEx(TerminalSupport.ConnectionState status, TerminalSupport.ConnectionSubstate substate, string text, double value)
        {
            var icon = TerminalSupport.StateAsIcon(status, substate);
            uiIcon.Text = icon;
            uiSubstatus.Text = TerminalSupport.StateAsString(status, substate, value);
            uiStatus.Text = status.ToString();
        }
    }
}
