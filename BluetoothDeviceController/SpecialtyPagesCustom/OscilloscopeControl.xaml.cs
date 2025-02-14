﻿using BluetoothDeviceController.Charts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Utilities;
using System.Reflection;
using BluetoothDeviceController.Names;
using Windows.UI.Xaml.Media;
using BluetoothProtocolsUwpXaml.ChartControl;
using System.Runtime.InteropServices.WindowsRuntime;

#if !NO_BT

using BluetoothProtocols;
using static BluetoothDeviceController.SpecialtyPages.PokitProMeterPage;
#endif

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.SpecialtyPagesCustom
{


    public sealed partial class OscilloscopeControl : UserControl, IChartControlOscilloscope
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
            UserPersonalization.Init();

            this.InitializeComponent();
            SetupPersonalizationList();
            uiChart = uiChartRaw;
            uiChartRaw.OnPointerPosition += UiChartRaw_OnPointerPosition;
            uiChartRaw.HandlePointerEvents = false; // This control will handle all of the pointer events, thanks.
            this.Loaded += OscilloscopeControl_Loaded;
        }

        #region DEVICE_STATE
#if !NO_BT
        PokitProMeter bleDevice = null;
#endif
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

        private void LogMini(string text)
        {
            uiState.Text = text;
        }
 
        private async void OscilloscopeControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(0); // Just to the compiler doesn't complain


            // Set up the graph
            // TODO: the MMData.MaxLength is the length of the data for the uiChart.
            MMData.RemoveAlgorithm = RemoveRecordAlgorithm.RemoveFirst;
            MMData.MaxLength = 8000;

            var EventTimeProperty = OscDataRecord.GetTimeProperty();
            var properties = OscDataRecord.GetValuePropertyList();
            var names = OscDataRecord.GetNames();

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

#if !NO_BT
            if (bleDevice == null)
            {
                return;
            }
#endif

            // Copied from the Pokit_ProPage.xaml.cs
            BtConnectionState = ConnectionState.Configuring;

#if !NO_BT
            bleDevice.DSO_ReadingEvent += BleDevice_DSO_ReadingEvent;
            bleDevice.DSO_MetadataEvent += BleDevice_DSO_MetadataEvent;
            var result1 = await bleDevice.NotifyDSO_ReadingAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            var result2 = await bleDevice.NotifyDSO_MetadataAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            BtConnectionState = (result1 &&  result2) ? ConnectionState.Configuring : ConnectionState.Failed;
#else
            BtConnectionState = ConnectionState.Configured;
            await Task.Delay(0);
#endif
        }

#if !NO_BT
        public async Task SetMeter(PokitProMeter value)
        {
            BtConnectionState = ConnectionState.Off; // Assume the new value isn't connected
            bleDevice = value;
            await Task.Delay(0); // added this only so that the compiler warning for async are turned off.
        }
#endif
        private void Log(string str)
        {
            UIThreadHelper.CallOnUIThread(() =>
            {
                uiLog.Text += str + "\n";
            });
        }

#if !NO_BT
        Status_DeviceRecord Curr_Status_DeviceRecord;
        Status_StatusRecord Curr_Status_StatusRecord;
        Status_Device_NameRecord Curr_Status_Device_NameRecord;
#endif
        private async void OnConnect(object sender, RoutedEventArgs e)
        {
#if !NO_BT

            if (bleDevice == null) return;

            BtConnectionState = ConnectionState.Configuring;
            await ConnectCallbacksAsync();

            //
            Curr_Status_DeviceRecord = await DoReadStatus_Device();
            Curr_Status_StatusRecord = await DoReadStatus_Status();
            Curr_Status_Device_NameRecord = await DoReadStatus_Device_Name();
            SetDeviceInfo(Curr_Status_DeviceRecord, Curr_Status_StatusRecord, Curr_Status_Device_NameRecord);
            bool isOK = Curr_Status_DeviceRecord != null
                && Curr_Status_StatusRecord != null
                && Curr_Status_Device_NameRecord != null;
            BtConnectionState = isOK ? ConnectionState.Configured : ConnectionState.Failed;
#else
            await Task.Delay(0);
#endif
        }

        private async void OnDisconnect(object sender, RoutedEventArgs e)
        {
#if !NO_BT
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
#else
            await Task.Delay(0);
#endif
        }

        #endregion DEVICE_STATE




