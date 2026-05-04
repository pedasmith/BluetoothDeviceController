using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothWinUI3
{
    public static class UtilitiesOxyColor
    {
        /// <summary>
        /// Converts an OxyColor into a uint color (ARGB) suitable for DeviceColors
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static uint OxyColorToUint(OxyColor color)
        {
            uint retval = ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
            return retval;
        }

        /// <summary>
        /// Converts a uint color (like in DeviceColors) in ARGB format into an OxyColor
        /// </summary>
        public static OxyColor WinUI3ColorToOxyColor(uint color)
        {
            var retval = OxyColor.FromUInt32(color);
            return retval;
        }
    }
}
