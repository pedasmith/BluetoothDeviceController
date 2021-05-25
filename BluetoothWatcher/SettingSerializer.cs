using BluetoothWatcher.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace BluetoothWatcher
{
    /// <summary>
    /// Singleton (static) class to handle all settings serialization
    /// </summary>
    public static class SettingSerializer
    {
        static string NAME_PROPERTY = "NAME";
        static string TEMPERATURE_PROPERTY = "TEMP";
        static string PRESSURE_PROPERTY = "PRESS";

        enum WhenNoSettings {  ReturnNull, MakeNew};

        private static ApplicationDataCompositeValue GetSettingsForMacAddress(string macAddress, WhenNoSettings makeIfNeeded)
        {
            var macSetting = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (!macSetting.Values.ContainsKey(macAddress))
            {
                if (makeIfNeeded == WhenNoSettings.ReturnNull) return null;
                macSetting.Values.Add(macAddress, new ApplicationDataCompositeValue());
            }
            var setting = macSetting.Values[macAddress] as ApplicationDataCompositeValue;
            return setting;
        }

        private static void SaveSettingsForMacAddress(string macAddress, ApplicationDataCompositeValue values)
        {
            var macSetting = Windows.Storage.ApplicationData.Current.RoamingSettings;
            if (!macSetting.Values.ContainsKey(macAddress))
            {
                macSetting.Values.Add(macAddress, values);
            }
            else
            {
                macSetting.Values.Remove(macAddress);
                macSetting.Values.Add(macAddress, values);
            }
            return;
        }


        public static void SaveName(string macAddress, string name)
        {
            var setting = GetSettingsForMacAddress(macAddress, WhenNoSettings.MakeNew);
            if (setting.ContainsKey (NAME_PROPERTY))
            {
                setting.Remove(NAME_PROPERTY);
            }
            setting.Add(NAME_PROPERTY, name);
            SaveSettingsForMacAddress(macAddress, setting);
        }

        public static string GetName(string macAddress)
        {
            var setting = GetSettingsForMacAddress(macAddress, WhenNoSettings.ReturnNull);
            if (setting == null || !setting.ContainsKey (NAME_PROPERTY))
            {
                return null;
            }
            var retval = setting[NAME_PROPERTY] as string;
            return retval;
        }

        private static void SetProperty (ApplicationDataCompositeValue setting, string settingName, int value)
        {
            if (setting.ContainsKey(settingName))
            {
                setting.Remove(settingName);
            }
            setting.Add(settingName, value);
        }

        public static void SaveUnits(string macAddress, UserUnits units)
        {
            var setting = GetSettingsForMacAddress(macAddress, WhenNoSettings.MakeNew);
            SetProperty(setting, TEMPERATURE_PROPERTY, (int)units.Temperature);
            SetProperty(setting, PRESSURE_PROPERTY, (int)units.Pressure);

            SaveSettingsForMacAddress(macAddress, setting);
        }

        public static void UpdateUnits(string macAddress, UserUnits units)
        {
            var setting = GetSettingsForMacAddress(macAddress, WhenNoSettings.ReturnNull);
            if (setting == null)
            {
                return;
            }
            if (setting.ContainsKey(TEMPERATURE_PROPERTY))
            {
                units.Temperature = (Temperature.Unit)setting[TEMPERATURE_PROPERTY];
            }
            if (setting.ContainsKey(PRESSURE_PROPERTY))
            {
                units.Pressure = (Pressure.Unit)setting[PRESSURE_PROPERTY];
            }
        }
    }
}
