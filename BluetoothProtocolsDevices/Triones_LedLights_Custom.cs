using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using enumUtilities;

namespace BluetoothProtocols
{
    public class TrionesModesConverter : EnumValueConverter<Triones_LedLight_Custom.Modes> { }

    /// <summary>
    /// Class that describes an LED settings. Some values are very LED specific (like the TrionesMode)
    /// </summary>
    public class LedStatus
    {
        public bool IsOn { get; set; } = false;
        public byte TrionesMode { get; set; } = 0x41; // solid color
        public byte Speed { get; set; } = 0x00;
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte W { get; set; }
    }

    /// <summary>
    /// Normally I try to use the auto-generated classes. That's too difficult with the Triones, so this
    /// radically custom class is used instead.
    /// </summary>
    public class Triones_LedLight_Custom
    {
        public enum Modes
        {
            None = 0x00,
            Seven_Color_Cross_Fade = 0x25,
            Red_Gradual_Change = 0x26,
            Green_Gradual_Change = 0x27,
            Blue_Gradual_Change = 0x28,
            Yellow_Gradual_Change = 0x29,
            Cyan_Gradual_Change = 0x2A,
            Purple_Gradual_Change = 0x2B,
            White_Gradual_Change = 0x2C,
            Red_Green_Cross_Fade = 0x2D,
            Red_Blue_Cross_Fade = 0x2E,
            Green_Blue_Cross_Fade = 0x2F,
            Seven_Color_Strobe_Flash = 0x30,
            Red_Strobe_Flash = 0x31,
            Green_Strobe_Flash = 0x32,
            Blue_Strobe_Flash = 0x33,
            Yellow_Strobe_Flash = 0x34,
            Cyan_Strobe_Flash = 0x35,
            Purple_Strobe_Flash = 0x36,
            White_Strobe_Flash = 0x37,
            Seven_Color_Jumping_Change = 0x38,
        };

        public delegate void TrionesStatusHandler(object source, LedStatus status);
        public event TrionesStatusHandler OnLedStatusUpdate;
        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        class GuidSet
        {
            public GuidSet(string name, Guid service, Guid characteristic)
            {
                Name = name;
                Service = service;
                Characteristic = characteristic;
            }
            public string Name;
            public Guid Service;
            public Guid Characteristic;
        }
        GuidSet[] Guids = new GuidSet[]
        {
            new GuidSet ("Triones", Guid.Parse("0000ffd5-0000-1000-8000-00805f9b34fb"), Guid.Parse("0000ffd9-0000-1000-8000-00805f9b34fb")),
            new GuidSet ("LEDBlue", Guid.Parse("0000ffe5-0000-1000-8000-00805f9b34fb"), Guid.Parse("0000ffe9-0000-1000-8000-00805f9b34fb")),
        };
        GattCharacteristic WriteCharacteristic = null;

        // This is the setup for 
        GuidSet[] NotificationGuids = new GuidSet[]
        {
            //new GuidSet ("Triones", Guid.ParseScanResponseServiceData("0000ffd5-0000-1000-8000-00805f9b34fb"), Guid.ParseScanResponseServiceData("0000ffd9-0000-1000-8000-00805f9b34fb")),
            new GuidSet ("LEDBlue", Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"), Guid.Parse("0000ffe4-0000-1000-8000-00805f9b34fb")),
        };
        GattCharacteristic NotificationCharacteristic = null;

        private async Task<bool> EnsureCharacteristicAsync()
        {
            if (WriteCharacteristic == null)
            {
                GattCharacteristicsResult characteristicsStatus = null;
                foreach (var guidset in Guids)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(guidset.Service);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        continue;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        continue;
                    }
                    var service = serviceStatus.Services[0];
                    characteristicsStatus = await service.GetCharacteristicsForUuidAsync(guidset.Characteristic);
                    if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                    {
                        continue;
                    }
                    if (characteristicsStatus.Characteristics.Count != 1)
                    {
                        continue;
                    }
                    WriteCharacteristic = characteristicsStatus.Characteristics[0];
                }

                foreach (var guidset in NotificationGuids)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(guidset.Service);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        continue;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        continue;
                    }
                    var service = serviceStatus.Services[0];
                    characteristicsStatus = await service.GetCharacteristicsForUuidAsync(guidset.Characteristic);
                    if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                    {
                        continue;
                    }
                    if (characteristicsStatus.Characteristics.Count != 1)
                    {
                        continue;
                    }
                    NotificationCharacteristic = characteristicsStatus.Characteristics[0];
                }

