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
using Microsoft.Toolkit.Uwp.UI.Controls;
using static BluetoothProtocols.SensyOne_S1ProMultiSense;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the HLK-LD2450_ device
    /// </summary>
    public sealed partial class SensyOne_S1ProMultiSensePage : Page, HasId, ISetHandleStatus
    {
        public SensyOne_S1ProMultiSensePage()
        {
            this.InitializeComponent();
            this.DataContext = this;

        }
        private string DeviceName = "SensyOne_S1ProMultiSense";
        private string DeviceNameUser = "HLK-LD2450_";

        int ncommand = 0;
        SensyOne_S1ProMultiSense bleDevice = new SensyOne_S1ProMultiSense();
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

        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickDevice_Name(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Device_Name_Device_Name.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Device_Name_Device_Name.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteDevice_Name(values);

        }

        private async void OnWriteDevice_Name(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Device_Name_Device_Name.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Device_Name_Device_Name.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteDevice_Name(values);

        }

        private async Task DoWriteDevice_Name(List<UxTextValue> values)
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

                String Device_Name;
                // History: used to go into Device_Name_Device_Name.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDevice_Name = Utilities.Parsers.TryParseString(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Device_Name);
                valueIndex++; // Change #5
                if (!parsedDevice_Name)
                {
                    parseError = "Device Name";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteDevice_Name(Device_Name);
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




        // Functions for Radar
        public class RadarControlRecord : INotifyPropertyChanged
        {
            public RadarControlRecord()
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

            private string _Unknown10;
            public string Unknown10 { get { return _Unknown10; } set { if (value == _Unknown10) return; _Unknown10 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<RadarControlRecord> RadarControlRecordData { get; } = new DataCollection<RadarControlRecord>();
    private void OnRadarControl_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (RadarControlRecordData.Count == 0)
            {
                RadarControlRecordData.AddRecord(new RadarControlRecord());
            }
            RadarControlRecordData[RadarControlRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountRadarControl(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        RadarControlRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmRadarControl(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        RadarControlRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyRadarControl(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Unknown10,Notes\n");
        foreach (var row in RadarControlRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown10},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickRadarControl(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(RadarControl_Unknown10.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(RadarControl_Unknown10.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteRadarControl(values);

        }

        private async void OnWriteRadarControl(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(RadarControl_Unknown10.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(RadarControl_Unknown10.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteRadarControl(values);

        }

        private async Task DoWriteRadarControl(List<UxTextValue> values)
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

                Bytes Unknown10;
                // History: used to go into RadarControl_Unknown10.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedUnknown10 = Utilities.Parsers.TryParseBytes(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Unknown10);
                valueIndex++; // Change #5
                if (!parsedUnknown10)
                {
                    parseError = "Unknown10";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteRadarControl(Unknown10);
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



        public class RadarDataRecord : INotifyPropertyChanged
        {
            public RadarDataRecord()
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

            private double _Opcode;
            public double Opcode { get { return _Opcode; } set { if (value == _Opcode) return; _Opcode = value; OnPropertyChanged(); } }
            private double _NElements;
            public double NElements { get { return _NElements; } set { if (value == _NElements) return; _NElements = value; OnPropertyChanged(); } }
            private double _X1cm;
            public double X1cm { get { return _X1cm; } set { if (value == _X1cm) return; _X1cm = value; OnPropertyChanged(); } }
            private double _Y1cm;
            public double Y1cm { get { return _Y1cm; } set { if (value == _Y1cm) return; _Y1cm = value; OnPropertyChanged(); } }
            private double _Speed1;
            public double Speed1 { get { return _Speed1; } set { if (value == _Speed1) return; _Speed1 = value; OnPropertyChanged(); } }
            private double _Unused1;
            public double Unused1 { get { return _Unused1; } set { if (value == _Unused1) return; _Unused1 = value; OnPropertyChanged(); } }
            private double _X2cm;
            public double X2cm { get { return _X2cm; } set { if (value == _X2cm) return; _X2cm = value; OnPropertyChanged(); } }
            private double _Y2cm;
            public double Y2cm { get { return _Y2cm; } set { if (value == _Y2cm) return; _Y2cm = value; OnPropertyChanged(); } }
            private double _Speed2;
            public double Speed2 { get { return _Speed2; } set { if (value == _Speed2) return; _Speed2 = value; OnPropertyChanged(); } }
            private double _Unused2;
            public double Unused2 { get { return _Unused2; } set { if (value == _Unused2) return; _Unused2 = value; OnPropertyChanged(); } }
            private double _X3cm;
            public double X3cm { get { return _X3cm; } set { if (value == _X3cm) return; _X3cm = value; OnPropertyChanged(); } }
            private double _Y3cm;
            public double Y3cm { get { return _Y3cm; } set { if (value == _Y3cm) return; _Y3cm = value; OnPropertyChanged(); } }
            private double _Speed3;
            public double Speed3 { get { return _Speed3; } set { if (value == _Speed3) return; _Speed3 = value; OnPropertyChanged(); } }
            private double _Unused3;
            public double Unused3 { get { return _Unused3; } set { if (value == _Unused3) return; _Unused3 = value; OnPropertyChanged(); } }
            private double _End55CC;
            public double End55CC { get { return _End55CC; } set { if (value == _End55CC) return; _End55CC = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<RadarDataRecord> RadarDataRecordData { get; } = new DataCollection<RadarDataRecord>();
    private void OnRadarData_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (RadarDataRecordData.Count == 0)
            {
                RadarDataRecordData.AddRecord(new RadarDataRecord());
            }
            RadarDataRecordData[RadarDataRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountRadarData(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        RadarDataRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmRadarData(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        RadarDataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyRadarData(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Opcode,NElements,X1cm,Y1cm,Speed1,Unused1,X2cm,Y2cm,Speed2,Unused2,X3cm,Y3cm,Speed3,Unused3,End55CC,Notes\n");
        foreach (var row in RadarDataRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Opcode},{row.NElements},{row.X1cm},{row.Y1cm},{row.Speed1},{row.Unused1},{row.X2cm},{row.Y2cm},{row.Speed2},{row.Unused2},{row.X3cm},{row.Y3cm},{row.Speed3},{row.Unused3},{row.End55CC},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyRadarDataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int RadarDataNotifyIndex = 0;
        bool RadarDataNotifySetup = false;
        private async void OnNotifyRadarData(object sender, RoutedEventArgs e)
        {
            await DoNotifyRadarData();
        }

        private async Task DoNotifyRadarData()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!RadarDataNotifySetup)
                {
                    RadarDataNotifySetup = true;
                    bleDevice.RadarDataEvent += BleDevice_RadarDataEvent;
                }
                var notifyType = NotifyRadarDataSettings[RadarDataNotifyIndex];
                RadarDataNotifyIndex = (RadarDataNotifyIndex + 1) % NotifyRadarDataSettings.Length;
                var result = await bleDevice.NotifyRadarDataAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_RadarDataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new RadarDataRecord();
                var Opcode = valueList.GetValue("Opcode");
                if (Opcode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Opcode.CurrentType == BCBasic.BCValue.ValueType.IsString || Opcode.IsArray)
                {
                    record.Opcode = (double)Opcode.AsDouble;
                    RadarData_Opcode.Text = record.Opcode.ToString("N0");
                }
                var NElements = valueList.GetValue("NElements");
                if (NElements.CurrentType == BCBasic.BCValue.ValueType.IsDouble || NElements.CurrentType == BCBasic.BCValue.ValueType.IsString || NElements.IsArray)
                {
                    record.NElements = (double)NElements.AsDouble;
                    RadarData_NElements.Text = record.NElements.ToString("N0");
                }
                var X1cm = valueList.GetValue("X1cm");
                if (X1cm.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X1cm.CurrentType == BCBasic.BCValue.ValueType.IsString || X1cm.IsArray)
                {
                    record.X1cm = (double)X1cm.AsDouble;
                    RadarData_X1cm.Text = record.X1cm.ToString("F3");
                }
                var Y1cm = valueList.GetValue("Y1cm");
                if (Y1cm.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y1cm.CurrentType == BCBasic.BCValue.ValueType.IsString || Y1cm.IsArray)
                {
                    record.Y1cm = (double)Y1cm.AsDouble;
                    RadarData_Y1cm.Text = record.Y1cm.ToString("F3");
                }
                var Speed1 = valueList.GetValue("Speed1");
                if (Speed1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Speed1.CurrentType == BCBasic.BCValue.ValueType.IsString || Speed1.IsArray)
                {
                    record.Speed1 = (double)Speed1.AsDouble;
                    RadarData_Speed1.Text = record.Speed1.ToString("N0");
                }
                var Unused1 = valueList.GetValue("Unused1");
                if (Unused1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unused1.CurrentType == BCBasic.BCValue.ValueType.IsString || Unused1.IsArray)
                {
                    record.Unused1 = (double)Unused1.AsDouble;
                    RadarData_Unused1.Text = record.Unused1.ToString("N0");
                }
                var X2cm = valueList.GetValue("X2cm");
                if (X2cm.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X2cm.CurrentType == BCBasic.BCValue.ValueType.IsString || X2cm.IsArray)
                {
                    record.X2cm = (double)X2cm.AsDouble;
                    RadarData_X2cm.Text = record.X2cm.ToString("F3");
                }
                var Y2cm = valueList.GetValue("Y2cm");
                if (Y2cm.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y2cm.CurrentType == BCBasic.BCValue.ValueType.IsString || Y2cm.IsArray)
                {
                    record.Y2cm = (double)Y2cm.AsDouble;
                    RadarData_Y2cm.Text = record.Y2cm.ToString("F3");
                }
                var Speed2 = valueList.GetValue("Speed2");
                if (Speed2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Speed2.CurrentType == BCBasic.BCValue.ValueType.IsString || Speed2.IsArray)
                {
                    record.Speed2 = (double)Speed2.AsDouble;
                    RadarData_Speed2.Text = record.Speed2.ToString("N0");
                }
                var Unused2 = valueList.GetValue("Unused2");
                if (Unused2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unused2.CurrentType == BCBasic.BCValue.ValueType.IsString || Unused2.IsArray)
                {
                    record.Unused2 = (double)Unused2.AsDouble;
                    RadarData_Unused2.Text = record.Unused2.ToString("N0");
                }
                var X3cm = valueList.GetValue("X3cm");
                if (X3cm.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X3cm.CurrentType == BCBasic.BCValue.ValueType.IsString || X3cm.IsArray)
                {
                    record.X3cm = (double)X3cm.AsDouble;
                    RadarData_X3cm.Text = record.X3cm.ToString("F3");
                }
                var Y3cm = valueList.GetValue("Y3cm");
                if (Y3cm.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y3cm.CurrentType == BCBasic.BCValue.ValueType.IsString || Y3cm.IsArray)
                {
                    record.Y3cm = (double)Y3cm.AsDouble;
                    RadarData_Y3cm.Text = record.Y3cm.ToString("F3");
                }
                var Speed3 = valueList.GetValue("Speed3");
                if (Speed3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Speed3.CurrentType == BCBasic.BCValue.ValueType.IsString || Speed3.IsArray)
                {
                    record.Speed3 = (double)Speed3.AsDouble;
                    RadarData_Speed3.Text = record.Speed3.ToString("N0");
                }
                var Unused3 = valueList.GetValue("Unused3");
                if (Unused3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unused3.CurrentType == BCBasic.BCValue.ValueType.IsString || Unused3.IsArray)
                {
                    record.Unused3 = (double)Unused3.AsDouble;
                    RadarData_Unused3.Text = record.Unused3.ToString("N0");
                }
                var End55CC = valueList.GetValue("End55CC");
                if (End55CC.CurrentType == BCBasic.BCValue.ValueType.IsDouble || End55CC.CurrentType == BCBasic.BCValue.ValueType.IsString || End55CC.IsArray)
                {
                    record.End55CC = (double)End55CC.AsDouble;
                    RadarData_End55CC.Text = record.End55CC.ToString("N0");
                }

                var addResult = RadarDataRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }






        // Functions for Unknown2
        public class Unknown20Record : INotifyPropertyChanged
        {
            public Unknown20Record()
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

            private string _Unknown20;
            public string Unknown20 { get { return _Unknown20; } set { if (value == _Unknown20) return; _Unknown20 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Unknown20Record> Unknown20RecordData { get; } = new DataCollection<Unknown20Record>();
    private void OnUnknown20_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Unknown20RecordData.Count == 0)
            {
                Unknown20RecordData.AddRecord(new Unknown20Record());
            }
            Unknown20RecordData[Unknown20RecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountUnknown20(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Unknown20RecordData.MaxLength = value;

        
    }

    private void OnAlgorithmUnknown20(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Unknown20RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyUnknown20(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Unknown20,Notes\n");
        foreach (var row in Unknown20RecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown20},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickUnknown20(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Unknown20_Unknown20.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Unknown20_Unknown20.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteUnknown20(values);

        }

        private async void OnWriteUnknown20(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Unknown20_Unknown20.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Unknown20_Unknown20.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteUnknown20(values);

        }

        private async Task DoWriteUnknown20(List<UxTextValue> values)
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

                Bytes Unknown20;
                // History: used to go into Unknown20_Unknown20.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedUnknown20 = Utilities.Parsers.TryParseBytes(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Unknown20);
                valueIndex++; // Change #5
                if (!parsedUnknown20)
                {
                    parseError = "Unknown20";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUnknown20(Unknown20);
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



        public class Unknown21Record : INotifyPropertyChanged
        {
            public Unknown21Record()
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

            private string _Unknown21;
            public string Unknown21 { get { return _Unknown21; } set { if (value == _Unknown21) return; _Unknown21 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<Unknown21Record> Unknown21RecordData { get; } = new DataCollection<Unknown21Record>();
    private void OnUnknown21_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (Unknown21RecordData.Count == 0)
            {
                Unknown21RecordData.AddRecord(new Unknown21Record());
            }
            Unknown21RecordData[Unknown21RecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountUnknown21(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Unknown21RecordData.MaxLength = value;

        
    }

    private void OnAlgorithmUnknown21(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        Unknown21RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyUnknown21(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Unknown21,Notes\n");
        foreach (var row in Unknown21RecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown21},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyUnknown21Settings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Unknown21NotifyIndex = 0;
        bool Unknown21NotifySetup = false;
        private async void OnNotifyUnknown21(object sender, RoutedEventArgs e)
        {
            await DoNotifyUnknown21();
        }

        private async Task DoNotifyUnknown21()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Unknown21NotifySetup)
                {
                    Unknown21NotifySetup = true;
                    bleDevice.Unknown21Event += BleDevice_Unknown21Event;
                }
                var notifyType = NotifyUnknown21Settings[Unknown21NotifyIndex];
                Unknown21NotifyIndex = (Unknown21NotifyIndex + 1) % NotifyUnknown21Settings.Length;
                var result = await bleDevice.NotifyUnknown21Async(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Unknown21Event(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new Unknown21Record();
                var Unknown21 = valueList.GetValue("Unknown21");
                if (Unknown21.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown21.CurrentType == BCBasic.BCValue.ValueType.IsString || Unknown21.IsArray)
                {
                    record.Unknown21 = (string)Unknown21.AsString;
                    Unknown21_Unknown21.Text = record.Unknown21.ToString();
                }

                var addResult = Unknown21RecordData.AddRecord(record);

                
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