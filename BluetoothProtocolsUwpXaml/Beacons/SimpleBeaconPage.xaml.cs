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
using Utilities;
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
using static BluetoothDeviceController.UserPreferences;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.Beacons
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SimpleBeaconPage : Page
    {


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
            //TODO: if (DeviceWrapper.BeaconPreferences != null) trackAll = DeviceWrapper.BeaconPreferences.DefaultTrackAll;
            //uiTrackAll.IsChecked = trackAll;
        }

        DeviceInformationWrapper DeviceWrapper = null;

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            DeviceWrapper = args.Parameter as DeviceInformationWrapper;
            // When will DeviceWrapper.BleAdvert be null? When we've created a page generically instead of for a particular device.
            // This happens when you do a beacon search.
            // DeviceWrapper will be null when we re-navigate to a SimpleBeaconPage after displaying a specific page.
            // When we try to re-display the SimpleBeaconPage, we only have a devicewrapper that's intended for
            // a specific page and it won't show the right page.
            if (DeviceWrapper?.BleAdvert != null)
            {
                DeviceWrapper.BleAdvert.UpdatedBleAdvertisement += BleAdvert_UpdatedBleAdvertisement;
                DeviceWrapper.BleAdvert.UpdatedBleAdvertisementWrapper += BleAdvert_UpdatedBleAdvertisementWrapper;
            }
            BleAdvertisementWrapper.UpdatedUniversalBleAdvertisement += BleAdvert_UpdatedUniversalBleAdvertisement;
            BleAdvertisementWrapper.UpdatedUniversalBleAdvertisementWrapper += BleAdvert_UpdatedUniversalBleAdvertisementWrapper;

            if (DeviceWrapper?.BleAdvert == null)
            {
                // Don't let the user switch off the track all.
                //uiTrackAll.Visibility = Visibility.Collapsed;
                //uiTrackAll.IsChecked = true; // belt and suspenders
            }
        }

        private void BleAdvert_UpdatedBleAdvertisementWrapper(BleAdvertisementWrapper data)
        {
            //Gets called sometimes? Why? 2022-12-30
            //throw new NotImplementedException(); //TODO: should not throw! Is this ever called?
            // Got called with a RuuviTag
        }


        /// <summary>
        /// Set by the main page via the Search settings; when set will pause updates to the advert page
        /// </summary>
        public bool IsPaused { get; set; } = false;

        HashSet<ulong> BleAddressesSeen = new HashSet<ulong>();
        private async void BleAdvert_UpdatedUniversalBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            var wrapper = new BleAdvertisementWrapper(data);
            await UpdateUI(wrapper, BluetoothCompanyIdentifier.CommonManufacturerType.Other, IsPaused);
        }
        private async void BleAdvert_UpdatedUniversalBleAdvertisementWrapper(BleAdvertisementWrapper data)
        {
            BluetoothCompanyIdentifier.CommonManufacturerType parseAs = BluetoothCompanyIdentifier.CommonManufacturerType.Other;
            if (Govee.AdvertIsGovee(data) != Govee.SensorType.NotGovee)
            {
                parseAs = BluetoothCompanyIdentifier.CommonManufacturerType.Govee;
            }
            else if (SwitchBot.AdvertIsSwitchBot(data))
            {
                parseAs = BluetoothCompanyIdentifier.CommonManufacturerType.SwitchBot;
            }
            await UpdateUI(data, parseAs, IsPaused);
        }
        private async void BleAdvert_UpdatedBleAdvertisement(Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementReceivedEventArgs data)
        {
            var wrapper = new BleAdvertisementWrapper(data);
            await UpdateUI(wrapper, BluetoothCompanyIdentifier.CommonManufacturerType.Other, IsPaused);
        }
        private string AdvertisementToString(BluetoothLEAdvertisementReceivedEventArgs bleAdvert, BluetoothCompanyIdentifier.CommonManufacturerType parseAs, DateTime eventTime)
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

            // Check for extended adverts
            var advertExtended = "";
            switch (bleAdvert.AdvertisementType)
            {
                case BluetoothLEAdvertisementType.ScanResponse:
                    advertExtended += "ScanResponse";
                    break;
                case BluetoothLEAdvertisementType.Extended:
                    advertExtended += "Extended";
                    break;
                default:
                    if (bleAdvert.IsScannable)
                    {
                        advertExtended += "IsScannable";
                    }
                    break;
            }
            if (bleAdvert.AdvertisementType == BluetoothLEAdvertisementType.Extended)
            {
                // Docs say to look at properties: https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.advertisement.bluetoothleadvertisementreceivedeventargs?view=winrt-22621
                ; // Handy spot for a debugger. Maybe a Govee extended advert?

            }


            var builder = new StringBuilder();
            foreach (var section in bleAdvert.Advertisement.DataSections)
            {
                var manufacturerType = BluetoothCompanyIdentifier.CommonManufacturerType.Other; // only set for some parses.
                var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                string result;
                switch (dtv)
                {
                    case AdvertisementDataSectionParser.DataTypeValue.Appearance: // 0x19==25
                        appearance = AdvertisementDataSectionParser.ParseAppearance(section);
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteLocalName:
                        var (name, rs) = DataReaderReadStringRobust.ReadStringEntire(DataReader.FromBuffer(section.Data));
                        completeLocalName = name;
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.TxPowerLevel: //0x0a==10
                        transmitPower = AdvertisementDataSectionParser.ParseTxPowerLevel(section);
                        haveTransmitPower = true;
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.IncompleteListOf16BitServiceUuids:
                    case AdvertisementDataSectionParser.DataTypeValue.CompleteListOf16BitServiceUuids:
                    case AdvertisementDataSectionParser.DataTypeValue.Flags:
                    default:
                        if (UserPreferences.MainUserPreferences.BeaconFullDetails)
                        {
                            (result, manufacturerType, companyId) = AdvertisementDataSectionParser.Parse(section, transmitPower, parseAs, indent);
                            builder.Append(result);
                        }
                        break;
                    case AdvertisementDataSectionParser.DataTypeValue.ManufacturerData://0xFF==255==-1
                        {
                            //string result;
                            //(result, manufacturerType, companyId) = AdvertisementDataSectionParser.ParseScanResponseServiceData(section, transmitPower, indent);
                            object speciality;
                            (result, manufacturerType, companyId, speciality) = BluetoothCompanyIdentifier.ParseManufacturerData(section, transmitPower, parseAs);

                            builder.Append(result);

                        }
                        break;
                }
                if (manufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.Apple10)
                {
                    isApple10 = true;
                }
            }


            // Note: Never need to pull data from ManufacturerData; it's already here
            // via the DataSections item.

            // We don't know all of the header information until later
            if (!completeLocalName.Contains("LC")) // just for debugging
            {
                // TODO: just for some debugging: return;
            }
            if (isApple10)
            {
                ; // Handy place to hang a debugger.
            }
            var header = $"{address}\t{time24}\t{bleAdvert.RawSignalStrengthInDBm}";
            if (haveTransmitPower) header += $"\t{transmitPower.ToString()}";
            if (advertExtended != "") header += $"\t{advertExtended}";
            header += "\t" + appearance;
            if (completeLocalName != "") header += "\t" + completeLocalName;
            var txt = $"{header}\n{builder}\n";
            return txt;
        }

        private bool IsApple10(BluetoothLEAdvertisementReceivedEventArgs bleAdvert)
        {

            sbyte transmitPower = 0;
            bool retval = false;

            foreach (var section in bleAdvert.Advertisement.DataSections)
            {
                var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                switch (dtv)
                {
                    case AdvertisementDataSectionParser.DataTypeValue.ManufacturerData:
                        {
                            var manufacturerType = BluetoothCompanyIdentifier.ParseManufacturerDataType(section, transmitPower);
                            retval = manufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.Apple10;
                        }
                        break;
                }
            }

            return retval;
        }
        private bool IsMicrosoftRome(BluetoothLEAdvertisementReceivedEventArgs bleAdvert)
        {

            sbyte transmitPower = 0;
            bool retval = false;

            var builder = new StringBuilder();
            foreach (var section in bleAdvert.Advertisement.DataSections)
            {
                var dtv = AdvertisementDataSectionParser.ConvertDataTypeValue(section.DataType);
                switch (dtv)
                {
                    case AdvertisementDataSectionParser.DataTypeValue.ManufacturerData:
                        {
                            var manufacturerType = BluetoothCompanyIdentifier.ParseManufacturerDataType(section, transmitPower);
                            retval = manufacturerType == BluetoothCompanyIdentifier.CommonManufacturerType.MicrosoftRome;
                        }
                        break;
                }
            }

            return retval;
        }

        List<SimpleBeaconHistory> AdvertisementHistory = new List<SimpleBeaconHistory>();


        private async Task UpdateUI(BleAdvertisementWrapper bleAdvertWrapper, BluetoothCompanyIdentifier.CommonManufacturerType parseAs, bool isPaused)
        {
            //bool includeLeadingAddress = true; // or UserPreferences.MainUserPreferences.BeaconTrackAll
            BluetoothLEAdvertisementReceivedEventArgs bleAdvert = bleAdvertWrapper.BleAdvert;

            var advertTime = DateTime.Now;
            var txt = AdvertisementToString(bleAdvert, parseAs, advertTime);
            string boldText = null;
            bool isNewAddress = BleAddressesSeen.Add(bleAdvert.BluetoothAddress); // add returns false if already present
            if (isNewAddress)
            {
                var (name, id, description) = BleAdvertisementFormat.GetBleName(bleAdvertWrapper);
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
            bool isTooFar = item.Args.RawSignalStrengthInDBm < UserPreferences.MainUserPreferences.BeaconDbLevel;
            if (isTooFar)
            {
                return true;
            }
            // If the bleAddressFilter is set to non-zero, then it's the only filter we want.
            if (bleAddressFilter != 0)
            {
                if (item.Address != bleAddressFilter)
                {
                    return true;
                }
                return false;
            }
            bool isApple10 = IsApple10(item.Args);
            bool isMicrosoftRome = IsMicrosoftRome(item.Args);
            var suppress = (isApple10 && UserPreferences.MainUserPreferences.BeaconIgnoreApple)
                || (isMicrosoftRome && UserPreferences.MainUserPreferences.BeaconIgnoreMicrosoftRome);

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
                var itemA = CurrSortDirection == SortDirection.Ascending ? item1 : item2;
                var itemB = CurrSortDirection == SortDirection.Ascending ? item2 : item1;
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

        public void ClearDisplay()
        {
            AdvertisementHistory.Clear();
            RedrawDisplay();
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
