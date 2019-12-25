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
    /// WITTI Designs DOTTI display
    /// </summary>
    public sealed partial class Witti_DottiPage : Page, HasId
    {

        public Witti_DottiPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        int ncommand = 0;
        Witti_Dotti bleDevice = new Witti_Dotti();
        bool IsFullyInitialized = false;

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            uiProgress.IsActive = true;
            var di = args.Parameter as DeviceInformation;
            var ble = await BluetoothLEDevice.FromIdAsync(di.Id);
            uiProgress.IsActive = false;

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += device_OnBluetoothStatus;

            OnResetTime(null, null);
            IsFullyInitialized = true;
        }

        public string GetId()
        {
            return bleDevice?.ble?.DeviceId ?? "";
        }
        private string DeviceName = "Witti_Dotti";
        private string DeviceNameUser = "Witti Dotti";
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
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var color = args.NewColor;
            await bleDevice.SetColor(color.R, color.G, color.B);
        }

        private  void ResetToGray()
        {
            // Reset the LEDs to a gray
            var brush = new SolidColorBrush(Colors.LightGray);
            foreach (var item in uiLedGrid.Children)
            {
                var r = item as Rectangle;
                if (r == null) continue;
                r.Fill = brush;
            }
        }

        private async void OnLoadFromSelect(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var page  = (Witti_Dotti.DottiPage)(e.AddedItems[0]);
            await bleDevice.LoadScreenFromMemory(page);

            ResetToGray();
       }

        private async void OnCellTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            switch (CurrCursorMeaning)
            {
                case CursorMeaning.Normal:
                    var celltag = (sender as FrameworkElement)?.Tag as string;
                    byte ledIndex;
                    var ok = byte.TryParse(celltag, out ledIndex);
                    await bleDevice.SetLEDIndex(ledIndex, SelectedColor.R, SelectedColor.G, SelectedColor.B);

                    (sender as Rectangle).Fill = new SolidColorBrush(SelectedColor);
                    break;

                case CursorMeaning.Eyedropper:
                    SelectedColor = ((sender as Rectangle).Fill as SolidColorBrush).Color;
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                    CurrCursorMeaning = CursorMeaning.Normal;
                    break;
            }
        }

        private async void OnCellEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var currPoint = e.GetCurrentPoint(sender as UIElement);
            bool pressing = currPoint.IsInContact;
            if (!pressing) return;

            var celltag = (sender as FrameworkElement)?.Tag as string;
            byte ledIndex;
            var ok = byte.TryParse(celltag, out ledIndex);
            await bleDevice.SetLEDIndex(ledIndex, SelectedColor.R, SelectedColor.G, SelectedColor.B);

            (sender as Rectangle).Fill = new SolidColorBrush(SelectedColor);
        }

        Color SelectedColor = new Color() { A = 0xff, R = 0x00, G = 0xFF, B = 0x00 };
        private void OnSelectedColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            SelectedColor = args.NewColor;
        }

        private async void OnLoadNow(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var page = (Witti_Dotti.DottiPage)(uiLoadFrom.SelectedItem);
            await bleDevice.LoadScreenFromMemory(page);

            ResetToGray();
        }

        private async void OnSaveNow(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var page = (Witti_Dotti.DottiPage)(uiSaveTo.SelectedItem);
            await bleDevice.SaveScreenToMemory(page);
        }

        enum CursorMeaning { Normal, Eyedropper};
        CursorMeaning CurrCursorMeaning = CursorMeaning.Normal;
        private void OnEyedropper(object sender, RoutedEventArgs e)
        {
            //Windows.UI.Xaml.Window.Current.CoreWindow.SetPointerCapture();
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Cross, 1);
            CurrCursorMeaning = CursorMeaning.Eyedropper;
            // When I get a click, I'll revert.
        }

        private void OnEditGridExit(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        //    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        //    CurrCursorMeaning = CursorMeaning.Normal;
        }

        private async void OnModeChanged(object sender, SelectionChangedEventArgs e)
        {
            uiProgress.IsActive = ncommand == 0;
            ncommand++;

            var mode = (Witti_Dotti.DottiMode)(uiMode.SelectedItem);
            await bleDevice.SetMode(mode);
        }

        private async void OnSyncTime(object sender, RoutedEventArgs e)
        {
            byte hours=0, minutes=0, seconds=0;
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

        private async void OnAnimationSpeedChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!IsFullyInitialized) return;
            var value = (byte)e.NewValue;
            await bleDevice.ChangeAnimationSpeed(value);
        }
    }
}
