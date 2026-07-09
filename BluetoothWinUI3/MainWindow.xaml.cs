using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWatcher.Units;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Exporters;
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
using Windows.ApplicationModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{

    // See https://sunriseprogrammer.blogspot.com/2026/04/il2104-il2026-trim-and-json-with-winui3.html
    // See https://stackoverflow.com/questions/70825664/how-to-implement-system-text-json-source-generator-with-a-generic-class
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(UserPreferences), TypeInfoPropertyName = "UserPreferencesWithPropertyName")]
    internal partial class UserPreferencesContext : JsonSerializerContext
    {

    }


    /// <summary>
    /// The main window for the app.
    /// </summary>
    public sealed partial class MainWindow : Window, IHandleNotifyDeviceControlChanges
    {
        // MARKDOWN: IImageProvider requires two methods, ShouldUseThisProvider() and GetImage()

        AdvertisementWatcher AdvertisementWatcher = new AdvertisementWatcher(); // Wraps the BluetoothLEAdvertisementWatcher
        int NAdvertisements = 0;
        UserPreferences CurrUserPrefs = new UserPreferences();

        SmartExportManager SmartExportManager = new SmartExportManager();
        SaveDataRunner SaveDataRunner = new SaveDataRunner();

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


        private async void MainWindow_Loaded(object sender, RoutedEventArgs args)
        {
            if (rootPanel.ActualHeight < 800)
            {
                // DOC: why this size? Because Windows does some kind of scaling that I'm not privy to?
                // The value selected was just kind of eyeballed.
                this.AppWindow.Resize(new Windows.Graphics.SizeInt32(1300, 1080));
            }
            await SetIconAsync();

            SetUxFromUserPreferences();
            if (CurrUserPrefs.AutostartAdvertisementWatcher)
            {
                AdvertisementWatcher.Start();
            }

            SaveDataRunner.Start();
        }

        // Code is From https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.windowing.appwindow.seticon?view=windows-app-sdk-1.8#Microsoft_UI_Windowing_AppWindow_SetIcon_System_String_
        private async Task SetIconAsync()
        {
            Uri uri = new Uri("ms-appx:///Assets/Icons/BTIcon.ico");
            StorageFile storageFile = null;
            try
            {
                storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
            }
            catch (Exception ex)
            {
                Log($"Error while setting icon: {ex.Message}");
            }

            if (storageFile is not null)
            {
                this.AppWindow.SetIcon(storageFile.Path);
            }
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
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
        /// <summary>
        /// List of all of the controls (like BTServicesCharacteristicsDisplay) that can do something 
        /// with all of the advertisements that are seen. Is added to in e.g., OnDebugShowAdvertisements 
        /// and is used by the AdvertisementWatcher_WatcherEvent.
        /// </summary>
        IList<IHandleBTAdvertisements> BTAdvertisementHandlers = new List<IHandleBTAdvertisements>();

        private void AdvertisementWatcher_WatcherEvent(BluetoothLEAdvertisementWatcher sender, BluetoothWatcher.AdvertismentWatcher.WatcherData e)
        {
            // A little bit of logging and storing stuff for debugging
            NAdvertisements++;
            AllAdvertisements.AppendLine($"{NAdvertisements}, " + e.ToStringFull());
            var fmt = WatcherData.AdvertisementStringFormat.CanCompare;
            UniqueAdvertisements[e.ToStringFull(fmt)] = e.ToStringFull();
            fmt = WatcherData.AdvertisementStringFormat.AddressOnly;
            UniqueBTAddresses[e.ToStringFull(fmt)] = e.ToStringFull();


            // Update the UI as needed and create the next device
            UIThreadHelper.CallOnUIThread(() =>
            {
                if (!rootPanel.IsLoaded) return;
                uiAdvertCount.Text = NAdvertisements.ToString();
                uiAdvertRaw.Text = e.ToString();

                if(e.ToString().Contains("GVH"))
                {
                    ; // handy place for a debugger
                }

                // If this is a new advert, see if it's a known type

                var known = KnownDevices.Get(e);
                if (known == null) // not known (e.g.: this specific one hasn't been seen before in this session)
                {
                    var supportedDevice = BluetoothWinUI3.BluetoothWinUI3Registration.SupportedDevices.GetSupported(e);
                    if (supportedDevice != null)
                    {
                        var control = Activator.CreateInstance(supportedDevice.FactoryInterface) as UserControl;
                        known = AddControl(e, control, supportedDevice); 
                        // will add to KnownDevices and updated UX and ...  a control is, e.g., a
                        // BTNordic_ThingyControl. AddControl will add to the Known Device list

                        SmartExportManager.HandleNewKnownDevice(known);
                    }
                }

                if (known != null)
                {
                    // Track the advertisement history
                    var saveData = AllSaveData.FindWithIdOrAdvertisementAddress(known.Id, e.Addr);
                    saveData?.History.UpdateAdvertisementHistory(e.MostRecentAdvertisement.Timestamp);
                }

                // known will be null when it's not a known device and the "known" wasn't created 
                // (most likely because it's not a supported device).

                if (known != null && known.Control is IHandleMyBTAdvertisements handleMy)
                {
                    handleMy.HandleMyAdvertisement(e);
                }



                foreach (var handler in BTAdvertisementHandlers)
                {
                    handler.HandleAdvertisement(e);
                }
            });
        }

        private KnownDevice AddControl(WatcherData e, UserControl control, SupportedDevice supportedDevice)
        {
            var userControl = control as IDeviceControlBasic;

            var zoomControl = new ZoomableDeviceControl();
            zoomControl.SetDeviceControl(control);
            uiKnownDevices.Items.Add(zoomControl);

            var known = KnownDevices.Add(e, control, zoomControl, supportedDevice);
            userControl?.SetNotifyDeviceControlChanges(this);
            AllSaveData.GetOrCreateSaveData(known);
            control.DataContext = known;
            userControl?.UpdateUX(CurrUserPrefs, null);

            if (uiKnownDevices.Items.Count == 1)
            {
                // Select it!
                uiKnownDevices.SelectedIndex = 0;
            }
            return known;
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

        private async void OnHelpAbout(object sender, RoutedEventArgs e)
        {
            var version = Package.Current.Id.Version;
            uiVersion.Text = $"{version.Major}.{version.Minor}";
            await uiDialogAbout.ShowAsync();
        }

        private void OnDebugLoadDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Restore();

        }

        private void OnDebugSaveDevices(object sender, RoutedEventArgs e)
        {
            AllSaveData.Save();
        }

        private async Task<ZoomableDeviceControl> GetZoomableSelectedAsync(string verb)
        {
            var selectedContainer = uiKnownDevices.SelectedItem as ZoomableDeviceControl;
            if (selectedContainer == null && uiKnownDevices.Items.Count == 1)
            {
                // If there is only one device, we can be nice and just rename that one without forcing the user to select it first.
                uiKnownDevices.SelectedIndex = 0;
                selectedContainer = uiKnownDevices.Items[0] as ZoomableDeviceControl;
            }
            if (selectedContainer == null && !String.IsNullOrEmpty(verb))
            {
                await ShowNotice("No device selected", $"You must select a device to {verb} it");
                return null;
            }
            return selectedContainer;
        }

        /// <summary>
        /// Critical note: when verb is NULL or empty, this won't actually await. The await is only
        /// for the ShowNotice.
        /// </summary>
        /// <param name="verb"></param>
        /// <returns></returns>
        private async Task<IDeviceControlBasic> GetBTSelectedAsync(string verb)
        {
            var selectedContainer = await GetZoomableSelectedAsync(verb);
            var selected = selectedContainer?.GetDeviceControl() as IDeviceControlBasic;
            return selected;
        }

        private async Task<KnownDevice> GetKnownDevice(IDeviceControlDevice selected, string verb)
        {
            var knownDevice = selected.DataContextAsKnownDevice;
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



        private async void OnBTColorBackground(object sender, RoutedEventArgs e)
        {
            await DoBTColor("BackgroundColor");
        }

        private async void OnBTColorText(object sender, RoutedEventArgs e)
        {
            await DoBTColor("TextColor");
        }

        private async Task DoBTColor(string colorType)
        {
            if (colorType == null) return;

            string verb = "color";
            var selected = await GetBTSelectedAsync(verb) as IDeviceControlDevice;
            if (selected == null) return;
            var knownDevice = await GetKnownDevice(selected, verb);
            if (knownDevice == null) return;
            var saveData = AllSaveData.GetOrCreateSaveData(knownDevice);

            var colorsSave = saveData.GetDeviceColors(Application.Current.RequestedTheme);
            var colors = new DeviceColorBrushes(colorsSave);
            Windows.UI.Color color = colors.Get(colorType)?.Color ?? Colors.Gray;

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

            var newcolor = UtilitiesWinUI3.UtilitiesWinUI3.ConvertBackIgnoreA(colorPicker.Color);

            // Save it and update colors!
            colorsSave.Set(colorType, newcolor);
            AllSaveData.Save();
            selected.UpdateUX(saveData);
        }

        private async void OnBTRename(object sender, RoutedEventArgs e)
        {
            // TODO: notice how much boilerplate there is here. Simplify it when I make the next change.
            string verb = "rename";
            var selected = await GetBTSelectedAsync(verb) as IDeviceControlDevice;
            if (selected == null) return;
            var knownDevice = await GetKnownDevice(selected, verb);
            if (knownDevice == null) return;
            var saveData = AllSaveData.GetOrCreateSaveData(knownDevice);


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

        private async void OnFileCopyGraphAsPNG(object sender, RoutedEventArgs e)
        {
            string verb = "copy";
            var selected = await GetBTSelectedAsync(verb) as IDeviceControlDevice;
            if (selected == null) return;

            var caps = selected.GetUXCapabilities();
            if ((caps & IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng) == 0)
            {
                await ShowNotice("Can't copy graph", "The selected device does not support copying the graph");
                return;
            }
            selected.ExportGraphAsPng();
        }

        private async void OnFileCopyDataForExcel(object sender, RoutedEventArgs e)
        {
            string verb = "copy";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;


            var exporter = new Exporters.ExportHtmlForExcel();
            var data = ExportDeviceData.ExportData(selected, exporter);
            //var data = selected.ExportData(exporter);

            try
            {
                var dataPackage = new DataPackage()
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(data);
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();
            }
            catch (Exception ex)
            {
                Log($"Error: unable to make export data for the clipboard; {ex.Message}");
            }
        }

        private async void OnFileCopyDataAsCSV(object sender, RoutedEventArgs e)
        {
            string verb = "copy";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;


            var exporter = new Exporters.ExportCsv();
            var data = ExportDeviceData.ExportData(selected, exporter);
            //var data = selected.ExportData(exporter);

            try
            {
                var dataPackage = new DataPackage()
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(data);
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();
            }
            catch (Exception ex)
            {
                Log($"Error: unable to make export data for the clipboard; {ex.Message}");
            }
        }

        private async void OnFileCopyDetailsAll(object sender, RoutedEventArgs e)
        {
            await DoFileCopyDetails(IDeviceControlBasic.DetailsType.All);
        }

        private async void OnFileCopyDetailsNormal(object sender, RoutedEventArgs e)
        {
            await DoFileCopyDetails(IDeviceControlBasic.DetailsType.Normal);
        }

        private async Task DoFileCopyDetails(IDeviceControlBasic.DetailsType detailsType)
        {
            string verb = "get details";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;


            var data = selected.GetDetails(detailsType);

            try
            {
                var dataPackage = new DataPackage()
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(data);
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();
            }
            catch (Exception ex)
            {
                Log($"Error: unable to get details for the clipboard; {ex.Message}");
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

        private void UpdateAllDeviceUserPreferences(UserPreferences currPrefs, UserPreferences oldPrefs)
        {
            foreach (var item in uiKnownDevices.Items)
            {
                var deviceContainer =item as ZoomableDeviceControl;
                var device = deviceContainer?.GetDeviceControl() as IDeviceControlBasic;
                if (device == null) continue;
                device.UpdateUX(currPrefs, oldPrefs);
            }
        }

        private void OnPreferencesPressure(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement)?.Tag as string;
            if (tag == null) return;
            var oldPrefs = CurrUserPrefs.Clone();
            switch (tag)
            {
                case "mmHg":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.mmHg_Torr;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
                case "inHg":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.inHg;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
                case "mbar_hpa": // same as hPa
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.hectoPascal_milliBar;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
                case "kiloPascal":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.kiloPascal;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
                case "Pascal":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.Pascal;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
                case "PSI":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.PSI;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
                case "Atmosphere":
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.Atmosphere;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
                default:
                    CurrUserPrefs.Pressure = Pressure.PressureUnit.hectoPascal_milliBar;
                    UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
                    break;
            }
        }
        private void OnPreferencesTemperature(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement)?.Tag as string;
            if (tag == null) return;
            var oldPrefs = CurrUserPrefs.Clone();
            switch (tag)
            {
                case "Celcius": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Celcius; break;
                case "Fahrenheit": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Fahrenheit; break;
                case "Kelvin": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Kelvin; break;
                case "Rankine": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Rankine; break;
                case "Réaumur": CurrUserPrefs.Temperature = Temperature.TemperatureUnit.Réaumur; break;
            }
            UpdateAllDeviceUserPreferences(CurrUserPrefs, oldPrefs);
        }

        private async void OnViewToggleTable(object sender, RoutedEventArgs e)
        {
            // Can be called either because the user clicked it (and it changed), or because
            // we moved the selected control and we have to update the menu.
            // That is, if the old control had the table visible, and the new one doesn't, we
            // have to flip the "show table" menu from checked to unchecked. But that will
            // cause this method to be called.
            // If the control is already in the correct state, bypass calling the SetDataGridVisibility

            string verb = "View Table";
            var selectedContainer = await GetZoomableSelectedAsync(verb);
            if (selectedContainer == null) return;
            var selectedControl = selectedContainer.GetDeviceControl();
            var selected = selectedControl as IDeviceControlDevice;
            if (selected == null) return; // should never happen

            var isChecked = (sender as ToggleMenuFlyoutItem).IsChecked;
            var currVisibility = selected.GetDataGridVisibility();
            var newVisibility = (isChecked) ? IDeviceControlDevice.Visibility.Visible : IDeviceControlDevice.Visibility.Collapsed;
            if (currVisibility != newVisibility)
            {
                selected.SetDataGridVisibility(newVisibility);
            }
        }

        WindowSize CurrSelectedWindowSize = WindowSize.Normal;
        private async void OnViewWindowSize(object sender, RoutedEventArgs e)
        {
            string verb = "Resize";
            var selectedContainer = await GetZoomableSelectedAsync(verb);
            if (selectedContainer == null) return;
            var selectedControl = selectedContainer.GetDeviceControl();
            var selected = selectedControl as IDeviceControlBasic;
            if (selected == null) return; // should never happen

            var newSize = (CurrSelectedWindowSize == WindowSize.Normal) ? WindowSize.Large : WindowSize.Normal;
            // NOTE: I am not sure where the 10 comes from.
            var largeActualSize = new Windows.Foundation.Size(uiZoomPanel.ActualWidth - 10, uiZoomPanel.ActualHeight - 10);
            switch (newSize)
            {
                case WindowSize.Normal:
                    uiZoomPanel.Children.Clear();
                    selectedContainer.ReparentDeviceControl();
                    break;
                case WindowSize.Large:
                    selectedContainer.UnparentDeviceControl();
                    selectedControl.HorizontalAlignment = HorizontalAlignment.Center;
                    selectedControl.VerticalAlignment = VerticalAlignment.Center;
                    uiZoomPanel.Children.Add(selectedControl);
                    break;
            }
            selected.UpdateUX(newSize, largeActualSize);
            CurrSelectedWindowSize = newSize;
            // Don't have to do anything on failure (which can't really happen)
        }

        private void UpdateGraphColorMenus(IDeviceControlDevice selectedDevice)
        {
            // Always clear even when selectedDevice is null (we might have gone 
            // from a device that's a IDeviceControlDevice and are now 
            // selecting something that isn't)
            uimBTGraphColorsMenu.Items.Clear();

            if (selectedDevice == null)
            {
                uimBTGraphColorsMenu.IsEnabled = false;
                return;
            }

            // In the menu, Update the set of graph line that can be colored
            var linenames = selectedDevice.LineNames;
            foreach (var linename in linenames)
            {
                var menu = new MenuFlyoutItem()
                {
                    Text = linename,
                    Tag = linename,
                };
                menu.Click += OnChangeGraphColor;
                uimBTGraphColorsMenu.Items.Add(menu);
            }

            var hasLineNames = linenames.Count > 0;
            uimBTGraphColorsMenu.IsEnabled = hasLineNames;
        }

        /// <summary>
        /// Updates menus based on the selected device capabilities.
        /// Capabilities is from GetUXCapabilities() from a device that supports IDeviceControlBasic.
        /// 
        /// </summary>
        private void UpdateFileCopyViewBTMenus(IDeviceControlBasic.UXCapabilities capabilities)
        {
            uimFileCopyGraphAsPNG.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng);
            uimFileCopyDataForExcel.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanGetData);
            uimFileCopyDataAsCSV.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanGetData);
            uimFileCopyDetailsAll.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanGetDetails);
            uimFileCopyDetailsNormal.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanGetDetails);

            uimViewShowTable.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanShowTable);
            uimBTCanRename.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanRename);
        }

        private async void OnKnownDeviceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = await GetBTSelectedAsync(null) as IDeviceControlBasic;
            if (selected == null) return;

            var selectedDevice = selected as IDeviceControlDevice;
            UpdateGraphColorMenus(selectedDevice);

            // Selectively enable the Bluetooth Device menu options. Note that UpdateFileCopyViewBTMenus
            // is also called by OnGetUXCapabilitiesChanged which is triggered when the device changes
            // the capabilities. This happens with e.g., the BTStandard_Demo when the selected device
            // doesn't have a battery and therefore the graph+table are removed.
            var capabilities = selected.GetUXCapabilities();
            UpdateFileCopyViewBTMenus(capabilities);

            uimBTColorBackground.IsEnabled = selectedDevice != null;
            uimBTColorText.IsEnabled = selectedDevice != null;

            var viewShowTableVisibleShouldBeChecked = selected.GetDataGridVisibility() == IDeviceControlBasic.Visibility.Visible;
            var mustChangeViewShowTableVisible = viewShowTableVisibleShouldBeChecked != uimViewShowTable.IsChecked;
            if (mustChangeViewShowTableVisible)
            {
                // Note that this will cause the OnViewToggleTable callback to be called
                // even though it won't have anything to do. We're just updating the menu to 
                // match the actual state of the control.
                uimViewShowTable.IsChecked = viewShowTableVisibleShouldBeChecked;
            }
            uimViewShowTable.IsEnabled = capabilities.HasFlag(IDeviceControlBasic.UXCapabilities.CanShowTable);
        }
        private async void OnChangeGraphColor(object sender, RoutedEventArgs e)
        {
            var axisTitle = (sender as FrameworkElement)?.Tag as string; // e.g., "Temperature" or "Pressure" or "Heart Rate". 
            // the tag is the user name (axisTitle) for the color, not the promptery name

            string verb = "color";
            var selected = await GetBTSelectedAsync(verb) as IDeviceControlDevice;
            if (selected == null) return;
            var knownDevice = await GetKnownDevice(selected, verb);
            if (knownDevice == null) return;
            var saveData = AllSaveData.GetOrCreateSaveData(knownDevice);

            var colorsSave = saveData.GetDeviceColors(Application.Current.RequestedTheme);

            uint coloruint = selected.GetGraphColor(axisTitle);
            Windows.UI.Color color = Windows.UI.Color.FromArgb(
                0xFF, // always fully opaque for graph colors#
                (byte)((coloruint >> 16) & 0xFF),
                (byte)((coloruint >> 8) & 0xFF),
                (byte)(coloruint & 0xFF)
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

            var newcolor = UtilitiesWinUI3.UtilitiesWinUI3.ConvertBackIgnoreA(colorPicker.Color);

            // Save it and update colors!
            colorsSave.Set("Graph:" + axisTitle, newcolor);
            AllSaveData.Save();
            selected.UpdateGraphColor(axisTitle, newcolor);
        }




        private async void OnHelpViewHelp(object sender, RoutedEventArgs e)
        {
            switch (uiGuidance.Visibility)
            {
                case Visibility.Collapsed:
                    var ok = await uiMarkdownHelp.ShowHelpAsync("Help_Main.md");
                    if (ok) uiGuidance.Visibility = Visibility.Visible;
                    break;
                case Visibility.Visible:
                    uiGuidance.Visibility = Visibility.Collapsed;
                    break;
            }

        }

        private async void OnDebugShowDirectories(object sender, RoutedEventArgs e)
        {
            // Path Directory
            uiDirectoriesPreferences.Text = UserPreferences.GetSaveDirectoryAsString();
            try
            {
                Uri uri = new Uri("ms-appx:///Assets/Icons/BTIcon.ico");
                StorageFile storageFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var path = storageFile.Path;
                var lastslash = path.LastIndexOf("\\");
                if (lastslash >= 0) path = path.Substring(0, lastslash); // Remove \BTIcon.ico
                lastslash = path.LastIndexOf("\\");
                if (lastslash >= 0) path = path.Substring(0, lastslash); // Remove \Icons
                lastslash = path.LastIndexOf("\\");
                if (lastslash >= 0) path = path.Substring(0, lastslash); // Remove \Assets
                uiDirectoriesMsAppx.Text = path;
            }
            catch (Exception ex)
            {
                uiDirectoriesMsAppx.Text = ex.Message;
            }
            uiDirectoriesInstallPath.Text = Package.Current.InstalledPath;
            // No amount of setting the width will help. 
            //https://github.com/microsoft/microsoft-ui-xaml/issues/424
            // uiDialogDirectories.Resources["ContentDialogMinWidth"] = 1080;
            await uiDialogDirectories.ShowAsync();
        }

        private async void OnZoomPanelSizeChanged(object sender, SizeChangedEventArgs e)
        {
            string verb = "";
            var selected = await GetBTSelectedAsync(verb);
            if (selected == null) return;

            if (CurrSelectedWindowSize == WindowSize.Large)
            {
                // NOTE: I am not sure where the 10 comes from.
                var largeActualSize = new Windows.Foundation.Size(uiZoomPanel.ActualWidth-10, uiZoomPanel.ActualHeight-10);
                selected.UpdateUX(CurrSelectedWindowSize, largeActualSize);
            }
        }

        private void OnDebugShowAdvertisements(object sender, RoutedEventArgs e)
        {
            var ctrl = new BTServicesCharacteristicsDisplay();
            BTAdvertisementHandlers.Add(ctrl);
            AddControl(null, ctrl, null);
            // null means no watcher data
            // null means not a supported device (since the supported device is determined from the watcher data)
        }

        private void OnDebugSmartExport(object sender, RoutedEventArgs e)
        {
            var data = SmartExportManager.Export();

            try
            {
                var dataPackage = new DataPackage()
                {
                    RequestedOperation = DataPackageOperation.Copy
                };
                dataPackage.SetText(data);
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();
            }
            catch (Exception ex)
            {
                Log($"Error: unable to make export data for the clipboard; {ex.Message}");
            }

        }

        private void OnDebugSetFilterRssiDb(object sender, RoutedEventArgs e)
        {
            var isChecked = (sender as ToggleMenuFlyoutItem)?.IsChecked ?? false;
            AdvertisementWatcher.FilterRssiDb = isChecked ? -65 : -200;
        }

        public async void OnGetUXCapabilitiesChanged(UserControl deviceControl, IDeviceControlBasic.UXCapabilities newCapabilities)
        {
            // If the deviceControl is the currently selected on ..
            var dcb = deviceControl as IDeviceControlBasic;
            if (dcb == null) return;
            var bt = await GetBTSelectedAsync(null); // null means it won't show a notice 
            if (bt == null) return;
            if (bt == dcb)
            {
                // The device that changed is the device that's selected.
                // Update the menus based on the capabilities.
                this.UpdateFileCopyViewBTMenus(newCapabilities);
                UpdateGraphColorMenus(deviceControl as IDeviceControlDevice); // theoretically might be null. 
            }
        }
    }
}
