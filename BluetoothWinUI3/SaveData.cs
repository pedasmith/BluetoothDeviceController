using BluetoothWinUI3.BluetoothWinUI3Registration;
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
        /// <summary>
        /// Saves the AllDevices into a .devices JSON file. This will be tucked into the Documents/BluetoothDevices 
        /// folder in a file called AllDeviceData.devices. It's restored with a call to Restore() which is done automatically
        /// in App.xaml.cs
        /// </summary>
        public static void Save()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "AllDeviceData.devices");

            var json = System.Text.Json.JsonSerializer.Serialize(AllDevices, typeof(List<SaveData>), SaveDataContext.Default);  //<List<SaveData>>((AllDevices, options);
            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Restores the AllDevices from the AllDevices.devices file. Is called from the App.xaml.cs file.
        /// </summary>
        public static void Restore()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "AllDeviceData.devices");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var list = (List<SaveData>)System.Text.Json.JsonSerializer.Deserialize(json, typeof(List<SaveData>), SaveDataContext.Default);
                AllDevices = list ?? new List<SaveData>();
            }
        }

        /// <summary>
        /// The ID is the BluetoothLEDevice.DeviceId and is assigned by Windows.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SaveData FindWithId(string id)
        {
            foreach (var device in AllDevices)
            {
                if (device.MatchesDeviceId(id))
                {
                    return device;
                }
            }

            return null;
        }

        public static void Insert(SaveData data)
        {
            AllDevices.Add(data);
            return;
        }
    }




    public class SaveData
    {
        public SaveData(KnownDevice knownDevice)
        {
            Id.AdvertisementAddress = knownDevice.Advertisement?.Addr ?? 0;
            Id.AdvertisementName = knownDevice.Advertisement?.CompleteLocalName ?? "";
            Id.ConnectAddress = knownDevice.BTLEDevice?.BluetoothAddress ?? 0;
            Id.ConnectName = knownDevice.BTLEDevice?.Name ?? "";
            Id.DeviceId = knownDevice.BTLEDevice?.DeviceId ?? "";
        }

        /// <summary>
        /// Must have a default constructor or the Json reload doesn't work
        /// </summary>
        public SaveData()
        {
            ; 
        }
        public bool MatchesDeviceId(string id)
        {
            if (id == "") return false;
            if (Id == null) return false;
            if (Id.DeviceId == id) return true;
            return false;
        }

        /// <summary>
        /// Returns the best name: prefers the user-set name, otherwise the device name or the advertisement name.
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            string retval = Preferences.UserName;
            if (retval == "") retval = Id.ConnectName;
            if (retval == "") retval = Id.AdvertisementName;
            return retval;
        }

        public void SetUserName(string name)
        {
            Preferences.UserName = name;
        }
        public DeviceIdentification Id { get; set; } = new DeviceIdentification();
        public DeviceHistory History { get; set; } = new DeviceHistory();
        public KnownDeviceUserPreferences Preferences { get; set; } = new KnownDeviceUserPreferences();
    }

    public class DeviceIdentification
    {
        public ulong AdvertisementAddress { get; set; }
        public ulong ConnectAddress { get; set; }
        /// <summary>
        /// The name (like "Thingy") taken from the Bluetooth LE Advertisements. 
        /// </summary>
        public string AdvertisementName { get; set; }
        /// <summary>
        /// The Name (like "Thingy") taken from the LE device after it's connected. This is often
        /// the same as the AdvertisementName, but for some (incorrect) devices its not.
        /// </summary>
        public string ConnectName { get; set; }
        /// <summary>
        /// From BluetoothLDDevice.DeviceId; is set in the BT_Control.xaml.cs when the DataContext
        /// is set and the BluetoothLEDevice is created.
        /// This is a long-ass string that is only barely human readable (unless you are a real expert)
        /// </summary>
        public string DeviceId { get; set; }
    }

    public class KnownDeviceUserPreferences
    {
        public string UserName { get; set; } = ""; // will depend on the DeviceIdentification, but could be set by the user to something more memorable
//#if NEVER_EVER_DEFINED
        public enum DisplayHighlight { Normal, Highlight, Dim };
        DisplayHighlight HighlightPreference { get; set; } = DisplayHighlight.Normal;
//#endif

    }

    // TODO: fill all this in!
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
