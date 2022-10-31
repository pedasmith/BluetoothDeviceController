using BluetoothDeviceController.Charts;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static BluetoothDeviceController.Beacons.Ruuvi_Tag_v1_Helper;

namespace BluetoothDeviceController.Beacons
{
    // Possible icon:  MICROSOFT SYMBOL APPBAR GLYPH WEBSITE

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RuuvitagPage : Page
    {
        public RuuvitagPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformationWrapper;
            if (di.BleAdvert != null)
            {
                di.BleAdvert.UpdatedRuuviAdvertisement += Di_UpdatedRuuviAdvertisement;
                di.BleAdvert.UpdatedGoveeAdvertisement += Di_UpdatedGoveeAdvertisement;
            }
            InitializeCharts();
        }

        private void InitializeCharts()
        {
            var EventTimeProperty = typeof(Ruuvi_DataRecord).GetProperty("EventTime");
            var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(Ruuvi_DataRecord).GetProperty("Temperature"),
typeof(Ruuvi_DataRecord).GetProperty("Pressure"),
typeof(Ruuvi_DataRecord).GetProperty("Humidity"),
                };
            var names = new List<string>()
                {
"Temperature",
"Pressure",
"Humidity",
                };
            Ruuvi_DataChart.SetDataProperties(properties, EventTimeProperty, names);
            Ruuvi_DataChart.SetTitle("Ruuvi Data Chart");
            Ruuvi_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
            {
                tableType = "standard",
                chartType = "standard",
                chartCommand = "AddYTime<Ruuvi_PeriodRecord>(addResult, Ruuvi_PeriodRecordData)",
                chartYAxisCombined = Names.UISpecifications.YMinMaxCombined.Separate,
                chartDefaultMinY0 = 15, // Common indoor temperature, C
                chartDefaultMaxY0 = 25,
                chartDefaultMinY1 = 990, // common pressure mbar
                chartDefaultMaxY1 = 1050, 
                chartDefaultMinY2 = 0, // Y2 is Humidity per the name and properties listings
                chartDefaultMaxY2 = 100,
            };
        }

        private async void Di_UpdatedRuuviAdvertisement(Ruuvi_Tag_v1_Helper.Ruuvi_DataRecord record)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
            {
                // Add to the text box!
                uiResults.Text += record.ToString() + "\n";

                Ruuvi_Data_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Ruuvi_Data_Humidity.Text = record.Humidity.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Ruuvi_Data_Pressure.Text = record.Pressure.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                var addResult = Ruuvi_DataRecordData.AddRecord(record);
                Ruuvi_DataChart.AddYTime<Ruuvi_DataRecord>(addResult, Ruuvi_DataRecordData);
            });
        }
        private async void Di_UpdatedGoveeAdvertisement(BluetoothProtocols.Beacons.Govee goveeRecord)
        {
            //TODO: make a more generic data type?
            var record = new Ruuvi_Tag_v1_Helper.Ruuvi_DataRecord();
            record.Pressure = 0;
            record.Temperature = goveeRecord.TemperatureInDegreesC;
            record.Humidity = goveeRecord.HumidityInPercent;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Add to the text box!
                uiResults.Text += record.ToString() + "\n";

                Ruuvi_Data_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Ruuvi_Data_Humidity.Text = record.Humidity.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Ruuvi_Data_Pressure.Text = record.Pressure.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                var addResult = Ruuvi_DataRecordData.AddRecord(record);
                Ruuvi_DataChart.AddYTime<Ruuvi_DataRecord>(addResult, Ruuvi_DataRecordData);
            });
        }
        public DataCollection<Ruuvi_DataRecord> Ruuvi_DataRecordData { get; } = new DataCollection<Ruuvi_DataRecord>();
        

        private void OnCopyRuuvi_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Temperature,Pressure,Humidity,Notes\n");
            foreach (var row in Ruuvi_DataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Temperature},{row.Pressure},{row.Humidity},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private void OnRuuvi_Data_NoteKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (Ruuvi_DataRecordData.Count == 0)
                {
                    Ruuvi_DataRecordData.AddRecord(new Ruuvi_DataRecord());
                }
                Ruuvi_DataRecordData[Ruuvi_DataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        private void OnKeepCountRuuvi_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Ruuvi_DataRecordData.MaxLength = value;

            Ruuvi_DataChart.RedrawYTime<Ruuvi_DataRecord>(Ruuvi_DataRecordData);
        }

        private void OnAlgorithmRuuvi_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            Ruuvi_DataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
    }
}
