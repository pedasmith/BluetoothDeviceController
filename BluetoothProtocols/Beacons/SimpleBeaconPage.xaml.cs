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
            //var trackAll = MainPage.TheMainPage.Preferences.BeaconTrackAll;
            //TODO: if (di.BeaconPreferences != null) trackAll = di.BeaconPreferences.DefaultTrackAll;
            //uiTrackAll.IsChecked = trackAll;
        }

        DeviceInformationWrapper di = null;

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            di = args.Parameter as DeviceInformationWrapper;
            // When will di.BleAdvert be null? When we've created a page generically instead of for a particular device.
            // This happens when you do a beacon search.
            if (di.BleAdvert != null) di.BleAdvert.UpdatedBleAdvertisement += BleAdvert_UpdatedBleAdvertisement;
            BleAdvertisementWrapper.UpdatedUniversalBleAdvertisement += BleAdvert_UpdatedUniversalBleAdvertisement;

            if (di.BleAdvert == null)
            {
                // Don't let the user switch off the track all.
                //uiTrackAll.Visibility = Visibility.Collapsed;
                //uiTrackAll.IsChecked = true; // belt and suspenders
            }
        }

        private bool TrackAll
        {
            get
            {
                //TODO: remove var retval = uiTrackAll.IsChecked.Value;
                //if (di.BeaconPreferences != null) retval = di.BeaconPreferences.DefaultTrackAll;
                var retval = MainPage.TheMainPage.Preferences.BeaconTrackAll;
                return retval;
            }
        }
        private bool IgnoreApple {  get { return MainPage.TheMainPage.Preferences.BeaconIgnoreApple; } }
        private bool FullDetails {  get { return MainPage.TheMainPage.Preferences.BeaconFullDetails; } }
        private double CLOSE_SIGNAL_STRENGTH { get { return MainPage.TheMainPage.Preferences.BeaconDbLevel; } }

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
            // For debugging: include only close-by signals
            bool isClose = bleAdvert.RawSignalStrengthInDBm > CLOSE_SIGNAL_STRENGTH;
            if (!isClose)
            {
                return;
            }

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
                UInt16 companyId = 0xFFFF;

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
                            if (FullDetails)
                            {
                                string result;
                                BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType;
                                (result, manufacturerType, companyId) = AdvertisementDataSectionParser.Parse(section, transmitPower, indent);
                                builder.Append(result);
                            }
                            break;
                        case AdvertisementDataSectionParser.DataTypeValue.ManufacturerData:
                            {
                                string result;
                                BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType;
                                (result, manufacturerType, companyId) = AdvertisementDataSectionParser.Parse(section, transmitPower, indent);
                                isApple10 = manufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.Apple10;
                                builder.Append(result);

                            }
                            break;
                        default:
                            {
                                string result;
                                BluetoothCompanyIdentifier.CommonManufacturerType manufacturerType;
                                (result, manufacturerType, companyId) = AdvertisementDataSectionParser.Parse(section, transmitPower, indent);
                                isApple10 = manufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.Apple10;
                                builder.Append(result);
                                if (bleAdvert.Advertisement.LocalName.Contains ("Govee") || completeLocalName.Contains ("Govee"))
                                {
                                    ;
                                }
                            }
                            break;
                    }
                }

                // Pull data from the ManufacturerData
                // Never need to pull data from ManufacturerData; it's already here
                // via the DataSections item.
#if NEVER_EVER_DEFINED
                var mds = new StringBuilder();
                if (bleAdvert.Advertisement.ManufacturerData.Count == 0)
                {
                    mds.Append("    No manufacturer data\n");
                }
                else
                {
                    foreach (var md in bleAdvert.Advertisement.ManufacturerData)
                    {
                        var company = md.CompanyId;
                        var data = md.Data.ToArray();
                        mds.Append($"    ManufacturerData: {company:X}: ");
                        foreach (var b in data)
                        {
                            mds.Append($"{b:X2} ");
                        }
                        mds.Append("\n");
                    }
                }
                builder.Append(mds);
#endif
                // We don't know all of the header information until later
                //TODO: microsoft manufacturer data includes swiftpair!
                if (!completeLocalName.Contains ("Govee") && !completeLocalName.Contains ("LC")) // just for debugging
                {
                    // TODO: just for some debugging: return;
                }
                var header = $"{address}\t{time24}\t{bleAdvert.RawSignalStrengthInDBm}";
                if (haveTransmitPower) header += $"\t{transmitPower.ToString()}";
                header += "\t" + appearance;
                if (completeLocalName != "") header += "\t" + completeLocalName;
                var txt = $"{header}\n{builder}\n";
                var run = new Run() { Text = txt };

                var suppress = isApple10 && IgnoreApple;
                // Just for debugging: suppress = (companyId != 1177);
                if (!suppress)
                {
                    if (bold != null) uiBeaconData.Inlines.Add(bold);
                    uiBeaconData.Inlines.Add(run);
                }
            });

        }
    }
}
