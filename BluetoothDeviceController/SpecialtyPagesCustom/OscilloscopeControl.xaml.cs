using BluetoothDeviceController.Charts;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static BluetoothDeviceController.SpecialtyPages.PokitProMeterPage;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.SpecialtyPagesCustom
{
    public sealed partial class OscilloscopeControl : UserControl
    {
        public enum ConnectionState { Off, Configuring, Configured, GotData, GotDataStale, Deconfiguring, Failed };
        private DataCollection<MMDataRecord> MMData = new DataCollection<MMDataRecord>();

        public enum OscTriggerType {  FreeRunning=0, TriggerRisingEdge=1,TriggerFallingEdge=2, ResendData=3 };
        public enum OscDataMode {  Idle=0, VDCCouple=1, VACCouple=2, CurrentDCCouple=3, CurrentACCouple=4 }
        public enum OscVRangeVMax {  V300mV=0, V02V=1, V06V=2, V12V=3, V30=4, V60=5}
        // TODO: RangeAmpMax, too.

        private void UpdateConnectionState()
        {
            uiState.Text = BtConnectionState.ToString();
        }
        public OscilloscopeControl()
        {
            this.InitializeComponent();
            this.Loaded += OscilloscopeControl_Loaded;
        }
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

        private async void OscilloscopeControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(0); // Just to the compiler doesn't complain


            // Set up the graph
            // TODO: the MMData.MaxLength is the length of the data for the uiChart.
            MMData.RemoveAlgorithm = RemoveRecordAlgorithm.RemoveFirst;
            MMData.MaxLength = 8000;

            var EventTimeProperty = typeof(MMDataRecord).GetProperty("EventTime");
            var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(MMDataRecord).GetProperty("Value"),
                };
            var names = new List<string>()
                {
"Osc.",
                };
            uiChart.SetDataProperties(properties, EventTimeProperty, names);
            uiChart.SetTitle("Oscilloscope");
            uiChart.UISpec = new BluetoothDeviceController.Names.UISpecifications()
            {
                tableType = "standard",
                chartType = "standard",
                chartCommand = "AddYTime<MagnetometerCalibrationRecord>(addResult, MMRecordData)", //TODO: What should the chart comand be>???
                chartDefaultMaxY = 5.0, // TODO: what's the best value here? 10_000,
                chartDefaultMinY = 0,
            }
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



        int DSO_NMetadataEvents = 0;
        int Curr_DSO_NMetadataEvents = -10;
        int DSO_NReadingsLeft = 0;
        DateTime ReadingStart = DateTime.MinValue;
        Double ReadingDeltaInSeconds = 0.0001;
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
                Log("DBG: GOT MetaData");
                DsoScale = data.ValueList.GetValue("DsoDataScale").AsDouble; // Change: grab this earlier.
                DSO_NMetadataEvents++;
                System.Diagnostics.Debug.WriteLine($"DBG: DSO_MetaDataEvent: got event {DSO_NMetadataEvents} new scale={DsoScale}");

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

                    //var addResult = DSO_MetadataRecordData.AddRecord(record);

                    // Change:
                    //DsoScale = DsoDataScale.AsDouble;
                    System.Diagnostics.Debug.WriteLine($"DBG: DSO_MetaDataEvent: On UX thread: got event {DSO_NMetadataEvents} new scale={DsoScale}");
                });
            }
        }

        int nread = 0;
        /// <summary>
        /// Called when data is available from the Oscilloscope (DSO)
        /// </summary>
        /// <param name="data"></param>
        private void BleDevice_DSO_ReadingEvent(BleEditor.ValueParserResult data)
        {
            nread++;
            Log($"NRead={nread}");

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
                if (DSO_NReadingsLeft <= 0) // NOTE: huh? why not zero? old comment: wll never actually be zero. And might fail if BT fails?
                {
                    Utilities.UIThreadHelper.CallOnUIThread(() =>
                    {
                        MMData.ClearAllRecords();
                        DateTime readingTime = ReadingStart; // increment by 
                        for (int i = 0; i < RawReadings.Count; i++)
                        {
                            var mm = new MMDataRecord(readingTime, RawReadings[i] * DsoScale);
                            var addResult = MMData.AddRecord(mm);
                            if (i == 1) addResult = AddResult.AddReplace; // TODO: this is super clunky.
                            uiChart.AddYTime<MMDataRecord>(addResult, MMData);
                            readingTime = readingTime.AddSeconds(ReadingDeltaInSeconds);
                        }
                        Log(MMData[0].Value.ToString("F3") + " ... ");
                    });
                }
            }
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
                uiLog.Text = $"Request Data {NRequest}\n";
            });

            // TODO: get this all hooked up to some UX t control it. And do continuous readings!
            var triggerType = OscTriggerType.FreeRunning;
            var triggerLevel = 3.0f; // TODO: select
            var datamode = OscDataMode.VDCCouple;
            var range = OscVRangeVMax.V30;


            ushort nSamples = 500; // not too many for testing!
            UInt32 timePerSampleInMicroseconds = 10;
            UInt32 samplingWindowInMicroseconds = nSamples * timePerSampleInMicroseconds; // total time

            if (bleDevice == null) return;

            DSO_NReadingsLeft = nSamples;
            ReadingStart = DateTime.Now;
            ReadingDeltaInSeconds = ((double)samplingWindowInMicroseconds) / (1_000_000.0); // Convert micro-seconds to seconds
            RawReadings.Clear();

            await bleDevice.WriteDSO_Settings((byte)triggerType, triggerLevel, (byte)datamode, (byte)range, samplingWindowInMicroseconds, nSamples);

        }

    }
}
