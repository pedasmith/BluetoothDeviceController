using BluetoothDeviceController.Charts;
using BluetoothProtocols;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Isolation;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static BluetoothDeviceController.SpecialtyPages.PokitProMeterPage;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.SpecialtyPagesCustom
{

    public class OscDataRecord
    {
        public OscDataRecord() { }
        public OscDataRecord(DateTime time, double value)
        {
            EventTime = time;
            Value = value;
        }
        public DateTime EventTime { get; set; }
        public double Value { get; set; }

        public override string ToString()
        {
            return $"{Value:F2} at {EventTime.Second}.{EventTime.Millisecond}";
        }
    }

    public sealed partial class OscilloscopeControl : UserControl
    {
        private DataCollection<OscDataRecord> MMData = new DataCollection<OscDataRecord>();


        public enum ConnectionState { Off, Configuring, Configured, GotData, GotDataStale, Deconfiguring, Failed };

        public enum OscTriggerType {  FreeRunning=0, TriggerRisingEdge=1,TriggerFallingEdge=2, ResendData=3 };
        public enum OscDataMode {  Idle=0, VDCCouple=1, VACCouple=2, CurrentDCCouple=3, CurrentACCouple=4 }
        public enum OscVRangeVMax {  V300mV=0, V02V=1, V06V=2, V12V=3, V30=4, V60=5}
        // TODO: RangeAmpMax, too.

        TriggerSetting TriggerSetting { get; } = new TriggerSetting();
        IChartControlOscilloscope uiChart;




        public OscilloscopeControl()
        {
            this.InitializeComponent();
            uiChart = uiChartRaw;
            uiChartRaw.OnPointerPosition += UiChartRaw_OnPointerPosition;
            this.Loaded += OscilloscopeControl_Loaded;
        }

        #region DEVICE_STATE
        PokitProMeter bleDevice = null;

        private ConnectionState _BtConnectionState = ConnectionState.Off;
        public ConnectionState BtConnectionState
        {
            get { return _BtConnectionState; }
            internal set
            {
                if (_BtConnectionState == value) return;
                _BtConnectionState = value;
                UIThreadHelper.CallOnUIThread(UpdateConnectionState);
            }
        }


        private void UpdateConnectionState()
        {
            uiState.Text = BtConnectionState.ToString();
        }
 
        private async void OscilloscopeControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(0); // Just to the compiler doesn't complain


            // Set up the graph
            // TODO: the MMData.MaxLength is the length of the data for the uiChart.
            MMData.RemoveAlgorithm = RemoveRecordAlgorithm.RemoveFirst;
            MMData.MaxLength = 8000;

            var EventTimeProperty = typeof(OscDataRecord).GetProperty("EventTime");
            var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(OscDataRecord).GetProperty("Value"),
                };
            var names = new List<string>()
                {
"Osc.",
                };
            uiChart.SetDataProperties(properties, EventTimeProperty, names);
            uiChart.SetTitle("Oscilloscope");
            uiChart.SetUISpec(new BluetoothDeviceController.Names.UISpecifications()
            {
                tableType = "standard",
                chartType = "standard",
                chartCommand = "AddYTime<MagnetometerCalibrationRecord>(addResult, MMRecordData)", //TODO: What should the chart command be>???
                chartDefaultMaxY = 5.0, // TODO: what's the best value here? 10_000,
                chartDefaultMinY = 0,
            });
;
        }
        private async Task ConnectCallbacksAsync()
        {
            if (BtConnectionState != ConnectionState.Off && BtConnectionState != ConnectionState.Failed && BtConnectionState != ConnectionState.Configuring)
            {
                return;
            }
            if (bleDevice == null)
            {
                return;
            }
            bleDevice.DSO_ReadingEvent += BleDevice_DSO_ReadingEvent;
            bleDevice.DSO_MetadataEvent += BleDevice_DSO_MetadataEvent;

            // Copied from the Pokit_ProPage.xaml.cs
            BtConnectionState = ConnectionState.Configuring;
            var result1 = await bleDevice.NotifyDSO_ReadingAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            var result2 = await bleDevice.NotifyDSO_MetadataAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            BtConnectionState = result1 &&  result2 ? ConnectionState.Configured : ConnectionState.Failed;
        }
        public async Task SetMeter(PokitProMeter value)
        {
            BtConnectionState = ConnectionState.Off; // Assume the new value isn't connected
            bleDevice = value;
            await Task.Delay(0); // added this only so that the compiler warning for async are turned off.
        }
        private void Log(string str)
        {
            UIThreadHelper.CallOnUIThread(() =>
            {
                uiLog.Text += str + "\n";
            });
        }

        private async void OnConnect(object sender, RoutedEventArgs e)
        {
            if (bleDevice == null) return;

            BtConnectionState = ConnectionState.Configuring;
            await ConnectCallbacksAsync();
        }

        private async void OnDisconnect(object sender, RoutedEventArgs e)
        {
            if (bleDevice == null)
            {
                return;
            }
            bleDevice.DSO_ReadingEvent -= BleDevice_DSO_ReadingEvent;
            bleDevice.DSO_MetadataEvent -= BleDevice_DSO_MetadataEvent;

            // TODO: Set to idle
            var triggerType = OscTriggerType.FreeRunning;
            var triggerLevel = 3.0f; 
            var datamode = OscDataMode.Idle;
            var range = OscVRangeVMax.V12V;
            ushort nSamples = 10; // Idle doesn't really need many samples
            UInt32 timePerSampleInMicroseconds = 5; // TODO: Kind of arbitrary.
            UInt32 samplingWindow = nSamples * timePerSampleInMicroseconds; // total time

            if (bleDevice == null) return;

            await bleDevice.WriteDSO_Settings((byte)triggerType, triggerLevel, (byte)datamode, (byte)range, samplingWindow, nSamples);


            var result1 = await bleDevice.NotifyDSO_ReadingAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            var result2 = await bleDevice.NotifyDSO_MetadataAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
            BtConnectionState = result1 && result2 ? ConnectionState.Off : ConnectionState.Failed;
        }

        #endregion DEVICE_STATE




        #region READ_DATA
        /// <summary>
        /// The ChartControl is able to display multiple lines of data at once, overlapping them
        /// as neeed. To make that work, we have a "CurrLineIndex" which is the index of the line
        /// in the ChartControl we're going to write into.
        /// 
        /// The value wraps at a max value MAX_LINES
        /// </summary>
        private int CurrLineIndex = 0;
        private const int MAX_LINES = 5;
        private void IncrementCurrLineIndex()
        {
            CurrLineIndex++;
            if (CurrLineIndex >= MAX_LINES)
            {
                CurrLineIndex = 0;
            }
        }

        /// <summary>
        /// Helpful value when debugging -- says how many "Metadata" events we've gotten. The 
        /// Oscilloscope will first send a Metadata event before all of the read events
        /// </summary>
        int DSO_NMetadataEvents_Trace = 0;
        int DSO_NReadEvents_Trace = 0;

        /// <summary>
        /// Number of Read values are left before we're done. We know ahead of time how many
        /// values to expect (e.g., we ask for exactly 500 samples, so that's what we get)
        /// </summary>
        int DSO_NReadingsLeft = 0;

        //int Curr_DSO_NMetadataEvents = -10;
        //DateTime ReadingStartTime = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        long ReadingDeltaInTicks = 100; // Good default; matches the "10 microseconds per sample" set in 2024-06-08
        List<double> RawReadings = new List<double>();
        double DsoScale = 1.0;


        /// <summary>
        /// Called when the DSO Oscilloscope wants to tell the system what the data format is. AFAICT, this is
        /// always sent before the data. Pull the data out on this thread; don't wait until stuff is called
        /// on the UX thread.
        /// </summary>
        /// <param name="data"></param>
        private async void BleDevice_DSO_MetadataEvent(BleEditor.ValueParserResult data)
        {
            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                DsoScale = data.ValueList.GetValue("DsoDataScale").AsDouble; // Change: grab this earlier.
                DSO_NMetadataEvents_Trace++;
                System.Diagnostics.Debug.WriteLine($"DBG: DSO_MetaDataEvent: got event {DSO_NMetadataEvents_Trace} new scale={DsoScale}");

                var record = new DSO_MetadataRecord();
                var valueList = data.ValueList;
                var DsoStatus = valueList.GetValue("DsoStatus");
                if (DsoStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoStatus.IsArray)
                {
                    record.DsoStatus = (double)DsoStatus.AsDouble;
                }
                var DsoDataScale = valueList.GetValue("DsoDataScale");
                if (DsoDataScale.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataScale.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataScale.IsArray)
                {
                    record.DsoDataScale = (double)DsoDataScale.AsDouble;
                }
                var DsoDataMode = valueList.GetValue("DsoDataMode");
                if (DsoDataMode.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataMode.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataMode.IsArray)
                {
                    record.DsoDataMode = (double)DsoDataMode.AsDouble;
                }
                var DsoDataRange = valueList.GetValue("DsoDataRange");
                if (DsoDataRange.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataRange.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataRange.IsArray)
                {
                    record.DsoDataRange = (double)DsoDataRange.AsDouble;
                }

                var DsoDataSamplingWindow = valueList.GetValue("DsoDataSamplingWindow");
                if (DsoDataSamplingWindow.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataSamplingWindow.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataSamplingWindow.IsArray)
                {
                    record.DsoDataSamplingWindow = (double)DsoDataSamplingWindow.AsDouble;
                }

                var DsoDataNsamples = valueList.GetValue("DsoDataNsamples");
                if (DsoDataNsamples.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoDataNsamples.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoDataNsamples.IsArray)
                {
                    record.DsoDataNsamples = (double)DsoDataNsamples.AsDouble;
                }

                var DsoSamplingRate = valueList.GetValue("DsoSamplingRate");
                if (DsoSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DsoSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsString || DsoSamplingRate.IsArray)
                {
                    record.DsoSamplingRate = (double)DsoSamplingRate.AsDouble;
                }


                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    DSO_Metadata_DsoStatus.Text = record.DsoStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    DSO_Metadata_DsoDataScale.Text = record.DsoDataScale.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    DSO_Metadata_DsoDataMode.Text = record.DsoDataMode.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    DSO_Metadata_DsoDataRange.Text = record.DsoDataRange.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    DSO_Metadata_DsoDataSamplingWindow.Text = record.DsoDataSamplingWindow.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    DSO_Metadata_DsoDataNsamples.Text = record.DsoDataNsamples.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                    DSO_Metadata_DsoSamplingRate.Text = record.DsoSamplingRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                });
            }
        }

        /// <summary>
        /// Called when data is available from the Oscilloscope (DSO)
        /// </summary>
        /// <param name="data"></param>
        private void BleDevice_DSO_ReadingEvent(BleEditor.ValueParserResult data)
        {
            DSO_NReadEvents_Trace++;
            // Works like a champ; no need for logging: Log($"NRead={nread}");

            if (data.Result == BleEditor.ValueParserResult.ResultValues.Ok)
            {
                var valueList = data.ValueList;
                var array = valueList.GetValue("DsoDataRaw").AsArray.data;
                for (int i = 0; i < array.Count; i++)
                {
                    var value = array[i].AsDouble;
                    RawReadings.Add(value);
                }
                DSO_NReadingsLeft -= array.Count;


                if (DSO_NReadingsLeft <= 0) // NOTE: what happens if the BT fails?
                {
                    Utilities.UIThreadHelper.CallOnUIThread(() =>
                    {
                        MMData.ClearAllRecords();
                        MMData.MaxLength = RawReadings.Count;

                        MMData.CurrTimeStampType = DataCollection<OscDataRecord>.TimeStampType.FromZeroMS;
                        DateTime readingTime = DateTime.MinValue;  // and not at all ReadingStartTime;
                        MMData.TimeStampStart = readingTime; // force these to always be in sync :-)
                        for (int i = 0; i < RawReadings.Count; i++)
                        {
                            var mm = new OscDataRecord(readingTime, RawReadings[i] * DsoScale);
                            var addResult = MMData.AddRecord(mm);
                            readingTime = readingTime.AddTicks(ReadingDeltaInTicks);
                        }

                        TriggerIndexes = TriggerSetting.FindTriggeredIndex(MMData);
                        var triggerNominalTime = MMData[0].EventTime;


                        uiChartRaw.RedrawOscilloscopeYTime(CurrLineIndex, MMData, TriggerIndexes); // Push the data into the ChartControl
                        Log($"Got data: {MMData[0].Value:F3}");
                        IncrementCurrLineIndex();
                    });
                }
            }
        }

        #endregion READ_DATA


        private List<int> TriggerIndexes;

        private void UiChartRaw_OnPointerPosition(object sender, PointerPositionArgs e)
        {
            if (MMData.Count == 0) return;
            var index = MMData.GetItemAtOrBeforeIndex(e.Ratio);
            if (index < 0) return;

            var item = MMData[index] as OscDataRecord;
            if (item == null) return;
            uiCursorTime.Text = item.EventTime.Subtract(DateTime.MinValue).TotalMilliseconds.ToString("F3") + " ms";
            uiCursorValue.Text = item.Value.ToString("F3") + " V DC";

            // What the closest marker before this?
            int closestMarkerIndex = 0;
            foreach (var triggerIndex in TriggerIndexes)
            {
                if (triggerIndex >= index) break;
                closestMarkerIndex = triggerIndex;
            }
            var marker = MMData[closestMarkerIndex]; // before or at the cursor
            var delta = item.EventTime.Subtract(marker.EventTime).TotalMilliseconds;
            uiCursorDeltaTime.Text = delta.ToString("F3") + " ms";

            // Shouldn't this be measured trigger-to-trigger?
            var freq = Math.Abs (1.0 / (delta / 1000.0));
            var freqtext = freq.ToString("F2");
            if (freq > 100) freqtext = freq.ToString("F1");
            else if (freq > 500) freqtext = freq.ToString("F0");
            uiCursorDeltaFreq.Text = freqtext;
        }



        /// <summary>
        /// Called when the user clicks the "Data" button. When data actually flows in, that's BleDevice_DSO_ReadingEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnData(object sender, RoutedEventArgs e)
        {
            await DoOneSweep();
        }

        int NRequest = 0;
        /// <summary>
        /// Asks the PokitPro to get data, then get all the resulting data and display it. 
        /// </summary>
        private async Task DoOneSweep()
        { 
            // Clear the log area
            UIThreadHelper.CallOnUIThread(() =>
            {
                NRequest++;
                uiLog.Text = $""; //  Request Data {NRequest}\n";
            });

            // TODO: get this all hooked up to some UX to control it. And do continuous readings!
            var triggerType = OscTriggerType.FreeRunning;
            var triggerLevel = 3.0f; // TODO: select
            var datamode = OscDataMode.VDCCouple;
            var range = OscVRangeVMax.V30;


            if (bleDevice == null) return;

            ushort nSamples = 500; // TODO: allow for settings not too many for testing!
            UInt32 timePerSampleInMicroseconds = 10; // FYI: there are 10 C# ticks per microsecond

            nSamples = 1000;
            //timePerSampleInMicroseconds = 125;

            // Examples of nSamples, timePerSample and samplingWindow sizes
            // NSamples     timePerSamples      samplingWindowSize
            //  500             10                   5 milliseconds
            // 2000             10                  20 milliseconds
            // 8000             10                  80 milliseconds
            // 2000            500                     1 second
            // 8000            125                     1 second

            // Also FYI: a 1000 Hz wave is 1000 microseconds per cycke
            // So a 1000 Hz wave with 8000 samples will have 8 samples per cycle

            // What the DSO needs is the nSamples and the samplingWindowsInMicroseconds.
            // These aren't really very user-friendly.
            UInt32 samplingWindowInMicroseconds = nSamples * timePerSampleInMicroseconds; // total time
            DSO_NReadingsLeft = nSamples;
            ReadingDeltaInTicks = timePerSampleInMicroseconds * 10;
            RawReadings.Clear();

            await bleDevice.WriteDSO_Settings((byte)triggerType, triggerLevel, (byte)datamode, (byte)range, samplingWindowInMicroseconds, nSamples);

        }

    }
}
