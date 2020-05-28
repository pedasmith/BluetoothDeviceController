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
    public sealed partial class Elegoo_MiniCarPage : Page, HasId, ISetHandleStatus
    {
        public Elegoo_MiniCarPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        private string DeviceName = "Elegoo_MiniCar";
        private string DeviceNameUser = "ELEGOO BT16";

        int ncommand = 0;
        Elegoo_MiniCar bleDevice = new Elegoo_MiniCar();
        protected async override void OnNavigatedTo(NavigationEventArgs args)
        {
            SetStatusActive(true);
            var di = args.Parameter as DeviceInformationWrapper;
            var ble = await BluetoothLEDevice.FromIdAsync(di.di.Id);
            SetStatusActive(false);

            bleDevice.ble = ble;
            bleDevice.Status.OnBluetoothStatus += bleDevice_OnBluetoothStatus;

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

        // Functions for Car


        public class ResultRecord : INotifyPropertyChanged
        {
            public ResultRecord()
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

            private string _Result;
            public string Result { get { return _Result; } set { if (value == _Result) return; _Result = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }
        }

        public DataCollection<ResultRecord> ResultRecordData { get; } = new DataCollection<ResultRecord>();
        private void OnResult_NoteKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (ResultRecordData.Count == 0)
                {
                    ResultRecordData.AddRecord(new ResultRecord());
                }
                ResultRecordData[ResultRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        // Functions called from the expander
        private void OnKeepCountResult(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ResultRecordData.MaxLength = value;


        }

        private void OnAlgorithmResult(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            ResultRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
        private void OnCopyResult(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Result,Notes\n");
            foreach (var row in ResultRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Result},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        GattClientCharacteristicConfigurationDescriptorValue[] NotifyResultSettings = {
            GattClientCharacteristicConfigurationDescriptorValue.Notify,

            GattClientCharacteristicConfigurationDescriptorValue.None,
        };
        int ResultNotifyIndex = 0;
        bool ResultNotifySetup = false;
        private async void OnNotifyResult(object sender, RoutedEventArgs e)
        {
            await DoNotifyResult();
        }

        private async Task DoNotifyResult()
        {
            SetStatusActive(true);
            ncommand++;
            try
            {
                // Only set up the event callback once.
                if (!ResultNotifySetup)
                {
                    ResultNotifySetup = true;
                    bleDevice.ResultEvent += BleDevice_ResultEvent;
                }
                var notifyType = NotifyResultSettings[ResultNotifyIndex];
                ResultNotifyIndex = (ResultNotifyIndex + 1) % NotifyResultSettings.Length;
                var result = await bleDevice.NotifyResultAsync(notifyType);



            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
        }

        private async void BleDevice_ResultEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    var valueList = data.ValueList;

                    var record = new ResultRecord();

                    var Result = valueList.GetValue("Result");
                    if (Result.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Result.CurrentType == BCBasic.BCValue.ValueType.IsString)
                    {
                        record.Result = (string)Result.AsString;
                        Result_Result.Text = record.Result.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    }

                    var addResult = ResultRecordData.AddRecord(record);

                });
            }
        }
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

                String Command;
                // History: used to go into Command_Command.Text instead of using the variable
                // History: used to used System.Globalization.NumberStyles.AllowHexSpecifier for parsing instead of the newer dec_or_hex variable that's passed in
                var parsedCommand = Utilities.Parsers.TryParseString(text, dec_or_hex, null, out Command);
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


        private async void OnRereadDevice(object sender, RoutedEventArgs e)
        {
            SetStatus("Reading device");
            SetStatusActive(true);
            await bleDevice.EnsureCharacteristicAsync(true);
            SetStatusActive(false);
        }
    }
}
