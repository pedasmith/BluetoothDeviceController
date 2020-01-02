using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController
{
    public class UserPreferences
    {
        public enum DisplayPreference {
            Specialized_Display,
            Device_Editor };
        public DisplayPreference Display { get; set; } = DisplayPreference.Specialized_Display;
        public enum SearchScope {  Has_specialized_display, Bluetooth_Com_Device, Device_is_named, All_bluetooth_devices };
        public SearchScope Scope { get; set; } = SearchScope.Has_specialized_display;

        public enum ReadSelection { Address, Name, Everything } // Same as DeviceSearchType: Standard, NameRead, Everything
        public ReadSelection DeviceReadSelection { get; set; } = UserPreferences.ReadSelection.Name;


        const string DisplayPreferenceSetting = "UserPreferenceDisplayPreference";
        const string SearchScopeSetting = "UserPreferenceSearchScope";
        const string ReadSelectionSetting = "UserPreferenceReadSelection";
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
        }

        public void SaveToLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[DisplayPreferenceSetting] = (int)Display;
            localSettings.Values[SearchScopeSetting] = (int)Scope;
            localSettings.Values[ReadSelectionSetting] = (int)DeviceReadSelection;
        }
    }
}
