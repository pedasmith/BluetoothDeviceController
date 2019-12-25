
using enumUtilities;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothProtocols
{
    public class DottiPageConverter : EnumValueConverter<Witti_Dotti.DottiPage> { }
    public class DottiModeConverter : EnumValueConverter<Witti_Dotti.DottiMode> { }

    /// <summary>
    /// The auto-generated Witti_Dotti class has a generic "Command" characteristic. This custom file includes all of the commands that use the Command 
    /// </summary>
    public partial class Witti_Dotti
    {
        public enum DottiMode
        {
            Default_On = 0x00,
            Animation = 0x01,
            Clock = 0x02,
            [enumUtilities.Display("")] // Items with a blank Display("") are not listed in the UI
            Dice = 0x03,
            Battery_Indication = 0x04,
            Screen_Off = 0x05,
            // Exploration: 0x06 is like a battery; 0x07 is some kind of test display.
            // None of the other values are useful.

        }
        public enum DottiPage
        {
            Default_On = 0x0000,

            Favorite_1 = 0x0280,
            Favorite_2 = 0x0290,
            Favorite_3 = 0x02A0,
            Favorite_4 = 0x02B0,
            Favorite_5 = 0x02C0,
            Favorite_6 = 0x02D0,
            Favorite_7 = 0x02E0,
            Favorite_8 = 0x02F0,

            Animation_1 = 0x0100,
            Animation_2 = 0x0110,
            Animation_3 = 0x0120,
            Animation_4 = 0x0130,
            Animation_5 = 0x0140,
            Animation_6 = 0x0150,
            Animation_7 = 0x0160,
            Animation_8 = 0x0170,

            Dice_1 = 0x0200,
            Dice_2 = 0x0210,
            Dice_3 = 0x0220,
            Dice_4 = 0x0230,
            Dice_5 = 0x0240,
            Dice_6 = 0x0250,

            Notification_1 = 0x1000,
            Notification_2 = 0x2000,
            Notification_3 = 0x3000,
            Notification_4 = 0x4000,
            Notification_5 = 0x5000,
            Notification_6 = 0x6000,
            Notification_7 = 0x7000,
            Notification_8 = 0x8000,
            Notification_9 = 0x9000,

        };
        public async Task SetLED(byte x, byte y, byte r, byte g, byte b)
        {
            // LEDS are 1=top left 9=second row left 64=bottom right
            var ledIndex = (byte)((y * 8 + r) + 1);
            await WriteCommand((UInt16)0x0702, ledIndex, r, g, b);
        }
        public async Task SetLEDIndex(byte ledIndex, byte r, byte g, byte b)
        {
            await WriteCommand((UInt16)0x0702, ledIndex, r, g, b);
        }
        public async Task SetLEDRow(byte rowIndex, byte r, byte g, byte b)
        {
            await WriteCommand((UInt16)0x0704, rowIndex, r, g, b);
        }
        public async Task SetLEDColumn(byte columnIndex, byte r, byte g, byte b)
        {
            await WriteCommand((UInt16)0x0703, columnIndex, r, g, b);
        }
        public async Task SetColor(byte r, byte g, byte b)
        {
            await WriteCommand((UInt16)0x0601, r, g, b, 0);
        }

        public async Task SyncTime(byte hour, byte minute, byte second)
        {
            await WriteCommand((UInt16)0x0609, hour, minute, second, 0);
        }


        //The docs say that the animation speed is 1..6. 
        // 1..3 are "normal" speeds
        // 4 is really slow (about 10 seconds)
        public async Task ChangeAnimationSpeed(byte speed)
        {
            await WriteCommand((UInt16)0x0415, speed, 0, 0, 0);
        }

        public async Task SaveScreenToMemory(DottiPage page)
        {
            byte part1 = (byte)((int)page >> 8 & 0xFF);
            byte part2 = (byte)((int)page & 0xFF);
            await WriteCommand((UInt16)0x0607, part1, part2, 0, 0);
        }

        public async Task SaveScreenToMemory(byte part1, byte part2)
        {
            await WriteCommand((UInt16)0x0607, part1, part2, 0, 0);
        }

        public async Task LoadScreenFromMemory(DottiPage page)
        {
            byte part1 = (byte)((int)page >> 8 & 0xFF);
            byte part2 = (byte)((int)page & 0xFF);
            await WriteCommand((UInt16)0x0608, part1, part2, 0, 0);
        }
        public async Task LoadScreenFromMemory(byte part1, byte part2)
        {
            await WriteCommand((UInt16)0x0608, part1, part2, 0, 0);
        }
        public async Task SetMode(DottiMode mode)
        {
            byte part1 = (byte)mode;
            await WriteCommand((UInt16)0x0405, part1, 0, 0, 0);
        }
        // Deliberatly don't add a SetUnitName!!!


    }
}
