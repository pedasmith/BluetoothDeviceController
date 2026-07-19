using BluetoothWatcher.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public class UserPreferences
    {
        public Distance.DistanceUnit Distance { get; set; } = BluetoothWatcher.Units.Distance.DistanceUnit.Kilometers;
        public Pressure.PressureUnit Pressure { get; set; } = BluetoothWatcher.Units.Pressure.PressureUnit.hectoPascal_milliBar;
        public Temperature.TemperatureUnit Temperature { get; set; } = BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius;
        public bool AutostartAdvertisementWatcher { get; set; } = true;

        public static UserPreferences Restore()
        {
            string folderPath = GetSaveDirectoryAsString();
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "UserPreferences.preferences");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                try
                {
                    var retval = (UserPreferences)System.Text.Json.JsonSerializer.Deserialize(json, typeof(UserPreferences), UserPreferencesContext.Default);
                    return retval;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Unable to parse JSON file {filePath}. Message:{ex.Message}");
                }
            }
            return new UserPreferences();// return a default value.
        }

        /// <summary>
        /// Saves the UserPreferences into a .preferences JSON file. This will be tucked into the Documents/BluetoothDevices 
        /// folder in a file called UserPreferences.devices. It's restored with a call to Restore() which is done automatically
        /// in MainWindow
        /// </summary>
        public void Save()
        {
            string folderPath = GetSaveDirectoryAsString();
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "UserPreferences.preferences");

            var json = System.Text.Json.JsonSerializer.Serialize(this, typeof(UserPreferences), UserPreferencesContext.Default);
            File.WriteAllText(filePath, json);
        }

        public UserPreferences Clone()
        {
            return this.MemberwiseClone() as UserPreferences;
        }


        public static string GetSaveDirectoryAsString()
        {
            var retval = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
            return retval;
        }
    }

}
