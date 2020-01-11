using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController
{
    public class UserSerialPortPreferences
    {
        public enum TerminalLineEnd { None, CR, LF, CRLF };
        public TerminalLineEnd LineEnd { get; set; } = TerminalLineEnd.CRLF; // Needed for Slant LittleBot
        public string ShortcutId { get; set; } = "";

        const string LineEndPreferenceSetting = "LineEndDisplayPreference";
        const string ShortcutIdPreferenceSetting = "ShortcutIdDisplayPreference";
        public void ReadFromLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey(LineEndPreferenceSetting))
            {
                LineEnd = (TerminalLineEnd)(int)localSettings.Values[LineEndPreferenceSetting];
            }
            if (localSettings.Values.ContainsKey(ShortcutIdPreferenceSetting))
            {
                ShortcutId = (string)localSettings.Values[ShortcutIdPreferenceSetting];
            }
        }

        public void SaveToLocalSettings()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[LineEndPreferenceSetting] = (int)LineEnd;
            localSettings.Values[ShortcutIdPreferenceSetting] = ShortcutId;
        }
    }
}
