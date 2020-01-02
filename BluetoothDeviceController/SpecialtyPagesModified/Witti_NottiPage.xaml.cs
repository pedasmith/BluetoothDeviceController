using BluetoothDeviceController;
using BluetoothProtocols;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// WITTI Designs NOTTI display
    /// </summary>
    public sealed partial class Witti_NottiPage : Page, HasId
    {

        public Witti_NottiPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        int ncommand = 0;
        Witti_Notti bleDevice = new Witti_Notti();
        bool IsFullyInitialized = false;

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            uiProgress.IsActive = true;
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            uiProgress.IsActive = false;

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += device_OnBluetoothStatus;

            OnResetTime(null, null);
            IsFullyInitialized = true;
        }

        public string GetId() //TODO: add to all pages
        {
            return bleDevice.ble.DeviceId;
        }
        private string DeviceName = "Wiiti_Notti";
        private string DeviceNameUser = "Witti Notti";
        public string GetPicturePath()
        {
            return $"/Assets/DevicePictures/{DeviceName}-175.PNG";
        }
        public string GetDeviceNameUser()
        {
            return $"{DeviceNameUser}";
        }
        private async void device_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
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
            if (!IsFullyInitialized) return;

            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var color = args.NewColor;
            //await deviceProtocol.SetMode(Witti_Notti.NottiMode.On);
            await bleDevice.SetColor(color.R, color.G, color.B);
        }

        Color Color1;
        Color Color2;

        private async void OnColor1Changed(ColorPicker sender, ColorChangedEventArgs args)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            Color1 = args.NewColor;
            //await deviceProtocol.SetMode(Witti_Notti.NottiMode.Color_Changing);
            await bleDevice.SetColorChange(Color1.R, Color1.G, Color1.B, Color2.R, Color2.G, Color2.B);
        }

        private async void OnColor2Changed(ColorPicker sender, ColorChangedEventArgs args)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            Color2 = args.NewColor;
            //await deviceProtocol.SetMode(Witti_Notti.NottiMode.Color_Changing);
            await bleDevice.SetColorChange(Color1.R, Color1.G, Color1.B, Color2.R, Color2.G, Color2.B);
        }

        private async void OnModeChanged(object sender, SelectionChangedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var mode = (Witti_Notti.NottiMode)(uiMode.SelectedItem);
            await bleDevice.SetMode(mode);
        }

        private async void OnSyncTime(object sender, RoutedEventArgs e)
        {
            byte hours = 0, minutes = 0, seconds = 0;
            var ok = byte.TryParse(uiHours.Text, out hours);
            ok = ok && byte.TryParse(uiMinutes.Text, out minutes);
            ok = ok && byte.TryParse(uiSeconds.Text, out seconds);
            if (!ok) return;
            await bleDevice.SyncTime(hours, minutes, seconds);
        }

        private void OnResetTime(object sender, RoutedEventArgs e)
        {
            var now = DateTime.Now;
            uiHours.Text = now.Hour.ToString();
            uiMinutes.Text = now.Minute.ToString();
            uiSeconds.Text = now.Second.ToString();
        }

        private async void OnSetAlarmTime(object sender, RoutedEventArgs e)
        {
            byte hours = 0, minutes = 0;
            var ok = byte.TryParse(uiAlarmHours.Text, out hours);
            ok = ok && byte.TryParse(uiAlarmMinutes.Text, out minutes);
            if (!ok) return;
            await bleDevice.SetAlarmTime(hours, minutes);

        }


        private async void OnSetAlarmMode(object sender, RoutedEventArgs e)
        {
            var item = uiAlarm.SelectedItem;
            var alarmMode = (item == null) ? Witti_Notti.NottiAlarm.Once :  (Witti_Notti.NottiAlarm)item;
            var alarmWakeupTime = (byte)uiAlarmWakeupTime.Value;
            var color = uiAlarmColor.Color;

            await bleDevice.SetAlarmMode(alarmMode, color.R, color.G, color.B, alarmWakeupTime); // 1==2 minutes 30 seconds 2.5 minutes
        }
    }
}
