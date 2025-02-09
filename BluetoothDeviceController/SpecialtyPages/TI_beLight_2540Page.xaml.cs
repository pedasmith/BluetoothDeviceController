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
using static BluetoothProtocols.TI_beLight_2540;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace BluetoothDeviceController.SpecialtyPages
{
    /// <summary>
    /// Specialty page for the beLight device
    /// </summary>
    public sealed partial class TI_beLight_2540Page : Page, HasId, ISetHandleStatus
    {
        public TI_beLight_2540Page()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.uiSetColorLampControl.Light = bleDevice;

        }
        private string DeviceName = "TI_beLight_2540";
        private string DeviceNameUser = "beLight";

        int ncommand = 0;
        TI_beLight_2540 bleDevice = new TI_beLight_2540();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive (true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive (false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;
            await Task.Delay(0); // No Device_Name to read, but still need to have an async operation.

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


        // Functions for Lamp Control
        public class RedRecord : INotifyPropertyChanged
        {
            public RedRecord()
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

            private double _Red;
            public double Red { get { return _Red; } set { if (value == _Red) return; _Red = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<RedRecord> RedRecordData { get; } = new DataCollection<RedRecord>();
    private void OnRed_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (RedRecordData.Count == 0)
            {
                RedRecordData.AddRecord(new RedRecord());
            }
            RedRecordData[RedRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountRed(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        RedRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmRed(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        RedRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyRed(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Red,Notes\n");
        foreach (var row in RedRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Red},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickRed(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Red_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Red_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteRed(values);

        }

        private async void OnWriteRed(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Red_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Red_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteRed(values);

        }

        private async Task DoWriteRed(List<UxTextValue> values)
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

                Byte Red;
                // History: used to go into Red_Red.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedRed = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Red);
                valueIndex++; // Change #5
                if (!parsedRed)
                {
                    parseError = "Red";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteRed(Red);
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



        public class GreenRecord : INotifyPropertyChanged
        {
            public GreenRecord()
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

            private double _Green;
            public double Green { get { return _Green; } set { if (value == _Green) return; _Green = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<GreenRecord> GreenRecordData { get; } = new DataCollection<GreenRecord>();
    private void OnGreen_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (GreenRecordData.Count == 0)
            {
                GreenRecordData.AddRecord(new GreenRecord());
            }
            GreenRecordData[GreenRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountGreen(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        GreenRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmGreen(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        GreenRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyGreen(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Green,Notes\n");
        foreach (var row in GreenRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Green},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickGreen(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Green_Green.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Green_Green.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteGreen(values);

        }

        private async void OnWriteGreen(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Green_Green.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Green_Green.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteGreen(values);

        }

        private async Task DoWriteGreen(List<UxTextValue> values)
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

                Byte Green;
                // History: used to go into Green_Green.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedGreen = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Green);
                valueIndex++; // Change #5
                if (!parsedGreen)
                {
                    parseError = "Green";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteGreen(Green);
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



        public class BlueRecord : INotifyPropertyChanged
        {
            public BlueRecord()
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

            private double _Blue;
            public double Blue { get { return _Blue; } set { if (value == _Blue) return; _Blue = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<BlueRecord> BlueRecordData { get; } = new DataCollection<BlueRecord>();
    private void OnBlue_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (BlueRecordData.Count == 0)
            {
                BlueRecordData.AddRecord(new BlueRecord());
            }
            BlueRecordData[BlueRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountBlue(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        BlueRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmBlue(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        BlueRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyBlue(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Blue,Notes\n");
        foreach (var row in BlueRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Blue},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickBlue(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Blue_Blue.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Blue_Blue.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteBlue(values);

        }

        private async void OnWriteBlue(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(Blue_Blue.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(Blue_Blue.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteBlue(values);

        }

        private async Task DoWriteBlue(List<UxTextValue> values)
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

                Byte Blue;
                // History: used to go into Blue_Blue.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedBlue = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Blue);
                valueIndex++; // Change #5
                if (!parsedBlue)
                {
                    parseError = "Blue";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteBlue(Blue);
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



        public class WhiteRecord : INotifyPropertyChanged
        {
            public WhiteRecord()
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

            private double _White;
            public double White { get { return _White; } set { if (value == _White) return; _White = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<WhiteRecord> WhiteRecordData { get; } = new DataCollection<WhiteRecord>();
    private void OnWhite_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (WhiteRecordData.Count == 0)
            {
                WhiteRecordData.AddRecord(new WhiteRecord());
            }
            WhiteRecordData[WhiteRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountWhite(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        WhiteRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmWhite(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        WhiteRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopyWhite(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,White,Notes\n");
        foreach (var row in WhiteRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.White},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickWhite(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(White_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(White_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteWhite(values);

        }

        private async void OnWriteWhite(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(White_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(White_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteWhite(values);

        }

        private async Task DoWriteWhite(List<UxTextValue> values)
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

                Byte White;
                // History: used to go into White_White.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedWhite = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out White);
                valueIndex++; // Change #5
                if (!parsedWhite)
                {
                    parseError = "White";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteWhite(White);
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



        public class SetColorRecord : INotifyPropertyChanged
        {
            public SetColorRecord()
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

            private double _Red;
            public double Red { get { return _Red; } set { if (value == _Red) return; _Red = value; OnPropertyChanged(); } }
            private double _Green;
            public double Green { get { return _Green; } set { if (value == _Green) return; _Green = value; OnPropertyChanged(); } }
            private double _Blue;
            public double Blue { get { return _Blue; } set { if (value == _Blue) return; _Blue = value; OnPropertyChanged(); } }
            private double _White;
            public double White { get { return _White; } set { if (value == _White) return; _White = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

    public DataCollection<SetColorRecord> SetColorRecordData { get; } = new DataCollection<SetColorRecord>();
    private void OnSetColor_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var text = (sender as TextBox).Text.Trim();
            (sender as TextBox).Text = "";
            // Add the text to the notes section
            if (SetColorRecordData.Count == 0)
            {
                SetColorRecordData.AddRecord(new SetColorRecord());
            }
            SetColorRecordData[SetColorRecordData.Count - 1].Note = text;
            e.Handled = true;
        }
    }

    // Functions called from the expander
    private void OnKeepCountSetColor(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        SetColorRecordData.MaxLength = value;

        
    }

    private void OnAlgorithmSetColor(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count != 1) return;
        int value;
        var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
        if (!ok) return;
        SetColorRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
    }
    private void OnCopySetColor(object sender, RoutedEventArgs e)
    {
        // Copy the contents over...
        var sb = new System.Text.StringBuilder();
        sb.Append("EventDate,EventTime,Red,Green,Blue,White,Notes\n");
        foreach (var row in SetColorRecordData)
        {
            var time24 = row.EventTime.ToString("HH:mm:ss.f");
            sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Red},{row.Green},{row.Blue},{row.White},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
        }
        var str = sb.ToString();
        var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
        datapackage.SetText(str);
        Clipboard.SetContent(datapackage);
    }



        // CS+CHARACTERISTIC+WRITE+METHOD
        // OK to include this method even if there are no defined buttons
        private async void OnClickSetColor(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(SetColor_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_Green.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_Blue.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            //var text = (sender as Button).Tag as String;
            await DoWriteSetColor(values);

        }

        private async void OnWriteSetColor(object sender, RoutedEventArgs e)
        {
            var values = new List<UxTextValue>()
            {
                // e.g., new UxTextValue(SetColor_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_Red.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_Green.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_Blue.Text, System.Globalization.NumberStyles.AllowHexSpecifier),
                new UxTextValue(SetColor_White.Text, System.Globalization.NumberStyles.AllowHexSpecifier),

            };
            await DoWriteSetColor(values);

        }

        private async Task DoWriteSetColor(List<UxTextValue> values)
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

                Byte Red;
                // History: used to go into SetColor_Red.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedRed = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Red);
                valueIndex++; // Change #5
                if (!parsedRed)
                {
                    parseError = "Red";
                }
                Byte Green;
                // History: used to go into SetColor_Green.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedGreen = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Green);
                valueIndex++; // Change #5
                if (!parsedGreen)
                {
                    parseError = "Green";
                }
                Byte Blue;
                // History: used to go into SetColor_Blue.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedBlue = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out Blue);
                valueIndex++; // Change #5
                if (!parsedBlue)
                {
                    parseError = "Blue";
                }
                Byte White;
                // History: used to go into SetColor_White.Text instead of using the variable
                // History: used to used DEC_OR_HEX for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedWhite = Utilities.Parsers.TryParseByte(values[valueIndex].Text, values[valueIndex].Dec_or_hex, null, out White);
                valueIndex++; // Change #5
                if (!parsedWhite)
                {
                    parseError = "White";
                }

                if (parseError == null)
                {
                    await bleDevice.WriteSetColor(Red, Green, Blue, White);
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