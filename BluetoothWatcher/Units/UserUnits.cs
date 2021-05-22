using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothWatcher.Units
{
    public class UserUnits
    {
        public Temperature.Unit Temperature { get; set; } = Units.Temperature.Unit.Fahrenheit;
        public Pressure.Unit Pressure { get; set; } = Units.Pressure.Unit.inHg;
    }
}
