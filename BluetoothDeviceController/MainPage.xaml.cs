using BluetoothDeviceController.Beacons;
using BluetoothDeviceController.BluetoothProtocolsCustom;
using BluetoothDeviceController.Names;
using BluetoothDeviceController.UserData;
using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
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
        void StartSearch(UserPreferences.ReadSelection searchType);
        void CancelSearch();
        event EventHandler DeviceEnumerationChanged;
        string GetCurrentSearchResults();
    }


    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IDoSearch, IDockParent, SpecialtyPages.IHandleStatus
    {
#if DEBUG
        const bool ALLOW_AD = false;
#else
        const bool ALLOW_AD = true;
#endif

        const string ALARM = "⏰";
        const string BEACON = "⛯"; // MAP SYMBOL FOR LIGHTHOUSE //"🖄";
        const string CAR = "🚗";
        const string DATA = "📈"; //"🥼";
        const string HEALTH = "🏥"; //"🥼";
        const string LIGHT = "💡";
        const string LIGHTNING = "🖄"; // envelope with lightning
        const string ROBOT = ""; // Part of the Segoe MDL2 Assets FontFamily
        const string TRAIN = "🚂";
        const string UART = "🖀";
        const string WAND = "🖉"; // yeah, a pencil isn't reall a wand.
        const string WATER = "🚰";

        
        /// <summary>
        /// Converts a list of possible Bluetooth names into a page to display
        /// </summary>
        List<Specialization> Specializations = new List<Specialization>()
        {
            // Fun specializations
            new Specialization (typeof(SpecialtyPages.Kano_WandPage), new string[] { "Kano-Wand" }, WAND, "Kano Wand", "Kano coding kit Harry Potter Wand"),
            new Specialization (typeof(SpecialtyPagesCustom.UartPage), new string[] { "Puck" }, UART, "Espruino (puck)", "Puck.js: a UART-based Espruino device", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(SpecialtyPagesCustom.UartPage), new string[] { Nordic_Uart.SpecializationName }, UART, "UART Comm port", "Nordic UART comm port", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(SpecialtyPagesCustom.CraftyRobot_SmartibotPage), new string[] { "Smartibot" }, ROBOT, "Smartibot", "Smartibot espruino-based robot", Specialization.ParentScrollType.ChildHandlesScrolling),
            new Specialization (typeof(SpecialtyPages.Elegoo_MiniCarPage), new string[] { "ELEGOO BT16" }, CAR, "Elegoo Mini-Car", "Elegoo small robot car"),
            new Specialization (typeof(SpecialtyPages.Lionel_LionChiefPage), new string[] { "LC" }, TRAIN, "LionChief Train", "Lionel Train Controller"),
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
            new Specialization (typeof(Beacons.RuuvitagPage), new string[] { "Ruuvi" }, DATA, "", "RuuviTag environmental sensor"),
        };

        public BleNames AllBleNames { get; set; }
        public UserPreferences Preferences { get; } = new UserPreferences();
        public UserSerialPortPreferences SerialPortPreferences { get; } = new UserSerialPortPreferences();
        public string CurrJsonSearch { get; internal set; }
        public string GetCurrentSearchResults()
        {
            return CurrJsonSearch;
        }
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            Test();
        }

        public enum SearchStatus
        {
            Searching,
            NotSearching,
        };
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

        private int Test()
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

            // It might look like AllBleNames is never used. In reality, this initializes a static class.
            AllBleNames = new BleNames();
            await AllBleNames.InitAsync();
            await UserNameMappings.InitAsync();
            var readSelection = Preferences.DeviceReadSelection;
            if (readSelection == UserPreferences.ReadSelection.Everything)
            {
                readSelection = UserPreferences.ReadSelection.Name; // Don't automaticaly get everything
            }
            NavView_Navigate("Help", "welcome.md", null);
            StartSearch(readSelection);
            uiDock.DockParent = this;
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
            var di = args.InvokedItemContainer.Tag as DeviceInformationWrapper;
            if (di != null)
            {
                await NavView_NavigateAsync(di, args.RecommendedNavigationTransitionInfo);
                return;
            }

            var tag = args.InvokedItemContainer.Tag as string;
            switch (tag)
            {
                case "Feedback":
                    var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
                    await launcher.LaunchAsync();
                    break;
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


        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ( "Help", typeof (HelpPage)),
            // e.g. ("home", typeof(HomePage)),
        };

        private async void NavView_Navigate(string navItemTag, string pagename, NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
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
                    System.Diagnostics.Debug.WriteLine($"Exception: SettingsDlg: {ex.Message}");
                }
                finally
                {
                    (App.Current as App).ReleaseAppLock("SettingsDlg");
                }
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (_page != null)
            {
                bool pageIsDifferentHelp = (_page == typeof(HelpPage)) && (pagename != HelpPage.NavigatedTo);
                bool pageIsDifferentPage = !Type.Equals(preNavPageType, _page);
                if ((pageIsDifferentPage || pageIsDifferentHelp))
                {
                    ContentFrame.Navigate(_page, pagename, transitionInfo);
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
        /// <param name="wrapper"></param>
        /// <param name="transitionInfo"></param>
        private async Task NavView_NavigateAsync(DeviceInformationWrapper wrapper, NavigationTransitionInfo transitionInfo)
        {
            Type _pageType = null;
            var preftype = Preferences.Display;
            var scrollType = Specialization.ParentScrollType.ParentShouldScroll;
            if (wrapper != null) wrapper.UserPreferences = Preferences;
            if (Preferences.Scope == UserPreferences.SearchScope.Bluetooth_Com_Device)
            {
                _pageType = typeof(SerialPort.SimpleTerminalPage);
                scrollType = Specialization.ParentScrollType.ChildHandlesScrolling;
                wrapper.SerialPortPreferences = SerialPortPreferences;
            }
            else if (Preferences.Scope == UserPreferences.SearchScope.Bluetooth_Beacons)
            {
                var ble = wrapper.BleAdvert;
                if (ble != null)
                {
                    switch (ble.AdvertisementType)
                    {
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
                if (wrapper?.di == null)
                {

                }
                else
                {
                    var deviceName = GetDeviceInformationName(wrapper?.di);

                    var specialization = Specialization.Get(Specializations, deviceName);
                    if (specialization == null && wrapper != null)
                    {
                        var isUart = await wrapper.IsNordicUartAsync();
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
                if (!NewPageIsSameIdAsCurrentPage(wrapper?.di?.Id ?? null))
                {
                    MinimizeCurrentWindowToDeviceDock();
                }
                ContentFrame.Navigate(_pageType, wrapper, transitionInfo);
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

        /// <summary>
        /// All of the device management methods
        /// </summary>

        private void ClearDevices()
        {
            List<object> removelist = new List<object>();
            bool removing = false;
            foreach (var item in uiNavigation.MenuItems)
            {
                var nvi = item as NavigationViewItemBase;
                if (nvi?.Tag as string == "LIST-START")
                {
                    removing = true;
                }
                else if (nvi?.Tag as string == "LIST-END")
                {
                    removing = false;
                }
                else if (removing)
                {
                    removelist.Add(item);
                }
            }
            foreach (var item in removelist)
            {
                uiNavigation.MenuItems.Remove(item);
            }
        }

        private int FindDevice(DeviceInformation diToFind)
        {
            int idx = -1;
            for (int i = 0; i < uiNavigation.MenuItems.Count && idx == -1; i++)
            {
                var nvi = uiNavigation.MenuItems[i] as NavigationViewItemBase;
                var wrapper = nvi?.Tag as DeviceInformationWrapper;
                if (wrapper != null && wrapper.di != null && wrapper.di.Id == diToFind.Id)
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

        private int FindListStart()
        {
            for (int i = 0; i < uiNavigation.MenuItems.Count; i++)
            {
                var nvi = uiNavigation.MenuItems[i] as NavigationViewItemBase;
                if (nvi?.Tag as string == "LIST-START")
                {
                    return i;
                }
            }
            return -1;
        }

        private int FindListEnd()
        {
            for (int i = 0; i < uiNavigation.MenuItems.Count; i++)
            {
                var nvi = uiNavigation.MenuItems[i] as NavigationViewItemBase;
                if (nvi?.Tag as string == "LIST-END")
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
        private static string GetDeviceInformationName(DeviceInformation di, bool keepUnnamed = true)
        {
            // Preferred names, in order, are the mapping name, the di.Name and the di.Id.
            var mapping = UserNameMappings.Get(di.Id);
            var name = mapping?.Name ?? null;
            if (String.IsNullOrEmpty(name)) name = di.Name;
            if (String.IsNullOrEmpty(name)) name = di.Id;

            if (name.StartsWith("BluetoothLE#BluetoothLEbc:83:85:22:5a:70-"))
            {
                if (!keepUnnamed)
                {
                    // These are sometimes interesting, but mostly are not
                    return null;
                }
                name = name.Replace("BluetoothLE#BluetoothLEbc:83:85:22:5a:70-", "Address:");
            }
            return name;
        }

        /// <summary>
        /// Called both when a device is added and when it's being updated
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
                id = wrapper.di.Id.Replace("BluetoothLE#BluetoothLEbc:83:85:22:5a:70-", "");
                var includeAll = Preferences.Scope == UserPreferences.SearchScope.All_bluetooth_devices;
                name = GetDeviceInformationName(wrapper.di, includeAll); // gets null for unnamed devices unless we want all devices.
                if (name == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Device {id} not added because there's no name");
                    return;
                }
                idx = FindDevice(wrapper.di);
            }

            if (idx != -1)
            {
                // Replace the old tag device information with this new one
                var nvib = uiNavigation.MenuItems[idx] as NavigationViewItemBase;
                nvib.Tag = wrapper;
                var entry = nvib.Content as DeviceMenuEntryControl;
                if (entry != null)
                {
                    entry.UpdateName(name);
                }
                //nvib.Content = name;
                return;
            }

            // Otherwise make a new entry
            idx = FindListEnd();
            if (idx == -1) idx = 0; // Impossible; the list always includes the list-stat and list-end

            var specialization = Specialization.Get(Specializations, name);
            if (specialization == null)
            {
                var isUart = await wrapper.IsNordicUartAsync();
                if (isUart)
                {
                    specialization = Specialization.Get(Specializations, Nordic_Uart.SpecializationName);
                }
            }

            if (specialization == null && Preferences.Scope == UserPreferences.SearchScope.Has_specialized_display)
            {
                // There's no specialization and the user asked for items with a specialization only.
                // That means we shouldn't add this to the list.
                System.Diagnostics.Debug.WriteLine($"Device {id} not added because there's no specialization");
                return;
            }

            var icon = GetIconForName(name);
            var dmec = new DeviceMenuEntryControl(wrapper, name, specialization, icon);
            dmec.SettingsClick += OnDeviceSettingsClick;
            var menu = new NavigationViewItem()
            {
                Content = dmec,
                Tag = wrapper,
            };
            menu.HorizontalAlignment = HorizontalAlignment.Stretch;
            uiNavigation.MenuItems.Insert(idx, menu);
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


        /// <summary>
        /// Called when a device is added and when it's updated
        /// </summary>
        /// <param name="wrapper"></param>
        private void AddOrUpdateDeviceBle(DeviceInformationWrapper wrapper)
        {
            const int SIGNAL_STRENGTH_THRESHOLD = -95;

            var bleAdvert = wrapper.BleAdvert.BleAdvert;
            if (bleAdvert == null) return;
            if (bleAdvert.RawSignalStrengthInDBm < SIGNAL_STRENGTH_THRESHOLD)
            {
                return;
            }

            if (bleAdvert.Advertisement.LocalName.Contains ("Wescale"))
            {
                ; // Hand hook for debugger.
            }
            if (bleAdvert.Advertisement.LocalName.StartsWith("LC"))
            {
                ; // Hand hook for debugger.
            }
            int idx = FindDeviceBle(bleAdvert);

            var (name, id, description) = BleAdvertisementFormat.GetBleName(wrapper, bleAdvert);


            var specialization = Specialization.Get(Specializations, name);
            if (specialization != null && string.IsNullOrEmpty(specialization.ShortDescription))
            {
                specialization.ShortDescription = description;
            }
            if (specialization == null)
            {
                specialization = Specialization.Get(Specializations, "Beacon"); // This will get the SimpleBeaconPage
                specialization.ShortDescription = description;

                // Not eddystone or ruuvitag. Let's do the event so it can be seen
                // by the SimpleBeaconPage. 
                wrapper.BleAdvert.Event(bleAdvert);
            }

            if (idx != -1)
            {
                // Replace the old tag device information with this new one
                var nvib = uiNavigation.MenuItems[idx] as NavigationViewItemBase;
                nvib.Tag = wrapper;
                var entry = nvib.Content as DeviceMenuEntryControl;
                if (entry != null)
                {
                    entry.Update(wrapper, name, specialization, null);
                }
                return;
            }

            // Otherwise make a new entry
            idx = FindListEnd();
            if (idx == -1) idx = 0; // Impossible; the list always includes the list-stat and list-end

            string icon = GetIconForName(name);
            var dmec = new DeviceMenuEntryControl(wrapper, name, specialization, icon);
            dmec.SettingsClick += OnDeviceSettingsClick;
            var menu = new NavigationViewItem()
            {
                Content = dmec,
                Tag = wrapper,
            };
            menu.HorizontalAlignment = HorizontalAlignment.Stretch;
            uiNavigation.MenuItems.Insert(idx, menu);
        }

        /// <summary>
        /// Display a pop-up for the ⚙ per-device setting
        /// </summary>
        private async void OnDeviceSettingsClick(object source, DeviceInformationWrapper di)
        {
            if (di == null) return; // The tag is always a DeviceInformation
            if (di.di == null) return;
            var name = GetDeviceInformationName(di.di);

            var oldName = GetDeviceInformationName(di.di, true);
            var perdevice = new PerDeviceSettings(di.di, oldName);

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
                System.Diagnostics.Debug.WriteLine($"Exception: JsonSearch: {ex.Message}");
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
        public void StartSearch(UserPreferences.ReadSelection readType)
        {
            switch (Preferences.Scope)
            {
                case UserPreferences.SearchScope.Bluetooth_Beacons:
                    // Navigate to a beacons page
                    // tODO: navigate
                    var _page = typeof(Beacons.SimpleBeaconPage);
                    var di = new DeviceInformationWrapper((BleAdvertisementWrapper)null);
                    di.BeaconPreferences = new UserBeaconPreferences()
                    {
                        DefaultTrackAll = true,
                    };
                    SetParentScrolltype(Specialization.ParentScrollType.ChildHandlesScrolling); // SimpleBeachPage knows how to scroll itself.
                    ContentFrame.Navigate(_page, di);
                    break;
            }
            ClearDevices();
            StartWatch();
            Task searchTask = null;
            switch (Preferences.Scope)
            {
                case UserPreferences.SearchScope.Bluetooth_Com_Device:
                    break;
                case UserPreferences.SearchScope.Bluetooth_Beacons: //TODO: do what?
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

        public bool GetSearchActive() 
        {
            return MenuDeviceInformationWatcher != null;
        }
        DeviceWatcher MenuDeviceInformationWatcher = null;
        BluetoothLEAdvertisementWatcher MenuBleWatcher = null;
        Task DeviceReadTask = null;
        CancellationTokenSource DeviceReadCts = null;

        private void StopWatch()
        {
            MenuDeviceInformationWatcher?.Stop();
            MenuDeviceInformationWatcher = null;

            MenuBleWatcher?.Stop();
            MenuBleWatcher = null;
        }

        enum WatcherType { DeviceInformationWatcher, BluetoothLEWatcher }
        private void StartWatch()
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
            switch (Preferences.Scope) // get either Bluetooth LE or COM port things
            {
                case UserPreferences.SearchScope.Bluetooth_Com_Device:
                    qstr = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
                    break;
                case UserPreferences.SearchScope.Bluetooth_Beacons:
                    watcherType = WatcherType.BluetoothLEWatcher;
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
                case WatcherType.BluetoothLEWatcher:
                    //TODO: watcher type!!!!
                    MenuBleWatcher = new BluetoothLEAdvertisementWatcher();
                    MenuBleWatcher.Received += MenuBleWatcher_Received;
                    MenuBleWatcher.Start();
                    break;
            }
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
        }

        Dictionary<ulong, BleAdvertisementWrapper> BleWrappers = new Dictionary<ulong, BleAdvertisementWrapper>();

        private async void MenuBleWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceBleWatcher: Device {args.Advertisement.LocalName} seen");
            if (args.Advertisement.LocalName.Contains("Wescale"))
            {
                ; // Handy hook for debugger.
            }
            if (args.Advertisement.LocalName.StartsWith("LC"))
            {
                ; // Handy hook for debugger.
            }

            // Get the appropriate ble advert wrapper (or make a new one)
            BleAdvertisementWrapper wrapper = null;
            BleWrappers.TryGetValue(args.BluetoothAddress, out wrapper);
            if (wrapper == null)
            {
                wrapper = new BleAdvertisementWrapper();
                wrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.BluetoothLE;
                BleWrappers.Add(args.BluetoothAddress, wrapper);
            }
            wrapper.BleAdvert = args;
            var diwrapper = new DeviceInformationWrapper(wrapper);

            await uiNavigation.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () => { AddOrUpdateDeviceBle(diwrapper); }
                );
        }

        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Stopped");
            MenuDeviceInformationWatcher = null;
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Enumeration Completed");
            MenuDeviceInformationWatcher = null;
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
        }



        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            //Testing the IsPresent value.
            //object result;
            //bool got = args.Properties.TryGetValue("System.Devices.Aep.IsPresent", out result);
            var id = args.Id.Replace("BluetoothLE#BluetoothLEbc:83:85:22:5a:70-", "");
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Device {id} Added");
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
            var id = args.Id.Replace("BluetoothLE#BluetoothLEbc:83:85:22:5a:70-", "");
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Device {id} Removed");
        }
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            var id = args.Id.Replace("BluetoothLE#BluetoothLEbc:83:85:22:5a:70-", "");

            // Play around with getting a strength value!
            object strength = null;
            args.Properties.TryGetValue("System.Devices.Aep.SignalStrength", out strength);
            if (strength != null && strength is int)
            {
                ;
                var strvalue = (int)strength;
                System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Device {id} updated strength {strength}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Device {id} updated ");
            }
        }


        private async Task StartDeviceReadAsync(UserPreferences.ReadSelection searchType)
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
            switch (searchType)
            {
                case UserPreferences.ReadSelection.Everything:
                    DeviceReadTask = FullReadAsync(DeviceReadCts.Token);
                    break;
                case UserPreferences.ReadSelection.Name:
                    DeviceReadTask = NameReadAsync(DeviceReadCts.Token);
                    break;
            }
        }



        private async Task FullReadAsync(CancellationToken ct)
        {
            CurrJsonSearch = "";
            await Task.Delay(1000); // pause 1000 ms = 1 s to get some BT
            int startidx = FindListStart() + 1;
            int endidx = FindListEnd() - 1;
            if (ct.IsCancellationRequested) return;
            for (int i=startidx; i<endidx; i++)
            {
                var nvi = uiNavigation.MenuItems[i] as NavigationViewItemBase;
                var di = nvi?.Tag as DeviceInformationWrapper;
                if (di != null)
                {
                    // Got a device to investigate.
                    var _page = typeof(BleEditor.BleEditorPage);
                    ContentFrame.Navigate(_page, di);
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
                        if (CurrJsonSearch != "")
                        {
                            CurrJsonSearch += ",\n";
                        }
                        CurrJsonSearch += json;
                    }
                }

                // Always re-get the end because it's always being updated.
                endidx = FindListEnd() - 1;
            }

            // Even on cancel show the text that we have.
            if (!String.IsNullOrEmpty(CurrJsonSearch))
            {
                // Write it out!
                var tb = new TextBlock()
                {
                    IsTextSelectionEnabled = true,
                    TextWrapping = TextWrapping.Wrap,
                    Text = CurrJsonSearch,
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
                    System.Diagnostics.Debug.WriteLine($"Exception: JsonSearch: {ex.Message}");
                }
                finally
                {
                    (App.Current as App).ReleaseAppLock("JsonSearch");
                }
            }
        }


        private async Task NameReadAsync(CancellationToken ct)
        {
            CurrJsonSearch = "";
            await Task.Delay(1000); // pause 1000 ms = 1 s to get some BT
            int startidx = FindListStart() + 1;
            int endidx = FindListEnd() - 1;
            if (ct.IsCancellationRequested) return;

            var commonConfigurationGuid = BluetoothLEStandardServices.Display; //  Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"); // service
            var deviceNameGuid = BluetoothLEStandardServices.Name;  // new Guid("00002a00-0000-1000-8000-00805f9b34fb"); // characteristic

            for (int i = startidx; i < endidx; i++)
            {
                var nvi = uiNavigation.MenuItems[i] as NavigationViewItemBase;
                var di = nvi?.Tag as DeviceInformationWrapper;
                if (di != null)
                {
                    // Just do a query for the name
                    try
                    {
                        var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
                        if (ble != null)
                        {
                            if (string.IsNullOrEmpty(ble.Name))
                            {
                                System.Diagnostics.Debug.WriteLine($"DEVICE: Get name for {di.di.Id}");
                                var services = await ble.GetGattServicesForUuidAsync(commonConfigurationGuid);
                                if (services.Status != GattCommunicationStatus.Success)
                                {
                                    continue;
                                }

                                foreach (var service in services.Services)
                                {
                                    var characteristics = await service.GetCharacteristicsForUuidAsync(deviceNameGuid);
                                    if (characteristics.Status != GattCommunicationStatus.Success)
                                    {
                                        continue;
                                    }
                                    foreach (var characteristic in characteristics.Characteristics)
                                    {
                                        var read = await characteristic.ReadValueAsync();
                                        if (read.Status != GattCommunicationStatus.Success)
                                        {
                                            continue;
                                        }
                                        var name = BluetoothLEStandardServices.CharacteristicData.ValueAsString(BluetoothLEStandardServices.DisplayType.String, read.Value);
                                        System.Diagnostics.Debug.WriteLine($"  --> read name as {name}");

                                        var entry = nvi.Content as DeviceMenuEntryControl;
                                        if (entry != null)
                                        {
                                            await this.Dispatcher.RunAsync(
                                                Windows.UI.Core.CoreDispatcherPriority.Normal, 
                                                () => { entry.UpdateName(name); });
                                        }

                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ERROR: unable to navigate {ex.Message}");
                        // I don't know of any exceptions. But if there are any, supress them completely.
                    }
                }

                // Always re-get the end because it's always being updated.
                endidx = FindListEnd() - 1;
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
        }


        private void OnAdRefreshed(object sender, RoutedEventArgs e)
        {
            var ad = sender as AdControl;
            System.Diagnostics.Debug.WriteLine($"NOTE: AdControl Refresh {ad.Name}");
            ;
        }

        private void OnAdErrorOccurred(object sender, Microsoft.Advertising.WinRT.UI.AdErrorEventArgs e)
        {
            var ad = sender as AdControl;
            System.Diagnostics.Debug.WriteLine($"NOTE: AdControl Error {ad.Name} Message:{e.ErrorMessage} ErrorCode:{e.ErrorCode}");
            ;
        }

        private async void OnRequestFeedback(object sender, TappedRoutedEventArgs e)
        {
            var launcher = Microsoft.Services.Store.Engagement.StoreServicesFeedbackLauncher.GetDefault();
            await launcher.LaunchAsync();
        }

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

    }
}