#region READ_DATA

#if !NO_BT
        /// <summary>
        /// Helpful value when debugging -- says how many "Metadata" events we've gotten. The 
        /// Oscilloscope will first send a Metadata event before all of the read events
        /// </summary>
        int DSO_NMetadataEvents_Trace = 0;
        int DSO_NReadEvents_Trace = 0;
#endif

        /// <summary>
        /// Number of Read values are left before we're done. We know ahead of time how many
        /// values to expect (e.g., we ask for exactly 500 samples, so that's what we get)
        /// </summary>
        /// 
        int DSO_NReadingsExpected = 0;
        int DSO_NReadingsSoFar = 0;
        int DSO_NReadingsLeft {  get {  return DSO_NReadingsExpected - DSO_NReadingsSoFar; } }

        //int Curr_DSO_NMetadataEvents = -10;
        //DateTime ReadingStartTime = DateTime.MinValue;

        /// <summary>
        /// 
        /// </summary>
        long ReadingDeltaInTicks = 100; // Good default; matches the "10 microseconds per sample" set in 2024-06-08
        List<double> RawReadings = new List<double>();
        double DsoScale = 1.0;


#if !NO_BT
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

                //
                // TODO: Update with actual data! (isn't this done right here? what's left?)
                // TODO: 
                //
                DSO_NReadingsExpected = (int)record.DsoDataNsamples;
                ;

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
#endif

#if !NO_BT

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
                // lock (this) // TODO: DBG: Attempt #3: try locking this one?
                {
                    var valueList = data.ValueList;
                    var array = valueList.GetValue("DsoDataRaw").AsArray.data;
                    for (int i = 0; i < array.Count; i++)
                    {
                        var value = array[i].AsDouble;
                        RawReadings.Add(value);
                    }
                    DSO_NReadingsSoFar += array.Count;
                    System.Diagnostics.Debug.WriteLine($"NRead={DSO_NReadEvents_Trace} readings={array.Count} nleft={DSO_NReadingsLeft} so_far={DSO_NReadingsSoFar}");
                }

                if (DSO_NReadingsLeft <= 0) // NOTE: what happens if the BT fails?
                {
                    Utilities.UIThreadHelper.CallOnUIThread(() => { DrawOscilloscopeLine(); });
                }
            }
        }
#endif

        /// <summary>
        /// Once the RawReadings are read, convert to the MMData, pop in the right time stamps,
        /// and draw the resulting graph
        /// </summary>
        private void DrawOscilloscopeLine()
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

            //
            // Calculate the trigger locationss
            //
            TriggerIndexes = TriggerSetting.FindTriggeredIndex(MMData);

            //
            // Draw the line
            //
            var lineIndex = uiChart.GetNextOscilloscopeLine();
            AddToClearList(lineIndex);
            uiChart.RedrawOscilloscopeYTime(lineIndex, MMData, TriggerIndexes); // Push the data into the ChartControl
            UpdateReticuleScale();
            Log($"Got data: {MMData[0].Value:F3}");

        }

        private void UpdateReticuleScale()
        {
            uiReticuleScalePanel.Visibility = Visibility.Visible;
            var value = uiChartRaw.GetReticuleSpace();
            uiReticuleScale.Text = value;
        }
        #endregion READ_DATA


        private List<int> TriggerIndexes;

        private void UiChartRaw_OnPointerPosition(object sender, PointerPositionArgs e)
        {
            if (MMData.Count == 0) return;
            // BT state doesn't really matter here: if (BtConnectionState != ConnectionState.Configured) return;

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
            if (BtConnectionState != ConnectionState.Configured) return;
#if !NO_BT
            await DoOneSweep();
#else
            await Task.Delay(0);
#endif
        }
