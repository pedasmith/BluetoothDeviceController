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
using static BluetoothProtocols.Daybetter_LedLight;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the P031_ device
    /// </summary>
    public sealed partial class Daybetter_LedLightPage : Page, HasId, ISetHandleStatus
    {
        public Daybetter_LedLightPage()
        {
            this.InitializeComponent();
            this.DataContext = this;

        }
        private string DeviceName = "Daybetter_LedLight";
        private string DeviceNameUser = "P031_";

        int ncommand = 0;
        Daybetter_LedLight bleDevice = new Daybetter_LedLight();
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




        // Functions for ModbusControl
        public class ModbusSendRecord : INotifyPropertyChanged
        {
            public ModbusSendRecord()
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

            private double _Address;
            public double Address { get { return _Address; } set { if (value == _Address) return; _Address = value; OnPropertyChanged(); } }
            private double _Function;
            public double Function { get { return _Function; } set { if (value == _Function) return; _Function = value; OnPropertyChanged(); } }
            private string _Command;
            public string Command { get { return _Command; } set { if (value == _Command) return; _Command = value; OnPropertyChanged(); } }
            private double _CRC;
            public double CRC { get { return _CRC; } set { if (value == _CRC) return; _CRC = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<ModbusSendRecord> ModbusSendRecordData { get; } = new DataCollection<ModbusSendRecord>();
    private void OnModbusSend_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (ModbusSendRecordData.Count == 0)
            {
                ModbusSendRecordData.AddRecord(new ModbusSendRecord());
            }
            ModbusSendRecordData[ModbusSendRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountModbusSend(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        ModbusSendRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmModbusSend(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        ModbusSendRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyModbusSend(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Address,Function,Command,CRC,Notes\n");
        foreach (var row in ModbusSendRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Address},{row.Function},{row.Command},{row.CRC},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickModbusSend(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(ModbusSend_Address.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_Address.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_Function.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_Command.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_CRC.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteModbusSend(values);

        }

        private async void OnWriteModbusSend(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(ModbusSend_Address.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_Address.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_Function.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_Command.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(ModbusSend_CRC.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteModbusSend(values);

        }

        private async Task DoWriteModbusSend(List<UxTextValue> values)
        {
            if (values.Count != 4) return;
            int valueIndex = 0; // Change #3;

            SetStatusActive (true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Address;
                // History: used to go into ModbusSend_Address.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedAddress = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Address);
                valueIndex++; // Change #5
                if (!parsedAddress)
                {
                    parseError = "Address";
                }
                Byte Function;
                // History: used to go into ModbusSend_Function.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedFunction = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Function);
                valueIndex++; // Change #5
                if (!parsedFunction)
                {
                    parseError = "Function";
                }
                Bytes Command;
                // History: used to go into ModbusSend_Command.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCommand = Utilities.Parsers.TryParseBytes(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Command);
                valueIndex++; // Change #5
                if (!parsedCommand)
                {
                    parseError = "Command";
                }
                UInt16 CRC;
                // History: used to go into ModbusSend_CRC.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCRC = Utilities.Parsers.TryParseUInt16(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out CRC);
                valueIndex++; // Change #5
                if (!parsedCRC)
                {
                    parseError = "CRC";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteModbusSend(Address, Function, Command, CRC);
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


        public class ModbusReplyRecord : INotifyPropertyChanged
        {
            public ModbusReplyRecord()
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

            private double _Address;
            public double Address { get { return _Address; } set { if (value == _Address) return; _Address = value; OnPropertyChanged(); } }
            private double _Function;
            public double Function { get { return _Function; } set { if (value == _Function) return; _Function = value; OnPropertyChanged(); } }
            private string _Result;
            public string Result { get { return _Result; } set { if (value == _Result) return; _Result = value; OnPropertyChanged(); } }
            private double _CRC;
            public double CRC { get { return _CRC; } set { if (value == _CRC) return; _CRC = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<ModbusReplyRecord> ModbusReplyRecordData { get; } = new DataCollection<ModbusReplyRecord>();
    private void OnModbusReply_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (ModbusReplyRecordData.Count == 0)
            {
                ModbusReplyRecordData.AddRecord(new ModbusReplyRecord());
            }
            ModbusReplyRecordData[ModbusReplyRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountModbusReply(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        ModbusReplyRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmModbusReply(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        ModbusReplyRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyModbusReply(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Address,Function,Result,CRC,Notes\n");
        foreach (var row in ModbusReplyRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Address},{row.Function},{row.Result},{row.CRC},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyModbusReplySettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int ModbusReplyNotifyIndex = 0;
        bool ModbusReplyNotifySetup = false;
        private async void OnNotifyModbusReply(object sender, RoutedEventArgs e)
        {
            await DoNotifyModbusReply();
        }

        private async Task DoNotifyModbusReply()
        {
            SetStatusActive (true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!ModbusReplyNotifySetup)
                {
                    ModbusReplyNotifySetup = true;
                    bleDevice.ModbusReplyEvent += BleDevice_ModbusReplyEvent;
                }
                var notifyType = NotifyModbusReplySettings[ModbusReplyNotifyIndex];
                ModbusReplyNotifyIndex = (ModbusReplyNotifyIndex + 1) % NotifyModbusReplySettings.Length;
                var result = await bleDevice.NotifyModbusReplyAsync(notifyType);
                


            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_ModbusReplyEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                var valueList = data.ValueList;
                
                var record = new ModbusReplyRecord();
                var Address = valueList.GetValue("Address");
                if (Address.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Address.CurrentType == BCBasic.BCValue.ValueType.IsString || Address.IsArray)
                {
                    record.Address = (double)Address.AsDouble;
                    ModbusReply_Address.Text = record.Address.ToString("N0");
                }
                var Function = valueList.GetValue("Function");
                if (Function.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Function.CurrentType == BCBasic.BCValue.ValueType.IsString || Function.IsArray)
                {
                    record.Function = (double)Function.AsDouble;
                    ModbusReply_Function.Text = record.Function.ToString("N0");
                }
                var Result = valueList.GetValue("Result");
                if (Result.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Result.CurrentType == BCBasic.BCValue.ValueType.IsString || Result.IsArray)
                {
                    record.Result = (string)Result.AsString;
                    ModbusReply_Result.Text = record.Result.ToString();
                }
                var CRC = valueList.GetValue("CRC");
                if (CRC.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CRC.CurrentType == BCBasic.BCValue.ValueType.IsString || CRC.IsArray)
                {
                    record.CRC = (double)CRC.AsDouble;
                    ModbusReply_CRC.Text = record.CRC.ToString("N0");
                }

                var addResult = ModbusReplyRecordData.AddRecord(record);

                
                // Original update was to make this CHART+COMMAND
                });
            }
        }

        private async void OnReadModbusReply(object sender, RoutedEventArgs e)
        {
            await DoReadModbusReply();
        }

        private async Task DoReadModbusReply()
        {
            SetStatusActive (true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadModbusReply();
                if (valueList == null)
                {
                    SetStatus ($"Error: unable to read ModbusReply");
                    return;
                }
                
                var record = new ModbusReplyRecord();
                var Address = valueList.GetValue("Address");
                if (Address.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Address.CurrentType == BCBasic.BCValue.ValueType.IsString || Address.IsArray)
                {
                    record.Address = (double)Address.AsDouble;
                    ModbusReply_Address.Text = record.Address.ToString("N0");
                }
                var Function = valueList.GetValue("Function");
                if (Function.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Function.CurrentType == BCBasic.BCValue.ValueType.IsString || Function.IsArray)
                {
                    record.Function = (double)Function.AsDouble;
                    ModbusReply_Function.Text = record.Function.ToString("N0");
                }
                var Result = valueList.GetValue("Result");
                if (Result.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Result.CurrentType == BCBasic.BCValue.ValueType.IsString || Result.IsArray)
                {
                    record.Result = (string)Result.AsString;
                    ModbusReply_Result.Text = record.Result.ToString();
                }
                var CRC = valueList.GetValue("CRC");
                if (CRC.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CRC.CurrentType == BCBasic.BCValue.ValueType.IsString || CRC.IsArray)
                {
                    record.CRC = (double)CRC.AsDouble;
                    ModbusReply_CRC.Text = record.CRC.ToString("N0");
                }

                ModbusReplyRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus ($"Error: exception: {ex.Message}");
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