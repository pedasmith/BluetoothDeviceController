using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Foundation.Diagnostics;
using WinUI3Controls;

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace TestNmeaGpsParserWinUI
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(UserPreferences))]
    internal partial class SourceGenerationContext : JsonSerializerContext
    {
    }


    public class UserPreferences
    {
        public UserMapPrivacyPreferences UserMapPrivacyPreferences { get; set; } = new UserMapPrivacyPreferences();

        public string ToJson()
        {
            // Old code used NewtonSoft, but that's no longer a viable technology thanks to the Trim options
            // var json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            var json = System.Text.Json.JsonSerializer.Serialize(this, SourceGenerationContext.Default.UserPreferences);
            return json;
        }

        public static TextBlock LogTextBlock { get; set; } = null;
        public static TextBlock LogTextBlock2 { get; set; } = null;
        public static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            if (LogTextBlock != null)
            {
                LogTextBlock.Text += str + Environment.NewLine;
            }
        }

        const string PrivacySettings = "PrivacySettingsV26_1526";

        public void Restore()
        {
            Log($"Json: json initial={ToJson()}");

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Log($"Json: settings name={localSettings.Name}");
            var settings = localSettings.Values[PrivacySettings] as string;
            UserPreferences saved = null;
            if (!string.IsNullOrEmpty(settings))
            {
                try
                {
                    saved = System.Text.Json.JsonSerializer.Deserialize<UserPreferences>(settings, SourceGenerationContext.Default.UserPreferences);
                }
                catch (Exception)
                {
                    saved = null;
                }
            }


            if (saved == null)
            {
                // Set the defaults. Default is that all third parties are blocked (user must explicitly
                // enable them because some people really need privacy), but once they click the general case,
                // in general all third party services are allowed.
                this.UserMapPrivacyPreferences.UserHasPickedPrivacySettings = false;
                this.UserMapPrivacyPreferences.Allow3rdPartyServices = false;
                this.UserMapPrivacyPreferences.AllowOpenStreetMapUnderlyingValue = true;
            }
            else
            {
                this.UserMapPrivacyPreferences.UserHasPickedPrivacySettings = saved.UserMapPrivacyPreferences.UserHasPickedPrivacySettings;
                this.UserMapPrivacyPreferences.Allow3rdPartyServices = saved.UserMapPrivacyPreferences.Allow3rdPartyServices;
                this.UserMapPrivacyPreferences.AllowOpenStreetMapUnderlyingValue = saved.UserMapPrivacyPreferences.AllowOpenStreetMapUnderlyingValue;
            }
        }
        public void Save()
        {
            var json = ToJson();
            Log($"Json: Save json={json}");

            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            Log($"Json: settings name={localSettings.Name}");
            localSettings.Values[PrivacySettings] = json;
        }
    }
}
