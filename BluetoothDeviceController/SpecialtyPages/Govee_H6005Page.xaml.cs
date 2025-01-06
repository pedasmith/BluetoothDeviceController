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
using static BluetoothProtocols.Govee_H6005;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the ihoment_H6005_ device
    /// </summary>
    public sealed partial class Govee_H6005Page : Page, HasId, ISetHandleStatus
    {
        public Govee_H6005Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Govee_H6005";
        private string DeviceNameUser = "ihoment_H6005_";

        int ncommand = 0;
        Govee_H6005 bleDevice = new Govee_H6005();
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

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyDevice_NameSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Device_NameNotifyIndex = 0;
        bool Device_NameNotifySetup = false;
        private async void OnNotifyDevice_Name(object sender, RoutedEventArgs e)
        {
            await DoNotifyDevice_Name();
        }

        private async Task DoNotifyDevice_Name()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Device_NameNotifySetup)
                {
                    Device_NameNotifySetup = true;
                    bleDevice.Device_NameEvent += BleDevice_Device_NameEvent;
                }
                var notifyType = NotifyDevice_NameSettings[Device_NameNotifyIndex];
                Device_NameNotifyIndex = (Device_NameNotifyIndex + 1) % NotifyDevice_NameSettings.Length;
                var result = await bleDevice.NotifyDevice_NameAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Device_NameEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Device_NameRecord();
                var Device_Name = valueList.GetValue("Device_Name");
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString || Device_Name.IsArray)
                {
                    record.Device_Name = (string)Device_Name.AsString;
                    Device_Name_Device_Name.Text = record.Device_Name.ToString();
                }

                var addResult = Device_NameRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
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





        // Functions for Device Info
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
                }

                PnP_IDRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }




        // Functions for LED_Command
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

            private string _Rx;
            public string Rx { get { return _Rx; } set { if (value == _Rx) return; _Rx = value; OnPropertyChanged(); } }

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
        sb.Append("EventDate,EventTime,Rx,Notes\n");
        foreach (var row in ResponseRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Rx},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
                var Rx = valueList.GetValue("Rx");
                if (Rx.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Rx.CurrentType == BCBasic.BCValue.ValueType.IsString || Rx.IsArray)
                {
                    record.Rx = (string)Rx.AsString;
                    Response_Rx.Text = record.Rx.ToString();
                }

                var addResult = ResponseRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadResponse(object sender, RoutedEventArgs e)
        {
            await DoReadResponse();
        }

        private async Task DoReadResponse()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadResponse();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Response");
                    return;
                }
                
                var record = new ResponseRecord();
                var Rx = valueList.GetValue("Rx");
                if (Rx.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Rx.CurrentType == BCBasic.BCValue.ValueType.IsString || Rx.IsArray)
                {
                    record.Rx = (string)Rx.AsString;
                    Response_Rx.Text = record.Rx.ToString();
                }

                ResponseRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class SendRecord : INotifyPropertyChanged
        {
            public SendRecord()
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
            private double _Command;
            public double Command { get { return _Command; } set { if (value == _Command) return; _Command = value; OnPropertyChanged(); } }
            private double _Mode;
            public double Mode { get { return _Mode; } set { if (value == _Mode) return; _Mode = value; OnPropertyChanged(); } }
            private double _R;
            public double R { get { return _R; } set { if (value == _R) return; _R = value; OnPropertyChanged(); } }
            private double _G;
            public double G { get { return _G; } set { if (value == _G) return; _G = value; OnPropertyChanged(); } }
            private double _B;
            public double B { get { return _B; } set { if (value == _B) return; _B = value; OnPropertyChanged(); } }
            private string _Blank;
            public string Blank { get { return _Blank; } set { if (value == _Blank) return; _Blank = value; OnPropertyChanged(); } }
            private double _CRC;
            public double CRC { get { return _CRC; } set { if (value == _CRC) return; _CRC = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<SendRecord> SendRecordData { get; } = new DataCollection<SendRecord>();
    private void OnSend_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (SendRecordData.Count == 0)
            {
                SendRecordData.AddRecord(new SendRecord());
            }
            SendRecordData[SendRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountSend(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        SendRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmSend(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        SendRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopySend(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Start,Command,Mode,R,G,B,Blank,CRC,Notes\n");
        foreach (var row in SendRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Start},{row.Command},{row.Mode},{row.R},{row.G},{row.B},{row.Blank},{row.CRC},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadSend(object sender, RoutedEventArgs e)
        {
            await DoReadSend();
        }

        private async Task DoReadSend()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSend();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read Send");
                    return;
                }
                
                var record = new SendRecord();
                var Start = valueList.GetValue("Start");
                if (Start.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Start.CurrentType == BCBasic.BCValue.ValueType.IsString || Start.IsArray)
                {
                    record.Start = (double)Start.AsDouble;
                    Send_Start.Text = record.Start.ToString("N0");
                }
                var Command = valueList.GetValue("Command");
                if (Command.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Command.CurrentType == BCBasic.BCValue.ValueType.IsString || Command.IsArray)
                {
                    record.Command = (double)Command.AsDouble;
                    Send_Command.Text = record.Command.ToString("N0");
                }
                var Mode = valueList.GetValue("Mode");
                if (Mode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Mode.CurrentType == BCBasic.BCValue.ValueType.IsString || Mode.IsArray)
                {
                    record.Mode = (double)Mode.AsDouble;
                    Send_Mode.Text = record.Mode.ToString("N0");
                }
                var R = valueList.GetValue("R");
                if (R.CurrentType == BCBasic.BCValue.ValueType.IsDouble || R.CurrentType == BCBasic.BCValue.ValueType.IsString || R.IsArray)
                {
                    record.R = (double)R.AsDouble;
                    Send_R.Text = record.R.ToString("N0");
                }
                var G = valueList.GetValue("G");
                if (G.CurrentType == BCBasic.BCValue.ValueType.IsDouble || G.CurrentType == BCBasic.BCValue.ValueType.IsString || G.IsArray)
                {
                    record.G = (double)G.AsDouble;
                    Send_G.Text = record.G.ToString("N0");
                }
                var B = valueList.GetValue("B");
                if (B.CurrentType == BCBasic.BCValue.ValueType.IsDouble || B.CurrentType == BCBasic.BCValue.ValueType.IsString || B.IsArray)
                {
                    record.B = (double)B.AsDouble;
                    Send_B.Text = record.B.ToString("N0");
                }
                var Blank = valueList.GetValue("Blank");
                if (Blank.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Blank.CurrentType == BCBasic.BCValue.ValueType.IsString || Blank.IsArray)
                {
                    record.Blank = (string)Blank.AsString;
                    Send_Blank.Text = record.Blank.ToString();
                }
                var CRC = valueList.GetValue("CRC");
                if (CRC.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CRC.CurrentType == BCBasic.BCValue.ValueType.IsString || CRC.IsArray)
                {
                    record.CRC = (double)CRC.AsDouble;
                    Send_CRC.Text = record.CRC.ToString("N0");
                }

                SendRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickSend(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Send_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Command.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Mode.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_R.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_G.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_B.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Blank.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_CRC.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteSend(values);

        }

        private async void OnWriteSend(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Send_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Start.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Command.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Mode.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_R.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_G.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_B.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_Blank.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Send_CRC.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteSend(values);

        }

        private async Task DoWriteSend(List<UxTextValue> values)
        {
            if (values.Count != 8) return;
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
                // History: used to go into Send_Start.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedStart = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Start);
                valueIndex++; // Change #5
                if (!parsedStart)
                {
                    parseError = "Start";
                }
                Byte Command;
                // History: used to go into Send_Command.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCommand = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Command);
                valueIndex++; // Change #5
                if (!parsedCommand)
                {
                    parseError = "Command";
                }
                Byte Mode;
                // History: used to go into Send_Mode.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedMode = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Mode);
                valueIndex++; // Change #5
                if (!parsedMode)
                {
                    parseError = "Mode";
                }
                Byte R;
                // History: used to go into Send_R.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedR = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out R);
                valueIndex++; // Change #5
                if (!parsedR)
                {
                    parseError = "R";
                }
                Byte G;
                // History: used to go into Send_G.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedG = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out G);
                valueIndex++; // Change #5
                if (!parsedG)
                {
                    parseError = "G";
                }
                Byte B;
                // History: used to go into Send_B.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedB = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out B);
                valueIndex++; // Change #5
                if (!parsedB)
                {
                    parseError = "B";
                }
                Bytes Blank;
                // History: used to go into Send_Blank.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedBlank = Utilities.Parsers.TryParseBytes(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Blank);
                valueIndex++; // Change #5
                if (!parsedBlank)
                {
                    parseError = "Blank";
                }
                Byte CRC;
                // History: used to go into Send_CRC.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCRC = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out CRC);
                valueIndex++; // Change #5
                if (!parsedCRC)
                {
                    parseError = "CRC";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteSend(Start, Command, Mode, R, G, B, Blank, CRC);
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



        // Functions for OtaCommand
        public class OTARecord : INotifyPropertyChanged
        {
            public OTARecord()
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

            private string _OTA;
            public string OTA { get { return _OTA; } set { if (value == _OTA) return; _OTA = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<OTARecord> OTARecordData { get; } = new DataCollection<OTARecord>();
    private void OnOTA_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (OTARecordData.Count == 0)
            {
                OTARecordData.AddRecord(new OTARecord());
            }
            OTARecordData[OTARecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountOTA(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        OTARecordData.MaxLength = value;

        
    }

    private void OnAlgorithmOTA(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        OTARecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyOTA(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,OTA,Notes\n");
        foreach (var row in OTARecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.OTA},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }


        private async void OnReadOTA(object sender, RoutedEventArgs e)
        {
            await DoReadOTA();
        }

        private async Task DoReadOTA()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadOTA();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read OTA");
                    return;
                }
                
                var record = new OTARecord();
                var OTA = valueList.GetValue("OTA");
                if (OTA.CurrentType == BCBasic.BCValue.ValueType.IsDouble || OTA.CurrentType == BCBasic.BCValue.ValueType.IsString || OTA.IsArray)
                {
                    record.OTA = (string)OTA.AsString;
                    OTA_OTA.Text = record.OTA.ToString();
                }

                OTARecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickOTA(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(OTA_OTA.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(OTA_OTA.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteOTA(values);

        }

        private async void OnWriteOTA(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(OTA_OTA.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(OTA_OTA.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteOTA(values);

        }

        private async Task DoWriteOTA(List<UxTextValue> values)
        {
            if (values.Count != 1) return;
            int valueIndex = 0; // Change #3;

            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes OTA;
                // History: used to go into OTA_OTA.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedOTA = Utilities.Parsers.TryParseBytes(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out OTA);
                valueIndex++; // Change #5
                if (!parsedOTA)
                {
                    parseError = "OTA";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteOTA(OTA);
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