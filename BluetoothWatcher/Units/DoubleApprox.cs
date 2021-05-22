using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;

namespace BluetoothWatcher.Units
{
    class DoubleApprox
    {
        public static bool Approx(double v1, double v2, double maxRatio = 0.000001)
        {
            if (v1 == v2) return true;
            if (v1 == 0.0 || v2 == 0.0) return false;
            var ratio = v1 / v2;
            if (ratio < 0) return false; // one positive, the other negative
            if (ratio > 1) ratio = 1 / ratio;
            var errRatio = 1 - ratio; // ideally the ratio is very close to 1. ratio is alway <= 1 so errRatio is always 0 (good) to 1 (bad).
            if (errRatio > maxRatio) return false;
            return true;
        }

        public static int TestOne(double v1, double v2, bool expected)
        {
            int nerror = 0;
            var actual = Approx(v1, v2);
            if (actual != expected)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: DoubleApprox ({v1}, {v2} expected={expected} actual={actual}");
                nerror++;
            }
            return nerror;
        }
        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne(0, 0, true);
            nerror += TestOne(0, 0.00000000000001, false);
            nerror += TestOne(0, -0.00000000000001, false);
            nerror += TestOne(1, 1.1, false);
            nerror += TestOne(1, 1.000001, true);
            nerror += TestOne(1000, 1000000000, false);
            nerror += TestOne(1000, 1000.001, true);
            return nerror;
        }
    }
}
