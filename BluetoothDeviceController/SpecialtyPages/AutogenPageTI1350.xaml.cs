using BluetoothProtocols;
using System;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AutogenTI1350Page : Page
    {
        public AutogenTI1350Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        int ncommand = 0;
        TI_CC1350SensorTag bleDevice = new TI_CC1350SensorTag();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            uiProgress.IsActive = true;
            var di = args.Parameter as DeviceInformation;
            var ble = await BluetoothLEDevice.FromIdAsync(di.Id);
            uiProgress.IsActive = false;

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
        }

        private void SetStatus (string status)
        {
            uiStatus.Text = status;
        }

        private async void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                SetStatus(nowstr + ": " + status.AsStatusString);
                uiProgress.IsActive = false;
            });
        }



        private async void OnReadOpticalServiceConfig(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = true;
            ncommand++;
            try
            {
                var result = await bleDevice.ReadOpticalServiceConfig();
                var Enable = result.GetValue("Enable").AsDouble;
                OpticalServiceConfig_Enable.Text = Enable.ToString("N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }
        private async void OnWriteOpticalServiceConfig(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = true;
            ncommand++;
            try
            {
                byte Enable = Byte.Parse(OpticalServiceConfig_Enable.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null);

                await bleDevice.OpticalServiceConfig(Enable);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnReadOpticalServicePeriod(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = true;
            ncommand++;
            try
            {
                var result = await bleDevice.ReadOpticalServicePeriod();
                if (result != null)
                {
                    var Period = result.GetValue("Period").AsDouble;
                    OpticalServicePeriod_Period.Text = Period.ToString("N0");
                }
                else
                {
                    OpticalServicePeriod_Period.Text = "?";
                }
            }
            catch (Exception ex)
            {
                OpticalServicePeriod_Period.Text = "?";
                SetStatus($"Error: exception: {ex.Message}");
            }
        }
        private async void OnWriteOpticalServicePeriod(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = true;
            ncommand++;
            try
            {
                byte Period = Byte.Parse(OpticalServicePeriod_Period.Text, System.Globalization.NumberStyles.Integer, null);

                await bleDevice.OpticalServicePeriod(Period);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        private async void OnReadOpticalServiceData(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = true;
            ncommand++;
            try
            {
                var result = await bleDevice.ReadOpticalServiceData();
                var Lux = result.GetValue("Lux").AsDouble;
                OpticalServiceData_Lux.Text = Lux.ToString("F3");
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnNotifyOpticalServiceData(object sender, RoutedEventArgs e)
        {
            uiProgress.IsActive = true;
            ncommand++;
            try
            {
                bleDevice.OpticalServiceDataEvent += BleDevice_OpticalServiceDataEvent;
                var notifyType = Windows.Devices.Bluetooth.GenericAttributeProfile.GattClientCharacteristicConfigurationDescriptorValue.Notify;
                var result = await bleDevice.NotifyOpticalServiceDataAsync(notifyType);
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_OpticalServiceDataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var Lux = data.ValueList.GetValue("Lux").AsDouble;
                    OpticalServiceData_Lux.Text = Lux.ToString("F3");
                });
            }
        }
    }
}
