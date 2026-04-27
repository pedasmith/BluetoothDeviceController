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
using Windows.UI;


#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public static class AllSaveData
    {
        public static List<SaveData> AllDevices = new List<SaveData>();

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
        public static ulong ColorIsDefault = 0xFF000000_000000; // not a valid ARGB color! Those are only 32 bits!
        public ulong TextColor { get; set; } = ColorIsDefault;
        public ulong BackgroundColor { get; set; } = ColorIsDefault;

        public Dictionary<string, ulong> GraphColors { get; set;  } = new Dictionary<string, ulong>(); //TODO: need a set for reading in?

        public const string GraphPrefix = "Graph:";

        public void Set(string tagName, ulong color)
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
            TextColorBrush = GetBrush(colors.TextColor);
            BackgroundColorBrush = GetBrush(colors.BackgroundColor);
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



        private static Color ConvertIgnoreA(ulong color)
        {
            Color retval = new Color()
            {
                A = 0xFF, // ignore the A value completely: (byte)((color >> 24) & 0xff),
                R = (byte)((color >> 16) & 0xff),
                G = (byte)((color >>  8) & 0xff),
                B = (byte)((color >>  0) & 0xff),
            };
            return retval;
        }

        public static ulong ConvertBackIgnoreA(Color color)
        {
            byte a = 0xFF; // would be color.A, but that's set to 0???
            ulong retval = ((ulong)a << 24) | ((ulong)color.R << 16) | ((ulong)color.G << 8) | ((ulong)color.B << 0);
            return retval;
        }

        /// <summary>
        /// Given a SaveData type color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static SolidColorBrush GetBrush(ulong color)
        {
            if (color == DeviceColors.ColorIsDefault) return default;
            return new SolidColorBrush(ConvertIgnoreA(color));
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
