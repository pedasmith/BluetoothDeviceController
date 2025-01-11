using BluetoothDeviceController.Charts;
using BluetoothDeviceController.Lamps;
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
using static BluetoothProtocols.Elk_LedLight;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the ELK-BTCWCTB device
    /// </summary>
    public sealed partial class Elk_LedLightPage : Page, HasId, ISetHandleStatus
    {
        public Elk_LedLightPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.uiLampControl.Light = bleDevice;
        }
        private string DeviceName = "Elk_LedLight";
        private string DeviceNameUser = "ELK-BTCWCTB";

        int ncommand = 0;
        Elk_LedLight bleDevice = new Elk_LedLight();
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

            private double _Interval_Min;
            public double Interval_Min { get { return _Interval_Min; } set { if (value == _Interval_Min) return; _Interval_Min = value; OnPropertyChanged(); } }
            private double _Interval_Max;
            public double Interval_Max { get { return _Interval_Max; } set { if (value == _Interval_Max) return; _Interval_Max = value; OnPropertyChanged(); } }
            private double _Latency;
            public double Latency { get { return _Latency; } set { if (value == _Latency) return; _Latency = value; OnPropertyChanged(); } }
            private double _Timeout;
            public double Timeout { get { return _Timeout; } set { if (value == _Timeout) return; _Timeout = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,Interval_Min,Interval_Max,Latency,Timeout,Notes\n");
        foreach (var row in Connection_ParameterRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Interval_Min},{row.Interval_Max},{row.Latency},{row.Timeout},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                var Interval_Min = valueList.GetValue("Interval_Min");
                if (Interval_Min.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Interval_Min.CurrentType == BCBasic.BCValue.ValueType.IsString || Interval_Min.IsArray)
                {
                    record.Interval_Min = (double)Interval_Min.AsDouble;
                    Connection_Parameter_Interval_Min.Text = record.Interval_Min.ToString("N0");
                }
                var Interval_Max = valueList.GetValue("Interval_Max");
                if (Interval_Max.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Interval_Max.CurrentType == BCBasic.BCValue.ValueType.IsString || Interval_Max.IsArray)
                {
                    record.Interval_Max = (double)Interval_Max.AsDouble;
                    Connection_Parameter_Interval_Max.Text = record.Interval_Max.ToString("N0");
                }
                var Latency = valueList.GetValue("Latency");
                if (Latency.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Latency.CurrentType == BCBasic.BCValue.ValueType.IsString || Latency.IsArray)
                {
                    record.Latency = (double)Latency.AsDouble;
                    Connection_Parameter_Latency.Text = record.Latency.ToString("N0");
                }
                var Timeout = valueList.GetValue("Timeout");
                if (Timeout.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Timeout.CurrentType == BCBasic.BCValue.ValueType.IsString || Timeout.IsArray)
                {
                    record.Timeout = (double)Timeout.AsDouble;
                    Connection_Parameter_Timeout.Text = record.Timeout.ToString("N0");
                }

                Connection_ParameterRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }




        // Functions for Generic Service
        public class Service_ChangesRecord : INotifyPropertyChanged
        {
            public Service_ChangesRecord()
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

            private double _StartRange;
            public double StartRange { get { return _StartRange; } set { if (value == _StartRange) return; _StartRange = value; OnPropertyChanged(); } }
            private double _EndRange;
            public double EndRange { get { return _EndRange; } set { if (value == _EndRange) return; _EndRange = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Service_ChangesRecord> Service_ChangesRecordData { get; } = new DataCollection<Service_ChangesRecord>();
    private void OnService_Changes_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Service_ChangesRecordData.Count == 0)
            {
                Service_ChangesRecordData.AddRecord(new Service_ChangesRecord());
            }
            Service_ChangesRecordData[Service_ChangesRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountService_Changes(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Service_ChangesRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmService_Changes(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Service_ChangesRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyService_Changes(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,StartRange,EndRange,Notes\n");
        foreach (var row in Service_ChangesRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.StartRange},{row.EndRange},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyService_ChangesSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Service_ChangesNotifyIndex = 0;
        bool Service_ChangesNotifySetup = false;
        private async void OnNotifyService_Changes(object sender, RoutedEventArgs e)
        {
            await DoNotifyService_Changes();
        }

        private async Task DoNotifyService_Changes()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Service_ChangesNotifySetup)
                {
                    Service_ChangesNotifySetup = true;
                    bleDevice.Service_ChangesEvent += BleDevice_Service_ChangesEvent;
                }
                var notifyType = NotifyService_ChangesSettings[Service_ChangesNotifyIndex];
                Service_ChangesNotifyIndex = (Service_ChangesNotifyIndex + 1) % NotifyService_ChangesSettings.Length;
                var result = await bleDevice.NotifyService_ChangesAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Service_ChangesEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Service_ChangesRecord();
                var StartRange = valueList.GetValue("StartRange");
                if (StartRange.CurrentType == BCBasic.BCValue.ValueType.IsDouble || StartRange.CurrentType == BCBasic.BCValue.ValueType.IsString || StartRange.IsArray)
                {
                    record.StartRange = (double)StartRange.AsDouble;
                    Service_Changes_StartRange.Text = record.StartRange.ToString("N0");
                }
                var EndRange = valueList.GetValue("EndRange");
                if (EndRange.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EndRange.CurrentType == BCBasic.BCValue.ValueType.IsString || EndRange.IsArray)
                {
                    record.EndRange = (double)EndRange.AsDouble;
                    Service_Changes_EndRange.Text = record.EndRange.ToString("N0");
                }

                var addResult = Service_ChangesRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }





        // Functions for Commands
        public class CommandRecord : INotifyPropertyChanged
        {
            public CommandRecord()
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

            private double _Start;
            public double Start { get { return _Start; } set { if (value == _Start) return; _Start = value; OnPropertyChanged(); } }
            private double _Counter;
            public double Counter { get { return _Counter; } set { if (value == _Counter) return; _Counter = value; OnPropertyChanged(); } }
            private double _Command;
            public double Command { get { return _Command; } set { if (value == _Command) return; _Command = value; OnPropertyChanged(); } }
            private double _P1;
            public double P1 { get { return _P1; } set { if (value == _P1) return; _P1 = value; OnPropertyChanged(); } }
            private double _P2;
            public double P2 { get { return _P2; } set { if (value == _P2) return; _P2 = value; OnPropertyChanged(); } }
            private double _P3;
            public double P3 { get { return _P3; } set { if (value == _P3) return; _P3 = value; OnPropertyChanged(); } }
            private double _P4;
            public double P4 { get { return _P4; } set { if (value == _P4) return; _P4 = value; OnPropertyChanged(); } }
            private double _P5;
            public double P5 { get { return _P5; } set { if (value == _P5) return; _P5 = value; OnPropertyChanged(); } }
            private double _End;
            public double End { get { return _End; } set { if (value == _End) return; _End = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<CommandRecord> CommandRecordData { get; } = new DataCollection<CommandRecord>();
    private void OnCommand_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (CommandRecordData.Count == 0)
            {
                CommandRecordData.AddRecord(new CommandRecord());
            }
            CommandRecordData[CommandRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountCommand(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        CommandRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmCommand(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        CommandRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyCommand(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Start,Counter,Command,P1,P2,P3,P4,P5,End,Notes\n");
        foreach (var row in CommandRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Start},{row.Counter},{row.Command},{row.P1},{row.P2},{row.P3},{row.P4},{row.P5},{row.End},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadCommand(object sender, RoutedEventArgs e)
        {
            await DoReadCommand();
        }

        private async Task DoReadCommand()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadCommand();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Command");
                    return;
                }
                
                var record = new CommandRecord();
                var Start = valueList.GetValue("Start");
                if (Start.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Start.CurrentType == BCBasic.BCValue.ValueType.IsString || Start.IsArray)
                {
                    record.Start = (double)Start.AsDouble;
                    Command_Start.Text = record.Start.ToString("N0");
                }
                var Counter = valueList.GetValue("Counter");
                if (Counter.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Counter.CurrentType == BCBasic.BCValue.ValueType.IsString || Counter.IsArray)
                {
                    record.Counter = (double)Counter.AsDouble;
                    Command_Counter.Text = record.Counter.ToString("N0");
                }
                var Command = valueList.GetValue("Command");
                if (Command.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Command.CurrentType == BCBasic.BCValue.ValueType.IsString || Command.IsArray)
                {
                    record.Command = (double)Command.AsDouble;
                    Command_Command.Text = record.Command.ToString("N0");
                }
                var P1 = valueList.GetValue("P1");
                if (P1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || P1.CurrentType == BCBasic.BCValue.ValueType.IsString || P1.IsArray)
                {
                    record.P1 = (double)P1.AsDouble;
                    Command_P1.Text = record.P1.ToString("N0");
                }
                var P2 = valueList.GetValue("P2");
                if (P2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || P2.CurrentType == BCBasic.BCValue.ValueType.IsString || P2.IsArray)
                {
                    record.P2 = (double)P2.AsDouble;
                    Command_P2.Text = record.P2.ToString("N0");
                }
                var P3 = valueList.GetValue("P3");
                if (P3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || P3.CurrentType == BCBasic.BCValue.ValueType.IsString || P3.IsArray)
                {
                    record.P3 = (double)P3.AsDouble;
                    Command_P3.Text = record.P3.ToString("N0");
                }
                var P4 = valueList.GetValue("P4");
                if (P4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || P4.CurrentType == BCBasic.BCValue.ValueType.IsString || P4.IsArray)
                {
                    record.P4 = (double)P4.AsDouble;
                    Command_P4.Text = record.P4.ToString("N0");
                }
                var P5 = valueList.GetValue("P5");
                if (P5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || P5.CurrentType == BCBasic.BCValue.ValueType.IsString || P5.IsArray)
                {
                    record.P5 = (double)P5.AsDouble;
                    Command_P5.Text = record.P5.ToString("N0");
                }
                var End = valueList.GetValue("End");
                if (End.CurrentType == BCBasic.BCValue.ValueType.IsDouble || End.CurrentType == BCBasic.BCValue.ValueType.IsString || End.IsArray)
                {
                    record.End = (double)End.AsDouble;
                    Command_End.Text = record.End.ToString("N0");
                }

                CommandRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickCommand(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Command_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_Counter.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_Command.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P1.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P2.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P3.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P4.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P5.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_End.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteCommand(values);

        }

        private async void OnWriteCommand(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Command_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_Counter.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_Command.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P1.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P2.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P3.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P4.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_P5.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Command_End.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteCommand(values);

        }

        private async Task DoWriteCommand(List<UxTextValue> values)
        {
            if (values.Count != 9) return;
            int valueIndex = 0; // Change #3;

            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Start;
                // History: used to go into Command_Start.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedStart = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Start);
                valueIndex++; // Change #5
                if (!parsedStart)
                {
                    parseError = "Start";
                }
                Byte Counter;
                // History: used to go into Command_Counter.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCounter = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Counter);
                valueIndex++; // Change #5
                if (!parsedCounter)
                {
                    parseError = "Counter";
                }
                Byte Command;
                // History: used to go into Command_Command.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCommand = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Command);
                valueIndex++; // Change #5
                if (!parsedCommand)
                {
                    parseError = "Command";
                }
                Byte P1;
                // History: used to go into Command_P1.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedP1 = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out P1);
                valueIndex++; // Change #5
                if (!parsedP1)
                {
                    parseError = "P1";
                }
                Byte P2;
                // History: used to go into Command_P2.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedP2 = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out P2);
                valueIndex++; // Change #5
                if (!parsedP2)
                {
                    parseError = "P2";
                }
                Byte P3;
                // History: used to go into Command_P3.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedP3 = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out P3);
                valueIndex++; // Change #5
                if (!parsedP3)
                {
                    parseError = "P3";
                }
                Byte P4;
                // History: used to go into Command_P4.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedP4 = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out P4);
                valueIndex++; // Change #5
                if (!parsedP4)
                {
                    parseError = "P4";
                }
                Byte P5;
                // History: used to go into Command_P5.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedP5 = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out P5);
                valueIndex++; // Change #5
                if (!parsedP5)
                {
                    parseError = "P5";
                }
                Byte End;
                // History: used to go into Command_End.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedEnd = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out End);
                valueIndex++; // Change #5
                if (!parsedEnd)
                {
                    parseError = "End";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteCommand(Start, Counter, Command, P1, P2, P3, P4, P5, End);
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


        public class ResponseRecord : INotifyPropertyChanged
        {
            public ResponseRecord()
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

            private string _Response;
            public string Response { get { return _Response; } set { if (value == _Response) return; _Response = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<ResponseRecord> ResponseRecordData { get; } = new DataCollection<ResponseRecord>();
    private void OnResponse_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (ResponseRecordData.Count == 0)
            {
                ResponseRecordData.AddRecord(new ResponseRecord());
            }
            ResponseRecordData[ResponseRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountResponse(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        ResponseRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmResponse(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        ResponseRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyResponse(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Response,Notes\n");
        foreach (var row in ResponseRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Response},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyResponseSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int ResponseNotifyIndex = 0;
        bool ResponseNotifySetup = false;
        private async void OnNotifyResponse(object sender, RoutedEventArgs e)
        {
            await DoNotifyResponse();
        }

        private async Task DoNotifyResponse()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!ResponseNotifySetup)
                {
                    ResponseNotifySetup = true;
                    bleDevice.ResponseEvent += BleDevice_ResponseEvent;
                }
                var notifyType = NotifyResponseSettings[ResponseNotifyIndex];
                ResponseNotifyIndex = (ResponseNotifyIndex + 1) % NotifyResponseSettings.Length;
                var result = await bleDevice.NotifyResponseAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_ResponseEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new ResponseRecord();
                var Response = valueList.GetValue("Response");
                if (Response.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Response.CurrentType == BCBasic.BCValue.ValueType.IsString || Response.IsArray)
                {
                    record.Response = (string)Response.AsString;
                    Response_Response.Text = record.Response.ToString();
                }

                var addResult = ResponseRecordData.AddRecord(record);

                
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