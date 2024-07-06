using BluetoothDeviceController.BleEditor;
using BluetoothDeviceController.Charts;
using BluetoothDeviceController.SpecialtyPagesCustom;
using BluetoothProtocolsUwpXaml.ChartControl;
using Microsoft.Toolkit.Uwp.UI.Animations.Behaviors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestOscilloscopeCode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            UserPersonalization.Init();
            this.InitializeComponent();
            uiOsc.SetPersonalization(UserPersonalization.Current);

            //int nerror = 0;
            //nerror += BluetoothDeviceController.Charts.ChartControl.TestMakeNiceReticuleSpace();

            this.Loaded += MainPage_Loaded;
        }

        private static double CalcSin(double Y_MIN, double Y_MAX, double x, double offsetInRadians, double XPerWave)
        {
            double Y_RANGE = (Y_MAX - Y_MIN);
            double Y_RANGE_HALF = Y_RANGE / 2.0;

            double value = Y_MIN + Y_RANGE_HALF + (Y_RANGE_HALF * Math.Sin(offsetInRadians + (x / XPerWave)));
            return value;
        }

        Random r = new Random();
        private DataCollection<OscDataRecord> MakeSinWave(double offsetInRadians)
        {
            const double N_SAMPLES = 2000; // two seconds of data

            var MMData = new DataCollection<OscDataRecord>();
            MMData.MaxLength = (int)N_SAMPLES;
            DateTime readingTime = DateTime.MinValue;  // and not at all ReadingStartTime;
            MMData.TimeStampStart = readingTime; // force these to always be in sync :-)
            MMData.CurrTimeStampType = DataCollection<OscDataRecord>.TimeStampType.FromZeroMS;


            const double NWAVE = 4;
            const double Y_MIN = 0.0;
            const double Y_MAX = 5.0;
            const double FrequencyInHz = 1000.0;

            // How many ticks? Answer: 
            // there are 500 samples per wave. At 1KHz, there are 500_000 samples/second
            double samplesPerWave = N_SAMPLES / NWAVE;
            double readingDeltaInSeconds = 1.0 / (FrequencyInHz * samplesPerWave);
            int ReadingDeltaInTicks = (int)(readingDeltaInSeconds * 10_000.0 * 1_000);
            // 10_000 ticks per millisecond and 1000 milliseconds per second.
            //const int ReadingDeltaInTicks = 10_000; // 10_000 ticks per millisecond = 1KHz sound

            //double Y_RANGE = (Y_MAX - Y_MIN);
            //double Y_RANGE_HALF = (Y_MAX - Y_MIN) / 2.0;



            double XPerWave = (N_SAMPLES / NWAVE) / (2.0 * Math.PI);
            for (double x = 0; x<N_SAMPLES; x++)
            {
                //double value = Y_MIN + Y_RANGE_HALF + (Y_RANGE_HALF * Math.Sin(x / XPerWave));
                double mainvalue = CalcSin(Y_MIN, Y_MAX, x, offsetInRadians, XPerWave);
                double overlay1 = CalcSin(-0.1, 0.1, x, offsetInRadians, XPerWave / 10);
                double overlay2 = CalcSin(-0.1, 0.1, x, offsetInRadians, XPerWave / 20);
                double overlay3 = (r.NextDouble() * 0.1) - (0.1/2.0);
                double value = mainvalue + overlay1 + overlay2 + overlay3;

                var mm = new OscDataRecord(readingTime, value);
                var addResult = MMData.AddRecord(mm);
                readingTime = readingTime.AddTicks(ReadingDeltaInTicks);
            }

            return MMData;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var EventTimeProperty = OscDataRecord.GetTimeProperty();
            var properties = OscDataRecord.GetValuePropertyList();
            var names = OscDataRecord.GetNames();


            uiOsc.SetDataProperties(properties, EventTimeProperty, names);
            uiOsc.SetUISpec(new BluetoothDeviceController.Names.UISpecifications()
            {
                tableType = "standard",
                chartType = "standard",
                chartCommand = "AddYTime<MagnetometerCalibrationRecord>(addResult, MMRecordData)", //TODO: What should the chart command be>???
                chartDefaultMaxY = 5.0, // TODO: what's the best value here? 10_000,
                chartDefaultMinY = 0,
            });

            uiOsc.SetTitle("Test Chart Control");

            await Task.Delay(1); // wait for screen up to get sizes correct.

            for (int line=0; line<2; line++)
            {
                var offset = line * 40;
                double offsetInRadians = 0.1 * line;
                var data = MakeSinWave(offsetInRadians);
                int triggerOffset = 80 + offset;
                var triggers = new List<int>() { triggerOffset, triggerOffset + 500, triggerOffset + 1000, triggerOffset + 1500 };
                uiOsc.RedrawOscilloscopeYTime(line, data, triggers);
            }
        }
    }
}
