//From template: Protocol_Body v2022-07-02 9:54
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using BluetoothDeviceController.Names;


namespace BluetoothProtocols
{
    /// <summary>
    /// .
    /// This class was automatically generated 2022-12-10::04:56
    /// </summary>

    public  class Triones_LedLight : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
    // Link: https://github.com/madhead/saberlight/blob/master/protocols/Triones/protocol.md
    // Link: https://github.com/hunsly/Ledblee-Triones-control
    // Link: https://github.com/Betree/magicblue/wiki/Characteristics-list


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("0000ffd5-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000ffe5-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Light Control B",
            "Unknown-fff0",
            "Unknown-ffe0",
            "Light Control",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("0000ffd6-0000-1000-8000-00805f9b34fb"), // #0 is Unknown ffd6
            Guid.Parse("0000ffd7-0000-1000-8000-00805f9b34fb"), // #1 is Unknown ffd7
            Guid.Parse("0000ffd8-0000-1000-8000-00805f9b34fb"), // #2 is Unknown ffd8
            Guid.Parse("0000ffd9-0000-1000-8000-00805f9b34fb"), // #3 is Triones Command B
            Guid.Parse("0000fff1-0000-1000-8000-00805f9b34fb"), // #0 is Unknown fff1
            Guid.Parse("0000fff2-0000-1000-8000-00805f9b34fb"), // #1 is Unknown fff2
            Guid.Parse("0000fff3-0000-1000-8000-00805f9b34fb"), // #2 is Unknown fff3
            Guid.Parse("0000fff4-0000-1000-8000-00805f9b34fb"), // #3 is Unknown fff4
            Guid.Parse("0000fff5-0000-1000-8000-00805f9b34fb"), // #4 is Unknown fff5
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #0 is Unknown ffe1
            Guid.Parse("0000ffe2-0000-1000-8000-00805f9b34fb"), // #1 is Unknown ffe2
            Guid.Parse("0000ffe3-0000-1000-8000-00805f9b34fb"), // #2 is Unknown ffe3
            Guid.Parse("0000ffe4-0000-1000-8000-00805f9b34fb"), // #3 is Status Reply
            Guid.Parse("0000ffe5-0000-1000-8000-00805f9b34fb"), // #4 is Unknown ffe5
            Guid.Parse("0000ffe6-0000-1000-8000-00805f9b34fb"), // #0 is Unknown ffe6
            Guid.Parse("0000ffe7-0000-1000-8000-00805f9b34fb"), // #1 is Unknown ffe7
            Guid.Parse("0000ffe8-0000-1000-8000-00805f9b34fb"), // #2 is Unknown ffe8
            Guid.Parse("0000ffe9-0000-1000-8000-00805f9b34fb"), // #3 is Triones Command

        };
        String[] CharacteristicNames = new string[] {
            "Unknown ffd6", // #0 is 0000ffd6-0000-1000-8000-00805f9b34fb
            "Unknown ffd7", // #1 is 0000ffd7-0000-1000-8000-00805f9b34fb
            "Unknown ffd8", // #2 is 0000ffd8-0000-1000-8000-00805f9b34fb
            "Triones Command B", // #3 is 0000ffd9-0000-1000-8000-00805f9b34fb
            "Unknown fff1", // #0 is 0000fff1-0000-1000-8000-00805f9b34fb
            "Unknown fff2", // #1 is 0000fff2-0000-1000-8000-00805f9b34fb
            "Unknown fff3", // #2 is 0000fff3-0000-1000-8000-00805f9b34fb
            "Unknown fff4", // #3 is 0000fff4-0000-1000-8000-00805f9b34fb
            "Unknown fff5", // #4 is 0000fff5-0000-1000-8000-00805f9b34fb
            "Unknown ffe1", // #0 is 0000ffe1-0000-1000-8000-00805f9b34fb
            "Unknown ffe2", // #1 is 0000ffe2-0000-1000-8000-00805f9b34fb
            "Unknown ffe3", // #2 is 0000ffe3-0000-1000-8000-00805f9b34fb
            "Status Reply", // #3 is 0000ffe4-0000-1000-8000-00805f9b34fb
            "Unknown ffe5", // #4 is 0000ffe5-0000-1000-8000-00805f9b34fb
            "Unknown ffe6", // #0 is 0000ffe6-0000-1000-8000-00805f9b34fb
            "Unknown ffe7", // #1 is 0000ffe7-0000-1000-8000-00805f9b34fb
            "Unknown ffe8", // #2 is 0000ffe8-0000-1000-8000-00805f9b34fb
            "Triones Command", // #3 is 0000ffe9-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3,  },
            new HashSet<int>(){ 4, 5, 6, 7, 8,  },
            new HashSet<int>(){ 9, 10, 11, 12, 13,  },
            new HashSet<int>(){ 14, 15, 16, 17,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            0, // Characteristic 2
            0, // Characteristic 3
            1, // Characteristic 4
            1, // Characteristic 5
            1, // Characteristic 6
            1, // Characteristic 7
            1, // Characteristic 8
            2, // Characteristic 9
            2, // Characteristic 10
            2, // Characteristic 11
            2, // Characteristic 12
            2, // Characteristic 13
            3, // Characteristic 14
            3, // Characteristic 15
            3, // Characteristic 16
            3, // Characteristic 17
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Unknown_ffd6_Light_Control_B_enum = 0,
            Unknown_ffd7_Light_Control_B_enum = 1,
            Unknown_ffd8_Light_Control_B_enum = 2,
            Triones_Command_B_Light_Control_B_enum = 3,
            Unknown_fff1_Unknown_fff0_enum = 4,
            Unknown_fff2_Unknown_fff0_enum = 5,
            Unknown_fff3_Unknown_fff0_enum = 6,
            Unknown_fff4_Unknown_fff0_enum = 7,
            Unknown_fff5_Unknown_fff0_enum = 8,
            Unknown_ffe1_Unknown_ffe0_enum = 9,
            Unknown_ffe2_Unknown_ffe0_enum = 10,
            Unknown_ffe3_Unknown_ffe0_enum = 11,
            Status_Reply_Unknown_ffe0_enum = 12,
            Unknown_ffe5_Unknown_ffe0_enum = 13,
            Unknown_ffe6_Light_Control_enum = 14,
            Unknown_ffe7_Light_Control_enum = 15,
            Unknown_ffe8_Light_Control_enum = 16,
            Triones_Command_Light_Control_enum = 17,

        };

        public async Task<GattCharacteristicsResult> EnsureCharacteristicOne(GattDeviceService service, CharacteristicsEnum characteristicIndex)
        {
            var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[(int)characteristicIndex]);
            Characteristics[(int)characteristicIndex] = null;
            if (characteristicsStatus.Status != GattCommunicationStatus.Success)
            {
                Status.ReportStatus($"unable to get characteristic for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
                return null;
            }
            if (characteristicsStatus.Characteristics.Count == 0)
            {
                Status.ReportStatus($"unable to get any characteristics for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
            }
            else if (characteristicsStatus.Characteristics.Count != 1)
            {
                Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[(int)characteristicIndex]}", characteristicsStatus);
            }
            else
            {
                Characteristics[(int)characteristicIndex] = characteristicsStatus.Characteristics[0];
            }
            return characteristicsStatus;
        }

        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(CharacteristicsEnum characteristicIndex = CharacteristicsEnum.All_enum, bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;
            if (ble == null) return false; // might not be initialized yet

            if (characteristicIndex != CharacteristicsEnum.All_enum)
            {
                var serviceIndex = MapCharacteristicToService[(int)characteristicIndex];
                var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                if (serviceStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                    return false;
                }
                if (serviceStatus.Services.Count != 1)
                {
                    Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                    return false;
                }
                var service = serviceStatus.Services[0];
                var characteristicsStatus = await EnsureCharacteristicOne(service, characteristicIndex);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    return false;
                }
                return true;
            }

            GattCharacteristicsResult lastResult = null;
            if (forceReread) 
            {
                readCharacteristics = false;
            }
            if (readCharacteristics == false)
            {
                for (int serviceIndex = 0; serviceIndex < MapServiceToCharacteristic.Count; serviceIndex++)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                        continue; //return false;
                    }
                    var service = serviceStatus.Services[0];
                    var characteristicIndexSet = MapServiceToCharacteristic[serviceIndex];
                    foreach (var index in characteristicIndexSet)
                    {
                        var characteristicsStatus = await EnsureCharacteristicOne(service, (CharacteristicsEnum)index);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            return false;
                        }
                        lastResult = characteristicsStatus;
                    }
                }
                // Do not call ReportStatus on OK -- the actual read/write/etc. call will
                // call ReportStatus for them. It's important that for any one actual call
                // (public method) that there's only one ReportStatus.
                //Status.ReportStatus("OK: Connected to device", lastResult);
                readCharacteristics = true;
            }
            return readCharacteristics;
        }


        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="method" ></param>
        /// <param name="command" ></param>
        /// <returns></returns>
        private async Task WriteCommandAsync(CharacteristicsEnum characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[(int)characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
            }
            catch (Exception)
            {
                result = GattCommunicationStatus.Unreachable;
            }
            Status.ReportStatus(method, result);
            if (result != GattCommunicationStatus.Success)
            {
                // NOTE: should add a way to reset
            }
        }
        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(CharacteristicsEnum characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[(int)characteristicIndex].ReadValueAsync(cacheMode);
                if (readResult.Status == GattCommunicationStatus.Success)
                {
                    buffer = readResult.Value;
                }
                else
                {
                    // NOTE: reset the characteristics array?
                }
                Status.ReportStatus(method, readResult.Status);
            }
            catch (Exception)
            {
                Status.ReportStatus(method, GattCommunicationStatus.Unreachable);
                // NOTE: reset the characteristics array?
            }
            return buffer;
        }

        /// <summary>
        /// Delegate for all Notify events
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);





















        /// <summary>
        /// Writes data for Triones_Command_B
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTriones_Command_B(byte[] param0)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Triones_Command_B_Light_Control_B_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  param0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Triones_Command_B_Light_Control_B_enum, "Triones_Command_B", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Triones_Command_B_Light_Control_B_enum, "Triones_Command_B", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }






































































        /// <summary>
        /// Writes data for Triones_Command
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTriones_Command(byte[] param0)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Triones_Command_Light_Control_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  param0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Triones_Command_Light_Control_enum, "Triones_Command", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Triones_Command_Light_Control_enum, "Triones_Command", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }



    }
}