using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothProtocolsCustom;
using BluetoothDeviceController.Names;
using BluetoothDeviceController.UserData;
using Microsoft.Advertising.WinRT.UI;
using SearchControllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation.Metadata;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothDeviceController
{

    public interface IDoSearch
    {
        bool GetSearchActive();
        void StartSearch(UserPreferences.ReadSelection searchType, UserPreferences.SearchScope scope);
        void StartSearchWithUserPreferences();
        void CancelSearch();
        event EventHandler DeviceEnumerationChanged;
        //string GetCurrentSearchResults();
        void ListFilteredOut();
        void PauseCheckUpdated(bool newIsChecked);
        void ClearDisplay();
    }
    public enum FoundDeviceInfo { IsNew, IsDuplicate, IsOutOfRange, IsFilteredOutDb, IsFilteredOutNoName, IsError };

    public enum SearchFeedbackType {  Normal, Advertisement }
    public interface IDoSearchFeedback // The SearchFeedbackControl
    {
        void StartSearchFeedback();
        void StopSearchFeedback();
        void FoundDevice(FoundDeviceInfo foundInfo);

        bool GetIsPaused();
        void SetPauseVisibility(bool visible);
        void SetSearchFeedbackType(SearchFeedbackType feedbackType);
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IDoSearch, IDockParent, SpecialtyPages.IHandleStatus, IDeviceDisplay
    {
#if DEBUG
        const bool ALLOW_AD = false;
#else
        const bool ALLOW_AD = true;
#endif

        const string ALARM = "⏰";
        const string BEACON = "⛯"; // MAP SYMBOL FOR LIGHTHOUSE //"🖄";
        const string CAR = "🚗";
        const string DATA = "📈"; 
        const string DICE = "🎲"; 
        const string HEALTH = "🏥"; //"🥼";
        const string LIGHT = "💡";
        const string LIGHTNING = "🖄"; // envelope with lightning
        const string ROBOT = ""; // Part of the Segoe MDL2 Assets FontFamily
        const string TRAIN = "🚂";
        const string UART = "🖀";
        const string WAND = "🖉"; // yeah, a pencil isn't really a wand.
        const string WATER = "🚰";

        
        /// <summary>
        /// Converts a list of possible Bluetooth names into a page to display
        /// </summary>
        List<Specialization> Specializations = new List<Specialization>()
        {
            // Fun specializations
            new Specialization (typeof(SpecialtyPages.Kano_WandPage), new string[] { "Kano-Wand" }, WAND, "Kano Wand", "Kano coding kit Harry Potter Wand"),
            new Specialization (typeof(SpecialtyPagesCustom.UartPage), new string[] { "Puck" }, UART, "Espruino (puck)", "Puck.js: a UART-based Espruino device", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(SpecialtyPagesCustom.UartPage), new string[] { Nordic_Uart.SpecializationName }, UART, "BLE UART COM port", "Nordic UART comm port", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(SpecialtyPagesCustom.CraftyRobot_SmartibotPage), new string[] { "Smartibot" }, ROBOT, "Smartibot", "Smartibot espruino-based robot", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(SpecialtyPages.Elegoo_MiniCarPage), new string[] { "ELEGOO BT16" }, CAR, "Elegoo Mini-Car", "Elegoo small robot car"),
            new Specialization (typeof(SpecialtyPages.Lionel_LionChiefPage), new string[] { "LC" }, TRAIN, "LionChief Train", "Lionel Train Controller"),
            new Specialization (typeof(SpecialtyPages.Particula_GoDicePage), new string[] { "GoDice" }, DICE, "Particula GoDice", "GoDice 6-sided game dice"),
            new Specialization (typeof(SpecialtyPages.WilliamWeilerEngineering_SkoobotPage), new string[] { "Skoobot" }, ROBOT, "Skoobot", "Skoobot tiny robot"),

            // Lights
            new Specialization (typeof(SpecialtyPages.TI_beLight_2540Page), new string[] { "beLight" }, LIGHT, "TI Light", "TI CC2540 Bluetooth kit"),
            new Specialization (typeof(SpecialtyPages.Witti_DottiPage), new string[] { "Dotti" }, LIGHT, "DOTTI", "Witti Designs DOTTI device"),
            new Specialization (typeof(SpecialtyPages.Witti_NottiPage), new string[] { "Notti" }, ALARM + LIGHT, "NOTTI", "Witti Designs NOTTI device"),
            new Specialization (typeof(SpecialtyPages.MIPOW_Playbulb_BTL201Page), new string[] { "PLAYBULB" }, LIGHT, "MIPOW Smart LED", "MIPOW PLAYBULB Smart LED Bulb BTL-201"),
            new Specialization (typeof(SpecialtyPages.TrionesPage), new string[] { "Triones", "LEDBlue" }, LIGHT, "LED Light", "Bluetooth controlled light (triones protocol)"),

            // // // Is actually empty :-) new Specialization (typeof(SpecialtyPages.DigHoseEndPage), new string[] { "DIG", "\0IG" }, WATER, "DIG water valve", "DIG Hose-end valve"),

            // Data Sensors
            new Specialization (typeof(SpecialtyPages.Bbc_MicroBitPage), new string[] { "BBC micro:bit" }, DATA, "BBC micro:bit", "micro:bit with data program"),
            new Specialization (typeof(SpecialtyPages.PokitProMeterPage), new string[] { "PokitPro" }, DATA, "Pokit Pro multimeter", "Pokit Pro multimeter and digital oscilloscope"),

            // Health Sensors
            new Specialization (typeof(SpecialtyPages.Samico_BloodPressure_BG512Page), new string[] { "Samico BP" }, HEALTH, "Pyle / Samico", "Blood pressure cuff"),

            // All of the TI sensor tags :-)
            // The 2650 and 1350 SensorTag have the same interface and use the same page
            new Specialization (typeof(SpecialtyPages.TI_SensorTag_1352Page), new string[] { "Multi-Sensor" }, DATA, "TI 1352 SensorTag", "SensorTag data"),
            new Specialization (typeof(SpecialtyPages.TI_SensorTag_1350Page), new string[] { "CC1350" }, DATA, "TI 1350 SensorTag", "SensorTag data"),
            new Specialization (typeof(SpecialtyPages.TI_SensorTag_1350Page), new string[] { "CC2650 SensorTag" }, DATA, "TI 2650 SensorTag", "SensorTag data"),
            new Specialization (typeof(SpecialtyPages.TI_SensorTag_1350Page), new string[] { "SensorTag 2.0" }, DATA, "TI 2650 SensorTag", "SensorTag data"),
            new Specialization (typeof(SpecialtyPages.TI_SensorTag_2541Page), new string[] { "SensorTag", "TI BLE Sensor Tag" }, DATA, "TI 2541 SensorTag", "SensorTag data"),

            //Sensor Bug isn't done yet. 
            new Specialization (typeof(SpecialtyPages.BleHome_SensorBugPage), new string[] { "SensorBug" }, DATA, "SensorBug", "SensorBug data"),
            new Specialization (typeof(SpecialtyPages.Protocentral_SensythingPage), new string[] { "Sensything" }, DATA, "Sensything", "Sensything data"),
            new Specialization (typeof(SpecialtyPages.Nordic_ThingyPage), new string[] { "Thingy" }, DATA, "Nordic Thingy", "Nordic Semiconductor Thingy sensor platform"),
            new Specialization (typeof(SpecialtyPages.Vion_MeterPage), new string[] { "Vion Meter" }, DATA, "Vion Meter", "Vion smart multimeter"),

            // RuuviTag and other EddyStone data sensors
            new Specialization (typeof(Beacons.SimpleBeaconPage), new string[] { "Beacon" }, LIGHTNING, "", "Advertisement", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(Beacons.EddystonePage), new string[] { "Eddystone" }, BEACON, "", "Eddystone beacon", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(Beacons.RuuvitagPage), new string[] { "Govee_H5074_", "Ruuvi" }, DATA, "", "Environmental sensor"),
        };

        public BleNames AllBleNames { get; set; }
        public UserPreferences Preferences { get; } = new UserPreferences();
        public UserSerialPortPreferences SerialPortPreferences { get; } = new UserSerialPortPreferences();
        public string CurrJsonSearch { get; internal set; }
        IDoSearchFeedback SearchFeedback { get; set; }  = null;

 
        public string ZZZZGetCurrentSearchResults()
        {
            return CurrJsonSearch;
        }
        public MainPage()
        {
            BTAdvertisementWatcher.DeviceDisplay = this;

            UserPreferences.MainUserPreferences = Preferences;
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            MainPage.Test();
        }

        public enum SearchStatus
        {
            Searching,
            NotSearching,
        };
#if NEVER_EVER_DEFINED
//NOTE: code was added but not completed?
        private SearchStatus _CurrSearchStatus = SearchStatus.NotSearching;
        public SearchStatus CurrSearchStatus
        {
            get { return _CurrSearchStatus; }
            set
            {
                if (value == _CurrSearchStatus)
                {
                    if (value == SearchStatus.Searching)
                    {
                        // If we're already searching and the user asks to be searching,
                        // cancel the existing search and start again.
                    }
                }
            }
        }
#endif

        private static int Test()
        {
            int NError = 0;
            NError += GuidGetCommon.Test();
            NError += BleEditor.ValueToStringTest.Test();
            NError += Charts.DataCollection<int>.Test();
            return NError;
        }
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ALLOW_AD)
            {
                uiAdVertical.Suspend(); // don't display the ad at all.
                uiAdVertical.Visibility = Visibility.Collapsed;
            }

            ContentFrame.Navigated += ContentFrame_Navigated;
            Preferences.ReadFromLocalSettings();
            SerialPortPreferences.ReadFromLocalSettings();
            UpdateUIFromPreferences();

            

            // must set up searchfeedback before first navigation
            SearchFeedback = uiSearchFeedback;
            uiSearchFeedback.Search = this;
            BTAdvertisementWatcher.SearchFeedback = SearchFeedback;
            BTAdvertisementWatcher.Dispatcher = this.Dispatcher;

            // It might look like AllBleNames is never used. In reality, this initializes a static class.
            AllBleNames = new BleNames();
            await AllBleNames.InitAsync();
            await UserNameMappings.InitAsync();
            // Always start with a 'name' style read. Nothing else truly makes sense.
            //var readSelection = Preferences.DeviceReadSelection;
            //if (readSelection == UserPreferences.ReadSelection.Everything)
            //{
            //    readSelection = UserPreferences.ReadSelection.Name; // Don't automaticaly get everything on startup
            //}
            NavView_Navigate("Help", "welcome.md", null);

            StartSearch(UserPreferences.ReadSelection.Name, Preferences.Scope);

            // Set the version number in the about box. https://stackoverflow.com/questions/28635208/retrieve-the-current-app-version-from-package
            var version = Windows.ApplicationModel.Package.Current.Id.Version;
            var versionStr = $"{version.Major}.{version.Minor}";
            uiAboutVersion.Text = versionStr;

            uiDock.DockParent = this;
        }

        private void UpdateUIFromPreferences()
        {
            uiMenuFilterSimpleBeaconPageAscending.IsChecked = Preferences.MenuFilterSortDirection == UserPreferences.SortDirection.Ascending;
        }


        /// <summary>
        /// Called when we've navigated to a new page. Maybe it's a specialty page, maybe not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var content = ContentFrame.Content as HasId;
            var hasmin = (content != null);
            uiMinimizeButton.Visibility = hasmin ? Visibility.Visible : Visibility.Collapsed;
            uiDeviceStatusBox.Visibility = hasmin ? Visibility.Visible : Visibility.Collapsed;

            var handleStatus = ContentFrame.Content as SpecialtyPages.ISetHandleStatus;
            handleStatus?.SetHandleStatus(this);

            // Set the max height of the content frame?
            switch (uiPageScroller.VerticalScrollMode)
            {
                case ScrollMode.Disabled:
                    ContentFrame.MaxHeight = uiPageScroller.ActualHeight;
                    break;
                default:
                    ContentFrame.MaxHeight = Double.PositiveInfinity;
                    break;
            }
        }

        private void UiNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {

        }

        private async void UiNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var deviceWrapper = args.InvokedItemContainer.Tag as DeviceInformationWrapper;
            if (deviceWrapper != null)
            {
                if (deviceWrapper.BleAdvert != null)
                {
                    // There are two states:
                    // -- all devices, or target just the one
                    // -- speciality page, or the simple beacon page
                    // Transitions are:
                    // all-->one: if the one is govee, display it, otherwise simple beacon page
                    // one-->all: simple beacon page
                    var newAddr = deviceWrapper.BleAdvert.BleAdvert.BluetoothAddress;
                    SavedBeaconFilter = (SavedBeaconFilter == newAddr) ? 0 : deviceWrapper.BleAdvert.BleAdvert.BluetoothAddress;
                    // TODO: maybe display the specialized page?
                    // Maybe use the actual list that we've got?

                    var bp = ContentFrame.Content as SimpleBeaconPage;
                    if (SavedBeaconFilter == 0)
                    {
                        // End state is to be a SimpleBeaconPage.
                        if (bp == null) // wasn't a simple beacon page before, so have to transition to one.
                        {
                            await NavView_NavigateAsync(null, null);
                            // TODO: Don't specifiy the device wrapper -- if you include the device wrapper
                        }
                        else
                        {
                            await UpdateSimpleBeaconPage();
                        }
                    }
                    else
                    {
                        if (deviceWrapper?.BleAdvert?.AdvertisementType == BleAdvertisementWrapper.BleAdvertisementType.Govee)
                        {
                            await NavView_NavigateAsync(deviceWrapper, args.RecommendedNavigationTransitionInfo);
                        }
                        else
                        {
                            await UpdateSimpleBeaconPage();
                        }
                    }
                    return;
                }
                else // Should display its own unique page
                {
                    await NavView_NavigateAsync(deviceWrapper, args.RecommendedNavigationTransitionInfo);
                    return;
                }
            }

            //NOTE: are these ever invoked?
            var tag = args.InvokedItemContainer.Tag as string;
            switch (tag)
            {
                default:
                    if (args.IsSettingsInvoked)
                    {
                        NavView_Navigate("settings", "", args.RecommendedNavigationTransitionInfo);
                    }
                    else
                    {
                        NavView_Navigate(tag, "Help.md", args.RecommendedNavigationTransitionInfo);
                    }
                    break;
            }
        }


        /// <summary>
        /// List of ValueTuple holding the Navigation Tag and the relative Navigation Page. Used in NavView_Navigate for Help.
        /// </summary>
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ( "Help", typeof (HelpPage)),
            // e.g. ("home", typeof(HomePage)),
        };

        /// <summary>
        /// Navigate to a string (e.g., "settings" or "Help")
        /// </summary>
        private async void NavView_Navigate(string navItemTag, string pagename, NavigationTransitionInfo transitionInfo)
        {
            Type _pageType = null;
            if (navItemTag == "settings")
            {
                //_page = typeof(SettingsPage);
                var settings = new UserPreferenceControl();
                settings.SetPreferences(Preferences);
                settings.Search = this;
                var dlg = new ContentDialog()
                {
                    Content = settings,
                    PrimaryButtonText="OK",
                    Title = "Settings"
                };

                // There are two different dialogs; 
                ContentDialogResult result = ContentDialogResult.None;
                try
                {
                    await (App.Current as App).WaitForAppLockAsync("SettingsDlg");
                    result = await dlg.ShowAsync();
                    Preferences.SaveToLocalSettings(); // Always save the old values.
                }
                catch (Exception ex)
                {
                    Log($"Exception: SettingsDlg: {ex.Message}");
                }
                finally
                {
                    (App.Current as App).ReleaseAppLock("SettingsDlg");
                }
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _pageType = item.Page;
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (_pageType != null)
            {
                bool pageIsDifferentHelp = (_pageType == typeof(HelpPage)) && (pagename != HelpPage.NavigatedTo);
                bool pageIsDifferentPage = !Type.Equals(preNavPageType, _pageType);
                if ((pageIsDifferentPage || pageIsDifferentHelp))
                {
                    // Show the Filter menu for the beacon page if we should
                    var pageIsBeacon = _pageType == typeof(Beacons.SimpleBeaconPage);
                    uiFilterSimpleBeaconPage.Visibility = pageIsBeacon ? Visibility.Visible : Visibility.Collapsed;
                    SearchFeedback.SetPauseVisibility(pageIsBeacon);
                    SearchFeedback.SetSearchFeedbackType(pageIsBeacon ? SearchFeedbackType.Advertisement : SearchFeedbackType.Normal);

                    ContentFrame.Navigate(_pageType, pagename, transitionInfo);
                }
            }
        }


        private void SetParentScrolltype(Specialization.ParentScrollType scrollType)
        {
            switch (scrollType)
            {
                case Specialization.ParentScrollType.ParentShouldScroll:
                    uiPageScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
                    uiPageScroller.VerticalScrollMode = ScrollMode.Enabled;
                    break;
                case Specialization.ParentScrollType.ChildHandlesScrolling:
                    uiPageScroller.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    uiPageScroller.VerticalScrollMode = ScrollMode.Disabled;
                    break;
            }
        }

        /// <summary>
        /// Primary call to show a page for a specific bluetooth device.
        /// </summary>
        /// <param name="deviceWrapper"></param>
        /// <param name="transitionInfo"></param>
        private async Task NavView_NavigateAsync(DeviceInformationWrapper deviceWrapper, NavigationTransitionInfo transitionInfo)
        {
            Type _pageType = null;
            var preftype = Preferences.Display;
            var scrollType = Specialization.ParentScrollType.ParentShouldScroll;
            if (deviceWrapper != null) deviceWrapper.UserPreferences = Preferences;
            if (Preferences.Scope == UserPreferences.SearchScope.Bluetooth_Com_Device)
            {
                _pageType = typeof(SerialPort.SimpleTerminalPage);
                scrollType = Specialization.ParentScrollType.ChildHandlesScrolling;
                deviceWrapper.SerialPortPreferences = SerialPortPreferences;
            }
            else if (Preferences.Scope == UserPreferences.SearchScope.Bluetooth_Beacons)
            {
                var ble = deviceWrapper?.BleAdvert;
                if (ble != null)
                {
                    switch (ble.AdvertisementType)
                    {
                        case BleAdvertisementWrapper.BleAdvertisementType.Govee:
                        case BleAdvertisementWrapper.BleAdvertisementType.RuuviTag:
                            _pageType = typeof(Beacons.RuuvitagPage);
                            break;
                        case BleAdvertisementWrapper.BleAdvertisementType.Eddystone:
                            _pageType = typeof(Beacons.EddystonePage);
                            scrollType = Specialization.ParentScrollType.ChildHandlesScrolling;
                            break;
                        default:
                            _pageType = typeof(Beacons.SimpleBeaconPage);
                            scrollType = Specialization.ParentScrollType.ChildHandlesScrolling;
                            break;
                    }
                }
                else
                {
                    _pageType = typeof(Beacons.SimpleBeaconPage);
                    scrollType = Specialization.ParentScrollType.ChildHandlesScrolling;
                }
            }
            else if (preftype == UserPreferences.DisplayPreference.Specialized_Display)
            {
                if (deviceWrapper?.di == null)
                {

                }
                else
                {
                    var (deviceName, hasDeviceName) = GetDeviceInformationName(deviceWrapper?.di);
                    if (deviceName.StartsWith("Pokit"))
                    {
                        ;// handy hook for debugging
                    }

                    var specialization = Specialization.Get(Specializations, deviceName);
                    if (specialization == null && deviceWrapper != null)
                    {
                        var isUart = await deviceWrapper.IsNordicUartAsync();
                        if (isUart)
                        {
                            specialization = Specialization.Get(Specializations, Nordic_Uart.SpecializationName);
                        }
                    }
                    if (specialization != null)
                    {
                        _pageType = specialization.Page;
                        scrollType = specialization.ParentShouldScroll;
                    }
                }
            }
            if (_pageType == null) // Either the preferences is for an editor, or this particular BT devices doesn't have a specialization.
            {
                _pageType = typeof(BleEditor.BleEditorPage);
                // There's also a device information page, but it's not really useful at all.
            }
            SetParentScrolltype(scrollType);

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack. Except that I use the same page type for e.g. different Thingy devices.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            // BUT note that DeviceInformation is always the same type DeviceInformationPage
            //if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                // Not super pleased with this; there's a strong assumption that the bluetooth device id will match the deviceinfo id.
                // That's not actually required.
                if (!NewPageIsSameIdAsCurrentPage(deviceWrapper?.di?.Id ?? null))
                {
                    MinimizeCurrentWindowToDeviceDock();
                }
                // Show the Filter menu for the beacon page if we should
                var pageIsBeacon = _pageType == typeof(Beacons.SimpleBeaconPage);
                uiFilterSimpleBeaconPage.Visibility = pageIsBeacon ? Visibility.Visible : Visibility.Collapsed;
                SearchFeedback.SetPauseVisibility(pageIsBeacon);
                SearchFeedback.SetSearchFeedbackType(pageIsBeacon ? SearchFeedbackType.Advertisement : SearchFeedbackType.Normal);

                ContentFrame.Navigate(_pageType, deviceWrapper, transitionInfo);
            }
        }

        public void SetStatusText(string text)
        {
            uiDeviceStatus.Text = text;
        }

        public void SetStatusActive(bool isActive)
        {
            uiDeviceProgress.IsActive = isActive;
        }

