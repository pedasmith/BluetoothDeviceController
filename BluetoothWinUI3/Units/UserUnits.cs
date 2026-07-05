using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothWatcher.Units
{
    /// <summary>
    /// Generic class that can be used by any device that needs to save and convert units.
    /// Note all devices will use all units. The setting page for each device will have to pick the right one.
    /// </summary>
    public class UserUnits
    {
        public Temperature.TemperatureUnit Temperature { get; set; } = Units.Temperature.TemperatureUnit.Fahrenheit;
        public Pressure.PressureUnit Pressure { get; set; } = Units.Pressure.PressureUnit.inHg;
    }
}
