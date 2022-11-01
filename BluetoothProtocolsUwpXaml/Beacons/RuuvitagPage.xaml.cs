using BluetoothDeviceController.Charts;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

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
            }
        }
        bool chartsInitialized = false;
        private void InitializeCharts(SensorDataRecord firstRecord=null)
        {
            if (chartsInitialized) return;
            chartsInitialized = true;
            var EventTimeProperty = typeof(SensorDataRecord).GetProperty("EventTime");
            var chartProperties = new List<System.Reflection.PropertyInfo>();
            var noteProperties = new List<System.Reflection.PropertyInfo>();
            var names = new List<string>();

            if (firstRecord == null)
            {
                names.Add("Temperature");
                names.Add("Pressure");
                names.Add("Humidity");
            }
            else
            {
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Temperature)) names.Add("Temperature");
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Pressure)) names.Add("Pressure");
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Humidity)) names.Add("Humidity");
            }
            noteProperties.Add(EventTimeProperty);
            foreach (var name in names)
            {
                noteProperties.Add(typeof(SensorDataRecord).GetProperty(name));
                chartProperties.Add(typeof(SensorDataRecord).GetProperty(name));
            }
            Sensor_DataChart.SetDataProperties(chartProperties, EventTimeProperty, names);
            Sensor_DataChart.SetTitle("Sensor Data Chart");
            Sensor_DataChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
            {
                tableType = "standard",
                chartType = "standard",
                chartCommand = "AddYTime<Sensor_PeriodRecord>(addResult, Sensor_PeriodRecordData)",
                chartYAxisCombined = Names.UISpecifications.YMinMaxCombined.Separate,
                chartDefaultMinY0 = 15, // Common indoor temperature, C
                chartDefaultMaxY0 = 25,
                chartDefaultMinY1 = 990, // common pressure mbar
                chartDefaultMaxY1 = 1050, 
                chartDefaultMinY2 = 0, // Y2 is Humidity per the name and chartProperties listings
                chartDefaultMaxY2 = 100,
            };

            // Update the UX as needed
            if (!names.Contains("Temperature")) Sensor_Data_Temperature.Visibility = Visibility.Collapsed;
            if (!names.Contains("Pressure")) Sensor_Data_Pressure.Visibility = Visibility.Collapsed;
            if (!names.Contains("Humidity")) Sensor_Data_Humidity.Visibility = Visibility.Collapsed;

            // Have to add the EventTime so that the summary box is correct.
            SensorDataRecordData.TProperties = noteProperties.ToArray();
        }

        private async void Di_UpdatedRuuviAdvertisement(SensorDataRecord record)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
            {
                // Add to the text box!
                InitializeCharts(record);

                uiResults.Text += record.ToString() + "\n";

                Sensor_Data_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Humidity.Text = record.Humidity.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Pressure.Text = record.Pressure.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                var addResult = SensorDataRecordData.AddRecord(record);
                Sensor_DataChart.AddYTime<SensorDataRecord>(addResult, SensorDataRecordData);
            });
        }
        private async void Di_UpdatedGoveeAdvertisement(BluetoothProtocols.Beacons.Govee goveeRecord)
        {
            //TODO: make a more generic data type?
            var record = goveeRecord as SensorDataRecord;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Add to the text box!
                uiResults.Text += record.ToString() + "\n";

                Sensor_Data_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Humidity.Text = record.Humidity.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Pressure.Text = record.Pressure.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                var addResult = SensorDataRecordData.AddRecord(record);
                Sensor_DataChart.AddYTime<SensorDataRecord>(addResult, SensorDataRecordData);
            });
        }
        public DataCollection<SensorDataRecord> SensorDataRecordData { get; } = new DataCollection<SensorDataRecord>();
        

        private void OnCopySensor_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime,Temperature,Pressure,Humidity,Notes\n");
            foreach (var row in SensorDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24},{row.Temperature},{row.Pressure},{row.Humidity},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }

        private void OnSensor_Data_NoteKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var text = (sender as TextBox).Text.Trim();
                (sender as TextBox).Text = "";
                // Add the text to the notes section
                if (SensorDataRecordData.Count == 0)
                {
                    SensorDataRecordData.AddRecord(new SensorDataRecord());
                }
                SensorDataRecordData[SensorDataRecordData.Count - 1].Note = text;
                e.Handled = true;
            }
        }

        private void OnKeepCountSensor_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            SensorDataRecordData.MaxLength = value;

            Sensor_DataChart.RedrawYTime<SensorDataRecord>(SensorDataRecordData);
        }

        private void OnAlgorithmSensor_Data(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            int value;
            var ok = Int32.TryParse((e.AddedItems[0] as FrameworkElement).Tag as string, out value);
            if (!ok) return;
            SensorDataRecordData.RemoveAlgorithm = (RemoveRecordAlgorithm)value;
        }
    }
}
