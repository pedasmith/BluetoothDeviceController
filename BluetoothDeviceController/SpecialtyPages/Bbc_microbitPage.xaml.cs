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
    public sealed partial class Bbc_MicroBitPage : Page, HasId, ISetHandleStatus
    {
        public Bbc_MicroBitPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Bbc_MicroBit";
        private string DeviceNameUser = "BBC micro:bit";

        int ncommand = 0;
        Bbc_MicroBit bleDevice = new Bbc_MicroBit();
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


        private async void OnWriteDevice_Name(object sender, RoutedEventArgs e)
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
                var parsedDevice_Name = Utilities.Parsers.TryParseString(Device_Name_Device_Name.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Device_Name);
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


        // Functions for Device Info


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
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSerial_Number();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Serial_Number");
                    return;
                }

                var record = new Serial_NumberRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.param0 = (string)param0.AsString;
                    Serial_Number_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Serial_NumberRecordData.Add(record);

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


        // Functions for EventReadWrite


        public class EventReadARecord : INotifyPropertyChanged
        {
            public EventReadARecord()
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

            private double _EventType1;
            public double EventType1 { get { return _EventType1; } set { if (value == _EventType1) return; _EventType1 = value; OnPropertyChanged(); } }

            private double _EventValue1;
            public double EventValue1 { get { return _EventValue1; } set { if (value == _EventValue1) return; _EventValue1 = value; OnPropertyChanged(); } }

            private double _EventType2;
            public double EventType2 { get { return _EventType2; } set { if (value == _EventType2) return; _EventType2 = value; OnPropertyChanged(); } }

            private double _EventValue2;
            public double EventValue2 { get { return _EventValue2; } set { if (value == _EventValue2) return; _EventValue2 = value; OnPropertyChanged(); } }

            private double _EventType3;
            public double EventType3 { get { return _EventType3; } set { if (value == _EventType3) return; _EventType3 = value; OnPropertyChanged(); } }

            private double _EventValue3;
            public double EventValue3 { get { return _EventValue3; } set { if (value == _EventValue3) return; _EventValue3 = value; OnPropertyChanged(); } }

            private double _EventType4;
            public double EventType4 { get { return _EventType4; } set { if (value == _EventType4) return; _EventType4 = value; OnPropertyChanged(); } }

            private double _EventValue4;
            public double EventValue4 { get { return _EventValue4; } set { if (value == _EventValue4) return; _EventValue4 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<EventReadARecord> EventReadARecordData { get; } = new DataCollection<EventReadARecord>();
        private void OnEventReadA_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (EventReadARecordData.Count == 0)
                {
                    EventReadARecordData.AddRecord(new EventReadARecord());
                }
                EventReadARecordData[EventReadARecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountEventReadA(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EventReadARecordData.MaxLength = value;


        }

        private void OnAlgorithmEventReadA(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EventReadARecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyEventReadA(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,EventType1,EventValue1,EventType2,EventValue2,EventType3,EventValue3,EventType4,EventValue4,Notes\n");
            foreach (var row in EventReadARecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.EventType1},{row.EventValue1},{row.EventType2},{row.EventValue2},{row.EventType3},{row.EventValue3},{row.EventType4},{row.EventValue4},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadEventReadA(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadEventReadA();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read EventReadA");
                    return;
                }

                var record = new EventReadARecord();

                var EventType1 = valueList.GetValue("EventType1");
                if (EventType1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType1 = (double)EventType1.AsDouble;
                    EventReadA_EventType1.Text = record.EventType1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue1 = valueList.GetValue("EventValue1");
                if (EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue1 = (double)EventValue1.AsDouble;
                    EventReadA_EventValue1.Text = record.EventValue1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventType2 = valueList.GetValue("EventType2");
                if (EventType2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType2 = (double)EventType2.AsDouble;
                    EventReadA_EventType2.Text = record.EventType2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue2 = valueList.GetValue("EventValue2");
                if (EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue2 = (double)EventValue2.AsDouble;
                    EventReadA_EventValue2.Text = record.EventValue2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventType3 = valueList.GetValue("EventType3");
                if (EventType3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType3 = (double)EventType3.AsDouble;
                    EventReadA_EventType3.Text = record.EventType3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue3 = valueList.GetValue("EventValue3");
                if (EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue3 = (double)EventValue3.AsDouble;
                    EventReadA_EventValue3.Text = record.EventValue3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventType4 = valueList.GetValue("EventType4");
                if (EventType4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType4 = (double)EventType4.AsDouble;
                    EventReadA_EventType4.Text = record.EventType4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue4 = valueList.GetValue("EventValue4");
                if (EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue4 = (double)EventValue4.AsDouble;
                    EventReadA_EventValue4.Text = record.EventValue4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                EventReadARecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyEventReadASettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int EventReadANotifyIndex = 0;
        bool EventReadANotifySetup = false;
        private async void OnNotifyEventReadA(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!EventReadANotifySetup)
                {
                    EventReadANotifySetup = true;
                    bleDevice.EventReadAEvent += BleDevice_EventReadAEvent;
                }
                var notifyType = NotifyEventReadASettings[EventReadANotifyIndex];
                EventReadANotifyIndex = (EventReadANotifyIndex + 1) % NotifyEventReadASettings.Length;
                var result = await bleDevice.NotifyEventReadAAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_EventReadAEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new EventReadARecord();

                    var EventType1 = valueList.GetValue("EventType1");
                    if (EventType1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType1 = (double)EventType1.AsDouble;
                        EventReadA_EventType1.Text = record.EventType1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue1 = valueList.GetValue("EventValue1");
                    if (EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue1 = (double)EventValue1.AsDouble;
                        EventReadA_EventValue1.Text = record.EventValue1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventType2 = valueList.GetValue("EventType2");
                    if (EventType2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType2 = (double)EventType2.AsDouble;
                        EventReadA_EventType2.Text = record.EventType2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue2 = valueList.GetValue("EventValue2");
                    if (EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue2 = (double)EventValue2.AsDouble;
                        EventReadA_EventValue2.Text = record.EventValue2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventType3 = valueList.GetValue("EventType3");
                    if (EventType3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType3 = (double)EventType3.AsDouble;
                        EventReadA_EventType3.Text = record.EventType3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue3 = valueList.GetValue("EventValue3");
                    if (EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue3 = (double)EventValue3.AsDouble;
                        EventReadA_EventValue3.Text = record.EventValue3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventType4 = valueList.GetValue("EventType4");
                    if (EventType4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType4 = (double)EventType4.AsDouble;
                        EventReadA_EventType4.Text = record.EventType4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue4 = valueList.GetValue("EventValue4");
                    if (EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue4 = (double)EventValue4.AsDouble;
                        EventReadA_EventValue4.Text = record.EventValue4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = EventReadARecordData.AddRecord(record);

                });
            }
        }
        public class EventReadBRecord : INotifyPropertyChanged
        {
            public EventReadBRecord()
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

            private double _EventType1;
            public double EventType1 { get { return _EventType1; } set { if (value == _EventType1) return; _EventType1 = value; OnPropertyChanged(); } }

            private double _EventValue1;
            public double EventValue1 { get { return _EventValue1; } set { if (value == _EventValue1) return; _EventValue1 = value; OnPropertyChanged(); } }

            private double _EventType2;
            public double EventType2 { get { return _EventType2; } set { if (value == _EventType2) return; _EventType2 = value; OnPropertyChanged(); } }

            private double _EventValue2;
            public double EventValue2 { get { return _EventValue2; } set { if (value == _EventValue2) return; _EventValue2 = value; OnPropertyChanged(); } }

            private double _EventType3;
            public double EventType3 { get { return _EventType3; } set { if (value == _EventType3) return; _EventType3 = value; OnPropertyChanged(); } }

            private double _EventValue3;
            public double EventValue3 { get { return _EventValue3; } set { if (value == _EventValue3) return; _EventValue3 = value; OnPropertyChanged(); } }

            private double _EventType4;
            public double EventType4 { get { return _EventType4; } set { if (value == _EventType4) return; _EventType4 = value; OnPropertyChanged(); } }

            private double _EventValue4;
            public double EventValue4 { get { return _EventValue4; } set { if (value == _EventValue4) return; _EventValue4 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<EventReadBRecord> EventReadBRecordData { get; } = new DataCollection<EventReadBRecord>();
        private void OnEventReadB_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (EventReadBRecordData.Count == 0)
                {
                    EventReadBRecordData.AddRecord(new EventReadBRecord());
                }
                EventReadBRecordData[EventReadBRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountEventReadB(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EventReadBRecordData.MaxLength = value;


        }

        private void OnAlgorithmEventReadB(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EventReadBRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyEventReadB(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,EventType1,EventValue1,EventType2,EventValue2,EventType3,EventValue3,EventType4,EventValue4,Notes\n");
            foreach (var row in EventReadBRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.EventType1},{row.EventValue1},{row.EventType2},{row.EventValue2},{row.EventType3},{row.EventValue3},{row.EventType4},{row.EventValue4},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadEventReadB(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadEventReadB();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read EventReadB");
                    return;
                }

                var record = new EventReadBRecord();

                var EventType1 = valueList.GetValue("EventType1");
                if (EventType1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType1 = (double)EventType1.AsDouble;
                    EventReadB_EventType1.Text = record.EventType1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue1 = valueList.GetValue("EventValue1");
                if (EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue1 = (double)EventValue1.AsDouble;
                    EventReadB_EventValue1.Text = record.EventValue1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventType2 = valueList.GetValue("EventType2");
                if (EventType2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType2 = (double)EventType2.AsDouble;
                    EventReadB_EventType2.Text = record.EventType2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue2 = valueList.GetValue("EventValue2");
                if (EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue2 = (double)EventValue2.AsDouble;
                    EventReadB_EventValue2.Text = record.EventValue2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventType3 = valueList.GetValue("EventType3");
                if (EventType3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType3 = (double)EventType3.AsDouble;
                    EventReadB_EventType3.Text = record.EventType3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue3 = valueList.GetValue("EventValue3");
                if (EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue3 = (double)EventValue3.AsDouble;
                    EventReadB_EventValue3.Text = record.EventValue3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventType4 = valueList.GetValue("EventType4");
                if (EventType4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventType4 = (double)EventType4.AsDouble;
                    EventReadB_EventType4.Text = record.EventType4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var EventValue4 = valueList.GetValue("EventValue4");
                if (EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.EventValue4 = (double)EventValue4.AsDouble;
                    EventReadB_EventValue4.Text = record.EventValue4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                EventReadBRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyEventReadBSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int EventReadBNotifyIndex = 0;
        bool EventReadBNotifySetup = false;
        private async void OnNotifyEventReadB(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!EventReadBNotifySetup)
                {
                    EventReadBNotifySetup = true;
                    bleDevice.EventReadBEvent += BleDevice_EventReadBEvent;
                }
                var notifyType = NotifyEventReadBSettings[EventReadBNotifyIndex];
                EventReadBNotifyIndex = (EventReadBNotifyIndex + 1) % NotifyEventReadBSettings.Length;
                var result = await bleDevice.NotifyEventReadBAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_EventReadBEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new EventReadBRecord();

                    var EventType1 = valueList.GetValue("EventType1");
                    if (EventType1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType1 = (double)EventType1.AsDouble;
                        EventReadB_EventType1.Text = record.EventType1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue1 = valueList.GetValue("EventValue1");
                    if (EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue1 = (double)EventValue1.AsDouble;
                        EventReadB_EventValue1.Text = record.EventValue1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventType2 = valueList.GetValue("EventType2");
                    if (EventType2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType2 = (double)EventType2.AsDouble;
                        EventReadB_EventType2.Text = record.EventType2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue2 = valueList.GetValue("EventValue2");
                    if (EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue2 = (double)EventValue2.AsDouble;
                        EventReadB_EventValue2.Text = record.EventValue2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventType3 = valueList.GetValue("EventType3");
                    if (EventType3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType3 = (double)EventType3.AsDouble;
                        EventReadB_EventType3.Text = record.EventType3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue3 = valueList.GetValue("EventValue3");
                    if (EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue3 = (double)EventValue3.AsDouble;
                        EventReadB_EventValue3.Text = record.EventValue3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventType4 = valueList.GetValue("EventType4");
                    if (EventType4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventType4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventType4 = (double)EventType4.AsDouble;
                        EventReadB_EventType4.Text = record.EventType4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var EventValue4 = valueList.GetValue("EventValue4");
                    if (EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || EventValue4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.EventValue4 = (double)EventValue4.AsDouble;
                        EventReadB_EventValue4.Text = record.EventValue4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = EventReadBRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteEventWriteA(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 EventType1;
                var parsedEventType1 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventType1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType1);
                if (!parsedEventType1)
                {
                    parseError = "EventType1";
                }

                UInt16 EventValue1;
                var parsedEventValue1 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventValue1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue1);
                if (!parsedEventValue1)
                {
                    parseError = "EventValue1";
                }

                UInt16 EventType2;
                var parsedEventType2 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventType2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType2);
                if (!parsedEventType2)
                {
                    parseError = "EventType2";
                }

                UInt16 EventValue2;
                var parsedEventValue2 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventValue2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue2);
                if (!parsedEventValue2)
                {
                    parseError = "EventValue2";
                }

                UInt16 EventType3;
                var parsedEventType3 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventType3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType3);
                if (!parsedEventType3)
                {
                    parseError = "EventType3";
                }

                UInt16 EventValue3;
                var parsedEventValue3 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventValue3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue3);
                if (!parsedEventValue3)
                {
                    parseError = "EventValue3";
                }

                UInt16 EventType4;
                var parsedEventType4 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventType4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType4);
                if (!parsedEventType4)
                {
                    parseError = "EventType4";
                }

                UInt16 EventValue4;
                var parsedEventValue4 = Utilities.Parsers.TryParseUInt16(EventWriteA_EventValue4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue4);
                if (!parsedEventValue4)
                {
                    parseError = "EventValue4";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteEventWriteA(EventType1, EventValue1, EventType2, EventValue2, EventType3, EventValue3, EventType4, EventValue4);
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

        private async void OnWriteEventWriteB(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 EventType1;
                var parsedEventType1 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventType1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType1);
                if (!parsedEventType1)
                {
                    parseError = "EventType1";
                }

                UInt16 EventValue1;
                var parsedEventValue1 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventValue1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue1);
                if (!parsedEventValue1)
                {
                    parseError = "EventValue1";
                }

                UInt16 EventType2;
                var parsedEventType2 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventType2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType2);
                if (!parsedEventType2)
                {
                    parseError = "EventType2";
                }

                UInt16 EventValue2;
                var parsedEventValue2 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventValue2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue2);
                if (!parsedEventValue2)
                {
                    parseError = "EventValue2";
                }

                UInt16 EventType3;
                var parsedEventType3 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventType3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType3);
                if (!parsedEventType3)
                {
                    parseError = "EventType3";
                }

                UInt16 EventValue3;
                var parsedEventValue3 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventValue3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue3);
                if (!parsedEventValue3)
                {
                    parseError = "EventValue3";
                }

                UInt16 EventType4;
                var parsedEventType4 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventType4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventType4);
                if (!parsedEventType4)
                {
                    parseError = "EventType4";
                }

                UInt16 EventValue4;
                var parsedEventValue4 = Utilities.Parsers.TryParseUInt16(EventWriteB_EventValue4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out EventValue4);
                if (!parsedEventValue4)
                {
                    parseError = "EventValue4";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteEventWriteB(EventType1, EventValue1, EventType2, EventValue2, EventType3, EventValue3, EventType4, EventValue4);
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


        public class AccelerometerDataRecord : INotifyPropertyChanged
        {
            public AccelerometerDataRecord()
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

        public DataCollection<AccelerometerDataRecord> AccelerometerDataRecordData { get; } = new DataCollection<AccelerometerDataRecord>();
        private void OnAccelerometerData_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (AccelerometerDataRecordData.Count == 0)
                {
                    AccelerometerDataRecordData.AddRecord(new AccelerometerDataRecord());
                }
                AccelerometerDataRecordData[AccelerometerDataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAccelerometerData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AccelerometerDataRecordData.MaxLength = value;

            AccelerometerDataChart.RedrawYTime<AccelerometerDataRecord>(AccelerometerDataRecordData);
        }

        private void OnAlgorithmAccelerometerData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AccelerometerDataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAccelerometerData(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,X,Y,Z,Notes\n");
            foreach (var row in AccelerometerDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.X},{row.Y},{row.Z},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAccelerometerData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometerData();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read AccelerometerData");
                    return;
                }

                var record = new AccelerometerDataRecord();

                var X = valueList.GetValue("X");
                if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.X = (double)X.AsDouble;
                    AccelerometerData_X.Text = record.X.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Y = valueList.GetValue("Y");
                if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Y = (double)Y.AsDouble;
                    AccelerometerData_Y.Text = record.Y.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Z = valueList.GetValue("Z");
                if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Z = (double)Z.AsDouble;
                    AccelerometerData_Z.Text = record.Z.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                AccelerometerDataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyAccelerometerDataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int AccelerometerDataNotifyIndex = 0;
        bool AccelerometerDataNotifySetup = false;
        private async void OnNotifyAccelerometerData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!AccelerometerDataNotifySetup)
                {
                    AccelerometerDataNotifySetup = true;
                    bleDevice.AccelerometerDataEvent += BleDevice_AccelerometerDataEvent;
                }
                var notifyType = NotifyAccelerometerDataSettings[AccelerometerDataNotifyIndex];
                AccelerometerDataNotifyIndex = (AccelerometerDataNotifyIndex + 1) % NotifyAccelerometerDataSettings.Length;
                var result = await bleDevice.NotifyAccelerometerDataAsync(notifyType);



                var EventTimeProperty = typeof(AccelerometerDataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(AccelerometerDataRecord).GetProperty("X"),
typeof(AccelerometerDataRecord).GetProperty("Y"),
typeof(AccelerometerDataRecord).GetProperty("Z"),
                };
                var names = new List<string>()
                {
"X",
"Y",
"Z",
                };
                AccelerometerDataChart.SetDataProperties(properties, EventTimeProperty, names);
                AccelerometerDataChart.SetTitle("AccelerometerData Chart");
                AccelerometerDataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<AccelerometerPeriodRecord>(addResult, AccelerometerPeriodRecordData)",
                    chartDefaultMaxY = 2,
                    chartDefaultMinY = -2,
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_AccelerometerDataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new AccelerometerDataRecord();

                    var X = valueList.GetValue("X");
                    if (X.CurrentType == BCBasic.BCValue.ValueType.IsDouble || X.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.X = (double)X.AsDouble;
                        AccelerometerData_X.Text = record.X.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Y = valueList.GetValue("Y");
                    if (Y.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Y.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Y = (double)Y.AsDouble;
                        AccelerometerData_Y.Text = record.Y.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Z = valueList.GetValue("Z");
                    if (Z.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Z.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Z = (double)Z.AsDouble;
                        AccelerometerData_Z.Text = record.Z.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = AccelerometerDataRecordData.AddRecord(record);
                    AccelerometerDataChart.AddYTime<AccelerometerDataRecord>(addResult, AccelerometerDataRecordData);
                });
            }
        }
        private async void OnWriteAccelerometerPeriod(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 AccelerometerPeriod;
                var parsedAccelerometerPeriod = Utilities.Parsers.TryParseUInt16(AccelerometerPeriod_AccelerometerPeriod.Text, System.Globalization.NumberStyles.None, null, out AccelerometerPeriod);
                if (!parsedAccelerometerPeriod)
                {
                    parseError = "AccelerometerPeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteAccelerometerPeriod(AccelerometerPeriod);
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

        public class AccelerometerPeriodRecord : INotifyPropertyChanged
        {
            public AccelerometerPeriodRecord()
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

        public DataCollection<AccelerometerPeriodRecord> AccelerometerPeriodRecordData { get; } = new DataCollection<AccelerometerPeriodRecord>();
        private void OnAccelerometerPeriod_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (AccelerometerPeriodRecordData.Count == 0)
                {
                    AccelerometerPeriodRecordData.AddRecord(new AccelerometerPeriodRecord());
                }
                AccelerometerPeriodRecordData[AccelerometerPeriodRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAccelerometerPeriod(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AccelerometerPeriodRecordData.MaxLength = value;


        }

        private void OnAlgorithmAccelerometerPeriod(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AccelerometerPeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAccelerometerPeriod(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,AccelerometerPeriod,Notes\n");
            foreach (var row in AccelerometerPeriodRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AccelerometerPeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadAccelerometerPeriod(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadAccelerometerPeriod();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read AccelerometerPeriod");
                    return;
                }

                var record = new AccelerometerPeriodRecord();

                var AccelerometerPeriod = valueList.GetValue("AccelerometerPeriod");
                if (AccelerometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AccelerometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AccelerometerPeriod = (double)AccelerometerPeriod.AsDouble;
                    AccelerometerPeriod_AccelerometerPeriod.Text = record.AccelerometerPeriod.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                AccelerometerPeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Button


        public class ButtonDataARecord : INotifyPropertyChanged
        {
            public ButtonDataARecord()
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

            private double _ButtonA;
            public double ButtonA { get { return _ButtonA; } set { if (value == _ButtonA) return; _ButtonA = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ButtonDataARecord> ButtonDataARecordData { get; } = new DataCollection<ButtonDataARecord>();
        private void OnButtonDataA_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ButtonDataARecordData.Count == 0)
                {
                    ButtonDataARecordData.AddRecord(new ButtonDataARecord());
                }
                ButtonDataARecordData[ButtonDataARecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountButtonDataA(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ButtonDataARecordData.MaxLength = value;


        }

        private void OnAlgorithmButtonDataA(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ButtonDataARecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyButtonDataA(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,ButtonA,Notes\n");
            foreach (var row in ButtonDataARecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ButtonA},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadButtonDataA(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadButtonDataA();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read ButtonDataA");
                    return;
                }

                var record = new ButtonDataARecord();

                var ButtonA = valueList.GetValue("ButtonA");
                if (ButtonA.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ButtonA.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ButtonA = (double)ButtonA.AsDouble;
                    ButtonDataA_ButtonA.Text = record.ButtonA.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                ButtonDataARecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyButtonDataASettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int ButtonDataANotifyIndex = 0;
        bool ButtonDataANotifySetup = false;
        private async void OnNotifyButtonDataA(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!ButtonDataANotifySetup)
                {
                    ButtonDataANotifySetup = true;
                    bleDevice.ButtonDataAEvent += BleDevice_ButtonDataAEvent;
                }
                var notifyType = NotifyButtonDataASettings[ButtonDataANotifyIndex];
                ButtonDataANotifyIndex = (ButtonDataANotifyIndex + 1) % NotifyButtonDataASettings.Length;
                var result = await bleDevice.NotifyButtonDataAAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_ButtonDataAEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new ButtonDataARecord();

                    var ButtonA = valueList.GetValue("ButtonA");
                    if (ButtonA.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ButtonA.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.ButtonA = (double)ButtonA.AsDouble;
                        ButtonDataA_ButtonA.Text = record.ButtonA.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = ButtonDataARecordData.AddRecord(record);

                });
            }
        }
        public class ButtonDataBRecord : INotifyPropertyChanged
        {
            public ButtonDataBRecord()
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

            private double _ButtonB;
            public double ButtonB { get { return _ButtonB; } set { if (value == _ButtonB) return; _ButtonB = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ButtonDataBRecord> ButtonDataBRecordData { get; } = new DataCollection<ButtonDataBRecord>();
        private void OnButtonDataB_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ButtonDataBRecordData.Count == 0)
                {
                    ButtonDataBRecordData.AddRecord(new ButtonDataBRecord());
                }
                ButtonDataBRecordData[ButtonDataBRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountButtonDataB(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ButtonDataBRecordData.MaxLength = value;


        }

        private void OnAlgorithmButtonDataB(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ButtonDataBRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyButtonDataB(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,ButtonB,Notes\n");
            foreach (var row in ButtonDataBRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ButtonB},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadButtonDataB(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadButtonDataB();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read ButtonDataB");
                    return;
                }

                var record = new ButtonDataBRecord();

                var ButtonB = valueList.GetValue("ButtonB");
                if (ButtonB.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ButtonB.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ButtonB = (double)ButtonB.AsDouble;
                    ButtonDataB_ButtonB.Text = record.ButtonB.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                ButtonDataBRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyButtonDataBSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int ButtonDataBNotifyIndex = 0;
        bool ButtonDataBNotifySetup = false;
        private async void OnNotifyButtonDataB(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!ButtonDataBNotifySetup)
                {
                    ButtonDataBNotifySetup = true;
                    bleDevice.ButtonDataBEvent += BleDevice_ButtonDataBEvent;
                }
                var notifyType = NotifyButtonDataBSettings[ButtonDataBNotifyIndex];
                ButtonDataBNotifyIndex = (ButtonDataBNotifyIndex + 1) % NotifyButtonDataBSettings.Length;
                var result = await bleDevice.NotifyButtonDataBAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_ButtonDataBEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new ButtonDataBRecord();

                    var ButtonB = valueList.GetValue("ButtonB");
                    if (ButtonB.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ButtonB.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.ButtonB = (double)ButtonB.AsDouble;
                        ButtonDataB_ButtonB.Text = record.ButtonB.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = ButtonDataBRecordData.AddRecord(record);

                });
            }
        }

        // Functions for IOPin


        private async void OnWritePinAnalog(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt32 SetAnalog;
                var parsedSetAnalog = Utilities.Parsers.TryParseUInt32(PinAnalog_SetAnalog.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SetAnalog);
                if (!parsedSetAnalog)
                {
                    parseError = "SetAnalog";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePinAnalog(SetAnalog);
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

        public class PinAnalogRecord : INotifyPropertyChanged
        {
            public PinAnalogRecord()
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

            private double _SetAnalog;
            public double SetAnalog { get { return _SetAnalog; } set { if (value == _SetAnalog) return; _SetAnalog = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<PinAnalogRecord> PinAnalogRecordData { get; } = new DataCollection<PinAnalogRecord>();
        private void OnPinAnalog_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (PinAnalogRecordData.Count == 0)
                {
                    PinAnalogRecordData.AddRecord(new PinAnalogRecord());
                }
                PinAnalogRecordData[PinAnalogRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPinAnalog(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PinAnalogRecordData.MaxLength = value;


        }

        private void OnAlgorithmPinAnalog(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PinAnalogRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPinAnalog(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,SetAnalog,Notes\n");
            foreach (var row in PinAnalogRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.SetAnalog},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPinAnalog(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPinAnalog();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read PinAnalog");
                    return;
                }

                var record = new PinAnalogRecord();

                var SetAnalog = valueList.GetValue("SetAnalog");
                if (SetAnalog.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SetAnalog.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SetAnalog = (double)SetAnalog.AsDouble;
                    PinAnalog_SetAnalog.Text = record.SetAnalog.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                PinAnalogRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWritePinInput(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt32 SetInput;
                var parsedSetInput = Utilities.Parsers.TryParseUInt32(PinInput_SetInput.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SetInput);
                if (!parsedSetInput)
                {
                    parseError = "SetInput";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePinInput(SetInput);
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

        public class PinInputRecord : INotifyPropertyChanged
        {
            public PinInputRecord()
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

            private double _SetInput;
            public double SetInput { get { return _SetInput; } set { if (value == _SetInput) return; _SetInput = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<PinInputRecord> PinInputRecordData { get; } = new DataCollection<PinInputRecord>();
        private void OnPinInput_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (PinInputRecordData.Count == 0)
                {
                    PinInputRecordData.AddRecord(new PinInputRecord());
                }
                PinInputRecordData[PinInputRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPinInput(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PinInputRecordData.MaxLength = value;


        }

        private void OnAlgorithmPinInput(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PinInputRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPinInput(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,SetInput,Notes\n");
            foreach (var row in PinInputRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.SetInput},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPinInput(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPinInput();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read PinInput");
                    return;
                }

                var record = new PinInputRecord();

                var SetInput = valueList.GetValue("SetInput");
                if (SetInput.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SetInput.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SetInput = (double)SetInput.AsDouble;
                    PinInput_SetInput.Text = record.SetInput.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                PinInputRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWritePinPwm(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte PinNumber1;
                var parsedPinNumber1 = Utilities.Parsers.TryParseByte(PinPwm_PinNumber1.Text, System.Globalization.NumberStyles.None, null, out PinNumber1);
                if (!parsedPinNumber1)
                {
                    parseError = "PinNumber1";
                }

                UInt16 Value1;
                var parsedValue1 = Utilities.Parsers.TryParseUInt16(PinPwm_Value1.Text, System.Globalization.NumberStyles.None, null, out Value1);
                if (!parsedValue1)
                {
                    parseError = "Value1";
                }

                UInt32 Period1;
                var parsedPeriod1 = Utilities.Parsers.TryParseUInt32(PinPwm_Period1.Text, System.Globalization.NumberStyles.None, null, out Period1);
                if (!parsedPeriod1)
                {
                    parseError = "Period1";
                }

                Byte PinNumber2;
                var parsedPinNumber2 = Utilities.Parsers.TryParseByte(PinPwm_PinNumber2.Text, System.Globalization.NumberStyles.None, null, out PinNumber2);
                if (!parsedPinNumber2)
                {
                    parseError = "PinNumber2";
                }

                UInt16 Value2;
                var parsedValue2 = Utilities.Parsers.TryParseUInt16(PinPwm_Value2.Text, System.Globalization.NumberStyles.None, null, out Value2);
                if (!parsedValue2)
                {
                    parseError = "Value2";
                }

                UInt32 Period2;
                var parsedPeriod2 = Utilities.Parsers.TryParseUInt32(PinPwm_Period2.Text, System.Globalization.NumberStyles.None, null, out Period2);
                if (!parsedPeriod2)
                {
                    parseError = "Period2";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePinPwm(PinNumber1, Value1, Period1, PinNumber2, Value2, Period2);
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

        private async void OnWritePinData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte PinNumber1;
                var parsedPinNumber1 = Utilities.Parsers.TryParseByte(PinData_PinNumber1.Text, System.Globalization.NumberStyles.None, null, out PinNumber1);
                if (!parsedPinNumber1)
                {
                    parseError = "PinNumber1";
                }

                Byte DEPinData1;
                var parsedDEPinData1 = Utilities.Parsers.TryParseByte(PinData_DEPinData1.Text, System.Globalization.NumberStyles.None, null, out DEPinData1);
                if (!parsedDEPinData1)
                {
                    parseError = "DEPinData1";
                }

                Byte PinNumber2;
                var parsedPinNumber2 = Utilities.Parsers.TryParseByte(PinData_PinNumber2.Text, System.Globalization.NumberStyles.None, null, out PinNumber2);
                if (!parsedPinNumber2)
                {
                    parseError = "PinNumber2";
                }

                Byte DEPinData2;
                var parsedDEPinData2 = Utilities.Parsers.TryParseByte(PinData_DEPinData2.Text, System.Globalization.NumberStyles.None, null, out DEPinData2);
                if (!parsedDEPinData2)
                {
                    parseError = "DEPinData2";
                }

                Byte PinNumber3;
                var parsedPinNumber3 = Utilities.Parsers.TryParseByte(PinData_PinNumber3.Text, System.Globalization.NumberStyles.None, null, out PinNumber3);
                if (!parsedPinNumber3)
                {
                    parseError = "PinNumber3";
                }

                Byte DEPinData3;
                var parsedDEPinData3 = Utilities.Parsers.TryParseByte(PinData_DEPinData3.Text, System.Globalization.NumberStyles.None, null, out DEPinData3);
                if (!parsedDEPinData3)
                {
                    parseError = "DEPinData3";
                }

                Byte PinNumber4;
                var parsedPinNumber4 = Utilities.Parsers.TryParseByte(PinData_PinNumber4.Text, System.Globalization.NumberStyles.None, null, out PinNumber4);
                if (!parsedPinNumber4)
                {
                    parseError = "PinNumber4";
                }

                Byte DEPinData4;
                var parsedDEPinData4 = Utilities.Parsers.TryParseByte(PinData_DEPinData4.Text, System.Globalization.NumberStyles.None, null, out DEPinData4);
                if (!parsedDEPinData4)
                {
                    parseError = "DEPinData4";
                }

                Byte PinNumber5;
                var parsedPinNumber5 = Utilities.Parsers.TryParseByte(PinData_PinNumber5.Text, System.Globalization.NumberStyles.None, null, out PinNumber5);
                if (!parsedPinNumber5)
                {
                    parseError = "PinNumber5";
                }

                Byte DEPinData5;
                var parsedDEPinData5 = Utilities.Parsers.TryParseByte(PinData_DEPinData5.Text, System.Globalization.NumberStyles.None, null, out DEPinData5);
                if (!parsedDEPinData5)
                {
                    parseError = "DEPinData5";
                }

                Byte PinNumber6;
                var parsedPinNumber6 = Utilities.Parsers.TryParseByte(PinData_PinNumber6.Text, System.Globalization.NumberStyles.None, null, out PinNumber6);
                if (!parsedPinNumber6)
                {
                    parseError = "PinNumber6";
                }

                Byte DEPinData6;
                var parsedDEPinData6 = Utilities.Parsers.TryParseByte(PinData_DEPinData6.Text, System.Globalization.NumberStyles.None, null, out DEPinData6);
                if (!parsedDEPinData6)
                {
                    parseError = "DEPinData6";
                }

                Byte PinNumber7;
                var parsedPinNumber7 = Utilities.Parsers.TryParseByte(PinData_PinNumber7.Text, System.Globalization.NumberStyles.None, null, out PinNumber7);
                if (!parsedPinNumber7)
                {
                    parseError = "PinNumber7";
                }

                Byte DEPinData7;
                var parsedDEPinData7 = Utilities.Parsers.TryParseByte(PinData_DEPinData7.Text, System.Globalization.NumberStyles.None, null, out DEPinData7);
                if (!parsedDEPinData7)
                {
                    parseError = "DEPinData7";
                }

                Byte PinNumber8;
                var parsedPinNumber8 = Utilities.Parsers.TryParseByte(PinData_PinNumber8.Text, System.Globalization.NumberStyles.None, null, out PinNumber8);
                if (!parsedPinNumber8)
                {
                    parseError = "PinNumber8";
                }

                Byte DEPinData8;
                var parsedDEPinData8 = Utilities.Parsers.TryParseByte(PinData_DEPinData8.Text, System.Globalization.NumberStyles.None, null, out DEPinData8);
                if (!parsedDEPinData8)
                {
                    parseError = "DEPinData8";
                }

                Byte PinNumber9;
                var parsedPinNumber9 = Utilities.Parsers.TryParseByte(PinData_PinNumber9.Text, System.Globalization.NumberStyles.None, null, out PinNumber9);
                if (!parsedPinNumber9)
                {
                    parseError = "PinNumber9";
                }

                Byte DEPinData9;
                var parsedDEPinData9 = Utilities.Parsers.TryParseByte(PinData_DEPinData9.Text, System.Globalization.NumberStyles.None, null, out DEPinData9);
                if (!parsedDEPinData9)
                {
                    parseError = "DEPinData9";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePinData(PinNumber1, DEPinData1, PinNumber2, DEPinData2, PinNumber3, DEPinData3, PinNumber4, DEPinData4, PinNumber5, DEPinData5, PinNumber6, DEPinData6, PinNumber7, DEPinData7, PinNumber8, DEPinData8, PinNumber9, DEPinData9);
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

        public class PinDataRecord : INotifyPropertyChanged
        {
            public PinDataRecord()
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

            private double _PinNumber1;
            public double PinNumber1 { get { return _PinNumber1; } set { if (value == _PinNumber1) return; _PinNumber1 = value; OnPropertyChanged(); } }

            private double _DEPinData1;
            public double DEPinData1 { get { return _DEPinData1; } set { if (value == _DEPinData1) return; _DEPinData1 = value; OnPropertyChanged(); } }

            private double _PinNumber2;
            public double PinNumber2 { get { return _PinNumber2; } set { if (value == _PinNumber2) return; _PinNumber2 = value; OnPropertyChanged(); } }

            private double _DEPinData2;
            public double DEPinData2 { get { return _DEPinData2; } set { if (value == _DEPinData2) return; _DEPinData2 = value; OnPropertyChanged(); } }

            private double _PinNumber3;
            public double PinNumber3 { get { return _PinNumber3; } set { if (value == _PinNumber3) return; _PinNumber3 = value; OnPropertyChanged(); } }

            private double _DEPinData3;
            public double DEPinData3 { get { return _DEPinData3; } set { if (value == _DEPinData3) return; _DEPinData3 = value; OnPropertyChanged(); } }

            private double _PinNumber4;
            public double PinNumber4 { get { return _PinNumber4; } set { if (value == _PinNumber4) return; _PinNumber4 = value; OnPropertyChanged(); } }

            private double _DEPinData4;
            public double DEPinData4 { get { return _DEPinData4; } set { if (value == _DEPinData4) return; _DEPinData4 = value; OnPropertyChanged(); } }

            private double _PinNumber5;
            public double PinNumber5 { get { return _PinNumber5; } set { if (value == _PinNumber5) return; _PinNumber5 = value; OnPropertyChanged(); } }

            private double _DEPinData5;
            public double DEPinData5 { get { return _DEPinData5; } set { if (value == _DEPinData5) return; _DEPinData5 = value; OnPropertyChanged(); } }

            private double _PinNumber6;
            public double PinNumber6 { get { return _PinNumber6; } set { if (value == _PinNumber6) return; _PinNumber6 = value; OnPropertyChanged(); } }

            private double _DEPinData6;
            public double DEPinData6 { get { return _DEPinData6; } set { if (value == _DEPinData6) return; _DEPinData6 = value; OnPropertyChanged(); } }

            private double _PinNumber7;
            public double PinNumber7 { get { return _PinNumber7; } set { if (value == _PinNumber7) return; _PinNumber7 = value; OnPropertyChanged(); } }

            private double _DEPinData7;
            public double DEPinData7 { get { return _DEPinData7; } set { if (value == _DEPinData7) return; _DEPinData7 = value; OnPropertyChanged(); } }

            private double _PinNumber8;
            public double PinNumber8 { get { return _PinNumber8; } set { if (value == _PinNumber8) return; _PinNumber8 = value; OnPropertyChanged(); } }

            private double _DEPinData8;
            public double DEPinData8 { get { return _DEPinData8; } set { if (value == _DEPinData8) return; _DEPinData8 = value; OnPropertyChanged(); } }

            private double _PinNumber9;
            public double PinNumber9 { get { return _PinNumber9; } set { if (value == _PinNumber9) return; _PinNumber9 = value; OnPropertyChanged(); } }

            private double _DEPinData9;
            public double DEPinData9 { get { return _DEPinData9; } set { if (value == _DEPinData9) return; _DEPinData9 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<PinDataRecord> PinDataRecordData { get; } = new DataCollection<PinDataRecord>();
        private void OnPinData_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (PinDataRecordData.Count == 0)
                {
                    PinDataRecordData.AddRecord(new PinDataRecord());
                }
                PinDataRecordData[PinDataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPinData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PinDataRecordData.MaxLength = value;

            PinDataChart.RedrawLineYTime<PinDataRecord>(PinDataRecordData);
        }

        private void OnAlgorithmPinData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PinDataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPinData(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,PinNumber1,DEPinData1,PinNumber2,DEPinData2,PinNumber3,DEPinData3,PinNumber4,DEPinData4,PinNumber5,DEPinData5,PinNumber6,DEPinData6,PinNumber7,DEPinData7,PinNumber8,DEPinData8,PinNumber9,DEPinData9,Notes\n");
            foreach (var row in PinDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.PinNumber1},{row.DEPinData1},{row.PinNumber2},{row.DEPinData2},{row.PinNumber3},{row.DEPinData3},{row.PinNumber4},{row.DEPinData4},{row.PinNumber5},{row.DEPinData5},{row.PinNumber6},{row.DEPinData6},{row.PinNumber7},{row.DEPinData7},{row.PinNumber8},{row.DEPinData8},{row.PinNumber9},{row.DEPinData9},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPinData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPinData();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read PinData");
                    return;
                }

                var record = new PinDataRecord();

                var PinNumber1 = valueList.GetValue("PinNumber1");
                if (PinNumber1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber1 = (double)PinNumber1.AsDouble;
                    PinData_PinNumber1.Text = record.PinNumber1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData1 = valueList.GetValue("DEPinData1");
                if (DEPinData1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData1 = (double)DEPinData1.AsDouble;
                    PinData_DEPinData1.Text = record.DEPinData1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber2 = valueList.GetValue("PinNumber2");
                if (PinNumber2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber2 = (double)PinNumber2.AsDouble;
                    PinData_PinNumber2.Text = record.PinNumber2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData2 = valueList.GetValue("DEPinData2");
                if (DEPinData2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData2 = (double)DEPinData2.AsDouble;
                    PinData_DEPinData2.Text = record.DEPinData2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber3 = valueList.GetValue("PinNumber3");
                if (PinNumber3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber3 = (double)PinNumber3.AsDouble;
                    PinData_PinNumber3.Text = record.PinNumber3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData3 = valueList.GetValue("DEPinData3");
                if (DEPinData3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData3 = (double)DEPinData3.AsDouble;
                    PinData_DEPinData3.Text = record.DEPinData3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber4 = valueList.GetValue("PinNumber4");
                if (PinNumber4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber4 = (double)PinNumber4.AsDouble;
                    PinData_PinNumber4.Text = record.PinNumber4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData4 = valueList.GetValue("DEPinData4");
                if (DEPinData4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData4 = (double)DEPinData4.AsDouble;
                    PinData_DEPinData4.Text = record.DEPinData4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber5 = valueList.GetValue("PinNumber5");
                if (PinNumber5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber5 = (double)PinNumber5.AsDouble;
                    PinData_PinNumber5.Text = record.PinNumber5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData5 = valueList.GetValue("DEPinData5");
                if (DEPinData5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData5 = (double)DEPinData5.AsDouble;
                    PinData_DEPinData5.Text = record.DEPinData5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber6 = valueList.GetValue("PinNumber6");
                if (PinNumber6.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber6.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber6 = (double)PinNumber6.AsDouble;
                    PinData_PinNumber6.Text = record.PinNumber6.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData6 = valueList.GetValue("DEPinData6");
                if (DEPinData6.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData6.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData6 = (double)DEPinData6.AsDouble;
                    PinData_DEPinData6.Text = record.DEPinData6.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber7 = valueList.GetValue("PinNumber7");
                if (PinNumber7.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber7.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber7 = (double)PinNumber7.AsDouble;
                    PinData_PinNumber7.Text = record.PinNumber7.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData7 = valueList.GetValue("DEPinData7");
                if (DEPinData7.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData7.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData7 = (double)DEPinData7.AsDouble;
                    PinData_DEPinData7.Text = record.DEPinData7.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber8 = valueList.GetValue("PinNumber8");
                if (PinNumber8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber8.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber8 = (double)PinNumber8.AsDouble;
                    PinData_PinNumber8.Text = record.PinNumber8.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData8 = valueList.GetValue("DEPinData8");
                if (DEPinData8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData8.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData8 = (double)DEPinData8.AsDouble;
                    PinData_DEPinData8.Text = record.DEPinData8.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var PinNumber9 = valueList.GetValue("PinNumber9");
                if (PinNumber9.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber9.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.PinNumber9 = (double)PinNumber9.AsDouble;
                    PinData_PinNumber9.Text = record.PinNumber9.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DEPinData9 = valueList.GetValue("DEPinData9");
                if (DEPinData9.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData9.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DEPinData9 = (double)DEPinData9.AsDouble;
                    PinData_DEPinData9.Text = record.DEPinData9.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                PinDataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyPinDataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int PinDataNotifyIndex = 0;
        bool PinDataNotifySetup = false;
        private async void OnNotifyPinData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!PinDataNotifySetup)
                {
                    PinDataNotifySetup = true;
                    bleDevice.PinDataEvent += BleDevice_PinDataEvent;
                }
                var notifyType = NotifyPinDataSettings[PinDataNotifyIndex];
                PinDataNotifyIndex = (PinDataNotifyIndex + 1) % NotifyPinDataSettings.Length;
                var result = await bleDevice.NotifyPinDataAsync(notifyType);



                var EventTimeProperty = typeof(PinDataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(PinDataRecord).GetProperty("PinNumber1"),
typeof(PinDataRecord).GetProperty("DEPinData1"),
typeof(PinDataRecord).GetProperty("PinNumber2"),
typeof(PinDataRecord).GetProperty("DEPinData2"),
typeof(PinDataRecord).GetProperty("PinNumber3"),
typeof(PinDataRecord).GetProperty("DEPinData3"),
typeof(PinDataRecord).GetProperty("PinNumber4"),
typeof(PinDataRecord).GetProperty("DEPinData4"),
typeof(PinDataRecord).GetProperty("PinNumber5"),
typeof(PinDataRecord).GetProperty("DEPinData5"),
typeof(PinDataRecord).GetProperty("PinNumber6"),
typeof(PinDataRecord).GetProperty("DEPinData6"),
typeof(PinDataRecord).GetProperty("PinNumber7"),
typeof(PinDataRecord).GetProperty("DEPinData7"),
typeof(PinDataRecord).GetProperty("PinNumber8"),
typeof(PinDataRecord).GetProperty("DEPinData8"),
typeof(PinDataRecord).GetProperty("PinNumber9"),
typeof(PinDataRecord).GetProperty("DEPinData9"),
                };
                var names = new List<string>()
                {
"PinNumber1",
"DEPinData1",
"PinNumber2",
"DEPinData2",
"PinNumber3",
"DEPinData3",
"PinNumber4",
"DEPinData4",
"PinNumber5",
"DEPinData5",
"PinNumber6",
"DEPinData6",
"PinNumber7",
"DEPinData7",
"PinNumber8",
"DEPinData8",
"PinNumber9",
"DEPinData9",
                };
                PinDataChart.SetDataProperties(properties, EventTimeProperty, names);
                PinDataChart.SetTitle("PinData Chart");
                PinDataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddLineYTime<PinDataRecord>(addResult, PinDataRecordData)",
                    chartDefaultMaxY = 255,
                    chartDefaultMinY = 0,
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_PinDataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new PinDataRecord();

                    var PinNumber1 = valueList.GetValue("PinNumber1");
                    if (PinNumber1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber1 = (double)PinNumber1.AsDouble;
                        PinData_PinNumber1.Text = record.PinNumber1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData1 = valueList.GetValue("DEPinData1");
                    if (DEPinData1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData1 = (double)DEPinData1.AsDouble;
                        PinData_DEPinData1.Text = record.DEPinData1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber2 = valueList.GetValue("PinNumber2");
                    if (PinNumber2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber2 = (double)PinNumber2.AsDouble;
                        PinData_PinNumber2.Text = record.PinNumber2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData2 = valueList.GetValue("DEPinData2");
                    if (DEPinData2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData2 = (double)DEPinData2.AsDouble;
                        PinData_DEPinData2.Text = record.DEPinData2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber3 = valueList.GetValue("PinNumber3");
                    if (PinNumber3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber3 = (double)PinNumber3.AsDouble;
                        PinData_PinNumber3.Text = record.PinNumber3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData3 = valueList.GetValue("DEPinData3");
                    if (DEPinData3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData3 = (double)DEPinData3.AsDouble;
                        PinData_DEPinData3.Text = record.DEPinData3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber4 = valueList.GetValue("PinNumber4");
                    if (PinNumber4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber4 = (double)PinNumber4.AsDouble;
                        PinData_PinNumber4.Text = record.PinNumber4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData4 = valueList.GetValue("DEPinData4");
                    if (DEPinData4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData4 = (double)DEPinData4.AsDouble;
                        PinData_DEPinData4.Text = record.DEPinData4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber5 = valueList.GetValue("PinNumber5");
                    if (PinNumber5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber5 = (double)PinNumber5.AsDouble;
                        PinData_PinNumber5.Text = record.PinNumber5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData5 = valueList.GetValue("DEPinData5");
                    if (DEPinData5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData5 = (double)DEPinData5.AsDouble;
                        PinData_DEPinData5.Text = record.DEPinData5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber6 = valueList.GetValue("PinNumber6");
                    if (PinNumber6.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber6.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber6 = (double)PinNumber6.AsDouble;
                        PinData_PinNumber6.Text = record.PinNumber6.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData6 = valueList.GetValue("DEPinData6");
                    if (DEPinData6.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData6.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData6 = (double)DEPinData6.AsDouble;
                        PinData_DEPinData6.Text = record.DEPinData6.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber7 = valueList.GetValue("PinNumber7");
                    if (PinNumber7.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber7.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber7 = (double)PinNumber7.AsDouble;
                        PinData_PinNumber7.Text = record.PinNumber7.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData7 = valueList.GetValue("DEPinData7");
                    if (DEPinData7.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData7.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData7 = (double)DEPinData7.AsDouble;
                        PinData_DEPinData7.Text = record.DEPinData7.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber8 = valueList.GetValue("PinNumber8");
                    if (PinNumber8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber8.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber8 = (double)PinNumber8.AsDouble;
                        PinData_PinNumber8.Text = record.PinNumber8.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData8 = valueList.GetValue("DEPinData8");
                    if (DEPinData8.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData8.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData8 = (double)DEPinData8.AsDouble;
                        PinData_DEPinData8.Text = record.DEPinData8.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var PinNumber9 = valueList.GetValue("PinNumber9");
                    if (PinNumber9.CurrentType == BCBasic.BCValue.ValueType.IsDouble || PinNumber9.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.PinNumber9 = (double)PinNumber9.AsDouble;
                        PinData_PinNumber9.Text = record.PinNumber9.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DEPinData9 = valueList.GetValue("DEPinData9");
                    if (DEPinData9.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DEPinData9.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.DEPinData9 = (double)DEPinData9.AsDouble;
                        PinData_DEPinData9.Text = record.DEPinData9.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = PinDataRecordData.AddRecord(record);
                    PinDataChart.AddLineYTime<PinDataRecord>(addResult, PinDataRecordData);
                });
            }
        }

        // Functions for LED


        private async void OnWriteLedPattern(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Row1;
                var parsedRow1 = Utilities.Parsers.TryParseByte(LedPattern_Row1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Row1);
                if (!parsedRow1)
                {
                    parseError = "Row1";
                }

                Byte Row2;
                var parsedRow2 = Utilities.Parsers.TryParseByte(LedPattern_Row2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Row2);
                if (!parsedRow2)
                {
                    parseError = "Row2";
                }

                Byte Row3;
                var parsedRow3 = Utilities.Parsers.TryParseByte(LedPattern_Row3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Row3);
                if (!parsedRow3)
                {
                    parseError = "Row3";
                }

                Byte Row4;
                var parsedRow4 = Utilities.Parsers.TryParseByte(LedPattern_Row4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Row4);
                if (!parsedRow4)
                {
                    parseError = "Row4";
                }

                Byte Row5;
                var parsedRow5 = Utilities.Parsers.TryParseByte(LedPattern_Row5.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Row5);
                if (!parsedRow5)
                {
                    parseError = "Row5";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteLedPattern(Row1, Row2, Row3, Row4, Row5);
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

        public class LedPatternRecord : INotifyPropertyChanged
        {
            public LedPatternRecord()
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

            private double _Row1;
            public double Row1 { get { return _Row1; } set { if (value == _Row1) return; _Row1 = value; OnPropertyChanged(); } }

            private double _Row2;
            public double Row2 { get { return _Row2; } set { if (value == _Row2) return; _Row2 = value; OnPropertyChanged(); } }

            private double _Row3;
            public double Row3 { get { return _Row3; } set { if (value == _Row3) return; _Row3 = value; OnPropertyChanged(); } }

            private double _Row4;
            public double Row4 { get { return _Row4; } set { if (value == _Row4) return; _Row4 = value; OnPropertyChanged(); } }

            private double _Row5;
            public double Row5 { get { return _Row5; } set { if (value == _Row5) return; _Row5 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<LedPatternRecord> LedPatternRecordData { get; } = new DataCollection<LedPatternRecord>();
        private void OnLedPattern_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (LedPatternRecordData.Count == 0)
                {
                    LedPatternRecordData.AddRecord(new LedPatternRecord());
                }
                LedPatternRecordData[LedPatternRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountLedPattern(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            LedPatternRecordData.MaxLength = value;


        }

        private void OnAlgorithmLedPattern(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            LedPatternRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyLedPattern(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Row1,Row2,Row3,Row4,Row5,Notes\n");
            foreach (var row in LedPatternRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Row1},{row.Row2},{row.Row3},{row.Row4},{row.Row5},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadLedPattern(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadLedPattern();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read LedPattern");
                    return;
                }

                var record = new LedPatternRecord();

                var Row1 = valueList.GetValue("Row1");
                if (Row1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Row1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Row1 = (double)Row1.AsDouble;
                    LedPattern_Row1.Text = record.Row1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Row2 = valueList.GetValue("Row2");
                if (Row2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Row2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Row2 = (double)Row2.AsDouble;
                    LedPattern_Row2.Text = record.Row2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Row3 = valueList.GetValue("Row3");
                if (Row3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Row3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Row3 = (double)Row3.AsDouble;
                    LedPattern_Row3.Text = record.Row3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Row4 = valueList.GetValue("Row4");
                if (Row4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Row4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Row4 = (double)Row4.AsDouble;
                    LedPattern_Row4.Text = record.Row4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Row5 = valueList.GetValue("Row5");
                if (Row5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Row5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Row5 = (double)Row5.AsDouble;
                    LedPattern_Row5.Text = record.Row5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                LedPatternRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteLedText(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                String LedText;
                var parsedLedText = Utilities.Parsers.TryParseString(LedText_LedText.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out LedText);
                if (!parsedLedText)
                {
                    parseError = "LedText";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteLedText(LedText);
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

        private async void OnWriteLedScrollTime(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 ScrollTime;
                var parsedScrollTime = Utilities.Parsers.TryParseUInt16(LedScrollTime_ScrollTime.Text, System.Globalization.NumberStyles.None, null, out ScrollTime);
                if (!parsedScrollTime)
                {
                    parseError = "ScrollTime";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteLedScrollTime(ScrollTime);
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

        public class LedScrollTimeRecord : INotifyPropertyChanged
        {
            public LedScrollTimeRecord()
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

            private double _ScrollTime;
            public double ScrollTime { get { return _ScrollTime; } set { if (value == _ScrollTime) return; _ScrollTime = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<LedScrollTimeRecord> LedScrollTimeRecordData { get; } = new DataCollection<LedScrollTimeRecord>();
        private void OnLedScrollTime_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (LedScrollTimeRecordData.Count == 0)
                {
                    LedScrollTimeRecordData.AddRecord(new LedScrollTimeRecord());
                }
                LedScrollTimeRecordData[LedScrollTimeRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountLedScrollTime(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            LedScrollTimeRecordData.MaxLength = value;


        }

        private void OnAlgorithmLedScrollTime(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            LedScrollTimeRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyLedScrollTime(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,ScrollTime,Notes\n");
            foreach (var row in LedScrollTimeRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ScrollTime},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadLedScrollTime(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadLedScrollTime();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read LedScrollTime");
                    return;
                }

                var record = new LedScrollTimeRecord();

                var ScrollTime = valueList.GetValue("ScrollTime");
                if (ScrollTime.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ScrollTime.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ScrollTime = (double)ScrollTime.AsDouble;
                    LedScrollTime_ScrollTime.Text = record.ScrollTime.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                LedScrollTimeRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Magnetometer


        public class MagnetometerDataRecord : INotifyPropertyChanged
        {
            public MagnetometerDataRecord()
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

            private double _MagX;
            public double MagX { get { return _MagX; } set { if (value == _MagX) return; _MagX = value; OnPropertyChanged(); } }

            private double _MagY;
            public double MagY { get { return _MagY; } set { if (value == _MagY) return; _MagY = value; OnPropertyChanged(); } }

            private double _MagZ;
            public double MagZ { get { return _MagZ; } set { if (value == _MagZ) return; _MagZ = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MagnetometerDataRecord> MagnetometerDataRecordData { get; } = new DataCollection<MagnetometerDataRecord>();
        private void OnMagnetometerData_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MagnetometerDataRecordData.Count == 0)
                {
                    MagnetometerDataRecordData.AddRecord(new MagnetometerDataRecord());
                }
                MagnetometerDataRecordData[MagnetometerDataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMagnetometerData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerDataRecordData.MaxLength = value;

            MagnetometerDataChart.RedrawYTime<MagnetometerDataRecord>(MagnetometerDataRecordData);
        }

        private void OnAlgorithmMagnetometerData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerDataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMagnetometerData(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MagX,MagY,MagZ,Notes\n");
            foreach (var row in MagnetometerDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MagX},{row.MagY},{row.MagZ},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMagnetometerData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMagnetometerData();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MagnetometerData");
                    return;
                }

                var record = new MagnetometerDataRecord();

                var MagX = valueList.GetValue("MagX");
                if (MagX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagX.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MagX = (double)MagX.AsDouble;
                    MagnetometerData_MagX.Text = record.MagX.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MagY = valueList.GetValue("MagY");
                if (MagY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagY.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MagY = (double)MagY.AsDouble;
                    MagnetometerData_MagY.Text = record.MagY.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MagZ = valueList.GetValue("MagZ");
                if (MagZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagZ.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MagZ = (double)MagZ.AsDouble;
                    MagnetometerData_MagZ.Text = record.MagZ.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MagnetometerDataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMagnetometerDataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int MagnetometerDataNotifyIndex = 0;
        bool MagnetometerDataNotifySetup = false;
        private async void OnNotifyMagnetometerData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!MagnetometerDataNotifySetup)
                {
                    MagnetometerDataNotifySetup = true;
                    bleDevice.MagnetometerDataEvent += BleDevice_MagnetometerDataEvent;
                }
                var notifyType = NotifyMagnetometerDataSettings[MagnetometerDataNotifyIndex];
                MagnetometerDataNotifyIndex = (MagnetometerDataNotifyIndex + 1) % NotifyMagnetometerDataSettings.Length;
                var result = await bleDevice.NotifyMagnetometerDataAsync(notifyType);



                var EventTimeProperty = typeof(MagnetometerDataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(MagnetometerDataRecord).GetProperty("MagX"),
typeof(MagnetometerDataRecord).GetProperty("MagY"),
typeof(MagnetometerDataRecord).GetProperty("MagZ"),
                };
                var names = new List<string>()
                {
"MagX",
"MagY",
"MagZ",
                };
                MagnetometerDataChart.SetDataProperties(properties, EventTimeProperty, names);
                MagnetometerDataChart.SetTitle("MagnetometerData Chart");
                MagnetometerDataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<MagnetometerCalibrationRecord>(addResult, MagnetometerCalibrationRecordData)",
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

        private async void BleDevice_MagnetometerDataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new MagnetometerDataRecord();

                    var MagX = valueList.GetValue("MagX");
                    if (MagX.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagX.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.MagX = (double)MagX.AsDouble;
                        MagnetometerData_MagX.Text = record.MagX.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var MagY = valueList.GetValue("MagY");
                    if (MagY.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagY.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.MagY = (double)MagY.AsDouble;
                        MagnetometerData_MagY.Text = record.MagY.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var MagZ = valueList.GetValue("MagZ");
                    if (MagZ.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagZ.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.MagZ = (double)MagZ.AsDouble;
                        MagnetometerData_MagZ.Text = record.MagZ.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = MagnetometerDataRecordData.AddRecord(record);
                    MagnetometerDataChart.AddYTime<MagnetometerDataRecord>(addResult, MagnetometerDataRecordData);
                });
            }
        }
        public class MagnetometerBearingRecord : INotifyPropertyChanged
        {
            public MagnetometerBearingRecord()
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

            private double _Bearing;
            public double Bearing { get { return _Bearing; } set { if (value == _Bearing) return; _Bearing = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MagnetometerBearingRecord> MagnetometerBearingRecordData { get; } = new DataCollection<MagnetometerBearingRecord>();
        private void OnMagnetometerBearing_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MagnetometerBearingRecordData.Count == 0)
                {
                    MagnetometerBearingRecordData.AddRecord(new MagnetometerBearingRecord());
                }
                MagnetometerBearingRecordData[MagnetometerBearingRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMagnetometerBearing(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerBearingRecordData.MaxLength = value;

            MagnetometerBearingChart.RedrawYTime<MagnetometerBearingRecord>(MagnetometerBearingRecordData);
        }

        private void OnAlgorithmMagnetometerBearing(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerBearingRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMagnetometerBearing(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Bearing,Notes\n");
            foreach (var row in MagnetometerBearingRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Bearing},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMagnetometerBearing(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMagnetometerBearing();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MagnetometerBearing");
                    return;
                }

                var record = new MagnetometerBearingRecord();

                var Bearing = valueList.GetValue("Bearing");
                if (Bearing.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Bearing.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Bearing = (double)Bearing.AsDouble;
                    MagnetometerBearing_Bearing.Text = record.Bearing.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MagnetometerBearingRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMagnetometerBearingSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int MagnetometerBearingNotifyIndex = 0;
        bool MagnetometerBearingNotifySetup = false;
        private async void OnNotifyMagnetometerBearing(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!MagnetometerBearingNotifySetup)
                {
                    MagnetometerBearingNotifySetup = true;
                    bleDevice.MagnetometerBearingEvent += BleDevice_MagnetometerBearingEvent;
                }
                var notifyType = NotifyMagnetometerBearingSettings[MagnetometerBearingNotifyIndex];
                MagnetometerBearingNotifyIndex = (MagnetometerBearingNotifyIndex + 1) % NotifyMagnetometerBearingSettings.Length;
                var result = await bleDevice.NotifyMagnetometerBearingAsync(notifyType);



                var EventTimeProperty = typeof(MagnetometerBearingRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(MagnetometerBearingRecord).GetProperty("Bearing"),
                };
                var names = new List<string>()
                {
"Bearing",
                };
                MagnetometerBearingChart.SetDataProperties(properties, EventTimeProperty, names);
                MagnetometerBearingChart.SetTitle("MagnetometerBearing Chart");
                MagnetometerBearingChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<MagnetometerCalibrationRecord>(addResult, MagnetometerCalibrationRecordData)",
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

        private async void BleDevice_MagnetometerBearingEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new MagnetometerBearingRecord();

                    var Bearing = valueList.GetValue("Bearing");
                    if (Bearing.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Bearing.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Bearing = (double)Bearing.AsDouble;
                        MagnetometerBearing_Bearing.Text = record.Bearing.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = MagnetometerBearingRecordData.AddRecord(record);
                    MagnetometerBearingChart.AddYTime<MagnetometerBearingRecord>(addResult, MagnetometerBearingRecordData);
                });
            }
        }
        private async void OnWriteMagnetometerPeriod(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 MagnetometerPeriod;
                var parsedMagnetometerPeriod = Utilities.Parsers.TryParseUInt16(MagnetometerPeriod_MagnetometerPeriod.Text, System.Globalization.NumberStyles.None, null, out MagnetometerPeriod);
                if (!parsedMagnetometerPeriod)
                {
                    parseError = "MagnetometerPeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMagnetometerPeriod(MagnetometerPeriod);
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

        public class MagnetometerPeriodRecord : INotifyPropertyChanged
        {
            public MagnetometerPeriodRecord()
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

        public DataCollection<MagnetometerPeriodRecord> MagnetometerPeriodRecordData { get; } = new DataCollection<MagnetometerPeriodRecord>();
        private void OnMagnetometerPeriod_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MagnetometerPeriodRecordData.Count == 0)
                {
                    MagnetometerPeriodRecordData.AddRecord(new MagnetometerPeriodRecord());
                }
                MagnetometerPeriodRecordData[MagnetometerPeriodRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMagnetometerPeriod(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerPeriodRecordData.MaxLength = value;


        }

        private void OnAlgorithmMagnetometerPeriod(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerPeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMagnetometerPeriod(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MagnetometerPeriod,Notes\n");
            foreach (var row in MagnetometerPeriodRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MagnetometerPeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMagnetometerPeriod(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMagnetometerPeriod();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MagnetometerPeriod");
                    return;
                }

                var record = new MagnetometerPeriodRecord();

                var MagnetometerPeriod = valueList.GetValue("MagnetometerPeriod");
                if (MagnetometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerPeriod.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MagnetometerPeriod = (double)MagnetometerPeriod.AsDouble;
                    MagnetometerPeriod_MagnetometerPeriod.Text = record.MagnetometerPeriod.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MagnetometerPeriodRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteMagnetometerCalibration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte MagnetometerCalibration;
                var parsedMagnetometerCalibration = Utilities.Parsers.TryParseByte(MagnetometerCalibration_MagnetometerCalibration.Text, System.Globalization.NumberStyles.None, null, out MagnetometerCalibration);
                if (!parsedMagnetometerCalibration)
                {
                    parseError = "MagnetometerCalibration";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMagnetometerCalibration(MagnetometerCalibration);
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

        public class MagnetometerCalibrationRecord : INotifyPropertyChanged
        {
            public MagnetometerCalibrationRecord()
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

            private double _MagnetometerCalibration;
            public double MagnetometerCalibration { get { return _MagnetometerCalibration; } set { if (value == _MagnetometerCalibration) return; _MagnetometerCalibration = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MagnetometerCalibrationRecord> MagnetometerCalibrationRecordData { get; } = new DataCollection<MagnetometerCalibrationRecord>();
        private void OnMagnetometerCalibration_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MagnetometerCalibrationRecordData.Count == 0)
                {
                    MagnetometerCalibrationRecordData.AddRecord(new MagnetometerCalibrationRecord());
                }
                MagnetometerCalibrationRecordData[MagnetometerCalibrationRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMagnetometerCalibration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerCalibrationRecordData.MaxLength = value;


        }

        private void OnAlgorithmMagnetometerCalibration(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MagnetometerCalibrationRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMagnetometerCalibration(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MagnetometerCalibration,Notes\n");
            foreach (var row in MagnetometerCalibrationRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MagnetometerCalibration},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMagnetometerCalibration(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMagnetometerCalibration();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MagnetometerCalibration");
                    return;
                }

                var record = new MagnetometerCalibrationRecord();

                var MagnetometerCalibration = valueList.GetValue("MagnetometerCalibration");
                if (MagnetometerCalibration.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MagnetometerCalibration.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MagnetometerCalibration = (double)MagnetometerCalibration.AsDouble;
                    MagnetometerCalibration_MagnetometerCalibration.Text = record.MagnetometerCalibration.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MagnetometerCalibrationRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Temperature


        public class TemperatureDataRecord : INotifyPropertyChanged
        {
            public TemperatureDataRecord()
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

        public DataCollection<TemperatureDataRecord> TemperatureDataRecordData { get; } = new DataCollection<TemperatureDataRecord>();
        private void OnTemperatureData_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (TemperatureDataRecordData.Count == 0)
                {
                    TemperatureDataRecordData.AddRecord(new TemperatureDataRecord());
                }
                TemperatureDataRecordData[TemperatureDataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTemperatureData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TemperatureDataRecordData.MaxLength = value;

            TemperatureDataChart.RedrawYTime<TemperatureDataRecord>(TemperatureDataRecordData);
        }

        private void OnAlgorithmTemperatureData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TemperatureDataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTemperatureData(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Temperature,Notes\n");
            foreach (var row in TemperatureDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Temperature},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTemperatureData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTemperatureData();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read TemperatureData");
                    return;
                }

                var record = new TemperatureDataRecord();

                var Temperature = valueList.GetValue("Temperature");
                if (Temperature.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Temperature.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Temperature = (double)Temperature.AsDouble;
                    TemperatureData_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                TemperatureDataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyTemperatureDataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int TemperatureDataNotifyIndex = 0;
        bool TemperatureDataNotifySetup = false;
        private async void OnNotifyTemperatureData(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!TemperatureDataNotifySetup)
                {
                    TemperatureDataNotifySetup = true;
                    bleDevice.TemperatureDataEvent += BleDevice_TemperatureDataEvent;
                }
                var notifyType = NotifyTemperatureDataSettings[TemperatureDataNotifyIndex];
                TemperatureDataNotifyIndex = (TemperatureDataNotifyIndex + 1) % NotifyTemperatureDataSettings.Length;
                var result = await bleDevice.NotifyTemperatureDataAsync(notifyType);



                var EventTimeProperty = typeof(TemperatureDataRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(TemperatureDataRecord).GetProperty("Temperature"),
                };
                var names = new List<string>()
                {
"Temperature",
                };
                TemperatureDataChart.SetDataProperties(properties, EventTimeProperty, names);
                TemperatureDataChart.SetTitle("TemperatureData Chart");
                TemperatureDataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<TemperaturePeriodRecord>(addResult, TemperaturePeriodRecordData)",
                    chartDefaultMaxY = 30,
                    chartDefaultMinY = 20,
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_TemperatureDataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new TemperatureDataRecord();

                    var Temperature = valueList.GetValue("Temperature");
                    if (Temperature.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Temperature.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Temperature = (double)Temperature.AsDouble;
                        TemperatureData_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = TemperatureDataRecordData.AddRecord(record);
                    TemperatureDataChart.AddYTime<TemperatureDataRecord>(addResult, TemperatureDataRecordData);
                });
            }
        }
        private async void OnWriteTemperaturePeriod(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                UInt16 TemperaturePeriod;
                var parsedTemperaturePeriod = Utilities.Parsers.TryParseUInt16(TemperaturePeriod_TemperaturePeriod.Text, System.Globalization.NumberStyles.None, null, out TemperaturePeriod);
                if (!parsedTemperaturePeriod)
                {
                    parseError = "TemperaturePeriod";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteTemperaturePeriod(TemperaturePeriod);
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

        public class TemperaturePeriodRecord : INotifyPropertyChanged
        {
            public TemperaturePeriodRecord()
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

            private double _TemperaturePeriod;
            public double TemperaturePeriod { get { return _TemperaturePeriod; } set { if (value == _TemperaturePeriod) return; _TemperaturePeriod = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<TemperaturePeriodRecord> TemperaturePeriodRecordData { get; } = new DataCollection<TemperaturePeriodRecord>();
        private void OnTemperaturePeriod_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (TemperaturePeriodRecordData.Count == 0)
                {
                    TemperaturePeriodRecordData.AddRecord(new TemperaturePeriodRecord());
                }
                TemperaturePeriodRecordData[TemperaturePeriodRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTemperaturePeriod(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TemperaturePeriodRecordData.MaxLength = value;


        }

        private void OnAlgorithmTemperaturePeriod(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TemperaturePeriodRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTemperaturePeriod(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,TemperaturePeriod,Notes\n");
            foreach (var row in TemperaturePeriodRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.TemperaturePeriod},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTemperaturePeriod(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTemperaturePeriod();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read TemperaturePeriod");
                    return;
                }

                var record = new TemperaturePeriodRecord();

                var TemperaturePeriod = valueList.GetValue("TemperaturePeriod");
                if (TemperaturePeriod.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TemperaturePeriod.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TemperaturePeriod = (double)TemperaturePeriod.AsDouble;
                    TemperaturePeriod_TemperaturePeriod.Text = record.TemperaturePeriod.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                TemperaturePeriodRecordData.Add(record);

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
