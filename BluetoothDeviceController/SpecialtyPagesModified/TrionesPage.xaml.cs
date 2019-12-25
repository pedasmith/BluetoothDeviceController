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
    public sealed partial class TrionesPage : Page, HasId
    {
        public TrionesPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += TrionesPage_Loaded;
        }

        bool isLoaded = false;
        bool isNavigated = false;
        private void TrionesPage_Loaded(object sender, RoutedEventArgs e)
        {
            isLoaded = true;
        }

        int ncommand = 0;
        Triones_LedLight_Custom bleDevice = new Triones_LedLight_Custom();
        public Triones_LedLight_Custom.Modes DemoLightingMode = Triones_LedLight_Custom.Modes.None;

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            uiProgress.IsActive = true;
            var di = args.Parameter as DeviceInformation;
            var ble = await BluetoothLEDevice.FromIdAsync(di.Id);
            uiProgress.IsActive = false;

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += Triones_OnBluetoothStatus;
            isNavigated = true;

            bleDevice.OnLedStatusUpdate += Triones_OnLedStatusUpdate;
            uiProgress.IsActive = true;
            await bleDevice.StartStatusNotificationsAsync();
            await bleDevice.RequestStatus();
        }

        public string GetId()
        {
            return bleDevice?.ble?.DeviceId ?? "";
        }
        private string DeviceName = "Triones";
        private string DeviceNameUser = "Triones";
        public string GetPicturePath()
        {
            return $"/Assets/DevicePictures/{DeviceName}-175.PNG";
        }
        public string GetDeviceNameUser()
        {
            return $"{DeviceNameUser}";
        }
        int NWhiteValueChangeSupress = 0;
        /// <summary>
        /// When we get a status message from the bulb, we update the UI to match the bulb's values.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="status"></param>
        private async void Triones_OnLedStatusUpdate(object source, LedStatus status)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
           {
               // The R, G, B values are sometimes reset to all-zero
               if (status.R != 0 || status.G != 0 || status.B != 0)
               {
                   uiColor.Color = new Windows.UI.Color() { A = 255, R = status.R, G = status.G, B = status.B };
               }
               if (status.TrionesMode >= 0x25 && status.TrionesMode <= 0x38)
               {
                   uiDemoMode.SelectedIndex = status.TrionesMode - 0x25 + 1;
               }
               else
               {
                   uiDemoMode.SelectedIndex = 0;
               }
               if (status.Speed > 0) uiSpeed.Value = status.Speed;
               NWhiteValueChangeSupress++;
               uiWhite.Value = status.W;
               uiPower.IsChecked = status.IsOn;

               uiProgress.IsActive = true;

           });
        }

        private async void Triones_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                uiStatus.Text = nowstr + ": " + status.AsStatusString;
                uiProgress.IsActive = false;
            });
        }

        private async void OnSetColor(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var color = uiColor.Color;
            await bleDevice.SetColor(color.R, color.G, color.B);
        }

        private async void OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var color = args.NewColor;
            await bleDevice.SetColor(color.R, color.G, color.B);
        }

        /*
        private async void OnPower(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            byte onoff = Byte.Parse((sender as Button).Tag as string);
            await Triones.Power((onoff == 0 ? false : true));
        }
        */

        private async void OnPowerChecked(object sender, RoutedEventArgs e)
        {
            if (!isLoaded || !isNavigated) return;

            uiProgress.IsActive = ncommand == 0;
            ncommand++;
            bool onoff = (sender as CheckBox).IsChecked.Value;
            await bleDevice.Power(onoff);

        }

        byte currDemoMode = 0;
        private async void OnDemoSelect(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            currDemoMode = (byte)(int)(e.AddedItems[0]);
            await bleDevice.SetMode(currDemoMode, currDemoSpeed);
        }

        byte currDemoSpeed = 0x01;
        private async void OnDemoSpeedChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            currDemoSpeed = (byte)e.NewValue;
            if (currDemoMode == 0) return;
            if (!isLoaded || !isNavigated) return;

            await bleDevice.SetMode(currDemoMode, currDemoSpeed);
        }

        private async void OnWhiteValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!isLoaded || !isNavigated) return;
            if (NWhiteValueChangeSupress-- > 0) return;
            var value = (byte)e.NewValue;
            await bleDevice.SetWhite(value);
        }


    }
}
