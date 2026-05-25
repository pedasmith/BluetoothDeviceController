using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Text;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

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
                    uiConnect.IsEnabled = false;
                    uiBack.IsEnabled = false;
                    break;
                case DetailPane.AdvertisementDetails:
                    uiAdvertisementList.Visibility = DetailsAlwaysShown ? Visibility.Visible : Visibility.Collapsed; ;
                    uiDetailsPane.Visibility = Visibility.Visible;
                    uiAdvertisementDetails.Visibility = Visibility.Visible;
                    uiDeviceDetails.Visibility = Visibility.Collapsed;
                    uiConnect.IsEnabled = true;
                    uiBack.IsEnabled = DetailsAlwaysShown ? false : true;
                    break;
                case DetailPane.DeviceDetails:
                    uiAdvertisementList.Visibility = DetailsAlwaysShown ? Visibility.Visible : Visibility.Collapsed; ;
                    uiDetailsPane.Visibility = Visibility.Visible;
                    uiAdvertisementDetails.Visibility = Visibility.Collapsed;
                    uiDeviceDetails.Visibility = Visibility.Visible;
                    uiConnect.IsEnabled = true;
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
            CurrWatcherData = data;
            var details = data.ToStringDetails();
            uiAdvertisementDetailsTextBlock.Text = details;
            ShowDetail(DetailPane.AdvertisementDetails);
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
            uiDeviceDetailsTextBlock.Text = $"Services for {CurrWatcherData.AddressAsString} {CurrWatcherData.CompleteLocalName}\n\n";
            foreach (var service in services.Services)
            {
                var servicesb = new StringBuilder();
                servicesb.AppendLine($"Service Uuid={service.Uuid}  handle={service.AttributeHandle}");
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
                    foreach (var characteristic in chresult.Characteristics)
                    {
                        var chsb = new StringBuilder();
                        chsb.AppendLine($"    Characteristic Uuid={characteristic.Uuid} handle={characteristic.AttributeHandle}");
                        uiDeviceDetailsTextBlock.Text += chsb.ToString();

                    }
                }

                uiDeviceDetailsTextBlock.Text += $"\n";
            }
        }
    }
}