                if (WriteCharacteristic == null)
                {
                    Status.ReportStatus($"unable to get correct characteristics ({characteristicsStatus?.Characteristics.Count})", characteristicsStatus);
                    return false;
                }
                Status.ReportStatus("OK: Connected to device", characteristicsStatus);
            }
            return WriteCharacteristic != null;
        }


        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private async Task Communicate(string command, IBuffer buffer)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await WriteCharacteristic.WriteValueAsync(buffer, GattWriteOption.WriteWithoutResponse);
            }
            catch (Exception)
            {
                result = GattCommunicationStatus.Unreachable;
            }
            Status.ReportStatus(command, result);
            if (result != GattCommunicationStatus.Success)
            {
                WriteCharacteristic = null;
            }
        }

        /// <summary>
        /// Turns the light on (true) or off (false)
        /// </summary>
        /// <param name="on"></param>
        /// <returns></returns>
        public async Task Power(bool on)
        {
            if (!await EnsureCharacteristicAsync()) return;

            byte[] command = new byte[] { 0xCC, (on ? (byte)0x23 : (byte)0x24), 0x33 };
            IBuffer buffer = command.AsBuffer();
            await Communicate("device power", buffer);
        }

        /// <summary>
        /// Sets the color of the lamp with an RGB value. The lamp must be ON to see the result!
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public async Task SetColor(byte r, byte g, byte b)
        {
            if (!await EnsureCharacteristicAsync()) return;

            byte[] command = new byte[] { 0x56, r, g, b, 0x00, 0xF0, 0xAA };
            IBuffer buffer = command.AsBuffer();
            await Communicate("setcolor", buffer);
        }

        /// <summary>
        /// Sets the mode; see the enum for possibilities 1=super fast and about 5 is normal for speed.
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="speed"></param>
        /// <returns></returns>
        public async Task SetMode(byte mode, byte speed)
        {
            if (!await EnsureCharacteristicAsync()) return;

            // Old code was 4 bytes long.
            // New code adds 4 more bytes per https://github.com/Betree/magicblue/wiki/Functions
            byte[] command = new byte[] { 0xBB, mode, speed, 0x44, 0x01, 0x00, 0x00, 0x00 };
            IBuffer buffer = command.AsBuffer();
            await Communicate("device setmode", buffer);
        }

        /// <summary>
        /// Sets the bulb in White mode with a brightness; this is much brighter than just the RGB value
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public async Task SetWhite(byte w)
        {
            if (!await EnsureCharacteristicAsync()) return;

            byte[] command = new byte[] { 0x56, 0, 0, 0, w, 0x0F, 0xAA };
            IBuffer buffer = command.AsBuffer();
            await Communicate("device setwhite", buffer);
        }

        public async Task RequestStatus()
        {
            if (!await EnsureCharacteristicAsync()) return;

            byte[] command = new byte[] { 0xEF, 0x01, 0x77 };
            IBuffer buffer = command.AsBuffer();
            await Communicate("device request_status", buffer);
        }


        public async Task<GattCommunicationStatus> StartStatusNotificationsAsync()
        {
            var ok = await EnsureCharacteristicAsync();
            if (!ok) return GattCommunicationStatus.Unreachable;
            if (NotificationCharacteristic == null) return GattCommunicationStatus.Unreachable;

            // Turn on notifications. The Config one (#6) doesn't do notifications.
            GattCommunicationStatus status;
            try
            {
                status = await NotificationCharacteristic
                    .WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }
            catch (Exception e)
            {
                status = GattCommunicationStatus.ProtocolError;
                Status.ReportStatus($"Unable to set LED notification {e.Message}", status);
                return status;
            }

            if (status != GattCommunicationStatus.Success)
            {
                Status.ReportStatus($"Unable to set LED notification", status);
                return status;
            }


            NotificationCharacteristic.ValueChanged += LedStatus_ValueChanged;



            return GattCommunicationStatus.Success;
        }

        public byte StatusPower = 0;
        public byte StatusMode = 0; // mode is 0x41 for static color
        public byte StatusSpeed = 0;
        public byte StatusR = 0;
        public byte StatusG = 0;
        public byte StatusB = 0;
        public byte StatusW = 0;
        private void LedStatus_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            /*
                When I send EF 01 77 to service ffe5/char ffe9, I get 66 15 23 41 20 00 FF 00 33 00 05 99 back on service ffe0/char ffe4
                According to here,
                Byte[2] = 23=power on 24=power off
                [3]  is the mode 0x25..0x38 and 0x41 is static color mode
                [4] ??
                [5] speed
                [6..9] r, g, b, w
                [10] ???
                [11] 0x99 static
             */
            // Value is four 2-byte uint_t values
            if (args.CharacteristicValue.Length != 12) return;
            var dr = DataReader.FromBuffer(args.CharacteristicValue);
            dr.ByteOrder = ByteOrder.LittleEndian; // Doesn't actually matter since it's just one byte
            var currLedStatus = new LedStatus();

            byte opcode = dr.ReadByte(); // 0
            byte junk1 = dr.ReadByte();  // 1
            byte powerCode = dr.ReadByte();
            currLedStatus.IsOn = powerCode == 0x24 ? false : true; // 2 23=on 24=off
            currLedStatus.TrionesMode = dr.ReadByte(); // 3
            byte junk2 = dr.ReadByte();
            currLedStatus.Speed = dr.ReadByte(); // 5
            currLedStatus.R = dr.ReadByte(); // 6
            currLedStatus.G = dr.ReadByte(); // 7
            currLedStatus.B = dr.ReadByte(); // 8
            currLedStatus.W = dr.ReadByte(); // 9

            OnLedStatusUpdate?.Invoke(this, currLedStatus);
        }

    }
}
