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
    public sealed partial class BleHome_SensorBugPage : Page, HasId, ISetHandleStatus
    {
        public BleHome_SensorBugPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "BleHome_SensorBug";
        private string DeviceNameUser = "SensorBug10B5D0";

        int ncommand = 0;
        BleHome_SensorBug bleDevice = new BleHome_SensorBug();
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

        private async void OnWritePrivacy(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Privacy;
                var parsedPrivacy = Utilities.Parsers.TryParseBytes(Privacy_Privacy.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Privacy);
                if (!parsedPrivacy)
                {
                    parseError = "Privacy";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePrivacy(Privacy);
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

        public class PrivacyRecord : INotifyPropertyChanged
        {
            public PrivacyRecord()
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

            private string _Privacy;
            public string Privacy { get { return _Privacy; } set { if (value == _Privacy) return; _Privacy = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<PrivacyRecord> PrivacyRecordData { get; } = new DataCollection<PrivacyRecord>();
        private void OnPrivacy_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (PrivacyRecordData.Count == 0)
                {
                    PrivacyRecordData.AddRecord(new PrivacyRecord());
                }
                PrivacyRecordData[PrivacyRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPrivacy(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PrivacyRecordData.MaxLength = value;


        }

        private void OnAlgorithmPrivacy(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PrivacyRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPrivacy(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Privacy,Notes\n");
            foreach (var row in PrivacyRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Privacy},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPrivacy(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPrivacy();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Privacy");
                    return;
                }

                var record = new PrivacyRecord();

                var Privacy = valueList.GetValue("Privacy");
                if (Privacy.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Privacy.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Privacy = (string)Privacy.AsString;
                    Privacy_Privacy.Text = record.Privacy.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                PrivacyRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteReconnect_Address(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes ReconnectAddress;
                var parsedReconnectAddress = Utilities.Parsers.TryParseBytes(Reconnect_Address_ReconnectAddress.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ReconnectAddress);
                if (!parsedReconnectAddress)
                {
                    parseError = "ReconnectAddress";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteReconnect_Address(ReconnectAddress);
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


        // Functions for Device Info


        public class Manuafacturer_NameRecord : INotifyPropertyChanged
        {
            public Manuafacturer_NameRecord()
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

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Manuafacturer_NameRecord> Manuafacturer_NameRecordData { get; } = new DataCollection<Manuafacturer_NameRecord>();
        private void OnManuafacturer_Name_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Manuafacturer_NameRecordData.Count == 0)
                {
                    Manuafacturer_NameRecordData.AddRecord(new Manuafacturer_NameRecord());
                }
                Manuafacturer_NameRecordData[Manuafacturer_NameRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountManuafacturer_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Manuafacturer_NameRecordData.MaxLength = value;


        }

        private void OnAlgorithmManuafacturer_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Manuafacturer_NameRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyManuafacturer_Name(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,Notes\n");
            foreach (var row in Manuafacturer_NameRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadManuafacturer_Name(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadManuafacturer_Name();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Manuafacturer_Name");
                    return;
                }

                var record = new Manuafacturer_NameRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (string)param0.AsString;
                    Manuafacturer_Name_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Manuafacturer_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Model_NumberRecord : INotifyPropertyChanged
        {
            public Model_NumberRecord()
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

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Model_NumberRecord> Model_NumberRecordData { get; } = new DataCollection<Model_NumberRecord>();
        private void OnModel_Number_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Model_NumberRecordData.Count == 0)
                {
                    Model_NumberRecordData.AddRecord(new Model_NumberRecord());
                }
                Model_NumberRecordData[Model_NumberRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountModel_Number(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Model_NumberRecordData.MaxLength = value;


        }

        private void OnAlgorithmModel_Number(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Model_NumberRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyModel_Number(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,Notes\n");
            foreach (var row in Model_NumberRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadModel_Number(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadModel_Number();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Model_Number");
                    return;
                }

                var record = new Model_NumberRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (string)param0.AsString;
                    Model_Number_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Model_NumberRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Hardware_RevisionRecord : INotifyPropertyChanged
        {
            public Hardware_RevisionRecord()
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

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Hardware_RevisionRecord> Hardware_RevisionRecordData { get; } = new DataCollection<Hardware_RevisionRecord>();
        private void OnHardware_Revision_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Hardware_RevisionRecordData.Count == 0)
                {
                    Hardware_RevisionRecordData.AddRecord(new Hardware_RevisionRecord());
                }
                Hardware_RevisionRecordData[Hardware_RevisionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountHardware_Revision(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Hardware_RevisionRecordData.MaxLength = value;


        }

        private void OnAlgorithmHardware_Revision(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Hardware_RevisionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyHardware_Revision(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,Notes\n");
            foreach (var row in Hardware_RevisionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadHardware_Revision(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHardware_Revision();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Hardware_Revision");
                    return;
                }

                var record = new Hardware_RevisionRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (string)param0.AsString;
                    Hardware_Revision_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Hardware_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Firmware_RevisionRecord : INotifyPropertyChanged
        {
            public Firmware_RevisionRecord()
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

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Firmware_RevisionRecord> Firmware_RevisionRecordData { get; } = new DataCollection<Firmware_RevisionRecord>();
        private void OnFirmware_Revision_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Firmware_RevisionRecordData.Count == 0)
                {
                    Firmware_RevisionRecordData.AddRecord(new Firmware_RevisionRecord());
                }
                Firmware_RevisionRecordData[Firmware_RevisionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountFirmware_Revision(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Firmware_RevisionRecordData.MaxLength = value;


        }

        private void OnAlgorithmFirmware_Revision(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Firmware_RevisionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyFirmware_Revision(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,Notes\n");
            foreach (var row in Firmware_RevisionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadFirmware_Revision(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadFirmware_Revision();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Firmware_Revision");
                    return;
                }

                var record = new Firmware_RevisionRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (string)param0.AsString;
                    Firmware_Revision_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Firmware_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Software_RevisionRecord : INotifyPropertyChanged
        {
            public Software_RevisionRecord()
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

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Software_RevisionRecord> Software_RevisionRecordData { get; } = new DataCollection<Software_RevisionRecord>();
        private void OnSoftware_Revision_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Software_RevisionRecordData.Count == 0)
                {
                    Software_RevisionRecordData.AddRecord(new Software_RevisionRecord());
                }
                Software_RevisionRecordData[Software_RevisionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountSoftware_Revision(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Software_RevisionRecordData.MaxLength = value;


        }

        private void OnAlgorithmSoftware_Revision(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Software_RevisionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopySoftware_Revision(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,Notes\n");
            foreach (var row in Software_RevisionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadSoftware_Revision(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSoftware_Revision();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Software_Revision");
                    return;
                }

                var record = new Software_RevisionRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (string)param0.AsString;
                    Software_Revision_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Software_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class PnP_IDRecord : INotifyPropertyChanged
        {
            public PnP_IDRecord()
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

            private string _param0;
            public string param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<PnP_IDRecord> PnP_IDRecordData { get; } = new DataCollection<PnP_IDRecord>();
        private void OnPnP_ID_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (PnP_IDRecordData.Count == 0)
                {
                    PnP_IDRecordData.AddRecord(new PnP_IDRecord());
                }
                PnP_IDRecordData[PnP_IDRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPnP_ID(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PnP_IDRecordData.MaxLength = value;


        }

        private void OnAlgorithmPnP_ID(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PnP_IDRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPnP_ID(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,param0,Notes\n");
            foreach (var row in PnP_IDRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPnP_ID(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPnP_ID();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read PnP_ID");
                    return;
                }

                var record = new PnP_IDRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (string)param0.AsString;
                    PnP_ID_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                PnP_IDRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


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

        // Functions for Link Loss


        private async void OnWriteLink_Loss_Alert_Level(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Level;
                var parsedLevel = Utilities.Parsers.TryParseByte(Link_Loss_Alert_Level_Level.Text, System.Globalization.NumberStyles.None, null, out Level);
                if (!parsedLevel)
                {
                    parseError = "Level";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteLink_Loss_Alert_Level(Level);
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

        public class Link_Loss_Alert_LevelRecord : INotifyPropertyChanged
        {
            public Link_Loss_Alert_LevelRecord()
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

            private double _Level;
            public double Level { get { return _Level; } set { if (value == _Level) return; _Level = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Link_Loss_Alert_LevelRecord> Link_Loss_Alert_LevelRecordData { get; } = new DataCollection<Link_Loss_Alert_LevelRecord>();
        private void OnLink_Loss_Alert_Level_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Link_Loss_Alert_LevelRecordData.Count == 0)
                {
                    Link_Loss_Alert_LevelRecordData.AddRecord(new Link_Loss_Alert_LevelRecord());
                }
                Link_Loss_Alert_LevelRecordData[Link_Loss_Alert_LevelRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountLink_Loss_Alert_Level(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Link_Loss_Alert_LevelRecordData.MaxLength = value;


        }

        private void OnAlgorithmLink_Loss_Alert_Level(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Link_Loss_Alert_LevelRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyLink_Loss_Alert_Level(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Level,Notes\n");
            foreach (var row in Link_Loss_Alert_LevelRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Level},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadLink_Loss_Alert_Level(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadLink_Loss_Alert_Level();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Link_Loss_Alert_Level");
                    return;
                }

                var record = new Link_Loss_Alert_LevelRecord();

                var Level = valueList.GetValue("Level");
                if (Level.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Level.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Level = (double)Level.AsDouble;
                    Link_Loss_Alert_Level_Level.Text = record.Level.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Link_Loss_Alert_LevelRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Immediate Alert


        private async void OnWriteImmediate_Alert_Level(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Level;
                var parsedLevel = Utilities.Parsers.TryParseByte(Immediate_Alert_Level_Level.Text, System.Globalization.NumberStyles.None, null, out Level);
                if (!parsedLevel)
                {
                    parseError = "Level";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteImmediate_Alert_Level(Level);
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

        public class Immediate_Alert_LevelRecord : INotifyPropertyChanged
        {
            public Immediate_Alert_LevelRecord()
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

            private double _Level;
            public double Level { get { return _Level; } set { if (value == _Level) return; _Level = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Immediate_Alert_LevelRecord> Immediate_Alert_LevelRecordData { get; } = new DataCollection<Immediate_Alert_LevelRecord>();
        private void OnImmediate_Alert_Level_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Immediate_Alert_LevelRecordData.Count == 0)
                {
                    Immediate_Alert_LevelRecordData.AddRecord(new Immediate_Alert_LevelRecord());
                }
                Immediate_Alert_LevelRecordData[Immediate_Alert_LevelRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountImmediate_Alert_Level(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Immediate_Alert_LevelRecordData.MaxLength = value;


        }

        private void OnAlgorithmImmediate_Alert_Level(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Immediate_Alert_LevelRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyImmediate_Alert_Level(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Level,Notes\n");
            foreach (var row in Immediate_Alert_LevelRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Level},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadImmediate_Alert_Level(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadImmediate_Alert_Level();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Immediate_Alert_Level");
                    return;
                }

                var record = new Immediate_Alert_LevelRecord();

                var Level = valueList.GetValue("Level");
                if (Level.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Level.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Level = (double)Level.AsDouble;
                    Immediate_Alert_Level_Level.Text = record.Level.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Immediate_Alert_LevelRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Tranmit Power


        public class Transmit_PowerRecord : INotifyPropertyChanged
        {
            public Transmit_PowerRecord()
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

            private double _Power;
            public double Power { get { return _Power; } set { if (value == _Power) return; _Power = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Transmit_PowerRecord> Transmit_PowerRecordData { get; } = new DataCollection<Transmit_PowerRecord>();
        private void OnTransmit_Power_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Transmit_PowerRecordData.Count == 0)
                {
                    Transmit_PowerRecordData.AddRecord(new Transmit_PowerRecord());
                }
                Transmit_PowerRecordData[Transmit_PowerRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTransmit_Power(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Transmit_PowerRecordData.MaxLength = value;


        }

        private void OnAlgorithmTransmit_Power(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Transmit_PowerRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTransmit_Power(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Power,Notes\n");
            foreach (var row in Transmit_PowerRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Power},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTransmit_Power(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTransmit_Power();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Transmit_Power");
                    return;
                }

                var record = new Transmit_PowerRecord();

                var Power = valueList.GetValue("Power");
                if (Power.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Power.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Power = (double)Power.AsDouble;
                    Transmit_Power_Power.Text = record.Power.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Transmit_PowerRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Accelerometer


        private async void OnWriteAccelerometer_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown0;
                var parsedUnknown0 = Utilities.Parsers.TryParseBytes(Accelerometer_Config_Unknown0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown0);
                if (!parsedUnknown0)
                {
                    parseError = "Unknown0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAccelerometer_Config(Unknown0);
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

        public class Accelerometer_ConfigRecord : INotifyPropertyChanged
        {
            public Accelerometer_ConfigRecord()
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

            private string _Unknown0;
            public string Unknown0 { get { return _Unknown0; } set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Accelerometer_ConfigRecord> Accelerometer_ConfigRecordData { get; } = new DataCollection<Accelerometer_ConfigRecord>();
        private void OnAccelerometer_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Accelerometer_ConfigRecordData.Count == 0)
                {
                    Accelerometer_ConfigRecordData.AddRecord(new Accelerometer_ConfigRecord());
                }
                Accelerometer_ConfigRecordData[Accelerometer_ConfigRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAccelerometer_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Accelerometer_ConfigRecordData.MaxLength = value;


        }

        private void OnAlgorithmAccelerometer_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Accelerometer_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAccelerometer_Config(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown0,Notes\n");
            foreach (var row in Accelerometer_ConfigRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAccelerometer_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometer_Config();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Accelerometer_Config");
                    return;
                }

                var record = new Accelerometer_ConfigRecord();

                var Unknown0 = valueList.GetValue("Unknown0");
                if (Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown0 = (string)Unknown0.AsString;
                    Accelerometer_Config_Unknown0.Text = record.Unknown0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Accelerometer_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Accelerometer_DataRecord : INotifyPropertyChanged
        {
            public Accelerometer_DataRecord()
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

        public DataCollection<Accelerometer_DataRecord> Accelerometer_DataRecordData { get; } = new DataCollection<Accelerometer_DataRecord>();
        private void OnAccelerometer_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Accelerometer_DataRecordData.Count == 0)
                {
                    Accelerometer_DataRecordData.AddRecord(new Accelerometer_DataRecord());
                }
                Accelerometer_DataRecordData[Accelerometer_DataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAccelerometer_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Accelerometer_DataRecordData.MaxLength = value;


        }

        private void OnAlgorithmAccelerometer_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Accelerometer_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAccelerometer_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,X,Y,Z,Notes\n");
            foreach (var row in Accelerometer_DataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.X},{row.Y},{row.Z},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAccelerometer_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometer_Data();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Accelerometer_Data");
                    return;
                }

                var record = new Accelerometer_DataRecord();

                var X = valueList.GetValue("X");
                if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.X = (double)X.AsDouble;
                    Accelerometer_Data_X.Text = record.X.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Y = valueList.GetValue("Y");
                if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Y = (double)Y.AsDouble;
                    Accelerometer_Data_Y.Text = record.Y.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Z = valueList.GetValue("Z");
                if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Z = (double)Z.AsDouble;
                    Accelerometer_Data_Z.Text = record.Z.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Accelerometer_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyAccelerometer_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Accelerometer_DataNotifyIndex = 0;
        bool Accelerometer_DataNotifySetup = false;
        private async void OnNotifyAccelerometer_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Accelerometer_DataNotifySetup)
                {
                    Accelerometer_DataNotifySetup = true;
                    bleDevice.Accelerometer_DataEvent += BleDevice_Accelerometer_DataEvent;
                }
                var notifyType = NotifyAccelerometer_DataSettings[Accelerometer_DataNotifyIndex];
                Accelerometer_DataNotifyIndex = (Accelerometer_DataNotifyIndex + 1) % NotifyAccelerometer_DataSettings.Length;
                var result = await bleDevice.NotifyAccelerometer_DataAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Accelerometer_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Accelerometer_DataRecord();

                    var X = valueList.GetValue("X");
                    if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.X = (double)X.AsDouble;
                        Accelerometer_Data_X.Text = record.X.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Y = valueList.GetValue("Y");
                    if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Y = (double)Y.AsDouble;
                        Accelerometer_Data_Y.Text = record.Y.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Z = valueList.GetValue("Z");
                    if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Z = (double)Z.AsDouble;
                        Accelerometer_Data_Z.Text = record.Z.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Accelerometer_DataRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteAccelerometer_Alert(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown2;
                var parsedUnknown2 = Utilities.Parsers.TryParseBytes(Accelerometer_Alert_Unknown2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown2);
                if (!parsedUnknown2)
                {
                    parseError = "Unknown2";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAccelerometer_Alert(Unknown2);
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

        public class Accelerometer_AlertRecord : INotifyPropertyChanged
        {
            public Accelerometer_AlertRecord()
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

            private string _Unknown2;
            public string Unknown2 { get { return _Unknown2; } set { if (value == _Unknown2) return; _Unknown2 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Accelerometer_AlertRecord> Accelerometer_AlertRecordData { get; } = new DataCollection<Accelerometer_AlertRecord>();
        private void OnAccelerometer_Alert_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Accelerometer_AlertRecordData.Count == 0)
                {
                    Accelerometer_AlertRecordData.AddRecord(new Accelerometer_AlertRecord());
                }
                Accelerometer_AlertRecordData[Accelerometer_AlertRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAccelerometer_Alert(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Accelerometer_AlertRecordData.MaxLength = value;


        }

        private void OnAlgorithmAccelerometer_Alert(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Accelerometer_AlertRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAccelerometer_Alert(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown2,Notes\n");
            foreach (var row in Accelerometer_AlertRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown2},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAccelerometer_Alert(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometer_Alert();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Accelerometer_Alert");
                    return;
                }

                var record = new Accelerometer_AlertRecord();

                var Unknown2 = valueList.GetValue("Unknown2");
                if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown2 = (string)Unknown2.AsString;
                    Accelerometer_Alert_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Accelerometer_AlertRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyAccelerometer_AlertSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Accelerometer_AlertNotifyIndex = 0;
        bool Accelerometer_AlertNotifySetup = false;
        private async void OnNotifyAccelerometer_Alert(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Accelerometer_AlertNotifySetup)
                {
                    Accelerometer_AlertNotifySetup = true;
                    bleDevice.Accelerometer_AlertEvent += BleDevice_Accelerometer_AlertEvent;
                }
                var notifyType = NotifyAccelerometer_AlertSettings[Accelerometer_AlertNotifyIndex];
                Accelerometer_AlertNotifyIndex = (Accelerometer_AlertNotifyIndex + 1) % NotifyAccelerometer_AlertSettings.Length;
                var result = await bleDevice.NotifyAccelerometer_AlertAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Accelerometer_AlertEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Accelerometer_AlertRecord();

                    var Unknown2 = valueList.GetValue("Unknown2");
                    if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown2 = (string)Unknown2.AsString;
                        Accelerometer_Alert_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Accelerometer_AlertRecordData.AddRecord(record);

                });
            }
        }

        // Functions for Light


        private async void OnWriteLight_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Enable;
                var parsedEnable = Utilities.Parsers.TryParseByte(Light_Config_Enable.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Enable);
                if (!parsedEnable)
                {
                    parseError = "Enable";
                }

                Byte ModeFlags;
                var parsedModeFlags = Utilities.Parsers.TryParseByte(Light_Config_ModeFlags.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ModeFlags);
                if (!parsedModeFlags)
                {
                    parseError = "ModeFlags";
                }

                UInt16 DataRate;
                var parsedDataRate = Utilities.Parsers.TryParseUInt16(Light_Config_DataRate.Text, System.Globalization.NumberStyles.None, null, out DataRate);
                if (!parsedDataRate)
                {
                    parseError = "DataRate";
                }

                UInt16 NotiRate;
                var parsedNotiRate = Utilities.Parsers.TryParseUInt16(Light_Config_NotiRate.Text, System.Globalization.NumberStyles.None, null, out NotiRate);
                if (!parsedNotiRate)
                {
                    parseError = "NotiRate";
                }

                UInt16 AlertLog;
                var parsedAlertLog = Utilities.Parsers.TryParseUInt16(Light_Config_AlertLog.Text, System.Globalization.NumberStyles.None, null, out AlertLog);
                if (!parsedAlertLog)
                {
                    parseError = "AlertLog";
                }

                UInt16 AlertHi;
                var parsedAlertHi = Utilities.Parsers.TryParseUInt16(Light_Config_AlertHi.Text, System.Globalization.NumberStyles.None, null, out AlertHi);
                if (!parsedAlertHi)
                {
                    parseError = "AlertHi";
                }

                UInt16 AlertFaults;
                var parsedAlertFaults = Utilities.Parsers.TryParseUInt16(Light_Config_AlertFaults.Text, System.Globalization.NumberStyles.None, null, out AlertFaults);
                if (!parsedAlertFaults)
                {
                    parseError = "AlertFaults";
                }

                UInt16 Reserved;
                var parsedReserved = Utilities.Parsers.TryParseUInt16(Light_Config_Reserved.Text, System.Globalization.NumberStyles.None, null, out Reserved);
                if (!parsedReserved)
                {
                    parseError = "Reserved";
                }

                Byte Range;
                var parsedRange = Utilities.Parsers.TryParseByte(Light_Config_Range.Text, System.Globalization.NumberStyles.None, null, out Range);
                if (!parsedRange)
                {
                    parseError = "Range";
                }

                Byte Resolution;
                var parsedResolution = Utilities.Parsers.TryParseByte(Light_Config_Resolution.Text, System.Globalization.NumberStyles.None, null, out Resolution);
                if (!parsedResolution)
                {
                    parseError = "Resolution";
                }

                UInt16 AlertResetCount;
                var parsedAlertResetCount = Utilities.Parsers.TryParseUInt16(Light_Config_AlertResetCount.Text, System.Globalization.NumberStyles.None, null, out AlertResetCount);
                if (!parsedAlertResetCount)
                {
                    parseError = "AlertResetCount";
                }

                UInt16 AertResetDiff;
                var parsedAertResetDiff = Utilities.Parsers.TryParseUInt16(Light_Config_AertResetDiff.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out AertResetDiff);
                if (!parsedAertResetDiff)
                {
                    parseError = "AertResetDiff";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteLight_Config(Enable, ModeFlags, DataRate, NotiRate, AlertLog, AlertHi, AlertFaults, Reserved, Range, Resolution, AlertResetCount, AertResetDiff);
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

        public class Light_ConfigRecord : INotifyPropertyChanged
        {
            public Light_ConfigRecord()
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

            private double _Enable;
            public double Enable { get { return _Enable; } set { if (value == _Enable) return; _Enable = value; OnPropertyChanged(); } }

            private double _ModeFlags;
            public double ModeFlags { get { return _ModeFlags; } set { if (value == _ModeFlags) return; _ModeFlags = value; OnPropertyChanged(); } }

            private double _DataRate;
            public double DataRate { get { return _DataRate; } set { if (value == _DataRate) return; _DataRate = value; OnPropertyChanged(); } }

            private double _NotiRate;
            public double NotiRate { get { return _NotiRate; } set { if (value == _NotiRate) return; _NotiRate = value; OnPropertyChanged(); } }

            private double _AlertLog;
            public double AlertLog { get { return _AlertLog; } set { if (value == _AlertLog) return; _AlertLog = value; OnPropertyChanged(); } }

            private double _AlertHi;
            public double AlertHi { get { return _AlertHi; } set { if (value == _AlertHi) return; _AlertHi = value; OnPropertyChanged(); } }

            private double _AlertFaults;
            public double AlertFaults { get { return _AlertFaults; } set { if (value == _AlertFaults) return; _AlertFaults = value; OnPropertyChanged(); } }

            private double _Reserved;
            public double Reserved { get { return _Reserved; } set { if (value == _Reserved) return; _Reserved = value; OnPropertyChanged(); } }

            private double _Range;
            public double Range { get { return _Range; } set { if (value == _Range) return; _Range = value; OnPropertyChanged(); } }

            private double _Resolution;
            public double Resolution { get { return _Resolution; } set { if (value == _Resolution) return; _Resolution = value; OnPropertyChanged(); } }

            private double _AlertResetCount;
            public double AlertResetCount { get { return _AlertResetCount; } set { if (value == _AlertResetCount) return; _AlertResetCount = value; OnPropertyChanged(); } }

            private double _AertResetDiff;
            public double AertResetDiff { get { return _AertResetDiff; } set { if (value == _AertResetDiff) return; _AertResetDiff = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Light_ConfigRecord> Light_ConfigRecordData { get; } = new DataCollection<Light_ConfigRecord>();
        private void OnLight_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Light_ConfigRecordData.Count == 0)
                {
                    Light_ConfigRecordData.AddRecord(new Light_ConfigRecord());
                }
                Light_ConfigRecordData[Light_ConfigRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountLight_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Light_ConfigRecordData.MaxLength = value;


        }

        private void OnAlgorithmLight_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Light_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyLight_Config(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Enable,ModeFlags,DataRate,NotiRate,AlertLog,AlertHi,AlertFaults,Reserved,Range,Resolution,AlertResetCount,AertResetDiff,Notes\n");
            foreach (var row in Light_ConfigRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Enable},{row.ModeFlags},{row.DataRate},{row.NotiRate},{row.AlertLog},{row.AlertHi},{row.AlertFaults},{row.Reserved},{row.Range},{row.Resolution},{row.AlertResetCount},{row.AertResetDiff},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadLight_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadLight_Config();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Light_Config");
                    return;
                }

                var record = new Light_ConfigRecord();

                var Enable = valueList.GetValue("Enable");
                if (Enable.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Enable.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Enable = (double)Enable.AsDouble;
                    Light_Config_Enable.Text = record.Enable.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var ModeFlags = valueList.GetValue("ModeFlags");
                if (ModeFlags.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ModeFlags.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ModeFlags = (double)ModeFlags.AsDouble;
                    Light_Config_ModeFlags.Text = record.ModeFlags.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DataRate = valueList.GetValue("DataRate");
                if (DataRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DataRate.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DataRate = (double)DataRate.AsDouble;
                    Light_Config_DataRate.Text = record.DataRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var NotiRate = valueList.GetValue("NotiRate");
                if (NotiRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || NotiRate.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.NotiRate = (double)NotiRate.AsDouble;
                    Light_Config_NotiRate.Text = record.NotiRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var AlertLog = valueList.GetValue("AlertLog");
                if (AlertLog.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AlertLog.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AlertLog = (double)AlertLog.AsDouble;
                    Light_Config_AlertLog.Text = record.AlertLog.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var AlertHi = valueList.GetValue("AlertHi");
                if (AlertHi.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AlertHi.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AlertHi = (double)AlertHi.AsDouble;
                    Light_Config_AlertHi.Text = record.AlertHi.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var AlertFaults = valueList.GetValue("AlertFaults");
                if (AlertFaults.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AlertFaults.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AlertFaults = (double)AlertFaults.AsDouble;
                    Light_Config_AlertFaults.Text = record.AlertFaults.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Reserved = valueList.GetValue("Reserved");
                if (Reserved.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Reserved.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Reserved = (double)Reserved.AsDouble;
                    Light_Config_Reserved.Text = record.Reserved.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Range = valueList.GetValue("Range");
                if (Range.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Range.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Range = (double)Range.AsDouble;
                    Light_Config_Range.Text = record.Range.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Resolution = valueList.GetValue("Resolution");
                if (Resolution.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Resolution.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Resolution = (double)Resolution.AsDouble;
                    Light_Config_Resolution.Text = record.Resolution.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var AlertResetCount = valueList.GetValue("AlertResetCount");
                if (AlertResetCount.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AlertResetCount.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AlertResetCount = (double)AlertResetCount.AsDouble;
                    Light_Config_AlertResetCount.Text = record.AlertResetCount.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var AertResetDiff = valueList.GetValue("AertResetDiff");
                if (AertResetDiff.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AertResetDiff.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AertResetDiff = (double)AertResetDiff.AsDouble;
                    Light_Config_AertResetDiff.Text = record.AertResetDiff.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Light_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Unknown1Record : INotifyPropertyChanged
        {
            public Unknown1Record()
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

            private string _Unknown1;
            public string Unknown1 { get { return _Unknown1; } set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Unknown1Record> Unknown1RecordData { get; } = new DataCollection<Unknown1Record>();
        private void OnUnknown1_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Unknown1RecordData.Count == 0)
                {
                    Unknown1RecordData.AddRecord(new Unknown1Record());
                }
                Unknown1RecordData[Unknown1RecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUnknown1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown1RecordData.MaxLength = value;


        }

        private void OnAlgorithmUnknown1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown1RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUnknown1(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown1,Notes\n");
            foreach (var row in Unknown1RecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown1},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUnknown1(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUnknown1();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Unknown1");
                    return;
                }

                var record = new Unknown1Record();

                var Unknown1 = valueList.GetValue("Unknown1");
                if (Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown1 = (string)Unknown1.AsString;
                    Unknown1_Unknown1.Text = record.Unknown1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Unknown1RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyUnknown1Settings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Unknown1NotifyIndex = 0;
        bool Unknown1NotifySetup = false;
        private async void OnNotifyUnknown1(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Unknown1NotifySetup)
                {
                    Unknown1NotifySetup = true;
                    bleDevice.Unknown1Event += BleDevice_Unknown1Event;
                }
                var notifyType = NotifyUnknown1Settings[Unknown1NotifyIndex];
                Unknown1NotifyIndex = (Unknown1NotifyIndex + 1) % NotifyUnknown1Settings.Length;
                var result = await bleDevice.NotifyUnknown1Async(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Unknown1Event(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Unknown1Record();

                    var Unknown1 = valueList.GetValue("Unknown1");
                    if (Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown1 = (string)Unknown1.AsString;
                        Unknown1_Unknown1.Text = record.Unknown1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Unknown1RecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteUnknown2(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown2;
                var parsedUnknown2 = Utilities.Parsers.TryParseBytes(Unknown2_Unknown2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown2);
                if (!parsedUnknown2)
                {
                    parseError = "Unknown2";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUnknown2(Unknown2);
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

        public class Unknown2Record : INotifyPropertyChanged
        {
            public Unknown2Record()
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

            private string _Unknown2;
            public string Unknown2 { get { return _Unknown2; } set { if (value == _Unknown2) return; _Unknown2 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Unknown2Record> Unknown2RecordData { get; } = new DataCollection<Unknown2Record>();
        private void OnUnknown2_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Unknown2RecordData.Count == 0)
                {
                    Unknown2RecordData.AddRecord(new Unknown2Record());
                }
                Unknown2RecordData[Unknown2RecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUnknown2(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown2RecordData.MaxLength = value;


        }

        private void OnAlgorithmUnknown2(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown2RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUnknown2(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown2,Notes\n");
            foreach (var row in Unknown2RecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown2},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUnknown2(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUnknown2();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Unknown2");
                    return;
                }

                var record = new Unknown2Record();

                var Unknown2 = valueList.GetValue("Unknown2");
                if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown2 = (string)Unknown2.AsString;
                    Unknown2_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Unknown2RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyUnknown2Settings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Unknown2NotifyIndex = 0;
        bool Unknown2NotifySetup = false;
        private async void OnNotifyUnknown2(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Unknown2NotifySetup)
                {
                    Unknown2NotifySetup = true;
                    bleDevice.Unknown2Event += BleDevice_Unknown2Event;
                }
                var notifyType = NotifyUnknown2Settings[Unknown2NotifyIndex];
                Unknown2NotifyIndex = (Unknown2NotifyIndex + 1) % NotifyUnknown2Settings.Length;
                var result = await bleDevice.NotifyUnknown2Async(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Unknown2Event(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Unknown2Record();

                    var Unknown2 = valueList.GetValue("Unknown2");
                    if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown2 = (string)Unknown2.AsString;
                        Unknown2_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Unknown2RecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteUnknown3(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown3;
                var parsedUnknown3 = Utilities.Parsers.TryParseBytes(Unknown3_Unknown3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown3);
                if (!parsedUnknown3)
                {
                    parseError = "Unknown3";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUnknown3(Unknown3);
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

        public class Unknown3Record : INotifyPropertyChanged
        {
            public Unknown3Record()
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

            private string _Unknown3;
            public string Unknown3 { get { return _Unknown3; } set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Unknown3Record> Unknown3RecordData { get; } = new DataCollection<Unknown3Record>();
        private void OnUnknown3_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Unknown3RecordData.Count == 0)
                {
                    Unknown3RecordData.AddRecord(new Unknown3Record());
                }
                Unknown3RecordData[Unknown3RecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUnknown3(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown3RecordData.MaxLength = value;


        }

        private void OnAlgorithmUnknown3(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown3RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUnknown3(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown3,Notes\n");
            foreach (var row in Unknown3RecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUnknown3(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUnknown3();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Unknown3");
                    return;
                }

                var record = new Unknown3Record();

                var Unknown3 = valueList.GetValue("Unknown3");
                if (Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown3 = (string)Unknown3.AsString;
                    Unknown3_Unknown3.Text = record.Unknown3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Unknown3RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Temperature


        private async void OnWriteTemperature_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown0;
                var parsedUnknown0 = Utilities.Parsers.TryParseBytes(Temperature_Config_Unknown0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown0);
                if (!parsedUnknown0)
                {
                    parseError = "Unknown0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteTemperature_Config(Unknown0);
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

        public class Temperature_ConfigRecord : INotifyPropertyChanged
        {
            public Temperature_ConfigRecord()
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

            private string _Unknown0;
            public string Unknown0 { get { return _Unknown0; } set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Temperature_ConfigRecord> Temperature_ConfigRecordData { get; } = new DataCollection<Temperature_ConfigRecord>();
        private void OnTemperature_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Temperature_ConfigRecordData.Count == 0)
                {
                    Temperature_ConfigRecordData.AddRecord(new Temperature_ConfigRecord());
                }
                Temperature_ConfigRecordData[Temperature_ConfigRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTemperature_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_ConfigRecordData.MaxLength = value;


        }

        private void OnAlgorithmTemperature_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTemperature_Config(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown0,Notes\n");
            foreach (var row in Temperature_ConfigRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTemperature_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTemperature_Config();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Temperature_Config");
                    return;
                }

                var record = new Temperature_ConfigRecord();

                var Unknown0 = valueList.GetValue("Unknown0");
                if (Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown0 = (string)Unknown0.AsString;
                    Temperature_Config_Unknown0.Text = record.Unknown0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Temperature_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Temperature_DataRecord : INotifyPropertyChanged
        {
            public Temperature_DataRecord()
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

            private string _Unknown1;
            public string Unknown1 { get { return _Unknown1; } set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Temperature_DataRecord> Temperature_DataRecordData { get; } = new DataCollection<Temperature_DataRecord>();
        private void OnTemperature_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Temperature_DataRecordData.Count == 0)
                {
                    Temperature_DataRecordData.AddRecord(new Temperature_DataRecord());
                }
                Temperature_DataRecordData[Temperature_DataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTemperature_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_DataRecordData.MaxLength = value;


        }

        private void OnAlgorithmTemperature_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTemperature_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown1,Notes\n");
            foreach (var row in Temperature_DataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown1},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTemperature_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTemperature_Data();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Temperature_Data");
                    return;
                }

                var record = new Temperature_DataRecord();

                var Unknown1 = valueList.GetValue("Unknown1");
                if (Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown1 = (string)Unknown1.AsString;
                    Temperature_Data_Unknown1.Text = record.Unknown1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Temperature_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyTemperature_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Temperature_DataNotifyIndex = 0;
        bool Temperature_DataNotifySetup = false;
        private async void OnNotifyTemperature_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Temperature_DataNotifySetup)
                {
                    Temperature_DataNotifySetup = true;
                    bleDevice.Temperature_DataEvent += BleDevice_Temperature_DataEvent;
                }
                var notifyType = NotifyTemperature_DataSettings[Temperature_DataNotifyIndex];
                Temperature_DataNotifyIndex = (Temperature_DataNotifyIndex + 1) % NotifyTemperature_DataSettings.Length;
                var result = await bleDevice.NotifyTemperature_DataAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Temperature_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Temperature_DataRecord();

                    var Unknown1 = valueList.GetValue("Unknown1");
                    if (Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown1 = (string)Unknown1.AsString;
                        Temperature_Data_Unknown1.Text = record.Unknown1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Temperature_DataRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteTemperature_Alert(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown2;
                var parsedUnknown2 = Utilities.Parsers.TryParseBytes(Temperature_Alert_Unknown2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown2);
                if (!parsedUnknown2)
                {
                    parseError = "Unknown2";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteTemperature_Alert(Unknown2);
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

        public class Temperature_AlertRecord : INotifyPropertyChanged
        {
            public Temperature_AlertRecord()
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

            private string _Unknown2;
            public string Unknown2 { get { return _Unknown2; } set { if (value == _Unknown2) return; _Unknown2 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Temperature_AlertRecord> Temperature_AlertRecordData { get; } = new DataCollection<Temperature_AlertRecord>();
        private void OnTemperature_Alert_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Temperature_AlertRecordData.Count == 0)
                {
                    Temperature_AlertRecordData.AddRecord(new Temperature_AlertRecord());
                }
                Temperature_AlertRecordData[Temperature_AlertRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTemperature_Alert(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_AlertRecordData.MaxLength = value;


        }

        private void OnAlgorithmTemperature_Alert(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_AlertRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTemperature_Alert(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown2,Notes\n");
            foreach (var row in Temperature_AlertRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown2},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTemperature_Alert(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTemperature_Alert();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Temperature_Alert");
                    return;
                }

                var record = new Temperature_AlertRecord();

                var Unknown2 = valueList.GetValue("Unknown2");
                if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown2 = (string)Unknown2.AsString;
                    Temperature_Alert_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Temperature_AlertRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyTemperature_AlertSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Temperature_AlertNotifyIndex = 0;
        bool Temperature_AlertNotifySetup = false;
        private async void OnNotifyTemperature_Alert(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Temperature_AlertNotifySetup)
                {
                    Temperature_AlertNotifySetup = true;
                    bleDevice.Temperature_AlertEvent += BleDevice_Temperature_AlertEvent;
                }
                var notifyType = NotifyTemperature_AlertSettings[Temperature_AlertNotifyIndex];
                Temperature_AlertNotifyIndex = (Temperature_AlertNotifyIndex + 1) % NotifyTemperature_AlertSettings.Length;
                var result = await bleDevice.NotifyTemperature_AlertAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Temperature_AlertEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Temperature_AlertRecord();

                    var Unknown2 = valueList.GetValue("Unknown2");
                    if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown2 = (string)Unknown2.AsString;
                        Temperature_Alert_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Temperature_AlertRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteTemperature_Status(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown3;
                var parsedUnknown3 = Utilities.Parsers.TryParseBytes(Temperature_Status_Unknown3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown3);
                if (!parsedUnknown3)
                {
                    parseError = "Unknown3";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteTemperature_Status(Unknown3);
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

        public class Temperature_StatusRecord : INotifyPropertyChanged
        {
            public Temperature_StatusRecord()
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

            private string _Unknown3;
            public string Unknown3 { get { return _Unknown3; } set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Temperature_StatusRecord> Temperature_StatusRecordData { get; } = new DataCollection<Temperature_StatusRecord>();
        private void OnTemperature_Status_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Temperature_StatusRecordData.Count == 0)
                {
                    Temperature_StatusRecordData.AddRecord(new Temperature_StatusRecord());
                }
                Temperature_StatusRecordData[Temperature_StatusRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTemperature_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_StatusRecordData.MaxLength = value;


        }

        private void OnAlgorithmTemperature_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Temperature_StatusRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTemperature_Status(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown3,Notes\n");
            foreach (var row in Temperature_StatusRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTemperature_Status(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTemperature_Status();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Temperature_Status");
                    return;
                }

                var record = new Temperature_StatusRecord();

                var Unknown3 = valueList.GetValue("Unknown3");
                if (Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown3 = (string)Unknown3.AsString;
                    Temperature_Status_Unknown3.Text = record.Unknown3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Temperature_StatusRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Pairing


        private async void OnWritePairing_Control_Status(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown0;
                var parsedUnknown0 = Utilities.Parsers.TryParseBytes(Pairing_Control_Status_Unknown0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown0);
                if (!parsedUnknown0)
                {
                    parseError = "Unknown0";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePairing_Control_Status(Unknown0);
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

        public class Pairing_Control_StatusRecord : INotifyPropertyChanged
        {
            public Pairing_Control_StatusRecord()
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

            private string _Unknown0;
            public string Unknown0 { get { return _Unknown0; } set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Pairing_Control_StatusRecord> Pairing_Control_StatusRecordData { get; } = new DataCollection<Pairing_Control_StatusRecord>();
        private void OnPairing_Control_Status_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Pairing_Control_StatusRecordData.Count == 0)
                {
                    Pairing_Control_StatusRecordData.AddRecord(new Pairing_Control_StatusRecord());
                }
                Pairing_Control_StatusRecordData[Pairing_Control_StatusRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPairing_Control_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_Control_StatusRecordData.MaxLength = value;


        }

        private void OnAlgorithmPairing_Control_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_Control_StatusRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPairing_Control_Status(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown0,Notes\n");
            foreach (var row in Pairing_Control_StatusRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPairing_Control_Status(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPairing_Control_Status();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Pairing_Control_Status");
                    return;
                }

                var record = new Pairing_Control_StatusRecord();

                var Unknown0 = valueList.GetValue("Unknown0");
                if (Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown0 = (string)Unknown0.AsString;
                    Pairing_Control_Status_Unknown0.Text = record.Unknown0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Pairing_Control_StatusRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyPairing_Control_StatusSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Pairing_Control_StatusNotifyIndex = 0;
        bool Pairing_Control_StatusNotifySetup = false;
        private async void OnNotifyPairing_Control_Status(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Pairing_Control_StatusNotifySetup)
                {
                    Pairing_Control_StatusNotifySetup = true;
                    bleDevice.Pairing_Control_StatusEvent += BleDevice_Pairing_Control_StatusEvent;
                }
                var notifyType = NotifyPairing_Control_StatusSettings[Pairing_Control_StatusNotifyIndex];
                Pairing_Control_StatusNotifyIndex = (Pairing_Control_StatusNotifyIndex + 1) % NotifyPairing_Control_StatusSettings.Length;
                var result = await bleDevice.NotifyPairing_Control_StatusAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Pairing_Control_StatusEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Pairing_Control_StatusRecord();

                    var Unknown0 = valueList.GetValue("Unknown0");
                    if (Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown0 = (string)Unknown0.AsString;
                        Pairing_Control_Status_Unknown0.Text = record.Unknown0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Pairing_Control_StatusRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWritePairing_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown1;
                var parsedUnknown1 = Utilities.Parsers.TryParseBytes(Pairing_Data_Unknown1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown1);
                if (!parsedUnknown1)
                {
                    parseError = "Unknown1";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePairing_Data(Unknown1);
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

        public class Pairing_DataRecord : INotifyPropertyChanged
        {
            public Pairing_DataRecord()
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

            private string _Unknown1;
            public string Unknown1 { get { return _Unknown1; } set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Pairing_DataRecord> Pairing_DataRecordData { get; } = new DataCollection<Pairing_DataRecord>();
        private void OnPairing_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Pairing_DataRecordData.Count == 0)
                {
                    Pairing_DataRecordData.AddRecord(new Pairing_DataRecord());
                }
                Pairing_DataRecordData[Pairing_DataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPairing_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_DataRecordData.MaxLength = value;


        }

        private void OnAlgorithmPairing_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPairing_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown1,Notes\n");
            foreach (var row in Pairing_DataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown1},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPairing_Data(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPairing_Data();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Pairing_Data");
                    return;
                }

                var record = new Pairing_DataRecord();

                var Unknown1 = valueList.GetValue("Unknown1");
                if (Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown1 = (string)Unknown1.AsString;
                    Pairing_Data_Unknown1.Text = record.Unknown1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Pairing_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWritePairing_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown2;
                var parsedUnknown2 = Utilities.Parsers.TryParseBytes(Pairing_Config_Unknown2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown2);
                if (!parsedUnknown2)
                {
                    parseError = "Unknown2";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePairing_Config(Unknown2);
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

        public class Pairing_ConfigRecord : INotifyPropertyChanged
        {
            public Pairing_ConfigRecord()
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

            private string _Unknown2;
            public string Unknown2 { get { return _Unknown2; } set { if (value == _Unknown2) return; _Unknown2 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Pairing_ConfigRecord> Pairing_ConfigRecordData { get; } = new DataCollection<Pairing_ConfigRecord>();
        private void OnPairing_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Pairing_ConfigRecordData.Count == 0)
                {
                    Pairing_ConfigRecordData.AddRecord(new Pairing_ConfigRecord());
                }
                Pairing_ConfigRecordData[Pairing_ConfigRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPairing_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_ConfigRecordData.MaxLength = value;


        }

        private void OnAlgorithmPairing_Config(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPairing_Config(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown2,Notes\n");
            foreach (var row in Pairing_ConfigRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown2},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPairing_Config(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPairing_Config();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Pairing_Config");
                    return;
                }

                var record = new Pairing_ConfigRecord();

                var Unknown2 = valueList.GetValue("Unknown2");
                if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown2 = (string)Unknown2.AsString;
                    Pairing_Config_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Pairing_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Pairing_AD_KeyRecord : INotifyPropertyChanged
        {
            public Pairing_AD_KeyRecord()
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

            private string _Unknown3;
            public string Unknown3 { get { return _Unknown3; } set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Pairing_AD_KeyRecord> Pairing_AD_KeyRecordData { get; } = new DataCollection<Pairing_AD_KeyRecord>();
        private void OnPairing_AD_Key_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Pairing_AD_KeyRecordData.Count == 0)
                {
                    Pairing_AD_KeyRecordData.AddRecord(new Pairing_AD_KeyRecord());
                }
                Pairing_AD_KeyRecordData[Pairing_AD_KeyRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPairing_AD_Key(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_AD_KeyRecordData.MaxLength = value;


        }

        private void OnAlgorithmPairing_AD_Key(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Pairing_AD_KeyRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPairing_AD_Key(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown3,Notes\n");
            foreach (var row in Pairing_AD_KeyRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPairing_AD_Key(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPairing_AD_Key();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Pairing_AD_Key");
                    return;
                }

                var record = new Pairing_AD_KeyRecord();

                var Unknown3 = valueList.GetValue("Unknown3");
                if (Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown3 = (string)Unknown3.AsString;
                    Pairing_AD_Key_Unknown3.Text = record.Unknown3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Pairing_AD_KeyRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyPairing_AD_KeySettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Pairing_AD_KeyNotifyIndex = 0;
        bool Pairing_AD_KeyNotifySetup = false;
        private async void OnNotifyPairing_AD_Key(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Pairing_AD_KeyNotifySetup)
                {
                    Pairing_AD_KeyNotifySetup = true;
                    bleDevice.Pairing_AD_KeyEvent += BleDevice_Pairing_AD_KeyEvent;
                }
                var notifyType = NotifyPairing_AD_KeySettings[Pairing_AD_KeyNotifyIndex];
                Pairing_AD_KeyNotifyIndex = (Pairing_AD_KeyNotifyIndex + 1) % NotifyPairing_AD_KeySettings.Length;
                var result = await bleDevice.NotifyPairing_AD_KeyAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Pairing_AD_KeyEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Pairing_AD_KeyRecord();

                    var Unknown3 = valueList.GetValue("Unknown3");
                    if (Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown3 = (string)Unknown3.AsString;
                        Pairing_AD_Key_Unknown3.Text = record.Unknown3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Pairing_AD_KeyRecordData.AddRecord(record);

                });
            }
        }

        // Functions for BR_Utilities


        private async void OnWriteUtility_DeviceName(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown0;
                var parsedUnknown0 = Utilities.Parsers.TryParseBytes(Utility_DeviceName_Unknown0.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown0);
                if (!parsedUnknown0)
                {
                    parseError = "Unknown0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_DeviceName(Unknown0);
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

        public class Utility_DeviceNameRecord : INotifyPropertyChanged
        {
            public Utility_DeviceNameRecord()
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

            private string _Unknown0;
            public string Unknown0 { get { return _Unknown0; } set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_DeviceNameRecord> Utility_DeviceNameRecordData { get; } = new DataCollection<Utility_DeviceNameRecord>();
        private void OnUtility_DeviceName_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_DeviceNameRecordData.Count == 0)
                {
                    Utility_DeviceNameRecordData.AddRecord(new Utility_DeviceNameRecord());
                }
                Utility_DeviceNameRecordData[Utility_DeviceNameRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_DeviceName(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_DeviceNameRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_DeviceName(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_DeviceNameRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_DeviceName(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown0,Notes\n");
            foreach (var row in Utility_DeviceNameRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_DeviceName(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_DeviceName();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_DeviceName");
                    return;
                }

                var record = new Utility_DeviceNameRecord();

                var Unknown0 = valueList.GetValue("Unknown0");
                if (Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown0 = (string)Unknown0.AsString;
                    Utility_DeviceName_Unknown0.Text = record.Unknown0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_DeviceNameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteUtility_Default_Conn_Param(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown1;
                var parsedUnknown1 = Utilities.Parsers.TryParseBytes(Utility_Default_Conn_Param_Unknown1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown1);
                if (!parsedUnknown1)
                {
                    parseError = "Unknown1";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_Default_Conn_Param(Unknown1);
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

        public class Utility_Default_Conn_ParamRecord : INotifyPropertyChanged
        {
            public Utility_Default_Conn_ParamRecord()
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

            private string _Unknown1;
            public string Unknown1 { get { return _Unknown1; } set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_Default_Conn_ParamRecord> Utility_Default_Conn_ParamRecordData { get; } = new DataCollection<Utility_Default_Conn_ParamRecord>();
        private void OnUtility_Default_Conn_Param_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_Default_Conn_ParamRecordData.Count == 0)
                {
                    Utility_Default_Conn_ParamRecordData.AddRecord(new Utility_Default_Conn_ParamRecord());
                }
                Utility_Default_Conn_ParamRecordData[Utility_Default_Conn_ParamRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_Default_Conn_Param(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Default_Conn_ParamRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_Default_Conn_Param(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Default_Conn_ParamRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_Default_Conn_Param(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown1,Notes\n");
            foreach (var row in Utility_Default_Conn_ParamRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown1},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_Default_Conn_Param(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_Default_Conn_Param();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_Default_Conn_Param");
                    return;
                }

                var record = new Utility_Default_Conn_ParamRecord();

                var Unknown1 = valueList.GetValue("Unknown1");
                if (Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown1 = (string)Unknown1.AsString;
                    Utility_Default_Conn_Param_Unknown1.Text = record.Unknown1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_Default_Conn_ParamRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteUtility_Curr_Conn_Param(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown2;
                var parsedUnknown2 = Utilities.Parsers.TryParseBytes(Utility_Curr_Conn_Param_Unknown2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown2);
                if (!parsedUnknown2)
                {
                    parseError = "Unknown2";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_Curr_Conn_Param(Unknown2);
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

        public class Utility_Curr_Conn_ParamRecord : INotifyPropertyChanged
        {
            public Utility_Curr_Conn_ParamRecord()
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

            private string _Unknown2;
            public string Unknown2 { get { return _Unknown2; } set { if (value == _Unknown2) return; _Unknown2 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_Curr_Conn_ParamRecord> Utility_Curr_Conn_ParamRecordData { get; } = new DataCollection<Utility_Curr_Conn_ParamRecord>();
        private void OnUtility_Curr_Conn_Param_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_Curr_Conn_ParamRecordData.Count == 0)
                {
                    Utility_Curr_Conn_ParamRecordData.AddRecord(new Utility_Curr_Conn_ParamRecord());
                }
                Utility_Curr_Conn_ParamRecordData[Utility_Curr_Conn_ParamRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_Curr_Conn_Param(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Curr_Conn_ParamRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_Curr_Conn_Param(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Curr_Conn_ParamRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_Curr_Conn_Param(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown2,Notes\n");
            foreach (var row in Utility_Curr_Conn_ParamRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown2},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_Curr_Conn_Param(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_Curr_Conn_Param();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_Curr_Conn_Param");
                    return;
                }

                var record = new Utility_Curr_Conn_ParamRecord();

                var Unknown2 = valueList.GetValue("Unknown2");
                if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown2 = (string)Unknown2.AsString;
                    Utility_Curr_Conn_Param_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_Curr_Conn_ParamRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyUtility_Curr_Conn_ParamSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Utility_Curr_Conn_ParamNotifyIndex = 0;
        bool Utility_Curr_Conn_ParamNotifySetup = false;
        private async void OnNotifyUtility_Curr_Conn_Param(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Utility_Curr_Conn_ParamNotifySetup)
                {
                    Utility_Curr_Conn_ParamNotifySetup = true;
                    bleDevice.Utility_Curr_Conn_ParamEvent += BleDevice_Utility_Curr_Conn_ParamEvent;
                }
                var notifyType = NotifyUtility_Curr_Conn_ParamSettings[Utility_Curr_Conn_ParamNotifyIndex];
                Utility_Curr_Conn_ParamNotifyIndex = (Utility_Curr_Conn_ParamNotifyIndex + 1) % NotifyUtility_Curr_Conn_ParamSettings.Length;
                var result = await bleDevice.NotifyUtility_Curr_Conn_ParamAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Utility_Curr_Conn_ParamEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Utility_Curr_Conn_ParamRecord();

                    var Unknown2 = valueList.GetValue("Unknown2");
                    if (Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Unknown2 = (string)Unknown2.AsString;
                        Utility_Curr_Conn_Param_Unknown2.Text = record.Unknown2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Utility_Curr_Conn_ParamRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteUtility_RF_Power(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown3;
                var parsedUnknown3 = Utilities.Parsers.TryParseBytes(Utility_RF_Power_Unknown3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown3);
                if (!parsedUnknown3)
                {
                    parseError = "Unknown3";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_RF_Power(Unknown3);
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

        public class Utility_RF_PowerRecord : INotifyPropertyChanged
        {
            public Utility_RF_PowerRecord()
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

            private string _Unknown3;
            public string Unknown3 { get { return _Unknown3; } set { if (value == _Unknown3) return; _Unknown3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_RF_PowerRecord> Utility_RF_PowerRecordData { get; } = new DataCollection<Utility_RF_PowerRecord>();
        private void OnUtility_RF_Power_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_RF_PowerRecordData.Count == 0)
                {
                    Utility_RF_PowerRecordData.AddRecord(new Utility_RF_PowerRecord());
                }
                Utility_RF_PowerRecordData[Utility_RF_PowerRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_RF_Power(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_RF_PowerRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_RF_Power(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_RF_PowerRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_RF_Power(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown3,Notes\n");
            foreach (var row in Utility_RF_PowerRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_RF_Power(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_RF_Power();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_RF_Power");
                    return;
                }

                var record = new Utility_RF_PowerRecord();

                var Unknown3 = valueList.GetValue("Unknown3");
                if (Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown3 = (string)Unknown3.AsString;
                    Utility_RF_Power_Unknown3.Text = record.Unknown3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_RF_PowerRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteUtility_Disconnect(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown4;
                var parsedUnknown4 = Utilities.Parsers.TryParseBytes(Utility_Disconnect_Unknown4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown4);
                if (!parsedUnknown4)
                {
                    parseError = "Unknown4";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_Disconnect(Unknown4);
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

        public class Utility_Public_AddressRecord : INotifyPropertyChanged
        {
            public Utility_Public_AddressRecord()
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

            private string _Unknown5;
            public string Unknown5 { get { return _Unknown5; } set { if (value == _Unknown5) return; _Unknown5 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_Public_AddressRecord> Utility_Public_AddressRecordData { get; } = new DataCollection<Utility_Public_AddressRecord>();
        private void OnUtility_Public_Address_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_Public_AddressRecordData.Count == 0)
                {
                    Utility_Public_AddressRecordData.AddRecord(new Utility_Public_AddressRecord());
                }
                Utility_Public_AddressRecordData[Utility_Public_AddressRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_Public_Address(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Public_AddressRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_Public_Address(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Public_AddressRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_Public_Address(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown5,Notes\n");
            foreach (var row in Utility_Public_AddressRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown5},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_Public_Address(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_Public_Address();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_Public_Address");
                    return;
                }

                var record = new Utility_Public_AddressRecord();

                var Unknown5 = valueList.GetValue("Unknown5");
                if (Unknown5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown5 = (string)Unknown5.AsString;
                    Utility_Public_Address_Unknown5.Text = record.Unknown5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_Public_AddressRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Utility_Config_CounterRecord : INotifyPropertyChanged
        {
            public Utility_Config_CounterRecord()
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

            private string _Unknown6;
            public string Unknown6 { get { return _Unknown6; } set { if (value == _Unknown6) return; _Unknown6 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_Config_CounterRecord> Utility_Config_CounterRecordData { get; } = new DataCollection<Utility_Config_CounterRecord>();
        private void OnUtility_Config_Counter_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_Config_CounterRecordData.Count == 0)
                {
                    Utility_Config_CounterRecordData.AddRecord(new Utility_Config_CounterRecord());
                }
                Utility_Config_CounterRecordData[Utility_Config_CounterRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_Config_Counter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Config_CounterRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_Config_Counter(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Config_CounterRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_Config_Counter(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown6,Notes\n");
            foreach (var row in Utility_Config_CounterRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown6},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_Config_Counter(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_Config_Counter();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_Config_Counter");
                    return;
                }

                var record = new Utility_Config_CounterRecord();

                var Unknown6 = valueList.GetValue("Unknown6");
                if (Unknown6.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown6.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown6 = (string)Unknown6.AsString;
                    Utility_Config_Counter_Unknown6.Text = record.Unknown6.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_Config_CounterRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteUtility_Advertising_Param(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown7;
                var parsedUnknown7 = Utilities.Parsers.TryParseBytes(Utility_Advertising_Param_Unknown7.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown7);
                if (!parsedUnknown7)
                {
                    parseError = "Unknown7";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_Advertising_Param(Unknown7);
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

        public class Utility_Advertising_ParamRecord : INotifyPropertyChanged
        {
            public Utility_Advertising_ParamRecord()
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

            private string _Unknown7;
            public string Unknown7 { get { return _Unknown7; } set { if (value == _Unknown7) return; _Unknown7 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_Advertising_ParamRecord> Utility_Advertising_ParamRecordData { get; } = new DataCollection<Utility_Advertising_ParamRecord>();
        private void OnUtility_Advertising_Param_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_Advertising_ParamRecordData.Count == 0)
                {
                    Utility_Advertising_ParamRecordData.AddRecord(new Utility_Advertising_ParamRecord());
                }
                Utility_Advertising_ParamRecordData[Utility_Advertising_ParamRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_Advertising_Param(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Advertising_ParamRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_Advertising_Param(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_Advertising_ParamRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_Advertising_Param(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown7,Notes\n");
            foreach (var row in Utility_Advertising_ParamRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown7},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_Advertising_Param(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_Advertising_Param();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_Advertising_Param");
                    return;
                }

                var record = new Utility_Advertising_ParamRecord();

                var Unknown7 = valueList.GetValue("Unknown7");
                if (Unknown7.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown7.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown7 = (string)Unknown7.AsString;
                    Utility_Advertising_Param_Unknown7.Text = record.Unknown7.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_Advertising_ParamRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteUtility_Unknown(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown8;
                var parsedUnknown8 = Utilities.Parsers.TryParseBytes(Utility_Unknown_Unknown8.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown8);
                if (!parsedUnknown8)
                {
                    parseError = "Unknown8";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_Unknown(Unknown8);
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

        public class Utility_UnknownRecord : INotifyPropertyChanged
        {
            public Utility_UnknownRecord()
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

            private string _Unknown8;
            public string Unknown8 { get { return _Unknown8; } set { if (value == _Unknown8) return; _Unknown8 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Utility_UnknownRecord> Utility_UnknownRecordData { get; } = new DataCollection<Utility_UnknownRecord>();
        private void OnUtility_Unknown_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Utility_UnknownRecordData.Count == 0)
                {
                    Utility_UnknownRecordData.AddRecord(new Utility_UnknownRecord());
                }
                Utility_UnknownRecordData[Utility_UnknownRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUtility_Unknown(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_UnknownRecordData.MaxLength = value;


        }

        private void OnAlgorithmUtility_Unknown(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Utility_UnknownRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUtility_Unknown(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown8,Notes\n");
            foreach (var row in Utility_UnknownRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown8},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUtility_Unknown(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUtility_Unknown();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Utility_Unknown");
                    return;
                }

                var record = new Utility_UnknownRecord();

                var Unknown8 = valueList.GetValue("Unknown8");
                if (Unknown8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown8.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown8 = (string)Unknown8.AsString;
                    Utility_Unknown_Unknown8.Text = record.Unknown8.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Utility_UnknownRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteUtility_Blink_LED(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte LEDs;
                var parsedLEDs = Utilities.Parsers.TryParseByte(Utility_Blink_LED_LEDs.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out LEDs);
                if (!parsedLEDs)
                {
                    parseError = "LEDs";
                }

                Byte NBlink;
                var parsedNBlink = Utilities.Parsers.TryParseByte(Utility_Blink_LED_NBlink.Text, System.Globalization.NumberStyles.None, null, out NBlink);
                if (!parsedNBlink)
                {
                    parseError = "NBlink";
                }

                Byte PercentOn;
                var parsedPercentOn = Utilities.Parsers.TryParseByte(Utility_Blink_LED_PercentOn.Text, System.Globalization.NumberStyles.None, null, out PercentOn);
                if (!parsedPercentOn)
                {
                    parseError = "PercentOn";
                }

                UInt16 Period;
                var parsedPeriod = Utilities.Parsers.TryParseUInt16(Utility_Blink_LED_Period.Text, System.Globalization.NumberStyles.None, null, out Period);
                if (!parsedPeriod)
                {
                    parseError = "Period";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUtility_Blink_LED(LEDs, NBlink, PercentOn, Period);
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


        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(true);
            SetStatusActive(false);
        }
    }
}
