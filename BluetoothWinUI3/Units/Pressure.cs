using System;


namespace BluetoothWatcher.Units
{
    public class Pressure
    {
        public enum PressureUnit {
            [enumUtilities.Display("mm Mercury (Torr)")]
            mmHg_Torr,
            [enumUtilities.Display("inches of Mercury")]
            inHg,
            [enumUtilities.Display("milliBar (hectoPascal)")]
            hectoPascal_milliBar,
            [enumUtilities.Display("kiloPascal")]
            kiloPascal,
            [enumUtilities.Display("Pascal")]
            Pascal,
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
            switch (from)
            {
                case PressureUnit.inHg: mmHg = 25.4 * value; break;
                case PressureUnit.hectoPascal_milliBar: mmHg = 0.750062 * value; break;
                case PressureUnit.kiloPascal: mmHg = 7.50062 * value; break;
                case PressureUnit.Pascal: mmHg = 0.00750062 * value; break;
                case PressureUnit.PSI: mmHg = 51.7149 * value; break;
                case PressureUnit.Atmosphere: mmHg = 760 * value; break;
            }

            switch (to) // always start with mmHg
            {
                case PressureUnit.mmHg_Torr: return 1 * mmHg;
                case PressureUnit.inHg: return 0.03937008 * mmHg;
                case PressureUnit.hectoPascal_milliBar: return 1.333224 * mmHg;
                case PressureUnit.kiloPascal: return 0.1333224 * mmHg;
                case PressureUnit.Pascal: return 133.3224 * mmHg;
                case PressureUnit.PSI: return 0.0193368 * mmHg;
                case PressureUnit.Atmosphere: return 0.0013157894 * mmHg;
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
            if (!DoubleApprox.Approx (actualValue, expectedValue, 0.001))
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: ({value}, {from}, {to}) expected {expectedValue} actual={actualValue}");
                nerror++;
            }
            return nerror;
        }

        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.inHg, 100);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.mmHg_Torr, 2540);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.hectoPascal_milliBar, 3386.39);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.hectoPascal_milliBar, 3386.39);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.kiloPascal, 338.639);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.PSI, 49.11);
            nerror += TestOne(100, PressureUnit.inHg, PressureUnit.Atmosphere, 3.342);

            return nerror;
        }
    }
}
