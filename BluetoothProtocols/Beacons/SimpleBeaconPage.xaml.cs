using BluetoothDeviceController.BleEditor;
using BluetoothDeviceController.BluetoothDefinitionLanguage;
using BluetoothProtocols.Beacons;
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
        public enum SortBy { Mac, Time, Rss, };
        public enum SortDirection { Ascending, Descending };

        public static SimpleBeaconPage TheSimpleBeaconPage = null;

        public SimpleBeaconPage()
        {
            TheSimpleBeaconPage = this;
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
                //TODO: how doe this actually work?
                var retval = UserPreferences.MainUserPreferences.BeaconTrackAll;
                return retval;
            }
        }
        private bool IgnoreApplePreference { get { return UserPreferences.MainUserPreferences.BeaconIgnoreApple; } }
        private bool FullDetailsPreferrence { get { return UserPreferences.MainUserPreferences.BeaconFullDetails; } }
        private double CloseSignalStrengthPreference { get { return UserPreferences.MainUserPreferences.BeaconDbLevel; } }

        public bool IsPaused { get; set; } = false;

        HashSet<ulong> BleAddressesSeen = new HashSet<ulong>();
        private async void BleAdvert_UpdatedUniversalBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            await UpdateUI(data, TrackAll, IsPaused);
        }

        private async void BleAdvert_UpdatedBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            await UpdateUI(data, TrackAll, IsPaused);
        }
        private string AdvertisementToString(BluetoothLEAdvertisementReceivedEventArgs bleAdvert, DateTime eventTime, bool includeLeadingAddress)
        {
            var address = BluetoothAddress.AsString(bleAdvert.BluetoothAddress);

            var time24 = eventTime.ToString("HH:mm:ss.f");
            var indent = "    ";

            sbyte transmitPower = 0;
            bool haveTransmitPower = false;
            bool isApple10 = false;
            string appearance = "";
            string completeLocalName = "";
            UInt16 companyId = 0xFFFF;

            var builder = new StringBuilder();
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
                        if (FullDetailsPreferrence)
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
                            if (bleAdvert.Advertisement.LocalName.Contains("Govee") || completeLocalName.Contains("Govee"))
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

            // We don't know all of the header information until later
            //TODO: microsoft manufacturer data includes swiftpair!
            if (!completeLocalName.Contains("Govee") && !completeLocalName.Contains("LC")) // just for debugging
            {
                // TODO: just for some debugging: return;
            }
            var header = $"{address}\t{time24}\t{bleAdvert.RawSignalStrengthInDBm}";
            if (haveTransmitPower) header += $"\t{transmitPower.ToString()}";
            header += "\t" + appearance;
            if (completeLocalName != "") header += "\t" + completeLocalName;
            var txt = $"{header}\n{builder}\n";
            return txt;
        }

        private bool IsApple10(BluetoothLEAdvertisementReceivedEventArgs bleAdvert)
        {

            sbyte transmitPower = 0;
            bool isApple10 = false;

            var builder = new StringBuilder();
            foreach (var section in bleAdvert.Advertisement.DataSections)
            {
                var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                switch (dtv)
                {
                    case AdvertisementDataSectionParser.DataTypeValue.ManufacturerData:
                        {
                            var manufacturerType = BluetoothCompanyIdentifier.ParseManufacturerDataType(section, transmitPower);
                            isApple10 = manufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.Apple10;
                        }
                        break;
                }
            }

            return isApple10;
        }

        List<SimpleBeaconHistory> AdvertisementHistory = new List<SimpleBeaconHistory>();


        private async Task UpdateUI(BluetoothLEAdvertisementReceivedEventArgs bleAdvert, bool includeLeadingAddress, bool isPaused)
        {
            var advertTime = DateTime.Now;
            var txt = AdvertisementToString(bleAdvert, advertTime, includeLeadingAddress);
            string boldText = null;
            bool isNewAddress = BleAddressesSeen.Add(bleAdvert.BluetoothAddress); // add returns false if already present
            if (isNewAddress)
            {
                var (name, id, description) = BleAdvertisementFormat.GetBleName(null, bleAdvert);
                //var str = $"New device: addr={addrstring}";
                //if (name != addrstring) str += ($" name={name}");
                //if (id != name) str += ($" id={id}");
                //str += ($" d={description}\n");
                boldText = (isNewAddress ? "New device: " : "") + description + "\n";
            }

            var h = SimpleBeaconHistory.MakeFromAdvertisement(bleAdvert, advertTime, boldText, txt);
            AdvertisementHistory.Add(h);

            var shouldIgnore = isPaused || ShouldIgnore(h, CurrBleAddressFilter);
            if (shouldIgnore) return;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                var lines = h.MakeRun();
                foreach (var run in lines)
                {
                    uiBeaconData.Inlines.Add(run);
                }
            });
        }

        bool ShouldIgnore(SimpleBeaconHistory item, ulong bleAddressFilter)
        {
            bool isTooFar = item.Args.RawSignalStrengthInDBm < CloseSignalStrengthPreference;
            if (isTooFar)
            {
                return true;
            }
            if (bleAddressFilter != 0 && item.Address != bleAddressFilter)
            {
                return true;
            }
            bool isApple10 = IsApple10(item.Args);
            var suppress = isApple10 && IgnoreApplePreference;

            return suppress;
        }

        public int CurrMillisecondsDelay { get; set; } = 100;
        public SortBy CurrSortBy { get; set; } = SortBy.Time;
        public SortDirection CurrSortDirection { get; set; } = SortDirection.Ascending;
        public ulong CurrBleAddressFilter { get; set; } = 0;


        public async Task SortAsync ()
        {
            uiBeaconData.Inlines.Clear();
            await Task.Delay(CurrMillisecondsDelay); // 100 is good

            AdvertisementHistory.Sort((item1, item2) =>
            {
                var itemA = CurrSortDirection == SortDirection.Ascending ? item2 : item1;
                var itemB = CurrSortDirection == SortDirection.Ascending ? item1 : item2;
                int retval = 0;
                switch (CurrSortBy)
                {
                    default:
                    case SortBy.Mac:
                        retval = itemA.Address.CompareTo(itemB.Address);
                        break;
                    case SortBy.Rss:
                        retval = itemA.Args.RawSignalStrengthInDBm.CompareTo(itemB.Args.RawSignalStrengthInDBm);
                        break;
                    case SortBy.Time:
                        retval = itemA.AdvertisementTime.CompareTo(itemB.AdvertisementTime);
                        break;
                }
                // e.g., sort by MAC address first and then by time. In the case where we sort by time first
                // and they are equal, we'll ust sort by time again -- slightly wasteful, but NBD.
                if (retval == 0)
                {
                    retval = itemA.AdvertisementTime.CompareTo(itemB.AdvertisementTime);
                }
                return retval;
            });
            RedrawDisplay(); // will double-clear the uiBeaconData.Inlines, but that's OK
        }

        public void RedrawDisplay()
        {
            uiBeaconData.Inlines.Clear();
            foreach (var h in AdvertisementHistory)
            {
                var shouldIgnore = ShouldIgnore(h, CurrBleAddressFilter);
                if (!shouldIgnore)
                {
                    var lines = h.MakeRun();
                    foreach (var run in lines)
                    {
                        uiBeaconData.Inlines.Add(run);
                    }
                }
            }
        }

    }
}
