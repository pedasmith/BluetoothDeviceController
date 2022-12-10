using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Names;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Utilities;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static BluetoothProtocols.TI_SensorTag_2541;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the SensorTag device
    /// </summary>
    public sealed partial class TI_SensorTag_2541Page : Page, HasId, ISetHandleStatus
    {
        public TI_SensorTag_2541Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "TI_SensorTag_2541";
        private string DeviceNameUser = "SensorTag";

        int ncommand = 0;
        TI_SensorTag_2541 bleDevice = new TI_SensorTag_2541();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive (true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive (false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            await DoReadDevice_Name();

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
        private void SetStatusActive (bool isActive)
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
                SetStatusActive (false);
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


        private async void OnReadDevice_Name(object sender, RoutedEventArgs e)
        {
            await DoReadDevice_Name();
        }

        private async Task DoReadDevice_Name()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDevice_Name();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Device_Name");
                    return;
                }
                
                var record = new Device_NameRecord();
                var Device_Name = valueList.GetValue("Device_Name");
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString || Device_Name.IsArray)
                {
                    record.Device_Name = (string)Device_Name.AsString;
                    Device_Name_Device_Name.Text = record.Device_Name.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Device_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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
            await DoReadAppearance();
        }

        private async Task DoReadAppearance()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAppearance();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Appearance");
                    return;
                }
                
                var record = new AppearanceRecord();
                var Appearance = valueList.GetValue("Appearance");
                if (Appearance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Appearance.CurrentType == BCBasic.BCValue.ValueType.IsString || Appearance.IsArray)
                {
                    record.Appearance = (double)Appearance.AsDouble;
                    Appearance_Appearance.Text = record.Appearance.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                AppearanceRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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
            await DoReadPrivacy();
        }

        private async Task DoReadPrivacy()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPrivacy();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Privacy");
                    return;
                }
                
                var record = new PrivacyRecord();
                var Privacy = valueList.GetValue("Privacy");
                if (Privacy.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Privacy.CurrentType == BCBasic.BCValue.ValueType.IsString || Privacy.IsArray)
                {
                    record.Privacy = (string)Privacy.AsString;
                    Privacy_Privacy.Text = record.Privacy.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                PrivacyRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Reconnect_AddressRecord : INotifyPropertyChanged
        {
            public Reconnect_AddressRecord()
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

            private string _ReconnectAddress;
            public string ReconnectAddress { get { return _ReconnectAddress; } set { if (value == _ReconnectAddress) return; _ReconnectAddress = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Reconnect_AddressRecord> Reconnect_AddressRecordData { get; } = new DataCollection<Reconnect_AddressRecord>();
    private void OnReconnect_Address_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Reconnect_AddressRecordData.Count == 0)
            {
                Reconnect_AddressRecordData.AddRecord(new Reconnect_AddressRecord());
            }
            Reconnect_AddressRecordData[Reconnect_AddressRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountReconnect_Address(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Reconnect_AddressRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmReconnect_Address(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Reconnect_AddressRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyReconnect_Address(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,ReconnectAddress,Notes\n");
        foreach (var row in Reconnect_AddressRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ReconnectAddress},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadReconnect_Address(object sender, RoutedEventArgs e)
        {
            await DoReadReconnect_Address();
        }

        private async Task DoReadReconnect_Address()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadReconnect_Address();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Reconnect_Address");
                    return;
                }
                
                var record = new Reconnect_AddressRecord();
                var ReconnectAddress = valueList.GetValue("ReconnectAddress");
                if (ReconnectAddress.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ReconnectAddress.CurrentType == BCBasic.BCValue.ValueType.IsString || ReconnectAddress.IsArray)
                {
                    record.ReconnectAddress = (string)ReconnectAddress.AsString;
                    Reconnect_Address_ReconnectAddress.Text = record.ReconnectAddress.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Reconnect_AddressRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickReconnect_Address(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteReconnect_Address (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteReconnect_Address(object sender, RoutedEventArgs e)
        {
            var text = Reconnect_Address_ReconnectAddress.Text;
            await DoWriteReconnect_Address (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteReconnect_Address(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes ReconnectAddress;
                // History: used to go into Reconnect_Address_ReconnectAddress.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedReconnectAddress = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out ReconnectAddress);
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
            await DoReadConnection_Parameter();
        }

        private async Task DoReadConnection_Parameter()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadConnection_Parameter();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Connection_Parameter");
                    return;
                }
                
                var record = new Connection_ParameterRecord();
                var ConnectionParameter = valueList.GetValue("ConnectionParameter");
                if (ConnectionParameter.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ConnectionParameter.CurrentType == BCBasic.BCValue.ValueType.IsString || ConnectionParameter.IsArray)
                {
                    record.ConnectionParameter = (string)ConnectionParameter.AsString;
                    Connection_Parameter_ConnectionParameter.Text = record.ConnectionParameter.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Connection_ParameterRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        // Functions for Device Info
        public class System_IDRecord : INotifyPropertyChanged
        {
            public System_IDRecord()
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

    public DataCollection<System_IDRecord> System_IDRecordData { get; } = new DataCollection<System_IDRecord>();
    private void OnSystem_ID_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (System_IDRecordData.Count == 0)
            {
                System_IDRecordData.AddRecord(new System_IDRecord());
            }
            System_IDRecordData[System_IDRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountSystem_ID(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        System_IDRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmSystem_ID(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        System_IDRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopySystem_ID(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in System_IDRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadSystem_ID(object sender, RoutedEventArgs e)
        {
            await DoReadSystem_ID();
        }

        private async Task DoReadSystem_ID()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSystem_ID();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read System_ID");
                    return;
                }
                
                var record = new System_IDRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    System_ID_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                System_IDRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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
            await DoReadModel_Number();
        }

        private async Task DoReadModel_Number()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadModel_Number();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Model_Number");
                    return;
                }
                
                var record = new Model_NumberRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Model_Number_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Model_NumberRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Serial_NumberRecord : INotifyPropertyChanged
        {
            public Serial_NumberRecord()
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

    public DataCollection<Serial_NumberRecord> Serial_NumberRecordData { get; } = new DataCollection<Serial_NumberRecord>();
    private void OnSerial_Number_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Serial_NumberRecordData.Count == 0)
            {
                Serial_NumberRecordData.AddRecord(new Serial_NumberRecord());
            }
            Serial_NumberRecordData[Serial_NumberRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountSerial_Number(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Serial_NumberRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmSerial_Number(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Serial_NumberRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopySerial_Number(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in Serial_NumberRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadSerial_Number(object sender, RoutedEventArgs e)
        {
            await DoReadSerial_Number();
        }

        private async Task DoReadSerial_Number()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSerial_Number();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Serial_Number");
                    return;
                }
                
                var record = new Serial_NumberRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Serial_Number_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Serial_NumberRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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
            await DoReadFirmware_Revision();
        }

        private async Task DoReadFirmware_Revision()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadFirmware_Revision();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Firmware_Revision");
                    return;
                }
                
                var record = new Firmware_RevisionRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Firmware_Revision_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Firmware_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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
            await DoReadHardware_Revision();
        }

        private async Task DoReadHardware_Revision()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHardware_Revision();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Hardware_Revision");
                    return;
                }
                
                var record = new Hardware_RevisionRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Hardware_Revision_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Hardware_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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
            await DoReadSoftware_Revision();
        }

        private async Task DoReadSoftware_Revision()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSoftware_Revision();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Software_Revision");
                    return;
                }
                
                var record = new Software_RevisionRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Software_Revision_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Software_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Manufacturer_NameRecord : INotifyPropertyChanged
        {
            public Manufacturer_NameRecord()
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

    public DataCollection<Manufacturer_NameRecord> Manufacturer_NameRecordData { get; } = new DataCollection<Manufacturer_NameRecord>();
    private void OnManufacturer_Name_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Manufacturer_NameRecordData.Count == 0)
            {
                Manufacturer_NameRecordData.AddRecord(new Manufacturer_NameRecord());
            }
            Manufacturer_NameRecordData[Manufacturer_NameRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountManufacturer_Name(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Manufacturer_NameRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmManufacturer_Name(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Manufacturer_NameRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyManufacturer_Name(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in Manufacturer_NameRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadManufacturer_Name(object sender, RoutedEventArgs e)
        {
            await DoReadManufacturer_Name();
        }

        private async Task DoReadManufacturer_Name()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadManufacturer_Name();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Manufacturer_Name");
                    return;
                }
                
                var record = new Manufacturer_NameRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Manufacturer_Name_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Manufacturer_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Regulatory_ListRecord : INotifyPropertyChanged
        {
            public Regulatory_ListRecord()
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

            private double _BodyType;
            public double BodyType { get { return _BodyType; } set { if (value == _BodyType) return; _BodyType = value; OnPropertyChanged(); } }
            private double _BodyStructure;
            public double BodyStructure { get { return _BodyStructure; } set { if (value == _BodyStructure) return; _BodyStructure = value; OnPropertyChanged(); } }
            private string _Data;
            public string Data { get { return _Data; } set { if (value == _Data) return; _Data = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Regulatory_ListRecord> Regulatory_ListRecordData { get; } = new DataCollection<Regulatory_ListRecord>();
    private void OnRegulatory_List_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Regulatory_ListRecordData.Count == 0)
            {
                Regulatory_ListRecordData.AddRecord(new Regulatory_ListRecord());
            }
            Regulatory_ListRecordData[Regulatory_ListRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountRegulatory_List(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Regulatory_ListRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmRegulatory_List(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Regulatory_ListRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyRegulatory_List(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,BodyType,BodyStructure,Data,Notes\n");
        foreach (var row in Regulatory_ListRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.BodyType},{row.BodyStructure},{row.Data},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadRegulatory_List(object sender, RoutedEventArgs e)
        {
            await DoReadRegulatory_List();
        }

        private async Task DoReadRegulatory_List()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadRegulatory_List();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Regulatory_List");
                    return;
                }
                
                var record = new Regulatory_ListRecord();
                var BodyType = valueList.GetValue("BodyType");
                if (BodyType.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BodyType.CurrentType == BCBasic.BCValue.ValueType.IsString || BodyType.IsArray)
                {
                    record.BodyType = (double)BodyType.AsDouble;
                    Regulatory_List_BodyType.Text = record.BodyType.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var BodyStructure = valueList.GetValue("BodyStructure");
                if (BodyStructure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BodyStructure.CurrentType == BCBasic.BCValue.ValueType.IsString || BodyStructure.IsArray)
                {
                    record.BodyStructure = (double)BodyStructure.AsDouble;
                    Regulatory_List_BodyStructure.Text = record.BodyStructure.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Data = valueList.GetValue("Data");
                if (Data.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Data.CurrentType == BCBasic.BCValue.ValueType.IsString || Data.IsArray)
                {
                    record.Data = (string)Data.AsString;
                    Regulatory_List_Data.Text = record.Data.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Regulatory_ListRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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
            await DoReadPnP_ID();
        }

        private async Task DoReadPnP_ID()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPnP_ID();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read PnP_ID");
                    return;
                }
                
                var record = new PnP_IDRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    PnP_ID_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                PnP_IDRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        // Functions for IR Service
        public class IR_DataRecord : INotifyPropertyChanged
        {
            public IR_DataRecord()
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

            private double _ObjTemp;
            public double ObjTemp { get { return _ObjTemp; } set { if (value == _ObjTemp) return; _ObjTemp = value; OnPropertyChanged(); } }
            private double _AmbientTemp;
            public double AmbientTemp { get { return _AmbientTemp; } set { if (value == _AmbientTemp) return; _AmbientTemp = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<IR_DataRecord> IR_DataRecordData { get; } = new DataCollection<IR_DataRecord>();
    private void OnIR_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (IR_DataRecordData.Count == 0)
            {
                IR_DataRecordData.AddRecord(new IR_DataRecord());
            }
            IR_DataRecordData[IR_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountIR_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_DataRecordData.MaxLength = value;

        IR_DataChart.RedrawYTime<IR_DataRecord>(IR_DataRecordData);

    }

    private void OnAlgorithmIR_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyIR_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,ObjTemp,AmbientTemp,Notes\n");
        foreach (var row in IR_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ObjTemp},{row.AmbientTemp},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyIR_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int IR_DataNotifyIndex = 0;
        bool IR_DataNotifySetup = false;
        private async void OnNotifyIR_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyIR_Data();
        }

        private async Task DoNotifyIR_Data()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!IR_DataNotifySetup)
                {
                    IR_DataNotifySetup = true;
                    bleDevice.IR_DataEvent += BleDevice_IR_DataEvent;
                }
                var notifyType = NotifyIR_DataSettings[IR_DataNotifyIndex];
                IR_DataNotifyIndex = (IR_DataNotifyIndex + 1) % NotifyIR_DataSettings.Length;
                var result = await bleDevice.NotifyIR_DataAsync(notifyType);
                await bleDevice.WriteIR_Service_ConfigNotify(notifyType);

                var EventTimeProperty = typeof(IR_DataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
                    typeof(IR_DataRecord).GetProperty("ObjTemp"),
                    typeof(IR_DataRecord).GetProperty("AmbientTemp"),

                };
                var names = new List<string>()
                {"ObjTemp","AmbientTemp",
                };
                IR_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                IR_DataChart.SetTitle("IR Data Chart");
                IR_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<IR_DataRecord>(addResult, IR_DataRecordData)",
        chartLineDefaults={
                        { "ObjTemp", new ChartLineDefaults() {
                            stroke="DarkRed",
                            }
                        },
                        { "AmbientTemp", new ChartLineDefaults() {
                            stroke="DarkGreen",
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

        private async void BleDevice_IR_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                valueList = bleDevice.ConvertIR();
                var record = new IR_DataRecord();
                var ObjTemp = valueList.GetValue("ObjTemp");
                if (ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || ObjTemp.IsArray)
                {
                    record.ObjTemp = (double)ObjTemp.AsDouble;
                    IR_Data_ObjTemp.Text = record.ObjTemp.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AmbientTemp = valueList.GetValue("AmbientTemp");
                if (AmbientTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AmbientTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || AmbientTemp.IsArray)
                {
                    record.AmbientTemp = (double)AmbientTemp.AsDouble;
                    IR_Data_AmbientTemp.Text = record.AmbientTemp.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = IR_DataRecordData.AddRecord(record);

                IR_DataChart.AddYTime<IR_DataRecord>(addResult, IR_DataRecordData);

                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadIR_Data(object sender, RoutedEventArgs e)
        {
            await DoReadIR_Data();
        }

        private async Task DoReadIR_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadIR_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read IR_Data");
                    return;
                }
                valueList = bleDevice.ConvertIR();
                var record = new IR_DataRecord();
                var ObjTemp = valueList.GetValue("ObjTemp");
                if (ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || ObjTemp.IsArray)
                {
                    record.ObjTemp = (double)ObjTemp.AsDouble;
                    IR_Data_ObjTemp.Text = record.ObjTemp.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AmbientTemp = valueList.GetValue("AmbientTemp");
                if (AmbientTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AmbientTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || AmbientTemp.IsArray)
                {
                    record.AmbientTemp = (double)AmbientTemp.AsDouble;
                    IR_Data_AmbientTemp.Text = record.AmbientTemp.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                IR_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class IR_Service_ConfigureRecord : INotifyPropertyChanged
        {
            public IR_Service_ConfigureRecord()
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

            private double _IRConfigure;
            public double IRConfigure { get { return _IRConfigure; } set { if (value == _IRConfigure) return; _IRConfigure = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<IR_Service_ConfigureRecord> IR_Service_ConfigureRecordData { get; } = new DataCollection<IR_Service_ConfigureRecord>();
    private void OnIR_Service_Configure_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (IR_Service_ConfigureRecordData.Count == 0)
            {
                IR_Service_ConfigureRecordData.AddRecord(new IR_Service_ConfigureRecord());
            }
            IR_Service_ConfigureRecordData[IR_Service_ConfigureRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountIR_Service_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_Service_ConfigureRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmIR_Service_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_Service_ConfigureRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyIR_Service_Configure(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,IRConfigure,Notes\n");
        foreach (var row in IR_Service_ConfigureRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.IRConfigure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadIR_Service_Configure(object sender, RoutedEventArgs e)
        {
            await DoReadIR_Service_Configure();
        }

        private async Task DoReadIR_Service_Configure()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadIR_Service_Configure();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read IR_Service_Configure");
                    return;
                }
                
                var record = new IR_Service_ConfigureRecord();
                var IRConfigure = valueList.GetValue("IRConfigure");
                if (IRConfigure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || IRConfigure.CurrentType == BCBasic.BCValue.ValueType.IsString || IRConfigure.IsArray)
                {
                    record.IRConfigure = (double)IRConfigure.AsDouble;
                    IR_Service_Configure_IRConfigure.Text = record.IRConfigure.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                IR_Service_ConfigureRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickIR_Service_Configure(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteIR_Service_Configure (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteIR_Service_Configure(object sender, RoutedEventArgs e)
        {
            var text = IR_Service_Configure_IRConfigure.Text;
            await DoWriteIR_Service_Configure (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteIR_Service_Configure(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte IRConfigure;
                // History: used to go into IR_Service_Configure_IRConfigure.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedIRConfigure = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out IRConfigure);
                if (!parsedIRConfigure)
                {
                    parseError = "IRConfigure";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteIR_Service_Configure(IRConfigure);
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

        public class IR_Service_PeriodRecord : INotifyPropertyChanged
        {
            public IR_Service_PeriodRecord()
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

            private double _IRPeriod;
            public double IRPeriod { get { return _IRPeriod; } set { if (value == _IRPeriod) return; _IRPeriod = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<IR_Service_PeriodRecord> IR_Service_PeriodRecordData { get; } = new DataCollection<IR_Service_PeriodRecord>();
    private void OnIR_Service_Period_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (IR_Service_PeriodRecordData.Count == 0)
            {
                IR_Service_PeriodRecordData.AddRecord(new IR_Service_PeriodRecord());
            }
            IR_Service_PeriodRecordData[IR_Service_PeriodRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountIR_Service_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_Service_PeriodRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmIR_Service_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_Service_PeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyIR_Service_Period(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,IRPeriod,Notes\n");
        foreach (var row in IR_Service_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.IRPeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadIR_Service_Period(object sender, RoutedEventArgs e)
        {
            await DoReadIR_Service_Period();
        }

        private async Task DoReadIR_Service_Period()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadIR_Service_Period();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read IR_Service_Period");
                    return;
                }
                
                var record = new IR_Service_PeriodRecord();
                var IRPeriod = valueList.GetValue("IRPeriod");
                if (IRPeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || IRPeriod.CurrentType == BCBasic.BCValue.ValueType.IsString || IRPeriod.IsArray)
                {
                    record.IRPeriod = (double)IRPeriod.AsDouble;
                    IR_Service_Period_IRPeriod.Text = record.IRPeriod.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                IR_Service_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickIR_Service_Period(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteIR_Service_Period (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteIR_Service_Period(object sender, RoutedEventArgs e)
        {
            var text = IR_Service_Period_IRPeriod.Text;
            await DoWriteIR_Service_Period (text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteIR_Service_Period(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte IRPeriod;
                // History: used to go into IR_Service_Period_IRPeriod.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedIRPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out IRPeriod);
                if (!parsedIRPeriod)
                {
                    parseError = "IRPeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteIR_Service_Period(IRPeriod);
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


        // Functions for Accelerometer
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

            private double _AccelX;
            public double AccelX { get { return _AccelX; } set { if (value == _AccelX) return; _AccelX = value; OnPropertyChanged(); } }
            private double _AccelY;
            public double AccelY { get { return _AccelY; } set { if (value == _AccelY) return; _AccelY = value; OnPropertyChanged(); } }
            private double _AccelZ;
            public double AccelZ { get { return _AccelZ; } set { if (value == _AccelZ) return; _AccelZ = value; OnPropertyChanged(); } }

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

        Accelerometer_DataChart.RedrawYTime<Accelerometer_DataRecord>(Accelerometer_DataRecordData);

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
        sb.Append("EventDate,EventTime,AccelX,AccelY,AccelZ,Notes\n");
        foreach (var row in Accelerometer_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AccelX},{row.AccelY},{row.AccelZ},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyAccelerometer_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Accelerometer_DataNotifyIndex = 0;
        bool Accelerometer_DataNotifySetup = false;
        private async void OnNotifyAccelerometer_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyAccelerometer_Data();
        }

        private async Task DoNotifyAccelerometer_Data()
        {
            SetStatusActive (true);
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
                await bleDevice.WriteAccelerometer_ConfigNotify(notifyType);

                var EventTimeProperty = typeof(Accelerometer_DataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
                    typeof(Accelerometer_DataRecord).GetProperty("AccelX"),
                    typeof(Accelerometer_DataRecord).GetProperty("AccelY"),
                    typeof(Accelerometer_DataRecord).GetProperty("AccelZ"),

                };
                var names = new List<string>()
                {"AccelX","AccelY","AccelZ",
                };
                Accelerometer_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                Accelerometer_DataChart.SetTitle("Accelerometer Data Chart");
                Accelerometer_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<Accelerometer_DataRecord>(addResult, Accelerometer_DataRecordData)",
chartDefaultMaxY=1.5,
chartDefaultMinY=-1.5,
        chartLineDefaults={
                        { "AccelX", new ChartLineDefaults() {
                            stroke="DarkRed",
                            }
                        },
                        { "AccelY", new ChartLineDefaults() {
                            stroke="DarkGreen",
                            }
                        },
                        { "AccelZ", new ChartLineDefaults() {
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

        private async void BleDevice_Accelerometer_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Accelerometer_DataRecord();
                var AccelX = valueList.GetValue("AccelX");
                if (AccelX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelX.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelX.IsArray)
                {
                    record.AccelX = (double)AccelX.AsDouble;
                    Accelerometer_Data_AccelX.Text = record.AccelX.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccelY = valueList.GetValue("AccelY");
                if (AccelY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelY.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelY.IsArray)
                {
                    record.AccelY = (double)AccelY.AsDouble;
                    Accelerometer_Data_AccelY.Text = record.AccelY.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccelZ = valueList.GetValue("AccelZ");
                if (AccelZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelZ.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelZ.IsArray)
                {
                    record.AccelZ = (double)AccelZ.AsDouble;
                    Accelerometer_Data_AccelZ.Text = record.AccelZ.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Accelerometer_DataRecordData.AddRecord(record);

                Accelerometer_DataChart.AddYTime<Accelerometer_DataRecord>(addResult, Accelerometer_DataRecordData);

                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadAccelerometer_Data(object sender, RoutedEventArgs e)
        {
            await DoReadAccelerometer_Data();
        }

        private async Task DoReadAccelerometer_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometer_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Accelerometer_Data");
                    return;
                }
                
                var record = new Accelerometer_DataRecord();
                var AccelX = valueList.GetValue("AccelX");
                if (AccelX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelX.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelX.IsArray)
                {
                    record.AccelX = (double)AccelX.AsDouble;
                    Accelerometer_Data_AccelX.Text = record.AccelX.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccelY = valueList.GetValue("AccelY");
                if (AccelY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelY.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelY.IsArray)
                {
                    record.AccelY = (double)AccelY.AsDouble;
                    Accelerometer_Data_AccelY.Text = record.AccelY.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccelZ = valueList.GetValue("AccelZ");
                if (AccelZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelZ.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelZ.IsArray)
                {
                    record.AccelZ = (double)AccelZ.AsDouble;
                    Accelerometer_Data_AccelZ.Text = record.AccelZ.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Accelerometer_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Accelerometer_ConfigureRecord : INotifyPropertyChanged
        {
            public Accelerometer_ConfigureRecord()
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

            private double _AccelerometerConfigure;
            public double AccelerometerConfigure { get { return _AccelerometerConfigure; } set { if (value == _AccelerometerConfigure) return; _AccelerometerConfigure = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Accelerometer_ConfigureRecord> Accelerometer_ConfigureRecordData { get; } = new DataCollection<Accelerometer_ConfigureRecord>();
    private void OnAccelerometer_Configure_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Accelerometer_ConfigureRecordData.Count == 0)
            {
                Accelerometer_ConfigureRecordData.AddRecord(new Accelerometer_ConfigureRecord());
            }
            Accelerometer_ConfigureRecordData[Accelerometer_ConfigureRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountAccelerometer_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Accelerometer_ConfigureRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmAccelerometer_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Accelerometer_ConfigureRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyAccelerometer_Configure(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,AccelerometerConfigure,Notes\n");
        foreach (var row in Accelerometer_ConfigureRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AccelerometerConfigure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadAccelerometer_Configure(object sender, RoutedEventArgs e)
        {
            await DoReadAccelerometer_Configure();
        }

        private async Task DoReadAccelerometer_Configure()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometer_Configure();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Accelerometer_Configure");
                    return;
                }
                
                var record = new Accelerometer_ConfigureRecord();
                var AccelerometerConfigure = valueList.GetValue("AccelerometerConfigure");
                if (AccelerometerConfigure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelerometerConfigure.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelerometerConfigure.IsArray)
                {
                    record.AccelerometerConfigure = (double)AccelerometerConfigure.AsDouble;
                    Accelerometer_Configure_AccelerometerConfigure.Text = record.AccelerometerConfigure.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Accelerometer_ConfigureRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickAccelerometer_Configure(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteAccelerometer_Configure (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteAccelerometer_Configure(object sender, RoutedEventArgs e)
        {
            var text = Accelerometer_Configure_AccelerometerConfigure.Text;
            await DoWriteAccelerometer_Configure (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteAccelerometer_Configure(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte AccelerometerConfigure;
                // History: used to go into Accelerometer_Configure_AccelerometerConfigure.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedAccelerometerConfigure = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out AccelerometerConfigure);
                if (!parsedAccelerometerConfigure)
                {
                    parseError = "AccelerometerConfigure";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAccelerometer_Configure(AccelerometerConfigure);
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

        public class Accelerometer_PeriodRecord : INotifyPropertyChanged
        {
            public Accelerometer_PeriodRecord()
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

            private double _AccelerometerPeriod;
            public double AccelerometerPeriod { get { return _AccelerometerPeriod; } set { if (value == _AccelerometerPeriod) return; _AccelerometerPeriod = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Accelerometer_PeriodRecord> Accelerometer_PeriodRecordData { get; } = new DataCollection<Accelerometer_PeriodRecord>();
    private void OnAccelerometer_Period_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Accelerometer_PeriodRecordData.Count == 0)
            {
                Accelerometer_PeriodRecordData.AddRecord(new Accelerometer_PeriodRecord());
            }
            Accelerometer_PeriodRecordData[Accelerometer_PeriodRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountAccelerometer_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Accelerometer_PeriodRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmAccelerometer_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Accelerometer_PeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyAccelerometer_Period(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,AccelerometerPeriod,Notes\n");
        foreach (var row in Accelerometer_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AccelerometerPeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadAccelerometer_Period(object sender, RoutedEventArgs e)
        {
            await DoReadAccelerometer_Period();
        }

        private async Task DoReadAccelerometer_Period()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometer_Period();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Accelerometer_Period");
                    return;
                }
                
                var record = new Accelerometer_PeriodRecord();
                var AccelerometerPeriod = valueList.GetValue("AccelerometerPeriod");
                if (AccelerometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelerometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsString || AccelerometerPeriod.IsArray)
                {
                    record.AccelerometerPeriod = (double)AccelerometerPeriod.AsDouble;
                    Accelerometer_Period_AccelerometerPeriod.Text = record.AccelerometerPeriod.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Accelerometer_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickAccelerometer_Period(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteAccelerometer_Period (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteAccelerometer_Period(object sender, RoutedEventArgs e)
        {
            var text = Accelerometer_Period_AccelerometerPeriod.Text;
            await DoWriteAccelerometer_Period (text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteAccelerometer_Period(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte AccelerometerPeriod;
                // History: used to go into Accelerometer_Period_AccelerometerPeriod.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedAccelerometerPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out AccelerometerPeriod);
                if (!parsedAccelerometerPeriod)
                {
                    parseError = "AccelerometerPeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAccelerometer_Period(AccelerometerPeriod);
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


        // Functions for Humidity
        public class Humidity_DataRecord : INotifyPropertyChanged
        {
            public Humidity_DataRecord()
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

            private double _Temp;
            public double Temp { get { return _Temp; } set { if (value == _Temp) return; _Temp = value; OnPropertyChanged(); } }
            private double _Humidity;
            public double Humidity { get { return _Humidity; } set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Humidity_DataRecord> Humidity_DataRecordData { get; } = new DataCollection<Humidity_DataRecord>();
    private void OnHumidity_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Humidity_DataRecordData.Count == 0)
            {
                Humidity_DataRecordData.AddRecord(new Humidity_DataRecord());
            }
            Humidity_DataRecordData[Humidity_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountHumidity_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_DataRecordData.MaxLength = value;

        Humidity_DataChart.RedrawYTime<Humidity_DataRecord>(Humidity_DataRecordData);

    }

    private void OnAlgorithmHumidity_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyHumidity_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Temp,Humidity,Notes\n");
        foreach (var row in Humidity_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Temp},{row.Humidity},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyHumidity_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Humidity_DataNotifyIndex = 0;
        bool Humidity_DataNotifySetup = false;
        private async void OnNotifyHumidity_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyHumidity_Data();
        }

        private async Task DoNotifyHumidity_Data()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Humidity_DataNotifySetup)
                {
                    Humidity_DataNotifySetup = true;
                    bleDevice.Humidity_DataEvent += BleDevice_Humidity_DataEvent;
                }
                var notifyType = NotifyHumidity_DataSettings[Humidity_DataNotifyIndex];
                Humidity_DataNotifyIndex = (Humidity_DataNotifyIndex + 1) % NotifyHumidity_DataSettings.Length;
                var result = await bleDevice.NotifyHumidity_DataAsync(notifyType);
                await bleDevice.WriteHumidity_ConfigNotify(notifyType);

                var EventTimeProperty = typeof(Humidity_DataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
                    typeof(Humidity_DataRecord).GetProperty("Temp"),
                    typeof(Humidity_DataRecord).GetProperty("Humidity"),

                };
                var names = new List<string>()
                {"Temp","Humidity",
                };
                Humidity_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                Humidity_DataChart.SetTitle("Humidity Data Chart");
                Humidity_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<Humidity_DataRecord>(addResult, Humidity_DataRecordData)",
chartDefaultMaxY=100,
chartDefaultMinY=0,
        chartLineDefaults={
                        { "Temp", new ChartLineDefaults() {
                            stroke="DarkGreen",
                            }
                        },
                        { "Humidity", new ChartLineDefaults() {
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

        private async void BleDevice_Humidity_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Humidity_DataRecord();
                var Temp = valueList.GetValue("Temp");
                if (Temp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Temp.CurrentType == BCBasic.BCValue.ValueType.IsString || Temp.IsArray)
                {
                    record.Temp = (double)Temp.AsDouble;
                    Humidity_Data_Temp.Text = record.Temp.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Humidity = valueList.GetValue("Humidity");
                if (Humidity.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Humidity.CurrentType == BCBasic.BCValue.ValueType.IsString || Humidity.IsArray)
                {
                    record.Humidity = (double)Humidity.AsDouble;
                    Humidity_Data_Humidity.Text = record.Humidity.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Humidity_DataRecordData.AddRecord(record);

                Humidity_DataChart.AddYTime<Humidity_DataRecord>(addResult, Humidity_DataRecordData);

                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadHumidity_Data(object sender, RoutedEventArgs e)
        {
            await DoReadHumidity_Data();
        }

        private async Task DoReadHumidity_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHumidity_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Humidity_Data");
                    return;
                }
                
                var record = new Humidity_DataRecord();
                var Temp = valueList.GetValue("Temp");
                if (Temp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Temp.CurrentType == BCBasic.BCValue.ValueType.IsString || Temp.IsArray)
                {
                    record.Temp = (double)Temp.AsDouble;
                    Humidity_Data_Temp.Text = record.Temp.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Humidity = valueList.GetValue("Humidity");
                if (Humidity.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Humidity.CurrentType == BCBasic.BCValue.ValueType.IsString || Humidity.IsArray)
                {
                    record.Humidity = (double)Humidity.AsDouble;
                    Humidity_Data_Humidity.Text = record.Humidity.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Humidity_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Humidity_ConfigureRecord : INotifyPropertyChanged
        {
            public Humidity_ConfigureRecord()
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

            private double _HumidityConfigure;
            public double HumidityConfigure { get { return _HumidityConfigure; } set { if (value == _HumidityConfigure) return; _HumidityConfigure = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Humidity_ConfigureRecord> Humidity_ConfigureRecordData { get; } = new DataCollection<Humidity_ConfigureRecord>();
    private void OnHumidity_Configure_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Humidity_ConfigureRecordData.Count == 0)
            {
                Humidity_ConfigureRecordData.AddRecord(new Humidity_ConfigureRecord());
            }
            Humidity_ConfigureRecordData[Humidity_ConfigureRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountHumidity_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_ConfigureRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmHumidity_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_ConfigureRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyHumidity_Configure(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,HumidityConfigure,Notes\n");
        foreach (var row in Humidity_ConfigureRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.HumidityConfigure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadHumidity_Configure(object sender, RoutedEventArgs e)
        {
            await DoReadHumidity_Configure();
        }

        private async Task DoReadHumidity_Configure()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHumidity_Configure();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Humidity_Configure");
                    return;
                }
                
                var record = new Humidity_ConfigureRecord();
                var HumidityConfigure = valueList.GetValue("HumidityConfigure");
                if (HumidityConfigure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || HumidityConfigure.CurrentType == BCBasic.BCValue.ValueType.IsString || HumidityConfigure.IsArray)
                {
                    record.HumidityConfigure = (double)HumidityConfigure.AsDouble;
                    Humidity_Configure_HumidityConfigure.Text = record.HumidityConfigure.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Humidity_ConfigureRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickHumidity_Configure(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteHumidity_Configure (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteHumidity_Configure(object sender, RoutedEventArgs e)
        {
            var text = Humidity_Configure_HumidityConfigure.Text;
            await DoWriteHumidity_Configure (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteHumidity_Configure(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte HumidityConfigure;
                // History: used to go into Humidity_Configure_HumidityConfigure.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedHumidityConfigure = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out HumidityConfigure);
                if (!parsedHumidityConfigure)
                {
                    parseError = "HumidityConfigure";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteHumidity_Configure(HumidityConfigure);
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

        public class Humidity_PeriodRecord : INotifyPropertyChanged
        {
            public Humidity_PeriodRecord()
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

            private double _HumidityPeriod;
            public double HumidityPeriod { get { return _HumidityPeriod; } set { if (value == _HumidityPeriod) return; _HumidityPeriod = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Humidity_PeriodRecord> Humidity_PeriodRecordData { get; } = new DataCollection<Humidity_PeriodRecord>();
    private void OnHumidity_Period_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Humidity_PeriodRecordData.Count == 0)
            {
                Humidity_PeriodRecordData.AddRecord(new Humidity_PeriodRecord());
            }
            Humidity_PeriodRecordData[Humidity_PeriodRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountHumidity_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_PeriodRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmHumidity_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_PeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyHumidity_Period(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,HumidityPeriod,Notes\n");
        foreach (var row in Humidity_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.HumidityPeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadHumidity_Period(object sender, RoutedEventArgs e)
        {
            await DoReadHumidity_Period();
        }

        private async Task DoReadHumidity_Period()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHumidity_Period();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Humidity_Period");
                    return;
                }
                
                var record = new Humidity_PeriodRecord();
                var HumidityPeriod = valueList.GetValue("HumidityPeriod");
                if (HumidityPeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || HumidityPeriod.CurrentType == BCBasic.BCValue.ValueType.IsString || HumidityPeriod.IsArray)
                {
                    record.HumidityPeriod = (double)HumidityPeriod.AsDouble;
                    Humidity_Period_HumidityPeriod.Text = record.HumidityPeriod.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Humidity_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickHumidity_Period(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteHumidity_Period (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteHumidity_Period(object sender, RoutedEventArgs e)
        {
            var text = Humidity_Period_HumidityPeriod.Text;
            await DoWriteHumidity_Period (text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteHumidity_Period(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte HumidityPeriod;
                // History: used to go into Humidity_Period_HumidityPeriod.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedHumidityPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out HumidityPeriod);
                if (!parsedHumidityPeriod)
                {
                    parseError = "HumidityPeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteHumidity_Period(HumidityPeriod);
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


        // Functions for Magnetometer
        public class Magnetometer_DataRecord : INotifyPropertyChanged
        {
            public Magnetometer_DataRecord()
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

    public DataCollection<Magnetometer_DataRecord> Magnetometer_DataRecordData { get; } = new DataCollection<Magnetometer_DataRecord>();
    private void OnMagnetometer_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Magnetometer_DataRecordData.Count == 0)
            {
                Magnetometer_DataRecordData.AddRecord(new Magnetometer_DataRecord());
            }
            Magnetometer_DataRecordData[Magnetometer_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountMagnetometer_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Magnetometer_DataRecordData.MaxLength = value;

        Magnetometer_DataChart.RedrawYTime<Magnetometer_DataRecord>(Magnetometer_DataRecordData);

    }

    private void OnAlgorithmMagnetometer_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Magnetometer_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyMagnetometer_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,X,Y,Z,Notes\n");
        foreach (var row in Magnetometer_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.X},{row.Y},{row.Z},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMagnetometer_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Magnetometer_DataNotifyIndex = 0;
        bool Magnetometer_DataNotifySetup = false;
        private async void OnNotifyMagnetometer_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyMagnetometer_Data();
        }

        private async Task DoNotifyMagnetometer_Data()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Magnetometer_DataNotifySetup)
                {
                    Magnetometer_DataNotifySetup = true;
                    bleDevice.Magnetometer_DataEvent += BleDevice_Magnetometer_DataEvent;
                }
                var notifyType = NotifyMagnetometer_DataSettings[Magnetometer_DataNotifyIndex];
                Magnetometer_DataNotifyIndex = (Magnetometer_DataNotifyIndex + 1) % NotifyMagnetometer_DataSettings.Length;
                var result = await bleDevice.NotifyMagnetometer_DataAsync(notifyType);
                await bleDevice.WriteMagnetometer_ConfigNotify(notifyType);

                var EventTimeProperty = typeof(Magnetometer_DataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
                    typeof(Magnetometer_DataRecord).GetProperty("X"),
                    typeof(Magnetometer_DataRecord).GetProperty("Y"),
                    typeof(Magnetometer_DataRecord).GetProperty("Z"),

                };
                var names = new List<string>()
                {"X","Y","Z",
                };
                Magnetometer_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                Magnetometer_DataChart.SetTitle("Magnetometer Data Chart");
                Magnetometer_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<Magnetometer_DataRecord>(addResult, Magnetometer_DataRecordData)",
chartDefaultMaxY=200,
chartDefaultMinY=-200,
        chartLineDefaults={
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

        private async void BleDevice_Magnetometer_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Magnetometer_DataRecord();
                var X = valueList.GetValue("X");
                if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString || X.IsArray)
                {
                    record.X = (double)X.AsDouble;
                    Magnetometer_Data_X.Text = record.X.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Y = valueList.GetValue("Y");
                if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString || Y.IsArray)
                {
                    record.Y = (double)Y.AsDouble;
                    Magnetometer_Data_Y.Text = record.Y.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Z = valueList.GetValue("Z");
                if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString || Z.IsArray)
                {
                    record.Z = (double)Z.AsDouble;
                    Magnetometer_Data_Z.Text = record.Z.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Magnetometer_DataRecordData.AddRecord(record);

                Magnetometer_DataChart.AddYTime<Magnetometer_DataRecord>(addResult, Magnetometer_DataRecordData);

                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadMagnetometer_Data(object sender, RoutedEventArgs e)
        {
            await DoReadMagnetometer_Data();
        }

        private async Task DoReadMagnetometer_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMagnetometer_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Magnetometer_Data");
                    return;
                }
                
                var record = new Magnetometer_DataRecord();
                var X = valueList.GetValue("X");
                if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString || X.IsArray)
                {
                    record.X = (double)X.AsDouble;
                    Magnetometer_Data_X.Text = record.X.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Y = valueList.GetValue("Y");
                if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString || Y.IsArray)
                {
                    record.Y = (double)Y.AsDouble;
                    Magnetometer_Data_Y.Text = record.Y.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Z = valueList.GetValue("Z");
                if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString || Z.IsArray)
                {
                    record.Z = (double)Z.AsDouble;
                    Magnetometer_Data_Z.Text = record.Z.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Magnetometer_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Magnetometer_ConfigureRecord : INotifyPropertyChanged
        {
            public Magnetometer_ConfigureRecord()
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

            private double _MagnetometerConfigure;
            public double MagnetometerConfigure { get { return _MagnetometerConfigure; } set { if (value == _MagnetometerConfigure) return; _MagnetometerConfigure = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Magnetometer_ConfigureRecord> Magnetometer_ConfigureRecordData { get; } = new DataCollection<Magnetometer_ConfigureRecord>();
    private void OnMagnetometer_Configure_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Magnetometer_ConfigureRecordData.Count == 0)
            {
                Magnetometer_ConfigureRecordData.AddRecord(new Magnetometer_ConfigureRecord());
            }
            Magnetometer_ConfigureRecordData[Magnetometer_ConfigureRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountMagnetometer_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Magnetometer_ConfigureRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmMagnetometer_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Magnetometer_ConfigureRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyMagnetometer_Configure(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,MagnetometerConfigure,Notes\n");
        foreach (var row in Magnetometer_ConfigureRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MagnetometerConfigure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadMagnetometer_Configure(object sender, RoutedEventArgs e)
        {
            await DoReadMagnetometer_Configure();
        }

        private async Task DoReadMagnetometer_Configure()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMagnetometer_Configure();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Magnetometer_Configure");
                    return;
                }
                
                var record = new Magnetometer_ConfigureRecord();
                var MagnetometerConfigure = valueList.GetValue("MagnetometerConfigure");
                if (MagnetometerConfigure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerConfigure.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerConfigure.IsArray)
                {
                    record.MagnetometerConfigure = (double)MagnetometerConfigure.AsDouble;
                    Magnetometer_Configure_MagnetometerConfigure.Text = record.MagnetometerConfigure.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Magnetometer_ConfigureRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickMagnetometer_Configure(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteMagnetometer_Configure (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteMagnetometer_Configure(object sender, RoutedEventArgs e)
        {
            var text = Magnetometer_Configure_MagnetometerConfigure.Text;
            await DoWriteMagnetometer_Configure (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteMagnetometer_Configure(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte MagnetometerConfigure;
                // History: used to go into Magnetometer_Configure_MagnetometerConfigure.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedMagnetometerConfigure = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out MagnetometerConfigure);
                if (!parsedMagnetometerConfigure)
                {
                    parseError = "MagnetometerConfigure";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMagnetometer_Configure(MagnetometerConfigure);
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

        public class Magnetometer_PeriodRecord : INotifyPropertyChanged
        {
            public Magnetometer_PeriodRecord()
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

            private double _MagnetometerPeriod;
            public double MagnetometerPeriod { get { return _MagnetometerPeriod; } set { if (value == _MagnetometerPeriod) return; _MagnetometerPeriod = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Magnetometer_PeriodRecord> Magnetometer_PeriodRecordData { get; } = new DataCollection<Magnetometer_PeriodRecord>();
    private void OnMagnetometer_Period_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Magnetometer_PeriodRecordData.Count == 0)
            {
                Magnetometer_PeriodRecordData.AddRecord(new Magnetometer_PeriodRecord());
            }
            Magnetometer_PeriodRecordData[Magnetometer_PeriodRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountMagnetometer_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Magnetometer_PeriodRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmMagnetometer_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Magnetometer_PeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyMagnetometer_Period(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,MagnetometerPeriod,Notes\n");
        foreach (var row in Magnetometer_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MagnetometerPeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadMagnetometer_Period(object sender, RoutedEventArgs e)
        {
            await DoReadMagnetometer_Period();
        }

        private async Task DoReadMagnetometer_Period()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMagnetometer_Period();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Magnetometer_Period");
                    return;
                }
                
                var record = new Magnetometer_PeriodRecord();
                var MagnetometerPeriod = valueList.GetValue("MagnetometerPeriod");
                if (MagnetometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerPeriod.IsArray)
                {
                    record.MagnetometerPeriod = (double)MagnetometerPeriod.AsDouble;
                    Magnetometer_Period_MagnetometerPeriod.Text = record.MagnetometerPeriod.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Magnetometer_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickMagnetometer_Period(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteMagnetometer_Period (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteMagnetometer_Period(object sender, RoutedEventArgs e)
        {
            var text = Magnetometer_Period_MagnetometerPeriod.Text;
            await DoWriteMagnetometer_Period (text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteMagnetometer_Period(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte MagnetometerPeriod;
                // History: used to go into Magnetometer_Period_MagnetometerPeriod.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedMagnetometerPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out MagnetometerPeriod);
                if (!parsedMagnetometerPeriod)
                {
                    parseError = "MagnetometerPeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMagnetometer_Period(MagnetometerPeriod);
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


        // Functions for Barometer
        public class Barometer_DataRecord : INotifyPropertyChanged
        {
            public Barometer_DataRecord()
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

            private double _TempRaw;
            public double TempRaw { get { return _TempRaw; } set { if (value == _TempRaw) return; _TempRaw = value; OnPropertyChanged(); } }
            private double _PressureRaw;
            public double PressureRaw { get { return _PressureRaw; } set { if (value == _PressureRaw) return; _PressureRaw = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Barometer_DataRecord> Barometer_DataRecordData { get; } = new DataCollection<Barometer_DataRecord>();
    private void OnBarometer_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Barometer_DataRecordData.Count == 0)
            {
                Barometer_DataRecordData.AddRecord(new Barometer_DataRecord());
            }
            Barometer_DataRecordData[Barometer_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountBarometer_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_DataRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmBarometer_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyBarometer_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,TempRaw,PressureRaw,Notes\n");
        foreach (var row in Barometer_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.TempRaw},{row.PressureRaw},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyBarometer_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Barometer_DataNotifyIndex = 0;
        bool Barometer_DataNotifySetup = false;
        private async void OnNotifyBarometer_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyBarometer_Data();
        }

        private async Task DoNotifyBarometer_Data()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Barometer_DataNotifySetup)
                {
                    Barometer_DataNotifySetup = true;
                    bleDevice.Barometer_DataEvent += BleDevice_Barometer_DataEvent;
                }
                var notifyType = NotifyBarometer_DataSettings[Barometer_DataNotifyIndex];
                Barometer_DataNotifyIndex = (Barometer_DataNotifyIndex + 1) % NotifyBarometer_DataSettings.Length;
                var result = await bleDevice.NotifyBarometer_DataAsync(notifyType);
                await bleDevice.WriteBarometer_ConfigNotify(notifyType);


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Barometer_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Barometer_DataRecord();
                var TempRaw = valueList.GetValue("TempRaw");
                if (TempRaw.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TempRaw.CurrentType == BCBasic.BCValue.ValueType.IsString || TempRaw.IsArray)
                {
                    record.TempRaw = (double)TempRaw.AsDouble;
                    Barometer_Data_TempRaw.Text = record.TempRaw.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var PressureRaw = valueList.GetValue("PressureRaw");
                if (PressureRaw.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PressureRaw.CurrentType == BCBasic.BCValue.ValueType.IsString || PressureRaw.IsArray)
                {
                    record.PressureRaw = (double)PressureRaw.AsDouble;
                    Barometer_Data_PressureRaw.Text = record.PressureRaw.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Barometer_DataRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadBarometer_Data(object sender, RoutedEventArgs e)
        {
            await DoReadBarometer_Data();
        }

        private async Task DoReadBarometer_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadBarometer_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Barometer_Data");
                    return;
                }
                
                var record = new Barometer_DataRecord();
                var TempRaw = valueList.GetValue("TempRaw");
                if (TempRaw.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TempRaw.CurrentType == BCBasic.BCValue.ValueType.IsString || TempRaw.IsArray)
                {
                    record.TempRaw = (double)TempRaw.AsDouble;
                    Barometer_Data_TempRaw.Text = record.TempRaw.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var PressureRaw = valueList.GetValue("PressureRaw");
                if (PressureRaw.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PressureRaw.CurrentType == BCBasic.BCValue.ValueType.IsString || PressureRaw.IsArray)
                {
                    record.PressureRaw = (double)PressureRaw.AsDouble;
                    Barometer_Data_PressureRaw.Text = record.PressureRaw.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Barometer_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Barometer_ConfigureRecord : INotifyPropertyChanged
        {
            public Barometer_ConfigureRecord()
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

            private double _BarometerConfigure;
            public double BarometerConfigure { get { return _BarometerConfigure; } set { if (value == _BarometerConfigure) return; _BarometerConfigure = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Barometer_ConfigureRecord> Barometer_ConfigureRecordData { get; } = new DataCollection<Barometer_ConfigureRecord>();
    private void OnBarometer_Configure_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Barometer_ConfigureRecordData.Count == 0)
            {
                Barometer_ConfigureRecordData.AddRecord(new Barometer_ConfigureRecord());
            }
            Barometer_ConfigureRecordData[Barometer_ConfigureRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountBarometer_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_ConfigureRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmBarometer_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_ConfigureRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyBarometer_Configure(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,BarometerConfigure,Notes\n");
        foreach (var row in Barometer_ConfigureRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.BarometerConfigure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadBarometer_Configure(object sender, RoutedEventArgs e)
        {
            await DoReadBarometer_Configure();
        }

        private async Task DoReadBarometer_Configure()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadBarometer_Configure();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Barometer_Configure");
                    return;
                }
                
                var record = new Barometer_ConfigureRecord();
                var BarometerConfigure = valueList.GetValue("BarometerConfigure");
                if (BarometerConfigure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BarometerConfigure.CurrentType == BCBasic.BCValue.ValueType.IsString || BarometerConfigure.IsArray)
                {
                    record.BarometerConfigure = (double)BarometerConfigure.AsDouble;
                    Barometer_Configure_BarometerConfigure.Text = record.BarometerConfigure.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Barometer_ConfigureRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickBarometer_Configure(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteBarometer_Configure (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteBarometer_Configure(object sender, RoutedEventArgs e)
        {
            var text = Barometer_Configure_BarometerConfigure.Text;
            await DoWriteBarometer_Configure (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteBarometer_Configure(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte BarometerConfigure;
                // History: used to go into Barometer_Configure_BarometerConfigure.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedBarometerConfigure = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out BarometerConfigure);
                if (!parsedBarometerConfigure)
                {
                    parseError = "BarometerConfigure";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteBarometer_Configure(BarometerConfigure);
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

        public class Barometer_CalibrationRecord : INotifyPropertyChanged
        {
            public Barometer_CalibrationRecord()
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

            private string _BarometerCalibration;
            public string BarometerCalibration { get { return _BarometerCalibration; } set { if (value == _BarometerCalibration) return; _BarometerCalibration = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Barometer_CalibrationRecord> Barometer_CalibrationRecordData { get; } = new DataCollection<Barometer_CalibrationRecord>();
    private void OnBarometer_Calibration_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Barometer_CalibrationRecordData.Count == 0)
            {
                Barometer_CalibrationRecordData.AddRecord(new Barometer_CalibrationRecord());
            }
            Barometer_CalibrationRecordData[Barometer_CalibrationRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountBarometer_Calibration(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_CalibrationRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmBarometer_Calibration(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_CalibrationRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyBarometer_Calibration(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,BarometerCalibration,Notes\n");
        foreach (var row in Barometer_CalibrationRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.BarometerCalibration},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadBarometer_Calibration(object sender, RoutedEventArgs e)
        {
            await DoReadBarometer_Calibration();
        }

        private async Task DoReadBarometer_Calibration()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadBarometer_Calibration();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Barometer_Calibration");
                    return;
                }
                
                var record = new Barometer_CalibrationRecord();
                var BarometerCalibration = valueList.GetValue("BarometerCalibration");
                if (BarometerCalibration.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BarometerCalibration.CurrentType == BCBasic.BCValue.ValueType.IsString || BarometerCalibration.IsArray)
                {
                    record.BarometerCalibration = (string)BarometerCalibration.AsString;
                    Barometer_Calibration_BarometerCalibration.Text = record.BarometerCalibration.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Barometer_CalibrationRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Barometer_PeriodRecord : INotifyPropertyChanged
        {
            public Barometer_PeriodRecord()
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

            private double _BarometerPeriod;
            public double BarometerPeriod { get { return _BarometerPeriod; } set { if (value == _BarometerPeriod) return; _BarometerPeriod = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Barometer_PeriodRecord> Barometer_PeriodRecordData { get; } = new DataCollection<Barometer_PeriodRecord>();
    private void OnBarometer_Period_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Barometer_PeriodRecordData.Count == 0)
            {
                Barometer_PeriodRecordData.AddRecord(new Barometer_PeriodRecord());
            }
            Barometer_PeriodRecordData[Barometer_PeriodRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountBarometer_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_PeriodRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmBarometer_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_PeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyBarometer_Period(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,BarometerPeriod,Notes\n");
        foreach (var row in Barometer_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.BarometerPeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadBarometer_Period(object sender, RoutedEventArgs e)
        {
            await DoReadBarometer_Period();
        }

        private async Task DoReadBarometer_Period()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadBarometer_Period();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Barometer_Period");
                    return;
                }
                
                var record = new Barometer_PeriodRecord();
                var BarometerPeriod = valueList.GetValue("BarometerPeriod");
                if (BarometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BarometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsString || BarometerPeriod.IsArray)
                {
                    record.BarometerPeriod = (double)BarometerPeriod.AsDouble;
                    Barometer_Period_BarometerPeriod.Text = record.BarometerPeriod.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Barometer_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        // Functions for Gyroscope
        public class Gyroscope_DataRecord : INotifyPropertyChanged
        {
            public Gyroscope_DataRecord()
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

    public DataCollection<Gyroscope_DataRecord> Gyroscope_DataRecordData { get; } = new DataCollection<Gyroscope_DataRecord>();
    private void OnGyroscope_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Gyroscope_DataRecordData.Count == 0)
            {
                Gyroscope_DataRecordData.AddRecord(new Gyroscope_DataRecord());
            }
            Gyroscope_DataRecordData[Gyroscope_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountGyroscope_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Gyroscope_DataRecordData.MaxLength = value;

        Gyroscope_DataChart.RedrawYTime<Gyroscope_DataRecord>(Gyroscope_DataRecordData);

    }

    private void OnAlgorithmGyroscope_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Gyroscope_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyGyroscope_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,X,Y,Z,Notes\n");
        foreach (var row in Gyroscope_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.X},{row.Y},{row.Z},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyGyroscope_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Gyroscope_DataNotifyIndex = 0;
        bool Gyroscope_DataNotifySetup = false;
        private async void OnNotifyGyroscope_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyGyroscope_Data();
        }

        private async Task DoNotifyGyroscope_Data()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Gyroscope_DataNotifySetup)
                {
                    Gyroscope_DataNotifySetup = true;
                    bleDevice.Gyroscope_DataEvent += BleDevice_Gyroscope_DataEvent;
                }
                var notifyType = NotifyGyroscope_DataSettings[Gyroscope_DataNotifyIndex];
                Gyroscope_DataNotifyIndex = (Gyroscope_DataNotifyIndex + 1) % NotifyGyroscope_DataSettings.Length;
                var result = await bleDevice.NotifyGyroscope_DataAsync(notifyType);
                await bleDevice.WriteGyroscope_ConfigNotify(notifyType);

                var EventTimeProperty = typeof(Gyroscope_DataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
                    typeof(Gyroscope_DataRecord).GetProperty("X"),
                    typeof(Gyroscope_DataRecord).GetProperty("Y"),
                    typeof(Gyroscope_DataRecord).GetProperty("Z"),

                };
                var names = new List<string>()
                {"X","Y","Z",
                };
                Gyroscope_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                Gyroscope_DataChart.SetTitle("Gyroscope Data Chart");
                Gyroscope_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<Gyroscope_DataRecord>(addResult, Gyroscope_DataRecordData)",
chartDefaultMaxY=250,
chartDefaultMinY=-250,
        chartLineDefaults={
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

        private async void BleDevice_Gyroscope_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Gyroscope_DataRecord();
                var X = valueList.GetValue("X");
                if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString || X.IsArray)
                {
                    record.X = (double)X.AsDouble;
                    Gyroscope_Data_X.Text = record.X.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Y = valueList.GetValue("Y");
                if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString || Y.IsArray)
                {
                    record.Y = (double)Y.AsDouble;
                    Gyroscope_Data_Y.Text = record.Y.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Z = valueList.GetValue("Z");
                if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString || Z.IsArray)
                {
                    record.Z = (double)Z.AsDouble;
                    Gyroscope_Data_Z.Text = record.Z.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Gyroscope_DataRecordData.AddRecord(record);

                Gyroscope_DataChart.AddYTime<Gyroscope_DataRecord>(addResult, Gyroscope_DataRecordData);

                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadGyroscope_Data(object sender, RoutedEventArgs e)
        {
            await DoReadGyroscope_Data();
        }

        private async Task DoReadGyroscope_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadGyroscope_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Gyroscope_Data");
                    return;
                }
                
                var record = new Gyroscope_DataRecord();
                var X = valueList.GetValue("X");
                if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString || X.IsArray)
                {
                    record.X = (double)X.AsDouble;
                    Gyroscope_Data_X.Text = record.X.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Y = valueList.GetValue("Y");
                if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString || Y.IsArray)
                {
                    record.Y = (double)Y.AsDouble;
                    Gyroscope_Data_Y.Text = record.Y.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Z = valueList.GetValue("Z");
                if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString || Z.IsArray)
                {
                    record.Z = (double)Z.AsDouble;
                    Gyroscope_Data_Z.Text = record.Z.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Gyroscope_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Gyroscope_ConfigureRecord : INotifyPropertyChanged
        {
            public Gyroscope_ConfigureRecord()
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

            private double _GyroscopeConfigure;
            public double GyroscopeConfigure { get { return _GyroscopeConfigure; } set { if (value == _GyroscopeConfigure) return; _GyroscopeConfigure = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Gyroscope_ConfigureRecord> Gyroscope_ConfigureRecordData { get; } = new DataCollection<Gyroscope_ConfigureRecord>();
    private void OnGyroscope_Configure_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Gyroscope_ConfigureRecordData.Count == 0)
            {
                Gyroscope_ConfigureRecordData.AddRecord(new Gyroscope_ConfigureRecord());
            }
            Gyroscope_ConfigureRecordData[Gyroscope_ConfigureRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountGyroscope_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Gyroscope_ConfigureRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmGyroscope_Configure(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Gyroscope_ConfigureRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyGyroscope_Configure(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,GyroscopeConfigure,Notes\n");
        foreach (var row in Gyroscope_ConfigureRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.GyroscopeConfigure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadGyroscope_Configure(object sender, RoutedEventArgs e)
        {
            await DoReadGyroscope_Configure();
        }

        private async Task DoReadGyroscope_Configure()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadGyroscope_Configure();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Gyroscope_Configure");
                    return;
                }
                
                var record = new Gyroscope_ConfigureRecord();
                var GyroscopeConfigure = valueList.GetValue("GyroscopeConfigure");
                if (GyroscopeConfigure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroscopeConfigure.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroscopeConfigure.IsArray)
                {
                    record.GyroscopeConfigure = (double)GyroscopeConfigure.AsDouble;
                    Gyroscope_Configure_GyroscopeConfigure.Text = record.GyroscopeConfigure.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Gyroscope_ConfigureRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickGyroscope_Configure(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteGyroscope_Configure (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteGyroscope_Configure(object sender, RoutedEventArgs e)
        {
            var text = Gyroscope_Configure_GyroscopeConfigure.Text;
            await DoWriteGyroscope_Configure (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteGyroscope_Configure(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte GyroscopeConfigure;
                // History: used to go into Gyroscope_Configure_GyroscopeConfigure.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedGyroscopeConfigure = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out GyroscopeConfigure);
                if (!parsedGyroscopeConfigure)
                {
                    parseError = "GyroscopeConfigure";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteGyroscope_Configure(GyroscopeConfigure);
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

        public class Gyroscope_PeriodRecord : INotifyPropertyChanged
        {
            public Gyroscope_PeriodRecord()
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

            private double _GyroscopePeriod;
            public double GyroscopePeriod { get { return _GyroscopePeriod; } set { if (value == _GyroscopePeriod) return; _GyroscopePeriod = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Gyroscope_PeriodRecord> Gyroscope_PeriodRecordData { get; } = new DataCollection<Gyroscope_PeriodRecord>();
    private void OnGyroscope_Period_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Gyroscope_PeriodRecordData.Count == 0)
            {
                Gyroscope_PeriodRecordData.AddRecord(new Gyroscope_PeriodRecord());
            }
            Gyroscope_PeriodRecordData[Gyroscope_PeriodRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountGyroscope_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Gyroscope_PeriodRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmGyroscope_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Gyroscope_PeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyGyroscope_Period(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,GyroscopePeriod,Notes\n");
        foreach (var row in Gyroscope_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.GyroscopePeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadGyroscope_Period(object sender, RoutedEventArgs e)
        {
            await DoReadGyroscope_Period();
        }

        private async Task DoReadGyroscope_Period()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadGyroscope_Period();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Gyroscope_Period");
                    return;
                }
                
                var record = new Gyroscope_PeriodRecord();
                var GyroscopePeriod = valueList.GetValue("GyroscopePeriod");
                if (GyroscopePeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroscopePeriod.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroscopePeriod.IsArray)
                {
                    record.GyroscopePeriod = (double)GyroscopePeriod.AsDouble;
                    Gyroscope_Period_GyroscopePeriod.Text = record.GyroscopePeriod.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Gyroscope_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickGyroscope_Period(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteGyroscope_Period (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteGyroscope_Period(object sender, RoutedEventArgs e)
        {
            var text = Gyroscope_Period_GyroscopePeriod.Text;
            await DoWriteGyroscope_Period (text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteGyroscope_Period(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte GyroscopePeriod;
                // History: used to go into Gyroscope_Period_GyroscopePeriod.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedGyroscopePeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out GyroscopePeriod);
                if (!parsedGyroscopePeriod)
                {
                    parseError = "GyroscopePeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteGyroscope_Period(GyroscopePeriod);
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


        // Functions for Key Press
        public class Key_Press_StateRecord : INotifyPropertyChanged
        {
            public Key_Press_StateRecord()
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

            private double _KeyPressState;
            public double KeyPressState { get { return _KeyPressState; } set { if (value == _KeyPressState) return; _KeyPressState = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Key_Press_StateRecord> Key_Press_StateRecordData { get; } = new DataCollection<Key_Press_StateRecord>();
    private void OnKey_Press_State_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Key_Press_StateRecordData.Count == 0)
            {
                Key_Press_StateRecordData.AddRecord(new Key_Press_StateRecord());
            }
            Key_Press_StateRecordData[Key_Press_StateRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountKey_Press_State(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Key_Press_StateRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmKey_Press_State(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Key_Press_StateRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyKey_Press_State(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,KeyPressState,Notes\n");
        foreach (var row in Key_Press_StateRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.KeyPressState},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyKey_Press_StateSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Key_Press_StateNotifyIndex = 0;
        bool Key_Press_StateNotifySetup = false;
        private async void OnNotifyKey_Press_State(object sender, RoutedEventArgs e)
        {
            await DoNotifyKey_Press_State();
        }

        private async Task DoNotifyKey_Press_State()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Key_Press_StateNotifySetup)
                {
                    Key_Press_StateNotifySetup = true;
                    bleDevice.Key_Press_StateEvent += BleDevice_Key_Press_StateEvent;
                }
                var notifyType = NotifyKey_Press_StateSettings[Key_Press_StateNotifyIndex];
                Key_Press_StateNotifyIndex = (Key_Press_StateNotifyIndex + 1) % NotifyKey_Press_StateSettings.Length;
                var result = await bleDevice.NotifyKey_Press_StateAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Key_Press_StateEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Key_Press_StateRecord();
                var KeyPressState = valueList.GetValue("KeyPressState");
                if (KeyPressState.CurrentType == BCBasic.BCValue.ValueType.IsDouble || KeyPressState.CurrentType == BCBasic.BCValue.ValueType.IsString || KeyPressState.IsArray)
                {
                    record.KeyPressState = (double)KeyPressState.AsDouble;
                    Key_Press_State_KeyPressState.Text = record.KeyPressState.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Key_Press_StateRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }





        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(CharacteristicsEnum.All_enum, true);
            SetStatusActive(false);
        }
    }
}