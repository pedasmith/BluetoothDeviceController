using System;


namespace BluetoothWinUI3.Units
{
    internal static class BatteryLevelIcon
    {
        public static string[] MobBattery = new string[] { "", "", "", "", "", "", "", "", "", "", "" };
        public static string Icon(double percent)
        {
            string[] icons = MobBattery;
            int index = (int)Math.Round(percent / 100 * (icons.Length - 1));
            if (index < 0) index = 0;
            if (index >= icons.Length) index = icons.Length - 1;
            var retval = icons[index];
            return retval;
        }
    }
}
