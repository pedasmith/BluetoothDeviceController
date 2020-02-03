using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SerialPort
{
    public interface IDoTerminalSendData
    {
        void DoInvoke(object obj, string cmd);
    }

    public sealed partial class SimpleTerminalPage : Page, ITerminal, IDoTerminalSendData, SpecialtyPages.ISetHandleStatus
    {
        DeviceInformationWrapper DI = null;
        UserSerialPortPreferences SerialPortPreferences { get { return DI.SerialPortPreferences; } }

        BluetoothCommTerminalAdapter TerminalAdapter;

        public event TerminalSendDataEventHandler OnSendData;
        public void DoInvoke(object sender, string data)
        {
            OnSendData?.Invoke(sender, data);
        }

        public SimpleTerminalPage()
        {
            this.InitializeComponent();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            DI = args.Parameter as DeviceInformationWrapper;
            ParentStatusHandler?.SetStatusActive(true);
            uiTerminalControl.ParentTerminal = this;
            uiTerminalControl.DoSendData = this;
            uiTerminalControl.DI = DI;
            uiTerminalControl.UpdateShortcutButtons();

            TerminalAdapter = new BluetoothCommTerminalAdapter(this, DI);
            await TerminalAdapter.InitAsync();

            ParentStatusHandler?.SetStatusActive(false);
        }


        private SpecialtyPages.IHandleStatus ParentStatusHandler = null;
        public void SetHandleStatus(SpecialtyPages.IHandleStatus handleStatus)
        {
            ParentStatusHandler = handleStatus;
        }



        public async void SetDeviceStatus(string text)
        {
            ;
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => {
                    ParentStatusHandler?.SetStatusText(text);
                });
        }


        private void OnSettingsClicked(object sender, RoutedEventArgs e)
        {
            uiTerminalControl.OnSettingsClicked(sender, e);
        }

        public void ReceivedData(string data)
        {
            uiTerminalControl.ReceivedData(data);
        }

        public void ErrorFromDevice(string error)
        {
            uiTerminalControl.ErrorFromDevice(error);
        }
    }
}
