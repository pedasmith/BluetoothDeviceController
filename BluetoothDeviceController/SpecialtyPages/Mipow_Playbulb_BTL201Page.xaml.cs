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
    public sealed partial class MIPOW_Playbulb_BTL201Page : Page, HasId, ISetHandleStatus
    {
        public MIPOW_Playbulb_BTL201Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "MIPOW_Playbulb_BTL201";
        private string DeviceNameUser = "PLAYBULB smart bulb";

        int ncommand = 0;
        MIPOW_Playbulb_BTL201 bleDevice = new MIPOW_Playbulb_BTL201();
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

        // Functions for Generic Service



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


        // Functions for DeviceInformationService


        private async void OnWriteApplicationNumber(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes ApplicationNumber;
                var parsedApplicationNumber = Utilities.Parsers.TryParseBytes(ApplicationNumber_ApplicationNumber.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ApplicationNumber);
                if (!parsedApplicationNumber)
                {
                    parseError = "ApplicationNumber";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteApplicationNumber(ApplicationNumber);
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

        public class ApplicationNumberRecord : INotifyPropertyChanged
        {
            public ApplicationNumberRecord()
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

            private string _ApplicationNumber;
            public string ApplicationNumber { get { return _ApplicationNumber; } set { if (value == _ApplicationNumber) return; _ApplicationNumber = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ApplicationNumberRecord> ApplicationNumberRecordData { get; } = new DataCollection<ApplicationNumberRecord>();
        private void OnApplicationNumber_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ApplicationNumberRecordData.Count == 0)
                {
                    ApplicationNumberRecordData.AddRecord(new ApplicationNumberRecord());
                }
                ApplicationNumberRecordData[ApplicationNumberRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountApplicationNumber(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ApplicationNumberRecordData.MaxLength = value;


        }

        private void OnAlgorithmApplicationNumber(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ApplicationNumberRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyApplicationNumber(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,ApplicationNumber,Notes\n");
            foreach (var row in ApplicationNumberRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ApplicationNumber},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadApplicationNumber(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadApplicationNumber();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read ApplicationNumber");
                    return;
                }

                var record = new ApplicationNumberRecord();

                var ApplicationNumber = valueList.GetValue("ApplicationNumber");
                if (ApplicationNumber.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ApplicationNumber.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ApplicationNumber = (string)ApplicationNumber.AsString;
                    ApplicationNumber_ApplicationNumber.Text = record.ApplicationNumber.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                ApplicationNumberRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteGetKeyBlock(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes KeyBlock;
                var parsedKeyBlock = Utilities.Parsers.TryParseBytes(GetKeyBlock_KeyBlock.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out KeyBlock);
                if (!parsedKeyBlock)
                {
                    parseError = "KeyBlock";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteGetKeyBlock(KeyBlock);
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

        public class XferCharacteristicsRecord : INotifyPropertyChanged
        {
            public XferCharacteristicsRecord()
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

            private string _XferCharacteristics;
            public string XferCharacteristics { get { return _XferCharacteristics; } set { if (value == _XferCharacteristics) return; _XferCharacteristics = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<XferCharacteristicsRecord> XferCharacteristicsRecordData { get; } = new DataCollection<XferCharacteristicsRecord>();
        private void OnXferCharacteristics_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (XferCharacteristicsRecordData.Count == 0)
                {
                    XferCharacteristicsRecordData.AddRecord(new XferCharacteristicsRecord());
                }
                XferCharacteristicsRecordData[XferCharacteristicsRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountXferCharacteristics(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            XferCharacteristicsRecordData.MaxLength = value;


        }

        private void OnAlgorithmXferCharacteristics(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            XferCharacteristicsRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyXferCharacteristics(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,XferCharacteristics,Notes\n");
            foreach (var row in XferCharacteristicsRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.XferCharacteristics},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadXferCharacteristics(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadXferCharacteristics();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read XferCharacteristics");
                    return;
                }

                var record = new XferCharacteristicsRecord();

                var XferCharacteristics = valueList.GetValue("XferCharacteristics");
                if (XferCharacteristics.CurrentType == BCBasic.BCValue.ValueType.IsDouble || XferCharacteristics.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.XferCharacteristics = (string)XferCharacteristics.AsString;
                    XferCharacteristics_XferCharacteristics.Text = record.XferCharacteristics.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                XferCharacteristicsRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyXferCharacteristicsSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int XferCharacteristicsNotifyIndex = 0;
        bool XferCharacteristicsNotifySetup = false;
        private async void OnNotifyXferCharacteristics(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!XferCharacteristicsNotifySetup)
                {
                    XferCharacteristicsNotifySetup = true;
                    bleDevice.XferCharacteristicsEvent += BleDevice_XferCharacteristicsEvent;
                }
                var notifyType = NotifyXferCharacteristicsSettings[XferCharacteristicsNotifyIndex];
                XferCharacteristicsNotifyIndex = (XferCharacteristicsNotifyIndex + 1) % NotifyXferCharacteristicsSettings.Length;
                var result = await bleDevice.NotifyXferCharacteristicsAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_XferCharacteristicsEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new XferCharacteristicsRecord();

                    var XferCharacteristics = valueList.GetValue("XferCharacteristics");
                    if (XferCharacteristics.CurrentType == BCBasic.BCValue.ValueType.IsDouble || XferCharacteristics.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.XferCharacteristics = (string)XferCharacteristics.AsString;
                        XferCharacteristics_XferCharacteristics.Text = record.XferCharacteristics.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = XferCharacteristicsRecordData.AddRecord(record);

                });
            }
        }
        public class GetVersionRecord : INotifyPropertyChanged
        {
            public GetVersionRecord()
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

            private string _Version;
            public string Version { get { return _Version; } set { if (value == _Version) return; _Version = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<GetVersionRecord> GetVersionRecordData { get; } = new DataCollection<GetVersionRecord>();
        private void OnGetVersion_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (GetVersionRecordData.Count == 0)
                {
                    GetVersionRecordData.AddRecord(new GetVersionRecord());
                }
                GetVersionRecordData[GetVersionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountGetVersion(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            GetVersionRecordData.MaxLength = value;


        }

        private void OnAlgorithmGetVersion(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            GetVersionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyGetVersion(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Version,Notes\n");
            foreach (var row in GetVersionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Version},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadGetVersion(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadGetVersion();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read GetVersion");
                    return;
                }

                var record = new GetVersionRecord();

                var Version = valueList.GetValue("Version");
                if (Version.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Version.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Version = (string)Version.AsString;
                    GetVersion_Version.Text = record.Version.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                GetVersionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for MipowBulb


        public class BulbHeartRateRecord : INotifyPropertyChanged
        {
            public BulbHeartRateRecord()
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

            private string _BulbHeartRate;
            public string BulbHeartRate { get { return _BulbHeartRate; } set { if (value == _BulbHeartRate) return; _BulbHeartRate = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<BulbHeartRateRecord> BulbHeartRateRecordData { get; } = new DataCollection<BulbHeartRateRecord>();
        private void OnBulbHeartRate_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (BulbHeartRateRecordData.Count == 0)
                {
                    BulbHeartRateRecordData.AddRecord(new BulbHeartRateRecord());
                }
                BulbHeartRateRecordData[BulbHeartRateRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountBulbHeartRate(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            BulbHeartRateRecordData.MaxLength = value;


        }

        private void OnAlgorithmBulbHeartRate(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            BulbHeartRateRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyBulbHeartRate(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,BulbHeartRate,Notes\n");
            foreach (var row in BulbHeartRateRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.BulbHeartRate},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyBulbHeartRateSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int BulbHeartRateNotifyIndex = 0;
        bool BulbHeartRateNotifySetup = false;
        private async void OnNotifyBulbHeartRate(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!BulbHeartRateNotifySetup)
                {
                    BulbHeartRateNotifySetup = true;
                    bleDevice.BulbHeartRateEvent += BleDevice_BulbHeartRateEvent;
                }
                var notifyType = NotifyBulbHeartRateSettings[BulbHeartRateNotifyIndex];
                BulbHeartRateNotifyIndex = (BulbHeartRateNotifyIndex + 1) % NotifyBulbHeartRateSettings.Length;
                var result = await bleDevice.NotifyBulbHeartRateAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_BulbHeartRateEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new BulbHeartRateRecord();

                    var BulbHeartRate = valueList.GetValue("BulbHeartRate");
                    if (BulbHeartRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BulbHeartRate.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.BulbHeartRate = (string)BulbHeartRate.AsString;
                        BulbHeartRate_BulbHeartRate.Text = record.BulbHeartRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = BulbHeartRateRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteUnknown1(object sender, RoutedEventArgs e)
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
                var parsedUnknown1 = Utilities.Parsers.TryParseBytes(Unknown1_Unknown1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown1);
                if (!parsedUnknown1)
                {
                    parseError = "Unknown1";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUnknown1(Unknown1);
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

        private async void OnWritePINPassword(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                String Password;
                var parsedPassword = Utilities.Parsers.TryParseString(PINPassword_Password.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Password);
                if (!parsedPassword)
                {
                    parseError = "Password";
                }

                if (parseError == null)
                {
                    await bleDevice.WritePINPassword(Password);
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

        public class PINPasswordRecord : INotifyPropertyChanged
        {
            public PINPasswordRecord()
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

            private string _Password;
            public string Password { get { return _Password; } set { if (value == _Password) return; _Password = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<PINPasswordRecord> PINPasswordRecordData { get; } = new DataCollection<PINPasswordRecord>();
        private void OnPINPassword_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (PINPasswordRecordData.Count == 0)
                {
                    PINPasswordRecordData.AddRecord(new PINPasswordRecord());
                }
                PINPasswordRecordData[PINPasswordRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountPINPassword(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PINPasswordRecordData.MaxLength = value;


        }

        private void OnAlgorithmPINPassword(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            PINPasswordRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyPINPassword(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Password,Notes\n");
            foreach (var row in PINPasswordRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Password},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadPINPassword(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadPINPassword();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read PINPassword");
                    return;
                }

                var record = new PINPasswordRecord();

                var Password = valueList.GetValue("Password");
                if (Password.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Password.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Password = (string)Password.AsString;
                    PINPassword_Password.Text = record.Password.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                PINPasswordRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class TimerEffectsRecord : INotifyPropertyChanged
        {
            public TimerEffectsRecord()
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

            private double _W1;
            public double W1 { get { return _W1; } set { if (value == _W1) return; _W1 = value; OnPropertyChanged(); } }

            private double _R1;
            public double R1 { get { return _R1; } set { if (value == _R1) return; _R1 = value; OnPropertyChanged(); } }

            private double _G1;
            public double G1 { get { return _G1; } set { if (value == _G1) return; _G1 = value; OnPropertyChanged(); } }

            private double _B1;
            public double B1 { get { return _B1; } set { if (value == _B1) return; _B1 = value; OnPropertyChanged(); } }

            private double _Time1;
            public double Time1 { get { return _Time1; } set { if (value == _Time1) return; _Time1 = value; OnPropertyChanged(); } }

            private double _W2;
            public double W2 { get { return _W2; } set { if (value == _W2) return; _W2 = value; OnPropertyChanged(); } }

            private double _R2;
            public double R2 { get { return _R2; } set { if (value == _R2) return; _R2 = value; OnPropertyChanged(); } }

            private double _G2;
            public double G2 { get { return _G2; } set { if (value == _G2) return; _G2 = value; OnPropertyChanged(); } }

            private double _B2;
            public double B2 { get { return _B2; } set { if (value == _B2) return; _B2 = value; OnPropertyChanged(); } }

            private double _Time2;
            public double Time2 { get { return _Time2; } set { if (value == _Time2) return; _Time2 = value; OnPropertyChanged(); } }

            private double _W3;
            public double W3 { get { return _W3; } set { if (value == _W3) return; _W3 = value; OnPropertyChanged(); } }

            private double _R3;
            public double R3 { get { return _R3; } set { if (value == _R3) return; _R3 = value; OnPropertyChanged(); } }

            private double _G3;
            public double G3 { get { return _G3; } set { if (value == _G3) return; _G3 = value; OnPropertyChanged(); } }

            private double _B3;
            public double B3 { get { return _B3; } set { if (value == _B3) return; _B3 = value; OnPropertyChanged(); } }

            private double _Time3;
            public double Time3 { get { return _Time3; } set { if (value == _Time3) return; _Time3 = value; OnPropertyChanged(); } }

            private double _W4;
            public double W4 { get { return _W4; } set { if (value == _W4) return; _W4 = value; OnPropertyChanged(); } }

            private double _R4;
            public double R4 { get { return _R4; } set { if (value == _R4) return; _R4 = value; OnPropertyChanged(); } }

            private double _G4;
            public double G4 { get { return _G4; } set { if (value == _G4) return; _G4 = value; OnPropertyChanged(); } }

            private double _B4;
            public double B4 { get { return _B4; } set { if (value == _B4) return; _B4 = value; OnPropertyChanged(); } }

            private double _Time4;
            public double Time4 { get { return _Time4; } set { if (value == _Time4) return; _Time4 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<TimerEffectsRecord> TimerEffectsRecordData { get; } = new DataCollection<TimerEffectsRecord>();
        private void OnTimerEffects_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (TimerEffectsRecordData.Count == 0)
                {
                    TimerEffectsRecordData.AddRecord(new TimerEffectsRecord());
                }
                TimerEffectsRecordData[TimerEffectsRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTimerEffects(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TimerEffectsRecordData.MaxLength = value;


        }

        private void OnAlgorithmTimerEffects(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TimerEffectsRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTimerEffects(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,W1,R1,G1,B1,Time1,W2,R2,G2,B2,Time2,W3,R3,G3,B3,Time3,W4,R4,G4,B4,Time4,Notes\n");
            foreach (var row in TimerEffectsRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.W1},{row.R1},{row.G1},{row.B1},{row.Time1},{row.W2},{row.R2},{row.G2},{row.B2},{row.Time2},{row.W3},{row.R3},{row.G3},{row.B3},{row.Time3},{row.W4},{row.R4},{row.G4},{row.B4},{row.Time4},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTimerEffects(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTimerEffects();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read TimerEffects");
                    return;
                }

                var record = new TimerEffectsRecord();

                var W1 = valueList.GetValue("W1");
                if (W1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || W1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.W1 = (double)W1.AsDouble;
                    TimerEffects_W1.Text = record.W1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var R1 = valueList.GetValue("R1");
                if (R1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || R1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.R1 = (double)R1.AsDouble;
                    TimerEffects_R1.Text = record.R1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var G1 = valueList.GetValue("G1");
                if (G1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || G1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.G1 = (double)G1.AsDouble;
                    TimerEffects_G1.Text = record.G1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var B1 = valueList.GetValue("B1");
                if (B1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || B1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.B1 = (double)B1.AsDouble;
                    TimerEffects_B1.Text = record.B1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Time1 = valueList.GetValue("Time1");
                if (Time1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Time1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Time1 = (double)Time1.AsDouble;
                    TimerEffects_Time1.Text = record.Time1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var W2 = valueList.GetValue("W2");
                if (W2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || W2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.W2 = (double)W2.AsDouble;
                    TimerEffects_W2.Text = record.W2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var R2 = valueList.GetValue("R2");
                if (R2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || R2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.R2 = (double)R2.AsDouble;
                    TimerEffects_R2.Text = record.R2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var G2 = valueList.GetValue("G2");
                if (G2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || G2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.G2 = (double)G2.AsDouble;
                    TimerEffects_G2.Text = record.G2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var B2 = valueList.GetValue("B2");
                if (B2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || B2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.B2 = (double)B2.AsDouble;
                    TimerEffects_B2.Text = record.B2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Time2 = valueList.GetValue("Time2");
                if (Time2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Time2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Time2 = (double)Time2.AsDouble;
                    TimerEffects_Time2.Text = record.Time2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var W3 = valueList.GetValue("W3");
                if (W3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || W3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.W3 = (double)W3.AsDouble;
                    TimerEffects_W3.Text = record.W3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var R3 = valueList.GetValue("R3");
                if (R3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || R3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.R3 = (double)R3.AsDouble;
                    TimerEffects_R3.Text = record.R3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var G3 = valueList.GetValue("G3");
                if (G3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || G3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.G3 = (double)G3.AsDouble;
                    TimerEffects_G3.Text = record.G3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var B3 = valueList.GetValue("B3");
                if (B3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || B3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.B3 = (double)B3.AsDouble;
                    TimerEffects_B3.Text = record.B3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Time3 = valueList.GetValue("Time3");
                if (Time3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Time3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Time3 = (double)Time3.AsDouble;
                    TimerEffects_Time3.Text = record.Time3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var W4 = valueList.GetValue("W4");
                if (W4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || W4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.W4 = (double)W4.AsDouble;
                    TimerEffects_W4.Text = record.W4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var R4 = valueList.GetValue("R4");
                if (R4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || R4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.R4 = (double)R4.AsDouble;
                    TimerEffects_R4.Text = record.R4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var G4 = valueList.GetValue("G4");
                if (G4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || G4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.G4 = (double)G4.AsDouble;
                    TimerEffects_G4.Text = record.G4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var B4 = valueList.GetValue("B4");
                if (B4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || B4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.B4 = (double)B4.AsDouble;
                    TimerEffects_B4.Text = record.B4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Time4 = valueList.GetValue("Time4");
                if (Time4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Time4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Time4 = (double)Time4.AsDouble;
                    TimerEffects_Time4.Text = record.Time4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                TimerEffectsRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteSecurityMode(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte SecurityCommand;
                var parsedSecurityCommand = Utilities.Parsers.TryParseByte(SecurityMode_SecurityCommand.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityCommand);
                if (!parsedSecurityCommand)
                {
                    parseError = "SecurityCommand";
                }

                Byte SecurityCurrentMinute;
                var parsedSecurityCurrentMinute = Utilities.Parsers.TryParseByte(SecurityMode_SecurityCurrentMinute.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityCurrentMinute);
                if (!parsedSecurityCurrentMinute)
                {
                    parseError = "SecurityCurrentMinute";
                }

                Byte SecurityCurrentHour;
                var parsedSecurityCurrentHour = Utilities.Parsers.TryParseByte(SecurityMode_SecurityCurrentHour.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityCurrentHour);
                if (!parsedSecurityCurrentHour)
                {
                    parseError = "SecurityCurrentHour";
                }

                Byte SecurityStartingMinute;
                var parsedSecurityStartingMinute = Utilities.Parsers.TryParseByte(SecurityMode_SecurityStartingMinute.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityStartingMinute);
                if (!parsedSecurityStartingMinute)
                {
                    parseError = "SecurityStartingMinute";
                }

                Byte SecurityStartingHour;
                var parsedSecurityStartingHour = Utilities.Parsers.TryParseByte(SecurityMode_SecurityStartingHour.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityStartingHour);
                if (!parsedSecurityStartingHour)
                {
                    parseError = "SecurityStartingHour";
                }

                Byte SecurityEndingMinute;
                var parsedSecurityEndingMinute = Utilities.Parsers.TryParseByte(SecurityMode_SecurityEndingMinute.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityEndingMinute);
                if (!parsedSecurityEndingMinute)
                {
                    parseError = "SecurityEndingMinute";
                }

                Byte SecurityEndingHour;
                var parsedSecurityEndingHour = Utilities.Parsers.TryParseByte(SecurityMode_SecurityEndingHour.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityEndingHour);
                if (!parsedSecurityEndingHour)
                {
                    parseError = "SecurityEndingHour";
                }

                Byte SecurityMinInterval;
                var parsedSecurityMinInterval = Utilities.Parsers.TryParseByte(SecurityMode_SecurityMinInterval.Text, System.Globalization.NumberStyles.None, null, out SecurityMinInterval);
                if (!parsedSecurityMinInterval)
                {
                    parseError = "SecurityMinInterval";
                }

                Byte SecurityMaxInterval;
                var parsedSecurityMaxInterval = Utilities.Parsers.TryParseByte(SecurityMode_SecurityMaxInterval.Text, System.Globalization.NumberStyles.None, null, out SecurityMaxInterval);
                if (!parsedSecurityMaxInterval)
                {
                    parseError = "SecurityMaxInterval";
                }

                Byte SecurityW;
                var parsedSecurityW = Utilities.Parsers.TryParseByte(SecurityMode_SecurityW.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityW);
                if (!parsedSecurityW)
                {
                    parseError = "SecurityW";
                }

                Byte SecurityR;
                var parsedSecurityR = Utilities.Parsers.TryParseByte(SecurityMode_SecurityR.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityR);
                if (!parsedSecurityR)
                {
                    parseError = "SecurityR";
                }

                Byte SecurityG;
                var parsedSecurityG = Utilities.Parsers.TryParseByte(SecurityMode_SecurityG.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityG);
                if (!parsedSecurityG)
                {
                    parseError = "SecurityG";
                }

                Byte SecurityB;
                var parsedSecurityB = Utilities.Parsers.TryParseByte(SecurityMode_SecurityB.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out SecurityB);
                if (!parsedSecurityB)
                {
                    parseError = "SecurityB";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteSecurityMode(SecurityCommand, SecurityCurrentMinute, SecurityCurrentHour, SecurityStartingMinute, SecurityStartingHour, SecurityEndingMinute, SecurityEndingHour, SecurityMinInterval, SecurityMaxInterval, SecurityW, SecurityR, SecurityG, SecurityB);
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

        public class SecurityModeRecord : INotifyPropertyChanged
        {
            public SecurityModeRecord()
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

            private double _SecurityCommand;
            public double SecurityCommand { get { return _SecurityCommand; } set { if (value == _SecurityCommand) return; _SecurityCommand = value; OnPropertyChanged(); } }

            private double _SecurityCurrentMinute;
            public double SecurityCurrentMinute { get { return _SecurityCurrentMinute; } set { if (value == _SecurityCurrentMinute) return; _SecurityCurrentMinute = value; OnPropertyChanged(); } }

            private double _SecurityCurrentHour;
            public double SecurityCurrentHour { get { return _SecurityCurrentHour; } set { if (value == _SecurityCurrentHour) return; _SecurityCurrentHour = value; OnPropertyChanged(); } }

            private double _SecurityStartingMinute;
            public double SecurityStartingMinute { get { return _SecurityStartingMinute; } set { if (value == _SecurityStartingMinute) return; _SecurityStartingMinute = value; OnPropertyChanged(); } }

            private double _SecurityStartingHour;
            public double SecurityStartingHour { get { return _SecurityStartingHour; } set { if (value == _SecurityStartingHour) return; _SecurityStartingHour = value; OnPropertyChanged(); } }

            private double _SecurityEndingMinute;
            public double SecurityEndingMinute { get { return _SecurityEndingMinute; } set { if (value == _SecurityEndingMinute) return; _SecurityEndingMinute = value; OnPropertyChanged(); } }

            private double _SecurityEndingHour;
            public double SecurityEndingHour { get { return _SecurityEndingHour; } set { if (value == _SecurityEndingHour) return; _SecurityEndingHour = value; OnPropertyChanged(); } }

            private double _SecurityMinInterval;
            public double SecurityMinInterval { get { return _SecurityMinInterval; } set { if (value == _SecurityMinInterval) return; _SecurityMinInterval = value; OnPropertyChanged(); } }

            private double _SecurityMaxInterval;
            public double SecurityMaxInterval { get { return _SecurityMaxInterval; } set { if (value == _SecurityMaxInterval) return; _SecurityMaxInterval = value; OnPropertyChanged(); } }

            private double _SecurityW;
            public double SecurityW { get { return _SecurityW; } set { if (value == _SecurityW) return; _SecurityW = value; OnPropertyChanged(); } }

            private double _SecurityR;
            public double SecurityR { get { return _SecurityR; } set { if (value == _SecurityR) return; _SecurityR = value; OnPropertyChanged(); } }

            private double _SecurityG;
            public double SecurityG { get { return _SecurityG; } set { if (value == _SecurityG) return; _SecurityG = value; OnPropertyChanged(); } }

            private double _SecurityB;
            public double SecurityB { get { return _SecurityB; } set { if (value == _SecurityB) return; _SecurityB = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<SecurityModeRecord> SecurityModeRecordData { get; } = new DataCollection<SecurityModeRecord>();
        private void OnSecurityMode_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (SecurityModeRecordData.Count == 0)
                {
                    SecurityModeRecordData.AddRecord(new SecurityModeRecord());
                }
                SecurityModeRecordData[SecurityModeRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountSecurityMode(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            SecurityModeRecordData.MaxLength = value;


        }

        private void OnAlgorithmSecurityMode(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            SecurityModeRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopySecurityMode(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,SecurityCommand,SecurityCurrentMinute,SecurityCurrentHour,SecurityStartingMinute,SecurityStartingHour,SecurityEndingMinute,SecurityEndingHour,SecurityMinInterval,SecurityMaxInterval,SecurityW,SecurityR,SecurityG,SecurityB,Notes\n");
            foreach (var row in SecurityModeRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.SecurityCommand},{row.SecurityCurrentMinute},{row.SecurityCurrentHour},{row.SecurityStartingMinute},{row.SecurityStartingHour},{row.SecurityEndingMinute},{row.SecurityEndingHour},{row.SecurityMinInterval},{row.SecurityMaxInterval},{row.SecurityW},{row.SecurityR},{row.SecurityG},{row.SecurityB},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadSecurityMode(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSecurityMode();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read SecurityMode");
                    return;
                }

                var record = new SecurityModeRecord();

                var SecurityCommand = valueList.GetValue("SecurityCommand");
                if (SecurityCommand.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityCommand.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityCommand = (double)SecurityCommand.AsDouble;
                    SecurityMode_SecurityCommand.Text = record.SecurityCommand.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityCurrentMinute = valueList.GetValue("SecurityCurrentMinute");
                if (SecurityCurrentMinute.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityCurrentMinute.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityCurrentMinute = (double)SecurityCurrentMinute.AsDouble;
                    SecurityMode_SecurityCurrentMinute.Text = record.SecurityCurrentMinute.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityCurrentHour = valueList.GetValue("SecurityCurrentHour");
                if (SecurityCurrentHour.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityCurrentHour.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityCurrentHour = (double)SecurityCurrentHour.AsDouble;
                    SecurityMode_SecurityCurrentHour.Text = record.SecurityCurrentHour.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityStartingMinute = valueList.GetValue("SecurityStartingMinute");
                if (SecurityStartingMinute.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityStartingMinute.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityStartingMinute = (double)SecurityStartingMinute.AsDouble;
                    SecurityMode_SecurityStartingMinute.Text = record.SecurityStartingMinute.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityStartingHour = valueList.GetValue("SecurityStartingHour");
                if (SecurityStartingHour.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityStartingHour.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityStartingHour = (double)SecurityStartingHour.AsDouble;
                    SecurityMode_SecurityStartingHour.Text = record.SecurityStartingHour.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityEndingMinute = valueList.GetValue("SecurityEndingMinute");
                if (SecurityEndingMinute.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityEndingMinute.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityEndingMinute = (double)SecurityEndingMinute.AsDouble;
                    SecurityMode_SecurityEndingMinute.Text = record.SecurityEndingMinute.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityEndingHour = valueList.GetValue("SecurityEndingHour");
                if (SecurityEndingHour.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityEndingHour.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityEndingHour = (double)SecurityEndingHour.AsDouble;
                    SecurityMode_SecurityEndingHour.Text = record.SecurityEndingHour.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityMinInterval = valueList.GetValue("SecurityMinInterval");
                if (SecurityMinInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityMinInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityMinInterval = (double)SecurityMinInterval.AsDouble;
                    SecurityMode_SecurityMinInterval.Text = record.SecurityMinInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityMaxInterval = valueList.GetValue("SecurityMaxInterval");
                if (SecurityMaxInterval.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityMaxInterval.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityMaxInterval = (double)SecurityMaxInterval.AsDouble;
                    SecurityMode_SecurityMaxInterval.Text = record.SecurityMaxInterval.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityW = valueList.GetValue("SecurityW");
                if (SecurityW.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityW.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityW = (double)SecurityW.AsDouble;
                    SecurityMode_SecurityW.Text = record.SecurityW.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityR = valueList.GetValue("SecurityR");
                if (SecurityR.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityR.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityR = (double)SecurityR.AsDouble;
                    SecurityMode_SecurityR.Text = record.SecurityR.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityG = valueList.GetValue("SecurityG");
                if (SecurityG.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityG.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityG = (double)SecurityG.AsDouble;
                    SecurityMode_SecurityG.Text = record.SecurityG.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var SecurityB = valueList.GetValue("SecurityB");
                if (SecurityB.CurrentType == BCBasic.BCValue.ValueType.IsDouble || SecurityB.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.SecurityB = (double)SecurityB.AsDouble;
                    SecurityMode_SecurityB.Text = record.SecurityB.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                SecurityModeRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteEffect(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte W;
                var parsedW = Utilities.Parsers.TryParseByte(Effect_W.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out W);
                if (!parsedW)
                {
                    parseError = "W";
                }

                Byte R;
                var parsedR = Utilities.Parsers.TryParseByte(Effect_R.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out R);
                if (!parsedR)
                {
                    parseError = "R";
                }

                Byte G;
                var parsedG = Utilities.Parsers.TryParseByte(Effect_G.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out G);
                if (!parsedG)
                {
                    parseError = "G";
                }

                Byte B;
                var parsedB = Utilities.Parsers.TryParseByte(Effect_B.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out B);
                if (!parsedB)
                {
                    parseError = "B";
                }

                Byte Effect;
                var parsedEffect = Utilities.Parsers.TryParseByte(Effect_Effect.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Effect);
                if (!parsedEffect)
                {
                    parseError = "Effect";
                }

                Byte Junk;
                var parsedJunk = Utilities.Parsers.TryParseByte(Effect_Junk.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Junk);
                if (!parsedJunk)
                {
                    parseError = "Junk";
                }

                Byte Delay1;
                var parsedDelay1 = Utilities.Parsers.TryParseByte(Effect_Delay1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Delay1);
                if (!parsedDelay1)
                {
                    parseError = "Delay1";
                }

                Byte Delay2;
                var parsedDelay2 = Utilities.Parsers.TryParseByte(Effect_Delay2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Delay2);
                if (!parsedDelay2)
                {
                    parseError = "Delay2";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteEffect(W, R, G, B, Effect, Junk, Delay1, Delay2);
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

        public class EffectRecord : INotifyPropertyChanged
        {
            public EffectRecord()
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

            private double _W;
            public double W { get { return _W; } set { if (value == _W) return; _W = value; OnPropertyChanged(); } }

            private double _R;
            public double R { get { return _R; } set { if (value == _R) return; _R = value; OnPropertyChanged(); } }

            private double _G;
            public double G { get { return _G; } set { if (value == _G) return; _G = value; OnPropertyChanged(); } }

            private double _B;
            public double B { get { return _B; } set { if (value == _B) return; _B = value; OnPropertyChanged(); } }

            private double _Effect;
            public double Effect { get { return _Effect; } set { if (value == _Effect) return; _Effect = value; OnPropertyChanged(); } }

            private double _Junk;
            public double Junk { get { return _Junk; } set { if (value == _Junk) return; _Junk = value; OnPropertyChanged(); } }

            private double _Delay1;
            public double Delay1 { get { return _Delay1; } set { if (value == _Delay1) return; _Delay1 = value; OnPropertyChanged(); } }

            private double _Delay2;
            public double Delay2 { get { return _Delay2; } set { if (value == _Delay2) return; _Delay2 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<EffectRecord> EffectRecordData { get; } = new DataCollection<EffectRecord>();
        private void OnEffect_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (EffectRecordData.Count == 0)
                {
                    EffectRecordData.AddRecord(new EffectRecord());
                }
                EffectRecordData[EffectRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountEffect(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EffectRecordData.MaxLength = value;


        }

        private void OnAlgorithmEffect(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            EffectRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyEffect(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,W,R,G,B,Effect,Junk,Delay1,Delay2,Notes\n");
            foreach (var row in EffectRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.W},{row.R},{row.G},{row.B},{row.Effect},{row.Junk},{row.Delay1},{row.Delay2},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadEffect(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadEffect();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Effect");
                    return;
                }

                var record = new EffectRecord();

                var W = valueList.GetValue("W");
                if (W.CurrentType == BCBasic.BCValue.ValueType.IsDouble || W.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.W = (double)W.AsDouble;
                    Effect_W.Text = record.W.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var R = valueList.GetValue("R");
                if (R.CurrentType == BCBasic.BCValue.ValueType.IsDouble || R.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.R = (double)R.AsDouble;
                    Effect_R.Text = record.R.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var G = valueList.GetValue("G");
                if (G.CurrentType == BCBasic.BCValue.ValueType.IsDouble || G.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.G = (double)G.AsDouble;
                    Effect_G.Text = record.G.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var B = valueList.GetValue("B");
                if (B.CurrentType == BCBasic.BCValue.ValueType.IsDouble || B.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.B = (double)B.AsDouble;
                    Effect_B.Text = record.B.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Effect = valueList.GetValue("Effect");
                if (Effect.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Effect.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Effect = (double)Effect.AsDouble;
                    Effect_Effect.Text = record.Effect.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Junk = valueList.GetValue("Junk");
                if (Junk.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Junk.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Junk = (double)Junk.AsDouble;
                    Effect_Junk.Text = record.Junk.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Delay1 = valueList.GetValue("Delay1");
                if (Delay1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Delay1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Delay1 = (double)Delay1.AsDouble;
                    Effect_Delay1.Text = record.Delay1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Delay2 = valueList.GetValue("Delay2");
                if (Delay2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Delay2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Delay2 = (double)Delay2.AsDouble;
                    Effect_Delay2.Text = record.Delay2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                EffectRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteColor(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte W;
                var parsedW = Utilities.Parsers.TryParseByte(Color_W.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out W);
                if (!parsedW)
                {
                    parseError = "W";
                }

                Byte R;
                var parsedR = Utilities.Parsers.TryParseByte(Color_R.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out R);
                if (!parsedR)
                {
                    parseError = "R";
                }

                Byte G;
                var parsedG = Utilities.Parsers.TryParseByte(Color_G.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out G);
                if (!parsedG)
                {
                    parseError = "G";
                }

                Byte B;
                var parsedB = Utilities.Parsers.TryParseByte(Color_B.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out B);
                if (!parsedB)
                {
                    parseError = "B";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteColor(W, R, G, B);
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

        public class ColorRecord : INotifyPropertyChanged
        {
            public ColorRecord()
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

            private double _W;
            public double W { get { return _W; } set { if (value == _W) return; _W = value; OnPropertyChanged(); } }

            private double _R;
            public double R { get { return _R; } set { if (value == _R) return; _R = value; OnPropertyChanged(); } }

            private double _G;
            public double G { get { return _G; } set { if (value == _G) return; _G = value; OnPropertyChanged(); } }

            private double _B;
            public double B { get { return _B; } set { if (value == _B) return; _B = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ColorRecord> ColorRecordData { get; } = new DataCollection<ColorRecord>();
        private void OnColor_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ColorRecordData.Count == 0)
                {
                    ColorRecordData.AddRecord(new ColorRecord());
                }
                ColorRecordData[ColorRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountColor(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ColorRecordData.MaxLength = value;


        }

        private void OnAlgorithmColor(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ColorRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyColor(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,W,R,G,B,Notes\n");
            foreach (var row in ColorRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.W},{row.R},{row.G},{row.B},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadColor(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadColor();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Color");
                    return;
                }

                var record = new ColorRecord();

                var W = valueList.GetValue("W");
                if (W.CurrentType == BCBasic.BCValue.ValueType.IsDouble || W.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.W = (double)W.AsDouble;
                    Color_W.Text = record.W.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var R = valueList.GetValue("R");
                if (R.CurrentType == BCBasic.BCValue.ValueType.IsDouble || R.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.R = (double)R.AsDouble;
                    Color_R.Text = record.R.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var G = valueList.GetValue("G");
                if (G.CurrentType == BCBasic.BCValue.ValueType.IsDouble || G.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.G = (double)G.AsDouble;
                    Color_G.Text = record.G.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var B = valueList.GetValue("B");
                if (B.CurrentType == BCBasic.BCValue.ValueType.IsDouble || B.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.B = (double)B.AsDouble;
                    Color_B.Text = record.B.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                ColorRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteReset(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte ResetNow;
                var parsedResetNow = Utilities.Parsers.TryParseByte(Reset_ResetNow.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ResetNow);
                if (!parsedResetNow)
                {
                    parseError = "ResetNow";
                }

                Bytes ResetAdditional;
                var parsedResetAdditional = Utilities.Parsers.TryParseBytes(Reset_ResetAdditional.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out ResetAdditional);
                if (!parsedResetAdditional)
                {
                    parseError = "ResetAdditional";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteReset(ResetNow, ResetAdditional);
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

        public class ResetRecord : INotifyPropertyChanged
        {
            public ResetRecord()
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

            private double _ResetNow;
            public double ResetNow { get { return _ResetNow; } set { if (value == _ResetNow) return; _ResetNow = value; OnPropertyChanged(); } }

            private string _ResetAdditional;
            public string ResetAdditional { get { return _ResetAdditional; } set { if (value == _ResetAdditional) return; _ResetAdditional = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ResetRecord> ResetRecordData { get; } = new DataCollection<ResetRecord>();
        private void OnReset_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ResetRecordData.Count == 0)
                {
                    ResetRecordData.AddRecord(new ResetRecord());
                }
                ResetRecordData[ResetRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountReset(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ResetRecordData.MaxLength = value;


        }

        private void OnAlgorithmReset(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ResetRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyReset(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,ResetNow,ResetAdditional,Notes\n");
            foreach (var row in ResetRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.ResetNow},{row.ResetAdditional},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadReset(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadReset();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Reset");
                    return;
                }

                var record = new ResetRecord();

                var ResetNow = valueList.GetValue("ResetNow");
                if (ResetNow.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ResetNow.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ResetNow = (double)ResetNow.AsDouble;
                    Reset_ResetNow.Text = record.ResetNow.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var ResetAdditional = valueList.GetValue("ResetAdditional");
                if (ResetAdditional.CurrentType == BCBasic.BCValue.ValueType.IsDouble || ResetAdditional.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.ResetAdditional = (string)ResetAdditional.AsString;
                    Reset_ResetAdditional.Text = record.ResetAdditional.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                ResetRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteTimer(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte TimerIndex;
                var parsedTimerIndex = Utilities.Parsers.TryParseByte(Timer_TimerIndex.Text, System.Globalization.NumberStyles.None, null, out TimerIndex);
                if (!parsedTimerIndex)
                {
                    parseError = "TimerIndex";
                }

                Byte TimerType;
                var parsedTimerType = Utilities.Parsers.TryParseByte(Timer_TimerType.Text, System.Globalization.NumberStyles.None, null, out TimerType);
                if (!parsedTimerType)
                {
                    parseError = "TimerType";
                }

                Byte TimerSeconds;
                var parsedTimerSeconds = Utilities.Parsers.TryParseByte(Timer_TimerSeconds.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerSeconds);
                if (!parsedTimerSeconds)
                {
                    parseError = "TimerSeconds";
                }

                Byte TimerMinutes;
                var parsedTimerMinutes = Utilities.Parsers.TryParseByte(Timer_TimerMinutes.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerMinutes);
                if (!parsedTimerMinutes)
                {
                    parseError = "TimerMinutes";
                }

                Byte TimerHours;
                var parsedTimerHours = Utilities.Parsers.TryParseByte(Timer_TimerHours.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerHours);
                if (!parsedTimerHours)
                {
                    parseError = "TimerHours";
                }

                Byte TimerRun;
                var parsedTimerRun = Utilities.Parsers.TryParseByte(Timer_TimerRun.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerRun);
                if (!parsedTimerRun)
                {
                    parseError = "TimerRun";
                }

                Byte TimerMinutesStart;
                var parsedTimerMinutesStart = Utilities.Parsers.TryParseByte(Timer_TimerMinutesStart.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerMinutesStart);
                if (!parsedTimerMinutesStart)
                {
                    parseError = "TimerMinutesStart";
                }

                Byte TimerHoursStart;
                var parsedTimerHoursStart = Utilities.Parsers.TryParseByte(Timer_TimerHoursStart.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerHoursStart);
                if (!parsedTimerHoursStart)
                {
                    parseError = "TimerHoursStart";
                }

                Byte TimerColorW;
                var parsedTimerColorW = Utilities.Parsers.TryParseByte(Timer_TimerColorW.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerColorW);
                if (!parsedTimerColorW)
                {
                    parseError = "TimerColorW";
                }

                Byte TimerColorR;
                var parsedTimerColorR = Utilities.Parsers.TryParseByte(Timer_TimerColorR.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerColorR);
                if (!parsedTimerColorR)
                {
                    parseError = "TimerColorR";
                }

                Byte TimerColorG;
                var parsedTimerColorG = Utilities.Parsers.TryParseByte(Timer_TimerColorG.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerColorG);
                if (!parsedTimerColorG)
                {
                    parseError = "TimerColorG";
                }

                Byte TimerColorB;
                var parsedTimerColorB = Utilities.Parsers.TryParseByte(Timer_TimerColorB.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerColorB);
                if (!parsedTimerColorB)
                {
                    parseError = "TimerColorB";
                }

                Byte TimerRuntime;
                var parsedTimerRuntime = Utilities.Parsers.TryParseByte(Timer_TimerRuntime.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out TimerRuntime);
                if (!parsedTimerRuntime)
                {
                    parseError = "TimerRuntime";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteTimer(TimerIndex, TimerType, TimerSeconds, TimerMinutes, TimerHours, TimerRun, TimerMinutesStart, TimerHoursStart, TimerColorW, TimerColorR, TimerColorG, TimerColorB, TimerRuntime);
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

        public class TimerRecord : INotifyPropertyChanged
        {
            public TimerRecord()
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

            private double _TimerIndex;
            public double TimerIndex { get { return _TimerIndex; } set { if (value == _TimerIndex) return; _TimerIndex = value; OnPropertyChanged(); } }

            private double _TimerType;
            public double TimerType { get { return _TimerType; } set { if (value == _TimerType) return; _TimerType = value; OnPropertyChanged(); } }

            private double _TimerSeconds;
            public double TimerSeconds { get { return _TimerSeconds; } set { if (value == _TimerSeconds) return; _TimerSeconds = value; OnPropertyChanged(); } }

            private double _TimerMinutes;
            public double TimerMinutes { get { return _TimerMinutes; } set { if (value == _TimerMinutes) return; _TimerMinutes = value; OnPropertyChanged(); } }

            private double _TimerHours;
            public double TimerHours { get { return _TimerHours; } set { if (value == _TimerHours) return; _TimerHours = value; OnPropertyChanged(); } }

            private double _TimerRun;
            public double TimerRun { get { return _TimerRun; } set { if (value == _TimerRun) return; _TimerRun = value; OnPropertyChanged(); } }

            private double _TimerMinutesStart;
            public double TimerMinutesStart { get { return _TimerMinutesStart; } set { if (value == _TimerMinutesStart) return; _TimerMinutesStart = value; OnPropertyChanged(); } }

            private double _TimerHoursStart;
            public double TimerHoursStart { get { return _TimerHoursStart; } set { if (value == _TimerHoursStart) return; _TimerHoursStart = value; OnPropertyChanged(); } }

            private double _TimerColorW;
            public double TimerColorW { get { return _TimerColorW; } set { if (value == _TimerColorW) return; _TimerColorW = value; OnPropertyChanged(); } }

            private double _TimerColorR;
            public double TimerColorR { get { return _TimerColorR; } set { if (value == _TimerColorR) return; _TimerColorR = value; OnPropertyChanged(); } }

            private double _TimerColorG;
            public double TimerColorG { get { return _TimerColorG; } set { if (value == _TimerColorG) return; _TimerColorG = value; OnPropertyChanged(); } }

            private double _TimerColorB;
            public double TimerColorB { get { return _TimerColorB; } set { if (value == _TimerColorB) return; _TimerColorB = value; OnPropertyChanged(); } }

            private double _TimerRuntime;
            public double TimerRuntime { get { return _TimerRuntime; } set { if (value == _TimerRuntime) return; _TimerRuntime = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<TimerRecord> TimerRecordData { get; } = new DataCollection<TimerRecord>();
        private void OnTimer_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (TimerRecordData.Count == 0)
                {
                    TimerRecordData.AddRecord(new TimerRecord());
                }
                TimerRecordData[TimerRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountTimer(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TimerRecordData.MaxLength = value;


        }

        private void OnAlgorithmTimer(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            TimerRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyTimer(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,TimerIndex,TimerType,TimerSeconds,TimerMinutes,TimerHours,TimerRun,TimerMinutesStart,TimerHoursStart,TimerColorW,TimerColorR,TimerColorG,TimerColorB,TimerRuntime,Notes\n");
            foreach (var row in TimerRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.TimerIndex},{row.TimerType},{row.TimerSeconds},{row.TimerMinutes},{row.TimerHours},{row.TimerRun},{row.TimerMinutesStart},{row.TimerHoursStart},{row.TimerColorW},{row.TimerColorR},{row.TimerColorG},{row.TimerColorB},{row.TimerRuntime},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadTimer(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadTimer();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Timer");
                    return;
                }

                var record = new TimerRecord();

                var TimerIndex = valueList.GetValue("TimerIndex");
                if (TimerIndex.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerIndex.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerIndex = (double)TimerIndex.AsDouble;
                    Timer_TimerIndex.Text = record.TimerIndex.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerType = valueList.GetValue("TimerType");
                if (TimerType.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerType.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerType = (double)TimerType.AsDouble;
                    Timer_TimerType.Text = record.TimerType.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerSeconds = valueList.GetValue("TimerSeconds");
                if (TimerSeconds.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerSeconds.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerSeconds = (double)TimerSeconds.AsDouble;
                    Timer_TimerSeconds.Text = record.TimerSeconds.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerMinutes = valueList.GetValue("TimerMinutes");
                if (TimerMinutes.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerMinutes.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerMinutes = (double)TimerMinutes.AsDouble;
                    Timer_TimerMinutes.Text = record.TimerMinutes.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerHours = valueList.GetValue("TimerHours");
                if (TimerHours.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerHours.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerHours = (double)TimerHours.AsDouble;
                    Timer_TimerHours.Text = record.TimerHours.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerRun = valueList.GetValue("TimerRun");
                if (TimerRun.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerRun.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerRun = (double)TimerRun.AsDouble;
                    Timer_TimerRun.Text = record.TimerRun.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerMinutesStart = valueList.GetValue("TimerMinutesStart");
                if (TimerMinutesStart.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerMinutesStart.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerMinutesStart = (double)TimerMinutesStart.AsDouble;
                    Timer_TimerMinutesStart.Text = record.TimerMinutesStart.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerHoursStart = valueList.GetValue("TimerHoursStart");
                if (TimerHoursStart.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerHoursStart.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerHoursStart = (double)TimerHoursStart.AsDouble;
                    Timer_TimerHoursStart.Text = record.TimerHoursStart.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerColorW = valueList.GetValue("TimerColorW");
                if (TimerColorW.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerColorW.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerColorW = (double)TimerColorW.AsDouble;
                    Timer_TimerColorW.Text = record.TimerColorW.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerColorR = valueList.GetValue("TimerColorR");
                if (TimerColorR.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerColorR.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerColorR = (double)TimerColorR.AsDouble;
                    Timer_TimerColorR.Text = record.TimerColorR.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerColorG = valueList.GetValue("TimerColorG");
                if (TimerColorG.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerColorG.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerColorG = (double)TimerColorG.AsDouble;
                    Timer_TimerColorG.Text = record.TimerColorG.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerColorB = valueList.GetValue("TimerColorB");
                if (TimerColorB.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerColorB.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerColorB = (double)TimerColorB.AsDouble;
                    Timer_TimerColorB.Text = record.TimerColorB.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var TimerRuntime = valueList.GetValue("TimerRuntime");
                if (TimerRuntime.CurrentType == BCBasic.BCValue.ValueType.IsDouble || TimerRuntime.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.TimerRuntime = (double)TimerRuntime.AsDouble;
                    Timer_TimerRuntime.Text = record.TimerRuntime.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                TimerRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteGivenName(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                String GivenName;
                var parsedGivenName = Utilities.Parsers.TryParseString(GivenName_GivenName.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out GivenName);
                if (!parsedGivenName)
                {
                    parseError = "GivenName";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteGivenName(GivenName);
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

        public class GivenNameRecord : INotifyPropertyChanged
        {
            public GivenNameRecord()
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

            private string _GivenName;
            public string GivenName { get { return _GivenName; } set { if (value == _GivenName) return; _GivenName = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<GivenNameRecord> GivenNameRecordData { get; } = new DataCollection<GivenNameRecord>();
        private void OnGivenName_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (GivenNameRecordData.Count == 0)
                {
                    GivenNameRecordData.AddRecord(new GivenNameRecord());
                }
                GivenNameRecordData[GivenNameRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountGivenName(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            GivenNameRecordData.MaxLength = value;


        }

        private void OnAlgorithmGivenName(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            GivenNameRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyGivenName(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,GivenName,Notes\n");
            foreach (var row in GivenNameRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.GivenName},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadGivenName(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadGivenName();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read GivenName");
                    return;
                }

                var record = new GivenNameRecord();

                var GivenName = valueList.GetValue("GivenName");
                if (GivenName.CurrentType == BCBasic.BCValue.ValueType.IsDouble || GivenName.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.GivenName = (string)GivenName.AsString;
                    GivenName_GivenName.Text = record.GivenName.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                GivenNameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteUnknown10(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown10;
                var parsedUnknown10 = Utilities.Parsers.TryParseBytes(Unknown10_Unknown10.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Unknown10);
                if (!parsedUnknown10)
                {
                    parseError = "Unknown10";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUnknown10(Unknown10);
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

        public class Unknown10Record : INotifyPropertyChanged
        {
            public Unknown10Record()
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

        public DataCollection<Unknown10Record> Unknown10RecordData { get; } = new DataCollection<Unknown10Record>();
        private void OnUnknown10_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Unknown10RecordData.Count == 0)
                {
                    Unknown10RecordData.AddRecord(new Unknown10Record());
                }
                Unknown10RecordData[Unknown10RecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUnknown10(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown10RecordData.MaxLength = value;


        }

        private void OnAlgorithmUnknown10(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Unknown10RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUnknown10(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Unknown10,Notes\n");
            foreach (var row in Unknown10RecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Unknown10},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUnknown10(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUnknown10();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Unknown10");
                    return;
                }

                var record = new Unknown10Record();

                var Unknown10 = valueList.GetValue("Unknown10");
                if (Unknown10.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Unknown10.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Unknown10 = (string)Unknown10.AsString;
                    Unknown10_Unknown10.Text = record.Unknown10.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Unknown10RecordData.Add(record);

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

        // Functions for AirCableSmartMeshService


        private async void OnWriteNetworkKey(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes NetworkKey;
                var parsedNetworkKey = Utilities.Parsers.TryParseBytes(NetworkKey_NetworkKey.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out NetworkKey);
                if (!parsedNetworkKey)
                {
                    parseError = "NetworkKey";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteNetworkKey(NetworkKey);
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

        public class DeviceUuidRecord : INotifyPropertyChanged
        {
            public DeviceUuidRecord()
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

            private string _DeviceUuid;
            public string DeviceUuid { get { return _DeviceUuid; } set { if (value == _DeviceUuid) return; _DeviceUuid = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<DeviceUuidRecord> DeviceUuidRecordData { get; } = new DataCollection<DeviceUuidRecord>();
        private void OnDeviceUuid_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (DeviceUuidRecordData.Count == 0)
                {
                    DeviceUuidRecordData.AddRecord(new DeviceUuidRecord());
                }
                DeviceUuidRecordData[DeviceUuidRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDeviceUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DeviceUuidRecordData.MaxLength = value;


        }

        private void OnAlgorithmDeviceUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DeviceUuidRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDeviceUuid(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DeviceUuid,Notes\n");
            foreach (var row in DeviceUuidRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DeviceUuid},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadDeviceUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDeviceUuid();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read DeviceUuid");
                    return;
                }

                var record = new DeviceUuidRecord();

                var DeviceUuid = valueList.GetValue("DeviceUuid");
                if (DeviceUuid.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DeviceUuid.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DeviceUuid = (string)DeviceUuid.AsString;
                    DeviceUuid_DeviceUuid.Text = record.DeviceUuid.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                DeviceUuidRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteDeviceId(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes DeviceId;
                var parsedDeviceId = Utilities.Parsers.TryParseBytes(DeviceId_DeviceId.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out DeviceId);
                if (!parsedDeviceId)
                {
                    parseError = "DeviceId";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteDeviceId(DeviceId);
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

        public class DeviceIdRecord : INotifyPropertyChanged
        {
            public DeviceIdRecord()
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

            private string _DeviceId;
            public string DeviceId { get { return _DeviceId; } set { if (value == _DeviceId) return; _DeviceId = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<DeviceIdRecord> DeviceIdRecordData { get; } = new DataCollection<DeviceIdRecord>();
        private void OnDeviceId_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (DeviceIdRecordData.Count == 0)
                {
                    DeviceIdRecordData.AddRecord(new DeviceIdRecord());
                }
                DeviceIdRecordData[DeviceIdRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDeviceId(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DeviceIdRecordData.MaxLength = value;


        }

        private void OnAlgorithmDeviceId(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DeviceIdRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDeviceId(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DeviceId,Notes\n");
            foreach (var row in DeviceIdRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DeviceId},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadDeviceId(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDeviceId();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read DeviceId");
                    return;
                }

                var record = new DeviceIdRecord();

                var DeviceId = valueList.GetValue("DeviceId");
                if (DeviceId.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DeviceId.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.DeviceId = (string)DeviceId.AsString;
                    DeviceId_DeviceId.Text = record.DeviceId.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                DeviceIdRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteMtlContinuationCpUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes MtlContinuationCpUuid;
                var parsedMtlContinuationCpUuid = Utilities.Parsers.TryParseBytes(MtlContinuationCpUuid_MtlContinuationCpUuid.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out MtlContinuationCpUuid);
                if (!parsedMtlContinuationCpUuid)
                {
                    parseError = "MtlContinuationCpUuid";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMtlContinuationCpUuid(MtlContinuationCpUuid);
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

        public class MtlContinuationCpUuidRecord : INotifyPropertyChanged
        {
            public MtlContinuationCpUuidRecord()
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

            private string _MtlContinuationCpUuid;
            public string MtlContinuationCpUuid { get { return _MtlContinuationCpUuid; } set { if (value == _MtlContinuationCpUuid) return; _MtlContinuationCpUuid = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MtlContinuationCpUuidRecord> MtlContinuationCpUuidRecordData { get; } = new DataCollection<MtlContinuationCpUuidRecord>();
        private void OnMtlContinuationCpUuid_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MtlContinuationCpUuidRecordData.Count == 0)
                {
                    MtlContinuationCpUuidRecordData.AddRecord(new MtlContinuationCpUuidRecord());
                }
                MtlContinuationCpUuidRecordData[MtlContinuationCpUuidRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMtlContinuationCpUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MtlContinuationCpUuidRecordData.MaxLength = value;


        }

        private void OnAlgorithmMtlContinuationCpUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MtlContinuationCpUuidRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMtlContinuationCpUuid(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MtlContinuationCpUuid,Notes\n");
            foreach (var row in MtlContinuationCpUuidRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MtlContinuationCpUuid},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMtlContinuationCpUuidSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int MtlContinuationCpUuidNotifyIndex = 0;
        bool MtlContinuationCpUuidNotifySetup = false;
        private async void OnNotifyMtlContinuationCpUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!MtlContinuationCpUuidNotifySetup)
                {
                    MtlContinuationCpUuidNotifySetup = true;
                    bleDevice.MtlContinuationCpUuidEvent += BleDevice_MtlContinuationCpUuidEvent;
                }
                var notifyType = NotifyMtlContinuationCpUuidSettings[MtlContinuationCpUuidNotifyIndex];
                MtlContinuationCpUuidNotifyIndex = (MtlContinuationCpUuidNotifyIndex + 1) % NotifyMtlContinuationCpUuidSettings.Length;
                var result = await bleDevice.NotifyMtlContinuationCpUuidAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_MtlContinuationCpUuidEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new MtlContinuationCpUuidRecord();

                    var MtlContinuationCpUuid = valueList.GetValue("MtlContinuationCpUuid");
                    if (MtlContinuationCpUuid.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MtlContinuationCpUuid.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.MtlContinuationCpUuid = (string)MtlContinuationCpUuid.AsString;
                        MtlContinuationCpUuid_MtlContinuationCpUuid.Text = record.MtlContinuationCpUuid.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = MtlContinuationCpUuidRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteMtlCompleteCpUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes MtlCompleteCpUuid;
                var parsedMtlCompleteCpUuid = Utilities.Parsers.TryParseBytes(MtlCompleteCpUuid_MtlCompleteCpUuid.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out MtlCompleteCpUuid);
                if (!parsedMtlCompleteCpUuid)
                {
                    parseError = "MtlCompleteCpUuid";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMtlCompleteCpUuid(MtlCompleteCpUuid);
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

        public class MtlCompleteCpUuidRecord : INotifyPropertyChanged
        {
            public MtlCompleteCpUuidRecord()
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

            private string _MtlCompleteCpUuid;
            public string MtlCompleteCpUuid { get { return _MtlCompleteCpUuid; } set { if (value == _MtlCompleteCpUuid) return; _MtlCompleteCpUuid = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MtlCompleteCpUuidRecord> MtlCompleteCpUuidRecordData { get; } = new DataCollection<MtlCompleteCpUuidRecord>();
        private void OnMtlCompleteCpUuid_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MtlCompleteCpUuidRecordData.Count == 0)
                {
                    MtlCompleteCpUuidRecordData.AddRecord(new MtlCompleteCpUuidRecord());
                }
                MtlCompleteCpUuidRecordData[MtlCompleteCpUuidRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMtlCompleteCpUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MtlCompleteCpUuidRecordData.MaxLength = value;


        }

        private void OnAlgorithmMtlCompleteCpUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MtlCompleteCpUuidRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMtlCompleteCpUuid(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MtlCompleteCpUuid,Notes\n");
            foreach (var row in MtlCompleteCpUuidRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MtlCompleteCpUuid},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMtlCompleteCpUuidSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int MtlCompleteCpUuidNotifyIndex = 0;
        bool MtlCompleteCpUuidNotifySetup = false;
        private async void OnNotifyMtlCompleteCpUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!MtlCompleteCpUuidNotifySetup)
                {
                    MtlCompleteCpUuidNotifySetup = true;
                    bleDevice.MtlCompleteCpUuidEvent += BleDevice_MtlCompleteCpUuidEvent;
                }
                var notifyType = NotifyMtlCompleteCpUuidSettings[MtlCompleteCpUuidNotifyIndex];
                MtlCompleteCpUuidNotifyIndex = (MtlCompleteCpUuidNotifyIndex + 1) % NotifyMtlCompleteCpUuidSettings.Length;
                var result = await bleDevice.NotifyMtlCompleteCpUuidAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_MtlCompleteCpUuidEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new MtlCompleteCpUuidRecord();

                    var MtlCompleteCpUuid = valueList.GetValue("MtlCompleteCpUuid");
                    if (MtlCompleteCpUuid.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MtlCompleteCpUuid.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.MtlCompleteCpUuid = (string)MtlCompleteCpUuid.AsString;
                        MtlCompleteCpUuid_MtlCompleteCpUuid.Text = record.MtlCompleteCpUuid.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = MtlCompleteCpUuidRecordData.AddRecord(record);

                });
            }
        }
        private async void OnWriteMtlTtlUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes MtlTtlUuid;
                var parsedMtlTtlUuid = Utilities.Parsers.TryParseBytes(MtlTtlUuid_MtlTtlUuid.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out MtlTtlUuid);
                if (!parsedMtlTtlUuid)
                {
                    parseError = "MtlTtlUuid";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMtlTtlUuid(MtlTtlUuid);
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

        public class MtlTtlUuidRecord : INotifyPropertyChanged
        {
            public MtlTtlUuidRecord()
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

            private string _MtlTtlUuid;
            public string MtlTtlUuid { get { return _MtlTtlUuid; } set { if (value == _MtlTtlUuid) return; _MtlTtlUuid = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MtlTtlUuidRecord> MtlTtlUuidRecordData { get; } = new DataCollection<MtlTtlUuidRecord>();
        private void OnMtlTtlUuid_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MtlTtlUuidRecordData.Count == 0)
                {
                    MtlTtlUuidRecordData.AddRecord(new MtlTtlUuidRecord());
                }
                MtlTtlUuidRecordData[MtlTtlUuidRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMtlTtlUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MtlTtlUuidRecordData.MaxLength = value;


        }

        private void OnAlgorithmMtlTtlUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MtlTtlUuidRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMtlTtlUuid(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MtlTtlUuid,Notes\n");
            foreach (var row in MtlTtlUuidRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MtlTtlUuid},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMtlTtlUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMtlTtlUuid();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MtlTtlUuid");
                    return;
                }

                var record = new MtlTtlUuidRecord();

                var MtlTtlUuid = valueList.GetValue("MtlTtlUuid");
                if (MtlTtlUuid.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MtlTtlUuid.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MtlTtlUuid = (string)MtlTtlUuid.AsString;
                    MtlTtlUuid_MtlTtlUuid.Text = record.MtlTtlUuid.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MtlTtlUuidRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void OnWriteMeshAppearanceUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes MeshAppearanceUuid;
                var parsedMeshAppearanceUuid = Utilities.Parsers.TryParseBytes(MeshAppearanceUuid_MeshAppearanceUuid.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out MeshAppearanceUuid);
                if (!parsedMeshAppearanceUuid)
                {
                    parseError = "MeshAppearanceUuid";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMeshAppearanceUuid(MeshAppearanceUuid);
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

        public class MeshAppearanceUuidRecord : INotifyPropertyChanged
        {
            public MeshAppearanceUuidRecord()
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

            private string _MeshAppearanceUuid;
            public string MeshAppearanceUuid { get { return _MeshAppearanceUuid; } set { if (value == _MeshAppearanceUuid) return; _MeshAppearanceUuid = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MeshAppearanceUuidRecord> MeshAppearanceUuidRecordData { get; } = new DataCollection<MeshAppearanceUuidRecord>();
        private void OnMeshAppearanceUuid_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MeshAppearanceUuidRecordData.Count == 0)
                {
                    MeshAppearanceUuidRecordData.AddRecord(new MeshAppearanceUuidRecord());
                }
                MeshAppearanceUuidRecordData[MeshAppearanceUuidRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMeshAppearanceUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MeshAppearanceUuidRecordData.MaxLength = value;


        }

        private void OnAlgorithmMeshAppearanceUuid(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MeshAppearanceUuidRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMeshAppearanceUuid(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,MeshAppearanceUuid,Notes\n");
            foreach (var row in MeshAppearanceUuidRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.MeshAppearanceUuid},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMeshAppearanceUuid(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMeshAppearanceUuid();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MeshAppearanceUuid");
                    return;
                }

                var record = new MeshAppearanceUuidRecord();

                var MeshAppearanceUuid = valueList.GetValue("MeshAppearanceUuid");
                if (MeshAppearanceUuid.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MeshAppearanceUuid.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.MeshAppearanceUuid = (string)MeshAppearanceUuid.AsString;
                    MeshAppearanceUuid_MeshAppearanceUuid.Text = record.MeshAppearanceUuid.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MeshAppearanceUuidRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Device Info


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


        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(true);
            SetStatusActive(false);
        }
    }
}
