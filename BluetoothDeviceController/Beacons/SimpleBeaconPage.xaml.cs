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
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
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
            this.Loaded += SimpleBeaconPage_Loaded;
        }

        private void SimpleBeaconPage_Loaded(object sender, RoutedEventArgs e)
        {
            var trackAll = false;
            if (di.BeaconPreferences != null) trackAll = di.BeaconPreferences.DefaultTrackAll;
            uiTrackAll.IsChecked = trackAll;
        }

        DeviceInformationWrapper di = null;

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            di = args.Parameter as DeviceInformationWrapper;
            // When will this be null? When we've created a page generically instead of for a particular device.
            // This happens when you do a beacon search.
            if (di.BleAdvert != null) di.BleAdvert.UpdatedBleAdvertisement += BleAdvert_UpdatedBleAdvertisement;
            BleAdvertisementWrapper.UpdatedUniversalBleAdvertisement += BleAdvert_UpdatedUniversalBleAdvertisement;

            if (di.BleAdvert == null)
            {
                // Don't let the user switch off the track all.
                uiTrackAll.Visibility = Visibility.Collapsed;
                uiTrackAll.IsChecked = true; // belt and suspenders
            }
        }

        private bool TrackAll
        {
            get
            {
                var retval = uiTrackAll.IsChecked.Value;
                //if (di.BeaconPreferences != null) retval = di.BeaconPreferences.DefaultTrackAll;
                return retval;
            }
        }

        HashSet<ulong> BleAddressesSeen = new HashSet<ulong>();
        private async void BleAdvert_UpdatedUniversalBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            // Only do this if the user wants to see all data...
            var isPause = uiPause.IsChecked.Value;
            if (TrackAll && !isPause)
            {
                await UpdateUI(data, TrackAll);
            }
        }

        private async void BleAdvert_UpdatedBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            var isPause = uiPause.IsChecked.Value;
            if (!isPause)
            {
                await UpdateUI(data, TrackAll);
            }
        }

        private async Task UpdateUI(BluetoothLEAdvertisementReceivedEventArgs bleAdvert, bool includeLeadingAddress = true)
        {
            bool isFullDetails = uiFullDetails.IsChecked.Value;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                bool isNewAddress = BleAddressesSeen.Add(bleAdvert.BluetoothAddress); // add returns false if already present
                var address = BluetoothAddress.AsString(bleAdvert.BluetoothAddress);

                var eventTime = DateTime.Now;
                var time24 = eventTime.ToString("HH:mm:ss.f");
                var indent = "    ";

                sbyte transmitPower = 0;
                bool haveTransmitPower = false;
                bool isApple10 = false; 
                string appearance = "";
                string completeLocalName = "";

                var builder = new StringBuilder();
                Run bold = null;
                if (isNewAddress)
                {
                    var (name, id, description) = BleAdvertisementFormat.GetBleName(null, bleAdvert);
                    //var str = $"New device: addr={addrstring}";
                    //if (name != addrstring) str += ($" name={name}");
                    //if (id != name) str += ($" id={id}");
                    //str += ($" d={description}\n");
                    var str = (isNewAddress ? "New device: " : "") + description + "\n";
                    bold = new Run() { Text = str, FontWeight = FontWeights.Bold };
                }
                foreach (var section in bleAdvert.Advertisement.DataSections)
                {
                    var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                    switch (dtv)
                    {
                        case AdvertisementDataSectionParser.DataTypeValue.Appearance:
                            appearance = AdvertisementDataSectionParser.ParseAppearance(section);
                            break;
                        case AdvertisementDataSectionParser.DataTypeValue.CompleteLocalName:
                            {
                                var dr = DataReader.FromBuffer(section.Data);
                                completeLocalName = dr.ReadString(dr.UnconsumedBufferLength);
                            }
                            break;
                        case AdvertisementDataSectionParser.DataTypeValue.TxPowerLevel:
                            transmitPower = AdvertisementDataSectionParser.ParseTxPowerLevel(section);
                            haveTransmitPower = true;
                            break;
                        case AdvertisementDataSectionParser.DataTypeValue.IncompleteListOf16BitServiceUuids:
                        case AdvertisementDataSectionParser.DataTypeValue.CompleteListOf16BitServiceUuids:
                        case AdvertisementDataSectionParser.DataTypeValue.Flags:
                            if (isFullDetails) builder.Append(AdvertisementDataSectionParser.Parse(section, transmitPower, indent));
                            break;
                        default:
                            {
                                (var result, var manufacturerType) = AdvertisementDataSectionParser.Parse(section, transmitPower, indent);
                                isApple10 = manufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.Apple10;
                                builder.Append(result);
                                if (result.Contains ("Cypress"))
                                {
                                    ;
                                }
                            }
                            break;
                    }
                }
                // We don't know all of the header information until later
                var header = $"{address}\t{time24}\t{bleAdvert.RawSignalStrengthInDBm}";
                if (haveTransmitPower) header += $"\t{transmitPower.ToString()}";
                header += "\t" + appearance;
                if (completeLocalName != "") header += "\t" + completeLocalName;
                header += "\n";
                var run = new Run() { Text = header + builder.ToString() };

                var suppress = isApple10 && uiIgnoreApple.IsChecked.Value;
                if (!suppress)
                {
                    if (bold != null) uiBeaconData.Inlines.Add(bold);
                    uiBeaconData.Inlines.Add(run);
                }
            });

        }
    }
}
