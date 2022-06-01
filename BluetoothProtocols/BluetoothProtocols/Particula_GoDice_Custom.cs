using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols
{
    public partial class Particula_GoDice
    {
        public enum Commands {  BatteryLevel = 0x03, GetColor = 0x17, SetColor = 0x08 };
        public enum DiceColors {  Black = 0, Red=1, Green=2, Blue = 3, Yellow = 4, Orange = 5 };

        public event EventHandler<int> OnBatteryLevel;
        public event EventHandler<int> OnColor;
        public event EventHandler OnRollStart;
        public event EventHandler<int> OnFakeStable;
        public event EventHandler<int> OnMoveStable;
        public event EventHandler<int> OnTiltStable;
        public event EventHandler<int> OnRollStable;

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
                OnFakeStable(this, die);
            }
            else if (Match(message, "MS"))
            {
                var die = DieRoll(message, 3);
                OnFakeStable(this, die);
            }
            else if (Match(message, "R"))
            {
                OnRollStart(this, null);
            }
            else if (Match(message, "S"))
            {
                var die = DieRoll(message, 2);
                OnRollStable(this, die);
            }
            else if (Match(message, "TS"))
            {
                var die = DieRoll(message, 3);
                OnTiltStable(this, die);
            }
            else if (Match(message, ""))
            {
                var die = DieRoll(message, 3);
                OnMoveStable(this, die);
            }
        }

        private bool Match(BCBasic.BCValueList message, string command)
        {
            int index = 0;
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

        private bool IsClose (int value, int target, int epsilon=5)
        {
            var retval = value >= (target-epsilon) && value <= (target+epsilon);
            return retval;
        }

    }
}
