using BluetoothDeviceController.SerialPort;
using BluetoothDeviceController.SpecialtyPages;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPagesCustom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CraftyRobot_SmartibotPage : Page, ITerminal, IGetPageDisplayPreferences, ISetHandleStatus
    {
        nRFUartTerminalAdapter TerminalAdapter;
        CraftyRobot_Smartibot bleDevice = new CraftyRobot_Smartibot();

        public CraftyRobot_SmartibotPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformationWrapper;
            if (di.SerialPortPreferences == null)
            {
                // Don't use the user-saved preferences; the Espruino has particular requirements
                // that we don't want the user to over-ride.
                di.SerialPortPreferences = new UserSerialPortPreferences()
                {
                    LineEnd = UserSerialPortPreferences.TerminalLineEnd.CR,
                    SavePrefix = "CraftyRobot_Smartibot_",
                };
            }
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive(false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;

            // Set up the terminal adapter, connecting the terminal control and the bluetooth device.
            TerminalAdapter = new nRFUartTerminalAdapter(uiTerminalControl, bleDevice);
            uiTerminalControl.ParentTerminal = this; 
            // the adapter tells the terminal control to display the status.
            // The terminal control would rather we display the status.

            await TerminalAdapter.InitAsync();
            uiTerminalControl.DI = di;
        }
        private IHandleStatus ParentStatusHandler = null;

        public void SetHandleStatus(IHandleStatus handleStatus)
        {
            ParentStatusHandler = handleStatus;
        }
        private void SetStatus(string status)
        {
            // // // uiStatus.Text = status;
            ParentStatusHandler?.SetStatusText(status);
        }
        private void SetStatusActive(bool isActive)
        {
            // // // uiProgress.IsActive = isActive;
            ParentStatusHandler?.SetStatusActive(isActive);
        }
        public void ReceivedData(string data)
        {
            uiTerminalControl.ReceivedData(data);
        }

        public void ErrorFromDevice(string error)
        {
            uiTerminalControl.ErrorFromDevice(error);
        }
        public event TerminalSendDataEventHandler OnSendData;
        public void DoInvoke(object sender, string data)
        {
            OnSendData?.Invoke(sender, data);
        }
        private async void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                SetStatus(nowstr + ": " + status.AsStatusString);
                SetStatusActive(false);
            });
        }


        public async void SetDeviceStatus(string text)
        {
            ;
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => {
                    ParentStatusHandler?.SetStatusText(text);
                });
        }

        PageDisplayPreferences CurrPageDisplayPreferences = new PageDisplayPreferences()
        {
            ParentShouldScroll = false, // this page will happily resize itself correctly to the parent size!
        };
        public PageDisplayPreferences GetPageDisplayPreferences()
        {
            return CurrPageDisplayPreferences;
        }
    }
}
