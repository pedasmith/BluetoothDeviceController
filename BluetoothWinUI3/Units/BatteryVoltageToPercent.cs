using BluetoothWatcher.Units;
using System;
using System.Collections.Generic;

namespace BluetoothWinUI3.Units
{
    public class BatteryVoltageToPercent
    {

        // Battery data
        // https://www.jlworld.com/wp-content/uploads/2018/09/CR2477.pdf

        public enum BatteryType { Unknown, NoBattery, CR2477 };
        class DischargeTable
        {
            public BatteryType BatteryType { get; set; } = BatteryType.CR2477; // Exmple: CR2477
            public List<(double Voltage, double Percent)> VoltageCurve { get; internal set; } = new();

            /// <summary>
            /// Given a voltage, find the corresponding percent, interpolating as needed.
            /// Is clamped to the first and last percentage values.
            /// </summary>
            /// <param name="voltage"></param>
            /// <returns></returns>
            public double Interpolate(double voltage)
            {
                var largerIndex = FindClosestGTE(voltage);
                var lastIndex = VoltageCurve.Count - 1;
                if (largerIndex < 0) return VoltageCurve[0].Percent;
                if (largerIndex >= lastIndex) return VoltageCurve[lastIndex].Percent;

                var smallerIndex = largerIndex + 1; // Guaranteed to be in range!
                var larger = VoltageCurve[largerIndex];
                var smaller = VoltageCurve[smallerIndex];

                var voltageRatio = (voltage - smaller.Voltage) / (larger.Voltage - smaller.Voltage); // is 0..1 
                var percent = (voltageRatio * (larger.Percent - smaller.Percent)) + smaller.Percent;
                return percent;
            }
            /// <summary>
            /// Given a voltage, return the index of Voltage/Percent which is just larger than the voltage
            /// (e.g.: if you ask for 2.6 volts CR2477 will return the '2.7' voltage
            /// Will return -1 if the first one is smaller
            /// Assumes the table is in order (!)
            /// </summary>
            private int FindClosestGTE(double voltage)
            {
                for (int i=0; i<VoltageCurve.Count; i++)
                {
                    var item = VoltageCurve[i];
                    if (item.Voltage < voltage) // went too far!
                    {
                        // Return previous. Is allowed to return -1
                        return i - 1;
                    }
                }
                return VoltageCurve.Count - 1;
            }
        }



        public static double GetPercent(BatteryType batteryType, double voltage, double defaultValue)
        {
            var table = GetTable(batteryType);
            if (table == null) return defaultValue;
            var percent = table.Interpolate(voltage);
            return percent;
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }
        
        private static int TestOne(BatteryType batteryType, double voltage, double defaultValue, double expectedValue)
        {
            int nerror = 0;
            var actualValue = GetPercent(batteryType, voltage, defaultValue);
            if (!DoubleApprox.Approx(actualValue, expectedValue))
            {
                nerror++;
                Log($"Error: BatteryVoltageToPercent({batteryType}, {voltage}) expected={expectedValue} actually got={actualValue}");
            }
            return nerror;
        }

        public static int Test()
        {
            int nerror = 0;
            // The 999 is the default value
            nerror += TestOne(BatteryType.CR2477, 5.0, 999, 100);
            nerror += TestOne(BatteryType.CR2477, 2.0, 999,   0);
            nerror += TestOne(BatteryType.CR2477, 3.2, 999,  95);
            nerror += TestOne(BatteryType.CR2477, 3.1, 999,  92.5);

            nerror += TestOne(BatteryType.Unknown, 5.0, 999, 999);
            nerror += TestOne(BatteryType.NoBattery, 5.0, 999, 999);
            return nerror;
        }

        private static DischargeTable? GetTable(BatteryType batteryType)
        {
            switch (batteryType)
            {
                case BatteryType.CR2477: return CR2477;
            }
            return null;
        }

        private static DischargeTable CR2477 = new()
        {
            // https://www.jlworld.com/wp-content/uploads/2018/09/CR2477.pdf
            // Using the 4.7KOhm curve since that's the easiest one to trace
            // https://energy.panasonic.com/dam/master/pdf/en/datasheet/lithium/CR2477_Datasheet_EN.pdf
            // The results are very temperature dependant!  
            BatteryType = BatteryType.CR2477,
            VoltageCurve =
            [
                (Voltage:3.4, Percent:100),
                (Voltage:3.0, Percent: 90),
                (Voltage:2.8, Percent: 75),
                (Voltage:2.7, Percent: 50),
                (Voltage:2.5, Percent: 25), // Panasonic says 20C 2.5V is about 50% not 25%
                (Voltage:2.0, Percent:  0),
            ]
        };
    }
}
