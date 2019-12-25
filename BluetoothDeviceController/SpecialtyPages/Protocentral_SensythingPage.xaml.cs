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
    public sealed partial class Protocentral_SensythingPage : Page, HasId, ISetHandleStatus
    {
        public Protocentral_SensythingPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Protocentral_Sensything";
        private string DeviceNameUser = "Sensything";

        int ncommand = 0;
        Protocentral_Sensything bleDevice = new Protocentral_Sensything();
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

        public class Central_Address_ResolutionRecord : INotifyPropertyChanged
        {
            public Central_Address_ResolutionRecord()
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

            private double _AddressResolutionSupported;
            public double AddressResolutionSupported { get { return _AddressResolutionSupported; } set { if (value == _AddressResolutionSupported) return; _AddressResolutionSupported = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<Central_Address_ResolutionRecord> Central_Address_ResolutionRecordData { get; } = new DataCollection<Central_Address_ResolutionRecord>();
        private void OnCentral_Address_Resolution_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Central_Address_ResolutionRecordData.Count == 0)
                {
                    Central_Address_ResolutionRecordData.AddRecord(new Central_Address_ResolutionRecord());
                }
                Central_Address_ResolutionRecordData[Central_Address_ResolutionRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountCentral_Address_Resolution(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Central_Address_ResolutionRecordData.MaxLength = value;


        }

        private void OnAlgorithmCentral_Address_Resolution(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Central_Address_ResolutionRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyCentral_Address_Resolution(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,AddressResolutionSupported,Notes\n");
            foreach (var row in Central_Address_ResolutionRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.AddressResolutionSupported},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadCentral_Address_Resolution(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadCentral_Address_Resolution();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Central_Address_Resolution");
                    return;
                }

                var record = new Central_Address_ResolutionRecord();

                var AddressResolutionSupported = valueList.GetValue("AddressResolutionSupported");
                if (AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.AddressResolutionSupported = (double)AddressResolutionSupported.AsDouble;
                    Central_Address_Resolution_AddressResolutionSupported.Text = record.AddressResolutionSupported.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Central_Address_ResolutionRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }


        // Functions for Battery


        private async void OnWriteBatteryLevel(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                SByte BatteryLevel;
                var parsedBatteryLevel = Utilities.Parsers.TryParseSByte(BatteryLevel_BatteryLevel.Text, System.Globalization.NumberStyles.None, null, out BatteryLevel);
                if (!parsedBatteryLevel)
                {
                    parseError = "BatteryLevel";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteBatteryLevel(BatteryLevel);
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

        // Functions for Primary


        public class AnalogRecord : INotifyPropertyChanged
        {
            public AnalogRecord()
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

            private double _A1;
            public double A1 { get { return _A1; } set { if (value == _A1) return; _A1 = value; OnPropertyChanged(); } }

            private double _A2;
            public double A2 { get { return _A2; } set { if (value == _A2) return; _A2 = value; OnPropertyChanged(); } }

            private double _A3;
            public double A3 { get { return _A3; } set { if (value == _A3) return; _A3 = value; OnPropertyChanged(); } }

            private double _A4;
            public double A4 { get { return _A4; } set { if (value == _A4) return; _A4 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<AnalogRecord> AnalogRecordData { get; } = new DataCollection<AnalogRecord>();
        private void OnAnalog_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (AnalogRecordData.Count == 0)
                {
                    AnalogRecordData.AddRecord(new AnalogRecord());
                }
                AnalogRecordData[AnalogRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountAnalog(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AnalogRecordData.MaxLength = value;

            AnalogChart.RedrawYTime<AnalogRecord>(AnalogRecordData);
        }

        private void OnAlgorithmAnalog(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            AnalogRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyAnalog(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,A1,A2,A3,A4,Notes\n");
            foreach (var row in AnalogRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.A1},{row.A2},{row.A3},{row.A4},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyAnalogSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int AnalogNotifyIndex = 0;
        bool AnalogNotifySetup = false;
        private async void OnNotifyAnalog(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!AnalogNotifySetup)
                {
                    AnalogNotifySetup = true;
                    bleDevice.AnalogEvent += BleDevice_AnalogEvent;
                }
                var notifyType = NotifyAnalogSettings[AnalogNotifyIndex];
                AnalogNotifyIndex = (AnalogNotifyIndex + 1) % NotifyAnalogSettings.Length;
                var result = await bleDevice.NotifyAnalogAsync(notifyType);



                var EventTimeProperty = typeof(AnalogRecord).GetProperty("EventTime");
                var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(AnalogRecord).GetProperty("A1"),
typeof(AnalogRecord).GetProperty("A2"),
typeof(AnalogRecord).GetProperty("A3"),
typeof(AnalogRecord).GetProperty("A4"),
                };
                var names = new List<string>()
                {
"A1",
"A2",
"A3",
"A4",
                };
                AnalogChart.SetDataProperties(properties, EventTimeProperty, names);
                AnalogChart.SetTitle("Analog Chart");
                AnalogChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
                {
                    tableType = "standard",
                    chartType = "standard",
                    chartCommand = "AddYTime<AnalogRecord>(addResult, AnalogRecordData)",
                    chartDefaultMaxY = 3,
                    chartDefaultMinY = 0,
                }
;

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_AnalogEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new AnalogRecord();

                    var A1 = valueList.GetValue("A1");
                    if (A1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || A1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.A1 = (double)A1.AsDouble;
                        Analog_A1.Text = record.A1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var A2 = valueList.GetValue("A2");
                    if (A2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || A2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.A2 = (double)A2.AsDouble;
                        Analog_A2.Text = record.A2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var A3 = valueList.GetValue("A3");
                    if (A3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || A3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.A3 = (double)A3.AsDouble;
                        Analog_A3.Text = record.A3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var A4 = valueList.GetValue("A4");
                    if (A4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || A4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.A4 = (double)A4.AsDouble;
                        Analog_A4.Text = record.A4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = AnalogRecordData.AddRecord(record);
                    AnalogChart.AddYTime<AnalogRecord>(addResult, AnalogRecordData);
                });
            }
        }

        // Functions for QWIIC


        private async void OnWriteQWIIC(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Byte Sensor;
                var parsedSensor = Utilities.Parsers.TryParseByte(QWIIC_Sensor.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Sensor);
                if (!parsedSensor)
                {
                    parseError = "Sensor";
                }

                UInt16 Channel1;
                var parsedChannel1 = Utilities.Parsers.TryParseUInt16(QWIIC_Channel1.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Channel1);
                if (!parsedChannel1)
                {
                    parseError = "Channel1";
                }

                UInt16 Channel2;
                var parsedChannel2 = Utilities.Parsers.TryParseUInt16(QWIIC_Channel2.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Channel2);
                if (!parsedChannel2)
                {
                    parseError = "Channel2";
                }

                UInt16 Channel3;
                var parsedChannel3 = Utilities.Parsers.TryParseUInt16(QWIIC_Channel3.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Channel3);
                if (!parsedChannel3)
                {
                    parseError = "Channel3";
                }

                UInt16 Channel4;
                var parsedChannel4 = Utilities.Parsers.TryParseUInt16(QWIIC_Channel4.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Channel4);
                if (!parsedChannel4)
                {
                    parseError = "Channel4";
                }

                UInt16 Channel5;
                var parsedChannel5 = Utilities.Parsers.TryParseUInt16(QWIIC_Channel5.Text, System.Globalization.NumberStyles.AllowHexSpecifier, null, out Channel5);
                if (!parsedChannel5)
                {
                    parseError = "Channel5";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteQWIIC(Sensor, Channel1, Channel2, Channel3, Channel4, Channel5);
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

        public class QWIICRecord : INotifyPropertyChanged
        {
            public QWIICRecord()
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

            private double _Sensor;
            public double Sensor { get { return _Sensor; } set { if (value == _Sensor) return; _Sensor = value; OnPropertyChanged(); } }

            private double _Channel1;
            public double Channel1 { get { return _Channel1; } set { if (value == _Channel1) return; _Channel1 = value; OnPropertyChanged(); } }

            private double _Channel2;
            public double Channel2 { get { return _Channel2; } set { if (value == _Channel2) return; _Channel2 = value; OnPropertyChanged(); } }

            private double _Channel3;
            public double Channel3 { get { return _Channel3; } set { if (value == _Channel3) return; _Channel3 = value; OnPropertyChanged(); } }

            private double _Channel4;
            public double Channel4 { get { return _Channel4; } set { if (value == _Channel4) return; _Channel4 = value; OnPropertyChanged(); } }

            private double _Channel5;
            public double Channel5 { get { return _Channel5; } set { if (value == _Channel5) return; _Channel5 = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<QWIICRecord> QWIICRecordData { get; } = new DataCollection<QWIICRecord>();
        private void OnQWIIC_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (QWIICRecordData.Count == 0)
                {
                    QWIICRecordData.AddRecord(new QWIICRecord());
                }
                QWIICRecordData[QWIICRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountQWIIC(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            QWIICRecordData.MaxLength = value;


        }

        private void OnAlgorithmQWIIC(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            QWIICRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyQWIIC(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Sensor,Channel1,Channel2,Channel3,Channel4,Channel5,Notes\n");
            foreach (var row in QWIICRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Sensor},{row.Channel1},{row.Channel2},{row.Channel3},{row.Channel4},{row.Channel5},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private async void OnReadQWIIC(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true); // the false happens in the bluetooth status handler.
            ncommand++;
            try
            {
                var valueList = await bleDevice.ReadQWIIC();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read QWIIC");
                    return;
                }

                var record = new QWIICRecord();

                var Sensor = valueList.GetValue("Sensor");
                if (Sensor.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Sensor.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Sensor = (double)Sensor.AsDouble;
                    QWIIC_Sensor.Text = record.Sensor.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Channel1 = valueList.GetValue("Channel1");
                if (Channel1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Channel1 = (double)Channel1.AsDouble;
                    QWIIC_Channel1.Text = record.Channel1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Channel2 = valueList.GetValue("Channel2");
                if (Channel2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Channel2 = (double)Channel2.AsDouble;
                    QWIIC_Channel2.Text = record.Channel2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Channel3 = valueList.GetValue("Channel3");
                if (Channel3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Channel3 = (double)Channel3.AsDouble;
                    QWIIC_Channel3.Text = record.Channel3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Channel4 = valueList.GetValue("Channel4");
                if (Channel4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Channel4 = (double)Channel4.AsDouble;
                    QWIIC_Channel4.Text = record.Channel4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Channel5 = valueList.GetValue("Channel5");
                if (Channel5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                {
                    record.Channel5 = (double)Channel5.AsDouble;
                    QWIIC_Channel5.Text = record.Channel5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                QWIICRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyQWIICSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int QWIICNotifyIndex = 0;
        bool QWIICNotifySetup = false;
        private async void OnNotifyQWIIC(object sender, RoutedEventArgs e)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!QWIICNotifySetup)
                {
                    QWIICNotifySetup = true;
                    bleDevice.QWIICEvent += BleDevice_QWIICEvent;
                }
                var notifyType = NotifyQWIICSettings[QWIICNotifyIndex];
                QWIICNotifyIndex = (QWIICNotifyIndex + 1) % NotifyQWIICSettings.Length;
                var result = await bleDevice.NotifyQWIICAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_QWIICEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new QWIICRecord();

                    var Sensor = valueList.GetValue("Sensor");
                    if (Sensor.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Sensor.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Sensor = (double)Sensor.AsDouble;
                        QWIIC_Sensor.Text = record.Sensor.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Channel1 = valueList.GetValue("Channel1");
                    if (Channel1.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel1.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Channel1 = (double)Channel1.AsDouble;
                        QWIIC_Channel1.Text = record.Channel1.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Channel2 = valueList.GetValue("Channel2");
                    if (Channel2.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel2.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Channel2 = (double)Channel2.AsDouble;
                        QWIIC_Channel2.Text = record.Channel2.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Channel3 = valueList.GetValue("Channel3");
                    if (Channel3.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel3.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Channel3 = (double)Channel3.AsDouble;
                        QWIIC_Channel3.Text = record.Channel3.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Channel4 = valueList.GetValue("Channel4");
                    if (Channel4.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel4.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Channel4 = (double)Channel4.AsDouble;
                        QWIIC_Channel4.Text = record.Channel4.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var Channel5 = valueList.GetValue("Channel5");
                    if (Channel5.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Channel5.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Channel5 = (double)Channel5.AsDouble;
                        QWIIC_Channel5.Text = record.Channel5.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = QWIICRecordData.AddRecord(record);

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
