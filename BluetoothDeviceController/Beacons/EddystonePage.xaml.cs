using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.Beacons
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EddystonePage : Page
    {
        public EddystonePage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformationWrapper;
            di.BleAdvert.UpdatedEddystoneAdvertisement += BleAdvert_UpdatedEddystoneAdvertisement;
            uiUrl.Text = di.BleAdvert.EddystoneUrl;
        }

        private async void BleAdvert_UpdatedEddystoneAdvertisement(string data)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var eventTime = DateTime.Now;
                var time24 = eventTime.ToString("HH:mm:ss.f");

                uiBeaconData.Text += $"{time24}\t{data}\n";
            });
        }
    }
}
