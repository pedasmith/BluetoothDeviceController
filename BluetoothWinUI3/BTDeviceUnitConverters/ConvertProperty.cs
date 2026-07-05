using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWinUI3.BTDeviceUnitConverters
{

    internal static class ConvertProperty
    {
        public static double Convert (this UserPreferences prefs, double value, string units)
        {
            double retval = value;
            switch (units)
            {
                // unit for which there is not default conversion.
                default:
                    break;
                case "C":
                case "celcius":
                    retval = BluetoothWatcher.Units.Temperature.Convert(value, BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius, prefs.Temperature);
                    break;
                case "hPA":
                    retval = BluetoothWatcher.Units.Pressure.Convert(value, BluetoothWatcher.Units.Pressure.PressureUnit.hectoPascal_milliBar, prefs.Pressure);
                    break;
            }
            return retval;
        }
    }
}
