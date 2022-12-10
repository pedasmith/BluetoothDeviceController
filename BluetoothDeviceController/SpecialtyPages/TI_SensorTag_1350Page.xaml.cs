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
using static BluetoothProtocols.TI_SensorTag_1350;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the CC1350 SensorTag device
    /// </summary>
    public sealed partial class TI_SensorTag_1350Page : Page, HasId, ISetHandleStatus
    {
        public TI_SensorTag_1350Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "TI_SensorTag_1350";
        private string DeviceNameUser = "CC1350 SensorTag";

        int ncommand = 0;
        TI_SensorTag_1350 bleDevice = new TI_SensorTag_1350();
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

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyBatteryLevelSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int BatteryLevelNotifyIndex = 0;
        bool BatteryLevelNotifySetup = false;
        private async void OnNotifyBatteryLevel(object sender, RoutedEventArgs e)
        {
            await DoNotifyBatteryLevel();
        }

        private async Task DoNotifyBatteryLevel()
        {
            SetStatusActive (true);
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
                if (BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsString || BatteryLevel.IsArray)
                {
                    record.BatteryLevel = (double)BatteryLevel.AsDouble;
                    BatteryLevel_BatteryLevel.Text = record.BatteryLevel.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = BatteryLevelRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadBatteryLevel(object sender, RoutedEventArgs e)
        {
            await DoReadBatteryLevel();
        }

        private async Task DoReadBatteryLevel()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadBatteryLevel();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read BatteryLevel");
                    return;
                }
                
                var record = new BatteryLevelRecord();
                var BatteryLevel = valueList.GetValue("BatteryLevel");
                if (BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsString || BatteryLevel.IsArray)
                {
                    record.BatteryLevel = (double)BatteryLevel.AsDouble;
                    BatteryLevel_BatteryLevel.Text = record.BatteryLevel.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                BatteryLevelRecordData.Add(record);

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
            private double _AmbTemp;
            public double AmbTemp { get { return _AmbTemp; } set { if (value == _AmbTemp) return; _AmbTemp = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,ObjTemp,AmbTemp,Notes\n");
        foreach (var row in IR_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ObjTemp},{row.AmbTemp},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                    typeof(IR_DataRecord).GetProperty("AmbTemp"),

                };
                var names = new List<string>()
                {"ObjTemp","AmbTemp",
                };
                IR_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                IR_DataChart.SetTitle("IR Data Chart");
                IR_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<IR_DataRecord>(addResult, IR_DataRecordData)",
chartDefaultMaxY=35,
chartDefaultMinY=15,
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
                
                var record = new IR_DataRecord();
                var ObjTemp = valueList.GetValue("ObjTemp");
                if (ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || ObjTemp.IsArray)
                {
                    record.ObjTemp = (double)ObjTemp.AsDouble;
                    IR_Data_ObjTemp.Text = record.ObjTemp.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AmbTemp = valueList.GetValue("AmbTemp");
                if (AmbTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AmbTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || AmbTemp.IsArray)
                {
                    record.AmbTemp = (double)AmbTemp.AsDouble;
                    IR_Data_AmbTemp.Text = record.AmbTemp.ToString("F3");
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
                
                var record = new IR_DataRecord();
                var ObjTemp = valueList.GetValue("ObjTemp");
                if (ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ObjTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || ObjTemp.IsArray)
                {
                    record.ObjTemp = (double)ObjTemp.AsDouble;
                    IR_Data_ObjTemp.Text = record.ObjTemp.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AmbTemp = valueList.GetValue("AmbTemp");
                if (AmbTemp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AmbTemp.CurrentType == BCBasic.BCValue.ValueType.IsString || AmbTemp.IsArray)
                {
                    record.AmbTemp = (double)AmbTemp.AsDouble;
                    IR_Data_AmbTemp.Text = record.AmbTemp.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                IR_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class IR_Service_ConfigRecord : INotifyPropertyChanged
        {
            public IR_Service_ConfigRecord()
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

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<IR_Service_ConfigRecord> IR_Service_ConfigRecordData { get; } = new DataCollection<IR_Service_ConfigRecord>();
    private void OnIR_Service_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (IR_Service_ConfigRecordData.Count == 0)
            {
                IR_Service_ConfigRecordData.AddRecord(new IR_Service_ConfigRecord());
            }
            IR_Service_ConfigRecordData[IR_Service_ConfigRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountIR_Service_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_Service_ConfigRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmIR_Service_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IR_Service_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyIR_Service_Config(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Enable,Notes\n");
        foreach (var row in IR_Service_ConfigRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Enable},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadIR_Service_Config(object sender, RoutedEventArgs e)
        {
            await DoReadIR_Service_Config();
        }

        private async Task DoReadIR_Service_Config()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadIR_Service_Config();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read IR_Service_Config");
                    return;
                }
                
                var record = new IR_Service_ConfigRecord();
                var Enable = valueList.GetValue("Enable");
                if (Enable.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Enable.CurrentType == BCBasic.BCValue.ValueType.IsString || Enable.IsArray)
                {
                    record.Enable = (double)Enable.AsDouble;
                    IR_Service_Config_Enable.Text = record.Enable.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                IR_Service_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickIR_Service_Config(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteIR_Service_Config (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteIR_Service_Config(object sender, RoutedEventArgs e)
        {
            var text = IR_Service_Config_Enable.Text;
            await DoWriteIR_Service_Config (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteIR_Service_Config(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Enable;
                // History: used to go into IR_Service_Config_Enable.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedEnable = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Enable);
                if (!parsedEnable)
                {
                    parseError = "Enable";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteIR_Service_Config(Enable);
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

            private double _Period;
            public double Period { get { return _Period; } set { if (value == _Period) return; _Period = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,Period,Notes\n");
        foreach (var row in IR_Service_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Period},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                var Period = valueList.GetValue("Period");
                if (Period.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Period.CurrentType == BCBasic.BCValue.ValueType.IsString || Period.IsArray)
                {
                    record.Period = (double)Period.AsDouble;
                    IR_Service_Period_Period.Text = record.Period.ToString("N0");
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
            var text = IR_Service_Period_Period.Text;
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

                Byte Period;
                // History: used to go into IR_Service_Period_Period.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Period);
                if (!parsedPeriod)
                {
                    parseError = "Period";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteIR_Service_Period(Period);
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


        public class Humidity_ConfigRecord : INotifyPropertyChanged
        {
            public Humidity_ConfigRecord()
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

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Humidity_ConfigRecord> Humidity_ConfigRecordData { get; } = new DataCollection<Humidity_ConfigRecord>();
    private void OnHumidity_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Humidity_ConfigRecordData.Count == 0)
            {
                Humidity_ConfigRecordData.AddRecord(new Humidity_ConfigRecord());
            }
            Humidity_ConfigRecordData[Humidity_ConfigRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountHumidity_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_ConfigRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmHumidity_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Humidity_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyHumidity_Config(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Enable,Notes\n");
        foreach (var row in Humidity_ConfigRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Enable},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadHumidity_Config(object sender, RoutedEventArgs e)
        {
            await DoReadHumidity_Config();
        }

        private async Task DoReadHumidity_Config()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHumidity_Config();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Humidity_Config");
                    return;
                }
                
                var record = new Humidity_ConfigRecord();
                var Enable = valueList.GetValue("Enable");
                if (Enable.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Enable.CurrentType == BCBasic.BCValue.ValueType.IsString || Enable.IsArray)
                {
                    record.Enable = (double)Enable.AsDouble;
                    Humidity_Config_Enable.Text = record.Enable.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Humidity_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickHumidity_Config(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteHumidity_Config (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteHumidity_Config(object sender, RoutedEventArgs e)
        {
            var text = Humidity_Config_Enable.Text;
            await DoWriteHumidity_Config (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteHumidity_Config(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Enable;
                // History: used to go into Humidity_Config_Enable.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedEnable = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Enable);
                if (!parsedEnable)
                {
                    parseError = "Enable";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteHumidity_Config(Enable);
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

            private double _Period;
            public double Period { get { return _Period; } set { if (value == _Period) return; _Period = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,Period,Notes\n");
        foreach (var row in Humidity_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Period},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                var Period = valueList.GetValue("Period");
                if (Period.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Period.CurrentType == BCBasic.BCValue.ValueType.IsString || Period.IsArray)
                {
                    record.Period = (double)Period.AsDouble;
                    Humidity_Period_Period.Text = record.Period.ToString("N0");
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
            var text = Humidity_Period_Period.Text;
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

                Byte Period;
                // History: used to go into Humidity_Period_Period.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Period);
                if (!parsedPeriod)
                {
                    parseError = "Period";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteHumidity_Period(Period);
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

            private double _Temp;
            public double Temp { get { return _Temp; } set { if (value == _Temp) return; _Temp = value; OnPropertyChanged(); } }
            private double _Pressure;
            public double Pressure { get { return _Pressure; } set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged(); } }

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

        Barometer_DataChart.RedrawYTime<Barometer_DataRecord>(Barometer_DataRecordData);

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
        sb.Append("EventDate,EventTime,Temp,Pressure,Notes\n");
        foreach (var row in Barometer_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Temp},{row.Pressure},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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

                var EventTimeProperty = typeof(Barometer_DataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
                    typeof(Barometer_DataRecord).GetProperty("Temp"),
                    typeof(Barometer_DataRecord).GetProperty("Pressure"),

                };
                var names = new List<string>()
                {"Temp","Pressure",
                };
                Barometer_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                Barometer_DataChart.SetTitle("Barometer Data Chart");
                Barometer_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<Barometer_DataRecord>(addResult, Barometer_DataRecordData)",
}
;

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
                var Temp = valueList.GetValue("Temp");
                if (Temp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Temp.CurrentType == BCBasic.BCValue.ValueType.IsString || Temp.IsArray)
                {
                    record.Temp = (double)Temp.AsDouble;
                    Barometer_Data_Temp.Text = record.Temp.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Pressure = valueList.GetValue("Pressure");
                if (Pressure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Pressure.CurrentType == BCBasic.BCValue.ValueType.IsString || Pressure.IsArray)
                {
                    record.Pressure = (double)Pressure.AsDouble;
                    Barometer_Data_Pressure.Text = record.Pressure.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Barometer_DataRecordData.AddRecord(record);

                Barometer_DataChart.AddYTime<Barometer_DataRecord>(addResult, Barometer_DataRecordData);

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
                var Temp = valueList.GetValue("Temp");
                if (Temp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Temp.CurrentType == BCBasic.BCValue.ValueType.IsString || Temp.IsArray)
                {
                    record.Temp = (double)Temp.AsDouble;
                    Barometer_Data_Temp.Text = record.Temp.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var Pressure = valueList.GetValue("Pressure");
                if (Pressure.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Pressure.CurrentType == BCBasic.BCValue.ValueType.IsString || Pressure.IsArray)
                {
                    record.Pressure = (double)Pressure.AsDouble;
                    Barometer_Data_Pressure.Text = record.Pressure.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Barometer_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Barometer_ConfigRecord : INotifyPropertyChanged
        {
            public Barometer_ConfigRecord()
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

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Barometer_ConfigRecord> Barometer_ConfigRecordData { get; } = new DataCollection<Barometer_ConfigRecord>();
    private void OnBarometer_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Barometer_ConfigRecordData.Count == 0)
            {
                Barometer_ConfigRecordData.AddRecord(new Barometer_ConfigRecord());
            }
            Barometer_ConfigRecordData[Barometer_ConfigRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountBarometer_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_ConfigRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmBarometer_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Barometer_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyBarometer_Config(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Enable,Notes\n");
        foreach (var row in Barometer_ConfigRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Enable},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadBarometer_Config(object sender, RoutedEventArgs e)
        {
            await DoReadBarometer_Config();
        }

        private async Task DoReadBarometer_Config()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadBarometer_Config();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Barometer_Config");
                    return;
                }
                
                var record = new Barometer_ConfigRecord();
                var Enable = valueList.GetValue("Enable");
                if (Enable.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Enable.CurrentType == BCBasic.BCValue.ValueType.IsString || Enable.IsArray)
                {
                    record.Enable = (double)Enable.AsDouble;
                    Barometer_Config_Enable.Text = record.Enable.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Barometer_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickBarometer_Config(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteBarometer_Config (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteBarometer_Config(object sender, RoutedEventArgs e)
        {
            var text = Barometer_Config_Enable.Text;
            await DoWriteBarometer_Config (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteBarometer_Config(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Enable;
                // History: used to go into Barometer_Config_Enable.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedEnable = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Enable);
                if (!parsedEnable)
                {
                    parseError = "Enable";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteBarometer_Config(Enable);
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

            private double _Period;
            public double Period { get { return _Period; } set { if (value == _Period) return; _Period = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,Period,Notes\n");
        foreach (var row in Barometer_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Period},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                var Period = valueList.GetValue("Period");
                if (Period.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Period.CurrentType == BCBasic.BCValue.ValueType.IsString || Period.IsArray)
                {
                    record.Period = (double)Period.AsDouble;
                    Barometer_Period_Period.Text = record.Period.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Barometer_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickBarometer_Period(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteBarometer_Period (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteBarometer_Period(object sender, RoutedEventArgs e)
        {
            var text = Barometer_Period_Period.Text;
            await DoWriteBarometer_Period (text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteBarometer_Period(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Period;
                // History: used to go into Barometer_Period_Period.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Period);
                if (!parsedPeriod)
                {
                    parseError = "Period";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteBarometer_Period(Period);
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

            private double _GyroX;
            public double GyroX { get { return _GyroX; } set { if (value == _GyroX) return; _GyroX = value; OnPropertyChanged(); } }
            private double _GyroY;
            public double GyroY { get { return _GyroY; } set { if (value == _GyroY) return; _GyroY = value; OnPropertyChanged(); } }
            private double _GyroZ;
            public double GyroZ { get { return _GyroZ; } set { if (value == _GyroZ) return; _GyroZ = value; OnPropertyChanged(); } }
            private double _AccX;
            public double AccX { get { return _AccX; } set { if (value == _AccX) return; _AccX = value; OnPropertyChanged(); } }
            private double _AccY;
            public double AccY { get { return _AccY; } set { if (value == _AccY) return; _AccY = value; OnPropertyChanged(); } }
            private double _AccZ;
            public double AccZ { get { return _AccZ; } set { if (value == _AccZ) return; _AccZ = value; OnPropertyChanged(); } }
            private double _MagnetometerX;
            public double MagnetometerX { get { return _MagnetometerX; } set { if (value == _MagnetometerX) return; _MagnetometerX = value; OnPropertyChanged(); } }
            private double _MagnetometerY;
            public double MagnetometerY { get { return _MagnetometerY; } set { if (value == _MagnetometerY) return; _MagnetometerY = value; OnPropertyChanged(); } }
            private double _MagnetometerZ;
            public double MagnetometerZ { get { return _MagnetometerZ; } set { if (value == _MagnetometerZ) return; _MagnetometerZ = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,GyroX,GyroY,GyroZ,AccX,AccY,AccZ,MagnetometerX,MagnetometerY,MagnetometerZ,Notes\n");
        foreach (var row in Accelerometer_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.GyroX},{row.GyroY},{row.GyroZ},{row.AccX},{row.AccY},{row.AccZ},{row.MagnetometerX},{row.MagnetometerY},{row.MagnetometerZ},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                    typeof(Accelerometer_DataRecord).GetProperty("GyroX"),
                    typeof(Accelerometer_DataRecord).GetProperty("GyroY"),
                    typeof(Accelerometer_DataRecord).GetProperty("GyroZ"),
                    typeof(Accelerometer_DataRecord).GetProperty("AccX"),
                    typeof(Accelerometer_DataRecord).GetProperty("AccY"),
                    typeof(Accelerometer_DataRecord).GetProperty("AccZ"),
                    typeof(Accelerometer_DataRecord).GetProperty("MagnetometerX"),
                    typeof(Accelerometer_DataRecord).GetProperty("MagnetometerY"),
                    typeof(Accelerometer_DataRecord).GetProperty("MagnetometerZ"),

                };
                var names = new List<string>()
                {"GyroX","GyroY","GyroZ","AccX","AccY","AccZ","MagnetometerX","MagnetometerY","MagnetometerZ",
                };
                Accelerometer_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                Accelerometer_DataChart.SetTitle("Accelerometer Data Chart");
                Accelerometer_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<Accelerometer_DataRecord>(addResult, Accelerometer_DataRecordData)",
chartDefaultMaxY=5,
chartDefaultMinY=-5,
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
                var GyroX = valueList.GetValue("GyroX");
                if (GyroX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroX.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroX.IsArray)
                {
                    record.GyroX = (double)GyroX.AsDouble;
                    Accelerometer_Data_GyroX.Text = record.GyroX.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var GyroY = valueList.GetValue("GyroY");
                if (GyroY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroY.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroY.IsArray)
                {
                    record.GyroY = (double)GyroY.AsDouble;
                    Accelerometer_Data_GyroY.Text = record.GyroY.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var GyroZ = valueList.GetValue("GyroZ");
                if (GyroZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroZ.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroZ.IsArray)
                {
                    record.GyroZ = (double)GyroZ.AsDouble;
                    Accelerometer_Data_GyroZ.Text = record.GyroZ.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccX = valueList.GetValue("AccX");
                if (AccX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccX.CurrentType == BCBasic.BCValue.ValueType.IsString || AccX.IsArray)
                {
                    record.AccX = (double)AccX.AsDouble;
                    Accelerometer_Data_AccX.Text = record.AccX.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccY = valueList.GetValue("AccY");
                if (AccY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccY.CurrentType == BCBasic.BCValue.ValueType.IsString || AccY.IsArray)
                {
                    record.AccY = (double)AccY.AsDouble;
                    Accelerometer_Data_AccY.Text = record.AccY.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccZ = valueList.GetValue("AccZ");
                if (AccZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccZ.CurrentType == BCBasic.BCValue.ValueType.IsString || AccZ.IsArray)
                {
                    record.AccZ = (double)AccZ.AsDouble;
                    Accelerometer_Data_AccZ.Text = record.AccZ.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var MagnetometerX = valueList.GetValue("MagnetometerX");
                if (MagnetometerX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerX.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerX.IsArray)
                {
                    record.MagnetometerX = (double)MagnetometerX.AsDouble;
                    Accelerometer_Data_MagnetometerX.Text = record.MagnetometerX.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var MagnetometerY = valueList.GetValue("MagnetometerY");
                if (MagnetometerY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerY.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerY.IsArray)
                {
                    record.MagnetometerY = (double)MagnetometerY.AsDouble;
                    Accelerometer_Data_MagnetometerY.Text = record.MagnetometerY.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var MagnetometerZ = valueList.GetValue("MagnetometerZ");
                if (MagnetometerZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerZ.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerZ.IsArray)
                {
                    record.MagnetometerZ = (double)MagnetometerZ.AsDouble;
                    Accelerometer_Data_MagnetometerZ.Text = record.MagnetometerZ.ToString("N0");
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
                var GyroX = valueList.GetValue("GyroX");
                if (GyroX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroX.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroX.IsArray)
                {
                    record.GyroX = (double)GyroX.AsDouble;
                    Accelerometer_Data_GyroX.Text = record.GyroX.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var GyroY = valueList.GetValue("GyroY");
                if (GyroY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroY.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroY.IsArray)
                {
                    record.GyroY = (double)GyroY.AsDouble;
                    Accelerometer_Data_GyroY.Text = record.GyroY.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var GyroZ = valueList.GetValue("GyroZ");
                if (GyroZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GyroZ.CurrentType == BCBasic.BCValue.ValueType.IsString || GyroZ.IsArray)
                {
                    record.GyroZ = (double)GyroZ.AsDouble;
                    Accelerometer_Data_GyroZ.Text = record.GyroZ.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccX = valueList.GetValue("AccX");
                if (AccX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccX.CurrentType == BCBasic.BCValue.ValueType.IsString || AccX.IsArray)
                {
                    record.AccX = (double)AccX.AsDouble;
                    Accelerometer_Data_AccX.Text = record.AccX.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccY = valueList.GetValue("AccY");
                if (AccY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccY.CurrentType == BCBasic.BCValue.ValueType.IsString || AccY.IsArray)
                {
                    record.AccY = (double)AccY.AsDouble;
                    Accelerometer_Data_AccY.Text = record.AccY.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var AccZ = valueList.GetValue("AccZ");
                if (AccZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccZ.CurrentType == BCBasic.BCValue.ValueType.IsString || AccZ.IsArray)
                {
                    record.AccZ = (double)AccZ.AsDouble;
                    Accelerometer_Data_AccZ.Text = record.AccZ.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var MagnetometerX = valueList.GetValue("MagnetometerX");
                if (MagnetometerX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerX.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerX.IsArray)
                {
                    record.MagnetometerX = (double)MagnetometerX.AsDouble;
                    Accelerometer_Data_MagnetometerX.Text = record.MagnetometerX.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var MagnetometerY = valueList.GetValue("MagnetometerY");
                if (MagnetometerY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerY.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerY.IsArray)
                {
                    record.MagnetometerY = (double)MagnetometerY.AsDouble;
                    Accelerometer_Data_MagnetometerY.Text = record.MagnetometerY.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }
                var MagnetometerZ = valueList.GetValue("MagnetometerZ");
                if (MagnetometerZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerZ.CurrentType == BCBasic.BCValue.ValueType.IsString || MagnetometerZ.IsArray)
                {
                    record.MagnetometerZ = (double)MagnetometerZ.AsDouble;
                    Accelerometer_Data_MagnetometerZ.Text = record.MagnetometerZ.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Accelerometer_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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

            private double _Enable;
            public double Enable { get { return _Enable; } set { if (value == _Enable) return; _Enable = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,Enable,Notes\n");
        foreach (var row in Accelerometer_ConfigRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Enable},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadAccelerometer_Config(object sender, RoutedEventArgs e)
        {
            await DoReadAccelerometer_Config();
        }

        private async Task DoReadAccelerometer_Config()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometer_Config();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Accelerometer_Config");
                    return;
                }
                
                var record = new Accelerometer_ConfigRecord();
                var Enable = valueList.GetValue("Enable");
                if (Enable.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Enable.CurrentType == BCBasic.BCValue.ValueType.IsString || Enable.IsArray)
                {
                    record.Enable = (double)Enable.AsDouble;
                    Accelerometer_Config_Enable.Text = record.Enable.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Accelerometer_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickAccelerometer_Config(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteAccelerometer_Config (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteAccelerometer_Config(object sender, RoutedEventArgs e)
        {
            var text = Accelerometer_Config_Enable.Text;
            await DoWriteAccelerometer_Config (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteAccelerometer_Config(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 Enable;
                // History: used to go into Accelerometer_Config_Enable.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedEnable = Utilities.Parsers.TryParseUInt16(text, dec_or_hex, null, out Enable);
                if (!parsedEnable)
                {
                    parseError = "Enable";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAccelerometer_Config(Enable);
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

            private double _Period;
            public double Period { get { return _Period; } set { if (value == _Period) return; _Period = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,Period,Notes\n");
        foreach (var row in Accelerometer_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Period},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                var Period = valueList.GetValue("Period");
                if (Period.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Period.CurrentType == BCBasic.BCValue.ValueType.IsString || Period.IsArray)
                {
                    record.Period = (double)Period.AsDouble;
                    Accelerometer_Period_Period.Text = record.Period.ToString("N0");
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
            var text = Accelerometer_Period_Period.Text;
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

                Byte Period;
                // History: used to go into Accelerometer_Period_Period.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Period);
                if (!parsedPeriod)
                {
                    parseError = "Period";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAccelerometer_Period(Period);
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


        // Functions for Optical Service
        public class Optical_Service_DataRecord : INotifyPropertyChanged
        {
            public Optical_Service_DataRecord()
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

            private double _Lux;
            public double Lux { get { return _Lux; } set { if (value == _Lux) return; _Lux = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Optical_Service_DataRecord> Optical_Service_DataRecordData { get; } = new DataCollection<Optical_Service_DataRecord>();
    private void OnOptical_Service_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Optical_Service_DataRecordData.Count == 0)
            {
                Optical_Service_DataRecordData.AddRecord(new Optical_Service_DataRecord());
            }
            Optical_Service_DataRecordData[Optical_Service_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountOptical_Service_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Optical_Service_DataRecordData.MaxLength = value;

        Optical_Service_DataChart.RedrawYTime<Optical_Service_DataRecord>(Optical_Service_DataRecordData);

    }

    private void OnAlgorithmOptical_Service_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Optical_Service_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyOptical_Service_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Lux,Notes\n");
        foreach (var row in Optical_Service_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Lux},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyOptical_Service_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Optical_Service_DataNotifyIndex = 0;
        bool Optical_Service_DataNotifySetup = false;
        private async void OnNotifyOptical_Service_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyOptical_Service_Data();
        }

        private async Task DoNotifyOptical_Service_Data()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Optical_Service_DataNotifySetup)
                {
                    Optical_Service_DataNotifySetup = true;
                    bleDevice.Optical_Service_DataEvent += BleDevice_Optical_Service_DataEvent;
                }
                var notifyType = NotifyOptical_Service_DataSettings[Optical_Service_DataNotifyIndex];
                Optical_Service_DataNotifyIndex = (Optical_Service_DataNotifyIndex + 1) % NotifyOptical_Service_DataSettings.Length;
                var result = await bleDevice.NotifyOptical_Service_DataAsync(notifyType);
                await bleDevice.WriteOptical_Service_ConfigNotify(notifyType);

                var EventTimeProperty = typeof(Optical_Service_DataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
                    typeof(Optical_Service_DataRecord).GetProperty("Lux"),

                };
                var names = new List<string>()
                {"Lux",
                };
                Optical_Service_DataChart.SetDataProperties(properties, EventTimeProperty, names);
                Optical_Service_DataChart.SetTitle("Optical Service Data Chart");
                Optical_Service_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
{
tableType="standard",
chartType="standard",
chartCommand="AddYTime<Optical_Service_DataRecord>(addResult, Optical_Service_DataRecordData)",
chartDefaultMaxY=35,
chartDefaultMinY=15,
}
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Optical_Service_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Optical_Service_DataRecord();
                var Lux = valueList.GetValue("Lux");
                if (Lux.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Lux.CurrentType == BCBasic.BCValue.ValueType.IsString || Lux.IsArray)
                {
                    record.Lux = (double)Lux.AsDouble;
                    Optical_Service_Data_Lux.Text = record.Lux.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Optical_Service_DataRecordData.AddRecord(record);

                Optical_Service_DataChart.AddYTime<Optical_Service_DataRecord>(addResult, Optical_Service_DataRecordData);

                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadOptical_Service_Data(object sender, RoutedEventArgs e)
        {
            await DoReadOptical_Service_Data();
        }

        private async Task DoReadOptical_Service_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadOptical_Service_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Optical_Service_Data");
                    return;
                }
                
                var record = new Optical_Service_DataRecord();
                var Lux = valueList.GetValue("Lux");
                if (Lux.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Lux.CurrentType == BCBasic.BCValue.ValueType.IsString || Lux.IsArray)
                {
                    record.Lux = (double)Lux.AsDouble;
                    Optical_Service_Data_Lux.Text = record.Lux.ToString("F3");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Optical_Service_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }


        public class Optical_Service_ConfigRecord : INotifyPropertyChanged
        {
            public Optical_Service_ConfigRecord()
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

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Optical_Service_ConfigRecord> Optical_Service_ConfigRecordData { get; } = new DataCollection<Optical_Service_ConfigRecord>();
    private void OnOptical_Service_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Optical_Service_ConfigRecordData.Count == 0)
            {
                Optical_Service_ConfigRecordData.AddRecord(new Optical_Service_ConfigRecord());
            }
            Optical_Service_ConfigRecordData[Optical_Service_ConfigRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountOptical_Service_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Optical_Service_ConfigRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmOptical_Service_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Optical_Service_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyOptical_Service_Config(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Enable,Notes\n");
        foreach (var row in Optical_Service_ConfigRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Enable},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadOptical_Service_Config(object sender, RoutedEventArgs e)
        {
            await DoReadOptical_Service_Config();
        }

        private async Task DoReadOptical_Service_Config()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadOptical_Service_Config();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Optical_Service_Config");
                    return;
                }
                
                var record = new Optical_Service_ConfigRecord();
                var Enable = valueList.GetValue("Enable");
                if (Enable.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Enable.CurrentType == BCBasic.BCValue.ValueType.IsString || Enable.IsArray)
                {
                    record.Enable = (double)Enable.AsDouble;
                    Optical_Service_Config_Enable.Text = record.Enable.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Optical_Service_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickOptical_Service_Config(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteOptical_Service_Config (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteOptical_Service_Config(object sender, RoutedEventArgs e)
        {
            var text = Optical_Service_Config_Enable.Text;
            await DoWriteOptical_Service_Config (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteOptical_Service_Config(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Enable;
                // History: used to go into Optical_Service_Config_Enable.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedEnable = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Enable);
                if (!parsedEnable)
                {
                    parseError = "Enable";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteOptical_Service_Config(Enable);
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

        public class Optical_Service_PeriodRecord : INotifyPropertyChanged
        {
            public Optical_Service_PeriodRecord()
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

            private double _Period;
            public double Period { get { return _Period; } set { if (value == _Period) return; _Period = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Optical_Service_PeriodRecord> Optical_Service_PeriodRecordData { get; } = new DataCollection<Optical_Service_PeriodRecord>();
    private void OnOptical_Service_Period_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Optical_Service_PeriodRecordData.Count == 0)
            {
                Optical_Service_PeriodRecordData.AddRecord(new Optical_Service_PeriodRecord());
            }
            Optical_Service_PeriodRecordData[Optical_Service_PeriodRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountOptical_Service_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Optical_Service_PeriodRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmOptical_Service_Period(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Optical_Service_PeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyOptical_Service_Period(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Period,Notes\n");
        foreach (var row in Optical_Service_PeriodRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Period},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadOptical_Service_Period(object sender, RoutedEventArgs e)
        {
            await DoReadOptical_Service_Period();
        }

        private async Task DoReadOptical_Service_Period()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadOptical_Service_Period();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Optical_Service_Period");
                    return;
                }
                
                var record = new Optical_Service_PeriodRecord();
                var Period = valueList.GetValue("Period");
                if (Period.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Period.CurrentType == BCBasic.BCValue.ValueType.IsString || Period.IsArray)
                {
                    record.Period = (double)Period.AsDouble;
                    Optical_Service_Period_Period.Text = record.Period.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Optical_Service_PeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickOptical_Service_Period(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteOptical_Service_Period (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteOptical_Service_Period(object sender, RoutedEventArgs e)
        {
            var text = Optical_Service_Period_Period.Text;
            await DoWriteOptical_Service_Period (text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteOptical_Service_Period(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Period;
                // History: used to go into Optical_Service_Period_Period.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedPeriod = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Period);
                if (!parsedPeriod)
                {
                    parseError = "Period";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteOptical_Service_Period(Period);
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

            private double _param0;
            public double param0 { get { return _param0; } set { if (value == _param0) return; _param0 = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in Key_Press_StateRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (double)param0.AsDouble;
                    Key_Press_State_param0.Text = record.param0.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var addResult = Key_Press_StateRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }




        // Functions for IO Service
        public class IO_Service_DataRecord : INotifyPropertyChanged
        {
            public IO_Service_DataRecord()
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

    public DataCollection<IO_Service_DataRecord> IO_Service_DataRecordData { get; } = new DataCollection<IO_Service_DataRecord>();
    private void OnIO_Service_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (IO_Service_DataRecordData.Count == 0)
            {
                IO_Service_DataRecordData.AddRecord(new IO_Service_DataRecord());
            }
            IO_Service_DataRecordData[IO_Service_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountIO_Service_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IO_Service_DataRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmIO_Service_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IO_Service_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyIO_Service_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in IO_Service_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadIO_Service_Data(object sender, RoutedEventArgs e)
        {
            await DoReadIO_Service_Data();
        }

        private async Task DoReadIO_Service_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadIO_Service_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read IO_Service_Data");
                    return;
                }
                
                var record = new IO_Service_DataRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    IO_Service_Data_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                IO_Service_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickIO_Service_Data(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteIO_Service_Data (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteIO_Service_Data(object sender, RoutedEventArgs e)
        {
            var text = IO_Service_Data_param0.Text;
            await DoWriteIO_Service_Data (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteIO_Service_Data(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes param0;
                // History: used to go into IO_Service_Data_param0.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedparam0 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteIO_Service_Data(param0);
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

        public class IO_Service_ConfigRecord : INotifyPropertyChanged
        {
            public IO_Service_ConfigRecord()
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

    public DataCollection<IO_Service_ConfigRecord> IO_Service_ConfigRecordData { get; } = new DataCollection<IO_Service_ConfigRecord>();
    private void OnIO_Service_Config_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (IO_Service_ConfigRecordData.Count == 0)
            {
                IO_Service_ConfigRecordData.AddRecord(new IO_Service_ConfigRecord());
            }
            IO_Service_ConfigRecordData[IO_Service_ConfigRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountIO_Service_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IO_Service_ConfigRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmIO_Service_Config(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        IO_Service_ConfigRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyIO_Service_Config(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in IO_Service_ConfigRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadIO_Service_Config(object sender, RoutedEventArgs e)
        {
            await DoReadIO_Service_Config();
        }

        private async Task DoReadIO_Service_Config()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadIO_Service_Config();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read IO_Service_Config");
                    return;
                }
                
                var record = new IO_Service_ConfigRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (double)param0.AsDouble;
                    IO_Service_Config_param0.Text = record.param0.ToString("N0");
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                IO_Service_ConfigRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickIO_Service_Config(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteIO_Service_Config (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteIO_Service_Config(object sender, RoutedEventArgs e)
        {
            var text = IO_Service_Config_param0.Text;
            await DoWriteIO_Service_Config (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteIO_Service_Config(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte param0;
                // History: used to go into IO_Service_Config_param0.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedparam0 = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteIO_Service_Config(param0);
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


        // Functions for Register service
        public class Register_DataRecord : INotifyPropertyChanged
        {
            public Register_DataRecord()
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

    public DataCollection<Register_DataRecord> Register_DataRecordData { get; } = new DataCollection<Register_DataRecord>();
    private void OnRegister_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Register_DataRecordData.Count == 0)
            {
                Register_DataRecordData.AddRecord(new Register_DataRecord());
            }
            Register_DataRecordData[Register_DataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountRegister_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Register_DataRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmRegister_Data(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Register_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyRegister_Data(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in Register_DataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadRegister_Data(object sender, RoutedEventArgs e)
        {
            await DoReadRegister_Data();
        }

        private async Task DoReadRegister_Data()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadRegister_Data();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Register_Data");
                    return;
                }
                
                var record = new Register_DataRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Register_Data_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Register_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickRegister_Data(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteRegister_Data (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteRegister_Data(object sender, RoutedEventArgs e)
        {
            var text = Register_Data_param0.Text;
            await DoWriteRegister_Data (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteRegister_Data(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes param0;
                // History: used to go into Register_Data_param0.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedparam0 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteRegister_Data(param0);
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

        public class Register_AddressRecord : INotifyPropertyChanged
        {
            public Register_AddressRecord()
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

    public DataCollection<Register_AddressRecord> Register_AddressRecordData { get; } = new DataCollection<Register_AddressRecord>();
    private void OnRegister_Address_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Register_AddressRecordData.Count == 0)
            {
                Register_AddressRecordData.AddRecord(new Register_AddressRecord());
            }
            Register_AddressRecordData[Register_AddressRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountRegister_Address(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Register_AddressRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmRegister_Address(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Register_AddressRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyRegister_Address(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in Register_AddressRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadRegister_Address(object sender, RoutedEventArgs e)
        {
            await DoReadRegister_Address();
        }

        private async Task DoReadRegister_Address()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadRegister_Address();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Register_Address");
                    return;
                }
                
                var record = new Register_AddressRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Register_Address_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Register_AddressRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickRegister_Address(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteRegister_Address (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteRegister_Address(object sender, RoutedEventArgs e)
        {
            var text = Register_Address_param0.Text;
            await DoWriteRegister_Address (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteRegister_Address(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes param0;
                // History: used to go into Register_Address_param0.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedparam0 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteRegister_Address(param0);
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

        public class Register_Device_IDRecord : INotifyPropertyChanged
        {
            public Register_Device_IDRecord()
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

    public DataCollection<Register_Device_IDRecord> Register_Device_IDRecordData { get; } = new DataCollection<Register_Device_IDRecord>();
    private void OnRegister_Device_ID_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Register_Device_IDRecordData.Count == 0)
            {
                Register_Device_IDRecordData.AddRecord(new Register_Device_IDRecord());
            }
            Register_Device_IDRecordData[Register_Device_IDRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountRegister_Device_ID(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Register_Device_IDRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmRegister_Device_ID(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Register_Device_IDRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyRegister_Device_ID(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,param0,Notes\n");
        foreach (var row in Register_Device_IDRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.param0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadRegister_Device_ID(object sender, RoutedEventArgs e)
        {
            await DoReadRegister_Device_ID();
        }

        private async Task DoReadRegister_Device_ID()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadRegister_Device_ID();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Register_Device_ID");
                    return;
                }
                
                var record = new Register_Device_IDRecord();
                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Register_Device_ID_param0.Text = record.param0.ToString();
                    // CHANGE: ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                Register_Device_IDRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickRegister_Device_ID(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteRegister_Device_ID (text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteRegister_Device_ID(object sender, RoutedEventArgs e)
        {
            var text = Register_Device_ID_param0.Text;
            await DoWriteRegister_Device_ID (text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteRegister_Device_ID(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes param0;
                // History: used to go into Register_Device_ID_param0.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedparam0 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out param0);
                if (!parsedparam0)
                {
                    parseError = "param0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteRegister_Device_ID(param0);
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
            await bleDevice.EnsureCharacteristicAsync(CharacteristicsEnum.All_enum, true);
            SetStatusActive(false);
        }
    }
}