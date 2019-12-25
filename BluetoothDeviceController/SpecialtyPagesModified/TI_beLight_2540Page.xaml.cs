using BluetoothDeviceController;
using BluetoothProtocols;
using System;
using System.Globalization;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// TI CC2540 beLight (beLight 0.2) high-intensity Bluetooth-enabled LED light kit.
    /// </summary>
    public sealed partial class TI_beLight_2540Page : Page, HasId
    {

        public TI_beLight_2540Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        int ncommand = 0;
        TI_beLight_2540 bleDevice = new TI_beLight_2540();

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            uiProgress.IsActive = true;
            var di = args.Parameter as DeviceInformation;
            var ble = await BluetoothLEDevice.FromIdAsync(di.Id);
            uiProgress.IsActive = false;

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += beLightTI_OnBluetoothStatus;
        }

        public string GetId()
        {
            return bleDevice?.ble?.DeviceId ?? "";
        }
        private string DeviceName = "TI_beLight_2540";
        private string DeviceNameUser = "TI beLight 2540";
        public string GetPicturePath()
        {
            return $"/Assets/DevicePictures/{DeviceName}-175.PNG";
        }
        public string GetDeviceNameUser()
        {
            return $"{DeviceNameUser}";
        }
        private async void beLightTI_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                uiStatus.Text = nowstr + ": " + status.AsStatusString;
                uiProgress.IsActive = false;
            });
        }

        // TODO: handle White color, too!

        private async void OnSetColor(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var color = uiColor.Color;
            await bleDevice.WriteSetColor(color.R, color.G, color.B, 0);
        }

        private async void OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var color = args.NewColor;
            await bleDevice.WriteSetColor(color.R, color.G, color.B, 0);
        }
    }
}