#if !NO_BT

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
            if (BtConnectionState != ConnectionState.Configured) return;

            int READING_SPARCITY = 8; // can be 1 2 4 8 16. 1 means as much resolution as possible, 16 as sparse as possible.

            // 2024-06-23 3:15: SPARCITY = 8 GOOD
            // 2024-06-23 3:22: SPARCITY = 4 GOOD for first half, then starts to fail.

            // the time window stays the same, so we always gather the same length of time.

            ushort nSamples; 

            var maxSamplesPerSecond = Curr_Status_DeviceRecord.MaxSamplingRate * 1000 / READING_SPARCITY; // value is in KHz. 1000 == 1 MHz sample rate
            var minMicrosecondsPerSample = (1.0 / maxSamplesPerSecond) * 1_000_000.0;
            UInt32 timePerSampleInMicroseconds = (UInt32)minMicrosecondsPerSample;


            nSamples = (UInt16)(Curr_Status_DeviceRecord.DeviceBufferSize / READING_SPARCITY); // Max number of samples
            nSamples = Math.Min(nSamples, (ushort)(Curr_Status_DeviceRecord.DeviceBufferSize - 1000)); // TODO: must reduce the amount, otherwise it doesn't work.



            // Examples of nSamples, timePerSample and samplingWindow sizes
            // NSamples     timePerSamples      samplingWindowSize
            //  500             10                   5 milliseconds
            // 2000             10                  20 milliseconds
            // 8000             10                  80 milliseconds
            // 2000            500                     1 second
            // 8000            125                     1 second

            // Also FYI: a 1000 Hz wave is 1000 microseconds per cycke
            // So a 1000 Hz wave with 8000 samples will have 8 samples per cycle

            // What the DSO needs during setup is the nSamples and the samplingWindowsInMicroseconds.
            // These aren't really very user-friendly.
            UInt32 samplingWindowInMicroseconds = nSamples * timePerSampleInMicroseconds; // total time

            DSO_NReadingsSoFar = 0;
            DSO_NReadingsExpected = nSamples; // will be updated by the metadata
            ReadingDeltaInTicks = timePerSampleInMicroseconds * 10;
            RawReadings.Clear();

            await bleDevice.WriteDSO_Settings((byte)triggerType, triggerLevel, (byte)datamode, (byte)range, samplingWindowInMicroseconds, nSamples);

            // Will call BleDevice_DSO_MetadataEvent to get the meta data
            // Will call BleDevice_DSO_ReadingEvent multiple times until all data is sent.
        }
