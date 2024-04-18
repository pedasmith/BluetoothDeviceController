using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using static SampleServerXaml.BtUnits;

namespace SampleServerXaml
{
    public static class UnitConvert
    {
        private static bool ApproxEqual(double v1, double v2)
        {
            if (v1 == v2) return true;
            if (v1 == 0 || v2 == 0) return false;
            var delta = (v1 - v2) / v1;
            const double eps = 0.0001;
            if(delta < eps) return true;
            return false;
        }
        public static double ConvertPressure(double value, Barometer from, Barometer to)
        {
            double retval = value;
            double pascals = value;
            switch (from)
            {
                case Barometer.pascal: pascals = value; break;
                case Barometer.atm: pascals = value * 101_325; break;
                case Barometer.bar: pascals = value * 100_000; break;
                case Barometer.hpa: pascals = value * 100; break;
                case Barometer.inHg: pascals = value * 3386.3886666667; break;
                case Barometer.mb: pascals = value * 100; break; // same as hpa
                case Barometer.mmHg: pascals = value * 133.3223684; break;
                case Barometer.psi: pascals = value * 6894.75729; break;
                    // case Barometer.inchHg: pascals = 3386.3886666667; break;
            }

            switch (to)
            {
                case Barometer.pascal: retval = pascals; break;
                case Barometer.atm: retval = pascals / 101_325; break;
                case Barometer.bar: retval = pascals / 100_0000; break;
                case Barometer.hpa: retval = pascals / 100; break;
                case Barometer.inHg: retval = pascals / 3386.3886666667; break;
                case Barometer.mb: retval = pascals / 100; break; // same as hpa
                case Barometer.mmHg: retval = pascals / 133.3223684; break;
                case Barometer.psi: retval = pascals / 6894.75729; break;
            }

            return retval;
        }

        private static int TestPressureOne(double value, Barometer from, Barometer to, double expected)
        {
            int nerror = 0;
            var actual = ConvertPressure(value, from, to);
            if (!ApproxEqual(actual, expected))
            {
                nerror++;
                Log($"ERROR: Pressure ({value}, {from}, {to}) expected={expected} actual={actual}");
            }
            return nerror;
        }
        public static int TestPressure()
        {
            int nerror = 0;
            nerror += TestPressureOne(1, Barometer.atm, Barometer.inHg, 29.92);
            nerror += TestPressureOne(1, Barometer.atm, Barometer.mmHg, 760);
            nerror += TestPressureOne(1, Barometer.atm, Barometer.mb, 1013.25);
            nerror += TestPressureOne(1, Barometer.atm, Barometer.hpa, 1013.25);
            nerror += TestPressureOne(1, Barometer.atm, Barometer.psi, 14.7);
            return nerror;
        }
        private static void Log(string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }

        public static double ConvertTemperature(double value, Temperature from, Temperature to)
        {
            double retval = value;
            double c = value;
            switch (from)
            {
                case Temperature.celsius: c = value; break;
                case Temperature.fahrenheit: c = (value - 32.0) * 5.0 / 9.0; break;
            }

            switch (to)
            {
                case Temperature.celsius: retval = c; break;
                case Temperature.fahrenheit: retval = (c * 9.0 / 5.0) + 32.0; break;
            }

            return retval;
        }
    }
}
