using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;

namespace TestNmeaGpsParserWinUI
{
    public class UserPreferences
    {
        private bool DisableAll3rdPartyServices = true;
        private bool _AllowOpenStreetMap = true;
        public bool AllowOpenStreetMap
        {
            get
            {
                if (DisableAll3rdPartyServices) return false;
                return AllowOpenStreetMap;
            }
            set
            {
                if (DisableAll3rdPartyServices) return;
                _AllowOpenStreetMap = value;
            }
        }

        public string ToJson()
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static void Log (string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
        public void Save()
        {
            string _localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string _defaultApplicationDataFolder = "Options";
            string _defaultLocalSettingsFile = "SimpleGpsOptions.json";
            var _applicationDataFolder = Path.Combine(_localApplicationData, _defaultApplicationDataFolder);
            var _localsettingsFile = _defaultLocalSettingsFile;

            Log($"Json: appdata={_localApplicationData}");
            Log($"Json: folder={_applicationDataFolder}");
            Log($"Json: json={ToJson()}");

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Log($"Json: settings={localSettings}");

        }
    }
}
