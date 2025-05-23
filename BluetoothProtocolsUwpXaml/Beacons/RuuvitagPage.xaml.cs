﻿using BluetoothDeviceController.Charts;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Net;
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
        int index = 0;
        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            var di = args.Parameter as DeviceInformationWrapper;
            if (di.BleAdvert != null)
            {
                di.BleAdvert.UpdatedRuuviAdvertisement += Di_UpdatedRuuviAdvertisement;
            }
            index = 0;
        }
        bool chartsInitialized = false;
        List<string> SensorPropertyNames = new List<string>();
        // List of Properties for each of the SensorPropertiesNames
        List<System.Reflection.PropertyInfo> ChartProperties = new List<System.Reflection.PropertyInfo>();
        private void InitializeCharts(SensorDataRecord firstRecord=null)
        {
            if (chartsInitialized) return;
            chartsInitialized = true;
            var EventTimeProperty = typeof(SensorDataRecord).GetProperty("EventTime");
            var noteProperties = new List<System.Reflection.PropertyInfo>(); // Same as ChartProperties but with EventTime added.

            // SensorDataRecord: Update
            if (firstRecord == null)
            {
                SensorPropertyNames.Add("Temperature");
                SensorPropertyNames.Add("Pressure");
                SensorPropertyNames.Add("Humidity");
                SensorPropertyNames.Add("PM25");
            }
            else
            {
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Temperature)) SensorPropertyNames.Add("Temperature");
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Pressure)) SensorPropertyNames.Add("Pressure");
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Humidity)) SensorPropertyNames.Add("Humidity");
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.PM25)) SensorPropertyNames.Add("PM25");
            }
            noteProperties.Add(EventTimeProperty);
            foreach (var name in SensorPropertyNames)
            {
                noteProperties.Add(typeof(SensorDataRecord).GetProperty(name));
                ChartProperties.Add(typeof(SensorDataRecord).GetProperty(name));
            }
            Sensor_DataChart.SetDataProperties(ChartProperties, EventTimeProperty, SensorPropertyNames);
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
                chartDefaultMinY2 = 0, // Y2 is Humidity per the name and ChartProperties listings
                chartDefaultMaxY2 = 100,
            };
            if (firstRecord != null)
            {
                int index = 0;
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Temperature))
                {
                    Sensor_DataChart.UISpec.SetChartDefaultMinY(index, 15); // Common indoor temperature, C
                    Sensor_DataChart.UISpec.SetChartDefaultMaxY(index, 25);
                    index++;
                }
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Pressure))
                {
                    Sensor_DataChart.UISpec.SetChartDefaultMinY(index, 990); // common pressure mbar
                    Sensor_DataChart.UISpec.SetChartDefaultMaxY(index, 1050);
                    index++;
                }
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Humidity))
                {
                    Sensor_DataChart.UISpec.SetChartDefaultMinY(index, 0);
                    Sensor_DataChart.UISpec.SetChartDefaultMaxY(index, 100);
                    index++;
                }
                if (firstRecord.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.PM25))
                {
                    Sensor_DataChart.UISpec.SetChartDefaultMinY(index, 0); // not sure what the best default here is.
                    Sensor_DataChart.UISpec.SetChartDefaultMaxY(index, 50);
                    index++;
                }

            }
            // Update the UX as needed
            // SensorDataRecord: Update
            if (!SensorPropertyNames.Contains("Temperature")) Sensor_Data_Temperature.Visibility = Visibility.Collapsed;
            if (!SensorPropertyNames.Contains("Humidity")) Sensor_Data_Humidity.Visibility = Visibility.Collapsed;
            if (!SensorPropertyNames.Contains("Pressure")) Sensor_Data_Pressure.Visibility = Visibility.Collapsed;
            if (!SensorPropertyNames.Contains("PM25")) Sensor_Data_PM25.Visibility = Visibility.Collapsed;

            // Have to add the EventTime so that the summary box is correct.
            SensorDataRecordData.TProperties = noteProperties.ToArray();

            Sensor_DataGrid.ItemsSource = SensorDataRecordData; // Have to delay setting the ItemsSource until SensorPropertyNames is set.
        }


        private async void Di_UpdatedRuuviAdvertisement(SensorDataRecord record)
        {
            if (record == null) return; 
            var now = DateTime.Now;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => 
            {
                // Add to the text box!
                InitializeCharts(record);

                if (index % 5 == 0) uiResults.Text = ""; // clear every n-th item
                index++;
                uiResults.Text += $"{index}: {now} {record}\n";

                // SensorDataRecord: Update
                Sensor_Data_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Humidity.Text = record.Humidity.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Pressure.Text = record.Pressure.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_PM25.Text = record.PM25.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                var addResult = SensorDataRecordData.AddRecord(record);
                Sensor_DataChart.AddYTime<SensorDataRecord>(addResult, SensorDataRecordData);
            });
        }
        private async void Di_UpdatedGoveeAdvertisement(BluetoothProtocols.Beacons.Govee goveeRecord)
        {
            var record = goveeRecord as SensorDataRecord;

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // Add to the text box!
                uiResults.Text += record.ToString() + "\n";

                // SensorDataRecord: Update
                Sensor_Data_Temperature.Text = record.Temperature.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Humidity.Text = record.Humidity.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_Pressure.Text = record.Pressure.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                Sensor_Data_PM25.Text = record.PM25.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                var addResult = SensorDataRecordData.AddRecord(record);
                Sensor_DataChart.AddYTime<SensorDataRecord>(addResult, SensorDataRecordData);
            });
        }
        public DataCollection<SensorDataRecord> SensorDataRecordData { get; } = new DataCollection<SensorDataRecord>();
        

        private void OnCopySensor_Data(object sender, RoutedEventArgs e)
        {
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("EventDate,EventTime");
            foreach (var name in SensorPropertyNames)
            {
                sb.Append(",");
                sb.Append(name);
            }
            sb.Append(",Notes\n");
            foreach (var row in SensorDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss.f");
                sb.Append($"{row.EventTime.ToShortDateString()},{time24}");
                foreach (var property in ChartProperties)
                {
                    var value = (double)property.GetValue(row);
                    sb.Append(",");
                    sb.Append(value);
                }
                sb.Append($",{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
                //,{row.Temperature},{row.Pressure},{row.Humidity},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            var str = sb.ToString();
            var datapackage = new DataPackage() { RequestedOperation = DataPackageOperation.Copy };
            datapackage.SetText(str);
            Clipboard.SetContent(datapackage);
        }
        private void OnCopyForExcelSensor_Data(object sender, RoutedEventArgs e)
        {
            // See http://sunriseprogrammer.blogspot.com/2022/09/clipboard-data-for-excel.html for why!
            // Copy the contents over...
            var sb = new System.Text.StringBuilder();
            sb.Append("<html><table><tr><td>EventDate</td><td>EventTime</td>");
            foreach (var name in SensorPropertyNames)
            {
                sb.Append($"<td>{name}</td>");
            }
            sb.Append($"<td>Notes</td>");
            foreach (var row in SensorDataRecordData)
            {
                var time24 = row.EventTime.ToString("HH:mm:ss");
                sb.Append($"<tr><td>{row.EventTime.ToShortDateString()}</td><td>{time24}</td>");
                foreach (var property in ChartProperties)
                {
                    var value = (double)property.GetValue(row);
                    sb.Append($"<td>{value}</td>");
                }
                var note = WebUtility.HtmlEncode(row.Note);
                sb.Append($"<td>{note}</td></tr>\n"); 
                //,{row.Temperature},{row.Pressure},{row.Humidity},{AdvancedCalculator.BCBasic.RunTimeLibrary.RTLCsvRfc4180.Encode(row.Note)}\n");
            }
            sb.Append("</table></html>");
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

        private void SensorDataGrid_AutoGeneratingColumn(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            var colName = e.Column.Header.ToString();
            switch (colName)
            {
                case "EventTime": // keep this
                case "Note":
                    break;
                case "IsSensorPresent": // never keep this
                    e.Cancel = true;
                    break;
                default:
                    // Only include columns which are included in the sensor data record.
                    if (!SensorPropertyNames.Contains (colName))
                    {
                        e.Cancel = true;
                    }
                    break;
            }
        }
    }
}
