using System;
using System.Collections.Generic;
using System.Text;
using static BluetoothWatcher.Units.Temperature;

namespace BluetoothWatcher.Units
{
    public class Distance
    {
        public enum DistanceUnit
        {
            [enumUtilities.Display("Kilometers")]
            Kilometers,
            [enumUtilities.Display("Miles")]
            Miles,
        };

        public static double Convert(double value, DistanceUnit from, DistanceUnit to)
        {
            if (from == to) return value;
            var kilometers = value;
            switch (from)
            {
                case DistanceUnit.Miles: kilometers = 1.609_344 * value; break; // exact conversion
            }
            switch (to) // always start with kilometers
            {
                case DistanceUnit.Miles: return kilometers / 1.609_344;
                default: return kilometers;
            }
        }

        public static string ConvertToString(double value, DistanceUnit from, DistanceUnit to)
        {
            var convertedValue = Convert(value, from, to);
            return AsString(convertedValue, to);
        }

        public static string AsString(double value, DistanceUnit units)
        {
            switch (units)
            {
                case DistanceUnit.Kilometers:
                    return $"{value:F2} K";
                case DistanceUnit.Miles:
                    return $"{value:F2} miles";
            }
            return value.ToString();
        }
    }
}
