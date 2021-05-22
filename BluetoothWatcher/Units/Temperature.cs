using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothWatcher.Units
{
    public class Temperature
    {
        public enum Unit { Fahrenheit, Celcius };
        public static double Convert (double value, Unit from, Unit to)
        {
            if (from == to) return value;
            var valueC = value;
            switch (from)
            {
                case Unit.Fahrenheit:
                    valueC = (value - 32) * 5 / 9;
                    break;
            }

            switch (to)
            {
                case Unit.Celcius:
                    return valueC;
                case Unit.Fahrenheit:
                    return (valueC * 9 / 5) + 32;
            }
            Console.WriteLine($"ERROR: Unknown conversion {from} {to}");
            return 0;
        }

        public static string ConvertToString(double value, Unit from, Unit to)
        {
            var convertedValue = Convert(value, from, to);
            return AsString(convertedValue, to);
        }

        public static string AsString (double value, Unit units)
        {
            switch(units)
            {
                case Unit.Celcius:
                    return $"{value:F2} °C";
                case Unit.Fahrenheit:
                    return $"{value:F2} °F";
            }
            return value.ToString();
        }

        private static int TestOne (double value, Unit from, Unit to, double expectedValue)
        {
            int nerror = 0;
            double actualValue = Convert(value, from, to);
            if (actualValue != expectedValue)
            {
                System.Diagnostics.Debug.WriteLine ($"ERROR: ({value}, {from}, {to}) expected {expectedValue} actual={actualValue}");
                nerror++;
            }
            return nerror;
        }

        public static int Test()
        {
            int nerror = 0;

            nerror += TestOne(-40, Unit.Fahrenheit, Unit.Celcius, -40);
            nerror += TestOne(-40, Unit.Celcius, Unit.Fahrenheit, -40);
            nerror += TestOne(32, Unit.Fahrenheit, Unit.Celcius, 0);
            nerror += TestOne(212, Unit.Fahrenheit, Unit.Celcius, 100);
            return nerror;
        }
    }
}
