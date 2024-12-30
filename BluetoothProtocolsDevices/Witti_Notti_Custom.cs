
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
    public class NottiModeConverter : EnumValueConverter<Witti_Notti.NottiMode> { }
    public class NottiAlarmConverter : EnumValueConverter<Witti_Notti.NottiAlarm> { }

    /// <summary>
    /// The auto-generated Witti_Dotti class has a generic "Command" characteristic. This custom file includes all of the commands that use the Command 
    /// </summary>
    public partial class Witti_Notti
    {
        public enum NottiMode
        {
            On = 0x00,
            Off = 0x08,//Docs say this is 0x01
            [enumUtilities.Display("")]
            Color_Changing = 0x02,
            // Color changing doesn't seem to work at all
            // None of the other possible values seem to have any effect.
            // Note that the device is either off 0x08 (but listening) or all the way off 0x10.
            // I don't see why anyone would actually do that, so I don't include it as an enum.
            [enumUtilities.Display("")]
            Completely_Off = 0x10,
        }
        public enum NottiAlarm
        {
            Off = 0x00,
            Once = 0x01,
            Every_Day = 0x02,
        }


        // Variant of Command that take a variable number of arguements
        public async Task Command(UInt16 Command, byte? Data1 = null, byte? Data2=null, byte? Data3=null, byte? Data4=null, byte? Data5=null, byte? Data6=null)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523:
            //Witti designs uses BIG endian for command bytes
            dw.ByteOrder = ByteOrder.BigEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteUInt16(Command);
            if (Data1.HasValue) dw.WriteByte(Data1.Value);
            if (Data2.HasValue) dw.WriteByte(Data2.Value);
            if (Data3.HasValue) dw.WriteByte(Data3.Value);
            if (Data4.HasValue) dw.WriteByte(Data4.Value);
            if (Data5.HasValue) dw.WriteByte(Data5.Value);
            if (Data6.HasValue) dw.WriteByte(Data6.Value);

            var command = dw.DetachBuffer().ToArray();
            await WriteCommandAsync(CharacteristicsEnum.Command_LED_Control_enum, "Command", command, GattWriteOption.WriteWithoutResponse);
        }


        public async Task SetColor(byte r, byte g, byte b)
        {
            await Command((UInt16)0x0601, r, g, b);
        }

        public async Task SetColorChange(byte r, byte g, byte b, byte r2, byte g2, byte b2)
        {
            await Command((UInt16)0x0908, r, g, b, r2, g2, b2);
        }

        public async Task SyncTime(byte hour, byte minute, byte second)
        {
            // DOTTI uses 0x609 for the clock time
            await Command((UInt16)0x0602, hour, minute, second);
        }

        public async Task SetAlarmTime(byte hour, byte minute)
        {
            // Doesn't take a value in seconds.
            await Command((UInt16)0x0503, hour, minute);
        }

        public async Task SetAlarmMode(NottiAlarm alarmMode, byte r, byte g, byte b, byte advanceTimeUnits)
        {
            if (advanceTimeUnits < 1) advanceTimeUnits = 1;
            else if (advanceTimeUnits > 0x10) advanceTimeUnits = 0x10;

            //FYI: here's the conversion equation: double advanceMinutes = 2.5 * advanceTimeUnits;
            await Command((UInt16)0x0815, (byte)alarmMode, r, g, b, advanceTimeUnits);
        }

        public async Task SetMode(NottiMode mode)
        {
            byte part1 = (byte)mode;
            await Command((UInt16)0x0405, part1);
        }
        // Deliberatly don't add a SetUnitName!!!


    }
}
