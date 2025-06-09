using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocolsSerial.Serial
{
    class RfcommOptions
    {
        public string Match = "*";
        public bool Matches(string devicename)
        {
            if (Match == "*") return true;

            var nameup = devicename.ToUpper();
            var matchup = Match.ToUpper();
            if (nameup.Contains(matchup)) return true;
            return false;
        }
    }
}
