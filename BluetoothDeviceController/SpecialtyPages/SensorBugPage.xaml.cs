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
    public sealed partial class SensorBugPage : Page
    {
        public SensorBugPage()
        {
            this.InitializeComponent();
        }

        SensorBug SensorBug = new SensorBug();

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformation;
            uiProgress.IsActive = true;
            var ble = await BluetoothLEDevice.FromIdAsync(di.Id);
            SensorBug.ble = ble;
            SensorBug.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
            SensorBug.PropertyChanged += SensorBug_PropertyChanged;

            var configureAccleration = new SensorBug.ConfigureAcceleration();
            configureAccleration.Enable = 1;
            await SensorBug.ConfigureAccelerationNotifications(configureAccleration);

            var configureLight = new SensorBug.ConfigureLight();
            configureLight.Enable = 1;
            await SensorBug.ConfigureLightNotifications(configureLight);

            var configureTemperature = new SensorBug.ConfigureTemperature();
            configureTemperature.Enable = 1;
            await SensorBug.ConfigureTemperatureNotifications(configureTemperature);

            uiProgress.IsActive = false;
        }

        private async void SensorBug_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () =>
            {
                switch (e.PropertyName)
                {
                    case "A1f": uiA1.Text = SensorBug.A1f.ToString("f3"); break;
                    case "A2f": uiA2.Text = SensorBug.A2f.ToString("f3"); break;
                    case "A3f": uiA3.Text = SensorBug.A3f.ToString("f3"); break;
                    case "A4f": uiA4.Text = SensorBug.A4f.ToString("f3"); break;
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
