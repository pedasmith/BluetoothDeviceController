using BluetoothConversions;
using BluetoothProtocols;
using BluetoothProtocolsNames;
using BluetoothWatcher.AdvertismentWatcher;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
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

        public BTServicesCharacteristicsDisplay()
        {
            InitializeComponent();
            Loaded += BTServicesCharacteristicsDisplay_Loaded;
            uiConnectionControl.SetBatteryVisibility(Visibility.Collapsed); // never show battery level here.
            uiConnectionControl.ConnectionChanged += UiConnectionControl_ConnectionChanged;
        }

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

        public void HandleAdvertisement(WatcherData data)
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
            uiLog.Text = data.ToString();
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
            var retval = IDeviceControlBasic.UXCapabilities.None; //  IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng | IDeviceControlBasic.UXCapabilities.CanGetData;
            return retval;
        }

        public void ExportGraphAsPng()
        {
        }

        public string ExportData(IExportData exporter)
        {
            return string.Empty;
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
                    uiAdvertisementDetails.Visibility = Visibility.Collapsed;
                    uiDeviceDetails.Visibility = Visibility.Collapsed;
                    uiConnectionControl.Visibility = Visibility.Collapsed;
                    uiBack.IsEnabled = false;
                    break;
                case DetailPane.AdvertisementDetails:
                    uiAdvertisementList.Visibility = DetailsAlwaysShown ? Visibility.Visible : Visibility.Collapsed; ;
                    uiDetailsPane.Visibility = Visibility.Visible;
                    uiAdvertisementDetails.Visibility = Visibility.Visible;
                    uiDeviceDetails.Visibility = Visibility.Collapsed;
                    uiConnectionControl.Visibility = Visibility.Visible;
                    uiBack.IsEnabled = DetailsAlwaysShown ? false : true;
                    break;
                case DetailPane.DeviceDetails:
                    uiAdvertisementList.Visibility = DetailsAlwaysShown ? Visibility.Visible : Visibility.Collapsed; ;
                    uiDetailsPane.Visibility = Visibility.Visible;
                    uiAdvertisementDetails.Visibility = Visibility.Collapsed;
                    uiDeviceDetails.Visibility = Visibility.Visible;
                    uiConnectionControl.Visibility = Visibility.Visible;
                    uiBack.IsEnabled = true;
                    break;
            }

            Grid.SetColumn(uiDetailsPane, DetailsAlwaysShown ? 1 : 0);
        }

        WatcherData CurrWatcherData = null;
        private void OnAdvertisementSelected(ItemsView sender, ItemsViewSelectionChangedEventArgs args)
        {
            var data = sender.SelectedItem as WatcherData;
            if (data == null) return;
            Log($"OnAdvertisementSelected: selected={data.AddressAsString} name={data.BestName}");
            if ( CurrWatcherData != null && CurrWatcherData.Addr == data.Addr)
            {
                return;
            }
            CurrWatcherData = data;
            var details = data.ToStringDetails();
            uiAdvertisementDetailsTextBlock.Text = details;
            ShowDetail(DetailPane.AdvertisementDetails);
            uiConnectionControl.SetAdvertisementData(data);
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

        private async Task DoConnected(BluetoothLEDevice le)
        {
            BluetoothCacheMode cacheMode = BluetoothCacheMode.Cached;

            
            //Log($"Starting connection to {CurrWatcherData.AddressAsString}");
            var addr = CurrWatcherData.Addr;
            //var le = await BluetoothLEDevice.FromBluetoothAddressAsync(addr);
            //if (le == null)
            //{
            //    Log($"Unable to connect to {CurrWatcherData.AddressAsString}");
            //    return;
            //}
            var services = await le.GetGattServicesAsync(cacheMode);
            if (services.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                DeviceDetailsLog($"Unable to get services for {CurrWatcherData.AddressAsString}. Reason: {services.Status}");
                return;
            }
            uiDeviceDetailsTextBlock.Text = $"Services for {CurrWatcherData.AddressAsString} {CurrWatcherData.BestName}\n\n";

            var nameDeviceList = new NameAllBleDevices();
            var nameDevice = new NameDevice();
            nameDevice.Name = le.Name;
            nameDevice.Details += "TODO: line 190";
            nameDeviceList.AllDevices.Add(nameDevice);
            // TODO: skipping copying classModifiers ClassName Description from knownDevice
            int serviceCount = 0;

            foreach (var service in services.Services)
            {
                var nameService = new NameService(service, null, serviceCount++);
                nameDevice.Services.Add(nameService);

                var shortuuid = BluetoothUuidHelper.TryGetShortId(service.Uuid);
                var serviceUuidStr = (shortuuid != null) ? $"{shortuuid:X4}" : service.Uuid.ToString();
                var servicename = (shortuuid != null) ? BluetoothServiceUuid16Bit.Decode((ushort)shortuuid) + " " : "";

                var servicesb = new StringBuilder();
                servicesb.AppendLine($"Service {servicename}Uuid={serviceUuidStr}  handle={service.AttributeHandle}");
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
                    DeviceDetailsLog ($"    Unable to get characteristics reason={chresult.Status} {chresult.ProtocolError}");
                }
                else
                {
                    int characteristicCount = 0;

                    foreach (var characteristic in chresult.Characteristics)
                    {
                        var chshortuuid = BluetoothUuidHelper.TryGetShortId(characteristic.Uuid);
                        var chUuidStr = (chshortuuid != null) ? $"{chshortuuid:X4}" : characteristic.Uuid.ToString();
                        var chname = (chshortuuid != null) ? $"name={BluetoothCharacteristic.Decode((ushort)chshortuuid)} " : "";


                        var nameCharacteristic = new NameCharacteristic(characteristic, null, null, characteristicCount++);
                        nameService.Characteristics.Add(nameCharacteristic);

                        var chsb = new StringBuilder();
                        chsb.AppendLine($"    Characteristic {chname}Uuid={chUuidStr} handle={characteristic.AttributeHandle}");
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
                                var (str, readstatus) = DataReaderReadStringRobust.ReadStringEntire(dr);

                                nameCharacteristic.ExampleData.Add(str);

                                chsb.AppendLine($"        Read: {str}");
                            }
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
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                TypeInfoResolver = resolver,
            };
            //var JsonAsList = System.Text.Json.JsonSerializer.Serialize(nameDeviceList, jsonOptions); // , jsonFormat, jsonSettings);
            var node = BluetoothWinUI3.SystemTextJsonCleaner.ToJsonNode(nameDeviceList);
            var JsonAsList = node?.ToJsonString(jsonOptions) ?? "";

            uiDeviceDetailsTextBlock.Text += $"\n\n\n" + JsonAsList;


        }

        private async void OnConnectClicked(object sender, RoutedEventArgs e)
        {
            BluetoothCacheMode cacheMode = BluetoothCacheMode.Cached;

            if (CurrWatcherData == null || CurrWatcherData.Addr == 0)
            {
                Log($"Error: no device selected to connect");
                return;
            }
            ShowDetail(DetailPane.DeviceDetails);
            Log($"Starting connection to {CurrWatcherData.AddressAsString}");
            var addr = CurrWatcherData.Addr;
            var le = await BluetoothLEDevice.FromBluetoothAddressAsync(addr);
            if (le == null)
            {
                Log($"Unable to connect to {CurrWatcherData.AddressAsString}");
                return;
            }
            var services = await le.GetGattServicesAsync(cacheMode);
            if (services.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                Log($"Unable to get services for {CurrWatcherData.AddressAsString}. Reason: {services.Status}");
                return;
            }
            uiDeviceDetailsTextBlock.Text = $"Services for {CurrWatcherData.AddressAsString} {CurrWatcherData.BestName}\n\n";

            var nameDeviceList = new NameAllBleDevices();
            var nameDevice = new NameDevice();
            nameDevice.Name = le.Name;
            nameDevice.Details += "TODO: line 190";
            nameDeviceList.AllDevices.Add(nameDevice);
            // TODO: skipping copying classModifiers ClassName Description from knownDevice
            int serviceCount = 0;

            foreach (var service in services.Services)
            {
                var nameService = new NameService(service, null, serviceCount++);
                nameDevice.Services.Add(nameService);

                var shortuuid = BluetoothUuidHelper.TryGetShortId(service.Uuid);
                var serviceUuidStr = (shortuuid != null) ? $"{shortuuid:X4}" : service.Uuid.ToString();
                var servicename = (shortuuid != null) ? BluetoothServiceUuid16Bit.Decode((ushort)shortuuid) + " " : "";

                var servicesb = new StringBuilder();
                servicesb.AppendLine($"Service {servicename}Uuid={serviceUuidStr}  handle={service.AttributeHandle}");
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
                    uiDeviceDetailsTextBlock.Text += $"    Unable to get characteristics reason={chresult.Status} {chresult.ProtocolError}";
                }
                else
                {
                    int characteristicCount = 0;

                    foreach (var characteristic in chresult.Characteristics)
                    {
                        var chshortuuid = BluetoothUuidHelper.TryGetShortId(characteristic.Uuid);
                        var chUuidStr = (chshortuuid != null) ? $"{chshortuuid:X4}" : characteristic.Uuid.ToString();
                        var chname = (chshortuuid != null) ? $"name={BluetoothCharacteristic.Decode((ushort)chshortuuid)} " : "";


                        var nameCharacteristic = new NameCharacteristic(characteristic, null, null, characteristicCount++);
                        nameService.Characteristics.Add(nameCharacteristic);

                        var chsb = new StringBuilder();
                        chsb.AppendLine($"    Characteristic {chname}Uuid={chUuidStr} handle={characteristic.AttributeHandle}");
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
                                var (str, readstatus) = DataReaderReadStringRobust.ReadStringEntire(dr);

                                nameCharacteristic.ExampleData.Add(str);

                                chsb.AppendLine($"        Read: {str}");
                            }
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
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                TypeInfoResolver = resolver,
            };
            //var JsonAsList = System.Text.Json.JsonSerializer.Serialize(nameDeviceList, jsonOptions); // , jsonFormat, jsonSettings);
            var node = BluetoothWinUI3.SystemTextJsonCleaner.ToJsonNode(nameDeviceList);
            var JsonAsList = node?.ToJsonString(jsonOptions) ?? "";

            uiDeviceDetailsTextBlock.Text += $"\n\n\n" + JsonAsList;

        }
    }
}
