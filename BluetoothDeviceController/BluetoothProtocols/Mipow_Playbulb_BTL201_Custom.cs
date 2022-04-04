using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using enumUtilities;

namespace BluetoothProtocols
{
    public class MipowPlaybulbModesConverter : EnumValueConverter<Mipow_Playbulb_BTL201_Custom.Modes> { }
    public class Mipow_Playbulb_BTL201_Custom
    {
        public enum Modes
        {
            //  "0=blink 1=pulse 2=rainbow hard 3=rainbow smooth 4=candle ff=off"
            Blink = 0x00,
            Pulse = 0x01,
            Rainbow = 0x02,
            Rainbow_Smooth = 0x03,
            Candle = 0x04,
            No_Demo = 0xFF,
        };
    }
}
