using BluetoothDeviceController.Charts;
using BluetoothDeviceController.SpecialtyPagesCustom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.UI;

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
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }


        private DataCollection<OscDataRecord> MakeSinWave()
        {
            const double LEN = 2000;
            const double NWAVE = 4;
            const double MIN = 0.0;
            const double MAX = 5.0;
            const int ReadingDeltaInTicks = 10_000; // 10_000 ticks per millisecond = 1KHz sound

            double RANGE = (MAX - MIN);
            double HRANGE = RANGE / 2.0;
            var MMData = new DataCollection<OscDataRecord>();
            MMData.MaxLength = (int)LEN;
            DateTime readingTime = DateTime.MinValue;  // and not at all ReadingStartTime;
            MMData.TimeStampStart = readingTime; // force these to always be in sync :-)

            double XPerWave = (LEN / NWAVE) / (2.0 * Math.PI);
            for (double x = 0; x<LEN; x++)
            {
                double value = MIN + HRANGE + (HRANGE * Math.Sin(x / XPerWave));

                var mm = new OscDataRecord(readingTime, value);
                var addResult = MMData.AddRecord(mm);
                readingTime = readingTime.AddTicks(ReadingDeltaInTicks);
            }

            return MMData;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
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

            uiChart.SetTitle("Test Chart Control");
            var uiOsc = uiChart as IChartControlOscilloscope;

            var data = MakeSinWave();
            int OFFSET = 80;
            var triggers = new List<int>() { OFFSET, OFFSET+500, OFFSET+1000, OFFSET+1500 };
            uiOsc.RedrawOscilloscopeYTime(0, data,  triggers); // #250 is the trigger.

        }
    }
}
