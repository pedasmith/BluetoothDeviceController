using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothWatcher.Units
{
    public class Pressure
    {
        public enum Unit {
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
        public static double Convert(double value, Unit from, Unit to)
        {
            if (from == to) return value;
            var mmHg = value;
            // https://www.weather.gov/media/epz/wxcalc/pressureConversion.pdf
            // all numbers are copy-pasted EXCEPT the kiloPascal from-conversion which was wrong as of 2021-05-21
            // via https://www.weather.gov/epz/wxcalc_pressureconvert
            switch (from)
            {
                case Unit.inHg: mmHg = 25.4 * value; break;
                case Unit.hectoPascal_milliBar: mmHg = 0.750062 * value; break;
                case Unit.kiloPascal: mmHg = 7.50062 * value; break;
                case Unit.Pascal: mmHg = 0.00750062 * value; break;
                case Unit.PSI: mmHg = 51.7149 * value; break;
                case Unit.Atmosphere: mmHg = 760 * value; break;
            }

            switch (to) // always start with mmHg
            {
                case Unit.mmHg_Torr: return 1 * mmHg;
                case Unit.inHg: return 0.03937008 * mmHg;
                case Unit.hectoPascal_milliBar: return 1.333224 * mmHg;
                case Unit.kiloPascal: return 0.1333224 * mmHg;
                case Unit.Pascal: return 133.3224 * mmHg;
                case Unit.PSI: return 0.0193368 * mmHg;
                case Unit.Atmosphere: return 0.0013157894 * mmHg;
            }
            Console.WriteLine($"ERROR: Unknown conversion {from} {to}");
            return 0;
        }

        public static string ConvertToString(double value, Unit from, Unit to)
        {
            var convertedValue = Convert(value, from, to);
            return AsString(convertedValue, to);
        }

        public static string AsString(double value, Unit units)
        {
            switch (units)
            {
                case Unit.mmHg_Torr: return $"{value:F2} mm";
                case Unit.inHg: return $"{value:F2} in";
                case Unit.hectoPascal_milliBar: return $"{value:F2} mb";
                case Unit.kiloPascal: return $"{value:F2} kPa";
                case Unit.Pascal: return $"{value:F2} Pa";
                case Unit.PSI: return $"{value:F2} PSI";
                case Unit.Atmosphere: return $"{value:F2} atm";
            }
            return value.ToString();
        }

        private static int TestOne(double value, Unit from, Unit to, double expectedValue)
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
            nerror += TestOne(100, Unit.inHg, Unit.inHg, 100);
            nerror += TestOne(100, Unit.inHg, Unit.mmHg_Torr, 2540);
            nerror += TestOne(100, Unit.inHg, Unit.hectoPascal_milliBar, 3386.39);
            nerror += TestOne(100, Unit.inHg, Unit.hectoPascal_milliBar, 3386.39);
            nerror += TestOne(100, Unit.inHg, Unit.kiloPascal, 338.639);
            nerror += TestOne(100, Unit.inHg, Unit.PSI, 49.11);
            nerror += TestOne(100, Unit.inHg, Unit.Atmosphere, 3.342);

            return nerror;
        }
    }
}