#region DOCK

        /// <summary>
        /// Returns true iff the new page is actually the same as the current page.
        /// </summary>
        /// <returns></returns>
        private bool NewPageIsSameIdAsCurrentPage(string newId)
        {
            if (newId == null) return false;

            var currId = (ContentFrame.Content as HasId)?.GetId() ?? null;
            if (currId == null) return false;

            var isSame = (currId == newId);
            return isSame;
        }
        /// <summary>
        /// Called when trying to minimize the current window up to the dock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMinimizeWindow(object sender, RoutedEventArgs e)
        {
            MinimizeCurrentWindowToDeviceDock();
        }

        private void MinimizeCurrentWindowToDeviceDock()
        {
            var page = ContentFrame.Content as Page;
            if (page == null) return; // should never happen
            ContentFrame.Content = null; // remove the old page
            uiDock.AddPage(page); // the dock is smart enough to ignore pages that can't be docked.

            var handleStatus = page as SpecialtyPages.ISetHandleStatus;
            handleStatus?.SetHandleStatus(null); // stop handling the status.
        }


        /// <summary>
        /// Called by the PageDock when a page is ready to be undocked
        /// </summary>
        /// <param name="page"></param>
        public void DisplayFromDock(Page page)
        {
            MinimizeCurrentWindowToDeviceDock();
            ContentFrame.Content = page;

            var handleStatus = page as SpecialtyPages.ISetHandleStatus;
            handleStatus?.SetHandleStatus(this); // start handling the status again.
        }

        #endregion // DOCK

        #region DEVICE_MANAGEMENT
        /// <summary>
        /// All of the device management methods
        /// </summary>

        private void ClearDevices()
        {
            uiNavigation.MenuItems.Clear();
            MenuItemCache.Clear();
        }

        private int FindDevice(List<NavigationViewItem> list, DeviceInformation diToFind)
        {
            int idx = -1;
            for (int i = 0; i < list.Count && idx == -1; i++)
            {
                var nvi = list[i] as NavigationViewItemBase;
                var wrapper = nvi?.Tag as DeviceInformationWrapper;
                if (wrapper != null && wrapper.di != null && String.Compare(wrapper.di.Id, diToFind.Id, true, CultureInfo.InvariantCulture)==0)
                {
                    return i;
                }
            }
            return -1;
        }
        private int FindDeviceBle(BluetoothLEAdvertisementReceivedEventArgs bleToFind)
        {
            int idx = -1;
            for (int i = 0; i < uiNavigation.MenuItems.Count && idx == -1; i++)
            {
                var nvi = uiNavigation.MenuItems[i] as NavigationViewItemBase;
                var wrapper = nvi?.Tag as DeviceInformationWrapper;
                var ble = wrapper?.BleAdvert?.BleAdvert;
                if (ble != null && ble.BluetoothAddress == bleToFind.BluetoothAddress)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Smart routine that returns the name of the device. Might return NULL if the device is an un-named one. 
        /// Uses the UserNameMapping as appropriate.
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        private static (string name, bool hasName) GetDeviceInformationName(DeviceInformation di)
        {
            // Preferred names, in order, are the mapping name, the navigateTo.Name and the navigateTo.Id.
            var mapping = UserNameMappings.Get(di.Id);
            var name = mapping?.Name ?? null;
            var hasName = true;
            if (String.IsNullOrEmpty(name)) name = di.Name;
            if (String.IsNullOrEmpty(name)) name = di.Id;

            if (name.StartsWith("BluetoothLE#BluetoothLE"))
            {
                hasName = false;
                name = "Address:" + NiceId(name);
            }
            return (name, hasName);
        }

        // The list of devices in the menu on the left-hand side.
        // We keep a list of them here because the UI doesn't update them
        // very quickly.
        List<NavigationViewItem> MenuItemCache = new List<NavigationViewItem>();

        /// <summary>
        /// Adds device to navigation list. Called both when a device is added and when it's being updated
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        private async Task AddDeviceAsync(DeviceInformationWrapper wrapper)
        {
            int idx = -1;
            string name = "???";
            string id = "??-??";
            if (wrapper.di != null)
            {
                id = NiceId (wrapper.di.Id);
                var isAll_bluetooth_devices = Preferences.Scope == UserPreferences.SearchScope.Ble_All_ble_devices;
                bool hasDeviceName;
                (name, hasDeviceName) = GetDeviceInformationName(wrapper.di);
                if (!hasDeviceName && !isAll_bluetooth_devices)
                {
                    Log($"Device {id} not added because there's no name");
                    SearchFeedback.FoundDevice(FoundDeviceInfo.IsFilteredOutNoName);
                    return;
                }
                idx = FindDevice(MenuItemCache, wrapper.di);
            }


            if (idx != -1)
            {
                // Replace the old tag device information with this new one
                var nvib = MenuItemCache[idx] as NavigationViewItemBase;
                nvib.Tag = wrapper;
                var entry = nvib.Content as DeviceMenuEntryControl;
                if (entry != null)
                {
                    entry.UpdateName(name);
                    SearchFeedback.FoundDevice(FoundDeviceInfo.IsDuplicate);
                }
                else
                {
                    SearchFeedback.FoundDevice(FoundDeviceInfo.IsError);
                }
                return;
            }

            // Otherwise make a new entry
            idx = uiNavigation.MenuItems.Count(); // End of the list
            var menu = new NavigationViewItem()
            {
                Tag = wrapper,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            MenuItemCache.Add(menu); // must add to cache before we do any awaits.


            var specialization = Specialization.Get(Specializations, name);
            if (specialization == null)
            {
                var isUart = await wrapper.IsNordicUartAsync();
                if (isUart)
                {
                    specialization = Specialization.Get(Specializations, Nordic_Uart.SpecializationName);
                }
            }

            if (specialization == null && Preferences.Scope == UserPreferences.SearchScope.Ble_Has_specialized_display)
            {
                // There's no specialization and the user asked for items with a specialization only.
                // That means we shouldn't add this to the list.
                SearchFeedback.FoundDevice(FoundDeviceInfo.IsFilteredOutDb);
                Log($"Device {id} not added because there's no specialization");
                return;
            }

            var icon = GetIconForName(name);
            var dmec = new DeviceMenuEntryControl(wrapper, name, specialization, icon);
            dmec.SettingsClick += OnDeviceSettingsClick;
            menu.Content = dmec;
            uiNavigation.MenuItems.Insert(idx, menu);
            SearchFeedback.FoundDevice(FoundDeviceInfo.IsNew);
        }

        const string OTHER = "🜹";

        private static string GetIconForName(string name)
        {
            string icon = null;
            switch (name)
            {
                case "MX Anywhere 2S": icon = "🖰"; break;
                case "MS1021": icon = "🖮"; break;
                case "Charge 2": icon = "⌚"; break;
                case "Surface Pen": icon = "🖉"; break;
                default: icon = OTHER; break;
            }
            return icon;
        }
        #endregion 
        private double SIGNAL_STRENGTH_THRESHOLD { get { return Preferences.BeaconDbLevel; } }
        /// <summary>
        /// Called when a device is added and when it's updated
        /// </summary>
        /// <param name="deviceWrapper"></param>
        public void AddOrUpdateDeviceBle(DeviceInformationWrapper deviceWrapper)
        {
            var bleAdvert = deviceWrapper.BleAdvert.BleAdvert;
            var bleAdvertWrapper = deviceWrapper.BleAdvert;
            if (bleAdvert == null)
            {
                SearchFeedback.FoundDevice(FoundDeviceInfo.IsError); // filtered=false means it's not one we want. 
                return;
            }
            if (bleAdvert.RawSignalStrengthInDBm <= SIGNAL_STRENGTH_THRESHOLD)
            {
                SearchFeedback.FoundDevice(FoundDeviceInfo.IsOutOfRange); // filtered=false means it's not one we want. 
                return;
            }

            if (bleAdvert.Advertisement.LocalName.Contains ("Govee"))
            {
                ; // Hand hook for debugger.
            }
            if (bleAdvert.Advertisement.LocalName.StartsWith("LC"))
            {
                ; // Hand hook for debugger.
            }
            int idx = FindDeviceBle(bleAdvert);

            var (name, id, description) = BleAdvertisementFormat.GetBleName(bleAdvertWrapper);
            bleAdvertWrapper.CallCorrectEvent();

            var specialization = Specialization.Get(Specializations, name);
            if (specialization != null && string.IsNullOrEmpty(specialization.ShortDescription))
            {
                specialization.ShortDescription = description;
            }
            if (specialization == null)
            {
                specialization = Specialization.Get(Specializations, "Beacon"); // This will get the SimpleBeaconPage
                specialization.ShortDescription = description;
                var isScanable = bleAdvert.IsScannable;
                var isScanResponse = bleAdvert.IsScanResponse;
                if (isScanable && bleAdvert.Advertisement.LocalName.Contains("Govee"))
                {
                    ; // handy place to hang a debugger
                }
            }

            // Not eddystone or ruuvitag. Let's do the event so it can be seen
            // by the SimpleBeaconPage. 
            SimpleBeaconPage.TheSimpleBeaconPage.IsPaused = SearchFeedback.GetIsPaused(); // Transfer the search feedback UX value to the SimpleBeaconPage.
            // Yes, this is pretty awkward.
            deviceWrapper.BleAdvert.Event(deviceWrapper.BleAdvert); // TODO: why is this called again?

            if (idx != -1)
            {
                // Replace the old tag device information with this new one
                var nvib = uiNavigation.MenuItems[idx] as NavigationViewItemBase;
                nvib.Tag = deviceWrapper;
                var entry = nvib.Content as DeviceMenuEntryControl;
                if (entry != null)
                {
                    entry.Update(deviceWrapper, name, specialization, null);
                }
                SearchFeedback.FoundDevice(FoundDeviceInfo.IsDuplicate);
                return;
            }

            // Otherwise make a new entry
            idx = uiNavigation.MenuItems.Count(); // Add to the end of the list

            string icon = GetIconForName(name);
            var dmec = new DeviceMenuEntryControl(deviceWrapper, name, specialization, icon);
            dmec.SettingsClick += OnDeviceSettingsClick;
            var menu = new NavigationViewItem()
            {
                Content = dmec,
                Tag = deviceWrapper,
            };
            menu.HorizontalAlignment = HorizontalAlignment.Stretch;
            uiNavigation.MenuItems.Insert(idx, menu);
            SearchFeedback.FoundDevice(FoundDeviceInfo.IsNew); 
        }

        /// <summary>
        /// Display a pop-up for the ⚙ per-device setting
        /// </summary>
        private async void OnDeviceSettingsClick(object source, DeviceInformationWrapper di)
        {
            if (di == null) return; // The tag is always a DeviceInformation
            if (di.di == null) return;
            var (deviceName, hasDeviceName) = GetDeviceInformationName(di.di);
            var perdevice = new PerDeviceSettings(di.di, deviceName);

            var cd = new ContentDialog()
            {
                Content = perdevice,
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };
            ContentDialogResult result = ContentDialogResult.None;
            try
            {
                await (App.Current as App).WaitForAppLockAsync("JsonSearch");
                result = await cd.ShowAsync();
            }
            catch (Exception ex)
            {
                Log($"Exception: JsonSearch: {ex.Message}");
            }
            finally
            {
                (App.Current as App).ReleaseAppLock("JsonSearch");
            }

            if (result == ContentDialogResult.Primary)
            {
                // Please update the settings.
                await UserNameMappings.AddAsync(di.di.Id, perdevice.UserName);
                // Update the UI????
            }
        }

        /// <summary>
        /// All of the device watcher methods
        /// 
        /// </summary>
        /// 
        public event EventHandler DeviceEnumerationChanged;
        public void StartSearchWithUserPreferences()
        {
            // Always do a Name search.
            StartSearch(UserPreferences.ReadSelection.Name, Preferences.Scope);
        }
        private int StartSearchCount = 0; // Started since the last 'clear'
        private UserPreferences.ReadSelection LastSearchReadType;
        private UserPreferences.SearchScope LastSearchScope;
        /// <summary>
        /// Primary function to kick off a search for devices (both automation and UX 'search now' button.
        /// </summary>
        /// <param name="readType">only used for the BLE scope items</param>
        /// <param name="scope">Can be any value: the BLE (has_specialty, named, all), advert (beacon) or COM types.</param>
        public void StartSearch(UserPreferences.ReadSelection readType, UserPreferences.SearchScope scope)
        {
            // Track whether or no we should clear the screen. We clean the screen and the list
            // when it's a "new" search
            bool shouldClear = true;
            if (StartSearchCount > 0)
            {
                if (readType == LastSearchReadType
                    && scope == LastSearchScope)
                {
                    shouldClear = false; // user just click search/scan again, so we want to update the results.
                }
            }
            LastSearchReadType = readType;
            LastSearchScope = scope;
            StartSearchCount++;

            switch (scope)
            {
                case UserPreferences.SearchScope.Bluetooth_Beacons:
                    // Navigate to a beacons page
                    var _pageType = typeof(Beacons.SimpleBeaconPage);
                    var currPageType = ContentFrame.Content.GetType();
                    if (_pageType != currPageType)
                    {
                        //TODO: actually, keep the old page shouldClear = true;
                    }
                    DeviceInformationWrapper navigateTo = null;
                    if (shouldClear)
                    {
                        navigateTo = new DeviceInformationWrapper((BleAdvertisementWrapper)null);
                        navigateTo.BeaconPreferences = new UserBeaconPreferences()
                        {
                            DefaultTrackAll = true,
                        };
                    }

                    SetParentScrolltype(Specialization.ParentScrollType.ChildHandlesScrolling); // SimpleBeachPage knows how to scroll itself.

                    // Show the Filter menu for the beacon page if we should
                    var pageIsBeacon = _pageType == typeof(Beacons.SimpleBeaconPage);
                    uiFilterSimpleBeaconPage.Visibility = pageIsBeacon ? Visibility.Visible : Visibility.Collapsed;
                    SearchFeedback.SetPauseVisibility(pageIsBeacon);
                    SearchFeedback.SetSearchFeedbackType(pageIsBeacon ? SearchFeedbackType.Advertisement : SearchFeedbackType.Normal)
                        ;
                    if (navigateTo != null)
                    {
                        ContentFrame.Navigate(_pageType, navigateTo);
                    }
                    break;
                default:
                    SearchFeedback.SetPauseVisibility(false); // not a beacon, so don't display the pause
                    SearchFeedback.SetSearchFeedbackType(false ? SearchFeedbackType.Advertisement : SearchFeedbackType.Normal);
                    break;
            }
            if (shouldClear)
            {
                ClearDevices();
            }
            SearchFeedback.StartSearchFeedback();
            StartWatchForBluetoothDevices(scope);

            // In some cases, we also want to automatically read the device information.
            // For the 'Everything' value, we read in all data and display it to the user.
            // For the 'Name' value, we just read in name stuff and update the menu.
            Task searchTask = null;
            switch (scope)
            {
                case UserPreferences.SearchScope.Bluetooth_Com_Device:
                    break;
                case UserPreferences.SearchScope.Bluetooth_Beacons: 
                    break;
                default:
                    switch (readType)
                    {
                        case UserPreferences.ReadSelection.Everything:
                        case UserPreferences.ReadSelection.Name:
                            searchTask = StartDeviceReadAsync(readType); // will set the DeviceReadTask global as needed.
                            break;
                    }
                    break;
            }
        }


        public void CancelSearch()
        {
            StopWatch();
        }
        public void ClearDisplay()
        {
            //TODO: how to clear the SimpleconPage display?
            StartSearchCount = 0;
            ClearDevices();
            var sbp = ContentFrame.Content as Beacons.SimpleBeaconPage;
            sbp?.ClearDisplay(); // if it's a SimpleBeaconPage then clear it. Don't clear anything else.
        }
        public bool GetSearchActive() 
        {
            var retval = MenuDeviceInformationWatcher != null || BTAdvertisementWatcher.IsActive;
            return retval;
        }
        DeviceWatcher MenuDeviceInformationWatcher = null;
        SearchControllers.BTAdvertisementController BTAdvertisementWatcher = new SearchControllers.BTAdvertisementController();
        Task DeviceReadTask = null;
        CancellationTokenSource DeviceReadCts = null;

        private void StopWatch()
        {
            Timeout_BluetoothLEAdvertisementWatcher?.Stop();

            MenuDeviceInformationWatcher?.Stop();
            MenuDeviceInformationWatcher = null;

            BTAdvertisementWatcher?.Stop();
            SearchFeedback?.StopSearchFeedback();
        }

        enum WatcherType { DeviceInformationWatcher, BluetoothLEAdvertisementWatcher }
        private async void StartWatchForBluetoothDevices(UserPreferences.SearchScope scope)
        {
            // Query for extra properties you want returned
            // See https://docs.microsoft.com/en-us/windows/desktop/properties/devices-bumper
            string[] requestedProperties = {
                "System.Devices.Aep.DeviceAddress",
                "System.Devices.Aep.CanPair",
                "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.IsPresent", // Because I often only want devices that are here right now.
                "System.Devices.Aep.SignalStrength",
                "System.Devices.Aep.Bluetooth.Le.Appearance",
                "System.Devices.Aep.Bluetooth.Le.IsConnectable",
                "System.Devices.GlyphIcon",
                "System.Devices.Icon",
            };

            //var qstr = BluetoothLEDevice.GetDeviceSelectorFromPairingState(false);
            WatcherType watcherType = WatcherType.DeviceInformationWatcher;
            string qstr = "";
            switch (scope) // get either Bluetooth LE or COM port things
            {
                case UserPreferences.SearchScope.Bluetooth_Com_Device:
                    qstr = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
                    break;
                case UserPreferences.SearchScope.Bluetooth_Beacons:
                    watcherType = WatcherType.BluetoothLEAdvertisementWatcher;
                    break;
                default:
                    qstr = "System.Devices.Aep.ProtocolId:=\"{BB7BB05E-5972-42B5-94FC-76EAA7084D49}\"";
                    break;
            }
            switch (watcherType)
            {
                case WatcherType.DeviceInformationWatcher:
                    MenuDeviceInformationWatcher = DeviceInformation.CreateWatcher(
                        qstr,
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

                    if (Timeout_BluetoothLEAdvertisementWatcher == null)
                    {
                        var dt = new DispatcherTimer();
                        dt.Tick += Timeout_BluetoothLEAdvertisementWatcher_Tick;
                        dt.Interval = TimeSpan.FromMilliseconds(Preferences.AdvertisementScanTimeInMilliseconds);
                        Timeout_BluetoothLEAdvertisementWatcher = dt;
                    }

                    // Register event handlers before starting the watcher.
                    // Added, Updated and Removed are required to get all nearby devices
                    MenuDeviceInformationWatcher.Added += DeviceWatcher_Added;
                    MenuDeviceInformationWatcher.Updated += DeviceWatcher_Updated;
                    MenuDeviceInformationWatcher.Removed += DeviceWatcher_Removed;

                    // EnumerationCompleted and Stopped are optional to implement.
                    MenuDeviceInformationWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
                    MenuDeviceInformationWatcher.Stopped += DeviceWatcher_Stopped;

                    // Start the watcher.
                    MenuDeviceInformationWatcher.Start();
                    break;
                case WatcherType.BluetoothLEAdvertisementWatcher:
                    if (Timeout_BluetoothLEAdvertisementWatcher == null)
                    {
                        var dt = new DispatcherTimer();
                        dt.Tick += Timeout_BluetoothLEAdvertisementWatcher_Tick;
                        dt.Interval = TimeSpan.FromMilliseconds(Preferences.AdvertisementScanTimeInMilliseconds);
                        Timeout_BluetoothLEAdvertisementWatcher = dt;
                    }
                    BTAdvertisementWatcher.Start(); 
                    Timeout_BluetoothLEAdvertisementWatcher.Start();

                    await ListAllAdapters();

                    break;
            }
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
        }

        private async Task ListAllAdapters()
        {
            // List stuff about the radio
            Log("List all adapters");
            var defaultAdapter = await BluetoothAdapter.GetDefaultAsync();

            var selector = BluetoothAdapter.GetDeviceSelector();
            var adapterList = await DeviceInformation.FindAllAsync(selector);
            foreach (var di in adapterList)
            {
                var adapter = await BluetoothAdapter.FromIdAsync(di.Id);
                var isDefault = defaultAdapter.BluetoothAddress == adapter.BluetoothAddress;
                var radio = await adapter.GetRadioAsync();
                var (diname, hasdiname) = GetDeviceInformationName(di);
                Log($"    Adapter: {diname} {adapter.DeviceId} default?={isDefault} address={BluetoothAddress.AsString(adapter.BluetoothAddress)}");
                Log($"        MaxAdvertisementDataLength={adapter.MaxAdvertisementDataLength}");
                Log($"        IsExtendedAdvertisingSupported={adapter.IsExtendedAdvertisingSupported}");
                Log($"        IsAdvertisementOffloadSupported={adapter.IsAdvertisementOffloadSupported}");
                Log($"        IsLowEnergySupported={adapter.IsLowEnergySupported}");
                Log($"        IsClassicSupported={adapter.IsClassicSupported}");
                Log($"        IsCentralRoleSupported={adapter.IsCentralRoleSupported}");
                Log($"        IsPeripheralRoleSupported={adapter.IsPeripheralRoleSupported}");
                Log($"        Radio Name={radio.Name}");
                Log($"        Radio State={radio.State}");
                Log($"        Radio Kind={radio.Kind}");
                Log("");
            }
        }

        private void Timeout_BluetoothLEAdvertisementWatcher_Tick(object sender, object e)
        {
            MenuDeviceInformationWatcher?.Stop();
            Timeout_BluetoothLEAdvertisementWatcher?.Stop();
            CancelSearch(); // will call BTAdvertisementWatcher?.Stop() // will call MenuBleWatcher?.Stop(); // will trigger MenuBleWatcher_Stopped
        }

        DispatcherTimer Timeout_BluetoothLEAdvertisementWatcher = null;

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            Log($"DeviceWatcher: Stopped");
            MenuDeviceInformationWatcher = null;
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
            SearchFeedback?.StopSearchFeedback();
        }

        private void Log(string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            Log($"DeviceWatcher: Enumeration Completed");
            MenuDeviceInformationWatcher = null;
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
            SearchFeedback?.StopSearchFeedback();
        }

        private static string NiceId (string id) // was DeviceInformation args)
        {
            var retval = id;
            var idx = retval.IndexOf('-');
            if (retval.StartsWith ("BluetoothLE#BluetoothLE") && idx >=0)
            {
                retval = retval.Substring(idx + 1);
            }
            return retval;
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            var id = NiceId(args.Id);
            await uiNavigation.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                async () => {
                    try
                    {
                        await AddDeviceAsync(new DeviceInformationWrapper(args));
                    }
                    catch (Exception)
                    {
                        ;
                    }
                });
        }
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var id = NiceId(args.Id);
            Log($"DeviceWatcher: Device {id} Removed");
        }
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var id = NiceId(args.Id);

            // Play around with getting a strength value!
            object strength = null;
            args.Properties.TryGetValue("System.Devices.Aep.SignalStrength", out strength);
            if (strength != null && strength is int)
            {
                ;
                var strvalue = (int)strength;
                Log($"DeviceWatcher: Device {id} updated strength {strength}");
            }
            else
            {
                Log($"DeviceWatcher: Device {id} updated ");
            }
        }

        /// <summary>
        /// Called from StartSearch. Starts a task to read data from devices; only valid for Name and Everthing (not Address)
        /// </summary>
        /// <param name="readType">must be name or everthing</param>
        /// <returns></returns>
        private async Task StartDeviceReadAsync(UserPreferences.ReadSelection readType)
        {
            // Step one: if we're already running, kill the old task
            if (DeviceReadTask != null)
            {
                // Must cancel
                if (DeviceReadCts != null)
                {
                    DeviceReadCts?.Cancel();
                    while (!DeviceReadTask.IsCompleted)
                    {
                        await Task.Delay(50);
                    }
                    DeviceReadTask = null;
                }
            }

            // Now go for realsie
            DeviceReadCts = new CancellationTokenSource();
            // Special code for automation: will do a complete sweep of all BT devices and fill in 
            // the global CurrJsonSearch string variable of the devices.
            switch (readType)
            {
                case UserPreferences.ReadSelection.Name:
                    DeviceReadTask = ReadNameAsync(DeviceReadCts.Token);
                    break;
                case UserPreferences.ReadSelection.Everything:
                    DeviceReadTask = ReadEverythingAsync(DeviceReadCts.Token);
                    break;
            }
        }


        /// <summary>
        /// For each item in the MenuItemCache, display is on the screen and grab the appropriate JSON. JSON is displayed in the end.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        private async Task ReadEverythingAsync(CancellationToken ct)
        {
            CurrJsonSearch = "";

            var deviceJson = "";
            await Task.Delay(1000); // pause 1000 ms = 1 s to get some BT
            //int startidx = FindListStart() + 1;
            //int endidx = FindListEnd() - 1;
            if (ct.IsCancellationRequested) return;
            for (int i=0; i< MenuItemCache.Count; i++)
            {
                var nvi = MenuItemCache[i] as NavigationViewItemBase;
                var di = nvi?.Tag as DeviceInformationWrapper;
                if (di != null)
                {
                    // Got a device to investigate.
                    var _pageType = typeof(BleEditor.BleEditorPage);

                    // Show the Filter menu for the beacon page if we should
                    var pageIsBeacon = _pageType == typeof(Beacons.SimpleBeaconPage);
                    uiFilterSimpleBeaconPage.Visibility = pageIsBeacon ? Visibility.Visible : Visibility.Collapsed;
                    SearchFeedback.SetPauseVisibility(pageIsBeacon);
                    SearchFeedback.SetSearchFeedbackType(pageIsBeacon ? SearchFeedbackType.Advertisement : SearchFeedbackType.Normal);

                    ContentFrame.Navigate(_pageType, di);
                    var editor = ContentFrame.Content as BleEditor.BleEditorPage;
                    while (!editor.NavigationComplete && !ct.IsCancellationRequested)
                    {
                        await Task.Delay(1000); // Navigation, alas, takes a while
                    }

                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    var json = editor?.JsonAsSingle;
                    if (!String.IsNullOrEmpty(json))
                    {
                        if (deviceJson != "")
                        {
                            deviceJson += ",\n";
                        }
                        deviceJson += json;
                    }
                }

                // Always re-get the end because it's always being updated.
                // note that this is no longer needed since I just use the MenuItemCache which has an easy count. endidx = FindListEnd() - 1;
            }

            // Even on cancel show the text that we have.
            if (!String.IsNullOrEmpty(deviceJson))
            {
                // Write it out!
                CurrJsonSearch = deviceJson;
                await DisplayTextFromSearchAsync(deviceJson);
            }
        }

        private async Task DisplayTextFromSearchAsync(string text)
        {
            var tb = new TextBlock()
            {
                IsTextSelectionEnabled = true,
                TextWrapping = TextWrapping.Wrap,
                Text = text,
            };
            var scroll = new ScrollViewer()
            {
                Content = tb,
                MaxHeight = 400,
            };
            var cdb = new ContentDialog()
            {
                Content = scroll,
                Title = "Raw Bluetooth devices",
                PrimaryButtonText = "OK",
            };
            try
            {
                await (App.Current as App).WaitForAppLockAsync("JsonSearch");
                await cdb.ShowAsync();
            }
            catch (Exception ex)
            {
                Log($"Exception: JsonSearch: {ex.Message}");
            }
            finally
            {
                (App.Current as App).ReleaseAppLock("JsonSearch");
            }
        }

        readonly Guid CommonConfigurationGuid1800 = BluetoothLEStandardServices.Display; //  Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"); // service
        readonly Guid DeviceNameGuid2a00 = BluetoothLEStandardServices.Name;  // new Guid("00002a00-0000-1000-8000-00805f9b34fb"); // characteristic

        /// <summary>
        /// Reads just the name from the list of devices in the MenuItemCache and also all services and characteristics
        /// </summary>
        private async Task ReadNameAsync(CancellationToken ct)
        {
            var deviceJson = "";
            await Task.Delay(1000); // pause 1000 ms = 1 s to get some BT
            //int startidx = FindListStart() + 1;
            //int endidx = FindListEnd() - 1;
            if (ct.IsCancellationRequested) return;


            for (int i = 0; i < MenuItemCache.Count; i++)
            {
                var nvi = MenuItemCache[i] as NavigationViewItemBase;
                var di = nvi?.Tag as DeviceInformationWrapper;
                if (di == null) continue;
                try
                {
                    var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
                    if (ble == null) continue;
                    if (!string.IsNullOrEmpty(ble.Name))
                    {
                        if (deviceJson != "")
                        {
                            deviceJson += "\n,";
                        }
                        deviceJson += $"\"Existing_{ble.Name}\""; continue;
                    }
                    Log($"DEVICE: Get name for {di.di.Id}");

                    // This looks like it's reading lots of services and characteristics, but it's not.
                    // It's only reading Service 1800 (common configuration) Chara 2a00 (device name).
                    // The loops are there because 'technically' there could be multiple services and
                    // characteristics with the same ID. In practice, never.
                    var services = await ble.GetGattServicesForUuidAsync(CommonConfigurationGuid1800);
                    if (services.Status != GattCommunicationStatus.Success) continue;

                    int nservice = 0;
                    int ncharacteristic = 0;
                    foreach (var service in services.Services)
                    {
                        var characteristics = await service.GetCharacteristicsForUuidAsync(DeviceNameGuid2a00);
                        if (characteristics.Status != GattCommunicationStatus.Success) continue;
                        nservice++;
                        foreach (var characteristic in characteristics.Characteristics)
                        {
                            var read = await characteristic.ReadValueAsync();
                            if (read.Status != GattCommunicationStatus.Success) continue;
                            ncharacteristic++;
                            var name = BluetoothLEStandardServices.CharacteristicData.ValueAsString(BluetoothLEStandardServices.DisplayType.String, read.Value);
                            Log($"  --> read name as {name}");

                            var entry = nvi.Content as DeviceMenuEntryControl;
                            if (entry != null)
                            {
                                await this.Dispatcher.RunAsync(
                                    Windows.UI.Core.CoreDispatcherPriority.Normal,
                                    () => { entry.UpdateName(name); });
                            }
                            if (deviceJson != "")
                            {
                                deviceJson += "\n,";
                            }
                            deviceJson += $"\"Found_{name}\"";
                        }
                    }
                    if (nservice > 1 || ncharacteristic > 1)
                    {
                        Log($"  SURPRISE: ble={ble.Name} nservice={nservice} ncharacteristic={ncharacteristic}");
                    }
                }
                catch (Exception ex)
                {
                    Log($"ERROR: unable to navigate {ex.Message}");
                    // I don't know of any exceptions. But if there are any, supress them completely.
                }
            }
        }
        public void PauseCheckUpdated(bool newIsChecked)
        {
            if (!newIsChecked)
            {
                // TODO: update 
                SimpleBeaconPage.TheSimpleBeaconPage.RedrawDisplay();
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
        }


        private void OnAdRefreshed(object sender, RoutedEventArgs e)
        {
            var ad = sender as AdControl;
            Log($"NOTE: AdControl Refresh {ad.Name}");
            ;
        }

        private void OnAdErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            var ad = sender as AdControl;
            Log($"NOTE: AdControl Error {ad.Name} Message:{e.ErrorMessage} ErrorCode:{e.ErrorCode}");
            ;
        }

        #region MENU
        private async void OnRequestFeedback(object sender, TappedRoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

#if NEVER_EVER_DEFINED
        private async void OnGenerateCode(object sender, RoutedEventArgs e)
        {
            var dlg = new ContentDialog()
            {
                Content = new GenerateCodeControl(),
                PrimaryButtonText = "OK",
                Title = "Generate Code from JSON descriptions",
                Width = 500,
            };
            await dlg.ShowAsync();
        }
#endif
        private void OnMenuAutomationSearchCopyJson(object sender, RoutedEventArgs e)
        {
            var value = CurrJsonSearch;
            if (String.IsNullOrEmpty(value)) return;
            var dp = new DataPackage();
            // Convert the list of individual items into a proper list.
            value = @"{
  ""AllDevices"": [
" + value + @"
  ]
}";

            dp.SetText(value);
            dp.Properties.Title = "JSON Bluetooth data";
            Clipboard.SetContent(dp);
        }

        private void MenuOnSweepBeaconAdvertisement(object sender, RoutedEventArgs e)
        {
            CancelSearch();
            StartSearch(UserPreferences.ReadSelection.Name, UserPreferences.SearchScope.Bluetooth_Beacons);
        }

        private void MenuOnSweepBleName(object sender, RoutedEventArgs e)
        {
            CancelSearch();
            StartSearch(UserPreferences.ReadSelection.Name, UserPreferences.SearchScope.Ble_All_ble_devices);
        }

        private void MenuOnSweepBleFull(object sender, RoutedEventArgs e)
        {
            CancelSearch();
            StartSearch(UserPreferences.ReadSelection.Everything, UserPreferences.SearchScope.Ble_All_ble_devices);
        }

        // Stuff needed to keep the screen on
        static Windows.System.Display.DisplayRequest CurrDisplayRequest = null;

        private void MenuOnToggleScreenOn(object sender, RoutedEventArgs e)
        {
            if (CurrDisplayRequest == null)
            {
                CurrDisplayRequest = new Windows.System.Display.DisplayRequest();
            }
            var ischeck = uiMenuKeepScreenOn.IsChecked;
            if (ischeck)
            {
                CurrDisplayRequest.RequestActive();
            }
            else
            {
                CurrDisplayRequest.RequestRelease();
            }
        }

        UserPreferences.SortBy SavedBeaconSortBy = UserPreferences.SortBy.Time;
        // Can delete any time: Sort direction is in preferences now: SimpleBeaconPage.SortDirection SavedBeaconSortDirection = SimpleBeaconPage.SortDirection.Ascending;

        /// <summary>
        /// Controls whether we're showing all beacons, or just the selected on. Is toggled in UiNavigation_ItemInvoked when an item is clicked.
        /// </summary>
        ulong SavedBeaconFilter = 0;


        const int DEFAULT_SEARCH_DELAY_UX = 100; // TOO SLOW: 500
        private async void MenuOnFilterBeaconSortByMac(object sender, RoutedEventArgs e)
        {
            SavedBeaconSortBy = UserPreferences.SortBy.Mac;
            await UpdateSimpleBeaconPage();
        }

        private async void MenuOnFilterBeaconSortByTime(object sender, RoutedEventArgs e)
        {
            SavedBeaconSortBy = UserPreferences.SortBy.Time;
            await UpdateSimpleBeaconPage();
        }

        private async void MenuOnFilterBeaconSortByRSS(object sender, RoutedEventArgs e)
        {
            SavedBeaconSortBy = UserPreferences.SortBy.Rss;
            await UpdateSimpleBeaconPage();
        }

        private async void OnClientFilterSimpleBeaconPageAscending(object sender, RoutedEventArgs e)
        {
            // Might be called while we're still loading. But it's not called? Evn through there's a callback, and
            // other UI elements are called when they are set while loading?
            if (!this.IsLoaded)
            {
                return;
            }
            Preferences.MenuFilterSortDirection  = uiMenuFilterSimpleBeaconPageAscending.IsChecked ? UserPreferences.SortDirection.Ascending : UserPreferences.SortDirection.Descending;
            Preferences.SaveToLocalSettings();
            await UpdateSimpleBeaconPage();
        }

        private async Task UpdateSimpleBeaconPage()
        {
            SimpleBeaconPage.TheSimpleBeaconPage.CurrMillisecondsDelay = DEFAULT_SEARCH_DELAY_UX;
            SimpleBeaconPage.TheSimpleBeaconPage.CurrSortBy = SavedBeaconSortBy;
            SimpleBeaconPage.TheSimpleBeaconPage.CurrSortDirection = Preferences.MenuFilterSortDirection;
            SimpleBeaconPage.TheSimpleBeaconPage.CurrBleAddressFilter = SavedBeaconFilter;

            await SimpleBeaconPage.TheSimpleBeaconPage.SortAsync();
        }

        private async void OnMenuHelpAbout(object sender, RoutedEventArgs e)
        {
            await uiDialogAbout.ShowAsync();
        }

        private void OnMenuHelpHelp(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement).Tag as string;
            NavView_Navigate(tag, "Help.md", null);
        }

        private void OnMenuHelpFeedback(object sender, RoutedEventArgs e)
        {
            OnRequestFeedback(null, null);
        }

        /// <summary>
        /// From the search feedback "Filtered Out" text. Goal is to show the user which devices were filtered out and why.
        /// </summary>
        public void ListFilteredOut()
        {
        }

        /// <summary>
        /// Hardly needed any more now that we have the newer command-line code generation tool BluetoothCodeGeneratordotNetCore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MenuOnGenerateCode(object sender, RoutedEventArgs e)
        {
            var dlg = new ContentDialog()
            {
                Content = new GenerateCodeControl(),
                PrimaryButtonText = "OK",
                Title = "Generate Code from JSON descriptions",
                Width = 500,
            };
            await dlg.ShowAsync();
        }
#endregion MENU
    }
}
