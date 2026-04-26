// Collections to support Nordic_Thingy data
//From template: Protocol_Core_Body v2026-04-21 11:40

using System;
using System.Collections.ObjectModel; // Needed for ObservableCollection

using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Needed for CallerMemberNameAttribute
using System.Runtime.InteropServices.WindowsRuntime; // Needed for IBuffer.ToArray extension method

#if NET8_0_OR_GREATER
#nullable disable
#endif


namespace BluetoothProtocols.NS_Nordic_Thingy
{

    /// <summary>
    /// The Nordic Thingy:52™ is an easy-to-use prototyping platform, designed to help in building prototypes and demos, without the need to build hardware or even write firmware. It is built around the nRF52832 Bluetooth 5 SoC.
    /// This code was automatically generated 2026-04-26::10:35
    /// </summary>

    ///<summary>
    ///TODO:
    ///Environment_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Environment_Data group from Environment.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Environment_DataCollection 
    {
        public void Add(Nordic_Thingy.Environment_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Temperature.Add (value.Temperature);
            Pressure.Add (value.Pressure);
            Humidity.Add (value.Humidity);
            eCOS.Add (value.eCOS);
            TVOC.Add (value.TVOC);
        }
        public void ReplaceMostRecent(Nordic_Thingy.Environment_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index] = value.Clone();
            Temperature[index] = value.Temperature;
            Pressure[index] = value.Pressure;
            Humidity[index] = value.Humidity;
            eCOS[index] = value.eCOS;
            TVOC[index] = value.TVOC;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Temperature (c)
        public ObservableCollection<double> Temperature { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Pressure (hpa)
        public ObservableCollection<double> Pressure { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Humidity (%)
        public ObservableCollection<double> Humidity { get; } = new ObservableCollection<double>();
        // Data values (properties) from characteristic Air Quality eCOS TVOC
        public ObservableCollection<double> eCOS { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> TVOC { get; } = new ObservableCollection<double>();
        public ObservableCollection<Nordic_Thingy.Environment_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.Environment_Data>();
    }
    ///<summary>
    ///TODO:
    ///EnvironmentColor_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the EnvironmentColor_Data group from Environment.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class EnvironmentColor_DataCollection 
    {
        public void Add(Nordic_Thingy.EnvironmentColor_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            Red.Add (value.Red);
            Green.Add (value.Green);
            Blue.Add (value.Blue);
            Clear.Add (value.Clear);
        }
        public void ReplaceMostRecent(Nordic_Thingy.EnvironmentColor_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index] = value.Clone();
            Red[index] = value.Red;
            Green[index] = value.Green;
            Blue[index] = value.Blue;
            Clear[index] = value.Clear;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Color RGB+Clear
        public ObservableCollection<double> Red { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Green { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Blue { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> Clear { get; } = new ObservableCollection<double>();
        public ObservableCollection<Nordic_Thingy.EnvironmentColor_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.EnvironmentColor_Data>();
    }
    ///<summary>
    ///TODO:
    ///EnvironmentConfiguration_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the EnvironmentConfiguration_Data group from Environment.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class EnvironmentConfiguration_DataCollection 
    {
        public void Add(Nordic_Thingy.EnvironmentConfiguration_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            TempInterval.Add (value.TempInterval);
            PressureInterval.Add (value.PressureInterval);
            HumidityInterval.Add (value.HumidityInterval);
            ColorInterval.Add (value.ColorInterval);
            GasMode.Add (value.GasMode);
            RedCalibration.Add (value.RedCalibration);
            GreenCalibration.Add (value.GreenCalibration);
            BlueCalibration.Add (value.BlueCalibration);
        }
        public void ReplaceMostRecent(Nordic_Thingy.EnvironmentConfiguration_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index] = value.Clone();
            TempInterval[index] = value.TempInterval;
            PressureInterval[index] = value.PressureInterval;
            HumidityInterval[index] = value.HumidityInterval;
            ColorInterval[index] = value.ColorInterval;
            GasMode[index] = value.GasMode;
            RedCalibration[index] = value.RedCalibration;
            GreenCalibration[index] = value.GreenCalibration;
            BlueCalibration[index] = value.BlueCalibration;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic Environment Configuration
        public ObservableCollection<double> TempInterval { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> PressureInterval { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> HumidityInterval { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> ColorInterval { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> GasMode { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> RedCalibration { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> GreenCalibration { get; } = new ObservableCollection<double>();
        public ObservableCollection<double> BlueCalibration { get; } = new ObservableCollection<double>();
        public ObservableCollection<Nordic_Thingy.EnvironmentConfiguration_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.EnvironmentConfiguration_Data>();
    }
    ///<summary>
    ///TODO:
    ///Battery_DataCollection contains lists of data, one list per property value for all
    ///of the characteristics groupled in the Battery_Data group from Battery.
    ///The lists are used when displaying historical graphs of the data.
    ///</summary>
    public class Battery_DataCollection 
    {
        public void Add(Nordic_Thingy.Battery_Data value)
        {
            TimestampMostRecentAdd = value.TimestampMostRecent;
            Data.Add (value.Clone());
            Timestamps.Add (value.TimestampMostRecent);
            TimestampsDT.Add (value.TimestampMostRecent.DateTime);
            BatteryLevel.Add (value.BatteryLevel);
        }
        public void ReplaceMostRecent(Nordic_Thingy.Battery_Data value)
        {
            var index = Timestamps.Count - 1;
            Timestamps[index] = value.TimestampMostRecent;
            Data[index] = value.Clone();
            BatteryLevel[index] = value.BatteryLevel;
        }

        ///<summary>
        ///Timestamp of the most recent add. This can be different from the value in the Timestamps because that value
        ///is also updated every time a value is replaced. This value is used when, e.g., observations often come in more
        ///frequently than the UI updates
        ///</summary>
        public DateTimeOffset TimestampMostRecentAdd { get; internal set; } = DateTimeOffset.MinValue;
        public ObservableCollection<DateTimeOffset> Timestamps { get; } = new ObservableCollection<DateTimeOffset>();
        public ObservableCollection<DateTime> TimestampsDT { get; } = new ObservableCollection<DateTime>();
        // Data values (properties) from characteristic BatteryLevel
        public ObservableCollection<double> BatteryLevel { get; } = new ObservableCollection<double>();
        public ObservableCollection<Nordic_Thingy.Battery_Data> Data { get; } = new ObservableCollection<Nordic_Thingy.Battery_Data>();
    }

}