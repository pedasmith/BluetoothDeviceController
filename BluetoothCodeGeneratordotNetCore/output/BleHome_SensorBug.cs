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
    /// This class was automatically generated 2022-07-24::10:01
    /// </summary>

    public  class BleHome_SensorBug : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // No links for this device

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        Guid[] ServiceGuids = new Guid[] {
           Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001803-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001802-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("00001804-0000-1000-8000-00805f9b34fb"),
           Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b10"),
           Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b20"),
           Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b30"),
           Guid.Parse("3188ac28-72d4-4006-bd96-c6c4bc6153a0"),
           Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec30"),

        };
        String[] ServiceNames = new string[] {
            "Common Configuration",
            "Device Info",
            "Battery",
            "Link Loss",
            "Immediate Alert",
            "Transmit Power",
            "Accelerometer",
            "Light",
            "Temperature",
            "Pairing",
            "BR_Utilities",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
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
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #0 is Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #1 is Appearance
            Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb"), // #2 is Privacy
            Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb"), // #3 is Reconnect Address
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #4 is Connection Parameter
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #0 is Manufacturer Name
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #1 is Model Number
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #2 is Hardware Revision
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #3 is Firmware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #4 is Software Revision
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #5 is PnP ID
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #0 is BatteryLevel
            Guid.Parse("00002a06-0000-1000-8000-00805f9b34fb"), // #0 is Link Loss Alert Level
            Guid.Parse("00002a06-0000-1000-8000-00805f9b34fb"), // #0 is Immediate Alert Level
            Guid.Parse("00002a07-0000-1000-8000-00805f9b34fb"), // #0 is Transmit Power
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b11"), // #0 is Accelerometer_Config
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b12"), // #1 is Accelerometer_Data
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b13"), // #2 is Accelerometer_Alert
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b21"), // #0 is Light Config
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b22"), // #1 is Unknown1
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b23"), // #2 is Unknown2
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b24"), // #3 is Unknown3
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b31"), // #0 is Temperature_Config
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b32"), // #1 is Temperature_Data
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b33"), // #2 is Temperature_Alert
            Guid.Parse("9dc84838-7619-4f09-a1ce-ddcf63225b34"), // #3 is Temperature_Status
            Guid.Parse("3188ac28-72d4-4006-bd96-c6c4bc6153a1"), // #0 is Pairing_Control_Status
            Guid.Parse("3188ac28-72d4-4006-bd96-c6c4bc6153a2"), // #1 is Pairing_Data
            Guid.Parse("3188ac28-72d4-4006-bd96-c6c4bc6153a3"), // #2 is Pairing_Config
            Guid.Parse("3188ac28-72d4-4006-bd96-c6c4bc6153a4"), // #3 is Pairing_AD_Key
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec32"), // #0 is Utility_DeviceName
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec33"), // #1 is Utility_Default_Conn_Param
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec34"), // #2 is Utility_Curr_Conn_Param
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec36"), // #3 is Utility_RF_Power
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec37"), // #4 is Utility_Disconnect
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec39"), // #5 is Utility_Public_Address
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec3c"), // #6 is Utility_Config_Counter
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec3e"), // #7 is Utility_Advertising_Param
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec41"), // #8 is Utility_Unknown
            Guid.Parse("4216378b-2073-47c4-83d6-a7df9a61ec42"), // #9 is Utility_Blink_LED

        };
        String[] CharacteristicNames = new string[] {
            "Device Name", // #0 is 00002a00-0000-1000-8000-00805f9b34fb
            "Appearance", // #1 is 00002a01-0000-1000-8000-00805f9b34fb
            "Privacy", // #2 is 00002a02-0000-1000-8000-00805f9b34fb
            "Reconnect Address", // #3 is 00002a03-0000-1000-8000-00805f9b34fb
            "Connection Parameter", // #4 is 00002a04-0000-1000-8000-00805f9b34fb
            "Manufacturer Name", // #0 is 00002a29-0000-1000-8000-00805f9b34fb
            "Model Number", // #1 is 00002a24-0000-1000-8000-00805f9b34fb
            "Hardware Revision", // #2 is 00002a27-0000-1000-8000-00805f9b34fb
            "Firmware Revision", // #3 is 00002a26-0000-1000-8000-00805f9b34fb
            "Software Revision", // #4 is 00002a28-0000-1000-8000-00805f9b34fb
            "PnP ID", // #5 is 00002a50-0000-1000-8000-00805f9b34fb
            "BatteryLevel", // #0 is 00002a19-0000-1000-8000-00805f9b34fb
            "Link Loss Alert Level", // #0 is 00002a06-0000-1000-8000-00805f9b34fb
            "Immediate Alert Level", // #0 is 00002a06-0000-1000-8000-00805f9b34fb
            "Transmit Power", // #0 is 00002a07-0000-1000-8000-00805f9b34fb
            "Accelerometer_Config", // #0 is 9dc84838-7619-4f09-a1ce-ddcf63225b11
            "Accelerometer_Data", // #1 is 9dc84838-7619-4f09-a1ce-ddcf63225b12
            "Accelerometer_Alert", // #2 is 9dc84838-7619-4f09-a1ce-ddcf63225b13
            "Light Config", // #0 is 9dc84838-7619-4f09-a1ce-ddcf63225b21
            "Unknown1", // #1 is 9dc84838-7619-4f09-a1ce-ddcf63225b22
            "Unknown2", // #2 is 9dc84838-7619-4f09-a1ce-ddcf63225b23
            "Unknown3", // #3 is 9dc84838-7619-4f09-a1ce-ddcf63225b24
            "Temperature_Config", // #0 is 9dc84838-7619-4f09-a1ce-ddcf63225b31
            "Temperature_Data", // #1 is 9dc84838-7619-4f09-a1ce-ddcf63225b32
            "Temperature_Alert", // #2 is 9dc84838-7619-4f09-a1ce-ddcf63225b33
            "Temperature_Status", // #3 is 9dc84838-7619-4f09-a1ce-ddcf63225b34
            "Pairing_Control_Status", // #0 is 3188ac28-72d4-4006-bd96-c6c4bc6153a1
            "Pairing_Data", // #1 is 3188ac28-72d4-4006-bd96-c6c4bc6153a2
            "Pairing_Config", // #2 is 3188ac28-72d4-4006-bd96-c6c4bc6153a3
            "Pairing_AD_Key", // #3 is 3188ac28-72d4-4006-bd96-c6c4bc6153a4
            "Utility_DeviceName", // #0 is 4216378b-2073-47c4-83d6-a7df9a61ec32
            "Utility_Default_Conn_Param", // #1 is 4216378b-2073-47c4-83d6-a7df9a61ec33
            "Utility_Curr_Conn_Param", // #2 is 4216378b-2073-47c4-83d6-a7df9a61ec34
            "Utility_RF_Power", // #3 is 4216378b-2073-47c4-83d6-a7df9a61ec36
            "Utility_Disconnect", // #4 is 4216378b-2073-47c4-83d6-a7df9a61ec37
            "Utility_Public_Address", // #5 is 4216378b-2073-47c4-83d6-a7df9a61ec39
            "Utility_Config_Counter", // #6 is 4216378b-2073-47c4-83d6-a7df9a61ec3c
            "Utility_Advertising_Param", // #7 is 4216378b-2073-47c4-83d6-a7df9a61ec3e
            "Utility_Unknown", // #8 is 4216378b-2073-47c4-83d6-a7df9a61ec41
            "Utility_Blink_LED", // #9 is 4216378b-2073-47c4-83d6-a7df9a61ec42

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
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3, 4,  },
            new HashSet<int>(){ 5, 6, 7, 8, 9, 10,  },
            new HashSet<int>(){ 11,  },
            new HashSet<int>(){ 12,  },
            new HashSet<int>(){ 13,  },
            new HashSet<int>(){ 14,  },
            new HashSet<int>(){ 15, 16, 17,  },
            new HashSet<int>(){ 18, 19, 20, 21,  },
            new HashSet<int>(){ 22, 23, 24, 25,  },
            new HashSet<int>(){ 26, 27, 28, 29,  },
            new HashSet<int>(){ 30, 31, 32, 33, 34, 35, 36, 37, 38, 39,  },

        };
        List<int> MapCharacteristicToService = new List<int>() {
            0, // Characteristic 0
            0, // Characteristic 1
            0, // Characteristic 2
            0, // Characteristic 3
            0, // Characteristic 4
            1, // Characteristic 5
            1, // Characteristic 6
            1, // Characteristic 7
            1, // Characteristic 8
            1, // Characteristic 9
            1, // Characteristic 10
            2, // Characteristic 11
            3, // Characteristic 12
            4, // Characteristic 13
            5, // Characteristic 14
            6, // Characteristic 15
            6, // Characteristic 16
            6, // Characteristic 17
            7, // Characteristic 18
            7, // Characteristic 19
            7, // Characteristic 20
            7, // Characteristic 21
            8, // Characteristic 22
            8, // Characteristic 23
            8, // Characteristic 24
            8, // Characteristic 25
            9, // Characteristic 26
            9, // Characteristic 27
            9, // Characteristic 28
            9, // Characteristic 29
            10, // Characteristic 30
            10, // Characteristic 31
            10, // Characteristic 32
            10, // Characteristic 33
            10, // Characteristic 34
            10, // Characteristic 35
            10, // Characteristic 36
            10, // Characteristic 37
            10, // Characteristic 38
            10, // Characteristic 39
            
        };
        public enum CharacteristicsEnum {
            All_enum = -1,
            Device_Name_Common_Configuration_enum = 0,
            Appearance_Common_Configuration_enum = 1,
            Privacy_Common_Configuration_enum = 2,
            Reconnect_Address_Common_Configuration_enum = 3,
            Connection_Parameter_Common_Configuration_enum = 4,
            Manufacturer_Name_Device_Info_enum = 5,
            Model_Number_Device_Info_enum = 6,
            Hardware_Revision_Device_Info_enum = 7,
            Firmware_Revision_Device_Info_enum = 8,
            Software_Revision_Device_Info_enum = 9,
            PnP_ID_Device_Info_enum = 10,
            BatteryLevel_Battery_enum = 11,
            Link_Loss_Alert_Level_Link_Loss_enum = 12,
            Immediate_Alert_Level_Immediate_Alert_enum = 13,
            Transmit_Power_Transmit_Power_enum = 14,
            Accelerometer_Config_Accelerometer_enum = 15,
            Accelerometer_Data_Accelerometer_enum = 16,
            Accelerometer_Alert_Accelerometer_enum = 17,
            Light_Config_Light_enum = 18,
            Unknown1_Light_enum = 19,
            Unknown2_Light_enum = 20,
            Unknown3_Light_enum = 21,
            Temperature_Config_Temperature_enum = 22,
            Temperature_Data_Temperature_enum = 23,
            Temperature_Alert_Temperature_enum = 24,
            Temperature_Status_Temperature_enum = 25,
            Pairing_Control_Status_Pairing_enum = 26,
            Pairing_Data_Pairing_enum = 27,
            Pairing_Config_Pairing_enum = 28,
            Pairing_AD_Key_Pairing_enum = 29,
            Utility_DeviceName_BR_Utilities_enum = 30,
            Utility_Default_Conn_Param_BR_Utilities_enum = 31,
            Utility_Curr_Conn_Param_BR_Utilities_enum = 32,
            Utility_RF_Power_BR_Utilities_enum = 33,
            Utility_Disconnect_BR_Utilities_enum = 34,
            Utility_Public_Address_BR_Utilities_enum = 35,
            Utility_Config_Counter_BR_Utilities_enum = 36,
            Utility_Advertising_Param_BR_Utilities_enum = 37,
            Utility_Unknown_BR_Utilities_enum = 38,
            Utility_Blink_LED_BR_Utilities_enum = 39,

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



        private string _Device_Name = "";
        private bool _Device_Name_set = false;
        public string Device_Name
        {
            get { return _Device_Name; }
            internal set { if (_Device_Name_set && value == _Device_Name) return; _Device_Name = value; _Device_Name_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadDevice_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Device_Name_Common_Configuration_enum, "Device_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII|Device_Name";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Device_Name = parseResult.ValueList.GetValue("Device_Name").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _Appearance = 0;
        private bool _Appearance_set = false;
        public double Appearance
        {
            get { return _Appearance; }
            internal set { if (_Appearance_set && value == _Appearance) return; _Appearance = value; _Appearance_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAppearance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Appearance_Common_Configuration_enum, "Appearance", cacheMode);
            if (result == null) return null;

            var datameaning = "U16|Speciality^Appearance|Appearance";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Appearance = parseResult.ValueList.GetValue("Appearance").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Privacy = null;
        private bool _Privacy_set = false;
        public string Privacy
        {
            get { return _Privacy; }
            internal set { if (_Privacy_set && value == _Privacy) return; _Privacy = value; _Privacy_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPrivacy(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Privacy_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Privacy_Common_Configuration_enum, "Privacy", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Privacy";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Privacy = parseResult.ValueList.GetValue("Privacy").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Privacy
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePrivacy(byte[] Privacy)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Privacy_Common_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Privacy);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Privacy_Common_Configuration_enum, "Privacy", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Privacy_Common_Configuration_enum, "Privacy", subcommand, GattWriteOption.WriteWithResponse);
            }
        }





        /// <summary>
        /// Writes data for Reconnect_Address
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteReconnect_Address(byte[] ReconnectAddress)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  ReconnectAddress);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum, "Reconnect_Address", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Reconnect_Address_Common_Configuration_enum, "Reconnect_Address", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private string _Connection_Parameter = null;
        private bool _Connection_Parameter_set = false;
        public string Connection_Parameter
        {
            get { return _Connection_Parameter; }
            internal set { if (_Connection_Parameter_set && value == _Connection_Parameter) return; _Connection_Parameter = value; _Connection_Parameter_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadConnection_Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Connection_Parameter_Common_Configuration_enum, "Connection_Parameter", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|ConnectionParameter";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Connection_Parameter = parseResult.ValueList.GetValue("ConnectionParameter").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Manufacturer_Name = "";
        private bool _Manufacturer_Name_set = false;
        public string Manufacturer_Name
        {
            get { return _Manufacturer_Name; }
            internal set { if (_Manufacturer_Name_set && value == _Manufacturer_Name) return; _Manufacturer_Name = value; _Manufacturer_Name_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadManufacturer_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Manufacturer_Name_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Manufacturer_Name_Device_Info_enum, "Manufacturer_Name", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Manufacturer_Name = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Model_Number = "";
        private bool _Model_Number_set = false;
        public string Model_Number
        {
            get { return _Model_Number; }
            internal set { if (_Model_Number_set && value == _Model_Number) return; _Model_Number = value; _Model_Number_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadModel_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Model_Number_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Model_Number_Device_Info_enum, "Model_Number", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Model_Number = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Hardware_Revision = "";
        private bool _Hardware_Revision_set = false;
        public string Hardware_Revision
        {
            get { return _Hardware_Revision; }
            internal set { if (_Hardware_Revision_set && value == _Hardware_Revision) return; _Hardware_Revision = value; _Hardware_Revision_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadHardware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Hardware_Revision_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Hardware_Revision_Device_Info_enum, "Hardware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Hardware_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Firmware_Revision = "";
        private bool _Firmware_Revision_set = false;
        public string Firmware_Revision
        {
            get { return _Firmware_Revision; }
            internal set { if (_Firmware_Revision_set && value == _Firmware_Revision) return; _Firmware_Revision = value; _Firmware_Revision_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadFirmware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Firmware_Revision_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Firmware_Revision_Device_Info_enum, "Firmware_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Firmware_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Software_Revision = "";
        private bool _Software_Revision_set = false;
        public string Software_Revision
        {
            get { return _Software_Revision; }
            internal set { if (_Software_Revision_set && value == _Software_Revision) return; _Software_Revision = value; _Software_Revision_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadSoftware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Software_Revision_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Software_Revision_Device_Info_enum, "Software_Revision", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Software_Revision = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _PnP_ID = "";
        private bool _PnP_ID_set = false;
        public string PnP_ID
        {
            get { return _PnP_ID; }
            internal set { if (_PnP_ID_set && value == _PnP_ID) return; _PnP_ID = value; _PnP_ID_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPnP_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.PnP_ID_Device_Info_enum, "PnP_ID", cacheMode);
            if (result == null) return null;

            var datameaning = "STRING|ASCII";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            PnP_ID = parseResult.ValueList.GetValue("param0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private double _BatteryLevel = 0;
        private bool _BatteryLevel_set = false;
        public double BatteryLevel
        {
            get { return _BatteryLevel; }
            internal set { if (_BatteryLevel_set && value == _BatteryLevel) return; _BatteryLevel = value; _BatteryLevel_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadBatteryLevel(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BatteryLevel_Battery_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.BatteryLevel_Battery_enum, "BatteryLevel", cacheMode);
            if (result == null) return null;

            var datameaning = "I8|DEC|BatteryLevel|%";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            BatteryLevel = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; BatteryLevelEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent BatteryLevelEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyBatteryLevel_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyBatteryLevelAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.BatteryLevel_Battery_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.BatteryLevel_Battery_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyBatteryLevel_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyBatteryLevel_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyBatteryLevelCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyBatteryLevel: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyBatteryLevel: set notification", result);

            return true;
        }

        private void NotifyBatteryLevelCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I8|DEC|BatteryLevel|%";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            BatteryLevel = parseResult.ValueList.GetValue("BatteryLevel").AsDouble;

            BatteryLevelEvent?.Invoke(parseResult);

        }

        public void NotifyBatteryLevelRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[11];
            if (ch == null) return;
            NotifyBatteryLevel_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyBatteryLevelCallback;
        }



        private double _Link_Loss_Alert_Level = 0;
        private bool _Link_Loss_Alert_Level_set = false;
        public double Link_Loss_Alert_Level
        {
            get { return _Link_Loss_Alert_Level; }
            internal set { if (_Link_Loss_Alert_Level_set && value == _Link_Loss_Alert_Level) return; _Link_Loss_Alert_Level = value; _Link_Loss_Alert_Level_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLink_Loss_Alert_Level(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Link_Loss_Alert_Level_Link_Loss_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Link_Loss_Alert_Level_Link_Loss_enum, "Link_Loss_Alert_Level", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Level";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Link_Loss_Alert_Level = parseResult.ValueList.GetValue("Level").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Link_Loss_Alert_Level
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLink_Loss_Alert_Level(byte Level)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Link_Loss_Alert_Level_Link_Loss_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  Level);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Link_Loss_Alert_Level_Link_Loss_enum, "Link_Loss_Alert_Level", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Link_Loss_Alert_Level_Link_Loss_enum, "Link_Loss_Alert_Level", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Immediate_Alert_Level = 0;
        private bool _Immediate_Alert_Level_set = false;
        public double Immediate_Alert_Level
        {
            get { return _Immediate_Alert_Level; }
            internal set { if (_Immediate_Alert_Level_set && value == _Immediate_Alert_Level) return; _Immediate_Alert_Level = value; _Immediate_Alert_Level_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadImmediate_Alert_Level(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Immediate_Alert_Level_Immediate_Alert_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Immediate_Alert_Level_Immediate_Alert_enum, "Immediate_Alert_Level", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|DEC|Level";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Immediate_Alert_Level = parseResult.ValueList.GetValue("Level").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Immediate_Alert_Level
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteImmediate_Alert_Level(byte Level)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Immediate_Alert_Level_Immediate_Alert_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  Level);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Immediate_Alert_Level_Immediate_Alert_enum, "Immediate_Alert_Level", command, GattWriteOption.WriteWithResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Immediate_Alert_Level_Immediate_Alert_enum, "Immediate_Alert_Level", subcommand, GattWriteOption.WriteWithResponse);
            }
        }


        private double _Transmit_Power = 0;
        private bool _Transmit_Power_set = false;
        public double Transmit_Power
        {
            get { return _Transmit_Power; }
            internal set { if (_Transmit_Power_set && value == _Transmit_Power) return; _Transmit_Power = value; _Transmit_Power_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTransmit_Power(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Transmit_Power_Transmit_Power_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Transmit_Power_Transmit_Power_enum, "Transmit_Power", cacheMode);
            if (result == null) return null;

            var datameaning = "I8|DEC|Power|db";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Transmit_Power = parseResult.ValueList.GetValue("Power").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Accelerometer_Config = null;
        private bool _Accelerometer_Config_set = false;
        public string Accelerometer_Config
        {
            get { return _Accelerometer_Config; }
            internal set { if (_Accelerometer_Config_set && value == _Accelerometer_Config) return; _Accelerometer_Config = value; _Accelerometer_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Config_Accelerometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Accelerometer_Config_Accelerometer_enum, "Accelerometer_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Accelerometer_Config = parseResult.ValueList.GetValue("Unknown0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Accelerometer_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccelerometer_Config(byte[] Unknown0)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Config_Accelerometer_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Config_Accelerometer_enum, "Accelerometer_Config", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Config_Accelerometer_enum, "Accelerometer_Config", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private double _Accelerometer_Data_X = 0;
        private bool _Accelerometer_Data_X_set = false;
        public double Accelerometer_Data_X
        {
            get { return _Accelerometer_Data_X; }
            internal set { if (_Accelerometer_Data_X_set && value == _Accelerometer_Data_X) return; _Accelerometer_Data_X = value; _Accelerometer_Data_X_set = true; OnPropertyChanged(); }
        }
        private double _Accelerometer_Data_Y = 0;
        private bool _Accelerometer_Data_Y_set = false;
        public double Accelerometer_Data_Y
        {
            get { return _Accelerometer_Data_Y; }
            internal set { if (_Accelerometer_Data_Y_set && value == _Accelerometer_Data_Y) return; _Accelerometer_Data_Y = value; _Accelerometer_Data_Y_set = true; OnPropertyChanged(); }
        }
        private double _Accelerometer_Data_Z = 0;
        private bool _Accelerometer_Data_Z_set = false;
        public double Accelerometer_Data_Z
        {
            get { return _Accelerometer_Data_Z; }
            internal set { if (_Accelerometer_Data_Z_set && value == _Accelerometer_Data_Z) return; _Accelerometer_Data_Z = value; _Accelerometer_Data_Z_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum, "Accelerometer_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "I16|DEC|X|mg I16|DEC|Y|mg I16|DEC|Z|mg";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Accelerometer_Data_X = parseResult.ValueList.GetValue("X").AsDouble;
            Accelerometer_Data_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Accelerometer_Data_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Accelerometer_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Accelerometer_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyAccelerometer_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyAccelerometer_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Accelerometer_Data_Accelerometer_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyAccelerometer_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyAccelerometer_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyAccelerometer_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyAccelerometer_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyAccelerometer_Data: set notification", result);

            return true;
        }

        private void NotifyAccelerometer_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "I16|DEC|X|mg I16|DEC|Y|mg I16|DEC|Z|mg";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Accelerometer_Data_X = parseResult.ValueList.GetValue("X").AsDouble;
            Accelerometer_Data_Y = parseResult.ValueList.GetValue("Y").AsDouble;
            Accelerometer_Data_Z = parseResult.ValueList.GetValue("Z").AsDouble;

            Accelerometer_DataEvent?.Invoke(parseResult);

        }

        public void NotifyAccelerometer_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[16];
            if (ch == null) return;
            NotifyAccelerometer_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyAccelerometer_DataCallback;
        }



        private string _Accelerometer_Alert = null;
        private bool _Accelerometer_Alert_set = false;
        public string Accelerometer_Alert
        {
            get { return _Accelerometer_Alert; }
            internal set { if (_Accelerometer_Alert_set && value == _Accelerometer_Alert) return; _Accelerometer_Alert = value; _Accelerometer_Alert_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadAccelerometer_Alert(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Alert_Accelerometer_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Accelerometer_Alert_Accelerometer_enum, "Accelerometer_Alert", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Accelerometer_Alert = parseResult.ValueList.GetValue("Unknown2").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Accelerometer_AlertEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Accelerometer_AlertEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyAccelerometer_Alert_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyAccelerometer_AlertAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Alert_Accelerometer_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Accelerometer_Alert_Accelerometer_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyAccelerometer_Alert_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyAccelerometer_Alert_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyAccelerometer_AlertCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyAccelerometer_Alert: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyAccelerometer_Alert: set notification", result);

            return true;
        }

        private void NotifyAccelerometer_AlertCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Accelerometer_Alert = parseResult.ValueList.GetValue("Unknown2").AsString;

            Accelerometer_AlertEvent?.Invoke(parseResult);

        }

        public void NotifyAccelerometer_AlertRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[17];
            if (ch == null) return;
            NotifyAccelerometer_Alert_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyAccelerometer_AlertCallback;
        }

        /// <summary>
        /// Writes data for Accelerometer_Alert
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteAccelerometer_Alert(byte[] Unknown2)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Accelerometer_Alert_Accelerometer_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown2);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Alert_Accelerometer_enum, "Accelerometer_Alert", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Accelerometer_Alert_Accelerometer_enum, "Accelerometer_Alert", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private double _Light_Config_Enable = 0;
        private bool _Light_Config_Enable_set = false;
        public double Light_Config_Enable
        {
            get { return _Light_Config_Enable; }
            internal set { if (_Light_Config_Enable_set && value == _Light_Config_Enable) return; _Light_Config_Enable = value; _Light_Config_Enable_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_ModeFlags = 0;
        private bool _Light_Config_ModeFlags_set = false;
        public double Light_Config_ModeFlags
        {
            get { return _Light_Config_ModeFlags; }
            internal set { if (_Light_Config_ModeFlags_set && value == _Light_Config_ModeFlags) return; _Light_Config_ModeFlags = value; _Light_Config_ModeFlags_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_DataRate = 0;
        private bool _Light_Config_DataRate_set = false;
        public double Light_Config_DataRate
        {
            get { return _Light_Config_DataRate; }
            internal set { if (_Light_Config_DataRate_set && value == _Light_Config_DataRate) return; _Light_Config_DataRate = value; _Light_Config_DataRate_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_NotiRate = 0;
        private bool _Light_Config_NotiRate_set = false;
        public double Light_Config_NotiRate
        {
            get { return _Light_Config_NotiRate; }
            internal set { if (_Light_Config_NotiRate_set && value == _Light_Config_NotiRate) return; _Light_Config_NotiRate = value; _Light_Config_NotiRate_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_AlertLog = 0;
        private bool _Light_Config_AlertLog_set = false;
        public double Light_Config_AlertLog
        {
            get { return _Light_Config_AlertLog; }
            internal set { if (_Light_Config_AlertLog_set && value == _Light_Config_AlertLog) return; _Light_Config_AlertLog = value; _Light_Config_AlertLog_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_AlertHi = 0;
        private bool _Light_Config_AlertHi_set = false;
        public double Light_Config_AlertHi
        {
            get { return _Light_Config_AlertHi; }
            internal set { if (_Light_Config_AlertHi_set && value == _Light_Config_AlertHi) return; _Light_Config_AlertHi = value; _Light_Config_AlertHi_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_AlertFaults = 0;
        private bool _Light_Config_AlertFaults_set = false;
        public double Light_Config_AlertFaults
        {
            get { return _Light_Config_AlertFaults; }
            internal set { if (_Light_Config_AlertFaults_set && value == _Light_Config_AlertFaults) return; _Light_Config_AlertFaults = value; _Light_Config_AlertFaults_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_Reserved = 0;
        private bool _Light_Config_Reserved_set = false;
        public double Light_Config_Reserved
        {
            get { return _Light_Config_Reserved; }
            internal set { if (_Light_Config_Reserved_set && value == _Light_Config_Reserved) return; _Light_Config_Reserved = value; _Light_Config_Reserved_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_Range = 0;
        private bool _Light_Config_Range_set = false;
        public double Light_Config_Range
        {
            get { return _Light_Config_Range; }
            internal set { if (_Light_Config_Range_set && value == _Light_Config_Range) return; _Light_Config_Range = value; _Light_Config_Range_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_Resolution = 0;
        private bool _Light_Config_Resolution_set = false;
        public double Light_Config_Resolution
        {
            get { return _Light_Config_Resolution; }
            internal set { if (_Light_Config_Resolution_set && value == _Light_Config_Resolution) return; _Light_Config_Resolution = value; _Light_Config_Resolution_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_AlertResetCount = 0;
        private bool _Light_Config_AlertResetCount_set = false;
        public double Light_Config_AlertResetCount
        {
            get { return _Light_Config_AlertResetCount; }
            internal set { if (_Light_Config_AlertResetCount_set && value == _Light_Config_AlertResetCount) return; _Light_Config_AlertResetCount = value; _Light_Config_AlertResetCount_set = true; OnPropertyChanged(); }
        }
        private double _Light_Config_AertResetDiff = 0;
        private bool _Light_Config_AertResetDiff_set = false;
        public double Light_Config_AertResetDiff
        {
            get { return _Light_Config_AertResetDiff; }
            internal set { if (_Light_Config_AertResetDiff_set && value == _Light_Config_AertResetDiff) return; _Light_Config_AertResetDiff = value; _Light_Config_AertResetDiff_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadLight_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Light_Config_Light_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Light_Config_Light_enum, "Light_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "U8|HEX|Enable U8|HEX|ModeFlags U16|DEC|DataRate U16|DEC|NotiRate U16|DEC|AlertLog U16|DEC|AlertHi U16|DEC|AlertFaults U16|DEC|Reserved U8|DEC|Range U8|DEC|Resolution U16|DEC|AlertResetCount U16|DC|AertResetDiff";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Light_Config_Enable = parseResult.ValueList.GetValue("Enable").AsDouble;
            Light_Config_ModeFlags = parseResult.ValueList.GetValue("ModeFlags").AsDouble;
            Light_Config_DataRate = parseResult.ValueList.GetValue("DataRate").AsDouble;
            Light_Config_NotiRate = parseResult.ValueList.GetValue("NotiRate").AsDouble;
            Light_Config_AlertLog = parseResult.ValueList.GetValue("AlertLog").AsDouble;
            Light_Config_AlertHi = parseResult.ValueList.GetValue("AlertHi").AsDouble;
            Light_Config_AlertFaults = parseResult.ValueList.GetValue("AlertFaults").AsDouble;
            Light_Config_Reserved = parseResult.ValueList.GetValue("Reserved").AsDouble;
            Light_Config_Range = parseResult.ValueList.GetValue("Range").AsDouble;
            Light_Config_Resolution = parseResult.ValueList.GetValue("Resolution").AsDouble;
            Light_Config_AlertResetCount = parseResult.ValueList.GetValue("AlertResetCount").AsDouble;
            Light_Config_AertResetDiff = parseResult.ValueList.GetValue("AertResetDiff").AsDouble;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Light_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteLight_Config(byte Enable, byte ModeFlags, UInt16 DataRate, UInt16 NotiRate, UInt16 AlertLog, UInt16 AlertHi, UInt16 AlertFaults, UInt16 Reserved, byte Range, byte Resolution, UInt16 AlertResetCount, UInt16 AertResetDiff)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Light_Config_Light_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  Enable);
            dw.WriteByte(  ModeFlags);
            dw.WriteUInt16(  DataRate);
            dw.WriteUInt16(  NotiRate);
            dw.WriteUInt16(  AlertLog);
            dw.WriteUInt16(  AlertHi);
            dw.WriteUInt16(  AlertFaults);
            dw.WriteUInt16(  Reserved);
            dw.WriteByte(  Range);
            dw.WriteByte(  Resolution);
            dw.WriteUInt16(  AlertResetCount);
            dw.WriteUInt16(  AertResetDiff);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Light_Config_Light_enum, "Light_Config", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Light_Config_Light_enum, "Light_Config", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Unknown1 = null;
        private bool _Unknown1_set = false;
        public string Unknown1
        {
            get { return _Unknown1; }
            internal set { if (_Unknown1_set && value == _Unknown1) return; _Unknown1 = value; _Unknown1_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUnknown1(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown1_Light_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Unknown1_Light_enum, "Unknown1", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Unknown1 = parseResult.ValueList.GetValue("Unknown1").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Unknown1Event += _my function_
        /// </summary>
        public event BluetoothDataEvent Unknown1Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyUnknown1_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyUnknown1Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown1_Light_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Unknown1_Light_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyUnknown1_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyUnknown1_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyUnknown1Callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyUnknown1: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyUnknown1: set notification", result);

            return true;
        }

        private void NotifyUnknown1Callback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Unknown1 = parseResult.ValueList.GetValue("Unknown1").AsString;

            Unknown1Event?.Invoke(parseResult);

        }

        public void NotifyUnknown1RemoveCharacteristicCallback() 
        {
            var ch = Characteristics[19];
            if (ch == null) return;
            NotifyUnknown1_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyUnknown1Callback;
        }



        private string _Unknown2 = null;
        private bool _Unknown2_set = false;
        public string Unknown2
        {
            get { return _Unknown2; }
            internal set { if (_Unknown2_set && value == _Unknown2) return; _Unknown2 = value; _Unknown2_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUnknown2(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown2_Light_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Unknown2_Light_enum, "Unknown2", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Unknown2 = parseResult.ValueList.GetValue("Unknown2").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Unknown2Event += _my function_
        /// </summary>
        public event BluetoothDataEvent Unknown2Event = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyUnknown2_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyUnknown2Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown2_Light_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Unknown2_Light_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyUnknown2_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyUnknown2_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyUnknown2Callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyUnknown2: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyUnknown2: set notification", result);

            return true;
        }

        private void NotifyUnknown2Callback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Unknown2 = parseResult.ValueList.GetValue("Unknown2").AsString;

            Unknown2Event?.Invoke(parseResult);

        }

        public void NotifyUnknown2RemoveCharacteristicCallback() 
        {
            var ch = Characteristics[20];
            if (ch == null) return;
            NotifyUnknown2_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyUnknown2Callback;
        }

        /// <summary>
        /// Writes data for Unknown2
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUnknown2(byte[] Unknown2)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown2_Light_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown2);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Unknown2_Light_enum, "Unknown2", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Unknown2_Light_enum, "Unknown2", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Unknown3 = null;
        private bool _Unknown3_set = false;
        public string Unknown3
        {
            get { return _Unknown3; }
            internal set { if (_Unknown3_set && value == _Unknown3) return; _Unknown3 = value; _Unknown3_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUnknown3(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown3_Light_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Unknown3_Light_enum, "Unknown3", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown3";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Unknown3 = parseResult.ValueList.GetValue("Unknown3").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Unknown3
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUnknown3(byte[] Unknown3)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Unknown3_Light_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown3);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Unknown3_Light_enum, "Unknown3", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Unknown3_Light_enum, "Unknown3", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Temperature_Config = null;
        private bool _Temperature_Config_set = false;
        public string Temperature_Config
        {
            get { return _Temperature_Config; }
            internal set { if (_Temperature_Config_set && value == _Temperature_Config) return; _Temperature_Config = value; _Temperature_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperature_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Config_Temperature_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Temperature_Config_Temperature_enum, "Temperature_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Temperature_Config = parseResult.ValueList.GetValue("Unknown0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Temperature_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTemperature_Config(byte[] Unknown0)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Config_Temperature_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Temperature_Config_Temperature_enum, "Temperature_Config", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Temperature_Config_Temperature_enum, "Temperature_Config", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Temperature_Data = null;
        private bool _Temperature_Data_set = false;
        public string Temperature_Data
        {
            get { return _Temperature_Data; }
            internal set { if (_Temperature_Data_set && value == _Temperature_Data) return; _Temperature_Data = value; _Temperature_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperature_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Data_Temperature_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Temperature_Data_Temperature_enum, "Temperature_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Temperature_Data = parseResult.ValueList.GetValue("Unknown1").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Temperature_DataEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Temperature_DataEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyTemperature_Data_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyTemperature_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Data_Temperature_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Temperature_Data_Temperature_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTemperature_Data_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTemperature_Data_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyTemperature_DataCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyTemperature_Data: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyTemperature_Data: set notification", result);

            return true;
        }

        private void NotifyTemperature_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Temperature_Data = parseResult.ValueList.GetValue("Unknown1").AsString;

            Temperature_DataEvent?.Invoke(parseResult);

        }

        public void NotifyTemperature_DataRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[23];
            if (ch == null) return;
            NotifyTemperature_Data_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyTemperature_DataCallback;
        }



        private string _Temperature_Alert = null;
        private bool _Temperature_Alert_set = false;
        public string Temperature_Alert
        {
            get { return _Temperature_Alert; }
            internal set { if (_Temperature_Alert_set && value == _Temperature_Alert) return; _Temperature_Alert = value; _Temperature_Alert_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperature_Alert(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Alert_Temperature_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Temperature_Alert_Temperature_enum, "Temperature_Alert", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Temperature_Alert = parseResult.ValueList.GetValue("Unknown2").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Temperature_AlertEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Temperature_AlertEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyTemperature_Alert_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyTemperature_AlertAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Alert_Temperature_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Temperature_Alert_Temperature_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyTemperature_Alert_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyTemperature_Alert_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyTemperature_AlertCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyTemperature_Alert: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyTemperature_Alert: set notification", result);

            return true;
        }

        private void NotifyTemperature_AlertCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Temperature_Alert = parseResult.ValueList.GetValue("Unknown2").AsString;

            Temperature_AlertEvent?.Invoke(parseResult);

        }

        public void NotifyTemperature_AlertRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[24];
            if (ch == null) return;
            NotifyTemperature_Alert_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyTemperature_AlertCallback;
        }

        /// <summary>
        /// Writes data for Temperature_Alert
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTemperature_Alert(byte[] Unknown2)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Alert_Temperature_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown2);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Temperature_Alert_Temperature_enum, "Temperature_Alert", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Temperature_Alert_Temperature_enum, "Temperature_Alert", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Temperature_Status = null;
        private bool _Temperature_Status_set = false;
        public string Temperature_Status
        {
            get { return _Temperature_Status; }
            internal set { if (_Temperature_Status_set && value == _Temperature_Status) return; _Temperature_Status = value; _Temperature_Status_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadTemperature_Status(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Status_Temperature_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Temperature_Status_Temperature_enum, "Temperature_Status", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown3";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Temperature_Status = parseResult.ValueList.GetValue("Unknown3").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Temperature_Status
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTemperature_Status(byte[] Unknown3)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Temperature_Status_Temperature_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown3);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Temperature_Status_Temperature_enum, "Temperature_Status", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Temperature_Status_Temperature_enum, "Temperature_Status", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Pairing_Control_Status = null;
        private bool _Pairing_Control_Status_set = false;
        public string Pairing_Control_Status
        {
            get { return _Pairing_Control_Status; }
            internal set { if (_Pairing_Control_Status_set && value == _Pairing_Control_Status) return; _Pairing_Control_Status = value; _Pairing_Control_Status_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPairing_Control_Status(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_Control_Status_Pairing_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Pairing_Control_Status_Pairing_enum, "Pairing_Control_Status", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Pairing_Control_Status = parseResult.ValueList.GetValue("Unknown0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Pairing_Control_StatusEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Pairing_Control_StatusEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyPairing_Control_Status_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyPairing_Control_StatusAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_Control_Status_Pairing_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Pairing_Control_Status_Pairing_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyPairing_Control_Status_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyPairing_Control_Status_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyPairing_Control_StatusCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyPairing_Control_Status: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyPairing_Control_Status: set notification", result);

            return true;
        }

        private void NotifyPairing_Control_StatusCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Pairing_Control_Status = parseResult.ValueList.GetValue("Unknown0").AsString;

            Pairing_Control_StatusEvent?.Invoke(parseResult);

        }

        public void NotifyPairing_Control_StatusRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[26];
            if (ch == null) return;
            NotifyPairing_Control_Status_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyPairing_Control_StatusCallback;
        }

        /// <summary>
        /// Writes data for Pairing_Control_Status
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePairing_Control_Status(byte[] Unknown0)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_Control_Status_Pairing_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Pairing_Control_Status_Pairing_enum, "Pairing_Control_Status", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Pairing_Control_Status_Pairing_enum, "Pairing_Control_Status", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Pairing_Data = null;
        private bool _Pairing_Data_set = false;
        public string Pairing_Data
        {
            get { return _Pairing_Data; }
            internal set { if (_Pairing_Data_set && value == _Pairing_Data) return; _Pairing_Data = value; _Pairing_Data_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPairing_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_Data_Pairing_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Pairing_Data_Pairing_enum, "Pairing_Data", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Pairing_Data = parseResult.ValueList.GetValue("Unknown1").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Pairing_Data
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePairing_Data(byte[] Unknown1)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_Data_Pairing_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown1);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Pairing_Data_Pairing_enum, "Pairing_Data", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Pairing_Data_Pairing_enum, "Pairing_Data", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Pairing_Config = null;
        private bool _Pairing_Config_set = false;
        public string Pairing_Config
        {
            get { return _Pairing_Config; }
            internal set { if (_Pairing_Config_set && value == _Pairing_Config) return; _Pairing_Config = value; _Pairing_Config_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPairing_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_Config_Pairing_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Pairing_Config_Pairing_enum, "Pairing_Config", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Pairing_Config = parseResult.ValueList.GetValue("Unknown2").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Pairing_Config
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WritePairing_Config(byte[] Unknown2)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_Config_Pairing_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown2);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Pairing_Config_Pairing_enum, "Pairing_Config", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Pairing_Config_Pairing_enum, "Pairing_Config", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Pairing_AD_Key = null;
        private bool _Pairing_AD_Key_set = false;
        public string Pairing_AD_Key
        {
            get { return _Pairing_AD_Key; }
            internal set { if (_Pairing_AD_Key_set && value == _Pairing_AD_Key) return; _Pairing_AD_Key = value; _Pairing_AD_Key_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadPairing_AD_Key(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_AD_Key_Pairing_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Pairing_AD_Key_Pairing_enum, "Pairing_AD_Key", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown3";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Pairing_AD_Key = parseResult.ValueList.GetValue("Unknown3").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Pairing_AD_KeyEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Pairing_AD_KeyEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyPairing_AD_Key_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyPairing_AD_KeyAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Pairing_AD_Key_Pairing_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Pairing_AD_Key_Pairing_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyPairing_AD_Key_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyPairing_AD_Key_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyPairing_AD_KeyCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyPairing_AD_Key: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyPairing_AD_Key: set notification", result);

            return true;
        }

        private void NotifyPairing_AD_KeyCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown3";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Pairing_AD_Key = parseResult.ValueList.GetValue("Unknown3").AsString;

            Pairing_AD_KeyEvent?.Invoke(parseResult);

        }

        public void NotifyPairing_AD_KeyRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[29];
            if (ch == null) return;
            NotifyPairing_AD_Key_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyPairing_AD_KeyCallback;
        }



        private string _Utility_DeviceName = null;
        private bool _Utility_DeviceName_set = false;
        public string Utility_DeviceName
        {
            get { return _Utility_DeviceName; }
            internal set { if (_Utility_DeviceName_set && value == _Utility_DeviceName) return; _Utility_DeviceName = value; _Utility_DeviceName_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_DeviceName(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_DeviceName_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_DeviceName_BR_Utilities_enum, "Utility_DeviceName", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown0";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_DeviceName = parseResult.ValueList.GetValue("Unknown0").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Utility_DeviceName
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_DeviceName(byte[] Unknown0)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_DeviceName_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown0);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_DeviceName_BR_Utilities_enum, "Utility_DeviceName", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_DeviceName_BR_Utilities_enum, "Utility_DeviceName", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Utility_Default_Conn_Param = null;
        private bool _Utility_Default_Conn_Param_set = false;
        public string Utility_Default_Conn_Param
        {
            get { return _Utility_Default_Conn_Param; }
            internal set { if (_Utility_Default_Conn_Param_set && value == _Utility_Default_Conn_Param) return; _Utility_Default_Conn_Param = value; _Utility_Default_Conn_Param_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_Default_Conn_Param(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Default_Conn_Param_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_Default_Conn_Param_BR_Utilities_enum, "Utility_Default_Conn_Param", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown1";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_Default_Conn_Param = parseResult.ValueList.GetValue("Unknown1").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Utility_Default_Conn_Param
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_Default_Conn_Param(byte[] Unknown1)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Default_Conn_Param_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown1);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_Default_Conn_Param_BR_Utilities_enum, "Utility_Default_Conn_Param", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_Default_Conn_Param_BR_Utilities_enum, "Utility_Default_Conn_Param", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Utility_Curr_Conn_Param = null;
        private bool _Utility_Curr_Conn_Param_set = false;
        public string Utility_Curr_Conn_Param
        {
            get { return _Utility_Curr_Conn_Param; }
            internal set { if (_Utility_Curr_Conn_Param_set && value == _Utility_Curr_Conn_Param) return; _Utility_Curr_Conn_Param = value; _Utility_Curr_Conn_Param_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_Curr_Conn_Param(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Curr_Conn_Param_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_Curr_Conn_Param_BR_Utilities_enum, "Utility_Curr_Conn_Param", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_Curr_Conn_Param = parseResult.ValueList.GetValue("Unknown2").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; Utility_Curr_Conn_ParamEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent Utility_Curr_Conn_ParamEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyUtility_Curr_Conn_Param_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyUtility_Curr_Conn_ParamAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Curr_Conn_Param_BR_Utilities_enum)) return false;
            var ch = Characteristics[(int)CharacteristicsEnum.Utility_Curr_Conn_Param_BR_Utilities_enum];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyUtility_Curr_Conn_Param_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyUtility_Curr_Conn_Param_ValueChanged_Set = true;
                    ch.ValueChanged += NotifyUtility_Curr_Conn_ParamCallback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyUtility_Curr_Conn_Param: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyUtility_Curr_Conn_Param: set notification", result);

            return true;
        }

        private void NotifyUtility_Curr_Conn_ParamCallback(GattCharacteristic sender, GattValueChangedEventArgs args) 
        {
            var datameaning = "BYTES|HEX|Unknown2";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);
            Utility_Curr_Conn_Param = parseResult.ValueList.GetValue("Unknown2").AsString;

            Utility_Curr_Conn_ParamEvent?.Invoke(parseResult);

        }

        public void NotifyUtility_Curr_Conn_ParamRemoveCharacteristicCallback() 
        {
            var ch = Characteristics[32];
            if (ch == null) return;
            NotifyUtility_Curr_Conn_Param_ValueChanged_Set = false;
            ch.ValueChanged -= NotifyUtility_Curr_Conn_ParamCallback;
        }

        /// <summary>
        /// Writes data for Utility_Curr_Conn_Param
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_Curr_Conn_Param(byte[] Unknown2)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Curr_Conn_Param_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown2);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_Curr_Conn_Param_BR_Utilities_enum, "Utility_Curr_Conn_Param", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_Curr_Conn_Param_BR_Utilities_enum, "Utility_Curr_Conn_Param", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Utility_RF_Power = null;
        private bool _Utility_RF_Power_set = false;
        public string Utility_RF_Power
        {
            get { return _Utility_RF_Power; }
            internal set { if (_Utility_RF_Power_set && value == _Utility_RF_Power) return; _Utility_RF_Power = value; _Utility_RF_Power_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_RF_Power(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_RF_Power_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_RF_Power_BR_Utilities_enum, "Utility_RF_Power", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown3";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_RF_Power = parseResult.ValueList.GetValue("Unknown3").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Utility_RF_Power
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_RF_Power(byte[] Unknown3)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_RF_Power_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown3);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_RF_Power_BR_Utilities_enum, "Utility_RF_Power", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_RF_Power_BR_Utilities_enum, "Utility_RF_Power", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }





        /// <summary>
        /// Writes data for Utility_Disconnect
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_Disconnect(byte[] Unknown4)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Disconnect_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown4);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_Disconnect_BR_Utilities_enum, "Utility_Disconnect", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_Disconnect_BR_Utilities_enum, "Utility_Disconnect", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Utility_Public_Address = null;
        private bool _Utility_Public_Address_set = false;
        public string Utility_Public_Address
        {
            get { return _Utility_Public_Address; }
            internal set { if (_Utility_Public_Address_set && value == _Utility_Public_Address) return; _Utility_Public_Address = value; _Utility_Public_Address_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_Public_Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Public_Address_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_Public_Address_BR_Utilities_enum, "Utility_Public_Address", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown5";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_Public_Address = parseResult.ValueList.GetValue("Unknown5").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Utility_Config_Counter = null;
        private bool _Utility_Config_Counter_set = false;
        public string Utility_Config_Counter
        {
            get { return _Utility_Config_Counter; }
            internal set { if (_Utility_Config_Counter_set && value == _Utility_Config_Counter) return; _Utility_Config_Counter = value; _Utility_Config_Counter_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_Config_Counter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Config_Counter_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_Config_Counter_BR_Utilities_enum, "Utility_Config_Counter", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown6";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_Config_Counter = parseResult.ValueList.GetValue("Unknown6").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }




        private string _Utility_Advertising_Param = null;
        private bool _Utility_Advertising_Param_set = false;
        public string Utility_Advertising_Param
        {
            get { return _Utility_Advertising_Param; }
            internal set { if (_Utility_Advertising_Param_set && value == _Utility_Advertising_Param) return; _Utility_Advertising_Param = value; _Utility_Advertising_Param_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_Advertising_Param(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Advertising_Param_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_Advertising_Param_BR_Utilities_enum, "Utility_Advertising_Param", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown7";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_Advertising_Param = parseResult.ValueList.GetValue("Unknown7").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Utility_Advertising_Param
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_Advertising_Param(byte[] Unknown7)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Advertising_Param_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown7);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_Advertising_Param_BR_Utilities_enum, "Utility_Advertising_Param", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_Advertising_Param_BR_Utilities_enum, "Utility_Advertising_Param", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }


        private string _Utility_Unknown = null;
        private bool _Utility_Unknown_set = false;
        public string Utility_Unknown
        {
            get { return _Utility_Unknown; }
            internal set { if (_Utility_Unknown_set && value == _Utility_Unknown) return; _Utility_Unknown = value; _Utility_Unknown_set = true; OnPropertyChanged(); }
        }

        /// <summary>
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>BCValueList of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<BCBasic.BCValueList> ReadUtility_Unknown(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Unknown_BR_Utilities_enum)) return null;
            IBuffer result = await ReadAsync(CharacteristicsEnum.Utility_Unknown_BR_Utilities_enum, "Utility_Unknown", cacheMode);
            if (result == null) return null;

            var datameaning = "BYTES|HEX|Unknown8";
            var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(result, datameaning);
            Utility_Unknown = parseResult.ValueList.GetValue("Unknown8").AsString;

            // Hint: get the data that's been read with e.g. 
            // var value = parseResult.ValueList.GetValue("LightRaw").AsDouble;
            return parseResult.ValueList;
        }


        /// <summary>
        /// Writes data for Utility_Unknown
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_Unknown(byte[] Unknown8)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Unknown_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteBytes(  Unknown8);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_Unknown_BR_Utilities_enum, "Utility_Unknown", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_Unknown_BR_Utilities_enum, "Utility_Unknown", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }





        /// <summary>
        /// Writes data for Utility_Blink_LED
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteUtility_Blink_LED(byte LEDs, byte NBlink, byte PercentOn, UInt16 Period)
        {
            if (!await EnsureCharacteristicAsync(CharacteristicsEnum.Utility_Blink_LED_BR_Utilities_enum)) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteByte(  LEDs);
            dw.WriteByte(  NBlink);
            dw.WriteByte(  PercentOn);
            dw.WriteUInt16(  Period);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            if (command.Length <= MAXBYTES) //TODO: make sure this works
            {
                await WriteCommandAsync(CharacteristicsEnum.Utility_Blink_LED_BR_Utilities_enum, "Utility_Blink_LED", command, GattWriteOption.WriteWithoutResponse);
            }
            else for (int i=0; i<command.Length; i+= MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(CharacteristicsEnum.Utility_Blink_LED_BR_Utilities_enum, "Utility_Blink_LED", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }



    }
}