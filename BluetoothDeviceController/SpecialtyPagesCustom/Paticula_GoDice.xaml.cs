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
using Windows.Media.SpeechSynthesis;
using Windows.Media.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the [[NAME]] device
    /// </summary>
    public sealed partial class Particula_GoDicePage : Page, HasId, ISetHandleStatus
    {
        public Particula_GoDicePage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Particula_GoDice";
        private string DeviceNameUser = "GoDice";

        int ncommand = 0;
        Particula_GoDice bleDevice = new Particula_GoDice();
        const string TiltGlyph = "⌧"; // Too large: "❌";

        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive(false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            await DoReadDevice_Name();

            bleDevice.OnBatteryLevel += DoOnBatteryLevel;
            bleDevice.OnRollStart += DoOnRollStart;
            bleDevice.OnRollStable += DoOnRollStable;
            bleDevice.OnFakeStable += DoOnFakeStable;
            bleDevice.OnMoveStable += DoOnMoveStable;
            bleDevice.OnTiltStable += DoOntiltStable;

            await DoNotifyReceive(); // Set up the notify on incoming events
            OnGetBatteryLevel(null, null);

            // The object for controlling the speech synthesis engine (voice).
            synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            bleDevice.Status.OnBluetoothStatus -= bleDevice_OnBluetoothStatus;
            bleDevice.NotifyReceiveRemoveCharacteristicCallback();
            bleDevice.OnBatteryLevel -= DoOnBatteryLevel;
            bleDevice.OnRollStart -= DoOnRollStart;
            bleDevice.OnRollStable -= DoOnRollStable;
            bleDevice.OnFakeStable -= DoOnFakeStable;
            bleDevice.OnMoveStable -= DoOnMoveStable;
            bleDevice.OnTiltStable -= DoOntiltStable;

            if (ReceiveNotifySetup) { bleDevice.ReceiveEvent -= BleDevice_ReceiveEvent; ReceiveNotifySetup = false; }

        }

        private async void DoOnRollStart(object obj, EventArgs args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                uiRollValue.Text = "Rolling!";
                uiDieIcon.Text = "···";
                uiDieText.Text = "Rolling";
            });
        }

        private async void DoOnBatteryLevel(object obj, int batteryLevel)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                uiBatteryLevelValue.Text = batteryLevel.ToString();
                var levelGlyph = Particula_GoDice.BatteryLevelToGlyph(batteryLevel);
                uiBatteryLevelGlyph.Text = levelGlyph;
                uiBatteryLevelPercent.Text = $"{batteryLevel}%";
            });
        }

        private async void DoOnRollStable(object obj, int dieRoll)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                uiRollValue.Text = dieRoll.ToString();
                uiRollTypeValue.Text = "(good roll)";
                if (dieRoll >= 1 && dieRoll <= 6)
                {
                    uiDieIcon.Text = $"{Particula_GoDice.DiceFaces[dieRoll - 1]}";
                    uiDieText.Text = $"{dieRoll.ToString()}";

                    // Generate the audio stream from plain text.
                    await SpeakAsync($"Rolled {dieRoll}");
                }
                else
                {
                    uiDieIcon.Text = TiltGlyph;
                    uiDieText.Text = $"Tilt  ({dieRoll.ToString()})";

                    // Generate the audio stream from plain text.
                    await SpeakAsync($"bad roll");
                }
            });
        }

        private async void DoOnFakeStable(object obj, int dieRoll)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                uiRollValue.Text = dieRoll.ToString();
                uiRollTypeValue.Text = "(fake stable)";
                uiDieIcon.Text = TiltGlyph;
                uiDieText.Text = $"Tilt  ({dieRoll.ToString()}) F";

                // Generate the audio stream from plain text.
                await SpeakAsync($"bad roll");
            });
        }

        private async void DoOnMoveStable(object obj, int dieRoll)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                uiRollValue.Text = dieRoll.ToString();
                uiRollTypeValue.Text = "(moved)";
                uiDieIcon.Text = TiltGlyph;
                uiDieText.Text = $"Tilt  ({dieRoll.ToString()}) M";

                // Generate the audio stream from plain text.
                await SpeakAsync($"bad roll");

                // Send the stream to the media object.
            });
        }

        private async void DoOntiltStable(object obj, int dieRoll)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                uiRollValue.Text = dieRoll.ToString();
                uiRollTypeValue.Text = "(tilted)";
                uiDieIcon.Text = TiltGlyph;
                uiDieText.Text = $"Tilt  ({dieRoll.ToString()})";

                // Generate the audio stream from plain text.
                await SpeakAsync($"bad roll");

            });
        }




        SpeechSynthesizer synth;

        private async Task SpeakAsync(string text)
        {
            var shouldSpeak = uiSpeakRolls.IsChecked.Value;
            if (!shouldSpeak) return;

            // Generate the audio stream from plain text.
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(text);

            // Send the stream to the media object.
            var mediaElement = uiTalkingDice;
            // Send the stream to the media object.
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
            //mediaElement.Source = MediaSource.CreateFromStream(stream, stream.ContentType);

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

                    var color = Particula_GoDice.GetDiceColorFromName(record.Device_Name);
                    if (color != "?") // Returned when the name doesn't really match e.g., when it's GoDice_Proto_Test (whoops)
                    {
                        uiDiceName.Text = color;
                    }
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
        public class Connection_ParameterRecord : INotifyPropertyChanged
        {
            public Connection_ParameterRecord()
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

                var Interval_Min = valueList.GetValue("Interval_Min");
                if (Interval_Min.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Interval_Min.CurrentType == BCBasic.BCValue.ValueType.IsString || Interval_Min.IsArray)
                {
                    record.Interval_Min = (double)Interval_Min.AsDouble;
                    Connection_Parameter_Interval_Min.Text = record.Interval_Min.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Interval_Max = valueList.GetValue("Interval_Max");
                if (Interval_Max.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Interval_Max.CurrentType == BCBasic.BCValue.ValueType.IsString || Interval_Max.IsArray)
                {
                    record.Interval_Max = (double)Interval_Max.AsDouble;
                    Connection_Parameter_Interval_Max.Text = record.Interval_Max.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Latency = valueList.GetValue("Latency");
                if (Latency.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Latency.CurrentType == BCBasic.BCValue.ValueType.IsString || Latency.IsArray)
                {
                    record.Latency = (double)Latency.AsDouble;
                    Connection_Parameter_Latency.Text = record.Latency.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }

                var Timeout = valueList.GetValue("Timeout");
                if (Timeout.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Timeout.CurrentType == BCBasic.BCValue.ValueType.IsString || Timeout.IsArray)
                {
                    record.Timeout = (double)Timeout.AsDouble;
                    Connection_Parameter_Timeout.Text = record.Timeout.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                }


                Connection_ParameterRecordData.Add(record);

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
            protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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
            await DoReadCentral_Address_Resolution();
        }

        private async Task DoReadCentral_Address_Resolution()
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
                if (AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsDouble || AddressResolutionSupported.CurrentType == BCBasic.BCValue.ValueType.IsString || AddressResolutionSupported.IsArray)
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


        // Functions for DiceTransmit


        // OK to include this method even if there are no defined buttons
        private async void OnClickTransmit(object sender, RoutedEventArgs e)
        {
            var text = (sender as Button).Tag as String;
            await DoWriteTransmit(text, System.Globalization.NumberStyles.Integer);
        }

        private async void OnWriteTransmit(object sender, RoutedEventArgs e)
        {
            var text = Transmit_DiceCommand.Text;
            await DoWriteTransmit(text, System.Globalization.NumberStyles.AllowHexSpecifier);
        }

        private async Task DoWriteTransmit(string text, System.Globalization.NumberStyles dec_or_hex)
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Note: This template isn't smart enough to piece together
                // multi-field characteristics. It can support simple characterisitics
                // where there's only one data item.
                string parseError = null;

                Bytes DiceCommand;
                // History: used to go into Transmit_DiceCommand.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedDiceCommand = Utilities.Parsers.TryParseBytes(text, dec_or_hex, null, out DiceCommand);
                if (!parsedDiceCommand)
                {
                    parseError = "DiceCommand";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteTransmit(DiceCommand);
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

        public class ReceiveRecord : INotifyPropertyChanged
        {
            public ReceiveRecord()
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

            private string _DiceEvent;
            public string DiceEvent { get { return _DiceEvent; } set { if (value == _DiceEvent) return; _DiceEvent = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ReceiveRecord> ReceiveRecordData { get; } = new DataCollection<ReceiveRecord>();
        private void OnReceive_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ReceiveRecordData.Count == 0)
                {
                    ReceiveRecordData.AddRecord(new ReceiveRecord());
                }
                ReceiveRecordData[ReceiveRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountReceive(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ReceiveRecordData.MaxLength = value;


        }

        private void OnAlgorithmReceive(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ReceiveRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyReceive(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,DiceEvent,Notes\n");
            foreach (var row in ReceiveRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.DiceEvent},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyReceiveSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int ReceiveNotifyIndex = 0;
        bool ReceiveNotifySetup = false;
        private async void OnNotifyReceive(object sender, RoutedEventArgs e)
        {
            await DoNotifyReceive();
        }

        private async Task DoNotifyReceive()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!ReceiveNotifySetup)
                {
                    ReceiveNotifySetup = true;
                    bleDevice.ReceiveEvent += BleDevice_ReceiveEvent;
                }
                var notifyType = NotifyReceiveSettings[ReceiveNotifyIndex];
                ReceiveNotifyIndex = (ReceiveNotifyIndex + 1) % NotifyReceiveSettings.Length;
                var result = await bleDevice.NotifyReceiveAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_ReceiveEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new ReceiveRecord();

                    var DiceEvent = valueList.GetValue("DiceEvent");
                    if (DiceEvent.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DiceEvent.CurrentType == BCBasic.BCValue.ValueType.IsString || DiceEvent.IsArray)
                    {
                        record.DiceEvent = (string)DiceEvent.AsString;
                        Receive_DiceEvent.Text = record.DiceEvent.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }
                    if (DiceEvent.IsArray)
                    {
                        // Kind of convoluted: set the arrray back to the bleDevice for processing. This will fire a message.
                        BCBasic.BCValueList message = DiceEvent.AsArray;
                        bleDevice.HandleReceiveMessageCustom(message);
                    }

                    var addResult = ReceiveRecordData.AddRecord(record);

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

        private async void OnGetBatteryLevel(object sender, RoutedEventArgs e)
        {
            await bleDevice.GetBatteryLevelAsync();
        }

        private async void OnUpdateBatteryLevel(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await bleDevice.GetBatteryLevelAsync();
        }
        private async void OnFlashLed1(object sender, RoutedEventArgs e)
        {
            // Don't hide when flashing LED uiColor1Flyout.Hide();

            var led1 = uiLed1Color.Color;
            var led2 = uiLed1Color.Color;
            await bleDevice.SetDiceColorAsync(led1.R, led1.G, led1.B, led2.R, led2.G, led2.B);
            await Task.Delay(500);
            await bleDevice.SetDiceColorAsync(0,0,0, 0,0,0);
        }

        private async void OnOkLed1(object sender, RoutedEventArgs e)
        {
            uiColor1Flyout.Hide();

            var led1 = uiLed1Color.Color;
            var led2 = uiLed1Color.Color;
            await bleDevice.SetDiceColorAsync(led1.R, led1.G, led1.B, led2.R, led2.G, led2.B);
        }

        private void OnCancelLed1(object sender, RoutedEventArgs e)
        {
            uiColor1Flyout.Hide();
        }
    }
}