#endif
        private void OnZoom(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement)?.Tag as string;
            if (string.IsNullOrEmpty(tag)) return;
            double value = 1.0;
            var parseok = double.TryParse(tag, out value);
            if (!parseok) return;
            uiChart.SetZoom(value); 
        }

        private void OnPan(object sender, RoutedEventArgs e)
        {
            var tag = (sender as FrameworkElement)?.Tag as string;
            if (string.IsNullOrEmpty(tag)) return;
            double value = 1.0;
            var parseok = double.TryParse(tag, out value);
            if (!parseok) return;
            uiChart.SetPan(value);
        }


        private List<int> ClearList = new List<int>();
        private void AddToClearList(int lineIndex)
        {
            ClearList.Remove(lineIndex);
            ClearList.Add(lineIndex);
        }
        private void OnClear(object sender, RoutedEventArgs e)
        {
            if (ClearList.Count < 1) return;

            var lastClearIndex = ClearList.Count - 1;   
            var lineIndexToClear = ClearList[lastClearIndex];
            ClearList.RemoveAt(lastClearIndex);

            uiChart.ClearLine(lineIndexToClear);
        }

        #region DEVICE_STATUS

        private void SetStatus(string str)
        {
            Log(str);
        }

        private void SetDeviceInfo(Status_DeviceRecord dr, Status_StatusRecord sr, Status_Device_NameRecord nr)
        {
            // Style note: only values are set here. Things like units which are fixed are already 
            // present in the XAML.
            uiName.Text = nr.Device_Name;
            uiFirmware.Text = $"{dr.FirmwareMajor}.{dr.FirmwareMinor}";
            uiBandwidth.Text = $"{(dr.MaxSamplingRate / 1000.0):F2}";

            uiBattery.Text = $"{sr.BatteryLevel:F2}";
            uiMaxVoltage.Text = $"{dr.MaxInputVoltage:F0}";
            uiMaxCurrent.Text = $"{dr.MaxInputCurrent:F0}";
            uiMaxResistance.Text = $"{dr.MaxInputResistance:F0}";

            uiMac.Text= $"{dr.MacAddress}";
        }

        #region CHART_OSCILLOSCOPE_PASSTHROUGH
        public void SetDataProperties(IList<PropertyInfo> dataProperties, PropertyInfo timeProperty, IList<string> names)
        {
            uiChart.SetDataProperties(dataProperties, timeProperty, names);
        }

        public void SetTitle(string title)
        {
            uiChart.SetTitle(title);
        }

        public void SetUISpec(UISpecifications uiSpec)
        {
            uiChart.SetUISpec(uiSpec);
        }

        public double GetPan()
        {
            return uiChart.GetPan();
        }

        public double GetMaxPan()
        {
            return uiChart.GetMaxPan();
        }

        public void SetPan(double value)
        {
            uiChart.SetPan(value);
        }

        public void SetZoom(double value)
        {
            uiChart.SetZoom(value);
        }

        public double GetZoom() 
        {  
            return uiChart.GetZoom(); 
        }  

        public int GetCurrMaxNLines()
        {
            return uiChart.GetCurrMaxNLines();
        }

        public void RedrawOscilloscopeYTime(int line, DataCollection<OscDataRecord> list, List<int> triggerIndex)
        {
            // The BT code sets up the MMData 
            MMData.ClearAllRecords();
            MMData.MaxLength = list.Count;
            MMData.CurrTimeStampType = DataCollection<OscDataRecord>.TimeStampType.HMS;
            MMData.TimeStampStart = list.TimeStampStart;
            foreach (var data in list)
            {
                MMData.AddRecord(data);
            }
            TriggerIndexes = triggerIndex; // alt: TriggerSetting.FindTriggeredIndex(MMData);

            uiChart.RedrawOscilloscopeYTime(line, list, triggerIndex);
            UpdateReticuleScale();

            SetNWaveRadio(uiChart.GetCurrMaxNLines());
            // here!here
        }

        public int GetNextOscilloscopeLine()
        {
            return uiChart.GetNextOscilloscopeLine();
        }

        public void ClearLine(int lineIndex)
        {
            uiChart.ClearLine(lineIndex);
        }

        #endregion // the CHARTCONTROL_OSCILLOSCOPE_PASSTHROUGH

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


