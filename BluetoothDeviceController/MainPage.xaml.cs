using BluetoothDeviceController.Names;
using BluetoothDeviceController.UserData;
using Microsoft.Advertising.WinRT.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothDeviceController
{
    public enum DeviceSearchType {  Standard, FullRead }
    public interface IDoSearch
    {
        bool GetSearchActive();
        void StartSearch(DeviceSearchType searchType);
        void CancelSearch();
        event EventHandler DeviceEnumerationChanged;
        string GetCurrentSearchResults();
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IDoSearch, IDockParent, SpecialtyPages.IHandleStatus
    {
        const string ALARM = "⏰";
        const string LIGHT = "💡";
        const string WATER = "🚰";
        const string DATA = "📈"; //"🥼";
        const string WAND = "🖉"; // yeah, a pencil isn't reall a wand.
        
        /// <summary>
        /// Converts a list of possible Bluetooth names into a page to display
        /// </summary>
        List<Specialization> Specializations = new List<Specialization>()
        {
            // Fun specializations
            new Specialization (typeof(SpecialtyPages.Kano_WandPage), new string[] { "Kano-Wand" }, WAND, "Kano Wand", "Kano coding kit Harry Potter Wand"),

            // Lights
            new Specialization (typeof(SpecialtyPages.TI_beLight_2540Page), new string[] { "beLight" }, LIGHT, "TI Light", "TI CC2540 Bluetooth kit"),
            new Specialization (typeof(SpecialtyPages.Witti_DottiPage), new string[] { "Dotti" }, LIGHT, "DOTTI", "Witti Designs DOTTI device"),
            new Specialization (typeof(SpecialtyPages.Witti_NottiPage), new string[] { "Notti" }, ALARM + LIGHT, "NOTTI", "Witti Designs NOTTI device"),
            new Specialization (typeof(SpecialtyPages.MIPOW_Playbulb_BTL201Page), new string[] { "PLAYBULB" }, LIGHT, "MIPOW Smart LED", "MIPOW PLAYBULB Smart LED Bulb BTL-201"),
            new Specialization (typeof(SpecialtyPages.TrionesPage), new string[] { "Triones", "LEDBlue" }, LIGHT, "LED Light", "Bluetooth controlled light (triones protocol)"),

            // // // Is actually empty :-) new Specialization (typeof(SpecialtyPages.DigHoseEndPage), new string[] { "DIG", "\0IG" }, WATER, "DIG water valve", "DIG Hose-end valve"),

            // Data Sensors
            new Specialization (typeof(SpecialtyPages.Bbc_MicroBitPage), new string[] { "BBC micro:bit" }, DATA, "BBC micro:bit", "micro:bit with data program"),

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

        };

        public BleNames AllBleNames { get; set; }
        public UserPreferences Preferences { get; } = new UserPreferences();

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
            ContentFrame.Navigated += ContentFrame_Navigated;

            // It might look like AllBleNames is never used. In reality, this initializes a static class.
            AllBleNames = new BleNames();
            await AllBleNames.InitAsync();
            await UserNameMappings.InitAsync();
            StartSearch(DeviceSearchType.Standard);
            NavView_Navigate("Help", "welcome.md", null);
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
        }

        private void UiNavigation_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {

        }

        private async void UiNavigation_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var di = args.InvokedItemContainer.Tag as DeviceInformation;
            if (di != null)
            {
                NavView_Navigate(di, args.RecommendedNavigationTransitionInfo);
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


        /// <summary>
        /// Primary call to show a page for a specific bluetooth device.
        /// </summary>
        /// <param name="di"></param>
        /// <param name="transitionInfo"></param>
        private void NavView_Navigate(DeviceInformation di, NavigationTransitionInfo transitionInfo)
        {
            Type _pageType = null;
            var preftype = Preferences.Display;
            if (preftype == UserPreferences.DisplayPreference.Specialized_Display)
            {
                var deviceName = GetDeviceInformationName(di);

                var specialized = Specialization.Get(Specializations, deviceName);
                if (specialized != null)
                {
                    _pageType = specialized.Page;
                }
            }
            if (_pageType == null) // Either the preferences is for an editor, or this particular BT devices doesn't have a specialization.
            {
                _pageType = typeof(BleEditor.BleEditorPage);
                // There's also a device information page, but it's not really useful at all.
            }

            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack. Except that I use the same page type for e.g. different Thingy devices.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            // BUT note that DeviceInformation is always the same type DeviceInformationPage
            //if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                // Not super pleased with this; there's a strong assumption that the bluetooth device id will match the deviceinfo id.
                // That's not actually required.
                if (!NewPageIsSameIdAsCurrentPage(di?.Id ?? null))
                {
                    MinimizeCurrentWindowToDock();
                }
                ContentFrame.Navigate(_pageType, di, transitionInfo);
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
            MinimizeCurrentWindowToDock();
        }

        private void MinimizeCurrentWindowToDock()
        {
            var page = ContentFrame.Content as Page;
            if (page == null) return; // should never happen
            ContentFrame.Content = null; // remove the old page
            var dock = uiDock;
            dock.AddPage(page); // the dock is smart enough to ignore pages that can't be docked.

            var handleStatus = page as SpecialtyPages.ISetHandleStatus;
            handleStatus?.SetHandleStatus(null); // stop handling the status.
        }


        /// <summary>
        /// Called by the PageDock when a page is ready to be undocked
        /// </summary>
        /// <param name="page"></param>
        public void DisplayFromDock(Page page)
        {
            MinimizeCurrentWindowToDock();
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

        private int FindDevice (DeviceInformation di)
        {
            int idx = -1;
            for (int i = 0; i < uiNavigation.MenuItems.Count && idx == -1; i++)
            {
                var nvi = uiNavigation.MenuItems[i] as NavigationViewItemBase;
                var dvi = nvi?.Tag as DeviceInformation;
                if (dvi != null && dvi.Id == di.Id)
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

        private void AddDevice(DeviceInformation di)
        {
            var includeAll = Preferences.Scope == UserPreferences.SearchScope.All_bluetooth_devices;
            var name = GetDeviceInformationName(di, includeAll); // gets null for unnamed devices unless we want all devices.
            if (name == null) return;

            var idx = FindDevice(di);
            if (idx != -1)
            {
                // Replace the old tag device information with this new one
                var nvib = uiNavigation.MenuItems[idx] as NavigationViewItemBase;
                nvib.Tag = di;
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
            if (specialization == null && Preferences.Scope == UserPreferences.SearchScope.Has_specialized_display)
            {
                // There's no specialization and the user asked for items with a specialization only.
                // That means we shouldn't add this to the list.
                return;
            }


            var dmec = new DeviceMenuEntryControl(di, name, specialization);
            dmec.SettingsClick += OnDeviceSettingsClick;
            var menu = new NavigationViewItem()
            {
                Content = dmec,
                Tag = di,
            };
            menu.HorizontalAlignment = HorizontalAlignment.Stretch;
            uiNavigation.MenuItems.Insert(idx, menu);
        }

        /// <summary>
        /// Display a pop-up for the ⚙ per-device setting
        /// </summary>
        private async void OnDeviceSettingsClick(object source, DeviceInformation di)
        {
            if (di == null) return; // The tag is always a DeviceInformation
            var name = GetDeviceInformationName(di);

            ; // throw new NotImplementedException();
            var oldName = GetDeviceInformationName(di, true);
            var perdevice = new PerDeviceSettings(di, oldName);

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
                await UserNameMappings.AddAsync(di.Id, perdevice.UserName);
                // Update the UI????
            }
        }

        /// <summary>
        /// All of the device watcher methods
        /// 
        /// </summary>
        /// 
        public event EventHandler DeviceEnumerationChanged;
        public void StartSearch(DeviceSearchType searchType)
        {
            ClearDevices();
            StartWatch();
            if (searchType == DeviceSearchType.FullRead)
            {
                var task = StartFullReadAsync(); // will set the FullReadTask global as needed.
            }
        }


        public void CancelSearch()
        {
            StopWatch();
        }

        public bool GetSearchActive() 
        {
            return MenuDeviceWatcher != null;
        }
        DeviceWatcher MenuDeviceWatcher = null;
        Task FullReadTask = null;
        CancellationTokenSource FullReadCts = null;

        private void StopWatch()
        {
            MenuDeviceWatcher?.Stop();
            MenuDeviceWatcher = null;
        }

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
            var qstr = "System.Devices.Aep.ProtocolId:=\"{BB7BB05E-5972-42B5-94FC-76EAA7084D49}\"";
            MenuDeviceWatcher = DeviceInformation.CreateWatcher(
                qstr,
                requestedProperties,
                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            MenuDeviceWatcher.Added += DeviceWatcher_Added;
            MenuDeviceWatcher.Updated += DeviceWatcher_Updated;
            MenuDeviceWatcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            MenuDeviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            MenuDeviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start the watcher.
            MenuDeviceWatcher.Start();
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
        }


        private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Stopped");
            MenuDeviceWatcher = null;
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
        }

        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Enumeration Completed");
            MenuDeviceWatcher = null;
            DeviceEnumerationChanged?.Invoke(this, new EventArgs());
        }



        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            object result;
            bool got = args.Properties.TryGetValue("System.Devices.Aep.IsPresent", out result);
            if (got)
            {
                var b = result as Boolean?;
                if(b.HasValue)
                {
                    if (b.Value)
                    {

                    }
                    else
                    {
                        ;
                    }
                }
                else
                {
                    ;
                }
            }
            else
            {
                ;
            }

            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Device Added");
            await uiNavigation.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, 
                () => { AddDevice(args); });
        }
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Device Removed");
        }
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            System.Diagnostics.Debug.WriteLine($"DeviceWatcher: Device updated");
        }


        private async Task StartFullReadAsync()
        {
            // Step one: if we're already running, kill the old task
            if (FullReadTask != null)
            {
                // Must cancel
                if (FullReadCts != null)
                {
                    FullReadCts?.Cancel();
                    while (!FullReadTask.IsCompleted)
                    {
                        await Task.Delay(50);
                    }
                    FullReadTask = null;
                }
            }

            // Now go for realsie
            FullReadCts = new CancellationTokenSource();
            var task = FullReadAsync(FullReadCts.Token);
            FullReadTask = task;
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
                var dvi = nvi?.Tag as DeviceInformation;
                if (dvi != null)
                {
                    // Got a device to investigate.
                    var _page = typeof(BleEditor.BleEditorPage);
                    ContentFrame.Navigate(_page, dvi);
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
