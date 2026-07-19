using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
