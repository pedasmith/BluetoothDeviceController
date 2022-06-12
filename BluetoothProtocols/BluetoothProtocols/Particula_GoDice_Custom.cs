using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothProtocols
{
    public partial class Particula_GoDice
    {
        /// <summary>
        /// Official commands. See the JSON file for some hints on additional commands.
        /// </summary>
        public enum Commands { BatteryLevel = 0x03, GetColor = 0x17, SetColor = 0x08 };
        /// <summary>
        /// Dice 'dot' colors (the ones that are painted on).
        /// </summary>
        public enum DiceColors { Black = 0, Red = 1, Green = 2, Blue = 3, Yellow = 4, Orange = 5 };
        /// <summary>
        /// Helper values: Unicode dice faces
        /// </summary>
        public static List<string> DiceFaces = new List<String>() { "⚀", "⚁", "⚂", "⚃", "⚄", "⚅" };

        public event EventHandler<int> OnBatteryLevel;
        public event EventHandler<int> OnColor;
        public event EventHandler OnRollStart;
        public event EventHandler<int> OnFakeStable;
        public event EventHandler<int> OnMoveStable;
        public event EventHandler<int> OnTiltStable;
        public event EventHandler<int> OnRollStable;

        /// <summary>
        /// Given a device name like GoDice_B0940_K_v03, return the dice color (K returns Black), or ? for unknown.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static string GetDiceColorFromName(string deviceName)
        {
            var parts = deviceName.Split(new char[] { '_' });
            if (parts.Length != 4) return "?";
            switch (parts[2])
            {
                case "K": return "Black";
                case "R": return "Red";
                case "G": return "Green";
                case "B": return "Blue";
                case "Y": return "Yellow";
                case "O": return "Orange";
            }
            return "?";
        }

        /// <summary>
        /// Level is 0..100 inclusive; return is e.g., 
        /// </summary>
        public static string BatteryLevelToGlyph(int level)
        {
            int index = (level + 5) / 10;
            if (index < 0) index = 0; else if (index > 10) index = 10;
            return BatteryGlyphs[index];
        }

        public static string[] BatteryGlyphs = new string[]
        {
            "", "", "", "", "", "", "", "", "", "", "",
        };

        public Task GetBatteryLevelAsync()
        {
            return WriteTransmit(new byte[] { (byte)Commands.BatteryLevel });
        }

        public Task SetDiceColorAsync(byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        {
            byte[] command = new byte[7] { (byte)Commands.SetColor, r1, g1, b1, r2, g2, b2 };
            return WriteTransmit(command);
        }

        /// <summary>
        /// Given a message from a dice, dispatch it to the various events as needed.
        /// </summary>
        public void HandleReceiveMessageCustom(BCBasic.BCValueList message)
        {
            if (Match (message, "Bat"))
            {
                var batteryLevel = (int)message.GetValue(4, "uint8"); // BCValueList is 1-based, not zero-based
                OnBatteryLevel?.Invoke(this, batteryLevel);
            }
            else if (Match(message, "Col"))
            {
                var color = (int)message.GetValue(4, "uint8"); // BCValueList is 1-based, not zero-based
                OnColor?.Invoke(this, color);
            }
            else if (Match(message, "FS"))
            {
                var die = DieRoll(message, 3);
                OnFakeStable?.Invoke(this, die);
            }
            else if (Match(message, "MS"))
            {
                var die = DieRoll(message, 3);
                OnMoveStable?.Invoke(this, die);
            }
            else if (Match(message, "R"))
            {
                OnRollStart?.Invoke(this, null);
            }
            else if (Match(message, "S"))
            {
                var die = DieRoll(message, 2);
                OnRollStable?.Invoke(this, die);
            }
            else if (Match(message, "TS"))
            {
                var die = DieRoll(message, 3);
                OnTiltStable?.Invoke(this, die);
            }

        }
        /// <summary>
        /// Helper routine: does the message start with the given string.
        /// </summary>
        private bool Match(BCBasic.BCValueList message, string command)
        {
            int index = 0;
            var maxlen = message.GetValue("Count").AsInt;
            if (command.Length > maxlen)
            {
                return false;
            }
            foreach (var ch in command)
            {
                if ((byte)message.GetValue(index+1, "uint8") != (byte)ch)
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        /// <summary>
        /// Given the X Y Z values from a dice, determine the roll.
        /// </summary>
        private int DieRoll (BCBasic.BCValueList message,int startIndex)
        {
            int x = (int)message.GetValue(startIndex + 0, "uint8");
            int y = (int)message.GetValue(startIndex + 1, "uint8");
            int z = (int)message.GetValue(startIndex + 2, "uint8");
            if (IsClose(x, 64)) return 6;
            if (IsClose(x, 192)) return 1;
            if (IsClose(y, 64)) return 3;
            if (IsClose(y, 192)) return 4;
            if (IsClose(z, 64)) return 2;
            if (IsClose(z, 192)) return 5;
            return 0;
        }

        /// <summary>
        /// Simple math: is the value close to the target
        /// </summary>
        private bool IsClose (int value, int target, int epsilon=5)
        {
            var retval = value >= (target-epsilon) && value <= (target+epsilon);
            return retval;
        }

    }
}
