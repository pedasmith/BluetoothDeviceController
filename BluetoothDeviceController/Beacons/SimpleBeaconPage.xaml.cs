using BluetoothDeviceController.BleEditor;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
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

namespace BluetoothDeviceController.Beacons
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimpleBeaconPage : Page
    {
        public SimpleBeaconPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformationWrapper;
            di.BleAdvert.UpdatedBleAdvertisement += BleAdvert_UpdatedBleAdvertisement;
            BleAdvertisementWrapper.UpdatedUniversalBleAdvertisement += BleAdvert_UpdatedUniversalBleAdvertisement;
        }

        HashSet<ulong> BleAddressesSeen = new HashSet<ulong>();
        private async void BleAdvert_UpdatedUniversalBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            // Only do this if the user wants to see all data...
            var trackAll = uiTrackAll.IsChecked;
            await UpdateUI(data);
        }

        private async void BleAdvert_UpdatedBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            await UpdateUI(data);
        }

        private async Task UpdateUI(BluetoothLEAdvertisementReceivedEventArgs bleAdvert)
        {
            bool isFullDetails = uiFullDetails.IsChecked.Value;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var address = bleAdvert.BluetoothAddress;
                bool isNewAddress = BleAddressesSeen.Add(address); // add returns false if already present

                var eventTime = DateTime.Now;
                var time24 = eventTime.ToString("HH:mm:ss.f");

                var builder = new StringBuilder();
                sbyte transmitPower = 0;
                if (isNewAddress)
                {
                    var (name, id, description) = BleAdvertisementFormat.GetBleName(null, bleAdvert);
                    builder.Append($"New device: {address}");
                    if (name != address.ToString()) builder.Append($" {name}");
                    if (id != name) builder.Append($" {id}");
                    builder.Append ($" {description}\n");
                }
                foreach (var section in bleAdvert.Advertisement.DataSections)
                {
                    var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                    switch (dtv)
                    {
                        case AdvertisementDataSectionParser.DataTypeValue.TxPowerLevel:
                            transmitPower = AdvertisementDataSectionParser.ParseTxPowerLevel(section);
                            break;
                        case AdvertisementDataSectionParser.DataTypeValue.Flags:
                            if (isFullDetails) builder.Append(AdvertisementDataSectionParser.Parse(section, transmitPower));
                            break;
                        default:
                            builder.Append(AdvertisementDataSectionParser.Parse(section, transmitPower));
                            break;
                    }
                }

                var header = $"{time24}\t{bleAdvert.RawSignalStrengthInDBm}\t{transmitPower.ToString()}\n";
                builder.Insert(0, header);
                uiBeaconData.Text += builder.ToString();
            });

        }
    }
}
