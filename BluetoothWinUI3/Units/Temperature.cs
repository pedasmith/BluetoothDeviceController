using Microsoft.UI.Xaml.Controls;
using System;

namespace BluetoothWatcher.Units
{
    public class Temperature
    {
        public enum TemperatureUnit { Celcius, Fahrenheit, Réaumur, Kelvin }; // TODO: Réaumur and Kelvin
        public static double Convert (double value, TemperatureUnit from, TemperatureUnit to)
        {
            if (from == to) return value;
            var valueC = value;
            switch (from)
            {
                case TemperatureUnit.Fahrenheit:
                    valueC = (value - 32) * 5 / 9;
                    break;
                case TemperatureUnit.Kelvin:
                    valueC = value - 273.15;
                    break;
                case TemperatureUnit.Réaumur:
                    valueC = value *1.25;
                    break;
            }

            switch (to)
            {
                case TemperatureUnit.Celcius:
                    return valueC;
                case TemperatureUnit.Fahrenheit:
                    return (valueC * 9 / 5) + 32;
                case TemperatureUnit.Kelvin:
                    return valueC + 273.15;
                case TemperatureUnit.Réaumur:
                    return value / 1.25;
            }
            Console.WriteLine($"ERROR: Unknown conversion {from} {to}");
            return 0;
        }

        public static string ConvertToString(double value, TemperatureUnit from, TemperatureUnit to)
        {
            var convertedValue = Convert(value, from, to);
            return AsString(convertedValue, to);
        }

        public static string AsString (double value, TemperatureUnit units)
        {
            switch(units)
            {
                case TemperatureUnit.Celcius:
                    return $"{value:F2} °C";
                case TemperatureUnit.Fahrenheit:
                    return $"{value:F2} °F";
            }
            return value.ToString();
        }

        private static int TestOne (double value, TemperatureUnit from, TemperatureUnit to, double expectedValue)
        {
            int nerror = 0;
            double actualValue = Convert(value, from, to);
            if (!DoubleApprox.Approx (actualValue, expectedValue))
            {
                System.Diagnostics.Debug.WriteLine ($"ERROR: ({value}, {from}, {to}) expected {expectedValue} actual={actualValue}");
                nerror++;
            }
            return nerror;
        }

        private static int TestBackAndForth(double trial = 100)
        {
            int nerror = 0;
            foreach (var from in Enum.GetValues<TemperatureUnit>())
            {
                foreach (var to in Enum.GetValues<TemperatureUnit>())
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

            nerror += TestOne(-40, TemperatureUnit.Fahrenheit, TemperatureUnit.Celcius, -40);
            nerror += TestOne(-40, TemperatureUnit.Celcius, TemperatureUnit.Fahrenheit, -40);
            nerror += TestOne(32, TemperatureUnit.Fahrenheit, TemperatureUnit.Celcius, 0);
            nerror += TestOne(212, TemperatureUnit.Fahrenheit, TemperatureUnit.Celcius, 100);
            nerror += TestOne(90, TemperatureUnit.Kelvin, TemperatureUnit.Celcius, -183.15);
            nerror += TestOne(90, TemperatureUnit.Réaumur, TemperatureUnit.Celcius, 112.5);
            nerror += TestBackAndForth(100);
            nerror += TestBackAndForth(0);
            return nerror;
        }
    }
}
