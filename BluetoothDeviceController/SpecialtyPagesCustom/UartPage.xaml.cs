using BluetoothDeviceController.BluetoothProtocolsCustom;
using BluetoothDeviceController.SerialPort;
using BluetoothDeviceController.SpecialtyPages;
using BluetoothProtocols;
using System;
using Windows.Devices.Bluetooth;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPagesCustom
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UartPage : Page, ITerminal
    {
        nRFUartTerminalAdapter TerminalAdapter;
        Nordic_Uart bleDevice = null; 
        
        public UartPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformationWrapper;
            if (di.SerialPortPreferences == null)
            {
                // Make sure we set up good defaults.
                const string DefaultShortcutId = "Nordic-Uart";
                di.SerialPortPreferences = new UserSerialPortPreferences()
                {
                    LineEnd = UserSerialPortPreferences.TerminalLineEnd.CR,
                    SavePrefix = "Nordic_Uart_",
                    ShortcutId = DefaultShortcutId, // must match the Id value in CraftyRobot_Smartibot_Commands.json
                };
                di.SerialPortPreferences.ReadFromLocalSettings();
                if (di.SerialPortPreferences.ShortcutId == "(name)")
                {
                    di.SerialPortPreferences.ShortcutId = DefaultShortcutId;
                }
            }
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive(false);

            bleDevice = new Nordic_Uart(ble);
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;

            // Set up the terminal adapter, connecting the terminal control and the bluetooth device.
            TerminalAdapter = new nRFUartTerminalAdapter(uiTerminalControl, bleDevice);
            uiTerminalControl.ParentTerminal = this;
            uiTerminalControl.UserCanSetSerialLineEndings = false;
            // the adapter tells the terminal control to display the status.
            // The terminal control would rather we display the status.

            await TerminalAdapter.InitAsync();
            uiTerminalControl.DI = di;

            // Get the buttons to show!
            uiTerminalControl.UpdateShortcutButtons();
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
        public async void SetDeviceStatusEx(TerminalSupport.ConnectionState status, TerminalSupport.ConnectionSubstate substate, string text, double value)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => {
                    ParentStatusHandler?.SetStatusText(text);
                });
        }
    }
}
