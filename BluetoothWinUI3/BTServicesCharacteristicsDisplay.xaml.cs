using BluetoothConversions;
using BluetoothProtocols;
using BluetoothProtocolsNames;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using IotNumberFormats;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    /// <summary>
    /// Despite the name, this displays both advertisements and also can connect to a device and dump out all the services + characteristics.
    /// </summary>
    public sealed partial class BTServicesCharacteristicsDisplay : UserControl, IHandleBTAdvertisements, IDeviceControlBasic
    {
        /// <summary>
        /// Standard: Panel size. Set in UpdateUX from MainWindow.
        /// </summary>
        MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400

        /// <summary>
        /// Used for logging only
        /// </summary>
        private readonly string InternalDeviceType = "BT_Services";

        /// <summary>
        /// Tags for the device. This is used to categorize the different devices.
        /// Common tags: environment exersise health cooking agriculture light
        /// </summary>
        public string Tags { get { return "#other"; } }

        public BTServicesCharacteristicsDisplay()
        {
            InitializeComponent();
            Loaded += BTServicesCharacteristicsDisplay_Loaded;
            uiConnectionControl.SetBatteryVisibility(Visibility.Collapsed); // never show battery level here.
            uiConnectionControl.ConnectionChanged += UiConnectionControl_ConnectionChanged;
        }

        /// <summary>
        /// AllAdvertisementHistory is used only by AddHistory and GetHistory
        /// </summary>
        private SortedDictionary<ulong, string> AllAdvertisementHistory = new SortedDictionary<ulong, string>();
        private SortedDictionary<ulong, string> AllAdvertisementManufacturerDataHistory = new SortedDictionary<ulong, string>();
        public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return null; } // TODO: is this correct?
        public void ClearAccumulatedFineGrainedData() {; } // do nothing
        /// <summary>
        /// Called from MainWindow when the user wants to clear their graph
        /// </summary>
        public void ClearData() { ; } // do nothing

        public IBTCommonMetaData GetDataMostRecent() { return null; }



        private async void UiConnectionControl_ConnectionChanged(object sender, ConnectionChangedEventArgs e)
        {
            switch (e.NewConnectionState)
            {
                case BTConnectionControl.ConnectionState.Connecting:
                    uiDeviceDetailsTextBlock.Text = $"Connecting to {e.CurrWatcherData.AddressAsString} {e.CurrWatcherData.BestName}";
                    ShowDetail(DetailPane.DeviceDetails);
                    break;
                case BTConnectionControl.ConnectionState.ConnectionFailed:
                    uiDeviceDetailsTextBlock.Text = $"Unable to connect to {e.CurrWatcherData.AddressAsString} {e.CurrWatcherData.BestName}";
                    break;
                case BTConnectionControl.ConnectionState.Connected:
                    // Connected, so let's grab data from the device
                    // There might still be failures BTW.
                    await DoConnected(e.Device);
                    break;
            }
        }
        /// <summary>
        /// If the device had become disconnected, the control uses this (via BTConnectionControl.GotAnotherAdvertisement)
        /// to trigger a reconnect attempt. GotAnotherAdvertisement is smart and will only reconnect as appropriate.
        /// </summary>
        public async Task HandleMyAdvertisementAsync(WatcherData data)
        {
            // This control doesn't ever want to reconnect automatically
            await Task.Delay(0); // uiBTConnectionControl.GotAnotherAdvertisementAsync();
        }


        // TODO: get this hooked up!
        public async Task ReconnectAsync()
        {
            await Task.Delay(0);
        }
        bool IsFirstLoad = true;
        private void BTServicesCharacteristicsDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsFirstLoad)
            {
                ShowDetail(DetailPane.None);
            }
            IsFirstLoad = false;
        }

        private void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }
        private void DeviceDetailsLog(string str)
        {
            uiDeviceDetailsTextBlock.Text += str + "\n";
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }

        private void AddHistory(WatcherData data)
        {
            var addr = data.Addr;
            if (AllAdvertisementHistory.ContainsKey(addr))
            {
                AllAdvertisementHistory[addr] += "\n\n" + data.ToStringDetails();
                AllAdvertisementManufacturerDataHistory[addr] += data.ToStringManufacturerData("");
            }
            else
            {
                AllAdvertisementHistory.Add(data.Addr, data.ToStringDetails());
                AllAdvertisementManufacturerDataHistory.Add(addr, data.ToStringManufacturerData(""));
            }
        }

        private string GetHistory(string defaultValue)
        {
            if (AllAdvertisementHistory.Count == 0) return defaultValue;

            StringBuilder sb = new StringBuilder();
            foreach (var (index, str) in AllAdvertisementHistory)
            {
                sb.Append(str);
                sb.Append("\n\n⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯\n");
                var md = AllAdvertisementManufacturerDataHistory[index];
                var lastComma = md.LastIndexOf(',');
                if (lastComma > 0) // Also implies there's data!
                {
                    md = md.Remove(lastComma, 1); // Remove the last comma so it's JSON compatible
                    sb.Append("\t\t\t\"AdvertisementData\" : [\n");
                    sb.Append(md);
                    sb.Append("\t\t\t],\n");
                }
                sb.Append("\n\n\n⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯\n");
            }
            var retval = sb.ToString();
            return retval;
        }




        /// <summary>
        /// Observable List of all of the WatcherData we have gotten. Is displayed in the 
        /// list of advertisements.
        /// </summary>
        public ObservableCollection<WatcherData> WatcherDataList { get; internal set; } = new ObservableCollection<WatcherData>();
        private int FindWatcherDataIndex(WatcherData data)
        {
            for (int i = 0; i < WatcherDataList.Count; i++)
            {
                if (WatcherDataList[i].Addr == data.Addr)
                {
                    return i;
                }
            }
            return -1;
        }

        public async Task HandleAdvertisementAsync(WatcherData data)
        {
            var index = FindWatcherDataIndex(data);
            if (index == -1)
            {
                WatcherDataList.Add(data);
            }
            else
            {
                WatcherDataList[index] = data;
            }
            AddHistory(data);
            MostRecentWatcherData = data;
            uiLog.Text = data.ToString();
            if (SelectedWatcherData != null && data.Addr == SelectedWatcherData.Addr)
            {
                // The device the user selected has sent a new (or scan-response) advertisement! Update!
                SelectedWatcherData = data;
                var details = data.ToStringDetails();
                uiAdvertisementDetailsControl.StartMostRecent(details);
                uiConnectionControl.SetAdvertisementData(data);
            }
        }


        /// <summary>
        /// SaveData is per-device and includes the display name (e.g., a "Thingy" might have a preferred name of "Living Room")
        /// and also a bunch of color information.
        /// </summary>
        public void UpdateUX(SaveData saveData)
        {
            if (saveData == null) return;

            var colors = saveData.GetDeviceColors(Application.Current.RequestedTheme);
            var brushes = new DeviceColorBrushes(colors);
            DeviceColorBrushes.SetUxColors(this.rootPanel, brushes);
        }

        public void UpdateUX(UserPreferences newPrefs, UserPreferences oldPrefs)
        {
            Log($"{InternalDeviceType}: UpdateUX with UserPreferences");
        }

        bool DetailsAlwaysShown = false;
        public void UpdateUX(MainWindow.WindowSize windowSize, Windows.Foundation.Size largeActualSize)
        {
            CurrWindowSize = windowSize;

            switch (CurrWindowSize)
            {
                default:
                case MainWindow.WindowSize.Normal:
                    rootPanel.Width = 380;
                    rootPanel.Height = 380;
                    uiLog.MaxWidth = rootPanel.Width - 30;
                    DetailsAlwaysShown = false;
                    break;
                case MainWindow.WindowSize.Large:
                    rootPanel.Width = largeActualSize.Width;
                    rootPanel.Height = largeActualSize.Height;
                    uiLog.MaxWidth = rootPanel.Width - 30;
                    DetailsAlwaysShown = largeActualSize.Width > 800;
                    break;
            }

            // This will update what's visible in the correct way (e.g., when switching
            // to large and currently showing advertisement details, will show
            // both the list and the details
            ShowDetail(CurrDetailPane);
        }

        public IDeviceControlBasic.Visibility GetDataGridVisibility()
        {
            return IDeviceControlBasic.Visibility.Collapsed;
        }

        public void SetDataGridVisibility(IDeviceControlBasic.Visibility visibility)
        {
        }

        public IDeviceControlBasic.UXCapabilities GetUXCapabilities()
        {
            var retval = IDeviceControlBasic.UXCapabilities.CanGetDetails; 
            return retval;
        }

        public void ExportGraphAsPng()
        {
        }

        /// <summary>
        /// Which detail pane to show?
        /// </summary>
        private enum DetailPane { None, AdvertisementDetails, DeviceDetails }
        private DetailPane CurrDetailPane = DetailPane.None;

        /// <summary>
        /// Shows the new detail pane, or none. Will correctly set visibility and the 
        /// column as appropriate (e.g., based on DetailsAlwaysShown).
        /// </summary>
        /// <param name="pane"></param>
        private void ShowDetail(DetailPane pane)
        {
            CurrDetailPane = pane;
            switch (pane)
            {
                case DetailPane.None:
                    uiAdvertisementList.Visibility = Visibility.Visible;
                    uiDetailsPane.Visibility = DetailsAlwaysShown ? Visibility.Visible : Visibility.Collapsed;
                    uiAdvertisementDetailsControl.Visibility = Visibility.Collapsed;
                    uiDeviceDetails.Visibility = Visibility.Collapsed;
                    uiConnectionControl.Visibility = Visibility.Collapsed;
                    uiBack.IsEnabled = false;
                    break;
                case DetailPane.AdvertisementDetails:
                    uiAdvertisementList.Visibility = DetailsAlwaysShown ? Visibility.Visible : Visibility.Collapsed; ;
                    uiDetailsPane.Visibility = Visibility.Visible;
                    uiAdvertisementDetailsControl.Visibility = Visibility.Visible;
                    uiDeviceDetails.Visibility = Visibility.Collapsed;
                    uiConnectionControl.Visibility = Visibility.Visible;
                    uiBack.IsEnabled = DetailsAlwaysShown ? false : true;
                    break;
                case DetailPane.DeviceDetails:
                    uiAdvertisementList.Visibility = DetailsAlwaysShown ? Visibility.Visible : Visibility.Collapsed; ;
                    uiDetailsPane.Visibility = Visibility.Visible;
                    uiAdvertisementDetailsControl.Visibility = Visibility.Collapsed;
                    uiDeviceDetails.Visibility = Visibility.Visible;
                    uiConnectionControl.Visibility = Visibility.Visible;
                    uiBack.IsEnabled = true;
                    break;
            }

            Grid.SetColumn(uiDetailsPane, DetailsAlwaysShown ? 1 : 0);
        }

        WatcherData SelectedWatcherData = null;
        WatcherData MostRecentWatcherData = null;

        private async void OnAdvertisementSelected(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
        {
            var data = sender.SelectedItem as WatcherData;
            if (data == null) return;
            Log($"OnAdvertisementSelected: selected={data.AddressAsString} name={data.BestName}");
            if (SelectedWatcherData != null && SelectedWatcherData.Addr == data.Addr)
            {
                return; // is already selected
            }
            SelectedWatcherData = data;
            var details = data.ToStringDetails();
            uiAdvertisementDetailsControl.StartMostRecent(details);
            uiConnectionControl.SetAdvertisementData(data);
            ShowDetail(DetailPane.AdvertisementDetails);

            // Let's run in through the smart analyzer, too!
            var analysis = await DeviceInformationSmartCache.AnalyzeAsync(SelectedWatcherData);
            if (analysis != null)
            {
                uiAdvertisementDetailsControl.AddToMostRecent($"\n\nSmart Analysis: {analysis.AnalysisResult} {analysis.Analysis}");
            }
        }

        private void OnBackClicked(object sender, RoutedEventArgs e)
        {
            switch (CurrDetailPane)
            {
                case DetailPane.None:
                    ShowDetail(DetailPane.None); // should not be possible
                    break;
                case DetailPane.AdvertisementDetails:
                    ShowDetail(DetailPane.None);
                    break;
                case DetailPane.DeviceDetails:
                    ShowDetail(DetailPane.AdvertisementDetails);
                    break;
            }
        }

        /// <summary>
        /// Handle all the stuff that happens on connect.
        /// - Create summary of the device in JSON
        /// - add to uimNotify for everything that can notify
        /// </summary>
        private async Task DoConnected(BluetoothLEDevice le)
        {
            uimIndicate.Items.Clear();
            uimNotify.Items.Clear();
            uimRead.Items.Clear();
            uimWrite.Items.Clear();

            try
            {

                BluetoothCacheMode cacheMode = BluetoothCacheMode.Cached;
                // cacheMode = BluetoothCacheMode.Uncached; // TODO: just for now while debugging
                var addr = SelectedWatcherData.Addr;

                /* 2026-09-04 all this was a failed attempt to connect to the BT-90EPD multimeter.
                var aresult = await le.RequestAccessAsync();
                var pto = BluetoothLEPreferredConnectionParameters.ThroughputOptimized; // timeout=0xC8
                var ppo = BluetoothLEPreferredConnectionParameters.PowerOptimized; // timeout=0x258
                var pb = BluetoothLEPreferredConnectionParameters.Balanced; // timeout=0x190
                var cpresult = le.RequestPreferredConnectionParameters(pto);
                var cparam = le.GetConnectionParameters(); // Returns 3C0=nine seconds???
                var dresult = await le.GetGattServicesForUuidAsync(BluetoothUuidHelper.FromShortId(0xFFB0), cacheMode); // prefetch the base services
                */
                var services = await le.GetGattServicesAsync(cacheMode);
                if (services.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
                {
                    DeviceDetailsLog($"Unable to get services for {SelectedWatcherData.AddressAsString}. Reason: {services.Status}");
                    return;
                }
                uiDeviceDetailsTextBlock.Text = $"Services for {SelectedWatcherData.AddressAsString} {SelectedWatcherData.BestName}\n\n";

                var nameDeviceList = new NameAllBleDevices();
                var nameDevice = new NameDevice();
                nameDevice.Name = le.Name;
                nameDevice.Details += "TODO: line 190";
                nameDeviceList.AllDevices.Add(nameDevice);
                // TODO: skipping copying classModifiers ClassName Description from knownDevice
                int serviceCount = 0;

                var defaultDevice = BleNames.GetDevice(nameDevice.Name);

                foreach (var service in services.Services)
                {
                    // Find the right default service
                    var defaultService = defaultDevice.GetService(service.Uuid);
                    var nameService = new NameService(service, defaultService, serviceCount++);
                    nameDevice.Services.Add(nameService);

                    var shortuuid = BluetoothUuidHelper.TryGetShortId(service.Uuid);
                    var guidAsAscii = service.Uuid.AsAscii();
                    if (guidAsAscii != "") guidAsAscii = $" ({guidAsAscii})";
                    var serviceUuidStr = (shortuuid != null) ? $"{shortuuid:X4}" : service.Uuid.ToString();
                    var servicename = (shortuuid != null) ? BluetoothServiceUuid16Bit.Decode((ushort)shortuuid) + " " : "";
                    if (shortuuid != null)
                    {
                        nameService.Name = BluetoothServiceUuid16Bit.Decode((ushort)shortuuid);
                    }

                    var servicesb = new StringBuilder();
                    servicesb.AppendLine($"Service {servicename}Uuid={serviceUuidStr}{guidAsAscii} handle={service.AttributeHandle}");
                    var dai = service.DeviceAccessInformation;
                    var session = service.Session;
                    servicesb.AppendLine($"    AccessInformation: status={dai.CurrentStatus} prompt={dai.UserPromptRequired}");
                    servicesb.AppendLine($"    DeviceId={service.DeviceId}");
                    servicesb.AppendLine($"    Session: Status={session.SessionStatus} MaxPduSize (MTU)={session.MaxPduSize}");
                    servicesb.AppendLine($"    Session: CanMaintainConnection={session.MaintainConnection} MaintainConnection={session.MaintainConnection}");
                    uiDeviceDetailsTextBlock.Text += servicesb.ToString();

                    var chresult = await service.GetCharacteristicsAsync(cacheMode);
                    if (chresult.Status != GattCommunicationStatus.Success)
                    {
                        DeviceDetailsLog($"    Unable to get characteristics reason={chresult.Status} {chresult.ProtocolError}");
                    }
                    else
                    {
                        int characteristicCount = 0;

                        foreach (var characteristic in chresult.Characteristics)
                        {
                            var chshortuuid = BluetoothUuidHelper.TryGetShortId(characteristic.Uuid);
                            var chUuidStr = (chshortuuid != null) ? $"{chshortuuid:X4}" : characteristic.Uuid.ToString();
                            var chname = (chshortuuid != null) ? $"name={BluetoothCharacteristic.Decode((ushort)chshortuuid)} " : "";
                            guidAsAscii = characteristic.Uuid.AsAscii();
                            if (guidAsAscii != "") guidAsAscii = $" ({guidAsAscii})";

                            var defaultCharacteristic = defaultService?.GetCharacteristic(characteristic.Uuid);
                            var nameCharacteristic = new NameCharacteristic(characteristic, nameService, defaultCharacteristic, characteristicCount++);
                            if (chshortuuid != null)
                            {
                                nameCharacteristic.Name = BluetoothCharacteristic.Decode((ushort)chshortuuid);
                            }
                            nameService.Characteristics.Add(nameCharacteristic);

                            var chsb = new StringBuilder();
                            chsb.AppendLine($"    Characteristic {chname}Uuid={chUuidStr}{guidAsAscii} handle={characteristic.AttributeHandle}");
                            if (!String.IsNullOrEmpty(characteristic.UserDescription))
                            {
                                chsb.AppendLine($"        Description: {characteristic.UserDescription}");
                            }
                            chsb.AppendLine($"        Properties: {characteristic.CharacteristicProperties}");
                            chsb.AppendLine($"        Protection Level: {characteristic.ProtectionLevel}");
                            foreach (var format in characteristic.PresentationFormats)
                            {
                                chsb.AppendLine($"        Presentation: type={format.FormatType} description={format.Description} unit={format.Unit} exp={format.Exponent} namespace={format.Namespace:X2} sig={GattPresentationFormat.BluetoothSigAssignedNumbers:X2}");
                            }

                            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                            {
                                var readresult = await characteristic.ReadValueAsync(cacheMode);
                                if (readresult.Status != GattCommunicationStatus.Success)
                                {
                                    chsb.AppendLine($"        Read failed: {readresult.Status} protocol error={readresult.ProtocolError}");
                                }
                                else
                                {
                                    var buff = readresult.Value;
                                    if (buff.Length == 1)
                                    {
                                        ;
                                    }
                                    var dr = DataReader.FromBuffer(buff);
                                    var (str, readstatus) = DataReaderReadStringRobust.ReadStringEntire(dr, DataReaderReadStringRobust.OptionsForReadString.ReplaceNull);

                                    nameCharacteristic.ExampleData.Add(str);

                                    chsb.AppendLine($"        Read: {str}");
                                }
                            }
                            bool addToMap = false;
                            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                            {
                                ToggleMenuFlyoutItem tmfi = new()
                                {
                                    Text = $"{nameService.Name} -- {nameCharacteristic.Name}",
                                    IsChecked = false,
                                    Tag = characteristic,
                                };
                                tmfi.Click += OnIndicateToggleClicked;
                                uimIndicate.Items.Add(tmfi);
                                addToMap = true;
                            }
                            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                            {
                                ToggleMenuFlyoutItem tmfi = new()
                                {
                                    Text = $"{nameService.Name} -- {nameCharacteristic.Name}",
                                    IsChecked = false,
                                    Tag = characteristic,
                                };
                                tmfi.Click += OnNotifyToggleClicked;
                                uimNotify.Items.Add(tmfi);
                                addToMap = true;
                            }
                            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                            {
                                MenuFlyoutItem tmfi = new()
                                {
                                    Text = $"{nameService.Name} -- {nameCharacteristic.Name}",
                                    Tag = characteristic,
                                };
                                tmfi.Click += OnReadClicked;
                                uimRead.Items.Add(tmfi);
                                addToMap = true;
                            }
                            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
                            {
                                MenuFlyoutItem tmfi = new()
                                {
                                    Text = $"{nameService.Name} -- {nameCharacteristic.Name}",
                                    Tag = characteristic,
                                };
                                tmfi.Click += OnWriteClicked;
                                uimWrite.Items.Add(tmfi);
                                addToMap = true;
                            }
                            if (addToMap)
                            {
                                CharacteristicNameMap[characteristic] = nameCharacteristic;
                            }
                            uiDeviceDetailsTextBlock.Text += chsb.ToString();
                        }
                    }

                    uiDeviceDetailsTextBlock.Text += $"\n";
                }
                // Build a JsonNode that omits empty strings and empty arrays, then
                // serialize with System.Text.Json.
                var resolver = new DefaultJsonTypeInfoResolver();
                var jsonOptions = new JsonSerializerOptions()
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWriting,
                    TypeInfoResolver = resolver,
                };
                //var JsonAsList = System.Text.Json.JsonSerializer.Serialize(nameDeviceList, jsonOptions); // , jsonFormat, jsonSettings);
                var node = BluetoothWinUI3.SystemTextJsonCleaner.ToJsonNode(nameDeviceList);
                var JsonAsList = node?.ToJsonString(jsonOptions) ?? "";
                JsonAsList = StripPointlessJson(JsonAsList);

                uiDeviceDetailsTextBlock.Text += $"\n\n\n" + JsonAsList;
            }
            catch (Exception ex)
            {
                DeviceDetailsLog($"Exception: {ex.Message}");
            }

        }

        Dictionary<GattCharacteristic, NameCharacteristic> CharacteristicNameMap = new();
        HashSet<GattCharacteristic> CharacteristicsWithValueChangedCallback = new();
        private async void OnIndicateToggleClicked(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleMenuFlyoutItem;
            if (toggle == null) return;
            var ch = toggle.Tag as GattCharacteristic;
            if (ch == null)
            {
                Log($"BTServices: indicate isn't a GattCharacteristic");
                return;
            }
            var notifyType = toggle.IsChecked ? GattClientCharacteristicConfigurationDescriptorValue.Indicate : GattClientCharacteristicConfigurationDescriptorValue.None;
            var result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
            if (!CharacteristicsWithValueChangedCallback.Contains(ch))
            {
                ch.ValueChanged += Characteristic_ValueChanged;
                CharacteristicsWithValueChangedCallback.Add(ch);
            }
        }
        private async void OnNotifyToggleClicked(object sender, RoutedEventArgs e)
        {
            var toggle = sender as ToggleMenuFlyoutItem;
            if (toggle == null) return;
            var ch = toggle.Tag as GattCharacteristic;
            if (ch == null)
            {
                Log($"BTServices: notify isn't a GattCharacteristic");
                return;
            }
            var notifyType = toggle.IsChecked ? GattClientCharacteristicConfigurationDescriptorValue.Notify : GattClientCharacteristicConfigurationDescriptorValue.None;
            try
            {
                var result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!CharacteristicsWithValueChangedCallback.Contains(ch))
                {
                    ch.ValueChanged += Characteristic_ValueChanged;
                    CharacteristicsWithValueChangedCallback.Add(ch);
                }
            }
            catch (Exception ex)
            {
                Log($"Notify: exception: {ex.Message}");
            }
        }
        private async void OnReadClicked(object sender, RoutedEventArgs e)
        {
            var toggle = sender as MenuFlyoutItem;
            if (toggle == null) return;
            var ch = toggle.Tag as GattCharacteristic;
            if (ch == null)
            {
                Log($"BTServices: read isn't a GattCharacteristic");
                return;
            }
            try
            {
                var readResult = await ch.ReadValueAsync(BluetoothCacheMode.Cached);
                if (readResult.Status != GattCommunicationStatus.Success)
                {
                    Log($"Read: error: status={readResult.Status}");
                }
                else
                {
                    NameCharacteristic name = null;
                    CharacteristicNameMap.TryGetValue(ch, out name);
                    string decodestr = name?.Type ?? "BYTES|HEX|data|";
                    var parseResult = IotNumberFormats.ValueParser.Parse(readResult.Value.ToArray(), decodestr);
                    var str = parseResult.AsString;
                    Log($"Read: {name} {str}");
                }
            }
            catch (Exception ex)
            {
                Log($"Read: exception: {ex.Message}");
            }
        }
        private async void OnWriteClicked(object sender, RoutedEventArgs e)
        {
            var toggle = sender as MenuFlyoutItem;
            if (toggle == null) return;
            var ch = toggle.Tag as GattCharacteristic;
            if (ch == null)
            {
                Log($"BTServices: write isn't a GattCharacteristic");
                return;
            }
            var nc = CharacteristicNameMap[ch]; // never null
            var content = new TextBox()
            {
                Header = "HEX bytes to write",
                Text = "b0 04 00 01 f0 d1 C2",
            };
            var dlg = new ContentDialog()
            {
                Title = "Write value",
                Content = content,
                PrimaryButtonText = "Write",
                SecondaryButtonText = "Cancel",
                XamlRoot = this.XamlRoot,
            };
            var result = await dlg.ShowAsync();
            if (result != ContentDialogResult.Primary)
            {
                Log($"Write: cancelled");
                return;
            }
            var hexstr = content.Text;
            var compiled = ParserFieldList.ParseLine(nc.Type);
            var bytes = ReverseCalculations.StringToBytes(compiled, hexstr);
            //var bytesx = HexUtilities.HexStringToByteArray(hexstr);
            // 

            try
            { 
            var writeResult = await ch.WriteValueAsync(bytes.ToArray().AsBuffer(), GattWriteOption.WriteWithoutResponse);
            if (writeResult != GattCommunicationStatus.Success)
            {
                Log($"Write: error: status={writeResult}");
            }
            else
            {
                Log($"Write: success");
            }
            }
            catch (Exception ex)
            {
                Log($"Write: exception: {ex.Message}");
            }
        }


        private void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            NameCharacteristic name = null;
            CharacteristicNameMap.TryGetValue(sender, out name);
            string decodestr = name?.Type ?? "BYTES|HEX|data|";
            //var vr = new IotNumberFormats.ValueParser(decodestr); // was just plain "BYTES|HEX|data|"
            //vr.Initialize(args.CharacteristicValue.ToArray());
            //var str = vr.GetNextString();
            var parseResult = IotNumberFormats.ValueParser.Parse(args.CharacteristicValue.ToArray(), decodestr);
            var str = parseResult.AsString;
            Log($"Characteristic Changed: {name.Name} {str}");
        }


        private static string StripPointlessJson(string json)
        {
            json = RemovePointlessJson(json, "Suppress", "false");
            json = RemovePointlessJson(json, "IsRead", "false");
            json = RemovePointlessJson(json, "IsWrite", "false");
            json = RemovePointlessJson(json, "SuppressRead", "false");
            json = RemovePointlessJson(json, "SuppressWrite", "false");
            json = RemovePointlessJson(json, "IsWriteWithoutResponse", "false");
            json = RemovePointlessJson(json, "IsNotify", "false");
            json = RemovePointlessJson(json, "IsIndicate", "false");
            json = RemovePointlessJson(json, "RegistrationOwner", "Bluetooth Standard");
            return json;
        }
        private static string RemovePointlessJson(string json, string fieldName, string removeValue)
        {
            fieldName = "\"" + fieldName + "\"";
            bool keepGoing = true;
            int idx = 0;
            while (keepGoing)
            {
                idx = json.IndexOf(fieldName, idx+1);
                if (idx < 0)
                {
                    keepGoing = false;
                }
                else
                {
                    var idxCR = json.IndexOf("\n", idx);
                    var idxValue = json.IndexOf(removeValue, idx);
                    if (idxCR >= 0 && idxValue >= 0 && idxValue < idxCR)
                    {
                        // Nuke it!
                        var idxStart = IndexOfPreviousCR(json, idx);
                        json = json.Substring(0, idxStart) + json.Substring(idxCR);
                    }
                }
            }
            return json;
        }

        private static int IndexOfPreviousCR(string json, int idx)
        {
            for (int i=idx-1; i>=0; i--)
            {
                if (json[i] == '\n') return i;
            }
            return -1;
        }
        public string GetDetails(IDeviceControlBasic.DetailsType detailsType)
        {
            string retval = "No details";

            switch (detailsType)
            {
                case IDeviceControlBasic.DetailsType.Normal:
                    if (SelectedWatcherData != null) retval = SelectedWatcherData.ToStringDetails();
                    else if (MostRecentWatcherData != null) retval = MostRecentWatcherData.ToStringDetails();
                    else retval = "No advertisement details";
                    break;

                default:
                case IDeviceControlBasic.DetailsType.All:
                    retval = GetHistory("No advertisements");
                    break;
            }
            return retval;
        }

        IHandleNotifyDeviceControlChanges NotifyDeviceControlChangesWindows = null;
        public void SetNotifyDeviceControlChanges(IHandleNotifyDeviceControlChanges mainWindow)
        {
            NotifyDeviceControlChangesWindows = mainWindow;
        }
    }
}
