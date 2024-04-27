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
using Windows.UI.Xaml.Controls.Primitives;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[NAME]] device
    /// </summary>
    public sealed partial class PokitProMeterPage : Page, HasId, ISetHandleStatus
    {
        public PokitProMeterPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "PokitProMeter";
        private string DeviceNameUser = "PokitPro";

        int ncommand = 0;
        PokitProMeter bleDevice = new PokitProMeter();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive(false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            // CHANGE: remove this. Page should do nothing automatically (it's all in the multimeter):
            // await DoReadDevice_Name();

            //Change
            await uiMultiMeter.SetMeter(bleDevice);
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


        // OK to include this method even if there are no defined buttons
        private async void OnClickServer_Supported_Features(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteServer_Supported_Features(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteServer_Supported_Features(object sender, RoutedEventArgs e)
        {
            var text = Server_Supported_Features_FeatureBitmap0.Text;
            await DoWriteServer_Supported_Features(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteServer_Supported_Features(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte FeatureBitmap0;
                // History: used to go into Server_Supported_Features_FeatureBitmap0.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedFeatureBitmap0 = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out FeatureBitmap0);
                if (!parsedFeatureBitmap0)
                {
                    parseError = "FeatureBitmap0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteServer_Supported_Features(FeatureBitmap0);
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

        public class Server_Supported_FeaturesRecord : INotifyPropertyChanged
        {
            public Server_Supported_FeaturesRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _FeatureBitmap0;
            public double FeatureBitmap0 { get { return _FeatureBitmap0; } set { if (value == _FeatureBitmap0) return; _FeatureBitmap0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Server_Supported_FeaturesRecord> Server_Supported_FeaturesRecordData { get; } = new DataCollection<Server_Supported_FeaturesRecord>();
        private void OnServer_Supported_Features_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Server_Supported_FeaturesRecordData.Count == 0)
                {
                    Server_Supported_FeaturesRecordData.AddRecord(new Server_Supported_FeaturesRecord());
                }
                Server_Supported_FeaturesRecordData[Server_Supported_FeaturesRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountServer_Supported_Features(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Server_Supported_FeaturesRecordData.MaxLength = value;


        }

        private void OnAlgorithmServer_Supported_Features(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Server_Supported_FeaturesRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyServer_Supported_Features(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,FeatureBitmap0,Notes\n");
            foreach (var row in Server_Supported_FeaturesRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.FeatureBitmap0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadServer_Supported_Features(object sender, RoutedEventArgs e)
        {
            await DoReadServer_Supported_Features();
        }

        private async Task DoReadServer_Supported_Features()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadServer_Supported_Features();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Server_Supported_Features");
                    return;
                }

                var record = new Server_Supported_FeaturesRecord();

                var FeatureBitmap0 = valueList.GetValue("FeatureBitmap0");
                if (FeatureBitmap0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || FeatureBitmap0.CurrentType == BCBasic.BCValue.ValueType.IsString || FeatureBitmap0.IsArray)
                {
                    record.FeatureBitmap0 = (double)FeatureBitmap0.AsDouble;
                    Server_Supported_Features_FeatureBitmap0.Text = record.FeatureBitmap0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Server_Supported_FeaturesRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        // OK to include this method even if there are no defined buttons
        private async void OnClickClient_Supported_Features(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteClient_Supported_Features(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteClient_Supported_Features(object sender, RoutedEventArgs e)
        {
            var text = Client_Supported_Features_FeatureBitmap0.Text;
            await DoWriteClient_Supported_Features(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteClient_Supported_Features(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte FeatureBitmap0;
                // History: used to go into Client_Supported_Features_FeatureBitmap0.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedFeatureBitmap0 = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out FeatureBitmap0);
                if (!parsedFeatureBitmap0)
                {
                    parseError = "FeatureBitmap0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteClient_Supported_Features(FeatureBitmap0);
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

        public class Client_Supported_FeaturesRecord : INotifyPropertyChanged
        {
            public Client_Supported_FeaturesRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _FeatureBitmap0;
            public double FeatureBitmap0 { get { return _FeatureBitmap0; } set { if (value == _FeatureBitmap0) return; _FeatureBitmap0 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Client_Supported_FeaturesRecord> Client_Supported_FeaturesRecordData { get; } = new DataCollection<Client_Supported_FeaturesRecord>();
        private void OnClient_Supported_Features_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Client_Supported_FeaturesRecordData.Count == 0)
                {
                    Client_Supported_FeaturesRecordData.AddRecord(new Client_Supported_FeaturesRecord());
                }
                Client_Supported_FeaturesRecordData[Client_Supported_FeaturesRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountClient_Supported_Features(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Client_Supported_FeaturesRecordData.MaxLength = value;


        }

        private void OnAlgorithmClient_Supported_Features(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Client_Supported_FeaturesRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyClient_Supported_Features(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,FeatureBitmap0,Notes\n");
            foreach (var row in Client_Supported_FeaturesRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.FeatureBitmap0},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadClient_Supported_Features(object sender, RoutedEventArgs e)
        {
            await DoReadClient_Supported_Features();
        }

        private async Task DoReadClient_Supported_Features()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadClient_Supported_Features();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Client_Supported_Features");
                    return;
                }

                var record = new Client_Supported_FeaturesRecord();

                var FeatureBitmap0 = valueList.GetValue("FeatureBitmap0");
                if (FeatureBitmap0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || FeatureBitmap0.CurrentType == BCBasic.BCValue.ValueType.IsString || FeatureBitmap0.IsArray)
                {
                    record.FeatureBitmap0 = (double)FeatureBitmap0.AsDouble;
                    Client_Supported_Features_FeatureBitmap0.Text = record.FeatureBitmap0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Client_Supported_FeaturesRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Database_HashRecord : INotifyPropertyChanged
        {
            public Database_HashRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Hash0;
            public double Hash0 { get { return _Hash0; } set { if (value == _Hash0) return; _Hash0 = value; OnPropertyChanged(); } }

            private double _Hash1;
            public double Hash1 { get { return _Hash1; } set { if (value == _Hash1) return; _Hash1 = value; OnPropertyChanged(); } }

            private double _Hash2;
            public double Hash2 { get { return _Hash2; } set { if (value == _Hash2) return; _Hash2 = value; OnPropertyChanged(); } }

            private double _Hash3;
            public double Hash3 { get { return _Hash3; } set { if (value == _Hash3) return; _Hash3 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Database_HashRecord> Database_HashRecordData { get; } = new DataCollection<Database_HashRecord>();
        private void OnDatabase_Hash_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Database_HashRecordData.Count == 0)
                {
                    Database_HashRecordData.AddRecord(new Database_HashRecord());
                }
                Database_HashRecordData[Database_HashRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDatabase_Hash(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Database_HashRecordData.MaxLength = value;


        }

        private void OnAlgorithmDatabase_Hash(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Database_HashRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDatabase_Hash(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Hash0,Hash1,Hash2,Hash3,Notes\n");
            foreach (var row in Database_HashRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Hash0},{row.Hash1},{row.Hash2},{row.Hash3},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadDatabase_Hash(object sender, RoutedEventArgs e)
        {
            await DoReadDatabase_Hash();
        }

        private async Task DoReadDatabase_Hash()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDatabase_Hash();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Database_Hash");
                    return;
                }

                var record = new Database_HashRecord();

                var Hash0 = valueList.GetValue("Hash0");
                if (Hash0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Hash0.CurrentType == BCBasic.BCValue.ValueType.IsString || Hash0.IsArray)
                {
                    record.Hash0 = (double)Hash0.AsDouble;
                    Database_Hash_Hash0.Text = record.Hash0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Hash1 = valueList.GetValue("Hash1");
                if (Hash1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Hash1.CurrentType == BCBasic.BCValue.ValueType.IsString || Hash1.IsArray)
                {
                    record.Hash1 = (double)Hash1.AsDouble;
                    Database_Hash_Hash1.Text = record.Hash1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Hash2 = valueList.GetValue("Hash2");
                if (Hash2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Hash2.CurrentType == BCBasic.BCValue.ValueType.IsString || Hash2.IsArray)
                {
                    record.Hash2 = (double)Hash2.AsDouble;
                    Database_Hash_Hash2.Text = record.Hash2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Hash3 = valueList.GetValue("Hash3");
                if (Hash3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Hash3.CurrentType == BCBasic.BCValue.ValueType.IsString || Hash3.IsArray)
                {
                    record.Hash3 = (double)Hash3.AsDouble;
                    Database_Hash_Hash3.Text = record.Hash3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Database_HashRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
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
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString || Device_Name.IsArray)
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
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                if (Appearance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Appearance.CurrentType == BCBasic.BCValue.ValueType.IsString || Appearance.IsArray)
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


        // Functions for Device Info


        public class Manufacturer_NameRecord : INotifyPropertyChanged
        {
            public Manufacturer_NameRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadManufacturer_Name();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Manufacturer_Name");
                    return;
                }

                var record = new Manufacturer_NameRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Manufacturer_Name_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Manufacturer_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
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
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
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

        public class Firmware_RevisionRecord : INotifyPropertyChanged
        {
            public Firmware_RevisionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
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

        public class Software_RevisionRecord : INotifyPropertyChanged
        {
            public Software_RevisionRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadSoftware_Revision();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Software_Revision");
                    return;
                }

                var record = new Software_RevisionRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Software_Revision_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Software_RevisionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
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
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadHardware_Revision();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Hardware_Revision");
                    return;
                }

                var record = new Hardware_RevisionRecord();

                var param0 = valueList.GetValue("param0");
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
                {
                    record.param0 = (string)param0.AsString;
                    Hardware_Revision_param0.Text = record.param0.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Hardware_RevisionRecordData.Add(record);

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
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
                if (param0.CurrentType == BCBasic.BCValue.ValueType.IsDouble || param0.CurrentType == BCBasic.BCValue.ValueType.IsString || param0.IsArray)
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


        // Functions for Service_Status


        public class Status_DeviceRecord : INotifyPropertyChanged
        {
            public Status_DeviceRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _FirmwareMajor;
            public double FirmwareMajor { get { return _FirmwareMajor; } set { if (value == _FirmwareMajor) return; _FirmwareMajor = value; OnPropertyChanged(); } }

            private double _FirmwareMinor;
            public double FirmwareMinor { get { return _FirmwareMinor; } set { if (value == _FirmwareMinor) return; _FirmwareMinor = value; OnPropertyChanged(); } }

            private double _MaxInputVoltage;
            public double MaxInputVoltage { get { return _MaxInputVoltage; } set { if (value == _MaxInputVoltage) return; _MaxInputVoltage = value; OnPropertyChanged(); } }

            private double _MaxInputCurrent;
            public double MaxInputCurrent { get { return _MaxInputCurrent; } set { if (value == _MaxInputCurrent) return; _MaxInputCurrent = value; OnPropertyChanged(); } }

            private double _MaxInputResistance;
            public double MaxInputResistance { get { return _MaxInputResistance; } set { if (value == _MaxInputResistance) return; _MaxInputResistance = value; OnPropertyChanged(); } }

            private double _MaxSamplingRate;
            public double MaxSamplingRate { get { return _MaxSamplingRate; } set { if (value == _MaxSamplingRate) return; _MaxSamplingRate = value; OnPropertyChanged(); } }

            private double _DeviceBufferSize;
            public double DeviceBufferSize { get { return _DeviceBufferSize; } set { if (value == _DeviceBufferSize) return; _DeviceBufferSize = value; OnPropertyChanged(); } }

            private double _Reserved01;
            public double Reserved01 { get { return _Reserved01; } set { if (value == _Reserved01) return; _Reserved01 = value; OnPropertyChanged(); } }

            private string _MacAddress;
            public string MacAddress { get { return _MacAddress; } set { if (value == _MacAddress) return; _MacAddress = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Status_DeviceRecord> Status_DeviceRecordData { get; } = new DataCollection<Status_DeviceRecord>();
        private void OnStatus_Device_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Status_DeviceRecordData.Count == 0)
                {
                    Status_DeviceRecordData.AddRecord(new Status_DeviceRecord());
                }
                Status_DeviceRecordData[Status_DeviceRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountStatus_Device(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_DeviceRecordData.MaxLength = value;


        }

        private void OnAlgorithmStatus_Device(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_DeviceRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyStatus_Device(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,FirmwareMajor,FirmwareMinor,MaxInputVoltage,MaxInputCurrent,MaxInputResistance,MaxSamplingRate,DeviceBufferSize,Reserved01,MacAddress,Notes\n");
            foreach (var row in Status_DeviceRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.FirmwareMajor},{row.FirmwareMinor},{row.MaxInputVoltage},{row.MaxInputCurrent},{row.MaxInputResistance},{row.MaxSamplingRate},{row.DeviceBufferSize},{row.Reserved01},{row.MacAddress},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadStatus_Device(object sender, RoutedEventArgs e)
        {
            await DoReadStatus_Device();
        }

        private async Task DoReadStatus_Device()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadStatus_Device();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Device");
                    return;
                }

                var record = new Status_DeviceRecord();

                var FirmwareMajor = valueList.GetValue("FirmwareMajor");
                if (FirmwareMajor.CurrentType == BCBasic.BCValue.ValueType.IsDouble || FirmwareMajor.CurrentType == BCBasic.BCValue.ValueType.IsString || FirmwareMajor.IsArray)
                {
                    record.FirmwareMajor = (double)FirmwareMajor.AsDouble;
                    Status_Device_FirmwareMajor.Text = record.FirmwareMajor.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var FirmwareMinor = valueList.GetValue("FirmwareMinor");
                if (FirmwareMinor.CurrentType == BCBasic.BCValue.ValueType.IsDouble || FirmwareMinor.CurrentType == BCBasic.BCValue.ValueType.IsString || FirmwareMinor.IsArray)
                {
                    record.FirmwareMinor = (double)FirmwareMinor.AsDouble;
                    Status_Device_FirmwareMinor.Text = record.FirmwareMinor.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MaxInputVoltage = valueList.GetValue("MaxInputVoltage");
                if (MaxInputVoltage.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxInputVoltage.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxInputVoltage.IsArray)
                {
                    record.MaxInputVoltage = (double)MaxInputVoltage.AsDouble;
                    Status_Device_MaxInputVoltage.Text = record.MaxInputVoltage.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MaxInputCurrent = valueList.GetValue("MaxInputCurrent");
                if (MaxInputCurrent.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxInputCurrent.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxInputCurrent.IsArray)
                {
                    record.MaxInputCurrent = (double)MaxInputCurrent.AsDouble;
                    Status_Device_MaxInputCurrent.Text = record.MaxInputCurrent.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MaxInputResistance = valueList.GetValue("MaxInputResistance");
                if (MaxInputResistance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxInputResistance.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxInputResistance.IsArray)
                {
                    record.MaxInputResistance = (double)MaxInputResistance.AsDouble;
                    Status_Device_MaxInputResistance.Text = record.MaxInputResistance.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MaxSamplingRate = valueList.GetValue("MaxSamplingRate");
                if (MaxSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxSamplingRate.IsArray)
                {
                    record.MaxSamplingRate = (double)MaxSamplingRate.AsDouble;
                    Status_Device_MaxSamplingRate.Text = record.MaxSamplingRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DeviceBufferSize = valueList.GetValue("DeviceBufferSize");
                if (DeviceBufferSize.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DeviceBufferSize.CurrentType == BCBasic.BCValue.ValueType.IsString || DeviceBufferSize.IsArray)
                {
                    record.DeviceBufferSize = (double)DeviceBufferSize.AsDouble;
                    Status_Device_DeviceBufferSize.Text = record.DeviceBufferSize.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Reserved01 = valueList.GetValue("Reserved01");
                if (Reserved01.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Reserved01.CurrentType == BCBasic.BCValue.ValueType.IsString || Reserved01.IsArray)
                {
                    record.Reserved01 = (double)Reserved01.AsDouble;
                    Status_Device_Reserved01.Text = record.Reserved01.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var MacAddress = valueList.GetValue("MacAddress");
                if (MacAddress.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MacAddress.CurrentType == BCBasic.BCValue.ValueType.IsString || MacAddress.IsArray)
                {
                    record.MacAddress = (string)MacAddress.AsString;
                    Status_Device_MacAddress.Text = record.MacAddress.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Status_DeviceRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        public class Status_StatusRecord : INotifyPropertyChanged
        {
            public Status_StatusRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _DeviceStatus;
            public double DeviceStatus { get { return _DeviceStatus; } set { if (value == _DeviceStatus) return; _DeviceStatus = value; OnPropertyChanged(); } }

            private double _BatteryLevel;
            public double BatteryLevel { get { return _BatteryLevel; } set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Status_StatusRecord> Status_StatusRecordData { get; } = new DataCollection<Status_StatusRecord>();
        private void OnStatus_Status_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Status_StatusRecordData.Count == 0)
                {
                    Status_StatusRecordData.AddRecord(new Status_StatusRecord());
                }
                Status_StatusRecordData[Status_StatusRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountStatus_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_StatusRecordData.MaxLength = value;


        }

        private void OnAlgorithmStatus_Status(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_StatusRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyStatus_Status(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DeviceStatus,BatteryLevel,Notes\n");
            foreach (var row in Status_StatusRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DeviceStatus},{row.BatteryLevel},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadStatus_Status(object sender, RoutedEventArgs e)
        {
            await DoReadStatus_Status();
        }

        private async Task DoReadStatus_Status()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadStatus_Status();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Status");
                    return;
                }

                var record = new Status_StatusRecord();

                var DeviceStatus = valueList.GetValue("DeviceStatus");
                if (DeviceStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DeviceStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DeviceStatus.IsArray)
                {
                    record.DeviceStatus = (double)DeviceStatus.AsDouble;
                    Status_Status_DeviceStatus.Text = record.DeviceStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var BatteryLevel = valueList.GetValue("BatteryLevel");
                if (BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsString || BatteryLevel.IsArray)
                {
                    record.BatteryLevel = (double)BatteryLevel.AsDouble;
                    Status_Status_BatteryLevel.Text = record.BatteryLevel.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Status_StatusRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyStatus_StatusSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Status_StatusNotifyIndex = 0;
        bool Status_StatusNotifySetup = false;
        private async void OnNotifyStatus_Status(object sender, RoutedEventArgs e)
        {
            await DoNotifyStatus_Status();
        }

        private async Task DoNotifyStatus_Status()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Status_StatusNotifySetup)
                {
                    Status_StatusNotifySetup = true;
                    bleDevice.Status_StatusEvent += BleDevice_Status_StatusEvent;
                }
                var notifyType = NotifyStatus_StatusSettings[Status_StatusNotifyIndex];
                Status_StatusNotifyIndex = (Status_StatusNotifyIndex + 1) % NotifyStatus_StatusSettings.Length;
                var result = await bleDevice.NotifyStatus_StatusAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Status_StatusEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Status_StatusRecord();

                    var DeviceStatus = valueList.GetValue("DeviceStatus");
                    if (DeviceStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DeviceStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DeviceStatus.IsArray)
                    {
                        record.DeviceStatus = (double)DeviceStatus.AsDouble;
                        Status_Status_DeviceStatus.Text = record.DeviceStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var BatteryLevel = valueList.GetValue("BatteryLevel");
                    if (BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsString || BatteryLevel.IsArray)
                    {
                        record.BatteryLevel = (double)BatteryLevel.AsDouble;
                        Status_Status_BatteryLevel.Text = record.BatteryLevel.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Status_StatusRecordData.AddRecord(record);

                });
            }
        }
        // OK to include this method even if there are no defined buttons
        private async void OnClickStatus_Device_Name(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteStatus_Device_Name(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteStatus_Device_Name(object sender, RoutedEventArgs e)
        {
            var text = Status_Device_Name_Device_Name.Text;
            await DoWriteStatus_Device_Name(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteStatus_Device_Name(string text, System.Globalization.NumberStyles dec_or_hex)
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
                // History: used to go into Status_Device_Name_Device_Name.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDevice_Name = Utilities.Parsers.TryParseString(text, dec_or_hex, null, out Device_Name);
                if (!parsedDevice_Name)
                {
                    parseError = "Device Name";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteStatus_Device_Name(Device_Name);
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

        public class Status_Device_NameRecord : INotifyPropertyChanged
        {
            public Status_Device_NameRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

        public DataCollection<Status_Device_NameRecord> Status_Device_NameRecordData { get; } = new DataCollection<Status_Device_NameRecord>();
        private void OnStatus_Device_Name_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Status_Device_NameRecordData.Count == 0)
                {
                    Status_Device_NameRecordData.AddRecord(new Status_Device_NameRecord());
                }
                Status_Device_NameRecordData[Status_Device_NameRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountStatus_Device_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_Device_NameRecordData.MaxLength = value;


        }

        private void OnAlgorithmStatus_Device_Name(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_Device_NameRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyStatus_Device_Name(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Device_Name,Notes\n");
            foreach (var row in Status_Device_NameRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Device_Name},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadStatus_Device_Name(object sender, RoutedEventArgs e)
        {
            await DoReadStatus_Device_Name();
        }

        private async Task DoReadStatus_Device_Name()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadStatus_Device_Name();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Device_Name");
                    return;
                }

                var record = new Status_Device_NameRecord();

                var Device_Name = valueList.GetValue("Device_Name");
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString || Device_Name.IsArray)
                {
                    record.Device_Name = (string)Device_Name.AsString;
                    Status_Device_Name_Device_Name.Text = record.Device_Name.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Status_Device_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        // OK to include this method even if there are no defined buttons
        private async void OnClickStatus_Flash_LED(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteStatus_Flash_LED(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteStatus_Flash_LED(object sender, RoutedEventArgs e)
        {
            var text = Status_Flash_LED_Beep.Text;
            await DoWriteStatus_Flash_LED(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteStatus_Flash_LED(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Red;
                // History: used to go into Status_Flash_LED_Red.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedRed = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Red);
                if (!parsedRed)
                {
                    parseError = "Red";
                }

                Byte Green;
                // History: used to go into Status_Flash_LED_Green.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedGreen = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Green);
                if (!parsedGreen)
                {
                    parseError = "Green";
                }

                Byte Blue;
                // History: used to go into Status_Flash_LED_Blue.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedBlue = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Blue);
                if (!parsedBlue)
                {
                    parseError = "Blue";
                }

                Byte Beep;
                // History: used to go into Status_Flash_LED_Beep.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedBeep = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Beep);
                if (!parsedBeep)
                {
                    parseError = "Beep";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteStatus_Flash_LED(Red, Green, Blue, Beep);
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

        public class Status_Flash_LEDRecord : INotifyPropertyChanged
        {
            public Status_Flash_LEDRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Red;
            public double Red { get { return _Red; } set { if (value == _Red) return; _Red = value; OnPropertyChanged(); } }

            private double _Green;
            public double Green { get { return _Green; } set { if (value == _Green) return; _Green = value; OnPropertyChanged(); } }

            private double _Blue;
            public double Blue { get { return _Blue; } set { if (value == _Blue) return; _Blue = value; OnPropertyChanged(); } }

            private double _Beep;
            public double Beep { get { return _Beep; } set { if (value == _Beep) return; _Beep = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Status_Flash_LEDRecord> Status_Flash_LEDRecordData { get; } = new DataCollection<Status_Flash_LEDRecord>();
        private void OnStatus_Flash_LED_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Status_Flash_LEDRecordData.Count == 0)
                {
                    Status_Flash_LEDRecordData.AddRecord(new Status_Flash_LEDRecord());
                }
                Status_Flash_LEDRecordData[Status_Flash_LEDRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountStatus_Flash_LED(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_Flash_LEDRecordData.MaxLength = value;


        }

        private void OnAlgorithmStatus_Flash_LED(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_Flash_LEDRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyStatus_Flash_LED(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Red,Green,Blue,Beep,Notes\n");
            foreach (var row in Status_Flash_LEDRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Red},{row.Green},{row.Blue},{row.Beep},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadStatus_Flash_LED(object sender, RoutedEventArgs e)
        {
            await DoReadStatus_Flash_LED();
        }

        private async Task DoReadStatus_Flash_LED()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadStatus_Flash_LED();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Flash_LED");
                    return;
                }

                var record = new Status_Flash_LEDRecord();

                var Red = valueList.GetValue("Red");
                if (Red.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Red.CurrentType == BCBasic.BCValue.ValueType.IsString || Red.IsArray)
                {
                    record.Red = (double)Red.AsDouble;
                    Status_Flash_LED_Red.Text = record.Red.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Green = valueList.GetValue("Green");
                if (Green.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Green.CurrentType == BCBasic.BCValue.ValueType.IsString || Green.IsArray)
                {
                    record.Green = (double)Green.AsDouble;
                    Status_Flash_LED_Green.Text = record.Green.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Blue = valueList.GetValue("Blue");
                if (Blue.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Blue.CurrentType == BCBasic.BCValue.ValueType.IsString || Blue.IsArray)
                {
                    record.Blue = (double)Blue.AsDouble;
                    Status_Flash_LED_Blue.Text = record.Blue.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Beep = valueList.GetValue("Beep");
                if (Beep.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Beep.CurrentType == BCBasic.BCValue.ValueType.IsString || Beep.IsArray)
                {
                    record.Beep = (double)Beep.AsDouble;
                    Status_Flash_LED_Beep.Text = record.Beep.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Status_Flash_LEDRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        // OK to include this method even if there are no defined buttons
        private async void OnClickStatus_Light_LED(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteStatus_Light_LED(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteStatus_Light_LED(object sender, RoutedEventArgs e)
        {
            var text = Status_Light_LED_Light.Text;
            await DoWriteStatus_Light_LED(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteStatus_Light_LED(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Light;
                // History: used to go into Status_Light_LED_Light.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedLight = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out Light);
                if (!parsedLight)
                {
                    parseError = "Light";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteStatus_Light_LED(Light);
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

        public class Status_Light_LEDRecord : INotifyPropertyChanged
        {
            public Status_Light_LEDRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Light;
            public double Light { get { return _Light; } set { if (value == _Light) return; _Light = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Status_Light_LEDRecord> Status_Light_LEDRecordData { get; } = new DataCollection<Status_Light_LEDRecord>();
        private void OnStatus_Light_LED_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Status_Light_LEDRecordData.Count == 0)
                {
                    Status_Light_LEDRecordData.AddRecord(new Status_Light_LEDRecord());
                }
                Status_Light_LEDRecordData[Status_Light_LEDRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountStatus_Light_LED(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_Light_LEDRecordData.MaxLength = value;


        }

        private void OnAlgorithmStatus_Light_LED(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Status_Light_LEDRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyStatus_Light_LED(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Light,Notes\n");
            foreach (var row in Status_Light_LEDRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Light},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadStatus_Light_LED(object sender, RoutedEventArgs e)
        {
            await DoReadStatus_Light_LED();
        }

        private async Task DoReadStatus_Light_LED()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadStatus_Light_LED();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Light_LED");
                    return;
                }

                var record = new Status_Light_LEDRecord();

                var Light = valueList.GetValue("Light");
                if (Light.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Light.CurrentType == BCBasic.BCValue.ValueType.IsString || Light.IsArray)
                {
                    record.Light = (double)Light.AsDouble;
                    Status_Light_LED_Light.Text = record.Light.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Status_Light_LEDRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyStatus_Light_LEDSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int Status_Light_LEDNotifyIndex = 0;
        bool Status_Light_LEDNotifySetup = false;
        private async void OnNotifyStatus_Light_LED(object sender, RoutedEventArgs e)
        {
            await DoNotifyStatus_Light_LED();
        }

        private async Task DoNotifyStatus_Light_LED()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!Status_Light_LEDNotifySetup)
                {
                    Status_Light_LEDNotifySetup = true;
                    bleDevice.Status_Light_LEDEvent += BleDevice_Status_Light_LEDEvent;
                }
                var notifyType = NotifyStatus_Light_LEDSettings[Status_Light_LEDNotifyIndex];
                Status_Light_LEDNotifyIndex = (Status_Light_LEDNotifyIndex + 1) % NotifyStatus_Light_LEDSettings.Length;
                var result = await bleDevice.NotifyStatus_Light_LEDAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_Status_Light_LEDEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new Status_Light_LEDRecord();

                    var Light = valueList.GetValue("Light");
                    if (Light.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Light.CurrentType == BCBasic.BCValue.ValueType.IsString || Light.IsArray)
                    {
                        record.Light = (double)Light.AsDouble;
                        Status_Light_LED_Light.Text = record.Light.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = Status_Light_LEDRecordData.AddRecord(record);

                });
            }
        }
        // OK to include this method even if there are no defined buttons
        private async void OnClickUnknownStatusValues(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteUnknownStatusValues(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteUnknownStatusValues(object sender, RoutedEventArgs e)
        {
            var text = UnknownStatusValues_StatusUnknown5.Text;
            await DoWriteUnknownStatusValues(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteUnknownStatusValues(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes StatusUnknown5;
                // History: used to go into UnknownStatusValues_StatusUnknown5.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedStatusUnknown5 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out StatusUnknown5);
                if (!parsedStatusUnknown5)
                {
                    parseError = "StatusUnknown5";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteUnknownStatusValues(StatusUnknown5);
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

        public class UnknownStatusValuesRecord : INotifyPropertyChanged
        {
            public UnknownStatusValuesRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _StatusUnknown5;
            public string StatusUnknown5 { get { return _StatusUnknown5; } set { if (value == _StatusUnknown5) return; _StatusUnknown5 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<UnknownStatusValuesRecord> UnknownStatusValuesRecordData { get; } = new DataCollection<UnknownStatusValuesRecord>();
        private void OnUnknownStatusValues_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (UnknownStatusValuesRecordData.Count == 0)
                {
                    UnknownStatusValuesRecordData.AddRecord(new UnknownStatusValuesRecord());
                }
                UnknownStatusValuesRecordData[UnknownStatusValuesRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountUnknownStatusValues(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            UnknownStatusValuesRecordData.MaxLength = value;


        }

        private void OnAlgorithmUnknownStatusValues(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            UnknownStatusValuesRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyUnknownStatusValues(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,StatusUnknown5,Notes\n");
            foreach (var row in UnknownStatusValuesRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.StatusUnknown5},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadUnknownStatusValues(object sender, RoutedEventArgs e)
        {
            await DoReadUnknownStatusValues();
        }

        private async Task DoReadUnknownStatusValues()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadUnknownStatusValues();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read UnknownStatusValues");
                    return;
                }

                var record = new UnknownStatusValuesRecord();

                var StatusUnknown5 = valueList.GetValue("StatusUnknown5");
                if (StatusUnknown5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || StatusUnknown5.CurrentType == BCBasic.BCValue.ValueType.IsString || StatusUnknown5.IsArray)
                {
                    record.StatusUnknown5 = (string)StatusUnknown5.AsString;
                    UnknownStatusValues_StatusUnknown5.Text = record.StatusUnknown5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                UnknownStatusValuesRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyUnknownStatusValuesSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int UnknownStatusValuesNotifyIndex = 0;
        bool UnknownStatusValuesNotifySetup = false;
        private async void OnNotifyUnknownStatusValues(object sender, RoutedEventArgs e)
        {
            await DoNotifyUnknownStatusValues();
        }

        private async Task DoNotifyUnknownStatusValues()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!UnknownStatusValuesNotifySetup)
                {
                    UnknownStatusValuesNotifySetup = true;
                    bleDevice.UnknownStatusValuesEvent += BleDevice_UnknownStatusValuesEvent;
                }
                var notifyType = NotifyUnknownStatusValuesSettings[UnknownStatusValuesNotifyIndex];
                UnknownStatusValuesNotifyIndex = (UnknownStatusValuesNotifyIndex + 1) % NotifyUnknownStatusValuesSettings.Length;
                var result = await bleDevice.NotifyUnknownStatusValuesAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_UnknownStatusValuesEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new UnknownStatusValuesRecord();

                    var StatusUnknown5 = valueList.GetValue("StatusUnknown5");
                    if (StatusUnknown5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || StatusUnknown5.CurrentType == BCBasic.BCValue.ValueType.IsString || StatusUnknown5.IsArray)
                    {
                        record.StatusUnknown5 = (string)StatusUnknown5.AsString;
                        UnknownStatusValues_StatusUnknown5.Text = record.StatusUnknown5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = UnknownStatusValuesRecordData.AddRecord(record);

                });
            }
        }

        // Functions for Multimeter


        // OK to include this method even if there are no defined buttons
        private async void OnClickMM_Settings(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteMM_Settings(text, System.Globalization.NumberStyles.Integer, text, System.Globalization.NumberStyles.Integer, text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteMM_Settings(object sender, RoutedEventArgs e)
        {
            var modeText = MM_Settings_Mode.Text;
            var rangeText = MM_Settings_Range.Text;
            var intervalText = MM_Settings_Interval.Text;
            await DoWriteMM_Settings(modeText, System.Globalization.NumberStyles.AllowHexSpecifier,
                rangeText, System.Globalization.NumberStyles.AllowHexSpecifier,
                intervalText, System.Globalization.NumberStyles.Integer);
        }

        private async Task DoWriteMM_Settings(string modeText, System.Globalization.NumberStyles modeDec_or_hex, 
            string rangeText, System.Globalization.NumberStyles rangeDec_or_hex, string intervalText, System.Globalization.NumberStyles intervalDec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Mode;
                // History: used to go into MM_Settings_Mode.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedMode = Utilities.Parsers.TryParseByte(modeText, modeDec_or_hex, null, out Mode);
                if (!parsedMode)
                {
                    parseError = "Mode";
                }

                Byte Range;
                // History: used to go into MM_Settings_Range.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedRange = Utilities.Parsers.TryParseByte(rangeText, rangeDec_or_hex, null, out Range);
                if (!parsedRange)
                {
                    parseError = "Range";
                }

                UInt32 Interval;
                // History: used to go into MM_Settings_Interval.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedInterval = Utilities.Parsers.TryParseUInt32(intervalText, intervalDec_or_hex, null, out Interval);
                if (!parsedInterval)
                {
                    parseError = "Interval";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteMM_Settings(Mode, Range, Interval);
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

        public class MM_DataRecord : INotifyPropertyChanged
        {
            public MM_DataRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Status;
            public double Status { get { return _Status; } set { if (value == _Status) return; _Status = value; OnPropertyChanged(); } }

            private double _Data;
            public double Data { get { return _Data; } set { if (value == _Data) return; _Data = value; OnPropertyChanged(); } }

            private double _OperationMode;
            public double OperationMode { get { return _OperationMode; } set { if (value == _OperationMode) return; _OperationMode = value; OnPropertyChanged(); } }

            private double _Range;
            public double Range { get { return _Range; } set { if (value == _Range) return; _Range = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<MM_DataRecord> MM_DataRecordData { get; } = new DataCollection<MM_DataRecord>();
        private void OnMM_Data_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (MM_DataRecordData.Count == 0)
                {
                    MM_DataRecordData.AddRecord(new MM_DataRecord());
                }
                MM_DataRecordData[MM_DataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountMM_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MM_DataRecordData.MaxLength = value;


        }

        private void OnAlgorithmMM_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            MM_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyMM_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Status,Data,OperationMode,Range,Notes\n");
            foreach (var row in MM_DataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Status},{row.Data},{row.OperationMode},{row.Range},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadMM_Data(object sender, RoutedEventArgs e)
        {
            await DoReadMM_Data();
        }

        private async Task DoReadMM_Data()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadMM_Data();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read MM_Data");
                    return;
                }

                var record = new MM_DataRecord();

                var Status = valueList.GetValue("Status");
                if (Status.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Status.CurrentType == BCBasic.BCValue.ValueType.IsString || Status.IsArray)
                {
                    record.Status = (double)Status.AsDouble;
                    MM_Data_Status.Text = record.Status.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Data = valueList.GetValue("Data");
                if (Data.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Data.CurrentType == BCBasic.BCValue.ValueType.IsString || Data.IsArray)
                {
                    record.Data = (double)Data.AsDouble;
                    MM_Data_Data.Text = record.Data.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var OperationMode = valueList.GetValue("OperationMode");
                if (OperationMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || OperationMode.CurrentType == BCBasic.BCValue.ValueType.IsString || OperationMode.IsArray)
                {
                    record.OperationMode = (double)OperationMode.AsDouble;
                    MM_Data_OperationMode.Text = record.OperationMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Range = valueList.GetValue("Range");
                if (Range.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Range.CurrentType == BCBasic.BCValue.ValueType.IsString || Range.IsArray)
                {
                    record.Range = (double)Range.AsDouble;
                    MM_Data_Range.Text = record.Range.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                MM_DataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyMM_DataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int MM_DataNotifyIndex = 0;
        bool MM_DataNotifySetup = false;
        private async void OnNotifyMM_Data(object sender, RoutedEventArgs e)
        {
            await DoNotifyMM_Data();
        }

        private async Task DoNotifyMM_Data()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!MM_DataNotifySetup)
                {
                    MM_DataNotifySetup = true;
                    bleDevice.MM_DataEvent += BleDevice_MM_DataEvent;
                }
                var notifyType = NotifyMM_DataSettings[MM_DataNotifyIndex];
                MM_DataNotifyIndex = (MM_DataNotifyIndex + 1) % NotifyMM_DataSettings.Length;
                var result = await bleDevice.NotifyMM_DataAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_MM_DataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new MM_DataRecord();

                    var Status = valueList.GetValue("Status");
                    if (Status.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Status.CurrentType == BCBasic.BCValue.ValueType.IsString || Status.IsArray)
                    {
                        record.Status = (double)Status.AsDouble;
                        MM_Data_Status.Text = record.Status.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Data = valueList.GetValue("Data");
                    if (Data.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Data.CurrentType == BCBasic.BCValue.ValueType.IsString || Data.IsArray)
                    {
                        record.Data = (double)Data.AsDouble;
                        MM_Data_Data.Text = record.Data.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var OperationMode = valueList.GetValue("OperationMode");
                    if (OperationMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || OperationMode.CurrentType == BCBasic.BCValue.ValueType.IsString || OperationMode.IsArray)
                    {
                        record.OperationMode = (double)OperationMode.AsDouble;
                        MM_Data_OperationMode.Text = record.OperationMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Range = valueList.GetValue("Range");
                    if (Range.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Range.CurrentType == BCBasic.BCValue.ValueType.IsString || Range.IsArray)
                    {
                        record.Range = (double)Range.AsDouble;
                        MM_Data_Range.Text = record.Range.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = MM_DataRecordData.AddRecord(record);

                    bleDevice.HandleMMMessageCustom(); // CHANGE: divert for event processing

                });
            }
        }

        // Functions for DSO_Oscilloscope


        // OK to include this method even if there are no defined buttons
        private async void OnClickDSO_Settings(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteDSO_Settings(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteDSO_Settings(object sender, RoutedEventArgs e)
        {
            var text = DSO_Settings_DsoNSamples.Text;
            await DoWriteDSO_Settings(text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteDSO_Settings(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            // here!here -- this code is just plain wrong. It's taking a single text field and dec/hex flag and using
            // the same text for every field. It instead needs to take an array of text?
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte DsoTriggerType;
                // History: used to go into DSO_Settings_DsoTriggerType.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDsoTriggerType = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out DsoTriggerType);
                if (!parsedDsoTriggerType)
                {
                    parseError = "DsoTriggerType";
                }

                Single DsoTriggerLevel;
                // History: used to go into DSO_Settings_DsoTriggerLevel.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDsoTriggerLevel = Utilities.Parsers.TryParseSingle(text, dec_or_hex, null, out DsoTriggerLevel);
                if (!parsedDsoTriggerLevel)
                {
                    parseError = "DsoTriggerLevel";
                }

                Byte DsoMode;
                // History: used to go into DSO_Settings_DsoMode.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDsoMode = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out DsoMode);
                if (!parsedDsoMode)
                {
                    parseError = "DsoMode";
                }

                Byte DsoRange;
                // History: used to go into DSO_Settings_DsoRange.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDsoRange = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out DsoRange);
                if (!parsedDsoRange)
                {
                    parseError = "DsoRange";
                }

                UInt32 DsoSamplingWindow;
                // History: used to go into DSO_Settings_DsoSamplingWindow.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.None for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDsoSamplingWindow = Utilities.Parsers.TryParseUInt32(text, dec_or_hex, null, out DsoSamplingWindow);
                if (!parsedDsoSamplingWindow)
                {
                    parseError = "DsoSamplingWindow";
                }

                UInt16 DsoNSamples;
                // History: used to go into DSO_Settings_DsoNSamples.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.None for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDsoNSamples = Utilities.Parsers.TryParseUInt16(text, dec_or_hex, null, out DsoNSamples);
                if (!parsedDsoNSamples)
                {
                    parseError = "DsoNSamples";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteDSO_Settings(DsoTriggerType, DsoTriggerLevel, DsoMode, DsoRange, DsoSamplingWindow, DsoNSamples);
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

        public class DSO_ReadingRecord : INotifyPropertyChanged
        {
            public DSO_ReadingRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _DsoDataRaw;
            public string DsoDataRaw { get { return _DsoDataRaw; } set { if (value == _DsoDataRaw) return; _DsoDataRaw = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<DSO_ReadingRecord> DSO_ReadingRecordData { get; } = new DataCollection<DSO_ReadingRecord>();
        private void OnDSO_Reading_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (DSO_ReadingRecordData.Count == 0)
                {
                    DSO_ReadingRecordData.AddRecord(new DSO_ReadingRecord());
                }
                DSO_ReadingRecordData[DSO_ReadingRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDSO_Reading(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DSO_ReadingRecordData.MaxLength = value;


        }

        private void OnAlgorithmDSO_Reading(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DSO_ReadingRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDSO_Reading(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DsoDataRaw,Notes\n");
            foreach (var row in DSO_ReadingRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DsoDataRaw},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyDSO_ReadingSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int DSO_ReadingNotifyIndex = 0;
        bool DSO_ReadingNotifySetup = false;
        private async void OnNotifyDSO_Reading(object sender, RoutedEventArgs e)
        {
            await DoNotifyDSO_Reading();
        }

        private async Task DoNotifyDSO_Reading()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!DSO_ReadingNotifySetup)
                {
                    DSO_ReadingNotifySetup = true;
                    bleDevice.DSO_ReadingEvent += BleDevice_DSO_ReadingEvent;
                }
                var notifyType = NotifyDSO_ReadingSettings[DSO_ReadingNotifyIndex];
                DSO_ReadingNotifyIndex = (DSO_ReadingNotifyIndex + 1) % NotifyDSO_ReadingSettings.Length;
                var result = await bleDevice.NotifyDSO_ReadingAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_DSO_ReadingEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new DSO_ReadingRecord();

                    var DsoDataRaw = valueList.GetValue("DsoDataRaw");
                    if (DsoDataRaw.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataRaw.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataRaw.IsArray)
                    {
                        record.DsoDataRaw = (string)DsoDataRaw.AsString;
                        DSO_Reading_DsoDataRaw.Text = record.DsoDataRaw.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = DSO_ReadingRecordData.AddRecord(record);

                });
            }
        }
        public class DSO_MetadataRecord : INotifyPropertyChanged
        {
            public DSO_MetadataRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _DsoStatus;
            public double DsoStatus { get { return _DsoStatus; } set { if (value == _DsoStatus) return; _DsoStatus = value; OnPropertyChanged(); } }

            private double _DsoDataScale;
            public double DsoDataScale { get { return _DsoDataScale; } set { if (value == _DsoDataScale) return; _DsoDataScale = value; OnPropertyChanged(); } }

            private double _DsoDataMode;
            public double DsoDataMode { get { return _DsoDataMode; } set { if (value == _DsoDataMode) return; _DsoDataMode = value; OnPropertyChanged(); } }

            private double _DsoDataRange;
            public double DsoDataRange { get { return _DsoDataRange; } set { if (value == _DsoDataRange) return; _DsoDataRange = value; OnPropertyChanged(); } }

            private double _DsoDataSamplingWindow;
            public double DsoDataSamplingWindow { get { return _DsoDataSamplingWindow; } set { if (value == _DsoDataSamplingWindow) return; _DsoDataSamplingWindow = value; OnPropertyChanged(); } }

            private double _DsoDataNsamples;
            public double DsoDataNsamples { get { return _DsoDataNsamples; } set { if (value == _DsoDataNsamples) return; _DsoDataNsamples = value; OnPropertyChanged(); } }

            private double _DsoSamplingRate;
            public double DsoSamplingRate { get { return _DsoSamplingRate; } set { if (value == _DsoSamplingRate) return; _DsoSamplingRate = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<DSO_MetadataRecord> DSO_MetadataRecordData { get; } = new DataCollection<DSO_MetadataRecord>();
        private void OnDSO_Metadata_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (DSO_MetadataRecordData.Count == 0)
                {
                    DSO_MetadataRecordData.AddRecord(new DSO_MetadataRecord());
                }
                DSO_MetadataRecordData[DSO_MetadataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDSO_Metadata(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DSO_MetadataRecordData.MaxLength = value;


        }

        private void OnAlgorithmDSO_Metadata(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DSO_MetadataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDSO_Metadata(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DsoStatus,DsoDataScale,DsoDataMode,DsoDataRange,DsoDataSamplingWindow,DsoDataNsamples,DsoSamplingRate,Notes\n");
            foreach (var row in DSO_MetadataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DsoStatus},{row.DsoDataScale},{row.DsoDataMode},{row.DsoDataRange},{row.DsoDataSamplingWindow},{row.DsoDataNsamples},{row.DsoSamplingRate},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadDSO_Metadata(object sender, RoutedEventArgs e)
        {
            await DoReadDSO_Metadata();
        }

        private async Task DoReadDSO_Metadata()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDSO_Metadata();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read DSO_Metadata");
                    return;
                }

                var record = new DSO_MetadataRecord();

                var DsoStatus = valueList.GetValue("DsoStatus");
                if (DsoStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoStatus.IsArray)
                {
                    record.DsoStatus = (double)DsoStatus.AsDouble;
                    DSO_Metadata_DsoStatus.Text = record.DsoStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DsoDataScale = valueList.GetValue("DsoDataScale");
                if (DsoDataScale.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataScale.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataScale.IsArray)
                {
                    record.DsoDataScale = (double)DsoDataScale.AsDouble;
                    DSO_Metadata_DsoDataScale.Text = record.DsoDataScale.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DsoDataMode = valueList.GetValue("DsoDataMode");
                if (DsoDataMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataMode.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataMode.IsArray)
                {
                    record.DsoDataMode = (double)DsoDataMode.AsDouble;
                    DSO_Metadata_DsoDataMode.Text = record.DsoDataMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DsoDataRange = valueList.GetValue("DsoDataRange");
                if (DsoDataRange.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataRange.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataRange.IsArray)
                {
                    record.DsoDataRange = (double)DsoDataRange.AsDouble;
                    DSO_Metadata_DsoDataRange.Text = record.DsoDataRange.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DsoDataSamplingWindow = valueList.GetValue("DsoDataSamplingWindow");
                if (DsoDataSamplingWindow.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataSamplingWindow.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataSamplingWindow.IsArray)
                {
                    record.DsoDataSamplingWindow = (double)DsoDataSamplingWindow.AsDouble;
                    DSO_Metadata_DsoDataSamplingWindow.Text = record.DsoDataSamplingWindow.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DsoDataNsamples = valueList.GetValue("DsoDataNsamples");
                if (DsoDataNsamples.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataNsamples.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataNsamples.IsArray)
                {
                    record.DsoDataNsamples = (double)DsoDataNsamples.AsDouble;
                    DSO_Metadata_DsoDataNsamples.Text = record.DsoDataNsamples.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DsoSamplingRate = valueList.GetValue("DsoSamplingRate");
                if (DsoSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoSamplingRate.IsArray)
                {
                    record.DsoSamplingRate = (double)DsoSamplingRate.AsDouble;
                    DSO_Metadata_DsoSamplingRate.Text = record.DsoSamplingRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                DSO_MetadataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyDSO_MetadataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int DSO_MetadataNotifyIndex = 0;
        bool DSO_MetadataNotifySetup = false;
        private async void OnNotifyDSO_Metadata(object sender, RoutedEventArgs e)
        {
            await DoNotifyDSO_Metadata();
        }

        private async Task DoNotifyDSO_Metadata()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!DSO_MetadataNotifySetup)
                {
                    DSO_MetadataNotifySetup = true;
                    bleDevice.DSO_MetadataEvent += BleDevice_DSO_MetadataEvent;
                }
                var notifyType = NotifyDSO_MetadataSettings[DSO_MetadataNotifyIndex];
                DSO_MetadataNotifyIndex = (DSO_MetadataNotifyIndex + 1) % NotifyDSO_MetadataSettings.Length;
                var result = await bleDevice.NotifyDSO_MetadataAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_DSO_MetadataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new DSO_MetadataRecord();

                    var DsoStatus = valueList.GetValue("DsoStatus");
                    if (DsoStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoStatus.IsArray)
                    {
                        record.DsoStatus = (double)DsoStatus.AsDouble;
                        DSO_Metadata_DsoStatus.Text = record.DsoStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DsoDataScale = valueList.GetValue("DsoDataScale");
                    if (DsoDataScale.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataScale.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataScale.IsArray)
                    {
                        record.DsoDataScale = (double)DsoDataScale.AsDouble;
                        DSO_Metadata_DsoDataScale.Text = record.DsoDataScale.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DsoDataMode = valueList.GetValue("DsoDataMode");
                    if (DsoDataMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataMode.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataMode.IsArray)
                    {
                        record.DsoDataMode = (double)DsoDataMode.AsDouble;
                        DSO_Metadata_DsoDataMode.Text = record.DsoDataMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DsoDataRange = valueList.GetValue("DsoDataRange");
                    if (DsoDataRange.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataRange.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataRange.IsArray)
                    {
                        record.DsoDataRange = (double)DsoDataRange.AsDouble;
                        DSO_Metadata_DsoDataRange.Text = record.DsoDataRange.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DsoDataSamplingWindow = valueList.GetValue("DsoDataSamplingWindow");
                    if (DsoDataSamplingWindow.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataSamplingWindow.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataSamplingWindow.IsArray)
                    {
                        record.DsoDataSamplingWindow = (double)DsoDataSamplingWindow.AsDouble;
                        DSO_Metadata_DsoDataSamplingWindow.Text = record.DsoDataSamplingWindow.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DsoDataNsamples = valueList.GetValue("DsoDataNsamples");
                    if (DsoDataNsamples.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataNsamples.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataNsamples.IsArray)
                    {
                        record.DsoDataNsamples = (double)DsoDataNsamples.AsDouble;
                        DSO_Metadata_DsoDataNsamples.Text = record.DsoDataNsamples.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DsoSamplingRate = valueList.GetValue("DsoSamplingRate");
                    if (DsoSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoSamplingRate.IsArray)
                    {
                        record.DsoSamplingRate = (double)DsoSamplingRate.AsDouble;
                        DSO_Metadata_DsoSamplingRate.Text = record.DsoSamplingRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = DSO_MetadataRecordData.AddRecord(record);

                });
            }
        }

        // Functions for DataLogger_Dlog


        // OK to include this method even if there are no defined buttons
        private async void OnClickDataLogger_Settings(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteDataLogger_Settings(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteDataLogger_Settings(object sender, RoutedEventArgs e)
        {
            var text = DataLogger_Settings_DlogTimestamp.Text;
            await DoWriteDataLogger_Settings(text, System.Globalization.NumberStyles.None);
        }

        private async Task DoWriteDataLogger_Settings(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte DlogCommand;
                // History: used to go into DataLogger_Settings_DlogCommand.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDlogCommand = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out DlogCommand);
                if (!parsedDlogCommand)
                {
                    parseError = "DlogCommand";
                }

                UInt16 DlogReserved1;
                // History: used to go into DataLogger_Settings_DlogReserved1.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDlogReserved1 = Utilities.Parsers.TryParseUInt16(text, dec_or_hex, null, out DlogReserved1);
                if (!parsedDlogReserved1)
                {
                    parseError = "DlogReserved1";
                }

                Byte DlogMode;
                // History: used to go into DataLogger_Settings_DlogMode.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDlogMode = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out DlogMode);
                if (!parsedDlogMode)
                {
                    parseError = "DlogMode";
                }

                Byte DlogRange;
                // History: used to go into DataLogger_Settings_DlogRange.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDlogRange = Utilities.Parsers.TryParseByte(text, dec_or_hex, null, out DlogRange);
                if (!parsedDlogRange)
                {
                    parseError = "DlogRange";
                }

                UInt16 DlogUpdateInterval;
                // History: used to go into DataLogger_Settings_DlogUpdateInterval.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.None for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDlogUpdateInterval = Utilities.Parsers.TryParseUInt16(text, dec_or_hex, null, out DlogUpdateInterval);
                if (!parsedDlogUpdateInterval)
                {
                    parseError = "DlogUpdateInterval";
                }

                UInt32 DlogTimestamp;
                // History: used to go into DataLogger_Settings_DlogTimestamp.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.None for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDlogTimestamp = Utilities.Parsers.TryParseUInt32(text, dec_or_hex, null, out DlogTimestamp);
                if (!parsedDlogTimestamp)
                {
                    parseError = "DlogTimestamp";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteDataLogger_Settings(DlogCommand, DlogReserved1, DlogMode, DlogRange, DlogUpdateInterval, DlogTimestamp);
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

        public class DataLogger_ReadingRecord : INotifyPropertyChanged
        {
            public DataLogger_ReadingRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _DlogData;
            public string DlogData { get { return _DlogData; } set { if (value == _DlogData) return; _DlogData = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<DataLogger_ReadingRecord> DataLogger_ReadingRecordData { get; } = new DataCollection<DataLogger_ReadingRecord>();
        private void OnDataLogger_Reading_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (DataLogger_ReadingRecordData.Count == 0)
                {
                    DataLogger_ReadingRecordData.AddRecord(new DataLogger_ReadingRecord());
                }
                DataLogger_ReadingRecordData[DataLogger_ReadingRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDataLogger_Reading(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DataLogger_ReadingRecordData.MaxLength = value;


        }

        private void OnAlgorithmDataLogger_Reading(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DataLogger_ReadingRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDataLogger_Reading(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DlogData,Notes\n");
            foreach (var row in DataLogger_ReadingRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DlogData},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyDataLogger_ReadingSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int DataLogger_ReadingNotifyIndex = 0;
        bool DataLogger_ReadingNotifySetup = false;
        private async void OnNotifyDataLogger_Reading(object sender, RoutedEventArgs e)
        {
            await DoNotifyDataLogger_Reading();
        }

        private async Task DoNotifyDataLogger_Reading()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!DataLogger_ReadingNotifySetup)
                {
                    DataLogger_ReadingNotifySetup = true;
                    bleDevice.DataLogger_ReadingEvent += BleDevice_DataLogger_ReadingEvent;
                }
                var notifyType = NotifyDataLogger_ReadingSettings[DataLogger_ReadingNotifyIndex];
                DataLogger_ReadingNotifyIndex = (DataLogger_ReadingNotifyIndex + 1) % NotifyDataLogger_ReadingSettings.Length;
                var result = await bleDevice.NotifyDataLogger_ReadingAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_DataLogger_ReadingEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new DataLogger_ReadingRecord();

                    var DlogData = valueList.GetValue("DlogData");
                    if (DlogData.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogData.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogData.IsArray)
                    {
                        record.DlogData = (string)DlogData.AsString;
                        DataLogger_Reading_DlogData.Text = record.DlogData.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = DataLogger_ReadingRecordData.AddRecord(record);

                });
            }
        }
        public class DataLogger_MetaDataRecord : INotifyPropertyChanged
        {
            public DataLogger_MetaDataRecord()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _DlogStatus;
            public double DlogStatus { get { return _DlogStatus; } set { if (value == _DlogStatus) return; _DlogStatus = value; OnPropertyChanged(); } }

            private double _DlogScale;
            public double DlogScale { get { return _DlogScale; } set { if (value == _DlogScale) return; _DlogScale = value; OnPropertyChanged(); } }

            private double _DlogMode;
            public double DlogMode { get { return _DlogMode; } set { if (value == _DlogMode) return; _DlogMode = value; OnPropertyChanged(); } }

            private double _DlogRange;
            public double DlogRange { get { return _DlogRange; } set { if (value == _DlogRange) return; _DlogRange = value; OnPropertyChanged(); } }

            private double _DlogCurrLogging;
            public double DlogCurrLogging { get { return _DlogCurrLogging; } set { if (value == _DlogCurrLogging) return; _DlogCurrLogging = value; OnPropertyChanged(); } }

            private double _DlogCurrNSample;
            public double DlogCurrNSample { get { return _DlogCurrNSample; } set { if (value == _DlogCurrNSample) return; _DlogCurrNSample = value; OnPropertyChanged(); } }

            private double _DlogCurrTimestamp;
            public double DlogCurrTimestamp { get { return _DlogCurrTimestamp; } set { if (value == _DlogCurrTimestamp) return; _DlogCurrTimestamp = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<DataLogger_MetaDataRecord> DataLogger_MetaDataRecordData { get; } = new DataCollection<DataLogger_MetaDataRecord>();
        private void OnDataLogger_MetaData_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (DataLogger_MetaDataRecordData.Count == 0)
                {
                    DataLogger_MetaDataRecordData.AddRecord(new DataLogger_MetaDataRecord());
                }
                DataLogger_MetaDataRecordData[DataLogger_MetaDataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountDataLogger_MetaData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DataLogger_MetaDataRecordData.MaxLength = value;


        }

        private void OnAlgorithmDataLogger_MetaData(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            DataLogger_MetaDataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyDataLogger_MetaData(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DlogStatus,DlogScale,DlogMode,DlogRange,DlogCurrLogging,DlogCurrNSample,DlogCurrTimestamp,Notes\n");
            foreach (var row in DataLogger_MetaDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DlogStatus},{row.DlogScale},{row.DlogMode},{row.DlogRange},{row.DlogCurrLogging},{row.DlogCurrNSample},{row.DlogCurrTimestamp},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadDataLogger_MetaData(object sender, RoutedEventArgs e)
        {
            await DoReadDataLogger_MetaData();
        }

        private async Task DoReadDataLogger_MetaData()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadDataLogger_MetaData();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read DataLogger_MetaData");
                    return;
                }

                var record = new DataLogger_MetaDataRecord();

                var DlogStatus = valueList.GetValue("DlogStatus");
                if (DlogStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogStatus.IsArray)
                {
                    record.DlogStatus = (double)DlogStatus.AsDouble;
                    DataLogger_MetaData_DlogStatus.Text = record.DlogStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DlogScale = valueList.GetValue("DlogScale");
                if (DlogScale.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogScale.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogScale.IsArray)
                {
                    record.DlogScale = (double)DlogScale.AsDouble;
                    DataLogger_MetaData_DlogScale.Text = record.DlogScale.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DlogMode = valueList.GetValue("DlogMode");
                if (DlogMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogMode.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogMode.IsArray)
                {
                    record.DlogMode = (double)DlogMode.AsDouble;
                    DataLogger_MetaData_DlogMode.Text = record.DlogMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DlogRange = valueList.GetValue("DlogRange");
                if (DlogRange.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogRange.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogRange.IsArray)
                {
                    record.DlogRange = (double)DlogRange.AsDouble;
                    DataLogger_MetaData_DlogRange.Text = record.DlogRange.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DlogCurrLogging = valueList.GetValue("DlogCurrLogging");
                if (DlogCurrLogging.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogCurrLogging.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogCurrLogging.IsArray)
                {
                    record.DlogCurrLogging = (double)DlogCurrLogging.AsDouble;
                    DataLogger_MetaData_DlogCurrLogging.Text = record.DlogCurrLogging.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DlogCurrNSample = valueList.GetValue("DlogCurrNSample");
                if (DlogCurrNSample.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogCurrNSample.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogCurrNSample.IsArray)
                {
                    record.DlogCurrNSample = (double)DlogCurrNSample.AsDouble;
                    DataLogger_MetaData_DlogCurrNSample.Text = record.DlogCurrNSample.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var DlogCurrTimestamp = valueList.GetValue("DlogCurrTimestamp");
                if (DlogCurrTimestamp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogCurrTimestamp.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogCurrTimestamp.IsArray)
                {
                    record.DlogCurrTimestamp = (double)DlogCurrTimestamp.AsDouble;
                    DataLogger_MetaData_DlogCurrTimestamp.Text = record.DlogCurrTimestamp.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                DataLogger_MetaDataRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyDataLogger_MetaDataSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int DataLogger_MetaDataNotifyIndex = 0;
        bool DataLogger_MetaDataNotifySetup = false;
        private async void OnNotifyDataLogger_MetaData(object sender, RoutedEventArgs e)
        {
            await DoNotifyDataLogger_MetaData();
        }

        private async Task DoNotifyDataLogger_MetaData()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!DataLogger_MetaDataNotifySetup)
                {
                    DataLogger_MetaDataNotifySetup = true;
                    bleDevice.DataLogger_MetaDataEvent += BleDevice_DataLogger_MetaDataEvent;
                }
                var notifyType = NotifyDataLogger_MetaDataSettings[DataLogger_MetaDataNotifyIndex];
                DataLogger_MetaDataNotifyIndex = (DataLogger_MetaDataNotifyIndex + 1) % NotifyDataLogger_MetaDataSettings.Length;
                var result = await bleDevice.NotifyDataLogger_MetaDataAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_DataLogger_MetaDataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new DataLogger_MetaDataRecord();

                    var DlogStatus = valueList.GetValue("DlogStatus");
                    if (DlogStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogStatus.IsArray)
                    {
                        record.DlogStatus = (double)DlogStatus.AsDouble;
                        DataLogger_MetaData_DlogStatus.Text = record.DlogStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DlogScale = valueList.GetValue("DlogScale");
                    if (DlogScale.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogScale.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogScale.IsArray)
                    {
                        record.DlogScale = (double)DlogScale.AsDouble;
                        DataLogger_MetaData_DlogScale.Text = record.DlogScale.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DlogMode = valueList.GetValue("DlogMode");
                    if (DlogMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogMode.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogMode.IsArray)
                    {
                        record.DlogMode = (double)DlogMode.AsDouble;
                        DataLogger_MetaData_DlogMode.Text = record.DlogMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DlogRange = valueList.GetValue("DlogRange");
                    if (DlogRange.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogRange.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogRange.IsArray)
                    {
                        record.DlogRange = (double)DlogRange.AsDouble;
                        DataLogger_MetaData_DlogRange.Text = record.DlogRange.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DlogCurrLogging = valueList.GetValue("DlogCurrLogging");
                    if (DlogCurrLogging.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogCurrLogging.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogCurrLogging.IsArray)
                    {
                        record.DlogCurrLogging = (double)DlogCurrLogging.AsDouble;
                        DataLogger_MetaData_DlogCurrLogging.Text = record.DlogCurrLogging.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DlogCurrNSample = valueList.GetValue("DlogCurrNSample");
                    if (DlogCurrNSample.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogCurrNSample.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogCurrNSample.IsArray)
                    {
                        record.DlogCurrNSample = (double)DlogCurrNSample.AsDouble;
                        DataLogger_MetaData_DlogCurrNSample.Text = record.DlogCurrNSample.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var DlogCurrTimestamp = valueList.GetValue("DlogCurrTimestamp");
                    if (DlogCurrTimestamp.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DlogCurrTimestamp.CurrentType == BCBasic.BCValue.ValueType.IsString || DlogCurrTimestamp.IsArray)
                    {
                        record.DlogCurrTimestamp = (double)DlogCurrTimestamp.AsDouble;
                        DataLogger_MetaData_DlogCurrTimestamp.Text = record.DlogCurrTimestamp.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = DataLogger_MetaDataRecordData.AddRecord(record);

                });
            }
        }

        // Functions for CalibrationService


        // OK to include this method even if there are no defined buttons
        private async void OnClickCalbrateTemperature(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteCalbrateTemperature(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteCalbrateTemperature(object sender, RoutedEventArgs e)
        {
            var text = CalbrateTemperature_CalibrateUnknown0.Text;
            await DoWriteCalbrateTemperature(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteCalbrateTemperature(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes CalibrateUnknown0;
                // History: used to go into CalbrateTemperature_CalibrateUnknown0.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCalibrateUnknown0 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out CalibrateUnknown0);
                if (!parsedCalibrateUnknown0)
                {
                    parseError = "CalibrateUnknown0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteCalbrateTemperature(CalibrateUnknown0);
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

        // OK to include this method even if there are no defined buttons
        private async void OnClickCalibrateUnknown1(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteCalibrateUnknown1(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteCalibrateUnknown1(object sender, RoutedEventArgs e)
        {
            var text = CalibrateUnknown1_CalibrateUnknown1.Text;
            await DoWriteCalibrateUnknown1(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteCalibrateUnknown1(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes CalibrateUnknown1;
                // History: used to go into CalibrateUnknown1_CalibrateUnknown1.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCalibrateUnknown1 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out CalibrateUnknown1);
                if (!parsedCalibrateUnknown1)
                {
                    parseError = "CalibrateUnknown1";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteCalibrateUnknown1(CalibrateUnknown1);
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

        public class CalibrateUnknown1Record : INotifyPropertyChanged
        {
            public CalibrateUnknown1Record()
            {
                this.EventTime = DateTime.Now;
            }
            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private string _CalibrateUnknown1;
            public string CalibrateUnknown1 { get { return _CalibrateUnknown1; } set { if (value == _CalibrateUnknown1) return; _CalibrateUnknown1 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<CalibrateUnknown1Record> CalibrateUnknown1RecordData { get; } = new DataCollection<CalibrateUnknown1Record>();
        private void OnCalibrateUnknown1_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (CalibrateUnknown1RecordData.Count == 0)
                {
                    CalibrateUnknown1RecordData.AddRecord(new CalibrateUnknown1Record());
                }
                CalibrateUnknown1RecordData[CalibrateUnknown1RecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountCalibrateUnknown1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            CalibrateUnknown1RecordData.MaxLength = value;


        }

        private void OnAlgorithmCalibrateUnknown1(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            CalibrateUnknown1RecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyCalibrateUnknown1(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,CalibrateUnknown1,Notes\n");
            foreach (var row in CalibrateUnknown1RecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.CalibrateUnknown1},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadCalibrateUnknown1(object sender, RoutedEventArgs e)
        {
            await DoReadCalibrateUnknown1();
        }

        private async Task DoReadCalibrateUnknown1()
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadCalibrateUnknown1();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read CalibrateUnknown1");
                    return;
                }

                var record = new CalibrateUnknown1Record();

                var CalibrateUnknown1 = valueList.GetValue("CalibrateUnknown1");
                if (CalibrateUnknown1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || CalibrateUnknown1.CurrentType == BCBasic.BCValue.ValueType.IsString || CalibrateUnknown1.IsArray)
                {
                    record.CalibrateUnknown1 = (string)CalibrateUnknown1.AsString;
                    CalibrateUnknown1_CalibrateUnknown1.Text = record.CalibrateUnknown1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                CalibrateUnknown1RecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        // OK to include this method even if there are no defined buttons
        private async void OnClickCalibrrateUnknown2(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteCalibrrateUnknown2(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteCalibrrateUnknown2(object sender, RoutedEventArgs e)
        {
            var text = CalibrrateUnknown2_CalibrrateUnknown2.Text;
            await DoWriteCalibrrateUnknown2(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteCalibrrateUnknown2(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes CalibrrateUnknown2;
                // History: used to go into CalibrrateUnknown2_CalibrrateUnknown2.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCalibrrateUnknown2 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out CalibrrateUnknown2);
                if (!parsedCalibrrateUnknown2)
                {
                    parseError = "CalibrrateUnknown2";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteCalibrrateUnknown2(CalibrrateUnknown2);
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


        // Functions for Silabs_Service_OTA


        // OK to include this method even if there are no defined buttons
        private async void OnClickOTA_Control(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteOTA_Control(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteOTA_Control(object sender, RoutedEventArgs e)
        {
            var text = OTA_Control_Unknown0.Text;
            await DoWriteOTA_Control(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteOTA_Control(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes Unknown0;
                // History: used to go into OTA_Control_Unknown0.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedUnknown0 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out Unknown0);
                if (!parsedUnknown0)
                {
                    parseError = "Unknown0";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteOTA_Control(Unknown0);
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

        // OK to include this method even if there are no defined buttons
        private async void OnClickOTA_Data(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteOTA_Data(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteOTA_Data(object sender, RoutedEventArgs e)
        {
            var text = OTA_Data_Unknown1.Text;
            await DoWriteOTA_Data(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteOTA_Data(string text, System.Globalization.NumberStyles dec_or_hex)
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
                // History: used to go into OTA_Data_Unknown1.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedUnknown1 = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out Unknown1);
                if (!parsedUnknown1)
                {
                    parseError = "Unknown1";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteOTA_Data(Unknown1);
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
            await bleDevice.EnsureCharacteristicAsync(PokitProMeter.CharacteristicsEnum.All_enum, true);
            SetStatusActive(false);
        }

    }
}
