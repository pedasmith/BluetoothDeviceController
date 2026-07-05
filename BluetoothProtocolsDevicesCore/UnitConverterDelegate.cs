using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols
{
    public class UnitConverterDelegate
    {
        public delegate double ConvertMethod(double value, string units);

        public static double NullConvertMethod(double value, string units)
        {
            return value;
        }
    }
}