#if !NO_BT

        private async Task<Status_DeviceRecord> DoReadStatus_Device()
        {
            //SetStatusActive(true); // the false happens in the bluetooth status handler.
            var record = new Status_DeviceRecord();
            try
            {
                var valueList = await bleDevice.ReadStatus_Device();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Device");
                    return record;
                }


                var FirmwareMajor = valueList.GetValue("FirmwareMajor");
                if (FirmwareMajor.CurrentType == BCBasic.BCValue.ValueType.IsDouble || FirmwareMajor.CurrentType == BCBasic.BCValue.ValueType.IsString || FirmwareMajor.IsArray)
                {
                    record.FirmwareMajor = (double)FirmwareMajor.AsDouble;
                }

                var FirmwareMinor = valueList.GetValue("FirmwareMinor");
                if (FirmwareMinor.CurrentType == BCBasic.BCValue.ValueType.IsDouble || FirmwareMinor.CurrentType == BCBasic.BCValue.ValueType.IsString || FirmwareMinor.IsArray)
                {
                    record.FirmwareMinor = (double)FirmwareMinor.AsDouble;
                }

                var MaxInputVoltage = valueList.GetValue("MaxInputVoltage");
                if (MaxInputVoltage.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxInputVoltage.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxInputVoltage.IsArray)
                {
                    record.MaxInputVoltage = (double)MaxInputVoltage.AsDouble;
                }

                var MaxInputCurrent = valueList.GetValue("MaxInputCurrent");
                if (MaxInputCurrent.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxInputCurrent.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxInputCurrent.IsArray)
                {
                    record.MaxInputCurrent = (double)MaxInputCurrent.AsDouble;
                }

                var MaxInputResistance = valueList.GetValue("MaxInputResistance");
                if (MaxInputResistance.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxInputResistance.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxInputResistance.IsArray)
                {
                    record.MaxInputResistance = (double)MaxInputResistance.AsDouble;
                }

                var MaxSamplingRate = valueList.GetValue("MaxSamplingRate");
                if (MaxSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MaxSamplingRate.CurrentType == BCBasic.BCValue.ValueType.IsString || MaxSamplingRate.IsArray)
                {
                    record.MaxSamplingRate = (double)MaxSamplingRate.AsDouble;
                }

                var DeviceBufferSize = valueList.GetValue("DeviceBufferSize");
                if (DeviceBufferSize.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DeviceBufferSize.CurrentType == BCBasic.BCValue.ValueType.IsString || DeviceBufferSize.IsArray)
                {
                    record.DeviceBufferSize = (double)DeviceBufferSize.AsDouble;
                }

                var Reserved01 = valueList.GetValue("Reserved01");
                if (Reserved01.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Reserved01.CurrentType == BCBasic.BCValue.ValueType.IsString || Reserved01.IsArray)
                {
                    record.Reserved01 = (double)Reserved01.AsDouble;
                }

                var MacAddress = valueList.GetValue("MacAddress");
                if (MacAddress.IsArray)
                {
                    var data = MacAddress.AsArray;
                    var mac = "";
                    foreach (var b in data.AsByteArray())
                    {
                        if (mac != "") mac += ":";
                        mac += b.ToString("X2");
                    }
                    record.MacAddress = mac;    
                }    
                //if (MacAddress.CurrentType == BCBasic.BCValue.ValueType.IsDouble || MacAddress.CurrentType == BCBasic.BCValue.ValueType.IsString || MacAddress.IsArray)
                ///{
                //    record.MacAddress = (string)MacAddress.AsString;
                //}
                //Status_Device_FirmwareMajor.Text = record.FirmwareMajor.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_FirmwareMinor.Text = record.FirmwareMinor.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_MaxInputVoltage.Text = record.MaxInputVoltage.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_MaxInputCurrent.Text = record.MaxInputCurrent.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_MaxInputResistance.Text = record.MaxInputResistance.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_MaxSamplingRate.Text = record.MaxSamplingRate.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_DeviceBufferSize.Text = record.DeviceBufferSize.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_Reserved01.Text = record.Reserved01.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_MacAddress.Text = record.MacAddress.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                // Status_DeviceRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }

            return record;
        }
#endif

#if !NO_BT

        private async Task<Status_StatusRecord> DoReadStatus_Status()
        {
            //SetStatusActive(true); // the false happens in the bluetooth status handler.
            //ncommand++;
            var record = new Status_StatusRecord();
            try
            {
                var valueList = await bleDevice.ReadStatus_Status();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Status");
                    return null;
                }


                var DeviceStatus = valueList.GetValue("DeviceStatus");
                if (DeviceStatus.CurrentType == BCBasic.BCValue.ValueType.IsDouble || DeviceStatus.CurrentType == BCBasic.BCValue.ValueType.IsString || DeviceStatus.IsArray)
                {
                    record.DeviceStatus = (double)DeviceStatus.AsDouble;
                }

                var BatteryLevel = valueList.GetValue("BatteryLevel");
                if (BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsDouble || BatteryLevel.CurrentType == BCBasic.BCValue.ValueType.IsString || BatteryLevel.IsArray)
                {
                    record.BatteryLevel = (double)BatteryLevel.AsDouble;
                }

                //Status_Status_DeviceStatus.Text = record.DeviceStatus.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Status_BatteryLevel.Text = record.BatteryLevel.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?

                //Status_StatusRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
            return record;
        }
