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
using static BluetoothProtocols.Zengge_LedLight;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the IOTWF8FF device
    /// </summary>
    public sealed partial class Zengge_LedLightPage : Page, HasId, ISetHandleStatus
    {
        public Zengge_LedLightPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Zengge_LedLight";
        private string DeviceNameUser = "IOTWF8FF";

        int ncommand = 0;
        Zengge_LedLight bleDevice = new Zengge_LedLight();
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




        // Functions for LED_Control
        public class LED_ResponseRecord : INotifyPropertyChanged
        {
            public LED_ResponseRecord()
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

            private double _Junk10;
            public double Junk10 { get { return _Junk10; } set { if (value == _Junk10) return; _Junk10 = value; OnPropertyChanged(); } }
            private double _Counter;
            public double Counter { get { return _Counter; } set { if (value == _Counter) return; _Counter = value; OnPropertyChanged(); } }
            private double _Junk11;
            public double Junk11 { get { return _Junk11; } set { if (value == _Junk11) return; _Junk11 = value; OnPropertyChanged(); } }
            private double _Junk12;
            public double Junk12 { get { return _Junk12; } set { if (value == _Junk12) return; _Junk12 = value; OnPropertyChanged(); } }
            private string _JsonResponse;
            public string JsonResponse { get { return _JsonResponse; } set { if (value == _JsonResponse) return; _JsonResponse = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<LED_ResponseRecord> LED_ResponseRecordData { get; } = new DataCollection<LED_ResponseRecord>();
    private void OnLED_Response_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (LED_ResponseRecordData.Count == 0)
            {
                LED_ResponseRecordData.AddRecord(new LED_ResponseRecord());
            }
            LED_ResponseRecordData[LED_ResponseRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountLED_Response(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        LED_ResponseRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmLED_Response(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        LED_ResponseRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyLED_Response(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Junk10,Counter,Junk11,Junk12,JsonResponse,Notes\n");
        foreach (var row in LED_ResponseRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Junk10},{row.Counter},{row.Junk11},{row.Junk12},{row.JsonResponse},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyLED_ResponseSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int LED_ResponseNotifyIndex = 0;
        bool LED_ResponseNotifySetup = false;
        private async void OnNotifyLED_Response(object sender, RoutedEventArgs e)
        {
            await DoNotifyLED_Response();
        }

        private async Task DoNotifyLED_Response()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!LED_ResponseNotifySetup)
                {
                    LED_ResponseNotifySetup = true;
                    bleDevice.LED_ResponseEvent += BleDevice_LED_ResponseEvent;
                }
                var notifyType = NotifyLED_ResponseSettings[LED_ResponseNotifyIndex];
                LED_ResponseNotifyIndex = (LED_ResponseNotifyIndex + 1) % NotifyLED_ResponseSettings.Length;
                var result = await bleDevice.NotifyLED_ResponseAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_LED_ResponseEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new LED_ResponseRecord();
                var Junk10 = valueList.GetValue("Junk10");
                if (Junk10.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Junk10.CurrentType == BCBasic.BCValue.ValueType.IsString || Junk10.IsArray)
                {
                    record.Junk10 = (double)Junk10.AsDouble;
                    LED_Response_Junk10.Text = record.Junk10.ToString("N0");
                }
                var Counter = valueList.GetValue("Counter");
                if (Counter.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Counter.CurrentType == BCBasic.BCValue.ValueType.IsString || Counter.IsArray)
                {
                    record.Counter = (double)Counter.AsDouble;
                    LED_Response_Counter.Text = record.Counter.ToString("N0");
                }
                var Junk11 = valueList.GetValue("Junk11");
                if (Junk11.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Junk11.CurrentType == BCBasic.BCValue.ValueType.IsString || Junk11.IsArray)
                {
                    record.Junk11 = (double)Junk11.AsDouble;
                    LED_Response_Junk11.Text = record.Junk11.ToString("N0");
                }
                var Junk12 = valueList.GetValue("Junk12");
                if (Junk12.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Junk12.CurrentType == BCBasic.BCValue.ValueType.IsString || Junk12.IsArray)
                {
                    record.Junk12 = (double)Junk12.AsDouble;
                    LED_Response_Junk12.Text = record.Junk12.ToString("N0");
                }
                var JsonResponse = valueList.GetValue("JsonResponse");
                if (JsonResponse.CurrentType == BCBasic.BCValue.ValueType.IsDouble || JsonResponse.CurrentType == BCBasic.BCValue.ValueType.IsString || JsonResponse.IsArray)
                {
                    record.JsonResponse = (string)JsonResponse.AsString;
                    LED_Response_JsonResponse.Text = record.JsonResponse.ToString();
                }

                var addResult = LED_ResponseRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadLED_Response(object sender, RoutedEventArgs e)
        {
            await DoReadLED_Response();
        }

        private async Task DoReadLED_Response()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadLED_Response();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read LED_Response");
                    return;
                }
                
                var record = new LED_ResponseRecord();
                var Junk10 = valueList.GetValue("Junk10");
                if (Junk10.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Junk10.CurrentType == BCBasic.BCValue.ValueType.IsString || Junk10.IsArray)
                {
                    record.Junk10 = (double)Junk10.AsDouble;
                    LED_Response_Junk10.Text = record.Junk10.ToString("N0");
                }
                var Counter = valueList.GetValue("Counter");
                if (Counter.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Counter.CurrentType == BCBasic.BCValue.ValueType.IsString || Counter.IsArray)
                {
                    record.Counter = (double)Counter.AsDouble;
                    LED_Response_Counter.Text = record.Counter.ToString("N0");
                }
                var Junk11 = valueList.GetValue("Junk11");
                if (Junk11.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Junk11.CurrentType == BCBasic.BCValue.ValueType.IsString || Junk11.IsArray)
                {
                    record.Junk11 = (double)Junk11.AsDouble;
                    LED_Response_Junk11.Text = record.Junk11.ToString("N0");
                }
                var Junk12 = valueList.GetValue("Junk12");
                if (Junk12.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Junk12.CurrentType == BCBasic.BCValue.ValueType.IsString || Junk12.IsArray)
                {
                    record.Junk12 = (double)Junk12.AsDouble;
                    LED_Response_Junk12.Text = record.Junk12.ToString("N0");
                }
                var JsonResponse = valueList.GetValue("JsonResponse");
                if (JsonResponse.CurrentType == BCBasic.BCValue.ValueType.IsDouble || JsonResponse.CurrentType == BCBasic.BCValue.ValueType.IsString || JsonResponse.IsArray)
                {
                    record.JsonResponse = (string)JsonResponse.AsString;
                    LED_Response_JsonResponse.Text = record.JsonResponse.ToString();
                }

                LED_ResponseRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
            }
        }



        public class LED_WriteRecord : INotifyPropertyChanged
        {
            public LED_WriteRecord()
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

            private double _Counter;
            public double Counter { get { return _Counter; } set { if (value == _Counter) return; _Counter = value; OnPropertyChanged(); } }
            private double _Junk1;
            public double Junk1 { get { return _Junk1; } set { if (value == _Junk1) return; _Junk1 = value; OnPropertyChanged(); } }
            private double _Len1;
            public double Len1 { get { return _Len1; } set { if (value == _Len1) return; _Len1 = value; OnPropertyChanged(); } }
            private double _Len2;
            public double Len2 { get { return _Len2; } set { if (value == _Len2) return; _Len2 = value; OnPropertyChanged(); } }
            private double _Junk2;
            public double Junk2 { get { return _Junk2; } set { if (value == _Junk2) return; _Junk2 = value; OnPropertyChanged(); } }
            private double _H;
            public double H { get { return _H; } set { if (value == _H) return; _H = value; OnPropertyChanged(); } }
            private double _S;
            public double S { get { return _S; } set { if (value == _S) return; _S = value; OnPropertyChanged(); } }
            private double _V;
            public double V { get { return _V; } set { if (value == _V) return; _V = value; OnPropertyChanged(); } }
            private double _White;
            public double White { get { return _White; } set { if (value == _White) return; _White = value; OnPropertyChanged(); } }
            private string _Junk3;
            public string Junk3 { get { return _Junk3; } set { if (value == _Junk3) return; _Junk3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<LED_WriteRecord> LED_WriteRecordData { get; } = new DataCollection<LED_WriteRecord>();
    private void OnLED_Write_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (LED_WriteRecordData.Count == 0)
            {
                LED_WriteRecordData.AddRecord(new LED_WriteRecord());
            }
            LED_WriteRecordData[LED_WriteRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountLED_Write(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        LED_WriteRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmLED_Write(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        LED_WriteRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyLED_Write(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Counter,Junk1,Len1,Len2,Junk2,H,S,V,White,Junk3,Notes\n");
        foreach (var row in LED_WriteRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Counter},{row.Junk1},{row.Len1},{row.Len2},{row.Junk2},{row.H},{row.S},{row.V},{row.White},{row.Junk3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickLED_Write(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(LED_Write_Counter.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Counter.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Junk1.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Len1.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_Len2.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_Junk2.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_H.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_S.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_V.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Junk3.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteLED_Write(values);

        }

        private async void OnWriteLED_Write(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(LED_Write_Counter.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Counter.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Junk1.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Len1.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_Len2.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_Junk2.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_H.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_S.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_V.Text, System.Globalization.NumberStyles.None),
                new UxTextValue(LED_Write_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(LED_Write_Junk3.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteLED_Write(values);

        }

        private async Task DoWriteLED_Write(List<UxTextValue> values)
        {
            if (values.Count != 7) return; // Change #2; TODO: Correct number here
            int valueIndex = 0; // Change #3;

            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 Counter;
                // History: used to go into LED_Write_Counter.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCounter = Utilities.Parsers.TryParseUInt16(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Counter);
                valueIndex++; // Change #5
                if (!parsedCounter)
                {
                    parseError = "Counter";
                }
                UInt32 Junk1;
                // History: used to go into LED_Write_Junk1.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedJunk1 = Utilities.Parsers.TryParseUInt32(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Junk1);
                valueIndex++; // Change #5
                if (!parsedJunk1)
                {
                    parseError = "Junk1";
                }
                Byte Len1;
                // History: used to go into LED_Write_Len1.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedLen1 = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Len1);
                valueIndex++; // Change #5
                if (!parsedLen1)
                {
                    parseError = "Len1";
                }
                Byte Len2;
                // History: used to go into LED_Write_Len2.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedLen2 = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Len2);
                valueIndex++; // Change #5
                if (!parsedLen2)
                {
                    parseError = "Len2";
                }
                UInt32 Junk2;
                // History: used to go into LED_Write_Junk2.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedJunk2 = Utilities.Parsers.TryParseUInt32(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Junk2);
                valueIndex++; // Change #5
                if (!parsedJunk2)
                {
                    parseError = "Junk2";
                }
                Byte H;
                // History: used to go into LED_Write_H.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedH = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out H);
                valueIndex++; // Change #5
                if (!parsedH)
                {
                    parseError = "H";
                }
                Byte S;
                // History: used to go into LED_Write_S.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedS = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out S);
                valueIndex++; // Change #5
                if (!parsedS)
                {
                    parseError = "S";
                }
                Byte V;
                // History: used to go into LED_Write_V.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedV = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out V);
                valueIndex++; // Change #5
                if (!parsedV)
                {
                    parseError = "V";
                }
                UInt16 White;
                // History: used to go into LED_Write_White.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedWhite = Utilities.Parsers.TryParseUInt16(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out White);
                valueIndex++; // Change #5
                if (!parsedWhite)
                {
                    parseError = "White";
                }
                Bytes Junk3;
                // History: used to go into LED_Write_Junk3.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedJunk3 = Utilities.Parsers.TryParseBytes(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Junk3);
                valueIndex++; // Change #5
                if (!parsedJunk3)
                {
                    parseError = "Junk3";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteLED_Write(Counter, Junk1, Len1, Len2, Junk2, H, S, V, White, Junk3);
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