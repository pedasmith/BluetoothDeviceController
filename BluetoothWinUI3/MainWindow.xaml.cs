using BluetoothWatcher.Units;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.System.Display;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    public class UserPreferences
    {
        public Pressure.PressureUnit Pressure { get; set; } = BluetoothWatcher.Units.Pressure.PressureUnit.hectoPascal_milliBar;
        public Temperature.TemperatureUnit Temperature { get; set; } = BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius;
        public bool AutostartAdvertisementWatcher { get; set; } = true;

        public static UserPreferences Restore()
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
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
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "BluetoothDevices");
            Directory.CreateDirectory(folderPath);
            string filePath = Path.Combine(folderPath, "UserPreferences.preferences");

            var json = System.Text.Json.JsonSerializer.Serialize(this, typeof(UserPreferences), UserPreferencesContext.Default);
            File.WriteAllText(filePath, json);
        }
    }

    // See https://sunriseprogrammer.blogspot.com/2026/04/il2104-il2026-trim-and-json-with-winui3.html
    // See https://stackoverflow.com/questions/70825664/how-to-implement-system-text-json-source-generator-with-a-generic-class
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(UserPreferences), TypeInfoPropertyName = "UserPreferencesWithPropertyName")]
    internal partial class UserPreferencesContext : JsonSerializerContext
    {

    }

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher AdvertisementWatcher = new BluetoothWatcher.AdvertismentWatcher.AdvertisementWatcher();
        int NAdvertisements = 0;
        UserPreferences CurrUserPrefs = new UserPreferences();

        public enum WindowSize { Normal, Large } // Normal is 400x400 large is 600x800 (HxW)
        public MainWindow()
        {
            CurrUserPrefs = UserPreferences.Restore();
            UIThreadHelper.DQueue = this.DispatcherQueue;
            AdvertisementWatcher.WatcherEvent += AdvertisementWatcher_WatcherEvent;
            InitializeComponent();
            // Activated is completely the wrong thing! this.Activated += MainWindow_Activated;
            rootPanel.Loaded += MainWindow_Loaded; // but it works for now. TODO: find a better way to trigger the autostart of the watcher after the window is ready.
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            CurrUserPrefs.Save();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs args)
        {
            SetUxFromUserPreferences();
            if (CurrUserPrefs.AutostartAdvertisementWatcher)
            {
                AdvertisementWatcher.Start();
            }
        }

        private void SetUxFromUserPreferences()
        {
            switch (CurrUserPrefs.Pressure)
            {
                case Pressure.PressureUnit.mmHg_Torr: uiPreferencesmmHg.IsChecked = true; break;
                case Pressure.PressureUnit.inHg: uiPreferencesinHg.IsChecked = true; break;
                case Pressure.PressureUnit.hectoPascal_milliBar: uiPreferencesmbar_hpa.IsChecked = true; break;
                case Pressure.PressureUnit.kiloPascal: uiPreferenceskiloPascal.IsChecked = true; break;
                case Pressure.PressureUnit.Pascal: uiPreferencesPascal.IsChecked = true; break;
                case Pressure.PressureUnit.PSI: uiPreferencesPSI.IsChecked = true; break;
                case Pressure.PressureUnit.Atmosphere: uiPreferencesAtmosphere.IsChecked = true; break;
            }
            switch (CurrUserPrefs.Temperature)
            {
                case Temperature.TemperatureUnit.Celcius: uiPreferencesCelcius.IsChecked = true; break;
                case Temperature.TemperatureUnit.Fahrenheit: uiPreferencesFahrenheit.IsChecked = true; break;
                case Temperature.TemperatureUnit.Kelvin: uiPreferencesKelvin.IsChecked = true; break;
                case Temperature.TemperatureUnit.Rankine: uiPreferencesRankine.IsChecked = true; break;
                case Temperature.TemperatureUnit.Réaumur: uiPreferencesRéaumur.IsChecked = true; break;
            }
        }


        /// <summary>
        /// The main way we show notices to the user. This is a little method so that it can be changed when we change
        /// how notices are shown. It's only for things that the user must acknowlege but they don't have to do anything.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="notice"></param>
        /// <returns></returns>
        private async Task ShowNotice(string title, string notice)
        {
            var dialog = new ContentDialog()
            {
                XamlRoot = rootPanel.XamlRoot,
                Title = title,
                Content = notice,
                CloseButtonText = "OK",
            };
            await dialog.ShowAsync();
        }

        StringBuilder AllAdvertisements = new StringBuilder();
        Dictionary<string, string> UniqueAdvertisements = new Dictionary<string, string>();
        Dictionary<string, string> UniqueBTAddresses = new Dictionary<string, string>();

        private void AdvertisementWatcher_WatcherEvent(BluetoothLEAdvertisementWatcher sender, BluetoothWatcher.AdvertismentWatcher.WatcherData e)
        {
            // A little bit of logging and storing stuff for debugging
            NAdvertisements++;
            AllAdvertisements.AppendLine($"{NAdvertisements}, " + e.ToStringFull());
            var fmt = BluetoothWatcher.AdvertismentWatcher.WatcherData.AdvertisementStringFormat.CanCompare;
            UniqueAdvertisements[e.ToStringFull(fmt)] = e.ToStringFull();
            fmt = BluetoothWatcher.AdvertismentWatcher.WatcherData.AdvertisementStringFormat.AddressOnly;
            UniqueBTAddresses[e.ToStringFull(fmt)] = e.ToStringFull();


            // Update the UI as needed and create the next device
            UIThreadHelper.CallOnUIThread(() =>
            {
                uiAdvertCount.Text = NAdvertisements.ToString();
                uiAdvertRaw.Text = e.ToString();



                // If this is a new advert, see if it's a known type
                var supportedDevice = BluetoothWinUI3.BluetoothWinUI3Registration.SupportedDevices.GetSupported(e);
                if (supportedDevice != null)
                {
                    var known = KnownDevices.Get(e);
                    if (known == null)
                    {
                        var control = Activator.CreateInstance(supportedDevice.FactoryInterface) as UserControl;

                        uiKnownDevices.Items.Add(control);
                        known = KnownDevices.Add(e, control, supportedDevice);
                        control.DataContext = known;
                        var userControl = control as IDeviceControl;
                        userControl?.UpdateUX(CurrUserPrefs);

                        if (uiKnownDevices.Items.Count == 1)
                        {
                            // Select it!
                            uiKnownDevices.SelectedIndex = 0;
                        }
                    }
                }
            });
        }

        private void OnAdvertisementStart(object sender, RoutedEventArgs e)
        {
            AdvertisementWatcher.Start();
        }
        private void OnAdvertisementStop(object sender, RoutedEventArgs e)
        {
            AdvertisementWatcher.Stop();
        }

        private void OnAdvertisementCopy(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + AllAdvertisements.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }
        private void OnAdvertisementCopyUnique(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var item in UniqueAdvertisements.Values)
            {
                sb.AppendLine(item);
            }

            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + sb.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }
        private void OnAdvertisementCopyUniqueAddresses(object sender, RoutedEventArgs e)
        {
            var sb = new StringBuilder();
            foreach (var item in UniqueBTAddresses.Values)
            {
                sb.AppendLine(item);
            }

            DataPackage dataPackage = new DataPackage();
            var header = "N," + BluetoothWatcher.AdvertismentWatcher.WatcherData.ToHeaderString() + "\n";
            dataPackage.SetText(header + sb.ToString());
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }
        private void OnFileExit(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void OnHelpAbout(object sender, RoutedEventArgs e)
        {
            // TODO: make this work
        }

        private void OnDebugLoadDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Restore();

        }

        private void OnDebugSaveDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Save();
        }

        private async Task<IDeviceControl> GetBTSelectedAsync(string verb)
        {
            var selected = uiKnownDevices.SelectedItem as IDeviceControl;
            if (selected == null && uiKnownDevices.Items.Count == 1)
            {
                // If there is only one device, we can be nice and just rename that one without forcing the user to select it first.
                uiKnownDevices.SelectedIndex = 0;
                selected = uiKnownDevices.Items[0] as IDeviceControl;
            }
            if (selected == null && !String.IsNullOrEmpty(verb))
            {
                await ShowNotice("No device selected", $"You must select a device to {verb} it");
                return null;
            }
            return selected;
        }

        private async Task<KnownDevice> GetKnownDevice(IDeviceControl selected, string verb)
        {
            var knownDevice = selected.GetKnownDevice();
            if (knownDevice == null)
            {
                await ShowNotice($"Can't ${verb} that device", "You cannot {verb} this device");
                return null;
            }
            if (knownDevice.Id == "")
            {
                await ShowNotice($"Can't {verb} that device", $"You cannot {verb} the selected device. it has no Windows device ID");
                return null;
            }
            return knownDevice;
        }

        private SaveData GetOrCreateSaveData(KnownDevice knownDevice)
        {
            var saveData = AllSaveData.FindWithId(knownDevice.Id);
            if (saveData == null)
            {
                // Must create a new SaveData.
                saveData = new SaveData(knownDevice);
                AllSaveData.Insert(saveData);
                AllSaveData.Save(); // quick update
            }
            return saveData;
        }

        private async void OnBTColor(object sender, RoutedEventArgs e)
        {
            var senderMenu = sender as MenuFlyoutItem;
            var tag = senderMenu?.Tag as string; // BackgroundColor or TextColor
            if (tag == null) return;

            string verb = "color";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;
            var knownDevice = await GetKnownDevice(selected, verb);
            if (knownDevice == null) return;
            var saveData = GetOrCreateSaveData(knownDevice);

            var colorsSave = saveData.GetDeviceColors(Application.Current.RequestedTheme);
            var colors = new DeviceColorBrushes(colorsSave);
            Windows.UI.Color color = colors.Get(tag)?.Color ?? Colors.Gray;

            var colorPicker = new ColorPicker
            {
                IsAlphaEnabled = false, // Allow transparency
                IsColorChannelTextInputVisible = true,
                IsHexInputVisible = true,
                Color = color,
            };
            var dialog = new ContentDialog
            {
                Title = "Select a Color",
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = colorPicker,
                XamlRoot = this.Content.XamlRoot // Required in WinUI 3
            };
            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;
            
            var newcolor = DeviceColorBrushes.ConvertBackIgnoreA (colorPicker.Color);

            // Save it and update colors!
            colorsSave.Set(tag, newcolor);
            AllSaveData.Save();
            selected.UpdateUX(saveData);
        }

        private async void OnBTRename(object sender, RoutedEventArgs e)
        {
            // TODO: notice how much boilerplate there is here. Simplify it when I make the next change.
            string verb = "rename";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;
            var knownDevice = await GetKnownDevice(selected, verb);
            if (knownDevice == null) return;
            var saveData = GetOrCreateSaveData(knownDevice);


            var dlg = uiDialogRenameDevice;
            uiDialogRenameDeviceName.Text = saveData.GetUserName();
            var result = await dlg.ShowAsync();

            string newname = uiDialogRenameDeviceName.Text;
            saveData.SetUserName(newname);
            AllSaveData.Save();
            ;
            selected.UpdateUX(saveData);
        }

        // Keep screen on so that we constantly monitor devices
        // https://learn.microsoft.com/en-us/uwp/api/windows.system.display.displayrequest?view=winrt-28000
        DisplayRequest g_DisplayRequest = null;
        private async void OnFileKeepScreenOn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (g_DisplayRequest == null)
                {
                    g_DisplayRequest = new DisplayRequest();
                }
            }
            catch (Exception ex)
            {
                await ShowNotice("Unable to start", $"Unable to create the DisplayRequest to keep the screen on. Reason={ex.Message}");
            }
            if (g_DisplayRequest == null) return; // just give up

            try
            {
                // This call activates a display-required request. If successful,  
                // the screen is guaranteed not to turn off automatically due to user inactivity. 
                if ((sender as ToggleButton)?.IsChecked ?? true)
                {
                    g_DisplayRequest.RequestActive();
                }
                else
                {
                    g_DisplayRequest.RequestRelease();
                }
            }
            catch (Exception ex)
            {
                await ShowNotice ("Unable to request", $"Error: {ex.Message}");
            }
        }

        private void OnDebugDarkTheme(object sender, RoutedEventArgs e)
        {
            if (this.Content.XamlRoot == null) return;// Replacement for IsLoaded per https://stackoverflow.com/questions/71181437/winui-3-1-0-window-ready-event
            var isChecked = (sender as ToggleMenuFlyoutItem)?.IsChecked;
            var theme = ElementTheme.Light; // Light is the best theme :-)
            if (!isChecked.HasValue)
            {
                theme = ElementTheme.Default;
            }
            else theme = isChecked.Value ? ElementTheme.Dark : ElementTheme.Light;
            rootPanel.RequestedTheme = theme;

        }

        private void UpdateAllDeviceUserPreferences()
        {
            foreach (var item in uiKnownDevices.Items)
            {
                var device = item as IDeviceControl;
                if (device == null) continue;
                device.UpdateUX(CurrUserPrefs);
            }
        }

        private void OnPreferencesPressure(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement)?.Tag as string;
            if (tag == null) return;
            switch (tag)
            {
                case "mmHg":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.mmHg_Torr;
                    UpdateAllDeviceUserPreferences();
                    break;
                case "inHg":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.inHg;
                    UpdateAllDeviceUserPreferences();
                    break;
                case "mbar_hpa": // same as hPa
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.hectoPascal_milliBar;
                    UpdateAllDeviceUserPreferences();
                    break;
                case "kiloPascal":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.kiloPascal;
                    UpdateAllDeviceUserPreferences();
                    break;
                case "Pascal":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.Pascal;
                    UpdateAllDeviceUserPreferences();
                    break;
                case "PSI":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.PSI;
                    UpdateAllDeviceUserPreferences();
                    break;
                case "Atmosphere":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.Atmosphere;
                    UpdateAllDeviceUserPreferences();
                    break;
                default:
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.hectoPascal_milliBar;
                    UpdateAllDeviceUserPreferences();
                    break;
            }
        }
        private void OnPreferencesTemperature(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement)?.Tag as string;
            if (tag == null) return;
            switch (tag)
            {
                case "Celcius": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Celcius; break;
                case "Fahrenheit": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Fahrenheit; break;
                case "Kelvin": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Kelvin; break;
                case "Rankine": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Rankine; break;
                case "Réaumur": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Réaumur; break;
            }
            UpdateAllDeviceUserPreferences();
        }


        WindowSize CurrSelectedWindowSize = WindowSize.Normal;
        private async void OnDebugWindowSize(object sender, RoutedEventArgs e)
        {
            string verb = "Resize";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;
            var newSize = (CurrSelectedWindowSize == WindowSize.Normal) ? WindowSize.Large : WindowSize.Normal;
            selected.UpdateUX(newSize);
            CurrSelectedWindowSize = newSize;
            // Don't have to do anything on failure (which can't really happen)
        }

        private async void OnKnownDeviceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = await GetBTSelectedAsync(null);
            if (selected == null) return;

            // Update the set of graph line that can be colored
            uiBTGraphColorsMenu.Items.Clear();
            var graphnames = selected.GraphNames;
            foreach (var graphname in graphnames)
            {
                var menu = new MenuFlyoutItem()
                {
                    Text = graphname,
                    Tag= graphname,
                };
                menu.Click += OnChangeGraphColor ;
                uiBTGraphColorsMenu.Items.Add(menu);
            }

        }
        private async void OnChangeGraphColor(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement)?.Tag as string; // e.g., "Temperature" or "Pressure"

            string verb = "color";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;
            var knownDevice = await GetKnownDevice(selected, verb);
            if (knownDevice == null) return;
            var saveData = GetOrCreateSaveData(knownDevice);

            var colorsSave = saveData.GetDeviceColors(Application.Current.RequestedTheme);

            ulong colorulong = selected.GetGraphColor(tag);
            Windows.UI.Color color= Windows.UI.Color.FromArgb(
                0xFF, // always fully opaque for graph colors#
                (byte)((colorulong >> 16) & 0xFF),
                (byte)((colorulong >> 8) & 0xFF),
                (byte)(colorulong & 0xFF)
                );


            var colorPicker = new ColorPicker
            {
                IsAlphaEnabled = false, // Allow transparency
                IsColorChannelTextInputVisible = true,
                IsHexInputVisible = true,
                Color = color,
            };
            var dialog = new ContentDialog
            {
                Title = "Select a Color",
                PrimaryButtonText = "OK",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                Content = colorPicker,
                XamlRoot = this.Content.XamlRoot // Required in WinUI 3
            };
            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Primary) return;

            var newcolor = DeviceColorBrushes.ConvertBackIgnoreA(colorPicker.Color);

            // Save it and update colors!
            colorsSave.Set("Graph:" + tag, newcolor);
            AllSaveData.Save();
            selected.UpdateGraph(tag, newcolor);
        }
    }
}
