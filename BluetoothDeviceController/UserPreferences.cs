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
        public enum SearchScope {  Has_specialized_display, Device_is_named, All_bluetooth_devices };
        public SearchScope Scope { get; set; } = SearchScope.Has_specialized_display;
    }
}
