using System;
using static BluetoothWatcher.Units.Temperature;


namespace BluetoothWatcher.Units
{
    public class Pressure
    {
        public enum PressureUnit {
            [enumUtilities.Display("mm Mercury (Torr)")]
            mmHg_Torr,

            // Hawkin's Electrical guide:3045: "atmosphere balances a column of mercury 30 inches high"
            [enumUtilities.Display("inches of Mercury")]
            inHg,

            [enumUtilities.Display("milliBar (hectoPascal)")]
            hectoPascal_milliBar,

            [enumUtilities.Display("kiloPascal")]
            kiloPascal,

            //GE Contractors:p334: "Kilograms per square centimeter = 14.225 PSI"
            //https://www.sensorsone.com/kgcm2-kilogram-per-square-centimetre-pressure-unit/
            /// <summary>
            /// Warning: not actually tested :-)
            /// </summary>
            [enumUtilities.Display("kgcm2")]
            kgcm2,

            [enumUtilities.Display("Pascal")]
            Pascal,

            //Hawkin's Electrical guide:3045: "corresponds to a pressure of 14.7 pounds per square inch"
            [enumUtilities.Display("PSI")]
            PSI,

            [enumUtilities.Display("Atmospheres")]
            Atmosphere
        };
        public static double Convert(double value, PressureUnit from, PressureUnit to)
        {
            if (from == to) return value;
            var mmHg = value;
            // https://www.weather.gov/media/epz/wxcalc/pressureConversion.pdf
            // all numbers are copy-pasted EXCEPT the kiloPascal from-conversion which was wrong as of 2021-05-21
            // via https://www.weather.gov/epz/wxcalc_pressureconvert
            // Update: more accurate values from https://www.convert-measurement-units.com/conversion-calculator.php
            // And updated all calculations to use the same number, just either multiplied or divided.
            switch (from)
            {
                case PressureUnit.inHg: mmHg = 25.4 * value; break;
                case PressureUnit.hectoPascal_milliBar: mmHg = 0.750_061_575_845_66 * value; break;
                case PressureUnit.kgcm2: mmHg = 735.559_135_276_68 * value; break;
                case PressureUnit.kiloPascal: mmHg = 7.500_615_758_456_6 * value; break;
                case PressureUnit.Pascal: mmHg = 0.007_500_615_758_456_6 * value; break;
                case PressureUnit.PSI: mmHg = 51.714_923_004_929 * value; break;
                case PressureUnit.Atmosphere: mmHg = 760 * value; break;
            }

            switch (to) // always start with mmHg
            {
                case PressureUnit.mmHg_Torr: return 1 * mmHg;
                case PressureUnit.inHg: return 0.03937008 * mmHg;
                case PressureUnit.hectoPascal_milliBar: return mmHg / 0.750_061_575_845_66;
                case PressureUnit.kgcm2: return mmHg / 735.559_135_276_68;
                case PressureUnit.kiloPascal: return mmHg / 7.500_615_758_456_6;
                case PressureUnit.Pascal: return mmHg / 0.007_500_615_758_456_6;
                case PressureUnit.PSI: return mmHg / 51.714_923_004_929;
                case PressureUnit.Atmosphere: return mmHg / 760;
            }
            Console.WriteLine($"ERROR: Unknown conversion {from} {to}");
            return 0;
        }

        public static string ConvertToString(double value, PressureUnit from, PressureUnit to)
        {
            var convertedValue = Convert(value, from, to);
            return AsString(convertedValue, to);
        }

        public static string AsString(double value, PressureUnit units)
        {
            switch (units)
            {
                case PressureUnit.mmHg_Torr: return $"{value:F2} mm";
                case PressureUnit.inHg: return $"{value:F2} in";
                case PressureUnit.hectoPascal_milliBar: return $"{value:F2} mb";
                case PressureUnit.kgcm2: return $"{value:F2} kg/cm²";
                case PressureUnit.kiloPascal: return $"{value:F2} kPa";
                case PressureUnit.Pascal: return $"{value:F2} Pa";
                case PressureUnit.PSI: return $"{value:F2} PSI";
                case PressureUnit.Atmosphere: return $"{value:F2} atm";
            }
            return value.ToString();
        }

        private static int TestOne(double value, PressureUnit from, PressureUnit to, double expectedValue)
        {
            int nerror = 0;
            double actualValue = Convert(value, from, to);
            if (!DoubleApprox.Approx (actualValue, expectedValue, 0.0000001))
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: ({value}, {from}, {to}) expected {expectedValue} actual={actualValue}");
                nerror++;
            }
            return nerror;
        }
        private static int TestBackAndForth(double trial = 100)
        {
            int nerror = 0;
            foreach (var from in Enum.GetValues<PressureUnit>())
            {
                foreach (var to in Enum.GetValues<PressureUnit>())
                {
                    var expected = Convert(trial, from, to);
                    TestOne(trial, from, to, expected);
                }
            }
            return nerror;
        }
        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.inHg, 100);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.mmHg_Torr, 2540);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.hectoPascal_milliBar, 3386.388640341);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.kiloPascal, 338.6388640341);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.PSI, 49.115416835445);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.Atmosphere, 3.3421052631578947);
            nerror += TestBackAndForth(40);
            return nerror;
        }
    }
}
