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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[NAME]] device
    /// </summary>
    public sealed partial class WilliamWeilerEngineering_SkoobotPage : Page, HasId, ISetHandleStatus
    {
        public WilliamWeilerEngineering_SkoobotPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "WilliamWeilerEngineering_Skoobot";
        private string DeviceNameUser = "Skoobot";

        int ncommand = 0;
        WilliamWeilerEngineering_Skoobot bleDevice = new WilliamWeilerEngineering_Skoobot();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive(false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            await DoReadDevice_Name();
            await DoNotifyDistance();
            await DoNotifyAmbientLight();

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


        // OK to include this method even if there are no defined buttons
        private async void OnClickDevice_Name(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteDevice_Name(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteDevice_Name(object sender, RoutedEventArgs e)
        {
            var text = Device_Name_Device_Name.Text;
            await DoWriteDevice_Name(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteDevice_Name(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                String Device_Name;
                // History: used to go into Device_Name_Device_Name.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDevice_Name = Utilities.Parsers.TryParseString(text, dec_or_hex, null, out Device_Name);
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


        // Functions for Robot


        // OK to include this method even if there are no defined buttons
        private async void OnClickCommand(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteCommand(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteCommand(object sender, RoutedEventArgs e)
        {
            var text = Command_Command.Text;
            await DoWriteCommand(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteCommand(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Command;
                // History: used to go into Command_Command.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCommand = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Command);
                if (!parsedCommand)
                {
                    parseError = "Command";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteCommand(Command);
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

            private double _Command;
            public double Command { get { return _Command; } set { if (value == _Command) return; _Command = value; OnPropertyChanged(); } }

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
            sb.Append("EventDate,EventTime,Command,Notes\n");
            foreach (var row in CommandRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Command},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
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
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadCommand();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Command");
                    return;
                }

                var record = new CommandRecord();

                var Command = valueList.GetValue("Command");
                if (Command.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Command.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Command = (double)Command.AsDouble;
                    Command_Command.Text = record.Command.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                CommandRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class DistanceRecord : INotifyPropertyChanged
        {
            public DistanceRecord()
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

            private double _Distance;
            public double Distance { get { return _Distance; } set { if (value == _Distance) return; _Distance = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<DistanceRecord> DistanceRecordData { get; } = new DataCollection<DistanceRecord>();
        private void OnDistance_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (DistanceRecordData.Count == 0)
                {
                    DistanceRecordData.AddRecord(new DistanceRecord());
                }
                DistanceRecordData[DistanceRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDistance(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DistanceRecordData.MaxLength = value;


        }

        private void OnAlgorithmDistance(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DistanceRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDistance(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Distance,Notes\n");
            foreach (var row in DistanceRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Distance},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadDistance(object sender, RoutedEventArgs e)
        {
            await DoReadDistance();
        }

        private async Task DoReadDistance()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDistance();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Distance");
                    return;
                }

                var record = new DistanceRecord();

                var Distance = valueList.GetValue("Distance");
                if (Distance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Distance.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Distance = (double)Distance.AsDouble;
                    Distance_Distance.Text = record.Distance.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                DistanceRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyDistanceSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int DistanceNotifyIndex = 0;
        bool DistanceNotifySetup = false;
        private async void OnNotifyDistance(object sender, RoutedEventArgs e)
        {
            await DoNotifyDistance();
        }

        private async Task DoNotifyDistance()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!DistanceNotifySetup)
                {
                    DistanceNotifySetup = true;
                    bleDevice.DistanceEvent += BleDevice_DistanceEvent;
                }
                var notifyType = NotifyDistanceSettings[DistanceNotifyIndex];
                DistanceNotifyIndex = (DistanceNotifyIndex + 1) % NotifyDistanceSettings.Length;
                var result = await bleDevice.NotifyDistanceAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_DistanceEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new DistanceRecord();

                    var Distance = valueList.GetValue("Distance");
                    if (Distance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Distance.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Distance = (double)Distance.AsDouble;
                        Distance_Distance.Text = record.Distance.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = DistanceRecordData.AddRecord(record);

                });
            }
        }
        public class AmbientLightRecord : INotifyPropertyChanged
        {
            public AmbientLightRecord()
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

            private double _AmbientLight;
            public double AmbientLight { get { return _AmbientLight; } set { if (value == _AmbientLight) return; _AmbientLight = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<AmbientLightRecord> AmbientLightRecordData { get; } = new DataCollection<AmbientLightRecord>();
        private void OnAmbientLight_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (AmbientLightRecordData.Count == 0)
                {
                    AmbientLightRecordData.AddRecord(new AmbientLightRecord());
                }
                AmbientLightRecordData[AmbientLightRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAmbientLight(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AmbientLightRecordData.MaxLength = value;


        }

        private void OnAlgorithmAmbientLight(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AmbientLightRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAmbientLight(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,AmbientLight,Notes\n");
            foreach (var row in AmbientLightRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AmbientLight},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAmbientLight(object sender, RoutedEventArgs e)
        {
            await DoReadAmbientLight();
        }

        private async Task DoReadAmbientLight()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAmbientLight();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read AmbientLight");
                    return;
                }

                var record = new AmbientLightRecord();

                var AmbientLight = valueList.GetValue("AmbientLight");
                if (AmbientLight.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AmbientLight.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AmbientLight = (double)AmbientLight.AsDouble;
                    AmbientLight_AmbientLight.Text = record.AmbientLight.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                AmbientLightRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyAmbientLightSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int AmbientLightNotifyIndex = 0;
        bool AmbientLightNotifySetup = false;
        private async void OnNotifyAmbientLight(object sender, RoutedEventArgs e)
        {
            await DoNotifyAmbientLight();
        }

        private async Task DoNotifyAmbientLight()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!AmbientLightNotifySetup)
                {
                    AmbientLightNotifySetup = true;
                    bleDevice.AmbientLightEvent += BleDevice_AmbientLightEvent;
                }
                var notifyType = NotifyAmbientLightSettings[AmbientLightNotifyIndex];
                AmbientLightNotifyIndex = (AmbientLightNotifyIndex + 1) % NotifyAmbientLightSettings.Length;
                var result = await bleDevice.NotifyAmbientLightAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_AmbientLightEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new AmbientLightRecord();

                    var AmbientLight = valueList.GetValue("AmbientLight");
                    if (AmbientLight.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AmbientLight.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.AmbientLight = (double)AmbientLight.AsDouble;
                        AmbientLight_AmbientLight.Text = record.AmbientLight.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = AmbientLightRecordData.AddRecord(record);

                });
            }
        }
        // OK to include this method even if there are no defined buttons
        private async void OnClickMicrophone(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteMicrophone(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteMicrophone(object sender, RoutedEventArgs e)
        {
            var text = Microphone_Audio.Text;
            await DoWriteMicrophone(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteMicrophone(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Audio;
                // History: used to go into Microphone_Audio.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedAudio = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out Audio);
                if (!parsedAudio)
                {
                    parseError = "Audio";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMicrophone(Audio);
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

        public class MicrophoneRecord : INotifyPropertyChanged
        {
            public MicrophoneRecord()
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

            private string _Audio;
            public string Audio { get { return _Audio; } set { if (value == _Audio) return; _Audio = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MicrophoneRecord> MicrophoneRecordData { get; } = new DataCollection<MicrophoneRecord>();
        private void OnMicrophone_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MicrophoneRecordData.Count == 0)
                {
                    MicrophoneRecordData.AddRecord(new MicrophoneRecord());
                }
                MicrophoneRecordData[MicrophoneRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMicrophone(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MicrophoneRecordData.MaxLength = value;


        }

        private void OnAlgorithmMicrophone(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MicrophoneRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMicrophone(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Audio,Notes\n");
            foreach (var row in MicrophoneRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Audio},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMicrophone(object sender, RoutedEventArgs e)
        {
            await DoReadMicrophone();
        }

        private async Task DoReadMicrophone()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMicrophone();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Microphone");
                    return;
                }

                var record = new MicrophoneRecord();

                var Audio = valueList.GetValue("Audio");
                if (Audio.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Audio.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Audio = (string)Audio.AsString;
                    Microphone_Audio.Text = record.Audio.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MicrophoneRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMicrophoneSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int MicrophoneNotifyIndex = 0;
        bool MicrophoneNotifySetup = false;
        private async void OnNotifyMicrophone(object sender, RoutedEventArgs e)
        {
            await DoNotifyMicrophone();
        }

        private async Task DoNotifyMicrophone()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!MicrophoneNotifySetup)
                {
                    MicrophoneNotifySetup = true;
                    bleDevice.MicrophoneEvent += BleDevice_MicrophoneEvent;
                }
                var notifyType = NotifyMicrophoneSettings[MicrophoneNotifyIndex];
                MicrophoneNotifyIndex = (MicrophoneNotifyIndex + 1) % NotifyMicrophoneSettings.Length;
                var result = await bleDevice.NotifyMicrophoneAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_MicrophoneEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new MicrophoneRecord();

                    var Audio = valueList.GetValue("Audio");
                    if (Audio.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Audio.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Audio = (string)Audio.AsString;
                        Microphone_Audio.Text = record.Audio.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = MicrophoneRecordData.AddRecord(record);

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
