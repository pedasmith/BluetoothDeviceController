using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Names;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[NAME]] device
    /// </summary>
    public sealed partial class Nordic_ThingyPage : Page, HasId, ISetHandleStatus
    {
        public Nordic_ThingyPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Nordic_Thingy";
        private string DeviceNameUser = "Thingy";

        int ncommand = 0;
        Nordic_Thingy bleDevice = new Nordic_Thingy();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformation;
            var ble = await BluetoothLEDevice.FromIdAsync(di.Id);
            SetStatusActive(false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            DoReadDevice_Name();
        }

        public string GetId()
        {
            return bleDevice?.ble?.DeviceId ?? "";
        }

        public string GetPicturePath()
        {
            return $"/Assets/DevicePictures/{DeviceName}-175.PNG";
        }

        public string GetDeviceNameUser()
        {
            return $"{DeviceNameUser}";
        }

        private IHandleStatus ParentStatusHandler = null;

        public void SetHandleStatus(IHandleStatus handleStatus)
        {
            ParentStatusHandler = handleStatus;
        }

        private void SetStatus(string status)
        {
            uiStatus.Text = status;
            ParentStatusHandler?.SetStatusText(status);
        }
        private void SetStatusActive(bool isActive)
        {
            uiProgress.IsActive = isActive;
            ParentStatusHandler?.SetStatusActive(isActive);
        }

        private async void bleDevice_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
        {
            var now = DateTime.Now;
            var nowstr = $"{now.Hour:D2}:{now.Minute:D2}:{now.Second:D2}.{now.Millisecond:D03}";
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                SetStatus(nowstr + ": " + status.AsStatusString);
                SetStatusActive(false);
            });
        }

        // Functions for Common Configuration


        public class Device_NameRecord : INotifyPropertyChanged
        {
            public Device_NameRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _Device_Name;
            public string Device_Name { get { return _Device_Name; } set { if (value == _Device_Name) return; _Device_Name = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Device_NameRecord> Device_NameRecordData { get; } = new DataCollection<Device_NameRecord>();
        private void OnDevice_Name_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Device_NameRecordData.Count == 0)
                {
                    Device_NameRecordData.AddRecord(new Device_NameRecord());
                }
                Device_NameRecordData[Device_NameRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDevice_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Device_NameRecordData.MaxLength = value;


        }

        private void OnAlgorithmDevice_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Device_NameRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDevice_Name(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Device_Name,Notes\n");
            foreach (var row in Device_NameRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Device_Name},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        public void DoReadDevice_Name()
        {
            OnReadDevice_Name(null, null);
        }

        private async void OnReadDevice_Name(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDevice_Name();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Device_Name");
                    return;
                }

                var record = new Device_NameRecord();

                var Device_Name = valueList.GetValue("Device_Name");
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Device_Name = (string)Device_Name.AsString;
                    Device_Name_Device_Name.Text = record.Device_Name.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Device_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class AppearanceRecord : INotifyPropertyChanged
        {
            public AppearanceRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Appearance;
            public double Appearance { get { return _Appearance; } set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<AppearanceRecord> AppearanceRecordData { get; } = new DataCollection<AppearanceRecord>();
        private void OnAppearance_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (AppearanceRecordData.Count == 0)
                {
                    AppearanceRecordData.AddRecord(new AppearanceRecord());
                }
                AppearanceRecordData[AppearanceRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAppearance(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AppearanceRecordData.MaxLength = value;


        }

        private void OnAlgorithmAppearance(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AppearanceRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAppearance(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Appearance,Notes\n");
            foreach (var row in AppearanceRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Appearance},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAppearance(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAppearance();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Appearance");
                    return;
                }

                var record = new AppearanceRecord();

                var Appearance = valueList.GetValue("Appearance");
                if (Appearance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Appearance.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Appearance = (double)Appearance.AsDouble;
                    Appearance_Appearance.Text = record.Appearance.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                AppearanceRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Connection_ParameterRecord : INotifyPropertyChanged
        {
            public Connection_ParameterRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _ConnectionParameter;
            public string ConnectionParameter { get { return _ConnectionParameter; } set { if (value == _ConnectionParameter) return; _ConnectionParameter = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Connection_ParameterRecord> Connection_ParameterRecordData { get; } = new DataCollection<Connection_ParameterRecord>();
        private void OnConnection_Parameter_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Connection_ParameterRecordData.Count == 0)
                {
                    Connection_ParameterRecordData.AddRecord(new Connection_ParameterRecord());
                }
                Connection_ParameterRecordData[Connection_ParameterRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountConnection_Parameter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Connection_ParameterRecordData.MaxLength = value;


        }

        private void OnAlgorithmConnection_Parameter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Connection_ParameterRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyConnection_Parameter(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,ConnectionParameter,Notes\n");
            foreach (var row in Connection_ParameterRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ConnectionParameter},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadConnection_Parameter(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadConnection_Parameter();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Connection_Parameter");
                    return;
                }

                var record = new Connection_ParameterRecord();

                var ConnectionParameter = valueList.GetValue("ConnectionParameter");
                if (ConnectionParameter.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ConnectionParameter.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ConnectionParameter = (string)ConnectionParameter.AsString;
                    Connection_Parameter_ConnectionParameter.Text = record.ConnectionParameter.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Connection_ParameterRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Central_Address_ResolutionRecord : INotifyPropertyChanged
        {
            public Central_Address_ResolutionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _AddressResolutionSupported;
            public double AddressResolutionSupported { get { return _AddressResolutionSupported; } set { if (value == _AddressResolutionSupported) return; _AddressResolutionSupported = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Central_Address_ResolutionRecord> Central_Address_ResolutionRecordData { get; } = new DataCollection<Central_Address_ResolutionRecord>();
        private void OnCentral_Address_Resolution_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Central_Address_ResolutionRecordData.Count == 0)
                {
                    Central_Address_ResolutionRecordData.AddRecord(new Central_Address_ResolutionRecord());
                }
                Central_Address_ResolutionRecordData[Central_Address_ResolutionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountCentral_Address_Resolution(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Central_Address_ResolutionRecordData.MaxLength = value;


        }

        private void OnAlgorithmCentral_Address_Resolution(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Central_Address_ResolutionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyCentral_Address_Resolution(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,AddressResolutionSupported,Notes\n");
            foreach (var row in Central_Address_ResolutionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AddressResolutionSupported},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadCentral_Address_Resolution(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadCentral_Address_Resolution();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Central_Address_Resolution");
                    return;
                }

                var record = new Central_Address_ResolutionRecord();

                var AddressResolutionSupported = valueList.GetValue("AddressResolutionSupported");
                if (AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AddressResolutionSupported = (double)AddressResolutionSupported.AsDouble;
                    Central_Address_Resolution_AddressResolutionSupported.Text = record.AddressResolutionSupported.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Central_Address_ResolutionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Generic Service



        // Functions for Battery


        public class BatteryLevelRecord : INotifyPropertyChanged
        {
            public BatteryLevelRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _BatteryLevel;
            public double BatteryLevel { get { return _BatteryLevel; } set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<BatteryLevelRecord> BatteryLevelRecordData { get; } = new DataCollection<BatteryLevelRecord>();
        private void OnBatteryLevel_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (BatteryLevelRecordData.Count == 0)
                {
                    BatteryLevelRecordData.AddRecord(new BatteryLevelRecord());
                }
                BatteryLevelRecordData[BatteryLevelRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountBatteryLevel(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            BatteryLevelRecordData.MaxLength = value;


        }

        private void OnAlgorithmBatteryLevel(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            BatteryLevelRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyBatteryLevel(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,BatteryLevel,Notes\n");
            foreach (var row in BatteryLevelRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.BatteryLevel},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadBatteryLevel(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadBatteryLevel();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read BatteryLevel");
                    return;
                }

                var record = new BatteryLevelRecord();

                var BatteryLevel = valueList.GetValue("BatteryLevel");
                if (BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.BatteryLevel = (double)BatteryLevel.AsDouble;
                    BatteryLevel_BatteryLevel.Text = record.BatteryLevel.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                BatteryLevelRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyBatteryLevelSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int BatteryLevelNotifyIndex = 0;
        bool BatteryLevelNotifySetup = false;
        private async void OnNotifyBatteryLevel(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!BatteryLevelNotifySetup)
                {
                    BatteryLevelNotifySetup = true;
                    bleDevice.BatteryLevelEvent += BleDevice_BatteryLevelEvent;
                }
                var notifyType = NotifyBatteryLevelSettings[BatteryLevelNotifyIndex];
                BatteryLevelNotifyIndex = (BatteryLevelNotifyIndex + 1) % NotifyBatteryLevelSettings.Length;
                var result = await bleDevice.NotifyBatteryLevelAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_BatteryLevelEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new BatteryLevelRecord();

                    var BatteryLevel = valueList.GetValue("BatteryLevel");
                    if (BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.BatteryLevel = (double)BatteryLevel.AsDouble;
                        BatteryLevel_BatteryLevel.Text = record.BatteryLevel.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = BatteryLevelRecordData.AddRecord(record);

                });
            }
        }

        // Functions for Configuration


        private async void OnWriteConfiguration_Device_Name(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                String Name;
                var parsedName = Utilities.Parsers.TryParseString(Configuration_Device_Name_Name.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Name);
                if (!parsedName)
                {
                    parseError = "Name";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteConfiguration_Device_Name(Name);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Configuration_Device_NameRecord : INotifyPropertyChanged
        {
            public Configuration_Device_NameRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _Name;
            public string Name { get { return _Name; } set { if (value == _Name) return; _Name = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Configuration_Device_NameRecord> Configuration_Device_NameRecordData { get; } = new DataCollection<Configuration_Device_NameRecord>();
        private void OnConfiguration_Device_Name_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Configuration_Device_NameRecordData.Count == 0)
                {
                    Configuration_Device_NameRecordData.AddRecord(new Configuration_Device_NameRecord());
                }
                Configuration_Device_NameRecordData[Configuration_Device_NameRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountConfiguration_Device_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Configuration_Device_NameRecordData.MaxLength = value;


        }

        private void OnAlgorithmConfiguration_Device_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Configuration_Device_NameRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyConfiguration_Device_Name(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Name,Notes\n");
            foreach (var row in Configuration_Device_NameRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Name},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadConfiguration_Device_Name(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadConfiguration_Device_Name();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Configuration_Device_Name");
                    return;
                }

                var record = new Configuration_Device_NameRecord();

                var Name = valueList.GetValue("Name");
                if (Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Name.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Name = (string)Name.AsString;
                    Configuration_Device_Name_Name.Text = record.Name.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Configuration_Device_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteAdvertising_Parameter(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 Interval;
                var parsedInterval = Utilities.Parsers.TryParseUInt16(Advertising_Parameter_Interval.Text, System.Globalization.NumberStyles.None, null, out Interval);
                if (!parsedInterval)
                {
                    parseError = "Interval";
                }

                Byte Timeout;
                var parsedTimeout = Utilities.Parsers.TryParseByte(Advertising_Parameter_Timeout.Text, System.Globalization.NumberStyles.None, null, out Timeout);
                if (!parsedTimeout)
                {
                    parseError = "Timeout";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAdvertising_Parameter(Interval, Timeout);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Advertising_ParameterRecord : INotifyPropertyChanged
        {
            public Advertising_ParameterRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Interval;
            public double Interval { get { return _Interval; } set { if (value == _Interval) return; _Interval = value; OnPropertyChanged(); } }

            private double _Timeout;
            public double Timeout { get { return _Timeout; } set { if (value == _Timeout) return; _Timeout = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Advertising_ParameterRecord> Advertising_ParameterRecordData { get; } = new DataCollection<Advertising_ParameterRecord>();
        private void OnAdvertising_Parameter_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Advertising_ParameterRecordData.Count == 0)
                {
                    Advertising_ParameterRecordData.AddRecord(new Advertising_ParameterRecord());
                }
                Advertising_ParameterRecordData[Advertising_ParameterRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAdvertising_Parameter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Advertising_ParameterRecordData.MaxLength = value;


        }

        private void OnAlgorithmAdvertising_Parameter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Advertising_ParameterRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAdvertising_Parameter(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Interval,Timeout,Notes\n");
            foreach (var row in Advertising_ParameterRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Interval},{row.Timeout},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAdvertising_Parameter(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAdvertising_Parameter();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Advertising_Parameter");
                    return;
                }

                var record = new Advertising_ParameterRecord();

                var Interval = valueList.GetValue("Interval");
                if (Interval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Interval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Interval = (double)Interval.AsDouble;
                    Advertising_Parameter_Interval.Text = record.Interval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Timeout = valueList.GetValue("Timeout");
                if (Timeout.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Timeout.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Timeout = (double)Timeout.AsDouble;
                    Advertising_Parameter_Timeout.Text = record.Timeout.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Advertising_ParameterRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteConnection_parameters(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 MinInterval;
                var parsedMinInterval = Utilities.Parsers.TryParseUInt16(Connection_parameters_MinInterval.Text, System.Globalization.NumberStyles.None, null, out MinInterval);
                if (!parsedMinInterval)
                {
                    parseError = "MinInterval";
                }

                UInt16 MaxInterval;
                var parsedMaxInterval = Utilities.Parsers.TryParseUInt16(Connection_parameters_MaxInterval.Text, System.Globalization.NumberStyles.None, null, out MaxInterval);
                if (!parsedMaxInterval)
                {
                    parseError = "MaxInterval";
                }

                UInt16 Latency;
                var parsedLatency = Utilities.Parsers.TryParseUInt16(Connection_parameters_Latency.Text, System.Globalization.NumberStyles.None, null, out Latency);
                if (!parsedLatency)
                {
                    parseError = "Latency";
                }

                UInt16 SupervisionTimeout;
                var parsedSupervisionTimeout = Utilities.Parsers.TryParseUInt16(Connection_parameters_SupervisionTimeout.Text, System.Globalization.NumberStyles.None, null, out SupervisionTimeout);
                if (!parsedSupervisionTimeout)
                {
                    parseError = "SupervisionTimeout";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteConnection_parameters(MinInterval, MaxInterval, Latency, SupervisionTimeout);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Connection_parametersRecord : INotifyPropertyChanged
        {
            public Connection_parametersRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _MinInterval;
            public double MinInterval { get { return _MinInterval; } set { if (value == _MinInterval) return; _MinInterval = value; OnPropertyChanged(); } }

            private double _MaxInterval;
            public double MaxInterval { get { return _MaxInterval; } set { if (value == _MaxInterval) return; _MaxInterval = value; OnPropertyChanged(); } }

            private double _Latency;
            public double Latency { get { return _Latency; } set { if (value == _Latency) return; _Latency = value; OnPropertyChanged(); } }

            private double _SupervisionTimeout;
            public double SupervisionTimeout { get { return _SupervisionTimeout; } set { if (value == _SupervisionTimeout) return; _SupervisionTimeout = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Connection_parametersRecord> Connection_parametersRecordData { get; } = new DataCollection<Connection_parametersRecord>();
        private void OnConnection_parameters_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Connection_parametersRecordData.Count == 0)
                {
                    Connection_parametersRecordData.AddRecord(new Connection_parametersRecord());
                }
                Connection_parametersRecordData[Connection_parametersRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountConnection_parameters(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Connection_parametersRecordData.MaxLength = value;


        }

        private void OnAlgorithmConnection_parameters(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Connection_parametersRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyConnection_parameters(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MinInterval,MaxInterval,Latency,SupervisionTimeout,Notes\n");
            foreach (var row in Connection_parametersRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MinInterval},{row.MaxInterval},{row.Latency},{row.SupervisionTimeout},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadConnection_parameters(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadConnection_parameters();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Connection_parameters");
                    return;
                }

                var record = new Connection_parametersRecord();

                var MinInterval = valueList.GetValue("MinInterval");
                if (MinInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MinInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MinInterval = (double)MinInterval.AsDouble;
                    Connection_parameters_MinInterval.Text = record.MinInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MaxInterval = valueList.GetValue("MaxInterval");
                if (MaxInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MaxInterval = (double)MaxInterval.AsDouble;
                    Connection_parameters_MaxInterval.Text = record.MaxInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Latency = valueList.GetValue("Latency");
                if (Latency.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Latency.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Latency = (double)Latency.AsDouble;
                    Connection_parameters_Latency.Text = record.Latency.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SupervisionTimeout = valueList.GetValue("SupervisionTimeout");
                if (SupervisionTimeout.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SupervisionTimeout.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SupervisionTimeout = (double)SupervisionTimeout.AsDouble;
                    Connection_parameters_SupervisionTimeout.Text = record.SupervisionTimeout.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Connection_parametersRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteEddystone_URL(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                String Eddystone;
                var parsedEddystone = Utilities.Parsers.TryParseString(Eddystone_URL_Eddystone.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Eddystone);
                if (!parsedEddystone)
                {
                    parseError = "Eddystone";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteEddystone_URL(Eddystone);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Eddystone_URLRecord : INotifyPropertyChanged
        {
            public Eddystone_URLRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _Eddystone;
            public string Eddystone { get { return _Eddystone; } set { if (value == _Eddystone) return; _Eddystone = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Eddystone_URLRecord> Eddystone_URLRecordData { get; } = new DataCollection<Eddystone_URLRecord>();
        private void OnEddystone_URL_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Eddystone_URLRecordData.Count == 0)
                {
                    Eddystone_URLRecordData.AddRecord(new Eddystone_URLRecord());
                }
                Eddystone_URLRecordData[Eddystone_URLRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountEddystone_URL(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Eddystone_URLRecordData.MaxLength = value;


        }

        private void OnAlgorithmEddystone_URL(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Eddystone_URLRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyEddystone_URL(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Eddystone,Notes\n");
            foreach (var row in Eddystone_URLRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Eddystone},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadEddystone_URL(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadEddystone_URL();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Eddystone_URL");
                    return;
                }

                var record = new Eddystone_URLRecord();

                var Eddystone = valueList.GetValue("Eddystone");
                if (Eddystone.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Eddystone.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Eddystone = (string)Eddystone.AsString;
                    Eddystone_URL_Eddystone.Text = record.Eddystone.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Eddystone_URLRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteCloud_Token(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes CloudToken;
                var parsedCloudToken = Utilities.Parsers.TryParseBytes(Cloud_Token_CloudToken.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out CloudToken);
                if (!parsedCloudToken)
                {
                    parseError = "CloudToken";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteCloud_Token(CloudToken);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Cloud_TokenRecord : INotifyPropertyChanged
        {
            public Cloud_TokenRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _CloudToken;
            public string CloudToken { get { return _CloudToken; } set { if (value == _CloudToken) return; _CloudToken = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Cloud_TokenRecord> Cloud_TokenRecordData { get; } = new DataCollection<Cloud_TokenRecord>();
        private void OnCloud_Token_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Cloud_TokenRecordData.Count == 0)
                {
                    Cloud_TokenRecordData.AddRecord(new Cloud_TokenRecord());
                }
                Cloud_TokenRecordData[Cloud_TokenRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountCloud_Token(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Cloud_TokenRecordData.MaxLength = value;


        }

        private void OnAlgorithmCloud_Token(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Cloud_TokenRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyCloud_Token(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,CloudToken,Notes\n");
            foreach (var row in Cloud_TokenRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.CloudToken},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadCloud_Token(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadCloud_Token();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Cloud_Token");
                    return;
                }

                var record = new Cloud_TokenRecord();

                var CloudToken = valueList.GetValue("CloudToken");
                if (CloudToken.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CloudToken.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.CloudToken = (string)CloudToken.AsString;
                    Cloud_Token_CloudToken.Text = record.CloudToken.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Cloud_TokenRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Firmware_VersionRecord : INotifyPropertyChanged
        {
            public Firmware_VersionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Major;
            public double Major { get { return _Major; } set { if (value == _Major) return; _Major = value; OnPropertyChanged(); } }

            private double _Minor;
            public double Minor { get { return _Minor; } set { if (value == _Minor) return; _Minor = value; OnPropertyChanged(); } }

            private double _Patch;
            public double Patch { get { return _Patch; } set { if (value == _Patch) return; _Patch = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Firmware_VersionRecord> Firmware_VersionRecordData { get; } = new DataCollection<Firmware_VersionRecord>();
        private void OnFirmware_Version_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Firmware_VersionRecordData.Count == 0)
                {
                    Firmware_VersionRecordData.AddRecord(new Firmware_VersionRecord());
                }
                Firmware_VersionRecordData[Firmware_VersionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountFirmware_Version(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Firmware_VersionRecordData.MaxLength = value;


        }

        private void OnAlgorithmFirmware_Version(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Firmware_VersionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyFirmware_Version(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Major,Minor,Patch,Notes\n");
            foreach (var row in Firmware_VersionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Major},{row.Minor},{row.Patch},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadFirmware_Version(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadFirmware_Version();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Firmware_Version");
                    return;
                }

                var record = new Firmware_VersionRecord();

                var Major = valueList.GetValue("Major");
                if (Major.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Major.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Major = (double)Major.AsDouble;
                    Firmware_Version_Major.Text = record.Major.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Minor = valueList.GetValue("Minor");
                if (Minor.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Minor.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Minor = (double)Minor.AsDouble;
                    Firmware_Version_Minor.Text = record.Minor.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Patch = valueList.GetValue("Patch");
                if (Patch.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Patch.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Patch = (double)Patch.AsDouble;
                    Firmware_Version_Patch.Text = record.Patch.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Firmware_VersionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteMTU_Request(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte param0;
                var parsedparam0 = Utilities.Parsers.TryParseByte(MTU_Request_param0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                UInt16 param1;
                var parsedparam1 = Utilities.Parsers.TryParseUInt16(MTU_Request_param1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param1);
                if (!parsedparam1)
                {
                    parseError = "param1";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMTU_Request(param0, param1);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class MTU_RequestRecord : INotifyPropertyChanged
        {
            public MTU_RequestRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private double _param1;
            public double param1 { get { return _param1; } set { if (value == _param1) return; _param1 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MTU_RequestRecord> MTU_RequestRecordData { get; } = new DataCollection<MTU_RequestRecord>();
        private void OnMTU_Request_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MTU_RequestRecordData.Count == 0)
                {
                    MTU_RequestRecordData.AddRecord(new MTU_RequestRecord());
                }
                MTU_RequestRecordData[MTU_RequestRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMTU_Request(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MTU_RequestRecordData.MaxLength = value;


        }

        private void OnAlgorithmMTU_Request(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MTU_RequestRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMTU_Request(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,param1,Notes\n");
            foreach (var row in MTU_RequestRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{row.param1},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMTU_Request(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMTU_Request();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MTU_Request");
                    return;
                }

                var record = new MTU_RequestRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (double)param0.AsDouble;
                    MTU_Request_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param1 = valueList.GetValue("param1");
                if (param1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param1 = (double)param1.AsDouble;
                    MTU_Request_param1.Text = record.param1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MTU_RequestRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Environment


        public class Temperature_cRecord : INotifyPropertyChanged
        {
            public Temperature_cRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Temperature;
            public double Temperature { get { return _Temperature; } set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Temperature_cRecord> Temperature_cRecordData { get; } = new DataCollection<Temperature_cRecord>();
        private void OnTemperature_c_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Temperature_cRecordData.Count == 0)
                {
                    Temperature_cRecordData.AddRecord(new Temperature_cRecord());
                }
                Temperature_cRecordData[Temperature_cRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTemperature_c(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_cRecordData.MaxLength = value;

            Temperature_cChart.RedrawYTime<Temperature_cRecord>(Temperature_cRecordData);
        }

        private void OnAlgorithmTemperature_c(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_cRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTemperature_c(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Temperature,Notes\n");
            foreach (var row in Temperature_cRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Temperature},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyTemperature_cSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Temperature_cNotifyIndex = 0;
        bool Temperature_cNotifySetup = false;
        private async void OnNotifyTemperature_c(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Temperature_cNotifySetup)
                {
                    Temperature_cNotifySetup = true;
                    bleDevice.Temperature_cEvent += BleDevice_Temperature_cEvent;
                }
                var notifyType = NotifyTemperature_cSettings[Temperature_cNotifyIndex];
                Temperature_cNotifyIndex = (Temperature_cNotifyIndex + 1) % NotifyTemperature_cSettings.Length;
                var result = await bleDevice.NotifyTemperature_cAsync(notifyType);



                var EventTimeProperty = typeof(Temperature_cRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(Temperature_cRecord).GetProperty("Temperature"),
                };
                var names = new List<string>()
                {
"Temperature",
                };
                Temperature_cChart.SetDataProperties(properties, EventTimeProperty, names);
                Temperature_cChart.SetTitle("Temperature (c) Chart");
                Temperature_cChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<Environment_ConfigurationRecord>(addResult, Environment_ConfigurationRecordData)",
                    chartDefaultMaxY = 100,
                    chartDefaultMinY = 0,
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Temperature_cEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Temperature_cRecord();

                    var Temperature = valueList.GetValue("Temperature");
                    if (Temperature.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Temperature.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Temperature = (double)Temperature.AsDouble;
                        Temperature_c_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Temperature_cRecordData.AddRecord(record);
                    Temperature_cChart.AddYTime<Temperature_cRecord>(addResult, Temperature_cRecordData);
                });
            }
        }
        public class Pressure_hpaRecord : INotifyPropertyChanged
        {
            public Pressure_hpaRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Pressure;
            public double Pressure { get { return _Pressure; } set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Pressure_hpaRecord> Pressure_hpaRecordData { get; } = new DataCollection<Pressure_hpaRecord>();
        private void OnPressure_hpa_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Pressure_hpaRecordData.Count == 0)
                {
                    Pressure_hpaRecordData.AddRecord(new Pressure_hpaRecord());
                }
                Pressure_hpaRecordData[Pressure_hpaRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPressure_hpa(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pressure_hpaRecordData.MaxLength = value;

            Pressure_hpaChart.RedrawYTime<Pressure_hpaRecord>(Pressure_hpaRecordData);
        }

        private void OnAlgorithmPressure_hpa(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pressure_hpaRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPressure_hpa(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Pressure,Notes\n");
            foreach (var row in Pressure_hpaRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Pressure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyPressure_hpaSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Pressure_hpaNotifyIndex = 0;
        bool Pressure_hpaNotifySetup = false;
        private async void OnNotifyPressure_hpa(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Pressure_hpaNotifySetup)
                {
                    Pressure_hpaNotifySetup = true;
                    bleDevice.Pressure_hpaEvent += BleDevice_Pressure_hpaEvent;
                }
                var notifyType = NotifyPressure_hpaSettings[Pressure_hpaNotifyIndex];
                Pressure_hpaNotifyIndex = (Pressure_hpaNotifyIndex + 1) % NotifyPressure_hpaSettings.Length;
                var result = await bleDevice.NotifyPressure_hpaAsync(notifyType);



                var EventTimeProperty = typeof(Pressure_hpaRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(Pressure_hpaRecord).GetProperty("Pressure"),
                };
                var names = new List<string>()
                {
"Pressure",
                };
                Pressure_hpaChart.SetDataProperties(properties, EventTimeProperty, names);
                Pressure_hpaChart.SetTitle("Pressure (hpa) Chart");
                Pressure_hpaChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<Environment_ConfigurationRecord>(addResult, Environment_ConfigurationRecordData)",
                    chartDefaultMaxY = 1043,
                    chartDefaultMinY = 983,
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Pressure_hpaEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Pressure_hpaRecord();

                    var Pressure = valueList.GetValue("Pressure");
                    if (Pressure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Pressure.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Pressure = (double)Pressure.AsDouble;
                        Pressure_hpa_Pressure.Text = record.Pressure.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Pressure_hpaRecordData.AddRecord(record);
                    Pressure_hpaChart.AddYTime<Pressure_hpaRecord>(addResult, Pressure_hpaRecordData);
                });
            }
        }
        public class HumidityRecord : INotifyPropertyChanged
        {
            public HumidityRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Humidity;
            public double Humidity { get { return _Humidity; } set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<HumidityRecord> HumidityRecordData { get; } = new DataCollection<HumidityRecord>();
        private void OnHumidity_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (HumidityRecordData.Count == 0)
                {
                    HumidityRecordData.AddRecord(new HumidityRecord());
                }
                HumidityRecordData[HumidityRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountHumidity(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            HumidityRecordData.MaxLength = value;

            HumidityChart.RedrawYTime<HumidityRecord>(HumidityRecordData);
        }

        private void OnAlgorithmHumidity(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            HumidityRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyHumidity(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Humidity,Notes\n");
            foreach (var row in HumidityRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Humidity},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyHumiditySettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int HumidityNotifyIndex = 0;
        bool HumidityNotifySetup = false;
        private async void OnNotifyHumidity(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!HumidityNotifySetup)
                {
                    HumidityNotifySetup = true;
                    bleDevice.HumidityEvent += BleDevice_HumidityEvent;
                }
                var notifyType = NotifyHumiditySettings[HumidityNotifyIndex];
                HumidityNotifyIndex = (HumidityNotifyIndex + 1) % NotifyHumiditySettings.Length;
                var result = await bleDevice.NotifyHumidityAsync(notifyType);



                var EventTimeProperty = typeof(HumidityRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(HumidityRecord).GetProperty("Humidity"),
                };
                var names = new List<string>()
                {
"Humidity",
                };
                HumidityChart.SetDataProperties(properties, EventTimeProperty, names);
                HumidityChart.SetTitle("Humidity (%) Chart");
                HumidityChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<Environment_ConfigurationRecord>(addResult, Environment_ConfigurationRecordData)",
                    chartDefaultMaxY = 100,
                    chartDefaultMinY = 0,
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_HumidityEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new HumidityRecord();

                    var Humidity = valueList.GetValue("Humidity");
                    if (Humidity.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Humidity.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Humidity = (double)Humidity.AsDouble;
                        Humidity_Humidity.Text = record.Humidity.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = HumidityRecordData.AddRecord(record);
                    HumidityChart.AddYTime<HumidityRecord>(addResult, HumidityRecordData);
                });
            }
        }
        public class Air_Quality_eCOS_TVOCRecord : INotifyPropertyChanged
        {
            public Air_Quality_eCOS_TVOCRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _eCOS;
            public double eCOS { get { return _eCOS; } set { if (value == _eCOS) return; _eCOS = value; OnPropertyChanged(); } }

            private double _TVOC;
            public double TVOC { get { return _TVOC; } set { if (value == _TVOC) return; _TVOC = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Air_Quality_eCOS_TVOCRecord> Air_Quality_eCOS_TVOCRecordData { get; } = new DataCollection<Air_Quality_eCOS_TVOCRecord>();
        private void OnAir_Quality_eCOS_TVOC_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Air_Quality_eCOS_TVOCRecordData.Count == 0)
                {
                    Air_Quality_eCOS_TVOCRecordData.AddRecord(new Air_Quality_eCOS_TVOCRecord());
                }
                Air_Quality_eCOS_TVOCRecordData[Air_Quality_eCOS_TVOCRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAir_Quality_eCOS_TVOC(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Air_Quality_eCOS_TVOCRecordData.MaxLength = value;

            Air_Quality_eCOS_TVOCChart.RedrawYTime<Air_Quality_eCOS_TVOCRecord>(Air_Quality_eCOS_TVOCRecordData);
        }

        private void OnAlgorithmAir_Quality_eCOS_TVOC(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Air_Quality_eCOS_TVOCRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAir_Quality_eCOS_TVOC(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,eCOS,TVOC,Notes\n");
            foreach (var row in Air_Quality_eCOS_TVOCRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.eCOS},{row.TVOC},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyAir_Quality_eCOS_TVOCSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Air_Quality_eCOS_TVOCNotifyIndex = 0;
        bool Air_Quality_eCOS_TVOCNotifySetup = false;
        private async void OnNotifyAir_Quality_eCOS_TVOC(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Air_Quality_eCOS_TVOCNotifySetup)
                {
                    Air_Quality_eCOS_TVOCNotifySetup = true;
                    bleDevice.Air_Quality_eCOS_TVOCEvent += BleDevice_Air_Quality_eCOS_TVOCEvent;
                }
                var notifyType = NotifyAir_Quality_eCOS_TVOCSettings[Air_Quality_eCOS_TVOCNotifyIndex];
                Air_Quality_eCOS_TVOCNotifyIndex = (Air_Quality_eCOS_TVOCNotifyIndex + 1) % NotifyAir_Quality_eCOS_TVOCSettings.Length;
                var result = await bleDevice.NotifyAir_Quality_eCOS_TVOCAsync(notifyType);



                var EventTimeProperty = typeof(Air_Quality_eCOS_TVOCRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(Air_Quality_eCOS_TVOCRecord).GetProperty("eCOS"),
typeof(Air_Quality_eCOS_TVOCRecord).GetProperty("TVOC"),
                };
                var names = new List<string>()
                {
"eCOS",
"TVOC",
                };
                Air_Quality_eCOS_TVOCChart.SetDataProperties(properties, EventTimeProperty, names);
                Air_Quality_eCOS_TVOCChart.SetTitle("Air Quality eCOS TVOC Chart");
                Air_Quality_eCOS_TVOCChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<Environment_ConfigurationRecord>(addResult, Environment_ConfigurationRecordData)",
                    chartLineDefaults ={
                        { "eCOS", new ChartLineDefaults() {
                            stroke="DarkGreen",
                            }
                        },
                        { "TVOC", new ChartLineDefaults() {
                            stroke="DarkBlue",
                            }
                        },
                    },
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Air_Quality_eCOS_TVOCEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Air_Quality_eCOS_TVOCRecord();

                    var eCOS = valueList.GetValue("eCOS");
                    if (eCOS.CurrentType == BCBasic.BCValue.ValueType.IsDouble || eCOS.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.eCOS = (double)eCOS.AsDouble;
                        Air_Quality_eCOS_TVOC_eCOS.Text = record.eCOS.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var TVOC = valueList.GetValue("TVOC");
                    if (TVOC.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TVOC.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.TVOC = (double)TVOC.AsDouble;
                        Air_Quality_eCOS_TVOC_TVOC.Text = record.TVOC.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Air_Quality_eCOS_TVOCRecordData.AddRecord(record);
                    Air_Quality_eCOS_TVOCChart.AddYTime<Air_Quality_eCOS_TVOCRecord>(addResult, Air_Quality_eCOS_TVOCRecordData);
                });
            }
        }
        public class Color_RGB_ClearRecord : INotifyPropertyChanged
        {
            public Color_RGB_ClearRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Red;
            public double Red { get { return _Red; } set { if (value == _Red) return; _Red = value; OnPropertyChanged(); } }

            private double _Green;
            public double Green { get { return _Green; } set { if (value == _Green) return; _Green = value; OnPropertyChanged(); } }

            private double _Blue;
            public double Blue { get { return _Blue; } set { if (value == _Blue) return; _Blue = value; OnPropertyChanged(); } }

            private double _Clear;
            public double Clear { get { return _Clear; } set { if (value == _Clear) return; _Clear = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Color_RGB_ClearRecord> Color_RGB_ClearRecordData { get; } = new DataCollection<Color_RGB_ClearRecord>();
        private void OnColor_RGB_Clear_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Color_RGB_ClearRecordData.Count == 0)
                {
                    Color_RGB_ClearRecordData.AddRecord(new Color_RGB_ClearRecord());
                }
                Color_RGB_ClearRecordData[Color_RGB_ClearRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountColor_RGB_Clear(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Color_RGB_ClearRecordData.MaxLength = value;

            Color_RGB_ClearChart.RedrawYTime<Color_RGB_ClearRecord>(Color_RGB_ClearRecordData);
        }

        private void OnAlgorithmColor_RGB_Clear(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Color_RGB_ClearRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyColor_RGB_Clear(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Red,Green,Blue,Clear,Notes\n");
            foreach (var row in Color_RGB_ClearRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Red},{row.Green},{row.Blue},{row.Clear},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyColor_RGB_ClearSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Color_RGB_ClearNotifyIndex = 0;
        bool Color_RGB_ClearNotifySetup = false;
        private async void OnNotifyColor_RGB_Clear(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Color_RGB_ClearNotifySetup)
                {
                    Color_RGB_ClearNotifySetup = true;
                    bleDevice.Color_RGB_ClearEvent += BleDevice_Color_RGB_ClearEvent;
                }
                var notifyType = NotifyColor_RGB_ClearSettings[Color_RGB_ClearNotifyIndex];
                Color_RGB_ClearNotifyIndex = (Color_RGB_ClearNotifyIndex + 1) % NotifyColor_RGB_ClearSettings.Length;
                var result = await bleDevice.NotifyColor_RGB_ClearAsync(notifyType);



                var EventTimeProperty = typeof(Color_RGB_ClearRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(Color_RGB_ClearRecord).GetProperty("Red"),
typeof(Color_RGB_ClearRecord).GetProperty("Green"),
typeof(Color_RGB_ClearRecord).GetProperty("Blue"),
typeof(Color_RGB_ClearRecord).GetProperty("Clear"),
                };
                var names = new List<string>()
                {
"Red",
"Green",
"Blue",
"Clear",
                };
                Color_RGB_ClearChart.SetDataProperties(properties, EventTimeProperty, names);
                Color_RGB_ClearChart.SetTitle("Color RGB+Clear Chart");
                Color_RGB_ClearChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<Environment_ConfigurationRecord>(addResult, Environment_ConfigurationRecordData)",
                    chartDefaultMaxY = 10000,
                    chartMinY = 0,
                    chartLineDefaults ={
                        { "Red", new ChartLineDefaults() {
                            stroke="DarkRed",
                            }
                        },
                        { "Green", new ChartLineDefaults() {
                            stroke="DarkGreen",
                            }
                        },
                        { "Blue", new ChartLineDefaults() {
                            stroke="DarkBlue",
                            }
                        },
                        { "Clear", new ChartLineDefaults() {
                            stroke="Black",
                            }
                        },
                    },
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Color_RGB_ClearEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Color_RGB_ClearRecord();

                    var Red = valueList.GetValue("Red");
                    if (Red.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Red.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Red = (double)Red.AsDouble;
                        Color_RGB_Clear_Red.Text = record.Red.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Green = valueList.GetValue("Green");
                    if (Green.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Green.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Green = (double)Green.AsDouble;
                        Color_RGB_Clear_Green.Text = record.Green.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Blue = valueList.GetValue("Blue");
                    if (Blue.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Blue.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Blue = (double)Blue.AsDouble;
                        Color_RGB_Clear_Blue.Text = record.Blue.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Clear = valueList.GetValue("Clear");
                    if (Clear.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Clear.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Clear = (double)Clear.AsDouble;
                        Color_RGB_Clear_Clear.Text = record.Clear.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Color_RGB_ClearRecordData.AddRecord(record);
                    Color_RGB_ClearChart.AddYTime<Color_RGB_ClearRecord>(addResult, Color_RGB_ClearRecordData);
                });
            }
        }
        private async void OnWriteEnvironment_Configuration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 TempInterval;
                var parsedTempInterval = Utilities.Parsers.TryParseUInt16(Environment_Configuration_TempInterval.Text, System.Globalization.NumberStyles.None, null, out TempInterval);
                if (!parsedTempInterval)
                {
                    parseError = "TempInterval";
                }

                UInt16 PressureInterval;
                var parsedPressureInterval = Utilities.Parsers.TryParseUInt16(Environment_Configuration_PressureInterval.Text, System.Globalization.NumberStyles.None, null, out PressureInterval);
                if (!parsedPressureInterval)
                {
                    parseError = "PressureInterval";
                }

                UInt16 HumidityInterval;
                var parsedHumidityInterval = Utilities.Parsers.TryParseUInt16(Environment_Configuration_HumidityInterval.Text, System.Globalization.NumberStyles.None, null, out HumidityInterval);
                if (!parsedHumidityInterval)
                {
                    parseError = "HumidityInterval";
                }

                UInt16 ColorInterval;
                var parsedColorInterval = Utilities.Parsers.TryParseUInt16(Environment_Configuration_ColorInterval.Text, System.Globalization.NumberStyles.None, null, out ColorInterval);
                if (!parsedColorInterval)
                {
                    parseError = "ColorInterval";
                }

                Byte GasMode;
                var parsedGasMode = Utilities.Parsers.TryParseByte(Environment_Configuration_GasMode.Text, System.Globalization.NumberStyles.None, null, out GasMode);
                if (!parsedGasMode)
                {
                    parseError = "GasMode";
                }

                Byte RedCalibration;
                var parsedRedCalibration = Utilities.Parsers.TryParseByte(Environment_Configuration_RedCalibration.Text, System.Globalization.NumberStyles.None, null, out RedCalibration);
                if (!parsedRedCalibration)
                {
                    parseError = "RedCalibration";
                }

                Byte GreenCalibration;
                var parsedGreenCalibration = Utilities.Parsers.TryParseByte(Environment_Configuration_GreenCalibration.Text, System.Globalization.NumberStyles.None, null, out GreenCalibration);
                if (!parsedGreenCalibration)
                {
                    parseError = "GreenCalibration";
                }

                Byte BlueCalibration;
                var parsedBlueCalibration = Utilities.Parsers.TryParseByte(Environment_Configuration_BlueCalibration.Text, System.Globalization.NumberStyles.None, null, out BlueCalibration);
                if (!parsedBlueCalibration)
                {
                    parseError = "BlueCalibration";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteEnvironment_Configuration(TempInterval, PressureInterval, HumidityInterval, ColorInterval, GasMode, RedCalibration, GreenCalibration, BlueCalibration);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Environment_ConfigurationRecord : INotifyPropertyChanged
        {
            public Environment_ConfigurationRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _TempInterval;
            public double TempInterval { get { return _TempInterval; } set { if (value == _TempInterval) return; _TempInterval = value; OnPropertyChanged(); } }

            private double _PressureInterval;
            public double PressureInterval { get { return _PressureInterval; } set { if (value == _PressureInterval) return; _PressureInterval = value; OnPropertyChanged(); } }

            private double _HumidityInterval;
            public double HumidityInterval { get { return _HumidityInterval; } set { if (value == _HumidityInterval) return; _HumidityInterval = value; OnPropertyChanged(); } }

            private double _ColorInterval;
            public double ColorInterval { get { return _ColorInterval; } set { if (value == _ColorInterval) return; _ColorInterval = value; OnPropertyChanged(); } }

            private double _GasMode;
            public double GasMode { get { return _GasMode; } set { if (value == _GasMode) return; _GasMode = value; OnPropertyChanged(); } }

            private double _RedCalibration;
            public double RedCalibration { get { return _RedCalibration; } set { if (value == _RedCalibration) return; _RedCalibration = value; OnPropertyChanged(); } }

            private double _GreenCalibration;
            public double GreenCalibration { get { return _GreenCalibration; } set { if (value == _GreenCalibration) return; _GreenCalibration = value; OnPropertyChanged(); } }

            private double _BlueCalibration;
            public double BlueCalibration { get { return _BlueCalibration; } set { if (value == _BlueCalibration) return; _BlueCalibration = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Environment_ConfigurationRecord> Environment_ConfigurationRecordData { get; } = new DataCollection<Environment_ConfigurationRecord>();
        private void OnEnvironment_Configuration_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Environment_ConfigurationRecordData.Count == 0)
                {
                    Environment_ConfigurationRecordData.AddRecord(new Environment_ConfigurationRecord());
                }
                Environment_ConfigurationRecordData[Environment_ConfigurationRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountEnvironment_Configuration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Environment_ConfigurationRecordData.MaxLength = value;


        }

        private void OnAlgorithmEnvironment_Configuration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Environment_ConfigurationRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyEnvironment_Configuration(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,TempInterval,PressureInterval,HumidityInterval,ColorInterval,GasMode,RedCalibration,GreenCalibration,BlueCalibration,Notes\n");
            foreach (var row in Environment_ConfigurationRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.TempInterval},{row.PressureInterval},{row.HumidityInterval},{row.ColorInterval},{row.GasMode},{row.RedCalibration},{row.GreenCalibration},{row.BlueCalibration},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadEnvironment_Configuration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadEnvironment_Configuration();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Environment_Configuration");
                    return;
                }

                var record = new Environment_ConfigurationRecord();

                var TempInterval = valueList.GetValue("TempInterval");
                if (TempInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TempInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TempInterval = (double)TempInterval.AsDouble;
                    Environment_Configuration_TempInterval.Text = record.TempInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PressureInterval = valueList.GetValue("PressureInterval");
                if (PressureInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PressureInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PressureInterval = (double)PressureInterval.AsDouble;
                    Environment_Configuration_PressureInterval.Text = record.PressureInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var HumidityInterval = valueList.GetValue("HumidityInterval");
                if (HumidityInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || HumidityInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.HumidityInterval = (double)HumidityInterval.AsDouble;
                    Environment_Configuration_HumidityInterval.Text = record.HumidityInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var ColorInterval = valueList.GetValue("ColorInterval");
                if (ColorInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ColorInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ColorInterval = (double)ColorInterval.AsDouble;
                    Environment_Configuration_ColorInterval.Text = record.ColorInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var GasMode = valueList.GetValue("GasMode");
                if (GasMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GasMode.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.GasMode = (double)GasMode.AsDouble;
                    Environment_Configuration_GasMode.Text = record.GasMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var RedCalibration = valueList.GetValue("RedCalibration");
                if (RedCalibration.CurrentType == BCBasic.BCValue.ValueType.IsDouble || RedCalibration.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.RedCalibration = (double)RedCalibration.AsDouble;
                    Environment_Configuration_RedCalibration.Text = record.RedCalibration.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var GreenCalibration = valueList.GetValue("GreenCalibration");
                if (GreenCalibration.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GreenCalibration.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.GreenCalibration = (double)GreenCalibration.AsDouble;
                    Environment_Configuration_GreenCalibration.Text = record.GreenCalibration.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var BlueCalibration = valueList.GetValue("BlueCalibration");
                if (BlueCalibration.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BlueCalibration.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.BlueCalibration = (double)BlueCalibration.AsDouble;
                    Environment_Configuration_BlueCalibration.Text = record.BlueCalibration.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Environment_ConfigurationRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for UI


        private async void OnWriteLED_Characteristics(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte param0;
                var parsedparam0 = Utilities.Parsers.TryParseByte(LED_Characteristics_param0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                Byte param1;
                var parsedparam1 = Utilities.Parsers.TryParseByte(LED_Characteristics_param1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param1);
                if (!parsedparam1)
                {
                    parseError = "param1";
                }

                Byte param2;
                var parsedparam2 = Utilities.Parsers.TryParseByte(LED_Characteristics_param2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param2);
                if (!parsedparam2)
                {
                    parseError = "param2";
                }

                Byte param3;
                var parsedparam3 = Utilities.Parsers.TryParseByte(LED_Characteristics_param3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param3);
                if (!parsedparam3)
                {
                    parseError = "param3";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteLED_Characteristics(param0, param1, param2, param3);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class LED_CharacteristicsRecord : INotifyPropertyChanged
        {
            public LED_CharacteristicsRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private double _param1;
            public double param1 { get { return _param1; } set { if (value == _param1) return; _param1 = value; OnPropertyChanged(); } }

            private double _param2;
            public double param2 { get { return _param2; } set { if (value == _param2) return; _param2 = value; OnPropertyChanged(); } }

            private double _param3;
            public double param3 { get { return _param3; } set { if (value == _param3) return; _param3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<LED_CharacteristicsRecord> LED_CharacteristicsRecordData { get; } = new DataCollection<LED_CharacteristicsRecord>();
        private void OnLED_Characteristics_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (LED_CharacteristicsRecordData.Count == 0)
                {
                    LED_CharacteristicsRecordData.AddRecord(new LED_CharacteristicsRecord());
                }
                LED_CharacteristicsRecordData[LED_CharacteristicsRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountLED_Characteristics(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            LED_CharacteristicsRecordData.MaxLength = value;


        }

        private void OnAlgorithmLED_Characteristics(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            LED_CharacteristicsRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyLED_Characteristics(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,param1,param2,param3,Notes\n");
            foreach (var row in LED_CharacteristicsRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{row.param1},{row.param2},{row.param3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadLED_Characteristics(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadLED_Characteristics();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read LED_Characteristics");
                    return;
                }

                var record = new LED_CharacteristicsRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (double)param0.AsDouble;
                    LED_Characteristics_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param1 = valueList.GetValue("param1");
                if (param1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param1 = (double)param1.AsDouble;
                    LED_Characteristics_param1.Text = record.param1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param2 = valueList.GetValue("param2");
                if (param2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param2 = (double)param2.AsDouble;
                    LED_Characteristics_param2.Text = record.param2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param3 = valueList.GetValue("param3");
                if (param3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param3 = (double)param3.AsDouble;
                    LED_Characteristics_param3.Text = record.param3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                LED_CharacteristicsRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class ButtonRecord : INotifyPropertyChanged
        {
            public ButtonRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Press;
            public double Press { get { return _Press; } set { if (value == _Press) return; _Press = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ButtonRecord> ButtonRecordData { get; } = new DataCollection<ButtonRecord>();
        private void OnButton_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ButtonRecordData.Count == 0)
                {
                    ButtonRecordData.AddRecord(new ButtonRecord());
                }
                ButtonRecordData[ButtonRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountButton(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ButtonRecordData.MaxLength = value;


        }

        private void OnAlgorithmButton(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ButtonRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyButton(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Press,Notes\n");
            foreach (var row in ButtonRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Press},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyButtonSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int ButtonNotifyIndex = 0;
        bool ButtonNotifySetup = false;
        private async void OnNotifyButton(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!ButtonNotifySetup)
                {
                    ButtonNotifySetup = true;
                    bleDevice.ButtonEvent += BleDevice_ButtonEvent;
                }
                var notifyType = NotifyButtonSettings[ButtonNotifyIndex];
                ButtonNotifyIndex = (ButtonNotifyIndex + 1) % NotifyButtonSettings.Length;
                var result = await bleDevice.NotifyButtonAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_ButtonEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new ButtonRecord();

                    var Press = valueList.GetValue("Press");
                    if (Press.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Press.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Press = (double)Press.AsDouble;
                        Button_Press.Text = record.Press.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = ButtonRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteExternal_pin(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte param0;
                var parsedparam0 = Utilities.Parsers.TryParseByte(External_pin_param0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                Byte param1;
                var parsedparam1 = Utilities.Parsers.TryParseByte(External_pin_param1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param1);
                if (!parsedparam1)
                {
                    parseError = "param1";
                }

                Byte param2;
                var parsedparam2 = Utilities.Parsers.TryParseByte(External_pin_param2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param2);
                if (!parsedparam2)
                {
                    parseError = "param2";
                }

                Byte param3;
                var parsedparam3 = Utilities.Parsers.TryParseByte(External_pin_param3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param3);
                if (!parsedparam3)
                {
                    parseError = "param3";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteExternal_pin(param0, param1, param2, param3);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class External_pinRecord : INotifyPropertyChanged
        {
            public External_pinRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private double _param1;
            public double param1 { get { return _param1; } set { if (value == _param1) return; _param1 = value; OnPropertyChanged(); } }

            private double _param2;
            public double param2 { get { return _param2; } set { if (value == _param2) return; _param2 = value; OnPropertyChanged(); } }

            private double _param3;
            public double param3 { get { return _param3; } set { if (value == _param3) return; _param3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<External_pinRecord> External_pinRecordData { get; } = new DataCollection<External_pinRecord>();
        private void OnExternal_pin_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (External_pinRecordData.Count == 0)
                {
                    External_pinRecordData.AddRecord(new External_pinRecord());
                }
                External_pinRecordData[External_pinRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountExternal_pin(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            External_pinRecordData.MaxLength = value;


        }

        private void OnAlgorithmExternal_pin(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            External_pinRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyExternal_pin(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,param1,param2,param3,Notes\n");
            foreach (var row in External_pinRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{row.param1},{row.param2},{row.param3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadExternal_pin(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadExternal_pin();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read External_pin");
                    return;
                }

                var record = new External_pinRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (double)param0.AsDouble;
                    External_pin_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param1 = valueList.GetValue("param1");
                if (param1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param1 = (double)param1.AsDouble;
                    External_pin_param1.Text = record.param1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param2 = valueList.GetValue("param2");
                if (param2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param2 = (double)param2.AsDouble;
                    External_pin_param2.Text = record.param2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param3 = valueList.GetValue("param3");
                if (param3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param3 = (double)param3.AsDouble;
                    External_pin_param3.Text = record.param3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                External_pinRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Motion


        private async void OnWriteMotion_Configuration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 param0;
                var parsedparam0 = Utilities.Parsers.TryParseUInt16(Motion_Configuration_param0.Text, System.Globalization.NumberStyles.None, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                UInt16 param1;
                var parsedparam1 = Utilities.Parsers.TryParseUInt16(Motion_Configuration_param1.Text, System.Globalization.NumberStyles.None, null, out param1);
                if (!parsedparam1)
                {
                    parseError = "param1";
                }

                UInt16 param2;
                var parsedparam2 = Utilities.Parsers.TryParseUInt16(Motion_Configuration_param2.Text, System.Globalization.NumberStyles.None, null, out param2);
                if (!parsedparam2)
                {
                    parseError = "param2";
                }

                UInt16 param3;
                var parsedparam3 = Utilities.Parsers.TryParseUInt16(Motion_Configuration_param3.Text, System.Globalization.NumberStyles.None, null, out param3);
                if (!parsedparam3)
                {
                    parseError = "param3";
                }

                Byte param4;
                var parsedparam4 = Utilities.Parsers.TryParseByte(Motion_Configuration_param4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out param4);
                if (!parsedparam4)
                {
                    parseError = "param4";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMotion_Configuration(param0, param1, param2, param3, param4);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Motion_ConfigurationRecord : INotifyPropertyChanged
        {
            public Motion_ConfigurationRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private double _param1;
            public double param1 { get { return _param1; } set { if (value == _param1) return; _param1 = value; OnPropertyChanged(); } }

            private double _param2;
            public double param2 { get { return _param2; } set { if (value == _param2) return; _param2 = value; OnPropertyChanged(); } }

            private double _param3;
            public double param3 { get { return _param3; } set { if (value == _param3) return; _param3 = value; OnPropertyChanged(); } }

            private double _param4;
            public double param4 { get { return _param4; } set { if (value == _param4) return; _param4 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Motion_ConfigurationRecord> Motion_ConfigurationRecordData { get; } = new DataCollection<Motion_ConfigurationRecord>();
        private void OnMotion_Configuration_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Motion_ConfigurationRecordData.Count == 0)
                {
                    Motion_ConfigurationRecordData.AddRecord(new Motion_ConfigurationRecord());
                }
                Motion_ConfigurationRecordData[Motion_ConfigurationRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMotion_Configuration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Motion_ConfigurationRecordData.MaxLength = value;


        }

        private void OnAlgorithmMotion_Configuration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Motion_ConfigurationRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMotion_Configuration(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,param1,param2,param3,param4,Notes\n");
            foreach (var row in Motion_ConfigurationRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{row.param1},{row.param2},{row.param3},{row.param4},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMotion_Configuration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMotion_Configuration();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Motion_Configuration");
                    return;
                }

                var record = new Motion_ConfigurationRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (double)param0.AsDouble;
                    Motion_Configuration_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param1 = valueList.GetValue("param1");
                if (param1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param1 = (double)param1.AsDouble;
                    Motion_Configuration_param1.Text = record.param1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param2 = valueList.GetValue("param2");
                if (param2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param2 = (double)param2.AsDouble;
                    Motion_Configuration_param2.Text = record.param2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param3 = valueList.GetValue("param3");
                if (param3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param3 = (double)param3.AsDouble;
                    Motion_Configuration_param3.Text = record.param3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var param4 = valueList.GetValue("param4");
                if (param4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param4 = (double)param4.AsDouble;
                    Motion_Configuration_param4.Text = record.param4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Motion_ConfigurationRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class TapsRecord : INotifyPropertyChanged
        {
            public TapsRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private double _param1;
            public double param1 { get { return _param1; } set { if (value == _param1) return; _param1 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<TapsRecord> TapsRecordData { get; } = new DataCollection<TapsRecord>();
        private void OnTaps_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (TapsRecordData.Count == 0)
                {
                    TapsRecordData.AddRecord(new TapsRecord());
                }
                TapsRecordData[TapsRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTaps(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TapsRecordData.MaxLength = value;


        }

        private void OnAlgorithmTaps(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TapsRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTaps(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,param1,Notes\n");
            foreach (var row in TapsRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{row.param1},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyTapsSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int TapsNotifyIndex = 0;
        bool TapsNotifySetup = false;
        private async void OnNotifyTaps(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!TapsNotifySetup)
                {
                    TapsNotifySetup = true;
                    bleDevice.TapsEvent += BleDevice_TapsEvent;
                }
                var notifyType = NotifyTapsSettings[TapsNotifyIndex];
                TapsNotifyIndex = (TapsNotifyIndex + 1) % NotifyTapsSettings.Length;
                var result = await bleDevice.NotifyTapsAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_TapsEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new TapsRecord();

                    var param0 = valueList.GetValue("param0");
                    if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param0 = (double)param0.AsDouble;
                        Taps_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param1 = valueList.GetValue("param1");
                    if (param1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param1 = (double)param1.AsDouble;
                        Taps_param1.Text = record.param1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = TapsRecordData.AddRecord(record);

                });
            }
        }
        public class OrientationRecord : INotifyPropertyChanged
        {
            public OrientationRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<OrientationRecord> OrientationRecordData { get; } = new DataCollection<OrientationRecord>();
        private void OnOrientation_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (OrientationRecordData.Count == 0)
                {
                    OrientationRecordData.AddRecord(new OrientationRecord());
                }
                OrientationRecordData[OrientationRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountOrientation(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            OrientationRecordData.MaxLength = value;


        }

        private void OnAlgorithmOrientation(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            OrientationRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyOrientation(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,Notes\n");
            foreach (var row in OrientationRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyOrientationSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int OrientationNotifyIndex = 0;
        bool OrientationNotifySetup = false;
        private async void OnNotifyOrientation(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!OrientationNotifySetup)
                {
                    OrientationNotifySetup = true;
                    bleDevice.OrientationEvent += BleDevice_OrientationEvent;
                }
                var notifyType = NotifyOrientationSettings[OrientationNotifyIndex];
                OrientationNotifyIndex = (OrientationNotifyIndex + 1) % NotifyOrientationSettings.Length;
                var result = await bleDevice.NotifyOrientationAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_OrientationEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new OrientationRecord();

                    var param0 = valueList.GetValue("param0");
                    if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param0 = (double)param0.AsDouble;
                        Orientation_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = OrientationRecordData.AddRecord(record);

                });
            }
        }
        public class QuaternionsRecord : INotifyPropertyChanged
        {
            public QuaternionsRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _W;
            public double W { get { return _W; } set { if (value == _W) return; _W = value; OnPropertyChanged(); } }

            private double _X;
            public double X { get { return _X; } set { if (value == _X) return; _X = value; OnPropertyChanged(); } }

            private double _Y;
            public double Y { get { return _Y; } set { if (value == _Y) return; _Y = value; OnPropertyChanged(); } }

            private double _Z;
            public double Z { get { return _Z; } set { if (value == _Z) return; _Z = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<QuaternionsRecord> QuaternionsRecordData { get; } = new DataCollection<QuaternionsRecord>();
        private void OnQuaternions_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (QuaternionsRecordData.Count == 0)
                {
                    QuaternionsRecordData.AddRecord(new QuaternionsRecord());
                }
                QuaternionsRecordData[QuaternionsRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountQuaternions(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            QuaternionsRecordData.MaxLength = value;


        }

        private void OnAlgorithmQuaternions(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            QuaternionsRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyQuaternions(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,W,X,Y,Z,Notes\n");
            foreach (var row in QuaternionsRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.W},{row.X},{row.Y},{row.Z},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyQuaternionsSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int QuaternionsNotifyIndex = 0;
        bool QuaternionsNotifySetup = false;
        private async void OnNotifyQuaternions(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!QuaternionsNotifySetup)
                {
                    QuaternionsNotifySetup = true;
                    bleDevice.QuaternionsEvent += BleDevice_QuaternionsEvent;
                }
                var notifyType = NotifyQuaternionsSettings[QuaternionsNotifyIndex];
                QuaternionsNotifyIndex = (QuaternionsNotifyIndex + 1) % NotifyQuaternionsSettings.Length;
                var result = await bleDevice.NotifyQuaternionsAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_QuaternionsEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new QuaternionsRecord();

                    var W = valueList.GetValue("W");
                    if (W.CurrentType == BCBasic.BCValue.ValueType.IsDouble || W.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.W = (double)W.AsDouble;
                        Quaternions_W.Text = record.W.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var X = valueList.GetValue("X");
                    if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.X = (double)X.AsDouble;
                        Quaternions_X.Text = record.X.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Y = valueList.GetValue("Y");
                    if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Y = (double)Y.AsDouble;
                        Quaternions_Y.Text = record.Y.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Z = valueList.GetValue("Z");
                    if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Z = (double)Z.AsDouble;
                        Quaternions_Z.Text = record.Z.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = QuaternionsRecordData.AddRecord(record);

                });
            }
        }
        public class Step_CounterRecord : INotifyPropertyChanged
        {
            public Step_CounterRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Steps;
            public double Steps { get { return _Steps; } set { if (value == _Steps) return; _Steps = value; OnPropertyChanged(); } }

            private double _Time;
            public double Time { get { return _Time; } set { if (value == _Time) return; _Time = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Step_CounterRecord> Step_CounterRecordData { get; } = new DataCollection<Step_CounterRecord>();
        private void OnStep_Counter_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Step_CounterRecordData.Count == 0)
                {
                    Step_CounterRecordData.AddRecord(new Step_CounterRecord());
                }
                Step_CounterRecordData[Step_CounterRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountStep_Counter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Step_CounterRecordData.MaxLength = value;


        }

        private void OnAlgorithmStep_Counter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Step_CounterRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyStep_Counter(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Steps,Time,Notes\n");
            foreach (var row in Step_CounterRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Steps},{row.Time},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyStep_CounterSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Step_CounterNotifyIndex = 0;
        bool Step_CounterNotifySetup = false;
        private async void OnNotifyStep_Counter(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Step_CounterNotifySetup)
                {
                    Step_CounterNotifySetup = true;
                    bleDevice.Step_CounterEvent += BleDevice_Step_CounterEvent;
                }
                var notifyType = NotifyStep_CounterSettings[Step_CounterNotifyIndex];
                Step_CounterNotifyIndex = (Step_CounterNotifyIndex + 1) % NotifyStep_CounterSettings.Length;
                var result = await bleDevice.NotifyStep_CounterAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Step_CounterEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Step_CounterRecord();

                    var Steps = valueList.GetValue("Steps");
                    if (Steps.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Steps.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Steps = (double)Steps.AsDouble;
                        Step_Counter_Steps.Text = record.Steps.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Time = valueList.GetValue("Time");
                    if (Time.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Time.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Time = (double)Time.AsDouble;
                        Step_Counter_Time.Text = record.Time.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Step_CounterRecordData.AddRecord(record);

                });
            }
        }
        public class Raw_MotionRecord : INotifyPropertyChanged
        {
            public Raw_MotionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _AccelX;
            public double AccelX { get { return _AccelX; } set { if (value == _AccelX) return; _AccelX = value; OnPropertyChanged(); } }

            private double _AccelY;
            public double AccelY { get { return _AccelY; } set { if (value == _AccelY) return; _AccelY = value; OnPropertyChanged(); } }

            private double _AccelZ;
            public double AccelZ { get { return _AccelZ; } set { if (value == _AccelZ) return; _AccelZ = value; OnPropertyChanged(); } }

            private double _GyroX;
            public double GyroX { get { return _GyroX; } set { if (value == _GyroX) return; _GyroX = value; OnPropertyChanged(); } }

            private double _GyroY;
            public double GyroY { get { return _GyroY; } set { if (value == _GyroY) return; _GyroY = value; OnPropertyChanged(); } }

            private double _GyroZ;
            public double GyroZ { get { return _GyroZ; } set { if (value == _GyroZ) return; _GyroZ = value; OnPropertyChanged(); } }

            private double _CompassX;
            public double CompassX { get { return _CompassX; } set { if (value == _CompassX) return; _CompassX = value; OnPropertyChanged(); } }

            private double _CompassY;
            public double CompassY { get { return _CompassY; } set { if (value == _CompassY) return; _CompassY = value; OnPropertyChanged(); } }

            private double _CompassZ;
            public double CompassZ { get { return _CompassZ; } set { if (value == _CompassZ) return; _CompassZ = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Raw_MotionRecord> Raw_MotionRecordData { get; } = new DataCollection<Raw_MotionRecord>();
        private void OnRaw_Motion_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Raw_MotionRecordData.Count == 0)
                {
                    Raw_MotionRecordData.AddRecord(new Raw_MotionRecord());
                }
                Raw_MotionRecordData[Raw_MotionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountRaw_Motion(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Raw_MotionRecordData.MaxLength = value;


        }

        private void OnAlgorithmRaw_Motion(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Raw_MotionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyRaw_Motion(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,AccelX,AccelY,AccelZ,GyroX,GyroY,GyroZ,CompassX,CompassY,CompassZ,Notes\n");
            foreach (var row in Raw_MotionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AccelX},{row.AccelY},{row.AccelZ},{row.GyroX},{row.GyroY},{row.GyroZ},{row.CompassX},{row.CompassY},{row.CompassZ},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyRaw_MotionSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Raw_MotionNotifyIndex = 0;
        bool Raw_MotionNotifySetup = false;
        private async void OnNotifyRaw_Motion(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Raw_MotionNotifySetup)
                {
                    Raw_MotionNotifySetup = true;
                    bleDevice.Raw_MotionEvent += BleDevice_Raw_MotionEvent;
                }
                var notifyType = NotifyRaw_MotionSettings[Raw_MotionNotifyIndex];
                Raw_MotionNotifyIndex = (Raw_MotionNotifyIndex + 1) % NotifyRaw_MotionSettings.Length;
                var result = await bleDevice.NotifyRaw_MotionAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Raw_MotionEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Raw_MotionRecord();

                    var AccelX = valueList.GetValue("AccelX");
                    if (AccelX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelX.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.AccelX = (double)AccelX.AsDouble;
                        Raw_Motion_AccelX.Text = record.AccelX.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var AccelY = valueList.GetValue("AccelY");
                    if (AccelY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelY.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.AccelY = (double)AccelY.AsDouble;
                        Raw_Motion_AccelY.Text = record.AccelY.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var AccelZ = valueList.GetValue("AccelZ");
                    if (AccelZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelZ.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.AccelZ = (double)AccelZ.AsDouble;
                        Raw_Motion_AccelZ.Text = record.AccelZ.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var GyroX = valueList.GetValue("GyroX");
                    if (GyroX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroX.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.GyroX = (double)GyroX.AsDouble;
                        Raw_Motion_GyroX.Text = record.GyroX.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var GyroY = valueList.GetValue("GyroY");
                    if (GyroY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroY.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.GyroY = (double)GyroY.AsDouble;
                        Raw_Motion_GyroY.Text = record.GyroY.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var GyroZ = valueList.GetValue("GyroZ");
                    if (GyroZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroZ.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.GyroZ = (double)GyroZ.AsDouble;
                        Raw_Motion_GyroZ.Text = record.GyroZ.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var CompassX = valueList.GetValue("CompassX");
                    if (CompassX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CompassX.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.CompassX = (double)CompassX.AsDouble;
                        Raw_Motion_CompassX.Text = record.CompassX.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var CompassY = valueList.GetValue("CompassY");
                    if (CompassY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CompassY.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.CompassY = (double)CompassY.AsDouble;
                        Raw_Motion_CompassY.Text = record.CompassY.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var CompassZ = valueList.GetValue("CompassZ");
                    if (CompassZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CompassZ.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.CompassZ = (double)CompassZ.AsDouble;
                        Raw_Motion_CompassZ.Text = record.CompassZ.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Raw_MotionRecordData.AddRecord(record);

                });
            }
        }
        public class EulerRecord : INotifyPropertyChanged
        {
            public EulerRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Roll;
            public double Roll { get { return _Roll; } set { if (value == _Roll) return; _Roll = value; OnPropertyChanged(); } }

            private double _Pitch;
            public double Pitch { get { return _Pitch; } set { if (value == _Pitch) return; _Pitch = value; OnPropertyChanged(); } }

            private double _Yaw;
            public double Yaw { get { return _Yaw; } set { if (value == _Yaw) return; _Yaw = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<EulerRecord> EulerRecordData { get; } = new DataCollection<EulerRecord>();
        private void OnEuler_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (EulerRecordData.Count == 0)
                {
                    EulerRecordData.AddRecord(new EulerRecord());
                }
                EulerRecordData[EulerRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountEuler(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EulerRecordData.MaxLength = value;


        }

        private void OnAlgorithmEuler(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EulerRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyEuler(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Roll,Pitch,Yaw,Notes\n");
            foreach (var row in EulerRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Roll},{row.Pitch},{row.Yaw},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyEulerSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int EulerNotifyIndex = 0;
        bool EulerNotifySetup = false;
        private async void OnNotifyEuler(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!EulerNotifySetup)
                {
                    EulerNotifySetup = true;
                    bleDevice.EulerEvent += BleDevice_EulerEvent;
                }
                var notifyType = NotifyEulerSettings[EulerNotifyIndex];
                EulerNotifyIndex = (EulerNotifyIndex + 1) % NotifyEulerSettings.Length;
                var result = await bleDevice.NotifyEulerAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_EulerEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new EulerRecord();

                    var Roll = valueList.GetValue("Roll");
                    if (Roll.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Roll.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Roll = (double)Roll.AsDouble;
                        Euler_Roll.Text = record.Roll.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Pitch = valueList.GetValue("Pitch");
                    if (Pitch.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Pitch.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Pitch = (double)Pitch.AsDouble;
                        Euler_Pitch.Text = record.Pitch.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Yaw = valueList.GetValue("Yaw");
                    if (Yaw.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Yaw.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Yaw = (double)Yaw.AsDouble;
                        Euler_Yaw.Text = record.Yaw.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = EulerRecordData.AddRecord(record);

                });
            }
        }
        public class RotationMatrixRecord : INotifyPropertyChanged
        {
            public RotationMatrixRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private double _param1;
            public double param1 { get { return _param1; } set { if (value == _param1) return; _param1 = value; OnPropertyChanged(); } }

            private double _param2;
            public double param2 { get { return _param2; } set { if (value == _param2) return; _param2 = value; OnPropertyChanged(); } }

            private double _param3;
            public double param3 { get { return _param3; } set { if (value == _param3) return; _param3 = value; OnPropertyChanged(); } }

            private double _param4;
            public double param4 { get { return _param4; } set { if (value == _param4) return; _param4 = value; OnPropertyChanged(); } }

            private double _param5;
            public double param5 { get { return _param5; } set { if (value == _param5) return; _param5 = value; OnPropertyChanged(); } }

            private double _param6;
            public double param6 { get { return _param6; } set { if (value == _param6) return; _param6 = value; OnPropertyChanged(); } }

            private double _param7;
            public double param7 { get { return _param7; } set { if (value == _param7) return; _param7 = value; OnPropertyChanged(); } }

            private double _param8;
            public double param8 { get { return _param8; } set { if (value == _param8) return; _param8 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<RotationMatrixRecord> RotationMatrixRecordData { get; } = new DataCollection<RotationMatrixRecord>();
        private void OnRotationMatrix_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (RotationMatrixRecordData.Count == 0)
                {
                    RotationMatrixRecordData.AddRecord(new RotationMatrixRecord());
                }
                RotationMatrixRecordData[RotationMatrixRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountRotationMatrix(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            RotationMatrixRecordData.MaxLength = value;


        }

        private void OnAlgorithmRotationMatrix(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            RotationMatrixRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyRotationMatrix(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,param1,param2,param3,param4,param5,param6,param7,param8,Notes\n");
            foreach (var row in RotationMatrixRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{row.param1},{row.param2},{row.param3},{row.param4},{row.param5},{row.param6},{row.param7},{row.param8},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyRotationMatrixSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int RotationMatrixNotifyIndex = 0;
        bool RotationMatrixNotifySetup = false;
        private async void OnNotifyRotationMatrix(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!RotationMatrixNotifySetup)
                {
                    RotationMatrixNotifySetup = true;
                    bleDevice.RotationMatrixEvent += BleDevice_RotationMatrixEvent;
                }
                var notifyType = NotifyRotationMatrixSettings[RotationMatrixNotifyIndex];
                RotationMatrixNotifyIndex = (RotationMatrixNotifyIndex + 1) % NotifyRotationMatrixSettings.Length;
                var result = await bleDevice.NotifyRotationMatrixAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_RotationMatrixEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new RotationMatrixRecord();

                    var param0 = valueList.GetValue("param0");
                    if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param0 = (double)param0.AsDouble;
                        RotationMatrix_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param1 = valueList.GetValue("param1");
                    if (param1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param1 = (double)param1.AsDouble;
                        RotationMatrix_param1.Text = record.param1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param2 = valueList.GetValue("param2");
                    if (param2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param2 = (double)param2.AsDouble;
                        RotationMatrix_param2.Text = record.param2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param3 = valueList.GetValue("param3");
                    if (param3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param3 = (double)param3.AsDouble;
                        RotationMatrix_param3.Text = record.param3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param4 = valueList.GetValue("param4");
                    if (param4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param4 = (double)param4.AsDouble;
                        RotationMatrix_param4.Text = record.param4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param5 = valueList.GetValue("param5");
                    if (param5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param5 = (double)param5.AsDouble;
                        RotationMatrix_param5.Text = record.param5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param6 = valueList.GetValue("param6");
                    if (param6.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param6.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param6 = (double)param6.AsDouble;
                        RotationMatrix_param6.Text = record.param6.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param7 = valueList.GetValue("param7");
                    if (param7.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param7.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param7 = (double)param7.AsDouble;
                        RotationMatrix_param7.Text = record.param7.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var param8 = valueList.GetValue("param8");
                    if (param8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param8.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.param8 = (double)param8.AsDouble;
                        RotationMatrix_param8.Text = record.param8.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = RotationMatrixRecordData.AddRecord(record);

                });
            }
        }
        public class Compass_HeadingRecord : INotifyPropertyChanged
        {
            public Compass_HeadingRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Heading;
            public double Heading { get { return _Heading; } set { if (value == _Heading) return; _Heading = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Compass_HeadingRecord> Compass_HeadingRecordData { get; } = new DataCollection<Compass_HeadingRecord>();
        private void OnCompass_Heading_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Compass_HeadingRecordData.Count == 0)
                {
                    Compass_HeadingRecordData.AddRecord(new Compass_HeadingRecord());
                }
                Compass_HeadingRecordData[Compass_HeadingRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountCompass_Heading(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Compass_HeadingRecordData.MaxLength = value;


        }

        private void OnAlgorithmCompass_Heading(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Compass_HeadingRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyCompass_Heading(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Heading,Notes\n");
            foreach (var row in Compass_HeadingRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Heading},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyCompass_HeadingSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Compass_HeadingNotifyIndex = 0;
        bool Compass_HeadingNotifySetup = false;
        private async void OnNotifyCompass_Heading(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Compass_HeadingNotifySetup)
                {
                    Compass_HeadingNotifySetup = true;
                    bleDevice.Compass_HeadingEvent += BleDevice_Compass_HeadingEvent;
                }
                var notifyType = NotifyCompass_HeadingSettings[Compass_HeadingNotifyIndex];
                Compass_HeadingNotifyIndex = (Compass_HeadingNotifyIndex + 1) % NotifyCompass_HeadingSettings.Length;
                var result = await bleDevice.NotifyCompass_HeadingAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Compass_HeadingEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Compass_HeadingRecord();

                    var Heading = valueList.GetValue("Heading");
                    if (Heading.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Heading.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Heading = (double)Heading.AsDouble;
                        Compass_Heading_Heading.Text = record.Heading.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Compass_HeadingRecordData.AddRecord(record);

                });
            }
        }
        public class GravityRecord : INotifyPropertyChanged
        {
            public GravityRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _X;
            public double X { get { return _X; } set { if (value == _X) return; _X = value; OnPropertyChanged(); } }

            private double _Y;
            public double Y { get { return _Y; } set { if (value == _Y) return; _Y = value; OnPropertyChanged(); } }

            private double _Z;
            public double Z { get { return _Z; } set { if (value == _Z) return; _Z = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<GravityRecord> GravityRecordData { get; } = new DataCollection<GravityRecord>();
        private void OnGravity_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (GravityRecordData.Count == 0)
                {
                    GravityRecordData.AddRecord(new GravityRecord());
                }
                GravityRecordData[GravityRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountGravity(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            GravityRecordData.MaxLength = value;

            GravityChart.RedrawYTime<GravityRecord>(GravityRecordData);
        }

        private void OnAlgorithmGravity(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            GravityRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyGravity(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,X,Y,Z,Notes\n");
            foreach (var row in GravityRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.X},{row.Y},{row.Z},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyGravitySettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int GravityNotifyIndex = 0;
        bool GravityNotifySetup = false;
        private async void OnNotifyGravity(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!GravityNotifySetup)
                {
                    GravityNotifySetup = true;
                    bleDevice.GravityEvent += BleDevice_GravityEvent;
                }
                var notifyType = NotifyGravitySettings[GravityNotifyIndex];
                GravityNotifyIndex = (GravityNotifyIndex + 1) % NotifyGravitySettings.Length;
                var result = await bleDevice.NotifyGravityAsync(notifyType);



                var EventTimeProperty = typeof(GravityRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(GravityRecord).GetProperty("X"),
typeof(GravityRecord).GetProperty("Y"),
typeof(GravityRecord).GetProperty("Z"),
                };
                var names = new List<string>()
                {
"X",
"Y",
"Z",
                };
                GravityChart.SetDataProperties(properties, EventTimeProperty, names);
                GravityChart.SetTitle("Gravity Chart");
                GravityChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<GravityRecord>(addResult, GravityRecordData)",
                    chartDefaultMaxY = 20,
                    chartDefaultMinY = -20,
                    chartLineDefaults ={
                        { "X", new ChartLineDefaults() {
                            stroke="DarkRed",
                            }
                        },
                        { "Y", new ChartLineDefaults() {
                            stroke="DarkGreen",
                            }
                        },
                        { "Z", new ChartLineDefaults() {
                            stroke="DarkBlue",
                            }
                        },
                    },
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_GravityEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new GravityRecord();

                    var X = valueList.GetValue("X");
                    if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.X = (double)X.AsDouble;
                        Gravity_X.Text = record.X.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Y = valueList.GetValue("Y");
                    if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Y = (double)Y.AsDouble;
                        Gravity_Y.Text = record.Y.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Z = valueList.GetValue("Z");
                    if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Z = (double)Z.AsDouble;
                        Gravity_Z.Text = record.Z.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = GravityRecordData.AddRecord(record);
                    GravityChart.AddYTime<GravityRecord>(addResult, GravityRecordData);
                });
            }
        }

        // Functions for Audio


        private async void OnWriteAudio_Configuration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte SpeakerMode;
                var parsedSpeakerMode = Utilities.Parsers.TryParseByte(Audio_Configuration_SpeakerMode.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SpeakerMode);
                if (!parsedSpeakerMode)
                {
                    parseError = "SpeakerMode";
                }

                Byte MicrophoneMode;
                var parsedMicrophoneMode = Utilities.Parsers.TryParseByte(Audio_Configuration_MicrophoneMode.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out MicrophoneMode);
                if (!parsedMicrophoneMode)
                {
                    parseError = "MicrophoneMode";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAudio_Configuration(SpeakerMode, MicrophoneMode);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Audio_ConfigurationRecord : INotifyPropertyChanged
        {
            public Audio_ConfigurationRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _SpeakerMode;
            public double SpeakerMode { get { return _SpeakerMode; } set { if (value == _SpeakerMode) return; _SpeakerMode = value; OnPropertyChanged(); } }

            private double _MicrophoneMode;
            public double MicrophoneMode { get { return _MicrophoneMode; } set { if (value == _MicrophoneMode) return; _MicrophoneMode = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Audio_ConfigurationRecord> Audio_ConfigurationRecordData { get; } = new DataCollection<Audio_ConfigurationRecord>();
        private void OnAudio_Configuration_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Audio_ConfigurationRecordData.Count == 0)
                {
                    Audio_ConfigurationRecordData.AddRecord(new Audio_ConfigurationRecord());
                }
                Audio_ConfigurationRecordData[Audio_ConfigurationRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAudio_Configuration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Audio_ConfigurationRecordData.MaxLength = value;


        }

        private void OnAlgorithmAudio_Configuration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Audio_ConfigurationRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAudio_Configuration(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,SpeakerMode,MicrophoneMode,Notes\n");
            foreach (var row in Audio_ConfigurationRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.SpeakerMode},{row.MicrophoneMode},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAudio_Configuration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAudio_Configuration();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Audio_Configuration");
                    return;
                }

                var record = new Audio_ConfigurationRecord();

                var SpeakerMode = valueList.GetValue("SpeakerMode");
                if (SpeakerMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SpeakerMode.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SpeakerMode = (double)SpeakerMode.AsDouble;
                    Audio_Configuration_SpeakerMode.Text = record.SpeakerMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MicrophoneMode = valueList.GetValue("MicrophoneMode");
                if (MicrophoneMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MicrophoneMode.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MicrophoneMode = (double)MicrophoneMode.AsDouble;
                    Audio_Configuration_MicrophoneMode.Text = record.MicrophoneMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Audio_ConfigurationRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteSpeaker_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Data;
                var parsedData = Utilities.Parsers.TryParseBytes(Speaker_Data_Data.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Data);
                if (!parsedData)
                {
                    parseError = "Data";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteSpeaker_Data(Data);
                }
                else
                { //NOTE: pop up a dialog?
                    SetStatus($"Error: could not parse {parseError}");
                }
            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Speaker_StatusRecord : INotifyPropertyChanged
        {
            public Speaker_StatusRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _SpeakerStatus;
            public double SpeakerStatus { get { return _SpeakerStatus; } set { if (value == _SpeakerStatus) return; _SpeakerStatus = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Speaker_StatusRecord> Speaker_StatusRecordData { get; } = new DataCollection<Speaker_StatusRecord>();
        private void OnSpeaker_Status_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Speaker_StatusRecordData.Count == 0)
                {
                    Speaker_StatusRecordData.AddRecord(new Speaker_StatusRecord());
                }
                Speaker_StatusRecordData[Speaker_StatusRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountSpeaker_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Speaker_StatusRecordData.MaxLength = value;


        }

        private void OnAlgorithmSpeaker_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Speaker_StatusRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopySpeaker_Status(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,SpeakerStatus,Notes\n");
            foreach (var row in Speaker_StatusRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.SpeakerStatus},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifySpeaker_StatusSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Speaker_StatusNotifyIndex = 0;
        bool Speaker_StatusNotifySetup = false;
        private async void OnNotifySpeaker_Status(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Speaker_StatusNotifySetup)
                {
                    Speaker_StatusNotifySetup = true;
                    bleDevice.Speaker_StatusEvent += BleDevice_Speaker_StatusEvent;
                }
                var notifyType = NotifySpeaker_StatusSettings[Speaker_StatusNotifyIndex];
                Speaker_StatusNotifyIndex = (Speaker_StatusNotifyIndex + 1) % NotifySpeaker_StatusSettings.Length;
                var result = await bleDevice.NotifySpeaker_StatusAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Speaker_StatusEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Speaker_StatusRecord();

                    var SpeakerStatus = valueList.GetValue("SpeakerStatus");
                    if (SpeakerStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SpeakerStatus.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.SpeakerStatus = (double)SpeakerStatus.AsDouble;
                        Speaker_Status_SpeakerStatus.Text = record.SpeakerStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Speaker_StatusRecordData.AddRecord(record);

                });
            }
        }
        public class Microphone_DataRecord : INotifyPropertyChanged
        {
            public Microphone_DataRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _MicrophoneStatus;
            public string MicrophoneStatus { get { return _MicrophoneStatus; } set { if (value == _MicrophoneStatus) return; _MicrophoneStatus = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Microphone_DataRecord> Microphone_DataRecordData { get; } = new DataCollection<Microphone_DataRecord>();
        private void OnMicrophone_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Microphone_DataRecordData.Count == 0)
                {
                    Microphone_DataRecordData.AddRecord(new Microphone_DataRecord());
                }
                Microphone_DataRecordData[Microphone_DataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMicrophone_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Microphone_DataRecordData.MaxLength = value;


        }

        private void OnAlgorithmMicrophone_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Microphone_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMicrophone_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MicrophoneStatus,Notes\n");
            foreach (var row in Microphone_DataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MicrophoneStatus},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMicrophone_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Microphone_DataNotifyIndex = 0;
        bool Microphone_DataNotifySetup = false;
        private async void OnNotifyMicrophone_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Microphone_DataNotifySetup)
                {
                    Microphone_DataNotifySetup = true;
                    bleDevice.Microphone_DataEvent += BleDevice_Microphone_DataEvent;
                }
                var notifyType = NotifyMicrophone_DataSettings[Microphone_DataNotifyIndex];
                Microphone_DataNotifyIndex = (Microphone_DataNotifyIndex + 1) % NotifyMicrophone_DataSettings.Length;
                var result = await bleDevice.NotifyMicrophone_DataAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Microphone_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Microphone_DataRecord();

                    var MicrophoneStatus = valueList.GetValue("MicrophoneStatus");
                    if (MicrophoneStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MicrophoneStatus.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.MicrophoneStatus = (string)MicrophoneStatus.AsString;
                        Microphone_Data_MicrophoneStatus.Text = record.MicrophoneStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Microphone_DataRecordData.AddRecord(record);

                });
            }
        }

        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(true);
            SetStatusActive(false);
        }
    }
}