#endif
#if !NO_BT

        private async Task<Status_Device_NameRecord> DoReadStatus_Device_Name()
        {
            //SetStatusActive(true); // the false happens in the bluetooth status handler.
            //ncommand++;
            var record = new Status_Device_NameRecord();
            try
            {
                var valueList = await bleDevice.ReadStatus_Device_Name();
                if (valueList == null)
                {
                    SetStatus($"Error: unable to read Status_Device_Name");
                    return null;
                }


                var Device_Name = valueList.GetValue("Device_Name");
                if (Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsDouble || Device_Name.CurrentType == BCBasic.BCValue.ValueType.IsString || Device_Name.IsArray)
                {
                    record.Device_Name = (string)Device_Name.AsString;
                }


                //Status_Device_Name_Device_Name.Text = record.Device_Name.ToString(); // "N0"); // either N or F3 based on DEC HEX FIXED. hex needs conversion to int first?
                //Status_Device_NameRecordData.Add(record);

            }
            catch (Exception ex)
            {
                SetStatus($"Error: exception: {ex.Message}");
            }
            return record;
        }
#endif
        #endregion


        bool InManipulation = false;
        private void OnPointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Overrides the ChartControl OnPointerMoved
            if (InManipulation) return;
            e.Handled = true;
            var position = e.GetCurrentPoint(uiChartRaw).Position;
            var y = position.Y / uiChartRaw.ActualHeight;
            if (y < CURSOR_DIVIDING_LINE)
            {
                uiChartRaw.DoPointerMove(position);
            }
        }

        private void OnPointerExit(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (InManipulation) return;
            e.Handled = true;
            uiChartRaw.PointerSetCursorVisible(false);
        }

        private void OnPointerPress(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }

        private void OnManipulationStarted(object sender, Windows.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            startY = e.Position.Y / uiChartRaw.ActualHeight;
        }

        private void OnManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            InManipulation = false;

        }
        static double CURSOR_DIVIDING_LINE = 0.5; // when y > CURSOR_DIVIDING_LINE, we're doing a pan (bottom half); otherwise a cursor move.

        static double MIN_ZOOM = 0.9; // normally 1.0, no zoom, but maybe people want to jiggle it a little
        static double MAX_ZOOM = 25.0; // Set by looking at a typical curve.

        double startPan = 0.0;
        double startY = 0.0;
        double startZoom = 1.0;
        private void OnManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Cumulative.Scale != 1.0)
            {
                if (!InManipulation)
                {
                    InManipulation = true;
                    uiChartRaw.PointerSetCursorVisible(false); // hide the cursor while zooming
                    startZoom = uiChart.GetZoom();
                }
                double newzoom = startZoom * e.Cumulative.Scale;
                if (newzoom < MIN_ZOOM) newzoom = MIN_ZOOM;
                else if (newzoom > MAX_ZOOM) newzoom = MAX_ZOOM;

                uiChart.SetZoom(newzoom);
                UpdateReticuleScale();
            }
            else if (startY > CURSOR_DIVIDING_LINE && e.Cumulative.Translation.X != 0)
            {
                // Start to pan
                if (!InManipulation)
                {
                    InManipulation = true;
                    uiChartRaw.PointerSetCursorVisible(false); // hide the cursor while zooming
                    startPan = 1.0 - uiChart.GetPan();
                }
                double pan = (e.Cumulative.Translation.X / uiChartRaw.ActualWidth);
                // Update based on the current zoom level
                pan = pan / uiChart.GetZoom();
                double newpan = 1.0 - (pan + startPan);
                double maxpan = uiChartRaw.GetMaxPan();
                if (newpan < 0.0) newpan = 0.0;
                if (newpan > maxpan) newpan = maxpan;
                uiChart.SetPan(newpan);
            }
        }


        #region PERSONALIZATION


        UserPersonalization.Item CurrPersonalization = UserPersonalization.Item.None;
        /// <summary>
        /// Radio button for selection a personalization was clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPersonalizationItem(object sender, RoutedEventArgs e)
        {
            if (!this.IsLoaded) return;

            var tag = (sender as FrameworkElement)?.Tag as string;
            if (string.IsNullOrEmpty(tag)) return;

            var oldPersonalization = CurrPersonalization;

            switch (tag)
            {
                default: CurrPersonalization = UserPersonalization.Item.None; break;
                case "BKG": CurrPersonalization = UserPersonalization.Item.ChartBackground; break;
                case "THN": CurrPersonalization = UserPersonalization.Item.ThinCursor; break;
                case "WV1": CurrPersonalization = UserPersonalization.Item.Wave1; break;
                case "WV2": CurrPersonalization = UserPersonalization.Item.Wave2; break;
                case "WV3": CurrPersonalization = UserPersonalization.Item.Wave3; break;
                case "WV4": CurrPersonalization = UserPersonalization.Item.Wave4; break;
                case "RETMAJ": CurrPersonalization = UserPersonalization.Item.ReticuleMajor; break;
                case "RETMIN": CurrPersonalization = UserPersonalization.Item.ReticuleMinor; break;
                case "LAB": CurrPersonalization = UserPersonalization.Item.TextLabel; break;
                case "LABBKG": CurrPersonalization = UserPersonalization.Item.TextLabelBackground; break;
                case "FRAMEBKG": CurrPersonalization = UserPersonalization.Item.FrameBackground; break;
                case "FRAMELABEL": CurrPersonalization = UserPersonalization.Item.FrameText; break;
            }

            if (CurrPersonalization != UserPersonalization.Item.None) // Not sure this distinction really makes any difference
            {
                var pref = UserPersonalization.Current;
                uiColorPicker.Color = pref.GetColor(CurrPersonalization);
                uiThickness.Value = pref.GetThickness(CurrPersonalization);
            }

            // Get / hide cursor
            switch (CurrPersonalization)
            {
                case UserPersonalization.Item.ThinCursor:
                case UserPersonalization.Item.TextLabel:
                case UserPersonalization.Item.TextLabelBackground:
                    uiChartRaw.PointerSetCursorVisibleOnLeft(true);
                    break;

                default:
                    uiChartRaw.PointerSetCursorVisible(false);
                    break;
            }


            // Should the thickness setting be shown?
            switch (CurrPersonalization)
            {
                case UserPersonalization.Item.ThinCursor:
                case UserPersonalization.Item.Wave1:
                case UserPersonalization.Item.Wave2:
                case UserPersonalization.Item.Wave3:
                case UserPersonalization.Item.Wave4:
                case UserPersonalization.Item.ReticuleMajor:
                case UserPersonalization.Item.ReticuleMinor:
                    uiThickness.Visibility = Visibility.Visible;
                    break;
                default:
                    uiThickness.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        /// <summary>
        /// Hides / makes visible the WV1, WV2, etc. radio buttons
        /// </summary>
        /// <param name="nwave"></param>
        private void SetNWaveRadio(int nwave)
        {
            var collapseTags = new List<string>();
            var visibleTags = new List<string>();
            for (int wv=1; wv<30; wv++) // It can't hurt to add too many
            {
                // wv is 1-based (WV1, WV2, etc) and nwave is zero based. When 2, set WV1 and WV2.
                if (wv <= nwave) visibleTags.Add($"WV{wv}");
                else collapseTags.Add($"WV{wv}");
            }
            foreach (var child in uiPersonalizationRadioPanel.Children)
            {
                var radio = child as RadioButton;
                if (radio == null) continue;
                var tag = radio.Tag as string;
                if (string.IsNullOrEmpty(tag)) continue;
                if (collapseTags.Contains(tag)) radio.Visibility = Visibility.Collapsed;
                if (visibleTags.Contains(tag)) radio.Visibility= Visibility.Visible;
                // ignore all of the tags that aren't a WV1 or WV2
            }
        }
        
        private void OnPersonalizationColorChanged(ColorPicker sender, ColorChangedEventArgs args)
        {
            var pref = UserPersonalization.Current;
            pref.SetColor(CurrPersonalization, args.NewColor);
            SetPersonalization(pref);
        }
        private void OnPersonalizationThicknessChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            var pref = UserPersonalization.Current;
            pref.SetThickness(CurrPersonalization, e.NewValue);
            SetPersonalization(pref);
        }

        public void SetPersonalization(UserPersonalization pref)
        {
            uiChartRaw.SetPersonalization(pref);

            // All the text labels.
            var fg = pref.GetBrush(UserPersonalization.Item.TextLabel);
            var bg = pref.GetBrush(UserPersonalization.Item.TextLabelBackground);

            uiReticuleScalePanel.BorderBrush = fg;
            uiReticuleScalePanel.Background = bg;
            uiReticuleScaleLabel.Foreground = fg;
            uiReticuleScale.Foreground = fg;

            uiControlBorder.Background = pref.GetBrush(UserPersonalization.Item.FrameBackground);
            var frametextbrush = pref.GetBrush(UserPersonalization.Item.FrameText);
            UserPersonalization.SetTextColor(uiControlTabViewPane, frametextbrush);
            foreach (var menu in uiControlTabView.MenuItems)
            {
                UserPersonalization.SetTextColor(menu as FrameworkElement, frametextbrush);
            }
            
        }

        private void OnOpenPersonalization(object sender, RoutedEventArgs e)
        {
            uiPersonalization.Visibility = uiPersonalization.Visibility == Visibility.Visible 
                ? Visibility.Collapsed : Visibility.Visible;
        }

        private void OnPersonalizationThemeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsLoaded) return;
            DoPersonalizationFromComboBox();
        }

        private void DoPersonalizationFromComboBox()
        {
            var pref = GetCurrentPersonalization();
            SetPersonalization(pref);
        }

        public UserPersonalization GetCurrentPersonalization()
        {
            var tag = ((uiPersonalizationThemeList.SelectedItem) as FrameworkElement)?.Tag as string;
            if (string.IsNullOrEmpty(tag)) return null;
            var pref = UserPersonalization.GetPersonalization(tag);
            if (pref == null) return null;
            return pref;
        }

        private void SetupPersonalizationList()
        {
            var prefname = "tek"; // TODO: get from user saved value.
            ComboBoxItem selectedItem = null;

            var cb = uiPersonalizationThemeList;
            var list = UserPersonalization.GetPersonalizationList;
            foreach (var pref in list)
            {
                var cbi = new ComboBoxItem()
                {
                    Content = pref.Name,
                    Tag = pref.Tag,
                };
                ToolTipService.SetToolTip(cbi, pref.Description);
                cb.Items.Add(cbi);
                if (selectedItem == null) selectedItem = cbi; // always select at least one!
                if (pref.Tag == prefname) selectedItem = cbi;
            }
            cb.SelectedItem = selectedItem;
        }

        #endregion // PERSONALIZATION

        private void OnNavigationInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var tag = (args.InvokedItemContainer as FrameworkElement)?.Tag as string;
            if (string.IsNullOrEmpty(tag)) return;  

            var children = uiControlTabViewPane.Children;
            foreach (var child in children)
            {
                var fe = child as FrameworkElement;
                if (fe == null) continue;
                var vis = (fe.Tag as string) == tag ? Visibility.Visible : Visibility.Collapsed;
                fe.Visibility = vis;
            }
        }
    }
}
