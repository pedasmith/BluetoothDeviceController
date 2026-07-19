using BluetoothProtocols;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Utilities;
using Windows.Devices.Bluetooth.Background;
using static BluetoothProtocols.BTStandard_CyclingSpeedCadence;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    /// <summary>
    /// The SpeedCadence_Data_Facade class supports the standard 
    /// </summary>
    public class SpeedCadence_Data_Facade : BTCommonMetaData<SpeedCadence_Data_Facade> //, IExportDataSource
    {
        /// <summary>
        /// Decoded per specs at https://files.bluetooth.com/wp-content/uploads/2024/10/CSCS_v.1.0.1.pdf
        /// section 3.1.1.1
        /// </summary>
        [Flags]
        public enum FlagsDecoded
        {
            /// <summary>
            /// 1=Includes wheel data
            /// </summary>
            WheelRevolutionDataPresent = 0x01,
            /// <summary>
            /// 1=Includes crank data
            /// </summary>
            CrankRevolutionDataPresent = 0x02,
        }
        public FlagsDecoded CurrFlagsDecoded { get { return (FlagsDecoded)Flags; } }
        public bool FlagsIsWheel { get { return CurrFlagsDecoded.HasFlag(FlagsDecoded.WheelRevolutionDataPresent); } }
        public bool FlagsIsCrank { get { return CurrFlagsDecoded.HasFlag(FlagsDecoded.CrankRevolutionDataPresent); } }
        public string SensorPosition {  get { return FlagsIsWheel ? "Wheel" : "Crank"; } }

        /// <summary>
        /// Time since the last report for either the wheel or the crank
        /// </summary>
        public double TimeSensor { get { return FlagsIsWheel ? TimeWheel : TimeCrank; } }
        /// <summary>
        /// Total revolutions since the sensor was last reset (generally since it was first powered on)
        /// </summary>
        public double RevolutionSensor { get { return FlagsIsWheel ? RevolutionWheel : RevolutionCrank; } }

        double LastTimeSensor = -1.0;
        double LastRevolutionSensor = 0.0;

        private double _RpsSensor = 0;
        /// <summary>
        /// Speed of the wheel or crank. Updating will also update the Ewma
        ///</summary>
        public double RpsSensor
        {
            get { return _RpsSensor; }
            set { _RpsSensorEwma.Update(value); bool hasChange = (value != _RpsSensor); _RpsSensor = value; if (hasChange) OnPropertyChanged(); OnPropertyChanged("RpsSensorEwma"); }
        }
        EWMA _RpsSensorEwma = new EWMA();
        public double RpsSensorEwma { get { return _RpsSensorEwma.CurrentAverage; } }



        /// <summary>
        /// Starting value for the RevolutionSensor.
        /// </summary>
        public double RideStartRevolutionSensor { get; internal set; } = double.NaN;

        /// <summary>
        /// Number of revolutions for this ride
        /// </summary>
        public double RideRevolutionSensor { get; internal set; } = 0;

        /// <summary>
        /// Initialized to the current time when the first data comes in. 
        /// </summary>
        public DateTimeOffset RideStartTime { get; internal set; } = DateTimeOffset.Now;

        /// <summary>
        /// Calculated current ride time in minutes. This is just now minutes the start time.
        /// </summary>
        public double CalculatedRideTimeInMinutes {  get { return DateTimeOffset.Now.Subtract(RideStartTime).TotalMinutes; } }

        /// <summary>
        /// Set up a reasonable default for most people.
        /// A 700C tire has a "bead seat" diameter of 622mm. Add in another 25mm (twice) means the actual
        /// diameter is 672mm. 672mm × 𝜋 ≅ 2.11 meters per revolution. 
        /// </summary>
        public double DistancePerRevolutionInMeters { get; set; } = 2.11;
        /// <summary>
        /// Calculated value of the current speed based on the distance per revolution and the curent RPS.
        /// Set in the Update... call
        /// </summary>
        public double CurrSpeed { get; set; }
        /// <summary>
        /// Calculated value of the distance for this ride based on the distance per revolution and the current
        /// delta revolutions. It's not the total distance.
        /// </summary>
        public double RideDistance { get; set; }

        /// <summary>
        /// Calculated value of the total distance since the sensor was reset.based on the distance per revolution and the current
        /// delta revolutions. 
        /// </summary>
        public double TotalDistance { get; set; }


        /// <summary>
        /// Giant routine that updates the data based on either the wheel or crank data.
        /// Also updates the facade properties like the times (start and ride time in minutes),
        /// speed, and distance.
        /// This is only used when the underlying sensor has new data -- among other things,
        /// some of the calculated values like Speed will always be calculated in a standard
        /// format (KPH)
        /// </summary>
        private void SetFacadeDataFromOriginalData()
        {

            // Both the wheel and crank times are UINT16
            // The wheel revolution count is UINT32 and the crank revolution count is UINT16.
            // The wheel revolution will essentially never overflow, but the crank might (64K revolutions
            // at 1 second per revolution will overflow in about 17 hours)

            // The time values are just U16 in 1024th (2^^10) of a second. That means they
            // roll over in 2^^6 seconds = 64 seconds!
            if (LastTimeSensor <= 0) // nothing is initialized
            {
                RideStartRevolutionSensor = RevolutionSensor;
            }
            else
            {
                var deltaTimeSensorInSeconds = TimeSensor - LastTimeSensor;
                if (deltaTimeSensorInSeconds < 0) deltaTimeSensorInSeconds += 64.00; // rollover is exactly every 64 seconds

                // Note that the crank can overflow during a ride, but the wheel
                // is designed to outlive the lifetime of the sensor.
                var deltaRevolutionSensor = SubtractWithSmartRollover(RevolutionSensor, LastRevolutionSensor);
                if (deltaRevolutionSensor == 0)
                {
                    RpsSensor = 0.0; // no change in revolution, so speed is zero
                }
                else
                {
                    var rps = (deltaTimeSensorInSeconds != 0.0) ? deltaRevolutionSensor / deltaTimeSensorInSeconds : double.NaN;
                    if (double.IsNaN(rps))
                    {
                        // Error? It means we got a revolution change with no time change.
                    }
                    else
                    {
                        RpsSensor = rps; // only update with new data
                    }
                }

                // From my EE training: depending on a delta to increment the value is always
                // a little dicey. You never know when "something" will happen such that a delta
                // interval is somehow missed, or a counter goes negative.
                // For the wheel, we use the more dependable value of just subtracting the
                // initial value from the starting value. That works because the wheel counter
                // has more bits in it than the crank.
                // But for the crank, we have to rely on the delta approach.
                if (FlagsIsCrank)
                {
                    RideRevolutionSensor += deltaRevolutionSensor;
                }
                else
                {
                    RideRevolutionSensor = RevolutionSensor - RideStartRevolutionSensor;
                }
                // TODO: Update speed and distance
                // RideTotalDistance is always calculated in kilometers
                RideDistance = RideRevolutionSensor * DistancePerRevolutionInMeters / 1000.0;
                TotalDistance = RevolutionSensor * DistancePerRevolutionInMeters / 1000.0;

                // CurrSpeed is in kilometers per hour
                var distanceInKilometers = deltaRevolutionSensor * DistancePerRevolutionInMeters / 1000.0;
                var timeInHours = deltaTimeSensorInSeconds / (60 * 60);
                CurrSpeed = distanceInKilometers == 0 ? 0.0 : (distanceInKilometers / timeInHours);
            }
            LastTimeSensor = TimeSensor;
            LastRevolutionSensor = RevolutionSensor;
        }

        /// <summary>
        /// Given either two numbers that are 0...2^^16 or are both between 0..2^^32,
        /// do a subtract. If the numbers rolled over, add either 2^^16 or 2^^32 to 
        /// make a positive number.
        /// </summary>
        private static double SubtractWithSmartRollover(double currValue, double lastValue)
        {
            double retval = currValue - lastValue;
            if (retval < 0)
            {
                const double K16 = 0x10000; // 2^^16
                const double K32 = 0x10000_0000; // 2^^32
                bool isbig = (currValue >= K16 || lastValue >= K16);
                double add = isbig ? K32 : K16;
                retval += add;
            }
            return retval;
        }

        #region Tests

        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne(0, 0, 5, 10, 2.0); // 5 seconds 10 rotations = 2 RPS
            return nerror;
        }

        private static int TestOne(double t0, double r0, double t1, double r1, double expectedRps)
        {
            int nerror = 0;
            var scf = MakeTestStock();
            scf.TestSet(t0, r0);
            scf.TestSet(t1, r1);
            var actualRps = scf.RpsSensor;
            if (actualRps != expectedRps)
            {
                nerror++;
                Log($"Cycling: {t0},{r0} -> {t1},{r1} expected {expectedRps} actual {actualRps}");
            }
            return nerror;
        }

        private void TestSet(double time, double revolution)
        {
            TimeWheel = time;
            TimeCrank = time;
            RevolutionWheel = revolution;
            RevolutionCrank = revolution;
            SetFacadeDataFromOriginalData(); // Inside TestSet()
        }
        private static SpeedCadence_Data_Facade MakeTestStock()
        {
            var retval = new SpeedCadence_Data_Facade();
            retval.Flags = 0x01; // Wheel
            return retval;
        }

        private static void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }

        #endregion



        #region From_SpeedCadence_Data
        //
        // copy-pasted from SpeedCadence_Data and then updated Clone, CopyFrom, CopyToWithConvertAndCreate
        // and radically change CopyToWithConvertAndCreate
        // and changed all fields to private
        // and updated all calls to OnPropertyChanged to include new fields


        /// <summary>
        /// Data from all of the characteristics in the Cycling Speed and Cadence Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        private double _Flags = 0;
        /// <summary>
        /// Flags (U8 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
        ///</summary>
        public double Flags
        {
            get { return _Flags; }
            set { if (value == _Flags) return; _Flags = value; OnPropertyChanged(); OnPropertyChanged("SensorPosition"); }
        }
        private double _RevolutionWheel = 0;
        /// <summary>
        /// RevolutionWheel (U32 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
        ///</summary>
        private double RevolutionWheel
        {
            get { return _RevolutionWheel; }
            set { if (value == _RevolutionWheel) return; _RevolutionWheel = value; OnPropertyChanged(); }
        }
        private double _TimeWheel = 0;
        /// <summary>
        /// TimeWheel (U16 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
        ///</summary>
        private double TimeWheel
        {
            get { return _TimeWheel; }
            set { if (value == _TimeWheel) return; _TimeWheel = value; OnPropertyChanged(); }
        }
        private double _RevolutionCrank = 0;
        /// <summary>
        /// RevolutionCrank (U16 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
        ///</summary>
        private double RevolutionCrank
        {
            get { return _RevolutionCrank; }
            set { if (value == _RevolutionCrank) return; _RevolutionCrank = value; OnPropertyChanged(); }
        }
        private double _TimeCrank = 0;
        /// <summary>
        /// TimeCrank (U16 ) from Service=Cycling Speed and Cadence and Characteristic=CSC Measurement
        ///</summary>
        private double TimeCrank
        {
            get { return _TimeCrank; }
            set { if (value == _TimeCrank) return; _TimeCrank = value; OnPropertyChanged(); }
        }
        public override SpeedCadence_Data_Facade Clone(string name = null)
        {
            var retval = this.MemberwiseClone() as SpeedCadence_Data_Facade;
            if (name != null)
            {
                retval.Name = name;
            }
            return retval;
        }

        public override void CopyFrom(SpeedCadence_Data_Facade source)
        {
            var dest = this;

            // All of the facade-specific values.
            dest._RpsSensor = source._RpsSensor;
            dest._RpsSensorEwma = source._RpsSensorEwma.Clone();
            dest.RideStartRevolutionSensor = source.RideStartRevolutionSensor;
            dest.RideRevolutionSensor = source.RideRevolutionSensor;
            dest.RideStartTime = source.RideStartTime;
            dest.DistancePerRevolutionInMeters = source.DistancePerRevolutionInMeters;
            dest.CurrSpeed = source.CurrSpeed;
            dest.RideDistance = source.RideDistance;


            // From the original class
            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = source.Name;
            dest.Flags = source.Flags;
            dest.RevolutionWheel = source.RevolutionWheel;
            dest.TimeWheel = source.TimeWheel;
            dest.RevolutionCrank = source.RevolutionCrank;
            dest.TimeCrank = source.TimeCrank;
        }

        // Like CopyFrom, but convert the doubles as appropriate + sets name
        public static SpeedCadence_Data_Facade CopyToWithConvertAndCreate(SpeedCadence_Data source, SpeedCadence_Data_Facade dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
        {
            if (dest == null)
            {
                dest = new();
            }

            // From the original class
            dest.TimestampMostRecent = source.TimestampMostRecent;
            dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
            dest.Flags = convert(source.Flags, "");
            dest.RevolutionWheel = convert(source.RevolutionWheel, "");
            dest.TimeWheel = convert(source.TimeWheel, "");
            dest.RevolutionCrank = convert(source.RevolutionCrank, "");
            dest.TimeCrank = convert(source.TimeCrank, "");

            // Facade-specific values are set up in the UpdateData() method.
            dest.SetFacadeDataFromOriginalData(); // will update the RPS, Speed, Distance, etc.

            return dest;
        }

        public override string[] ExportGetHeaders(IExportData _)
        {
            return ["SensorPosition", "Revolutions", "RevolutionsPerSecond"];
        }

        public override void ExportRow(IExportData exporter)
        {
            // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
            // RowEnd and add in the timestamps
            exporter.CellSet(SensorPosition);
            exporter.CellSet(RevolutionSensor);
            exporter.CellSet(RpsSensor);
        }

        public override string ToString()
        {
            return String.Format($"{TimestampMostRecentDT.ToString("HH:mm:ss")} {Flags} {RevolutionWheel} {TimeWheel} {RevolutionCrank} {TimeCrank}");
        }
        #endregion
    }
}
