namespace BluetoothDeviceController
{
    public class UserPreferences
    {
        public static UserPreferences MainUserPreferences { get; set; } = null;
        public enum DisplayPreference {
            Specialized_Display,
            Device_Editor };
        public DisplayPreference Display { get; set; } = DisplayPreference.Specialized_Display;
        public enum SearchScope {
            [enumUtilities.Display("BLE known device types")]
            Ble_Has_specialized_display,
            [enumUtilities.Display("BLE advertisements and beacons")]
            Bluetooth_Beacons,
            [enumUtilities.Display("Bluetooth COM port")]
            Bluetooth_Com_Device, 
            [enumUtilities.Display("BLE devices with names")]
            Ble_Device_is_named, 
            [enumUtilities.Display("All BLE devices")]
            Ble_All_ble_devices };
        public SearchScope Scope { get; set; } = SearchScope.Ble_Has_specialized_display;

        public enum ReadSelection { Address, Name, Everything } // Same as DeviceSearchType: Standard, NameRead, Everything
        // Name is the typical; Everything is used by 
        public ReadSelection DeviceReadSelection { get; set; } = UserPreferences.ReadSelection.Name;
        public bool AutomaticallyReadData { get; set; } = true;
        /// <summary>
        /// When the user asks to scan for advertisements (beacons), the underlying WinRT API will scan forever. That's
        /// not what most users will want. Instead scan for a limited amount of time. As of 2022-04-16, this isn't settable
        /// by the user.
        /// </summary>
        public int AdvertisementScanTimeInMilliseconds { get; set; } = 120_000; // 30 seconds is a good amount of time
        // Was 5 seconds, but that felt too quick.

        public bool BeaconFullDetails { get; set; } = false;
        // BeaconTrackAll was used with the old UX to track all adverts or not. But now the advert page is done differently: all adverts are tracked and stored
        // and the advert page just filters based on context. It had also been used in the UX to show or not show the BT address, but now I just always show it.
        //public bool BeaconTrackAll { get; set; } = false;
        public bool BeaconIgnoreApple { get; set; } = true;
        public bool BeaconIgnoreMicrosoftRome { get; set; } = true; // TODO: Save/Restore and add to UX
        public double BeaconDbLevel { get; set; } = -80.0; // default min db level for beacon advert

        public enum SortBy { Mac, Time, Rss, };
        public enum SortDirection { Ascending, Descending };
        /// <summary>
        /// The setting from Menu / Filter / Sort AZ; is Ascending[D] or Descending 
        /// </summary>
        public SortDirection MenuFilterSortDirection { get; set; } = SortDirection.Ascending;


        const string DisplayPreferenceSetting = "UserPreferenceDisplayPreference";
        const string SearchScopeSetting = "UserPreferenceSearchScope";
        const string ReadSelectionSetting = "UserPreferenceReadSelection";
        const string AutomaticallyReadDataSetting = "AutomaticallyReadData";

        const string BeaconFullDetailsSetting = "UserPreferenceBeaconFullDetails";
        //const string BeaconTrackAllSetting = "UserPreferenceBeaconTrackAll";
        const string BeaconIgnoreAppleSetting = "UserPreferenceBeaconIgnoreApple";
        const string BeaconIgnoreMicrosoftRomeSetting = "UserPreferenceBeaconIgnoreMicrosoftRome";
        const string BeaconDbLevelSetting = "UserPreferenceBeaconDbLevel";

        const string MenuFilterSortDirectionSetting = "MenuFilterSortDirection";

        /// <summary>
        /// Read from the LocalSettings saved settings into this object. This should always be in parallel to the Save... 
        /// </summary>
        public void ReadFromLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(DisplayPreferenceSetting))
            {
                Display = (DisplayPreference)(int)localSettings.Values[DisplayPreferenceSetting];
            }
            if (localSettings.Values.ContainsKey(SearchScopeSetting))
            {
                Scope = (SearchScope)(int)localSettings.Values[SearchScopeSetting];
            }
            if (localSettings.Values.ContainsKey(ReadSelectionSetting))
            {
                DeviceReadSelection = (ReadSelection)(int)localSettings.Values[ReadSelectionSetting];
            }
            if (localSettings.Values.ContainsKey(AutomaticallyReadDataSetting))
            {
                AutomaticallyReadData = (bool)localSettings.Values[AutomaticallyReadDataSetting];
            }

            if (localSettings.Values.ContainsKey(BeaconFullDetailsSetting))
            {
                BeaconFullDetails = (bool)localSettings.Values[BeaconFullDetailsSetting];
            }
            //if (localSettings.Values.ContainsKey(BeaconTrackAllSetting))
            //{
            //    BeaconTrackAll = (bool)localSettings.Values[BeaconTrackAllSetting];
            //}
            if (localSettings.Values.ContainsKey(BeaconIgnoreAppleSetting))
            {
                BeaconIgnoreApple = (bool)localSettings.Values[BeaconIgnoreAppleSetting];
            }
            if (localSettings.Values.ContainsKey(BeaconIgnoreMicrosoftRomeSetting))
            {
                BeaconIgnoreMicrosoftRome = (bool)localSettings.Values[BeaconIgnoreMicrosoftRomeSetting];
            }
            if (localSettings.Values.ContainsKey(BeaconDbLevelSetting))
            {
                BeaconDbLevel = (double)localSettings.Values[BeaconDbLevelSetting];
            }
            if (localSettings.Values.ContainsKey(MenuFilterSortDirectionSetting))
            {
                MenuFilterSortDirection = (UserPreferences.SortDirection)localSettings.Values[MenuFilterSortDirectionSetting];
            }
        }

        public void SaveToLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[DisplayPreferenceSetting] = (int)Display;
            localSettings.Values[SearchScopeSetting] = (int)Scope;
            localSettings.Values[ReadSelectionSetting] = (int)DeviceReadSelection;
            localSettings.Values[AutomaticallyReadDataSetting] = AutomaticallyReadData;

            localSettings.Values[BeaconFullDetailsSetting] = BeaconFullDetails;
            //localSettings.Values[BeaconTrackAllSetting] = BeaconTrackAll;
            localSettings.Values[BeaconIgnoreAppleSetting] = BeaconIgnoreApple;
            localSettings.Values[BeaconIgnoreMicrosoftRomeSetting] = BeaconIgnoreMicrosoftRome;
            localSettings.Values[BeaconDbLevelSetting] = BeaconDbLevel;

            localSettings.Values[MenuFilterSortDirectionSetting] = (int)MenuFilterSortDirection;
        }
    }
}
