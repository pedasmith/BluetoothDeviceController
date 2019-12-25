using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
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

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SensythingPage : Page
    {
        public SensythingPage()
        {
            this.InitializeComponent();
        }

        Sensything Sensything = new Sensything();

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformation;
            uiProgress.IsActive = true;
            var ble = await BluetoothLEDevice.FromIdAsync(di.Id);
            Sensything.ble = ble;
            Sensything.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
            Sensything.PropertyChanged += Sensything_PropertyChanged;

            await Sensything.StartDataNotifications();
            uiProgress.IsActive = false;
        }

        private async void Sensything_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                switch (e.PropertyName)
                {
                    case "A1f": uiA1.Text = Sensything.A1f.ToString("f3"); break;
                    case "A2f": uiA2.Text = Sensything.A2f.ToString("f3"); break;
                    case "A3f": uiA3.Text = Sensything.A3f.ToString("f3"); break;
                    case "A4f": uiA4.Text = Sensything.A4f.ToString("f3"); break;
                }
            });
        }

        private async void Status_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                uiStatus.Text = nowstr + ": " + status.AsStatusString;
                uiProgress.IsActive = false;
            });
        }
    }
}
