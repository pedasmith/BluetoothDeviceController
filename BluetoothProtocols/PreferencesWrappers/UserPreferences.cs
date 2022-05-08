using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Has_specialized_display, 
            Bluetooth_Com_Device, 
            Bluetooth_Beacons,
            Device_is_named, 
            All_bluetooth_devices };
        public SearchScope Scope { get; set; } = SearchScope.Has_specialized_display;

        public enum ReadSelection { Address, Name, Everything } // Same as DeviceSearchType: Standard, NameRead, Everything
        public ReadSelection DeviceReadSelection { get; set; } = UserPreferences.ReadSelection.Name;
        public bool AutomaticallyReadData { get; set; } = true;
        /// <summary>
        /// When the user asks to scan for advertisements (beacons), the underlying WinRT API will scan forever. That's
        /// not what most users will want. Instead scan for a limited amount of time. As of 2022-04-16, this isn't settable
        /// by the user.
        /// </summary>
        public int AdvertisementScanTimeInMilliseconds { get; set; } = 10_000; // 10 seconds is a good amount of time
        // Was 5 seconds, but that felt too quick.

        public bool BeaconFullDetails { get; set; } = false;
        public bool BeaconTrackAll { get; set; } = false;
        public bool BeaconIgnoreApple { get; set; } = true;
        public double BeaconDbLevel { get; set; } = -80.0; // default min db level for beacon advert


        const string DisplayPreferenceSetting = "UserPreferenceDisplayPreference";
        const string SearchScopeSetting = "UserPreferenceSearchScope";
        const string ReadSelectionSetting = "UserPreferenceReadSelection";
        const string AutomaticallyReadDataSetting = "AutomaticallyReadData";

        const string BeaconFullDetailsSetting = "UserPreferenceBeaconFullDetails";
        const string BeaconTrackAllSetting = "UserPreferenceBeaconTrackAll";
        const string BeaconIgnoreAppleSetting = "UserPreferenceBeaconIgnoreApple";
        const string BeaconDbLevelSetting = "UserPreferenceBeaconDbLevel";

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
            if (localSettings.Values.ContainsKey(BeaconTrackAllSetting))
            {
                BeaconTrackAll = (bool)localSettings.Values[BeaconTrackAllSetting];
            }
            if (localSettings.Values.ContainsKey(BeaconIgnoreAppleSetting))
            {
                BeaconIgnoreApple = (bool)localSettings.Values[BeaconIgnoreAppleSetting];
            }
            if (localSettings.Values.ContainsKey(BeaconDbLevelSetting))
            {
                BeaconDbLevel = (double)localSettings.Values[BeaconDbLevelSetting];
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
            localSettings.Values[BeaconTrackAllSetting] = BeaconTrackAll;
            localSettings.Values[BeaconIgnoreAppleSetting] = BeaconIgnoreApple;
            localSettings.Values[BeaconDbLevelSetting] = BeaconDbLevel;
        }
    }
}
