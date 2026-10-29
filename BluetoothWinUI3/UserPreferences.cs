using BluetoothWatcher.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public class UserPreferences
    {
        public static UserPreferences Global = null;
        public Distance.DistanceUnit Distance { get; set; } = BluetoothWatcher.Units.Distance.DistanceUnit.Kilometers;
        public Pressure.PressureUnit Pressure { get; set; } = BluetoothWatcher.Units.Pressure.PressureUnit.hectoPascal_milliBar;
        public Temperature.TemperatureUnit Temperature { get; set; } = BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius;

        public enum SoundLevelType { NotSet, Off, Quiet, On };
        public SoundLevelType BeepLevel { get; set; } = SoundLevelType.On;
        public SoundLevelType VoiceLevel { get; set; } = SoundLevelType.On;

        /// <summary>
        /// Returns the BeepLevel adjusted. One level is from the per-device preferences, and the other is from here
        /// </summary>
        public static SoundLevelType SoundLevelAdjusted(SoundLevelType userPreferenceSoundLevel, SoundLevelType saveDataSoundLevel)
        {
            if (saveDataSoundLevel != SoundLevelType.NotSet) return saveDataSoundLevel;
            if (userPreferenceSoundLevel != SoundLevelType.NotSet) return userPreferenceSoundLevel;
            return SoundLevelType.On; // Can never get here; the UserPreference sound levels are always set!
        }

        public bool AutostartAdvertisementWatcher { get; set; } = true;

        /// <summary>
        /// Reads in UserPreferences.preferences, a JSON file, into a new UserPreferences object.
        /// The file is in the Documents directory in folder "BluetoothDevices"
        /// </summary>
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
                    var retval = (UserPreferences)System.Text.Json.JsonSerializer.Deserialize(json, typeof(UserPreferences), JsonOptions);
                    return retval;
                }
                catch (Exception ex)
                {
                    Log($"Error: Unable to parse JSON file {filePath}. Message:{ex.Message}");
                }
            }
            return new UserPreferences();// return a default value.
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
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

            var json = System.Text.Json.JsonSerializer.Serialize(this, typeof(UserPreferences), JsonOptions);
            File.WriteAllText(filePath, json);
        }

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            Converters = { new JsonStringEnumConverter() },
        };

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
