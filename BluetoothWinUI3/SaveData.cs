using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Devices.Bluetooth;
using Windows.UI;


#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public static class AllSaveData
    {
        private static List<SaveData> AllDevices = new List<SaveData>();

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

            Log($"Saved configuration to {filePath}");
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        /// <summary>
        /// Restores the AllDevices from the AllDevices.devices file. Is called from the App.xaml.cs file.
        /// </summary>
        public static void Restore()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "AllDeviceData.devices");
            Log($"Reading configuration from {filePath}");

            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                try
                {
                    var list = (List<SaveData>)System.Text.Json.JsonSerializer.Deserialize(json, typeof(List<SaveData>), SaveDataContext.Default);
                    AllDevices = list ?? new List<SaveData>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: Unable to parse JSON file {filePath}. Message:{ex.Message}");
                }
            }
            Log($"Completed reading configuration from {filePath}");
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

        /// <summary>
        /// The advertisement address is from the device advertisement
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SaveData FindWithAdvertisementAddress(ulong address)
        {
            foreach (var device in AllDevices)
            {
                if (device.Id.AdvertisementAddress == address)
                {
                    return device;
                }
            }

            return null;
        }

        /// <summary>
        /// The ID is the BluetoothLEDevice.DeviceId and is assigned by Windows. The device
        /// id is the primary ID and the address is the fallback.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SaveData FindWithIdOrAdvertisementAddress(string id, ulong address)
        {
            SaveData foundViaAdvertisement = null;
            foreach (var device in AllDevices)
            {
                if (device.MatchesDeviceId(id))
                {
                    return device;
                }
                if (device.Id.AdvertisementAddress == address)
                {
                    foundViaAdvertisement = device;
                }
            }

            return foundViaAdvertisement;
        }

        public static void Insert(SaveData data)
        {
            AllDevices.Add(data);
            return;
        }

        public static SaveData GetOrCreateSaveData(KnownDevice knownDevice)
        {
            var saveData = AllSaveData.FindWithId(knownDevice.Id);
            if (saveData == null && knownDevice.Advertisement != null)
            {
                // TODO: this is actually hard. When the known device is created, it's
                // not yet associated with an advertisement. This is always the case with
                // the services and characteristics control which is created by the 
                // user, not with an advertisement.
                saveData = AllSaveData.FindWithAdvertisementAddress(knownDevice.Advertisement.Addr);
            }

            if (saveData == null)
            {
                // Must create a new SaveData.
                saveData = new SaveData(knownDevice);
                AllSaveData.Insert(saveData);
                AllSaveData.Save(); // quick update
            }
            return saveData;
        }

        /// <summary>
        /// When we first connect, there's a SaveData based on a BT advertisement address (one will have 
        /// been created already as needed). But now that there's a connection, we should switch to the 
        /// more correct DeviceId based on Device.ble.DeviceId.
        /// 
        /// Note that a device that changes the BT advertisement address (for privacy) will have a new,
        /// blank CurrSaveData to begin with. The DeviceId is the stable address.
        /// </summary>
        public static SaveData SwitchToDeviceIdCurrSaveData(SaveData currSaveData, KnownDevice knownDevice)
        {
            var newSaveData = AllSaveData.FindWithId(knownDevice.Id); // Use the stable form of the device id.

            if (newSaveData == null && currSaveData == null)
            {
                // Something catastrophic happened. Won't really be able to save anything,
                // but at least we not crash while "working"
                currSaveData = new SaveData(knownDevice);
                return currSaveData;
            }

            if (newSaveData == currSaveData)
            {
                // If it's the same as the advertisement-based, we're all good. The device has (for now)
                // a stable advertisement address.
                return currSaveData;
            }

            if (newSaveData == null)
            {
                // The device ID doesn't exist, but the advertisement one does.
                // We will keep on using it and update it with the device id.
                // The race condition -- what if the BT advertisement address gets
                // reused somehow, and we are stepping on some other device's
                // data -- can only happen in deliberately contrived circumstances.
                currSaveData.UpdateWithDevice(knownDevice);
                return currSaveData;
            }

            // There's both an advertisement SaveData and a deviceId one, and they aren't
            // merged. This will happen when the device changes it's advertisement address.
            // We had made a new tmporary SaveData with the advertisement address, but when
            // we finally do connect, there's a better permanent choice available. 
            // Switch to the permanent one. 
            // The temporary one will just be abandoned. Technically, it will hang around
            // in the 
            currSaveData.History.StatusInformation = "Temporary (reason A8.2)";
            currSaveData = newSaveData;
            currSaveData.UpdateWithDevice(knownDevice);
            return currSaveData;
        }

    }




    public class SaveData
    {
        public SaveData(KnownDevice knownDevice)
        {
            Id.AdvertisementAddress = knownDevice.Advertisement?.Addr ?? 0;
            Id.AdvertisementName = knownDevice.Advertisement?.BestName ?? "";
            Id.ConnectAddress = knownDevice.BTLEDevice?.BluetoothAddress ?? 0;
            Id.ConnectName = knownDevice.BTLEDevice?.Name ?? "";
            Id.DeviceId = knownDevice.BTLEDevice?.DeviceId ?? knownDevice.Id;
        }


        /// <summary>
        /// The first SaveData is created using just the advertisement address.
        /// Update when connected 
        /// </summary>
        /// <param name="knownDevice"></param>
        public void UpdateWithDevice(KnownDevice knownDevice)
        {
            if (knownDevice.Advertisement != null)
            {
                Id.AdvertisementAddress = knownDevice.Advertisement.Addr;
                Id.AdvertisementName = knownDevice.Advertisement.BestName;
            }
            if (knownDevice.BTLEDevice != null)
            {
                Id.ConnectAddress = knownDevice.BTLEDevice.BluetoothAddress;
                if (!string.IsNullOrEmpty(knownDevice.BTLEDevice.Name))
                {
                    Id.ConnectName = knownDevice.BTLEDevice.Name;
                }
                if (!string.IsNullOrEmpty(Id.DeviceId) && Id.DeviceId != knownDevice.BTLEDevice?.DeviceId)
                {
                    ; // handy place for a debugger
                }
                if (string.IsNullOrEmpty(Id.DeviceId))
                {
                    Id.DeviceId = knownDevice.BTLEDevice?.DeviceId ?? knownDevice.Id;
                }
            }
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
        /// Gets the correct colors (which can be updated). The theme should be Application.Current.RequestedTheme
        /// </summary>
        /// <param name="theme"></param>
        /// <returns></returns>
        public DeviceColors GetDeviceColors(ApplicationTheme theme)
        {
            switch (theme)
            {
                case ApplicationTheme.Dark:
                    return Preferences.DarkColors;
            }
            return Preferences.LightColors;
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
    public class DeviceColors
    {
        public static uint ColorIsDefault = 0xAA000000; // Valid but uncommon ARGB color.
        /// <summary>
        /// Color is a uint of ARGB (msb to lsb, of course)
        /// </summary>
        public uint TextColor { get; set; } = ColorIsDefault;
        public uint BackgroundColor { get; set; } = ColorIsDefault;

        public Dictionary<string, uint> GraphColors { get; set;  } = new Dictionary<string, uint>(); //DOC: need a set for reading in?

        public const string GraphPrefix = "Graph:";

        public void Set(string tagName, uint color)
        {
            switch (tagName)
            {
                case "BackgroundColor": BackgroundColor = color; break;
                case "TextColor": TextColor = color; break;
                default:
                    if (tagName.StartsWith(GraphPrefix))
                    {
                        var gname = tagName.Substring(GraphPrefix.Length); // Remove "Graph:" prefix
                        GraphColors[gname] = color;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Helpful converter class has SolidColorBrush equivilents of the colors in DeviceColors
    /// and a simple constructor.
    /// </summary>
    public class DeviceColorBrushes
    {
        public DeviceColorBrushes(DeviceColors colors)
        {
            TextColorBrush = UtilitiesWinUI3.UtilitiesWinUI3.GetBrush(colors.TextColor);
            BackgroundColorBrush = UtilitiesWinUI3.UtilitiesWinUI3.GetBrush(colors.BackgroundColor);
        }
        public SolidColorBrush TextColorBrush;
        public SolidColorBrush BackgroundColorBrush;


        public SolidColorBrush Get(string tagName)
        {
            switch (tagName)
            {
                case "BackgroundColor": return BackgroundColorBrush;
                case "TextColor": return TextColorBrush;
            }
            return null;
        }



        /// <summary>
        /// Recursively sets the Foreground color for all TextBlocks inside a parent container.
        /// This is a bridge between what all of the device controls need, XAML-wise, and the 
        /// SaveData which is kept XAML-free.
        /// </summary>
        /// 

        public static void SetUxColors(UIElement root, DeviceColorBrushes brushes)
        {
            if (root is TextBlock tb)
            {
                if (brushes.TextColorBrush != null) tb.Foreground = brushes.TextColorBrush;
            }
            else if (root is Border border)
            {
                if (brushes.BackgroundColorBrush != null) border.Background = brushes.BackgroundColorBrush;
                SetUxColors(border.Child, brushes);
            }
            else if (root is Panel parent)
            {
                foreach (var child in parent.Children) 
                {
                    SetUxColors(child, brushes);
                }
            }
            else if (root is ContentControl cc && cc.Content is UIElement ccContent)
            {
                SetUxColors(ccContent, brushes);
            }
            else if (root is UserControl uc && uc.Content is UIElement ucContent)
            {
                SetUxColors(ucContent, brushes);
            }
        }
    }

    public class KnownDeviceUserPreferences
    {
        public string UserName { get; set; } = ""; // will depend on the DeviceIdentification, but could be set by the user to something more memorable
        public DeviceColors DarkColors { get; set; } = new DeviceColors();
        public DeviceColors LightColors { get; set; } = new DeviceColors();

    }

    /// <summary>
    /// Contains the advertisement / connection / data history of the device.
    /// </summary>
    public class DeviceHistory
    {
        public void UpdateAdvertisementHistory(DateTimeOffset dto)
        {
            if (FirstAdvertisement == DateTimeOffset.MinValue) FirstAdvertisement = dto;
            MostRecentAdvertisement = dto;
        }

        public void UpdateConnectionHistory(DateTimeOffset dto, BluetoothConnectionStatus status)
        {
            switch (status)
            {
                case BluetoothConnectionStatus.Connected:
                    UpdateConnectionOkHistory(dto);
                    break;
                default:
                case BluetoothConnectionStatus.Disconnected:
                    UpdateConnectionFailedHistory(dto);
                    break;
            }
        }

        
        private void UpdateConnectionOkHistory(DateTimeOffset dto)
        {
            if (FirstConnectionOk == DateTimeOffset.MinValue) FirstConnectionOk = dto;
            MostRecentConnectionOk = dto;
        }

        private void UpdateConnectionFailedHistory(DateTimeOffset dto)
        {
            if (FirstConnectionFailed == DateTimeOffset.MinValue) FirstConnectionFailed = dto;
            MostRecentConnectionFailed = dto;
        }

        public void UpdateDataHistory(DateTimeOffset dto)
        {
            if (FirstData == DateTimeOffset.MinValue) FirstData = dto;
            MostRecentData = dto;
        }

        public DateTimeOffset FirstAdvertisement { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset FirstConnectionOk { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset FirstConnectionFailed { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset FirstData { get; set; } = DateTimeOffset.MinValue;

        public DateTimeOffset MostRecentAdvertisement { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset MostRecentConnectionOk { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset MostRecentConnectionFailed { get; set; } = DateTimeOffset.MinValue;
        public DateTimeOffset MostRecentData { get; set; } = DateTimeOffset.MinValue;

        public string StatusInformation { get; set; } = "";
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
