using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.Mime.MediaTypeNames;


#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public static class AllSaveData
    {
        public static List<SaveData> AllDevices = new List<SaveData>();
#if NEVER_EVER_DEFINED
        // Only needed to help kick-start the save/load code. Can be deleted later on.
        {
            new SaveData()
            {
                Id = new DeviceIdentification()
                {
                    AdvertisementAddress = 123456789,
                    ConnectionAddress = 987654321,
                    AdvertisementName = "ThingyAdv",
                    ConnectName = "ThingyConn",
                },
            },
        };
#endif
        public static void Save()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "AllDeviceData.json");

            var json = System.Text.Json.JsonSerializer.Serialize(AllDevices, typeof(List<SaveData>), SaveDataContext.Default);  //<List<SaveData>>((AllDevices, options);
            File.WriteAllText(filePath, json);
        }

        public static void Restore()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "AllDeviceData.json");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var list = (List<SaveData>)System.Text.Json.JsonSerializer.Deserialize(json, typeof(List<SaveData>), SaveDataContext.Default);
                AllDevices = list ?? new List<SaveData>();
            }
        }
    }




    public class SaveData
    {
        public DeviceIdentification Id { get; set; } = new DeviceIdentification();
        public DeviceHistory History = new DeviceHistory();
        public KnownDeviceUserPreferences Preferences = new KnownDeviceUserPreferences();
    }

    public class DeviceIdentification
    {
        public ulong AdvertisementAddress { get; set; }
        public ulong ConnectionAddress { get; set; }
        public string AdvertisementName { get; set; }
        public string ConnectName { get; set; }
 
    }

    public class KnownDeviceUserPreferences
    {
#if NEVER_EVER_DEFINED
        public string UserName { get; set; } // will depend on the DeviceIdentification, but could be set by the user to something more memorable
        public enum DisplayHighlight { Normal, Highlight, Dim };
        DisplayHighlight HighlightPreference { get; set; } = DisplayHighlight.Normal;
#endif

    }
    public class DeviceHistory
    {
        public DateTimeOffset FirstAdvertisement { get; set; }
        public DateTimeOffset FirstConnection { get; set;  }
        public DateTimeOffset FirstData { get; set; }
        public DateTimeOffset MostRecentAdvertisement { get; set; }
        public DateTimeOffset MostRecentConnection { get; set; }
        public DateTimeOffset MostRecentData { get; set; }
    }


    // JsonSerializableAttribute

    // See https://sunriseprogrammer.blogspot.com/2026/04/il2104-il2026-trim-and-json-with-winui3.html
    // See https://stackoverflow.com/questions/70825664/how-to-implement-system-text-json-source-generator-with-a-generic-class
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(List<SaveData>), TypeInfoPropertyName = "ListSaveDataWithPropertyName")]
    [JsonSerializable(typeof(SaveData), TypeInfoPropertyName = "SaveDataWithPropertyName")]
    [JsonSerializable(typeof(DeviceHistory), TypeInfoPropertyName = "DeviceHistoryWithPropertyName")]
    [JsonSerializable(typeof(KnownDeviceUserPreferences), TypeInfoPropertyName = "KnownDeviceUserPreferencesWithPropertyName")]
    [JsonSerializable(typeof(DeviceIdentification), TypeInfoPropertyName = "DeviceIdentificationWithPropertyName")]
    //[JsonSerializable(typeof(DateTimeOffset), TypeInfoPropertyName = "DateTimeOffsetWithPropertyName")]
    //[JsonSerializable(typeof(ulong), TypeInfoPropertyName = "ulongWithPropertyName")]
    //[JsonSerializable(typeof(string), TypeInfoPropertyName = "stringWithPropertyName")]
    internal partial class SaveDataContext : JsonSerializerContext
    {

    }
}
